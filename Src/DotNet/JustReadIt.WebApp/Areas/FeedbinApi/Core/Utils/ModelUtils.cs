using System;
using System.Globalization;
using JustReadIt.Core.Common;

namespace JustReadIt.WebApp.Areas.FeedbinApi.Core.Utils {

  public static class ModelUtils {

    public static DateTime? ParseFeedbinDateTime(string dateTimeString) {
      Guard.ArgNotNullNorEmpty(dateTimeString, "dateTimeString");

      return
        DateTime.Parse(
          dateTimeString,
          CultureInfo.InvariantCulture,
          DateTimeStyles.RoundtripKind);
    }

  }

}
