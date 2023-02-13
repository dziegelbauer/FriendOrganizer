using System.Windows;
using FriendOrganizer.UI.ViewModel;
using MahApps.Metro.Controls;

namespace FriendOrganizer.UI;

public partial class MainWindow : MetroWindow
{
  private readonly MainViewModel _viewModel;

  public MainWindow(MainViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    DataContext = _viewModel;
    Loaded += OnLoaded;
  }

  private async void OnLoaded(object sender, RoutedEventArgs e)
  {
    await _viewModel.LoadAsync();
  }
}