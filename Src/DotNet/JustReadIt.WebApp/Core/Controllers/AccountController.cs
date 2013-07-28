using System;
using System.Web.Mvc;
using JustReadIt.Core.Common;
using JustReadIt.Core.Domain.Repositories;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Core.App;
using JustReadIt.WebApp.Core.Common;
using JustReadIt.WebApp.Core.Models.Account;
using JustReadIt.WebApp.Core.MvcEx;
using JustReadIt.WebApp.Core.Resources;
using IAuthenticationService = JustReadIt.WebApp.Core.Services.IAuthenticationService;

namespace JustReadIt.WebApp.Core.Controllers {

  public class AccountController : JustReadItController {

    private readonly IAuthenticationService _authenticationService;
    private readonly IMembershipService _membershipService;
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;

    public AccountController(IAuthenticationService authenticationService, IMembershipService membershipService, IEmailVerificationTokenRepository emailVerificationTokenRepository) {
      Guard.ArgNotNull(authenticationService, "authenticationService");
      Guard.ArgNotNull(membershipService, "membershipService");
      Guard.ArgNotNull(emailVerificationTokenRepository, "emailVerificationTokenRepository");

      _authenticationService = authenticationService;
      _membershipService = membershipService;
      _emailVerificationTokenRepository = emailVerificationTokenRepository;
    }

    public AccountController()
      : this(
        IoC.CreateAuthenticationService(),
        IoC.CreateMembershipService(),
        IoC.CreateEmailVerificationTokenRepository()) {
    }

    [HttpGet]
    public ActionResult SignIn() {
      if (Request.IsAuthenticated) {
        return RedirectToHome();
      }

      var viewModel = new SignInViewModel();

      return View(viewModel);
    }

    [HttpPost]
    [SubmitFilter(typeof(Buttons), Buttons.Account_SignIn, true)]
    public ActionResult SignIn_Do(SignInViewModel signInViewModel) {
      if (!ModelValidator.Validate(signInViewModel, ModelState)) {
        return View("SignIn", signInViewModel);
      }

      int userAccountId;

      bool signInFailed =
        !_membershipService.ValidateUser(
          signInViewModel.EmailAddress,
          signInViewModel.Password,
          out userAccountId);

      if (!signInFailed) {
        // TODO IMM HI: pass user account id?
        _authenticationService.SignIn(signInViewModel.EmailAddress, true);

        return RedirectFromSignInPage();
      }

      return View(
        "SignIn",
        new SignInViewModel {
          EmailAddress = signInViewModel.EmailAddress,
          Password = null,
          SignInFailed = true,
        });
    }

    [HttpGet]
    public ActionResult SignUp() {
      if (Request.IsAuthenticated) {
        return RedirectToHome();
      }

      var viewModel = new SignUpViewModel();

      return View(viewModel);
    }

    [HttpPost]
    [SubmitFilter(typeof(Buttons), Buttons.Account_SignUp, true)]
    public ActionResult SignUp_Do(SignUpViewModel signUpViewModel) {
      if (!ModelValidator.Validate(signUpViewModel, ModelState)) {
        return View(
          "SignUp",
          signUpViewModel);
      }

      CreateUserResult createUserResult =
        _membershipService.CreateUser(
          signUpViewModel.EmailAddress,
          signUpViewModel.Password);

      switch (createUserResult) {
        case CreateUserResult.Success:
          return RedirectToAction(
            "SignUpSuccess",
            new { emailAddress = signUpViewModel.EmailAddress, });

        case CreateUserResult.Failed_EmailAddressExists:
          signUpViewModel.SignUpFailed = true;
          signUpViewModel.SignUpFailedMessage = AccountResources.Message_SignUpFailed_EmailAddressExists;

          return View("SignUp", signUpViewModel);

        default:
          throw new InternalException(string.Format("Unknown create user result: '{0}'.", createUserResult));
      }
    }

    [HttpGet]
    public ActionResult SignUpSuccess(string emailAddress) {
      if (string.IsNullOrEmpty(emailAddress)) {
        return BadRequest();
      }

      if (Request.IsAuthenticated) {
        return RedirectToHome();
      }

      var signUpSuccessViewModel =
        new SignUpSuccessViewModel {
          EmailAddress = emailAddress
        };

      return View(signUpSuccessViewModel);
    }

    [HttpGet]
    [Authorize]
    public ActionResult SignOut() {
      _authenticationService.SignOut();

      return RedirectToHome();
    }

    [HttpGet]
    public ActionResult VerifyEmail(Guid? token) {
      if (!token.HasValue) {
        return BadRequest();
      }

      int? userAccountId;

      if (_emailVerificationTokenRepository.IsTokenValid(token.Value, out userAccountId)
       && userAccountId.HasValue) {
        _membershipService.VerifyEmailAddress(userAccountId.Value, token.Value);

        return View("EmailVerificationTokenValid");
      }

      return View("EmailVerificationTokenInvalid");
    }

    private ActionResult RedirectFromSignInPage() {
      string returnUrl = Request.QueryString["ReturnUrl"];

      if (string.IsNullOrEmpty(returnUrl) || !IsPathOnSameServer(returnUrl)) {
        return RedirectToHome();
      }

      return Redirect(returnUrl);
    }

    private bool IsPathOnSameServer(string absUriOrLocalPath) {
      Uri currentRequestUri = Request.Url;

      if (currentRequestUri == null) {
        return false;
      }

      Uri uri;

      if (Uri.TryCreate(absUriOrLocalPath, UriKind.Absolute, out uri)
          && !uri.IsLoopback) {
        return
          string.Equals(
            currentRequestUri.Host,
            uri.Host,
            StringComparison.OrdinalIgnoreCase);
      }

      return true;
    }

  }

}
