// -----------------------------------------------
//     Author: Ramon Bollen
//      File: DependencyProperties.ViewModelBase.cs
// Created on: 20210329
// -----------------------------------------------

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DependencyProperties
{
    /// <summary>
    /// Base class for view models.
    /// https://stackoverflow.com/questions/36149863/how-to-write-a-viewmodelbase-in-mvvm
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs>
            CachedPropertyChangedEventArgs = new();

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Indicates if <see cref="PropertyChanged"/> has any subscribers.
        /// </summary>
        protected bool AnyPropertyChangedSubscribers => PropertyChanged != null;

        /// <summary>
        /// Assigns a new value to a property's backing field. Then, raises the <see cref="PropertyChanged"/> event if needed.
        /// </summary>
        /// <typeparam name="T">The type of the backing field.</typeparam>
        /// <param name="field">The backing field storing the property's value.</param>
        /// <param name="newValue">The property's new value.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>True if the <see cref="PropertyChanged"/> event was raised, false otherwise.</returns>
        protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>Raises the PropertyChanged event if needed.</summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (propertyName == null)
            {
                return;
            }

            PropertyChanged?.Invoke(this, GetCachedPropertyChangedEventArgs(propertyName));
        }

        private PropertyChangedEventArgs GetCachedPropertyChangedEventArgs(string propertyName)
        {
            if (!CachedPropertyChangedEventArgs.TryGetValue(propertyName, out PropertyChangedEventArgs? args))
            {
                args = new PropertyChangedEventArgs(propertyName);
                CachedPropertyChangedEventArgs.TryAdd(propertyName, args);
            }

            return args;
        }
    }
}