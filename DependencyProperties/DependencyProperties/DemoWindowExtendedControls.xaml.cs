// -----------------------------------------------
//     Author: Ramon Bollen
//      File: DependencyProperties.DemoWindowExtendedControls.xaml.cs
// Created on: 20201209
// -----------------------------------------------

namespace DependencyProperties
{
    /// <summary>
    /// Interaction logic for DemoWindowExtendedControls.xaml
    /// </summary>
    public partial class DemoWindowExtendedControls
    {
        public DemoWindowExtendedControls()
        {
            InitializeComponent();

            Loaded += (_, _) => DemoWindowExtendedControlsLoaded();
        }

        private void DemoWindowExtendedControlsLoaded()
        {
            DataContext = new DemoWindowExtendedControlsViewModel();
        }
    }
}