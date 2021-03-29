// -----------------------------------------------
//     Author: Ramon Bollen
//      File: DependencyProperties.App.xaml.cs
// Created on: 20201209
// -----------------------------------------------

using System.Windows;

namespace DependencyProperties
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            new DemoWindowExtendedControls().Show();
            new DemoWindowSegmentedScrollBar().Show();
        }
    }
}