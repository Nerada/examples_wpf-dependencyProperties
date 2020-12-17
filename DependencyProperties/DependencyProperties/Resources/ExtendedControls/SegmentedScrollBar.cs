// -----------------------------------------------
//     Author: Ramon Bollen
//      File: DependencyProperties.SegmentedScrollBar.cs
// Created on: 20201208
// -----------------------------------------------

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Prism.Commands;

namespace DependencyProperties.Resources.ExtendedControls
{
    public class SegmentedScrollBar : ScrollBar
    {
        public static readonly DependencyProperty CanExecutePreviousCommandProperty =
            DependencyProperty.Register("CanExecutePreviousCommand", typeof(bool), typeof(SegmentedScrollBar));

        public static readonly DependencyProperty CanExecuteNextCommandProperty =
            DependencyProperty.Register("CanExecuteNextCommand", typeof(bool), typeof(SegmentedScrollBar));

        public static readonly DependencyProperty SegmentBoundariesProperty =
            DependencyProperty.Register("SegmentBoundaries", typeof(List<double>), typeof(SegmentedScrollBar),
                                        new PropertyMetadata(default(List<double>), SegmentBoundariesChangedCallback));

        public static readonly DependencyProperty SegmentColorsProperty =
            DependencyProperty.Register("SegmentColors", typeof(List<Brush>), typeof(SegmentedScrollBar),
                                        new PropertyMetadata(default(List<Brush>), SegmentAppearanceChangedCallback));

        public static readonly DependencyProperty DrawRegionsProperty =
            DependencyProperty.Register("DrawRegions", typeof(bool), typeof(SegmentedScrollBar),
                                        new PropertyMetadata(default(bool), SegmentAppearanceChangedCallback));

        public static readonly DependencyProperty PreviousSegmentCommandProperty =
            DependencyProperty.Register("PreviousSegmentCommand", typeof(DelegateCommand), typeof(SegmentedScrollBar));

        public static readonly DependencyProperty NextSegmentCommandProperty =
            DependencyProperty.Register("NextSegmentCommand", typeof(DelegateCommand), typeof(SegmentedScrollBar));

        // Using a DependencyProperty as the backing store for BoundScrollViewer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(SegmentedScrollBar),
                                        new PropertyMetadata(default(ScrollViewer), ScrollViewerChangedCallback));

        private readonly SegmentedScrollBarBehaviors _segmentedScrollBarBehaviors;
        private readonly SegmentedScrollBarDrawing   _segmentedScrollBarDrawing;

        public SegmentedScrollBar()
        {
            _segmentedScrollBarBehaviors = new SegmentedScrollBarBehaviors(this);
            _segmentedScrollBarDrawing   = new SegmentedScrollBarDrawing(this);

            ValueChanged += (_, _) => OnScroll(this, new ScrollEventArgs(ScrollEventType.ThumbTrack, Value));
        }

        public List<double>? SegmentBoundaries
        {
            get => (List<double>)GetValue(SegmentBoundariesProperty);
            set
            {
                SetValue(SegmentBoundariesProperty, value);
                _segmentedScrollBarBehaviors.SegmentBoundariesChanged();
            }
        }

        public List<Brush>? SegmentColors
        {
            get => (List<Brush>)GetValue(SegmentColorsProperty);
            set => SetValue(SegmentColorsProperty, value);
        }

        public bool DrawRegions
        {
            get => (bool)GetValue(DrawRegionsProperty);
            set => SetValue(DrawRegionsProperty, value);
        }

        public DelegateCommand PreviousSegmentCommand
        {
            get => (DelegateCommand)GetValue(PreviousSegmentCommandProperty);
            set => SetValue(PreviousSegmentCommandProperty, value);
        }

        public DelegateCommand NextSegmentCommand
        {
            get => (DelegateCommand)GetValue(NextSegmentCommandProperty);
            set => SetValue(NextSegmentCommandProperty, value);
        }

        public bool CanExecutePreviousCommand
        {
            get => (bool)GetValue(CanExecutePreviousCommandProperty);
            set => SetValue(CanExecutePreviousCommandProperty, value);
        }

        public bool CanExecuteNextCommand
        {
            get => (bool)GetValue(CanExecuteNextCommandProperty);
            set => SetValue(CanExecuteNextCommandProperty, value);
        }

        public ScrollViewer? ScrollViewer
        {
            get => (ScrollViewer)GetValue(ScrollViewerProperty);
            set => SetValue(ScrollViewerProperty, value);
        }

        private static void ScrollViewerChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is not SegmentedScrollBar scrollBar || args.NewValue == null) return;

            scrollBar.UpdateBindings();
        }

        private static void SegmentBoundariesChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is not SegmentedScrollBar scrollBar || args.NewValue == null) return;
            if (!(scrollBar.SegmentBoundaries is { })) return;

            scrollBar._segmentedScrollBarDrawing.DrawSegmentBoundaries();
            scrollBar._segmentedScrollBarBehaviors.SegmentBoundariesChanged();
        }

        private static void SegmentAppearanceChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is not SegmentedScrollBar scrollBar || args.NewValue == null) return;
            if (scrollBar.SegmentBoundaries is not { }) return;

            scrollBar._segmentedScrollBarDrawing.DrawSegmentBoundaries();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _segmentedScrollBarDrawing.OnApplyTemplate();
            _segmentedScrollBarBehaviors.OnApplyTemplate();
        }

        private void UpdateBindings()
        {
            AddHandler(ScrollEvent, new ScrollEventHandler(OnScroll));

            ScrollViewer?.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(BoundScrollChanged));
            Minimum = 0;
            if (Orientation == Orientation.Horizontal)
            {
                SetBinding(MaximumProperty,      new Binding("ScrollableWidth") {Source = ScrollViewer, Mode = BindingMode.OneWay});
                SetBinding(ViewportSizeProperty, new Binding("ViewportWidth") {Source   = ScrollViewer, Mode = BindingMode.OneWay});
            }
            else
            {
                SetBinding(MaximumProperty,      new Binding("ScrollableHeight") {Source = ScrollViewer, Mode = BindingMode.OneWay});
                SetBinding(ViewportSizeProperty, new Binding("ViewportHeight") {Source   = ScrollViewer, Mode = BindingMode.OneWay});
            }

            LargeChange = 242;
            SmallChange = 16;
        }

        private void BoundScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Value = Orientation == Orientation.Horizontal ? e.HorizontalOffset : e.VerticalOffset;
        }

        private void OnScroll(object sender, ScrollEventArgs e)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    ScrollViewer?.ScrollToHorizontalOffset(e.NewValue);
                    return;
                case Orientation.Vertical:
                    ScrollViewer?.ScrollToVerticalOffset(e.NewValue);
                    return;
            }
        }
    }
}