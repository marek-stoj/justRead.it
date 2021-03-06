﻿using JustReadIt.Core.Common;
using JustReadIt.Core.Services;
using JustReadIt.WebApp.Core.WebApiEx;

namespace JustReadIt.WebApp.Areas.Feedbin.Core.Controllers {

  public class FeedbinAuthorizeAttribute : BasicAuthorizeAttribute {

    private const string _BasicAuthRealm = "JustReadIt - Feedbin API";

    private readonly IMembershipService _membershipService;

    public FeedbinAuthorizeAttribute(IMembershipService membershipService)
      : base(_BasicAuthRealm) {
      Guard.ArgNotNull(membershipService, "membershipService");

      _membershipService = membershipService;
    }

    public FeedbinAuthorizeAttribute()
      : this(CommonIoC.GetMembershipService()) {
    }

    protected override bool AreCredentialsValid(string username, string password, out int userAccountId) {
      Guard.ArgNotNullNorEmpty(username, "username");
      Guard.ArgNotNullNorEmpty(password, "password");

      return _membershipService.ValidateUser(username, password, out userAccountId);
    }

  }

}
