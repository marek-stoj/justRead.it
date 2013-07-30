using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Models.Subscriptions {

  public class UpdateInputModel {

    private string _title;

    public string title {
      get { return _title; }
      set { _title = value.TrimmedOrNull(); }
    }

  }

}
