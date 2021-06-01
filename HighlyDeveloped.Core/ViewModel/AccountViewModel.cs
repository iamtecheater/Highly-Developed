using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models;

namespace HighlyDeveloped.Core.ViewModel
{
    /// <summary>
    /// This is the view model for the my account page
    /// </summary>
    public class AccountViewModel
    {
        [DisplayName("Full Name")]
        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }
        [DisplayName("Full Name")]
        [Required(ErrorMessage = "Please enter your email address")]
        [Email(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        public string Username { get; set; }

        [UIHint("Password")]
        [DisplayName("Password")]
        [MinLength(10, ErrorMessage = "Please make your password at least 10 characters")]
        public string Password { get; set; }
        [UIHint("Confirm Password")]
        [DisplayName("Confirm Password")]
        [EqualTo("Password", ErrorMessage = "Please ensure your passwords match")]
        public string ConfirmPassword { get; set; }
    }
}
