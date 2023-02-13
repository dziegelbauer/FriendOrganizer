using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac.Features.Indexed;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel;

public class MainViewModel : ViewModelBase
{
  private readonly IIndex<string, IDetailViewModel> _detailViewModelCreator;
  private readonly IEventAggregator _eventAggregator;
  private readonly IMessageDialogService _messageDialogService;

  private int _nextNewItemId;
  private IDetailViewModel? _selectedDetailViewModel;

  public MainViewModel(INavigationViewModel navigationViewModel,
    IIndex<string, IDetailViewModel> detailViewModelCreator,
    IEventAggregator eventAggregator,
    IMessageDialogService messageDialogService)
  {
    NavigationViewModel = navigationViewModel;
    DetailViewModels = new ObservableCollection<IDetailViewModel>();
    _detailViewModelCreator = detailViewModelCreator;
    _eventAggregator = eventAggregator;
    _messageDialogService = messageDialogService;

    _eventAggregator.GetEvent<OpenDetailViewEvent>()
      .Subscribe(OnOpenDetailView);

    _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
      .Subscribe(AfterDetailDeleted);

    _eventAggregator.GetEvent<AfterDetailClosedEvent>()
      .Subscribe(AfterDetailClosed);

    CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
    OpenSingleDetailViewCommand = new DelegateCommand<Type>(OnOpenSingleDetailViewExecute);
  }

  public INavigationViewModel NavigationViewModel { get; }

  public IDetailViewModel? SelectedDetailViewModel
  {
    get => _selectedDetailViewModel;
    set
    {
      _selectedDetailViewModel = value;
      OnPropertyChanged();
    }
  }

  public ObservableCollection<IDetailViewModel> DetailViewModels { get; }

  public ICommand CreateNewDetailCommand { get; }

  public ICommand OpenSingleDetailViewCommand { get; }

  private void OnOpenSingleDetailViewExecute(Type viewModelType)
  {
    OnOpenDetailView(new OpenDetailViewEventArgs
    {
      Id = -1,
      ViewModelName = viewModelType.Name
    });
  }

  private void AfterDetailClosed(AfterDetailClosedEventArgs args)
  {
    RemoveDetailViewModel(args.Id, args.ViewModelName);
  }

  private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
  {
    RemoveDetailViewModel(args.Id, args.ViewModelName);
  }

  private void RemoveDetailViewModel(int id, string viewModelName)
  {
    var detailViewModel = DetailViewModels
      .SingleOrDefault(vm => vm.Id == id && vm.GetType().Name == viewModelName);

    if (detailViewModel is not null) DetailViewModels.Remove(detailViewModel);
  }

  private void OnCreateNewDetailExecute(Type viewModelType)
  {
    OnOpenDetailView(new OpenDetailViewEventArgs
    {
      Id = _nextNewItemId--,
      ViewModelName = viewModelType.Name
    });
  }

  public async Task LoadAsync()
  {
    await NavigationViewModel.LoadAsync();
  }

  private async void OnOpenDetailView(OpenDetailViewEventArgs args)
  {
    var detailViewModel = DetailViewModels
      .SingleOrDefault(vm => vm.Id == args.Id && vm.GetType().Name == args.ViewModelName);

    if (detailViewModel is null)
    {
      detailViewModel = _detailViewModelCreator[args.ViewModelName];
      try
      {
        await detailViewModel.LoadAsync(args.Id);
      }
      catch
      {
        _messageDialogService.ShowInfoDialogAsync("The record has been deleted by another user.");
        await NavigationViewModel.LoadAsync();
        return;
      }

      DetailViewModels.Add(detailViewModel);
    }

    SelectedDetailViewModel = detailViewModel;
  }
}