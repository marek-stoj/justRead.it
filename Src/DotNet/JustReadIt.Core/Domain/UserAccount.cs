using System;

namespace JustReadIt.Core.Domain {

  public class UserAccount {

    private DateTime _dateCreated;

    public int Id { get; set; }

    public DateTime DateCreated {
      get { return _dateCreated; }
      set { _dateCreated = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    public string EmailAddress { get; set; }

    public string PasswordHash { get; set; }

    public bool IsEmailVerified { get; set; }

  }

}
