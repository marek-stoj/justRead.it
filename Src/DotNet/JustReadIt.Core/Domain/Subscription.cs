using System;
using JustReadIt.Core.Common;

namespace JustReadIt.Core.Domain {

  public class Subscription {

    private string _customTitle;

    public int Id { get; set; }

    public int UserAccountId { get; set; }

    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Can be null.
    /// </summary>
    public string CustomTitle {
      get { return _customTitle; }
      set { _customTitle = value.TrimmedOrNull(); }
    }

    public Feed Feed { get; set; }

    public string Title {
      get {
        return
          !string.IsNullOrEmpty(_customTitle)
            ? _customTitle
            : Feed != null
                ? Feed.Title
                : null;
      }
    }

  }

}
