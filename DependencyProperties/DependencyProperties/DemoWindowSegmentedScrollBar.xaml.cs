// -----------------------------------------------
//     Author: Ramon Bollen
//      File: DependencyProperties.DemoWindowSegmentedScrollBar.xaml.cs
// Created on: 20201209
// -----------------------------------------------

namespace DependencyProperties
{
    /// <summary>
    /// Interaction logic for DemoWindowSegmentedScrollBar.xaml
    /// </summary>
    public partial class DemoWindowSegmentedScrollBar
    {
        public DemoWindowSegmentedScrollBar()
        {
            InitializeComponent();

            Loaded += (_, _) => DemoWindowSegmentedScrollBarLoaded();
        }

        private void DemoWindowSegmentedScrollBarLoaded()
        {
            DataContext = new DemoWindowSegmentedScrollBarViewModel();
        }
    }
}