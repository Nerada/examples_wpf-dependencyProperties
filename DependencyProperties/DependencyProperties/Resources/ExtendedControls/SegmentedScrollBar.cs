﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using Prism.Commands;

namespace DependencyProperties.Resources.ExtendedControls
{
    public class SegmentedScrollBar : ScrollBar
    {
        private enum ButtonType
        {
            LeftSegmentButton,
            RightSegmentButton,
        }

        private readonly SegmentedScrollBarSegmentDrawing _segmentDrawing;

        public SegmentedScrollBar()
        {
            //Scroll += (_, __) => CheckSegmentBoundaries();

            MouseEnter += (_, __) => CheckNavigation();
            MouseLeave += (_, __) => CheckNavigation();
            ValueChanged += (_, __) => CheckNavigation();

            _segmentDrawing = new SegmentedScrollBarSegmentDrawing(this);

            PreviousSegmentCommand = new DelegateCommand(() => OnButtonClick(ButtonType.LeftSegmentButton),  CanExecutePreviousSegmentCommand);
            NextSegmentCommand     = new DelegateCommand(() => OnButtonClick(ButtonType.RightSegmentButton), CanExecuteNextSegmentCommand);
        }

        public static readonly DependencyProperty CanExecutePreviousCommandProperty =
            DependencyProperty.Register("CanExecutePreviousCommand", typeof(bool), typeof(SegmentedScrollBar));

        public static readonly DependencyProperty CanExecuteNextCommandProperty =
            DependencyProperty.Register("CanExecuteNextCommand", typeof(bool), typeof(SegmentedScrollBar));

        public static readonly DependencyProperty SegmentBoundariesProperty =
            DependencyProperty.Register("SegmentBoundaries", typeof(List<double>), typeof(SegmentedScrollBar),
                                        new PropertyMetadata(default(List<double>), SegmentBoundariesChangedCallback));

        public static readonly DependencyProperty PreviousSegmentCommandProperty =
            DependencyProperty.Register("PreviousSegmentCommand", typeof(DelegateCommand), typeof(SegmentedScrollBar));

        public static readonly DependencyProperty NextSegmentCommandProperty =
            DependencyProperty.Register("NextSegmentCommand", typeof(DelegateCommand), typeof(SegmentedScrollBar));

        private static void SegmentBoundariesChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            (dependencyObject as SegmentedScrollBar)?.UpdateBoundaries();
        }

        public List<double>? SegmentBoundaries
        {
            get => (List<double>)GetValue(SegmentBoundariesProperty);
            set => SetValue(SegmentBoundariesProperty, value);
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

        private List<double> Boundaries => SegmentBoundaries ?? new List<double>();

        private new double Value
        {
            get => (double)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
                RaiseAllCanExecuteChanged();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_segmentDrawing.ScrollBarCanvas != null || !(Template.FindName(@"PART_Canvas", this) is Canvas canvas)) return;

            Track.Thumb.LostMouseCapture += (_, __) => CheckSegmentBoundaries();
            Track.Thumb.LostStylusCapture += (_, __) => CheckSegmentBoundaries();
            Track.Thumb.LostTouchCapture += (_, __) => CheckSegmentBoundaries();

            _segmentDrawing.ScrollBarCanvas = canvas;

            SizeChanged += (sender, args) => _segmentDrawing.DrawSegmentBoundaries(Boundaries);
        }

        private void RaiseAllCanExecuteChanged()
        {
            PreviousSegmentCommand.RaiseCanExecuteChanged();
            NextSegmentCommand.RaiseCanExecuteChanged();
        }

        private void CheckNavigation()
        {
            CanExecutePreviousCommand = IsMouseOver && Math.Abs(Value - Minimum) > double.Epsilon;
            CanExecuteNextCommand = IsMouseOver && Math.Abs(Maximum - Value) > double.Epsilon;
        }

