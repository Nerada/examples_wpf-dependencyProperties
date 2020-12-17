// -----------------------------------------------
//     Author: Ramon Bollen
//      File: DependencyProperties.SegmentedScrollBarDrawing.cs
// Created on: 20201216
// -----------------------------------------------

using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DependencyProperties.Resources.ExtendedControls
{
    public class SegmentedScrollBarDrawing
    {
        private readonly SegmentedScrollBar _scrollBar;

        public SegmentedScrollBarDrawing(SegmentedScrollBar scrollBar)
        {
            _scrollBar = scrollBar;
        }

        private Canvas? TopCanvas { get; set; }

        private Canvas? BottomCanvas { get; set; }

        private List<double> SegmentBoundaries => _scrollBar.SegmentBoundaries ?? new List<double>();

        private List<Brush> SegmentColors => _scrollBar.SegmentColors ?? new List<Brush>();

        private bool DrawRegions => _scrollBar.DrawRegions;

        public void OnApplyTemplate()
        {
            if (TopCanvas    != null                                                                  ||
                BottomCanvas != null                                                                  ||
                _scrollBar.Template.FindName("PART_TopCanvas",    _scrollBar) is not Canvas topCanvas ||
                _scrollBar.Template.FindName("PART_BottomCanvas", _scrollBar) is not Canvas bottomCanvas)
            {
                return;
            }

            TopCanvas    = topCanvas;
            BottomCanvas = bottomCanvas;

            _scrollBar.SizeChanged += (_, _) => DrawSegmentBoundaries();
        }

        public void DrawSegmentBoundaries()
        {
            if (TopCanvas == null || BottomCanvas == null) return;

            TopCanvas?.Children.Clear();
            BottomCanvas?.Children.Clear();

            var drawBoundaries = new List<double>(SegmentBoundaries);

            if (DrawRegions)
            {
                drawBoundaries.Insert(0, 0);
                drawBoundaries.Add(_scrollBar.Track.Maximum + _scrollBar.Track.ViewportSize);
            }

            for (var index = 0; index < drawBoundaries.Count; index++)
            {
                double segmentBoundary = drawBoundaries[index];
                Brush  color           = SegmentColors == null || index > SegmentColors.Count - 1 ? new SolidColorBrush(Colors.OrangeRed) : SegmentColors[index];

                if (DrawRegions && index + 1 > drawBoundaries.Count - 1) return;

                double size     = DrawRegions ? CalculatePixelBoundaryPosition(drawBoundaries[index + 1]) - CalculatePixelBoundaryPosition(drawBoundaries[index]) : 2;
                double position = DrawRegions ? CalculatePixelBoundaryPosition(drawBoundaries[index]) : CalculatePixelBoundaryPosition(segmentBoundary);

                if (double.IsInfinity(position)) return;

                DrawBoundary(size, position, color);
            }
        }

        private double CalculatePixelBoundaryPosition(double segmentBoundary)
        {
            double fullBar = _scrollBar.Track.Maximum + _scrollBar.Track.ViewportSize;

            double relativeBoundaryPosition = segmentBoundary / fullBar;

            return relativeBoundaryPosition * (_scrollBar.Orientation == Orientation.Horizontal ? _scrollBar.Track.ActualWidth : _scrollBar.Track.ActualHeight);
        }

        private void DrawBoundary(double size, double position, Brush color)
        {
            var rect = new Rectangle()
            {
                Width  = _scrollBar.Orientation == Orientation.Horizontal ? size : _scrollBar.Track.ActualWidth,
                Height = _scrollBar.Orientation == Orientation.Horizontal ? _scrollBar.Track.ActualHeight : size,
                Fill   = color,

                StrokeThickness = 2
            };

            if (DrawRegions)
                BottomCanvas?.Children.Add(rect);
            else
                TopCanvas?.Children.Add(rect);

            if (_scrollBar.Orientation == Orientation.Horizontal)
                Canvas.SetLeft(rect, position);
            else
                Canvas.SetTop(rect, position);
        }
    }
}