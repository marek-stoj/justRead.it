using ImmRafSoft.Validation;

namespace JustReadIt.WebApp.Core.Models.Account
{
  public class SignInViewModel
  {
    private string _emailAddress;

    #region Properties

    [RequiredValidator(Tag = ValidationTags.SignInViewModel_EmailAddressIsRequired)]
    [StringLengthValidatorEx(1, 256, Tag = ValidationTags.SignInViewModel_EmailAddressIsTooLong)]
    public string EmailAddress
    {
      get { return _emailAddress; }
      set { _emailAddress = value != null ? value.ToLower() : null; }
    }

    [RequiredValidator(Tag = ValidationTags.SignInViewModel_PasswordIsRequired)]
    [StringLengthValidatorEx(1, 32, Tag = ValidationTags.SignInViewModel_PasswordIsTooLong)]
    public string Password { get; set; }

    public bool SignInFailed { get; set; }

    #endregion
  }
}
