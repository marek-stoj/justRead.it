using System;

namespace JustReadIt.WebApp.Core.Mvc
{
  public class ActionLink
  {
    #region Constructor(s)

    public ActionLink(string actionName, string controllerName)
    {
      if (string.IsNullOrEmpty(actionName))
      {
        throw new ArgumentNullException("actionName");
      }

      if (string.IsNullOrEmpty(controllerName))
      {
        throw new ArgumentNullException("controllerName");
      }

      ActionName = actionName;
      ControllerName = controllerName;
    }

    #endregion

    #region Public static methods

    /// <summary>
    /// Parses given string (<paramref name="actionLinkStr"/>) into actionName and controllerName.
    /// </summary>
    public static ActionLink FromString(string actionLinkStr)
    {
      if (string.IsNullOrEmpty(actionLinkStr))
      {
        throw new ArgumentNullException("actionLinkStr");
      }

      const string invalidFormatMessage = "Format of the action link string is invalid. Should be: controllerName/actionName.";
      string[] splitted = actionLinkStr.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

      if (splitted == null || splitted.Length != 2)
      {
        throw new FormatException(invalidFormatMessage);
      }

      string actionName = splitted[1].Trim();
      string controllerName = splitted[0].Trim();

      if (actionName.Length == 0 || controllerName.Length == 0)
      {
        throw new FormatException(invalidFormatMessage);
      }

      return new ActionLink(actionName, controllerName);
    }

    #endregion

    #region Properties

    public string ActionName { get; set; }

    public string ControllerName { get; set; }

    #endregion
  }
}
