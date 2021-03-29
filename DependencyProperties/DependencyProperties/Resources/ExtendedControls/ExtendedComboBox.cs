// -----------------------------------------------
//     Author: Ramon Bollen
//      File: DependencyProperties.ExtendedComboBox.cs
// Created on: 20210329
// -----------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DependencyProperties.Resources.ExtendedControls
{
    public class ExtendedComboBox : ComboBox
    {
        /// <summary>
        /// Foreground hover color extension
        /// </summary>
        public static readonly DependencyProperty HasForegroundHoverColorProperty =
            DependencyProperty.Register("HasForegroundHoverColor", typeof(bool), typeof(ExtendedComboBox), new PropertyMetadata(false));

        public static readonly DependencyProperty ForegroundHoverColorProperty =
            DependencyProperty.Register("ForegroundHoverColor", typeof(Brush), typeof(ExtendedComboBox), new PropertyMetadata(default(Brush), ForegroundHoverColorChangedCallback));

        public bool HasForegroundHoverColor => (bool)GetValue(HasForegroundHoverColorProperty);

        public Brush ForegroundHoverColor
        {
            get => (Brush)GetValue(ForegroundHoverColorProperty);
            set => SetValue(ForegroundHoverColorProperty, value);
        }

        private static void ForegroundHoverColorChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            (dependencyObject as ExtendedComboBox)?.SetValue(HasForegroundHoverColorProperty, true);
        }

        /// <summary>
        /// Background hover color extension
        /// </summary>
        public static readonly DependencyProperty HasBackgroundHoverColorProperty =
            DependencyProperty.Register("HasBackgroundHoverColor", typeof(bool), typeof(ExtendedComboBox), new PropertyMetadata(false));

        public static readonly DependencyProperty BackgroundHoverColorProperty =
            DependencyProperty.Register("BackgroundHoverColor", typeof(Brush), typeof(ExtendedComboBox), new PropertyMetadata(default(Brush), BackgroundHoverColorChangedCallback));

        public bool HasBackgroundHoverColor => (bool)GetValue(HasBackgroundHoverColorProperty);

        public Brush BackgroundHoverColor
        {
            get => (Brush)GetValue(BackgroundHoverColorProperty);
            set => SetValue(BackgroundHoverColorProperty, value);
        }

        private static void BackgroundHoverColorChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            (dependencyObject as ExtendedComboBox)?.SetValue(HasBackgroundHoverColorProperty, true);
        }
    }
}