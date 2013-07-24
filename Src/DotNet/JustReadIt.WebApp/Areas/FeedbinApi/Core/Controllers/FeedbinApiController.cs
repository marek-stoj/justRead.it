using System;
using System.Globalization;
using System.Web.Http;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Controllers {

  [FeedbinAuthorize]
  public abstract class FeedbinApiController : ApiController {

    protected static DateTime? ParseFeedbinDateTime(string dateTimeString) {
      return
        DateTime.Parse(
          dateTimeString,
          CultureInfo.InvariantCulture,
          DateTimeStyles.RoundtripKind);
    }

  }

}
