using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel;

public class FriendDetailViewModel : DetailViewModelBase, IFriendDetailViewModel
{
  private readonly IFriendRepository _friendRepository;
  private readonly IProgrammingLanguageLookupDataService _programmingLanguageLookupDataService;
  private FriendWrapper? _friend;
  private FriendPhoneNumberWrapper? _selectedPhoneNumber;

  public FriendDetailViewModel(IFriendRepository friendRepository,
    IEventAggregator eventAggregator,
    IMessageDialogService messageDialogService,
    IProgrammingLanguageLookupDataService programmingLanguageLookupDataService) : base(eventAggregator,
    messageDialogService)
  {
    _friendRepository = friendRepository;
    _programmingLanguageLookupDataService = programmingLanguageLookupDataService;

    _eventAggregator.GetEvent<AfterCollectionSavedEvent>()
      .Subscribe(AfterCollectionSaved);

    SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
    DeleteCommand = new DelegateCommand(OnDeleteExecute);
    AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
    RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);

    ProgrammingLanguages = new ObservableCollection<LookupItem>();
    PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
  }

  public ICommand AddPhoneNumberCommand { get; }
  public ICommand RemovePhoneNumberCommand { get; }
  public ObservableCollection<LookupItem> ProgrammingLanguages { get; }
  public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }

  public FriendPhoneNumberWrapper? SelectedPhoneNumber
  {
    get => _selectedPhoneNumber;
    set
    {
      _selectedPhoneNumber = value;
      OnPropertyChanged();
      ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
    }
  }

  public FriendWrapper? Friend
  {
    get => _friend;
    set
    {
      _friend = value;
      OnPropertyChanged();
    }
  }

  public override async Task LoadAsync(int id)
  {
    var friend = id > 0
      ? new FriendWrapper(await _friendRepository.GetByIdAsync(id))
      : CreateNewFriend();

    _id = friend.Id;

    InitializeFriend(friend);

    InitializeFriendPhoneNumbers(friend.PhoneNumbers);

    await LoadProgrammingLookupAsync();
  }

  private async void AfterCollectionSaved(AfterCollectionSavedEventArgs args)
  {
    if (args.ViewModelName == nameof(ProgrammingLanguageDetailViewModel)) await LoadProgrammingLookupAsync();
  }

  private bool OnRemovePhoneNumberCanExecute()
  {
    return SelectedPhoneNumber is not null;
  }

  private void OnRemovePhoneNumberExecute()
  {
    if (SelectedPhoneNumber is not null && Friend is not null)
    {
      SelectedPhoneNumber.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
      _friendRepository.RemovePhoneNumber(SelectedPhoneNumber.Model);
      PhoneNumbers.Remove(SelectedPhoneNumber);
      SelectedPhoneNumber = null;
      HasChanges = _friendRepository.HasChanges();
      ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
    }
  }

  private void OnAddPhoneNumberExecute()
  {
    var newNumber = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
    newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
    PhoneNumbers.Add(newNumber);
    Friend?.Model.PhoneNumbers.Add(newNumber.Model);
    newNumber.Number = string.Empty;
  }


  protected override async void OnDeleteExecute()
  {
    if (Friend is not null)
    {
      if (await _friendRepository.HasMeetingsAsync(Friend.Id))
      {
        await _messageDialogService.ShowInfoDialogAsync(
          $"{Friend.FirstName} {Friend.LastName} can't be deleted because they have scheduled meetings.");
        return;
      }

      var result = await _messageDialogService.ShowOkCancelDialogAsync(
        $"Do you really want to delete {Friend.FirstName}?",
        "Question");
      if (result == MessageDialogResult.Ok)
      {
        _friendRepository.Remove(Friend.Model);
        await _friendRepository.SaveAsync();
        RaiseDetailDeletedEvent(Friend.Id);
      }
    }
  }

  private void InitializeFriendPhoneNumbers(ICollection<FriendPhoneNumber> phoneNumbers)
  {
    foreach (var wrapper in PhoneNumbers) wrapper.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;

    PhoneNumbers.Clear();
    foreach (var friendPhoneNumber in phoneNumbers)
    {
      var wrapper = new FriendPhoneNumberWrapper(friendPhoneNumber);
      PhoneNumbers.Add(wrapper);
      wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
    }
  }

  private void FriendPhoneNumberWrapper_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (!HasChanges) HasChanges = _friendRepository.HasChanges();

    if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
      ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
  }

  private void InitializeFriend(FriendWrapper friend)
  {
    Friend = friend;

    Friend.PropertyChanged += (_, e) =>
    {
      if (!HasChanges) HasChanges = _friendRepository.HasChanges();

      if (e.PropertyName == nameof(Friend.HasErrors)) ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

      if (e.PropertyName == nameof(Friend.FirstName)
          || e.PropertyName == nameof(Friend.LastName))
        SetTitle();
    };
    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
    if (Friend.Id == 0) Friend.FirstName = string.Empty;

    SetTitle();
  }

  private void SetTitle()
  {
    if (Friend is not null) Title = $"{Friend.FirstName} {Friend.LastName}";
  }

  private async Task LoadProgrammingLookupAsync()
  {
    ProgrammingLanguages.Clear();
    ProgrammingLanguages.Add(new NullLookupItem());
    var lookups = await _programmingLanguageLookupDataService.GetProgrammingLanguageLookupAsync();
    foreach (var item in lookups) ProgrammingLanguages.Add(item);
  }

  private FriendWrapper CreateNewFriend()
  {
    var friend = new Friend
    {
      Id = 0,
      FirstName = string.Empty,
      LastName = string.Empty,
      Email = string.Empty
    };
    _friendRepository.Add(friend);
    return new FriendWrapper(friend);
  }

  protected override async void OnSaveExecute()
  {
    await SaveWithOptimisticConcurrencyAsync(_friendRepository.SaveAsync, () =>
    {
      if (Friend is not null)
      {
        _id = Friend.Id;
        HasChanges = _friendRepository.HasChanges();
        RaiseDetailSavedEvent(Friend.Id, $"{Friend.FirstName} {Friend.LastName}");
      }
    });
  }

  protected override bool OnSaveCanExecute()
  {
    return Friend is not null
           && !Friend.HasErrors
           && PhoneNumbers.All(pn => !pn.HasErrors)
           && HasChanges;
  }
}