using System;

namespace JustReadIt.Core.Domain {

  public class Subscription {

    public int Id { get; set; }

    public DateTime DateCreated { get; set; }

    public Feed Feed { get; set; }

  }

}
