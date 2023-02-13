using System;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using FriendOrganizer.UI.Startup;

namespace FriendOrganizer.UI;

public partial class App : Application
{
  private void App_OnStartup(object sender, StartupEventArgs e)
  {
    var bootstrapper = new Bootstrapper();
    var container = bootstrapper.Bootstrap();

    var mainWindow = container.Resolve<MainWindow>();

    mainWindow.Show();
  }

  private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
  {
    MessageBox.Show(
      $"Unexpected error occured.  Please inform an administrator.{Environment.NewLine}{e.Exception.Message}",
      "Unexpected Error");

    e.Handled = true;
  }
}