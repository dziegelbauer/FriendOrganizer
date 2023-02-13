﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel;

public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
{
  private readonly IMeetingRepository _meetingRepository;
  private List<Friend>? _allFriends;
  private MeetingWrapper? _meeting;
  private Friend? _selectedAddedFriend;
  private Friend? _selectedAvailableFriend;

  public MeetingDetailViewModel(IEventAggregator eventAggregator,
    IMessageDialogService messageDialogService,
    IMeetingRepository meetingRepository) : base(eventAggregator, messageDialogService)
  {
    _meetingRepository = meetingRepository;

    eventAggregator.GetEvent<AfterDetailSavedEvent>()
      .Subscribe(AfterDetailSaved);

    eventAggregator.GetEvent<AfterDetailDeletedEvent>()
      .Subscribe(AfterDetailDeleted);

    AddedFriends = new ObservableCollection<Friend>();
    AvailableFriends = new ObservableCollection<Friend>();

    AddFriendCommand = new DelegateCommand(OnAddFriendExecute, OnAddFriendCanExecute);
    RemoveFriendCommand = new DelegateCommand(OnRemoveFriendExecute, OnRemoveFriendCanExecute);
  }

  public ICommand RemoveFriendCommand { get; }
  public ICommand AddFriendCommand { get; }
  public ObservableCollection<Friend> AvailableFriends { get; }
  public ObservableCollection<Friend> AddedFriends { get; }

  public Friend? SelectedAvailableFriend
  {
    get => _selectedAvailableFriend;
    set
    {
      _selectedAvailableFriend = value;
      OnPropertyChanged();
      ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
    }
  }

  public Friend? SelectedAddedFriend
  {
    get => _selectedAddedFriend;
    set
    {
      _selectedAddedFriend = value;
      OnPropertyChanged();
      ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
    }
  }

  public MeetingWrapper? Meeting
  {
    get => _meeting;
    set
    {
      _meeting = value;
      OnPropertyChanged();
    }
  }

  public override async Task LoadAsync(int id)
  {
    var meeting = id > 0
      ? await _meetingRepository.GetByIdAsync(id)
      : CreateNewMeeting();

    _id = meeting.Id;

    InitializeMeeting(meeting);

    _allFriends = await _meetingRepository.GetAllFriendsAsync();

    SetupPicklist();
  }

  private async void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
  {
    if (args.ViewModelName == nameof(FriendDetailViewModel))
    {
      _allFriends = await _meetingRepository.GetAllFriendsAsync();
      SetupPicklist();
    }
  }

  private async void AfterDetailSaved(AfterDetailSavedEventArgs args)
  {
    if (args.ViewModelName == nameof(FriendDetailViewModel))
    {
      await _meetingRepository.ReloadFriendAsync(args.Id);
      _allFriends = await _meetingRepository.GetAllFriendsAsync();
      SetupPicklist();
    }
  }

  private void SetupPicklist()
  {
    if (Meeting is not null && _allFriends is not null)
    {
      var meetingFriendIds = Meeting.Model.Friends.Select(f => f.Id).ToList();
      var addedFriends = _allFriends.Where(f => meetingFriendIds.Contains(f.Id)).OrderBy(f => f.FirstName).ToList();
      var availableFriends = _allFriends.Except(addedFriends).OrderBy(f => f.FirstName).ToList();

      AddedFriends.Clear();
      AvailableFriends.Clear();

      foreach (var addedFriend in addedFriends) AddedFriends.Add(addedFriend);

      foreach (var availableFriend in availableFriends) AvailableFriends.Add(availableFriend);
    }
  }

  private bool OnRemoveFriendCanExecute()
  {
    return SelectedAddedFriend is not null;
  }

  private void OnRemoveFriendExecute()
  {
    if (SelectedAddedFriend is not null && Meeting is not null)
    {
      var friendToRemove = SelectedAddedFriend;

      Meeting.Model.Friends.Remove(friendToRemove);
      AddedFriends.Remove(friendToRemove);
      AvailableFriends.Add(friendToRemove);
      HasChanges = _meetingRepository.HasChanges();
      ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
    }
  }

  private bool OnAddFriendCanExecute()
  {
    return SelectedAvailableFriend is not null;
  }

  private void OnAddFriendExecute()
  {
    if (SelectedAvailableFriend is not null && Meeting is not null)
    {
      var friendToAdd = SelectedAvailableFriend;

      Meeting.Model.Friends.Add(friendToAdd);
      AddedFriends.Add(friendToAdd);
      AvailableFriends.Remove(friendToAdd);
      HasChanges = _meetingRepository.HasChanges();
      ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
    }
  }

  private void InitializeMeeting(Meeting meeting)
  {
    Meeting = new MeetingWrapper(meeting);
    Meeting.PropertyChanged += (_, e) =>
    {
      if (!HasChanges) HasChanges = _meetingRepository.HasChanges();

      if (e.PropertyName == nameof(Meeting.HasErrors)) ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

      if (e.PropertyName == nameof(Meeting.Title)) SetTitle();
    };
    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

    if (Meeting.Id == 0) Meeting.Title = string.Empty;

    SetTitle();
  }

  private void SetTitle()
  {
    if (Meeting is not null) Title = Meeting.Title;
  }

  private Meeting CreateNewMeeting()
  {
    var meeting = new Meeting
    {
      DateFrom = DateTime.Now.Date,
      DateTo = DateTime.Now.Date
    };

    _meetingRepository.Add(meeting);
    return meeting;
  }

  protected override async void OnDeleteExecute()
  {
    if (Meeting is not null)
    {
      var result =
        await _messageDialogService.ShowOkCancelDialogAsync($"Do you really want to delete the meeting: {Meeting.Title}?",
          "Question");
      if (result == MessageDialogResult.Ok)
      {
        _meetingRepository.Remove(Meeting.Model);
        await _meetingRepository.SaveAsync();
        RaiseDetailDeletedEvent(Meeting.Id);
      }
    }
  }

  protected override bool OnSaveCanExecute()
  {
    return Meeting is not null && !Meeting.HasErrors && HasChanges;
  }

  protected override async void OnSaveExecute()
  {
    if (Meeting is not null)
    {
      await _meetingRepository.SaveAsync();
      _id = Meeting.Id;
      HasChanges = _meetingRepository.HasChanges();
      RaiseDetailSavedEvent(Meeting.Id, Meeting.Title);
    }
  }
}