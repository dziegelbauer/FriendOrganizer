using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FriendOrganizer.UI.ViewModel;

namespace FriendOrganizer.UI.Wrapper;

public class NotifyDataErrorInfoBase : ViewModelBase, INotifyDataErrorInfo
{
  private readonly Dictionary<string, List<string>> _errorsByPropertyName = new();

  public IEnumerable GetErrors(string? propertyName)
  {
    if (propertyName is not null) return _errorsByPropertyName[propertyName];

    return new List<string>();
  }

  public bool HasErrors => _errorsByPropertyName.Any();
  public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

  private void OnErrorsChanged(string propertyName)
  {
    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    base.OnPropertyChanged(nameof(HasErrors));
  }

  protected void AddError(string propertyName, string error)
  {
    if (!_errorsByPropertyName.ContainsKey(propertyName)) _errorsByPropertyName[propertyName] = new List<string>();

    if (!_errorsByPropertyName[propertyName].Contains(error))
    {
      _errorsByPropertyName[propertyName].Add(error);
      OnErrorsChanged(propertyName);
    }
  }

  protected void ClearErrors(string propertyName)
  {
    if (_errorsByPropertyName.ContainsKey(propertyName))
    {
      _errorsByPropertyName.Remove(propertyName);
      OnErrorsChanged(propertyName);
    }
  }
}