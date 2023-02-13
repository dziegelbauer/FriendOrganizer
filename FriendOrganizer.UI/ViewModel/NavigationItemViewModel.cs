using System.Windows.Input;
using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel;

public class NavigationItemViewModel : ViewModelBase
{
  private readonly string _detailViewModelName;
  private readonly IEventAggregator _eventAggregator;
  private string _displayMember;

  public NavigationItemViewModel(int id, string displayMember, IEventAggregator eventAggregator,
    string detailViewModelName)
  {
    _eventAggregator = eventAggregator;
    Id = id;
    DisplayMember = displayMember;
    _detailViewModelName = detailViewModelName;
    OpenDetailViewCommand = new DelegateCommand(OnOpenDetailViewExecute);
  }

  public int Id { get; }

  public string DisplayMember
  {
    get => _displayMember;
    set
    {
      _displayMember = value;
      OnPropertyChanged();
    }
  }

  public ICommand OpenDetailViewCommand { get; }

  private void OnOpenDetailViewExecute()
  {
    _eventAggregator.GetEvent<OpenDetailViewEvent>()
      .Publish(new OpenDetailViewEventArgs
      {
        Id = Id,
        ViewModelName = _detailViewModelName
      });
  }
}