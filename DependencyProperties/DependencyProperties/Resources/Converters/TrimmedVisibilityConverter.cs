// -----------------------------------------------
//     Author: Ramon Bollen
//      File: DependencyProperties.TrimmedVisibilityConverter.cs
// Created on: 20210329
// -----------------------------------------------

using System;
using System.Windows;
using System.Windows.Data;

namespace DependencyProperties.Resources.Converters
{
    /// <summary>
    /// https://stackoverflow.com/a/21863054
    /// </summary>
    internal sealed class TrimmedVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            FrameworkElement element = (FrameworkElement)value;

            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return ((FrameworkElement)value).ActualWidth < ((FrameworkElement)value).DesiredSize.Width ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => DependencyProperty.UnsetValue;
    }
}