using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Event;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel;

public class NavigationViewModel : ViewModelBase, INavigationViewModel
{
  private readonly IEventAggregator _eventAggregator;
  private readonly ILookupDataService _lookupDataService;

  public NavigationViewModel(ILookupDataService lookupDataService, IEventAggregator eventAggregator)
  {
    _lookupDataService = lookupDataService;
    _eventAggregator = eventAggregator;
    Friends = new ObservableCollection<NavigationItemViewModel>();
    Meetings = new ObservableCollection<NavigationItemViewModel>();

    _eventAggregator.GetEvent<AfterDetailSavedEvent>()
      .Subscribe(AfterDetailSaved);
    _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
      .Subscribe(AfterDetailDeleted);
  }

  public ObservableCollection<NavigationItemViewModel> Friends { get; set; }
  public ObservableCollection<NavigationItemViewModel> Meetings { get; set; }

  public async Task LoadAsync()
  {
    var lookup = await _lookupDataService.GetFriendLookupAsync();
    Friends.Clear();
    foreach (var item in lookup)
      Friends.Add(new NavigationItemViewModel(item.Id, item.DisplayMember, _eventAggregator,
        nameof(FriendDetailViewModel)));

    lookup = await _lookupDataService.GetMeetingLookupAsync();
    Meetings.Clear();
    foreach (var item in lookup)
      Meetings.Add(new NavigationItemViewModel(item.Id, item.DisplayMember, _eventAggregator,
        nameof(MeetingDetailViewModel)));
  }

  private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
  {
    switch (args.ViewModelName)
    {
      case nameof(FriendDetailViewModel):
        AfterDetailDeleted(Friends, args);
        break;
      case nameof(MeetingDetailViewModel):
        AfterDetailDeleted(Meetings, args);
        break;
    }
  }

  private void AfterDetailDeleted(ObservableCollection<NavigationItemViewModel> items, AfterDetailDeletedEventArgs args)
  {
    var item = items.SingleOrDefault(f => f.Id == args.Id);
    if (item is not null) items.Remove(item);
  }

  private void AfterDetailSaved(AfterDetailSavedEventArgs args)
  {
    switch (args.ViewModelName)
    {
      case nameof(FriendDetailViewModel):
        AfterDetailSaved(Friends, args);
        break;
      case nameof(MeetingDetailViewModel):
        AfterDetailSaved(Meetings, args);
        break;
    }
  }

  private void AfterDetailSaved(ObservableCollection<NavigationItemViewModel> items, AfterDetailSavedEventArgs args)
  {
    var lookupItem = items.SingleOrDefault(f => f.Id == args.Id);
    if (lookupItem is null)
      items.Add(new NavigationItemViewModel(args.Id, args.DisplayMember, _eventAggregator, args.ViewModelName));
    else
      lookupItem.DisplayMember = args.DisplayMember;
  }
}