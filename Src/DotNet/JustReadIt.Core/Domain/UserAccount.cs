using System;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Domain {

  public class UserAccount {

    private DateTime _dateCreated;
    private string _authProviderId;
    private string _emailAddress;
    private string _passwordHash;

    public int Id { get; set; }

    public DateTime DateCreated {
      get { return _dateCreated; }
      set { _dateCreated = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    public string EmailAddress {
      get { return _emailAddress; }
      set { _emailAddress = value.TrimmedOrNull(); }
    }

    public bool IsEmailAddressVerified { get; set; }

    /// <summary>
    /// Can be null.
    /// </summary>
    public string AuthProviderId {
      get { return _authProviderId; }
      set { _authProviderId = value.TrimmedOrNull(); }
    }

    /// <summary>
    /// Can be null.
    /// </summary>
    public string PasswordHash {
      get { return _passwordHash; }
      set { _passwordHash = value.TrimmedOrNull(); }
    }

  }

}
