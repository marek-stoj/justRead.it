using JustReadIt.Core.Common;

namespace JustReadIt.Core.Domain.Query.Model {

  public class GroupedSubscription {

    private string _title;
    private string _siteUrl;
    private string _groupTitle;

    public int Id { get; set; }

    public int FeedId { get; set; }

    public int GroupId { get; set; }

    public string GroupTitle {
      get { return _groupTitle; }
      set { _groupTitle = value.TrimmedOrNull(); }
    }

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
