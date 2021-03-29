// -----------------------------------------------
//     Author: Ramon Bollen
//      File: DependencyProperties.ComboBoxItemPositionToBorderThicknessConverter.cs
// Created on: 20210329
// -----------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DependencyProperties.Resources.Converters
{
    /// <summary>
    /// Shows a bottom border on all menu items except the last one
    /// https://stackoverflow.com/questions/12125764/change-style-of-last-item-in-listbox/24163954
    /// </summary>
    internal sealed class ComboBoxItemPositionToBorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DependencyObject item = (DependencyObject)value;
            ItemsControl     ic   = ItemsControl.ItemsControlFromItemContainer(item);

            return (ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1) ? new Thickness(0) : new Thickness(0, 0, 0, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
    }
}