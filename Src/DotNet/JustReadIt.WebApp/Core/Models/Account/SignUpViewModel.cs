using ImmRafSoft.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace JustReadIt.WebApp.Core.Models.Account {

  [HasSelfValidation]
  public class SignUpViewModel {

    private string _emailAddress;

    [SelfValidation]
    public void SelfValidate(ValidationResults validationResults) {
      if (!string.IsNullOrEmpty(Password)
          && !string.IsNullOrEmpty(PasswordConfirmation)
          && Password != PasswordConfirmation) {
        validationResults.AddResult(
          new ValidationResult(
            "Password and password confirmation are different.",
            this,
            "PasswordConfirmation",
            ValidationTags.SignUpViewModel_PasswordAndPasswordConfirmationAreDifferent,
            null));
      }
    }

    [RequiredValidator(Tag = ValidationTags.SignUpViewModel_EmailAddressIsRequired)]
    [StringLengthValidatorEx(1, 64, Tag = ValidationTags.SignUpViewModel_EmailAddressIsTooLong)]
    public string EmailAddress {
      get { return _emailAddress; }
      set { _emailAddress = value != null ? value.ToLower() : null; }
    }

    [RequiredValidator(Tag = ValidationTags.SignUpViewModel_PasswordIsRequired)]
    [StringLengthValidatorEx(1, 32, Tag = ValidationTags.SignUpViewModel_PasswordIsTooLong)]
    public string Password { get; set; }

    [RequiredValidator(Tag = ValidationTags.SignUpViewModel_PasswordConfirmationIsRequired)]
    [StringLengthValidatorEx(1, 32, Tag = ValidationTags.SignUpViewModel_PasswordConfirmationIsTooLong)]
    public string PasswordConfirmation { get; set; }

    public bool SignUpFailed { get; set; }

    public string SignUpFailedMessage { get; set; }

  }

}
