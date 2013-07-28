using System;

namespace JustReadIt.Core.Domain {

  public class Subscription {

    public int Id { get; set; }

    public int UserAccountId { get; set; }

    public DateTime DateCreated { get; set; }

    public Feed Feed { get; set; }

  }

}