        private void UpdateBoundaries()
        {
            if (!(SegmentBoundaries is {})) return;

            CheckSegmentBoundaries();
            _segmentDrawing.DrawSegmentBoundaries(SegmentBoundaries);
        }

        private bool CanExecutePreviousSegmentCommand() => Boundaries.Count != 0 && Value >= Boundaries[0];

        private bool CanExecuteNextSegmentCommand() => Boundaries.Count != 0 && Value < Boundaries[^1];

        private void OnButtonClick(ButtonType buttonType)
        {
            // check if it comes from the right or left button, depending on that, switch to right or left segment.
            // If we go to the left direction - set scrollbar value to the end of the previous segment.
            // If the direction is to the right - to the beginning of the next segment.

            double? segmentValue;

            // Get current segment
            switch (buttonType)
            {
                case ButtonType.LeftSegmentButton:
                    segmentValue = Boundaries.LastOrDefault(b => b <= Value);
                    break;
                case ButtonType.RightSegmentButton:
                    segmentValue = Boundaries.FirstOrDefault(b => b > Value);
                    break;
                default:
                    throw new ArgumentException(@$"{nameof(OnButtonClick)}: + Unsupported button type used.");
            }

            // Check if there is no right/left
            if (!(segmentValue is {} segValue) || segValue == 0) return;

            Value = buttonType == ButtonType.LeftSegmentButton ? segValue - ViewportSize : segValue;
        }

        /// <summary>
        /// Check if ScrollBar thumb is at a segment boundary. Introduce jumping behaviour.
        /// </summary>
        private void CheckSegmentBoundaries()
        {
            RaiseAllCanExecuteChanged();

            double? boundaryValue = Boundaries.Find(s => s > Value && s < (Value + ViewportSize));

            if (!(boundaryValue is {} boundary) || boundary == 0) return;

            double halfThumbValue = Value + (ViewportSize / 2);

            // Jump to the left or right of a segment boundary
            Value = halfThumbValue < boundary ? boundary - ViewportSize : boundary;
        }

        private class SegmentedScrollBarSegmentDrawing
        {
            private readonly SegmentedScrollBar _scrollBar;

            public SegmentedScrollBarSegmentDrawing(SegmentedScrollBar scrollBar)
            {
                _scrollBar = scrollBar;
            }

            public Canvas? ScrollBarCanvas { get; set; }

            public void DrawSegmentBoundaries(List<double> segmentBoundaries)
            {
                if (ScrollBarCanvas == null) return;

                ScrollBarCanvas?.Children.Clear();

                foreach (double segmentBoundary in segmentBoundaries)
                {
                    ScrollBarCanvas?.Children.Add(CreateBoundaryLine(CalculatePixelBoundaryPosition(segmentBoundary)));
                }
            }

            private double CalculatePixelBoundaryPosition(double segmentBoundary)
            {
                double fullBar = _scrollBar.Track.Maximum + _scrollBar.Track.ViewportSize;

                double relativeBoundaryPosition = segmentBoundary / fullBar;

                return relativeBoundaryPosition * (_scrollBar.Orientation == Orientation.Horizontal ? _scrollBar.Track.ActualWidth : _scrollBar.Track.ActualHeight);
            }

            private Line CreateBoundaryLine(double pixelPosition) =>
                new Line
                {
                    Stroke          = new SolidColorBrush(Colors.OrangeRed),
                    X1              = _scrollBar.Orientation == Orientation.Horizontal ? pixelPosition : 0,
                    X2              = _scrollBar.Orientation == Orientation.Horizontal ? pixelPosition : _scrollBar.Track.ActualWidth,
                    Y1              = _scrollBar.Orientation == Orientation.Horizontal ? 0 : pixelPosition,
                    Y2              = _scrollBar.Orientation == Orientation.Horizontal ? _scrollBar.Track.ActualHeight : pixelPosition,
                    StrokeThickness = 2,
                };
        }
    }
}