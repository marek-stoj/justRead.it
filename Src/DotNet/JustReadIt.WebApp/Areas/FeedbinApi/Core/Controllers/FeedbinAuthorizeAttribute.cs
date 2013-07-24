using JustReadIt.Core.Common;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Core.App;
using JustReadIt.WebApp.Core.WebApiEx;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Controllers {

  public class FeedbinAuthorizeAttribute : BasicAuthorizeAttribute {

    private const string _BasicAuthRealm = "JustReadIt - Feedbin API";

    private readonly IMembershipService _membershipService;

    public FeedbinAuthorizeAttribute(IMembershipService membershipService)
      : base(_BasicAuthRealm) {
      Guard.ArgNotNull(membershipService, "membershipService");

      _membershipService = membershipService;
    }

    public FeedbinAuthorizeAttribute()
      : this(IoC.CreateMembershipService()) {
    }

    protected override bool AreCredentialsValid(string username, string password) {
      Guard.ArgNotNullNorEmpty(username, "username");
      Guard.ArgNotNullNorEmpty(password, "password");

      return _membershipService.ValidateUser(username, password);
    }

  }

}
