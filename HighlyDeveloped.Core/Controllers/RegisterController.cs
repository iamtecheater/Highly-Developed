using HighlyDeveloped.Core.Interfaces;
using HighlyDeveloped.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace HighlyDeveloped.Core.Controllers
{
    /// <summary>
    /// Handle member registration
    /// </summary>
    public class RegisterController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/";
        private IEmailService _emailService;

        public RegisterController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        #region Register Form
        /// <summary>
        /// Render the registration form
        /// </summary>
        /// <returns></returns>
        public ActionResult RenderRegister()
        {
            var vm = new RegisterViewModel();
            return PartialView(PARTIAL_VIEW_FOLDER + "Register.cshtml", vm);
        }

        /// <summary>
        /// Handle the registration form post
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleRegister(RegisterViewModel vm)
        {
            //If form not valid - return
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            //Check if there is already a member with that email address
            var existingMember = Services.MemberService.GetByEmail(vm.EmailAddress);

            if(existingMember != null)
            {
                ModelState.AddModelError("Account Error", "There's already a user with that email address.");
                return CurrentUmbracoPage();
            }

            //Check if their username is already in use
            existingMember = Services.MemberService.GetByUsername(vm.Username);

            if(existingMember != null)
            {
                ModelState.AddModelError("Account Error", "There's already a user with that username. Please choose a different one.");
                return CurrentUmbracoPage();
            }

            //Create "member" in Umbraco with the details
            var newMember = Services.MemberService
                                .CreateMember(vm.Username, vm.EmailAddress, $"{vm.FirstName} {vm.LastName}", "Member");
            newMember.PasswordQuestion = "";
            newMember.RawPasswordAnswerValue = "";
            //Need to save the member before you can set the password
            Services.MemberService.Save(newMember);
            Services.MemberService.SavePassword(newMember, vm.Password);
            //Assign a role - i.e. Normal User
            Services.MemberService.AssignRole(newMember.Id, "Normal User");

            //Create email verification token
            //Token creation
            var token = Guid.NewGuid().ToString();
            newMember.SetValue("emailVerifyToken", token);
            Services.MemberService.Save(newMember);

            //Send email verification
            _emailService.SendVerifyEmailAddressNotification(newMember.Email, token);


            //Thank the user
            //Return confirmation message to user
            TempData["status"] = "OK";

            return RedirectToCurrentUmbracoPage();
        }
        #endregion

        #region Verification
        public ActionResult RenderEmailVerification(string token)
        {
            //Get token (querystring)
            //Look for a member matching this token
            var member = Services.MemberService.GetMembersByPropertyValue("emailVerifyToken", token).SingleOrDefault();

            if(member != null)
            {
                //If we find one, set them to verified
                var alreadyVerified = member.GetValue<bool>("emailVerified");
                if (alreadyVerified)
                {
                    ModelState.AddModelError("Verified", "You've already verified your email address thanks!");
                    return CurrentUmbracoPage();
                }
                member.SetValue("emailVerified", true);
                member.SetValue("emailVerifiedDate", DateTime.Now);
                Services.MemberService.Save(member);

                //Thank the user
                TempData["status"] = "OK";
                return PartialView(PARTIAL_VIEW_FOLDER + "EmailVerification.cshtml");
            }

            //Otherwise... some problem
            ModelState.AddModelError("Error", "Apologies there has been some problem!");
            return PartialView(PARTIAL_VIEW_FOLDER + "EmailVerification.cshtml");
        }
        #endregion
    }
}
