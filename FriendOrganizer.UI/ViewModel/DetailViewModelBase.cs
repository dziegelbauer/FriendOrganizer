using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel;

public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
{
  protected readonly IEventAggregator _eventAggregator;
  protected readonly IMessageDialogService _messageDialogService;
  private bool _hasChanges;
  protected int _id;
  private string _title;

  public DetailViewModelBase(IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
  {
    _eventAggregator = eventAggregator;
    _messageDialogService = messageDialogService;
    SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
    DeleteCommand = new DelegateCommand(OnDeleteExecute);
    CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
  }

  public ICommand CloseDetailViewCommand { get; }

  public ICommand DeleteCommand { get; set; }

  public ICommand SaveCommand { get; set; }

  public string Title
  {
    get => _title;
    set
    {
      _title = value;
      OnPropertyChanged();
    }
  }

  public abstract Task LoadAsync(int id);

  public bool HasChanges
  {
    get => _hasChanges;
    set
    {
      if (_hasChanges != value)
      {
        _hasChanges = value;
        OnPropertyChanged();
        ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
      }
    }
  }

  public int Id
  {
    get => _id;
    protected set => _id = value;
  }

  protected virtual async void OnCloseDetailViewExecute()
  {
    if (HasChanges)
    {
      var result = await _messageDialogService.ShowOkCancelDialogAsync(
        "You've made changes. Close this item?", "Question");

      if (result == MessageDialogResult.Cancel) return;
    }

    _eventAggregator.GetEvent<AfterDetailClosedEvent>()
      .Publish(new AfterDetailClosedEventArgs
      {
        Id = Id,
        ViewModelName = GetType().Name
      });
  }

  protected abstract void OnDeleteExecute();

  protected abstract bool OnSaveCanExecute();

  protected abstract void OnSaveExecute();

  protected virtual void RaiseDetailDeletedEvent(int id)
  {
    _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
      .Publish(new AfterDetailDeletedEventArgs
      {
        Id = id,
        ViewModelName = GetType().Name
      });
  }

  protected virtual void RaiseDetailSavedEvent(int id, string displayMember)
  {
    _eventAggregator.GetEvent<AfterDetailSavedEvent>()
      .Publish(new AfterDetailSavedEventArgs
      {
        Id = id,
        DisplayMember = displayMember,
        ViewModelName = GetType().Name
      });
  }

  protected virtual void RaiseCollectionSavedEvent()
  {
    _eventAggregator.GetEvent<AfterCollectionSavedEvent>()
      .Publish(new AfterCollectionSavedEventArgs
      {
        ViewModelName = GetType().Name
      });
  }

  protected async Task SaveWithOptimisticConcurrencyAsync(Func<Task> saveFunc, Action afterSaveAction)
  {
    try
    {
      await saveFunc();
    }
    catch (DbUpdateConcurrencyException e)
    {
      var databaseValues = await e.Entries.Single().GetDatabaseValuesAsync();
      if (databaseValues is null)
      {
        await _messageDialogService.ShowInfoDialogAsync("The record has been deleted by another user.");
        RaiseDetailDeletedEvent(Id);
        return;
      }

      var result = await _messageDialogService.ShowOkCancelDialogAsync("This record has been changed by someone else"
                                                            + "Click OK to save your changes anyway, click Cancel"
                                                            + " to reload the record.", "Question");

      if (result ==MessageDialogResult.Ok)
      {
        var entry = e.Entries.Single();
        entry.OriginalValues.SetValues(databaseValues);
        await saveFunc();
      }
      else
      {
        await e.Entries.Single().ReloadAsync();
        await LoadAsync(Id);
      }
    }

    afterSaveAction();
  }
}