using System;

namespace JustReadIt.Core.Domain
{
  public class EmailVerificationToken
  {
    private static readonly TimeSpan _TokenValidTimeSpan = new TimeSpan(14, 0, 0, 0);
    
    private DateTime _dateCreated;

    public int Id { get; set; }

    public int UserAccountId { get; set; }

    public DateTime DateCreated {
      get { return _dateCreated; }
      set { _dateCreated = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    public Guid Token { get; set; }

    public bool IsUsed { get; set; }

    public bool HasExpired
    {
      get { return (DateTime.UtcNow - DateCreated) > _TokenValidTimeSpan; }
    }
  }
}
