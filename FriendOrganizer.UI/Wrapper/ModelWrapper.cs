using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FriendOrganizer.UI.Wrapper;

public class ModelWrapper<T> : NotifyDataErrorInfoBase
{
  public ModelWrapper(T model)
  {
    Model = model;
  }

  public T Model { get; set; }

  protected virtual TValue GetValue<TValue>([CallerMemberName] string? propertyName = null)
  {
    return (TValue)typeof(T).GetProperty(propertyName).GetValue(Model);
  }

  protected virtual void SetValue<TValue>(TValue value, [CallerMemberName] string? propertyName = null)
  {
    typeof(T).GetProperty(propertyName).SetValue(Model, value);
    OnPropertyChanged(propertyName);
    ValidatePropertyInternal(propertyName, value);
  }

  private void ValidatePropertyInternal(string propertyName, object currentValue)
  {
    ClearErrors(propertyName);

    ValidateDataAnnotations(propertyName, currentValue);

    ValidateCustomErrors(propertyName);
  }

  private void ValidateDataAnnotations(string propertyName, object currentValue)
  {
    var validationContext = new ValidationContext(Model)
    {
      MemberName = propertyName
    };

    var validationResults = new List<ValidationResult>();

    Validator.TryValidateProperty(currentValue, validationContext, validationResults);

    foreach (var result in validationResults) AddError(propertyName, result.ErrorMessage);
  }

  private void ValidateCustomErrors(string propertyName)
  {
    var errors = ValidateProperty(propertyName);

    if (errors is not null)
      foreach (var error in errors)
        AddError(propertyName, error);
  }

  protected virtual IEnumerable<string>? ValidateProperty(string propertyName)
  {
    return null;
  }
}