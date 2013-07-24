using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace JustReadIt.WebApp.Core.MvcEx
{
  public sealed class SubmitFilterAttribute : ActionNameSelectorAttribute
  {
    private readonly Type _buttonsEnumType;
    private readonly object _submitButton;
    private readonly List<ActionLink> _actionLinks;
    private readonly bool _ignoreActionName;

    #region Constructor(s)

    /// <summary>
    /// Atrybut filtrujący akcję kontrolera wg. podanej kolekcji nazw akcji oraz przycisku html wywołującego akcję. 
    /// </summary>
    /// <param name="buttonsEnumType">Typ num dla przycisków.</param>
    /// <param name="actions">Lista nazw akcji pasująca do metody</param>
    /// <param name="submitButton">Przycisk wywołujący akcję</param>
    public SubmitFilterAttribute(Type buttonsEnumType, string[] actions, object submitButton, bool ignoreActionName)
    {
      if (buttonsEnumType == null)
      {
        throw new ArgumentNullException("buttonsEnumType");
      }

      if (!buttonsEnumType.IsEnum)
      {
        throw new ArgumentException("Argument buttonsEnumType must be an enum.", "buttonsEnumType");
      }

      if (actions == null)
      {
        throw new ArgumentNullException("actions");
      }

      if (submitButton != null && submitButton.GetType() != buttonsEnumType)
      {
        throw new ArgumentException("Argument submitButton must of type buttonsEnumType.", "submitButton");
      }

      _buttonsEnumType = buttonsEnumType;
      _submitButton = submitButton;

      _actionLinks =
        new List<ActionLink>(
          actions.Select(actionName => ActionLink.FromString(actionName)));

      _ignoreActionName = ignoreActionName;
    }

    /// <summary>
    /// Atrybut filtrujący akcję kontrolera wg. podanej nazwy akcji oraz przycisku html wywołującego akcję. 
    /// </summary>
    /// <param name="buttonsEnumType">Typ enum buttonów.</param>
    /// <param name="action">Nazwa akcji kontrolera.</param>
    /// <param name="submitButton">Przycisk wywołujący akcję.</param>
    public SubmitFilterAttribute(Type buttonsEnumType, string action, object submitButton)
      : this(buttonsEnumType, new[] { action }, submitButton, false)
    {
    }

    /// <summary>
    /// Atrybut filtrujący akcję kontrolera wg. podanej nazwy akcji. 
    /// </summary>
    /// <param name="action">Nazwa akcji kontrolera.</param>
    public SubmitFilterAttribute(Type buttonsEnumType, string action)
      : this(buttonsEnumType, new[] { action }, null, false)
    {
    }

    /// <summary>
    /// Atrybut filtrujący akcję kontrolera wg. podanej kolekcji nazw akcji. 
    /// </summary>
    /// <param name="actions">Lista nazw akcji pasująca do metody.</param>
    public SubmitFilterAttribute(Type buttonsEnumType, string[] actions)
      : this(buttonsEnumType, actions, null, false)
    {
    }

    /// <summary>
    /// Atrybut filtrujący akcję kontrolera wg. przycisku html wywołującego akcję. 
    /// </summary>
    /// <param name="submitButton">Przycisk wywołujący akcję</param>
    public SubmitFilterAttribute(Type buttonsEnumType, object submitButton)
      : this(buttonsEnumType, new string[0], submitButton, false)
    {
    }

    public SubmitFilterAttribute(Type buttonsEnumType, object submitButton, bool ignoreActionName)
      : this(buttonsEnumType, new string[0], submitButton, ignoreActionName)
    {
    }

    #endregion

    #region ActionNameSelectorAttribute overrides

    public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
    {
      bool isProperActionName = false;

      if (_ignoreActionName)
      {
        isProperActionName = true;
      }
      else if (_actionLinks == null)
      {
        isProperActionName = methodInfo.Name == actionName;
      }
      else
      {
        foreach (ActionLink actionLink in _actionLinks)
        {
          isProperActionName = controllerContext.RouteData.Values["action"].ToString() == actionLink.ActionName;

          if (isProperActionName)
          {
            break;
          }
        }
      }

      if (_submitButton == null && isProperActionName)
      {
        string[] buttons = Enum.GetNames(_buttonsEnumType);

        foreach (string button in buttons)
        {
          if (controllerContext.HttpContext.Request.Form.AllKeys.Contains(button))
          {
            return false;
          }
        }

        return true;
      }

      var form = controllerContext.HttpContext.Request.Form;

      return isProperActionName
             && form.AllKeys.Contains(_buttonsEnumType.Name + "_" + _submitButton);
    }

    #endregion
  }
}
