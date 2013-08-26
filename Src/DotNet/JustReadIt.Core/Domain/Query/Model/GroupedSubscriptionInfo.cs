using JustReadIt.Core.Common;

namespace JustReadIt.Core.Domain.Query.Model {

  public class GroupedSubscriptionInfo {

    private string _title;
    private string _siteUrl;

    public int Id { get; set; }

    public int FeedId { get; set; }

    public string Title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

    public string SiteUrl {
      get { return _siteUrl; }
      set { _siteUrl = value.TrimmedOrNull(); }
    }

    public int UnreadItemsCount { get; set; }

  }

}
