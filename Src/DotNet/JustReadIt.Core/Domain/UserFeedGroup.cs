namespace JustReadIt.Core.Domain {

  public class UserFeedGroup {

    public int Id { get; set; }

    public int UserAccountId { get; set; }

    public SpecialUserFeedGroupType? SpecialType { get; set; }

    public string Title { get; set; }

  }

}
