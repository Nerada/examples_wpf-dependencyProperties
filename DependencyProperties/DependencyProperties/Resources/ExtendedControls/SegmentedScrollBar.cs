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
        private enum ButtonType
        {
            LeftButton,
            RightButton,
        }

        private readonly SegmentedScrollBarSegmentDrawing _segmentDrawing;

        private List<double> _segmentBoundaries = new List<double>();

        public SegmentedScrollBar()
        {
            Scroll += (sender, args) => OnScroll();

            _segmentDrawing = new SegmentedScrollBarSegmentDrawing(this);

            PreviousSegmentCommand = new DelegateCommand(() => OnButtonClick(ButtonType.LeftButton),  CanExecutePreviousSegmentCommand);
            NextSegmentCommand     = new DelegateCommand(() => OnButtonClick(ButtonType.RightButton), CanExecuteNextSegmentCommand);
        }

        public static readonly DependencyProperty SegmentBoundariesProperty =
            DependencyProperty.Register("SegmentBoundaries", typeof(DelegateCommand), typeof(SegmentedScrollBar));

        public static readonly DependencyProperty PreviousSegmentCommandProperty =
            DependencyProperty.Register("PreviousSegmentCommand", typeof(DelegateCommand), typeof(SegmentedScrollBar));

        public static readonly DependencyProperty NextSegmentCommandProperty =
            DependencyProperty.Register("NextSegmentCommand", typeof(DelegateCommand), typeof(SegmentedScrollBar));

        public List<double> SegmentBoundaries
        {
            get => _segmentBoundaries;
            set
            {
                _segmentBoundaries = value;
                CheckSegmentBoundaries();
            }
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

        private bool CanExecutePreviousSegmentCommand() => Value > 0;

        private bool CanExecuteNextSegmentCommand() => SegmentBoundaries.Count != 0 && SegmentBoundaries[^1] > Value;

        private void OnButtonClick(ButtonType buttonType)
        {
            // check if it comes from the right or left button, depending on that, switch to right or left segment.
            // If we go to the left direction - set scrollbar value to the end of the previous segment.
            // If the direction is to the right - to the beginning of the next segment.

            // Get current segment
            int segmentIndex = SegmentBoundaries.IndexOf(SegmentBoundaries.FirstOrDefault(b => b > Value));
            switch (buttonType)
            {
                case ButtonType.LeftButton:
                    --segmentIndex;
                    break;
                case ButtonType.RightButton:
                    break;
                default:
                    throw new ArgumentException(@$"{nameof(OnButtonClick)}: + Unsupported button type used.");
            }

            // Check if there is no right/left
            if (segmentIndex < 0 || segmentIndex >= SegmentBoundaries.Count) return;

            double boundaryValue = SegmentBoundaries[segmentIndex];

            Value = buttonType == ButtonType.LeftButton ? boundaryValue - ViewportSize : boundaryValue;
        }

        private new double Value
        {
            get => (double)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
                PreviousSegmentCommand.RaiseCanExecuteChanged();
                NextSegmentCommand.RaiseCanExecuteChanged();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_segmentDrawing.ScrollBarCanvas != null || !(Template.FindName(@"PART_Canvas", this) is Canvas canvas)) return;

            _segmentDrawing.ScrollBarCanvas = canvas;

            SizeChanged += (sender, args) => _segmentDrawing.DrawSegmentBoundaries(SegmentBoundaries);
        }

        /// <summary>
        /// Scroll segment jump behavior
        /// </summary>
        private void OnScroll()
        {
            CheckSegmentBoundaries();
        }

        /// <summary>
        /// Check if ScrollBar thumb is at a segment boundary. Introduce jumping behaviour.
        /// </summary>
        private void CheckSegmentBoundaries()
        {
            double? boundaryValue = SegmentBoundaries.Find(s => s > Value && s < (Value + ViewportSize));

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

            public void DrawSegmentBoundaries(IReadOnlyList<double> segmentBoundaries)
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

                return relativeBoundaryPosition * _scrollBar.Track.ActualWidth;
            }

            private Line CreateBoundaryLine(double pixelPosition) =>
                new Line
                {
                    Stroke          = new SolidColorBrush(Colors.OrangeRed),
                    X1              = pixelPosition,
                    X2              = pixelPosition,
                    Y1              = 0,
                    Y2              = _scrollBar.Track.ActualHeight,
                    StrokeThickness = 2,
                };
        }
    }
}