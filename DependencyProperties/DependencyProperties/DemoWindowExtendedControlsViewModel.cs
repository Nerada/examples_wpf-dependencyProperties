// -----------------------------------------------
//     Author: Ramon Bollen
//      File: DependencyProperties.DemoWindowExtendedControlsViewModel.cs
// Created on: 20210329
// -----------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DependencyProperties
{
    public class DemoWindowExtendedControlsViewModel : ViewModelBase
    {
        private DemoItem _selectedDemoItem;

        public DemoWindowExtendedControlsViewModel()
        {
            DemoItems = new List<DemoItem>()
            {
                new("Item1", isDefault: false),
                new("Item2 is default and thus bold", isDefault: true),
                new("Item3 is a lot of text in this list that wont fit in the full horizontal space", isDefault: false),
                new("Item4", isDefault: false),
                new("Item5", isDefault: false)
            }.ToImmutableArray();

            _selectedDemoItem = DemoItems[1];
        }

        public ImmutableArray<DemoItem> DemoItems { get; }

        public DemoItem SelectedDemoItem
        {
            get => _selectedDemoItem;
            set
            {
                if (!Set(ref _selectedDemoItem, value)) throw new InvalidOperationException($"Cannot set {nameof(SelectedDemoItem)} with value {value}.");
            }
        }
    }

    public class DemoItem
    {
        public DemoItem(string content, bool isDefault)
        {
            Content   = content;
            IsDefault = isDefault;
        }

        // ReSharper disable once MemberCanBePrivate.Global, reason=Used by Xaml
        public string Content { get; }

        // ReSharper disable once MemberCanBePrivate.Global, reason=Used by Xaml
        // ReSharper disable once UnusedAutoPropertyAccessor.Global, reason=Used by Xaml
        public bool IsDefault { get; }

        public override string ToString() => Content;
    }
}