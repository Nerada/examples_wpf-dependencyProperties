// -----------------------------------------------
//     Author: Ramon Bollen
//      File: DependencyProperties.SegmentedScrollBar.cs
// Created on: 20201208
// -----------------------------------------------

using System;
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

        private readonly SegmentedScrollBarSegmentDrawing _segmentDrawing;

        private bool _thumbDragging;

        public SegmentedScrollBar()
        {
            MouseEnter += (_, _) => NavigationCanExecuteChanged();
            MouseLeave += (_, _) => NavigationCanExecuteChanged();
            ValueChanged += (_, _) =>
            {
                NavigationCanExecuteChanged();
                SegmentNavigationCanExecuteChanged();
            };
            ValueChanged += (_, _) => JumpOffSegmentBoundary();

            _segmentDrawing = new SegmentedScrollBarSegmentDrawing(this);

            PreviousSegmentCommand = new DelegateCommand(() => OnButtonClick(ButtonType.LeftSegmentButton),  () => CanExecutePreviousSegmentCommand);
            NextSegmentCommand     = new DelegateCommand(() => OnButtonClick(ButtonType.RightSegmentButton), () => CanExecuteNextSegmentCommand);
        }

        public List<double>? SegmentBoundaries
        {
            get => (List<double>)GetValue(SegmentBoundariesProperty);
            set
            {
                SetValue(SegmentBoundariesProperty, value);
                SegmentNavigationCanExecuteChanged();
            }
        }

        public DelegateCommand PreviousSegmentCommand
        {
            get => (DelegateCommand)GetValue(PreviousSegmentCommandProperty);
            private init => SetValue(PreviousSegmentCommandProperty, value);
        }

        public DelegateCommand NextSegmentCommand
        {
            get => (DelegateCommand)GetValue(NextSegmentCommandProperty);
            private init => SetValue(NextSegmentCommandProperty, value);
        }

        public bool CanExecutePreviousCommand
        {
            get => (bool)GetValue(CanExecutePreviousCommandProperty);
            private set => SetValue(CanExecutePreviousCommandProperty, value);
        }

        public bool CanExecuteNextCommand
        {
            get => (bool)GetValue(CanExecuteNextCommandProperty);
            private set => SetValue(CanExecuteNextCommandProperty, value);
        }

        private bool CanExecutePreviousSegmentCommand => Boundaries.Count != 0 && Value >= Boundaries[0];

        private bool CanExecuteNextSegmentCommand => Boundaries.Count != 0 && Value < Boundaries[^1];

        private List<double> Boundaries => SegmentBoundaries ?? new List<double>();

        private static void SegmentBoundariesChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            (dependencyObject as SegmentedScrollBar)?.UpdateBoundaries();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_segmentDrawing.ScrollBarCanvas != null || !(Template.FindName(@"PART_Canvas", this) is Canvas canvas))
            {
                return;
            }

            Track.Thumb.GotMouseCapture  += (_, _) => _thumbDragging = true;
            Track.Thumb.GotStylusCapture += (_, _) => _thumbDragging = true;
            Track.Thumb.GotTouchCapture  += (_, _) => _thumbDragging = true;

            Track.Thumb.LostMouseCapture += (_, _) =>
            {
                _thumbDragging = false;
                JumpOffSegmentBoundary();
            };
            Track.Thumb.LostStylusCapture += (_, _) =>
            {
                _thumbDragging = false;
                JumpOffSegmentBoundary();
            };
            Track.Thumb.LostTouchCapture += (_, _) =>
            {
                _thumbDragging = false;
                JumpOffSegmentBoundary();
            };

            _segmentDrawing.ScrollBarCanvas = canvas;

            SizeChanged += (_, _) => _segmentDrawing.DrawSegmentBoundaries(Boundaries);
        }

        private void SegmentNavigationCanExecuteChanged()
        {
            PreviousSegmentCommand.RaiseCanExecuteChanged();
            NextSegmentCommand.RaiseCanExecuteChanged();
        }

        private void NavigationCanExecuteChanged()
        {
            CanExecutePreviousCommand = IsMouseOver && Math.Abs(Value   - Minimum) > double.Epsilon;
            CanExecuteNextCommand     = IsMouseOver && Math.Abs(Maximum - Value)   > double.Epsilon;
        }

        private void UpdateBoundaries()
        {
            if (!(SegmentBoundaries is { }))
            {
                return;
            }

            JumpOffSegmentBoundary();
            _segmentDrawing.DrawSegmentBoundaries(SegmentBoundaries);
        }

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
            if (!(segmentValue is { } segValue) || segValue == 0)
            {
                return;
            }

            Value = buttonType == ButtonType.LeftSegmentButton ? segValue - ViewportSize : segValue;
        }

        /// <summary>
        ///     Check if ScrollBar thumb is at a segment boundary. Introduce jumping behaviour.
        /// </summary>
        private void JumpOffSegmentBoundary()
        {
            if (_thumbDragging) return;

            double? boundaryValue = Boundaries.Find(segment => segment > Value && segment < Value + ViewportSize);

            if (!(boundaryValue is { } boundary) || boundary == 0) return;

            double halfThumbValue = Value + ViewportSize / 2;

            // Jump to the left or right of a segment boundary
            Value = halfThumbValue < boundary ? boundary - ViewportSize : boundary;
        }

        private enum ButtonType
        {
            LeftSegmentButton,
            RightSegmentButton
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
                if (ScrollBarCanvas == null)
                {
                    return;
                }

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
                new()
                {
                    Stroke          = new SolidColorBrush(Colors.OrangeRed),
                    X1              = _scrollBar.Orientation == Orientation.Horizontal ? pixelPosition : 0,
                    X2              = _scrollBar.Orientation == Orientation.Horizontal ? pixelPosition : _scrollBar.Track.ActualWidth,
                    Y1              = _scrollBar.Orientation == Orientation.Horizontal ? 0 : pixelPosition,
                    Y2              = _scrollBar.Orientation == Orientation.Horizontal ? _scrollBar.Track.ActualHeight : pixelPosition,
                    StrokeThickness = 2
                };
        }
    }
}