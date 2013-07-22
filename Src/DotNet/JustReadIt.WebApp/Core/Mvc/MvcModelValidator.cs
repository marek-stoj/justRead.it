using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web.Mvc;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace JustReadIt.WebApp.Core.Mvc
{
  public class MvcModelValidator
  {
    private readonly IEnumerable<ResourceManager> _resourceManagers;
    private readonly string _genericErrorMessage;

    #region Constructor(s)

    public MvcModelValidator(IEnumerable<ResourceManager> resourceManagers, string genericErrorMessage)
    {
      if (resourceManagers == null)
      {
        throw new ArgumentNullException("resourceManagers");
      }

      if (string.IsNullOrEmpty(genericErrorMessage))
      {
        throw new ArgumentNullException("genericErrorMessage");
      }

      _resourceManagers = resourceManagers;
      _genericErrorMessage = genericErrorMessage;
    }

    #endregion

    #region Public validation methods

    public bool Validate<T>(T model, ModelStateDictionary modelState, string ruleSet)
    {
      bool validatorResult = ValidateInternal(model, modelState, "", ruleSet);

      // oprocz walidacji entlibowej uwzgledniamy rowniez aktualny stan obiektu ModelState
      return validatorResult
          && (modelState == null || modelState.IsValid);
    }

    public bool Validate<T>(T model, ModelStateDictionary modelState)
    {
      return Validate(model, modelState, "");
    }

    #endregion

    #region Helper methods

    private bool ValidateInternal<T>(T model, ModelStateDictionary modelState, string prefix, string ruleSet)
    {
      Validator validator;

      if (string.IsNullOrEmpty(ruleSet))
      {
        validator = ValidationFactory.CreateValidator(typeof(T));
      }
      else
      {
        validator = ValidationFactory.CreateValidator(typeof(T), ruleSet);
      }

      var validationResults = validator.Validate(model);

      if (validationResults.IsValid)
      {
        return true;
      }

      SetErrorsRecursive(prefix, model, validationResults, modelState);

      return false;
    }

    private void SetErrorsRecursive(string prefix, object model, ValidationResults validationResults, ModelStateDictionary modelState)
    {
      if (model == null)
      {
        throw new ArgumentNullException("model");
      }

      SetErrors(prefix, model, validationResults, modelState);

      if (model is string)
      {
        return;
      }

      if (model is IEnumerable)
      {
        foreach (object elem in (model as IEnumerable))
        {
          if (elem == null)
          {
            continue;
          }

          SetErrorsRecursive(prefix, elem, validationResults, modelState);
        }
      }
      else
      {
        PropertyInfo[] properties = model.GetType().GetProperties();

        foreach (PropertyInfo propertyInfo in properties)
        {
          object propertyValue = propertyInfo.GetValue(model, null);

          if (propertyValue == null)
          {
            continue;
          }

          string newPrefix = string.IsNullOrEmpty(prefix)
            ? propertyInfo.Name
            : prefix + "." + propertyInfo.Name;

          SetErrorsRecursive(newPrefix, propertyValue, validationResults, modelState);
        }
      }
    }

    private void SetErrors(string prefix, object model, IEnumerable<ValidationResult> validationResults, ModelStateDictionary modelState)
    {
      foreach (ValidationResult validationResult in validationResults)
      {
        if (validationResult.Target == model)
        {
          string key;

          if (string.IsNullOrEmpty(prefix))
          {
            key = validationResult.Key;
          }
          else
          {
            key = prefix + (!string.IsNullOrEmpty(validationResult.Key) ? "." + validationResult.Key : "");
          }

          string errorMessage = GetErrorFromResource(validationResult);

          if (!string.IsNullOrEmpty(errorMessage))
          {
            modelState.AddModelError(key, errorMessage);
          }
        }

        if (validationResult.NestedValidationResults != null)
        {
          SetErrors(prefix, model, validationResult.NestedValidationResults, modelState);
        }
      }
    }

    private string GetErrorFromResource(ValidationResult validationResult)
    {
      if (string.IsNullOrEmpty(validationResult.Tag))
      {
        if (!(validationResult.Validator is NotNullValidator) && (validationResult.NestedValidationResults == null || validationResult.NestedValidationResults.Count() == 0))
        {
          return _genericErrorMessage;
        }

        return validationResult.Message ?? "";
      }

      string result = null;

      foreach (var resourceManager in _resourceManagers)
      {
        result = resourceManager.GetString(validationResult.Tag);

        if (result != null)
        {
          break;
        }
      }

      if (result == null)
      {
        throw new KeyNotFoundException(string.Format("Couldn't find validation error message for tag '{0}'.", validationResult.Tag));
      }

      return result;
    }

    #endregion
  }
}
