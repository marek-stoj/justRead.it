using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.Subscriptions {

  public class CreateInputModel {

    private string _feedUrl;

    public string feed_url {
      get { return _feedUrl; }
      set { _feedUrl = value.TrimmedOrNull(); }
    }

  }

}
