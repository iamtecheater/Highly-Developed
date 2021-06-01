The credentials for access to the back office are:

admin@highlydeveloped.net
djkfk32$3lddep2


Sort out the web.config so it works in your environment.

Amend the temporary mail directory for the email service - or convert ot use smtp.

<smtp deliveryMethod="SpecifiedPickupDirectory" from="highlyd3veloped@gmail.com">
    <specifiedPickupDirectory pickupDirectoryLocation="c:\temp\mail" />
</smtp>