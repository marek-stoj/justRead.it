using JustReadIt.Core.Common;

namespace JustReadIt.Core.Domain.Query.Model {

  public class GroupedSubscription {

    private string _groupTitle;

    public int GroupId { get; set; }

    public string GroupTitle
    {
      get { return _groupTitle; }
      set { _groupTitle = value.TrimmedOrNull(); }
    }

    public GroupedSubscriptionInfo SubscriptionInfo { get; set; }

  }
}
