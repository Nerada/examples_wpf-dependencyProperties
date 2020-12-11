// -----------------------------------------------
//     Author: Ramon Bollen
//      File: DependencyProperties.ExtendedButton.cs
// Created on: 20201201
// -----------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DependencyProperties.Resources.ExtendedControls
{
    /// <summary>
    /// Based on: https://stackoverflow.com/questions/815797/add-dependency-property-to-control
    /// </summary>
    public sealed class ExtendedButton : Button
    {
        /// <summary>
        /// Foreground color extension
        /// </summary>
        public new static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(ExtendedButton), new PropertyMetadata(default(Brush), ForegroundColorChangedCallback));

        public static readonly DependencyProperty HasForegroundColorProperty =
            DependencyProperty.Register("HasForegroundColor", typeof(bool), typeof(ExtendedButton), new PropertyMetadata(false));

        public bool HasForegroundColor => (bool)GetValue(HasForegroundColorProperty);

        public new Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        private static void ForegroundColorChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            (dependencyObject as ExtendedButton)?.SetValue(HasForegroundColorProperty, true);
        }

        /// <summary>
        /// Background color extension
        /// </summary>
        public new static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ExtendedButton), new PropertyMetadata(default(Brush), BackgroundColorChangedCallback));

        public static readonly DependencyProperty HasBackgroundColorProperty =
            DependencyProperty.Register("HasBackgroundColor", typeof(bool), typeof(ExtendedButton), new PropertyMetadata(false));

        public bool HasBackgroundColor => (bool)GetValue(HasBackgroundColorProperty);

        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        private static void BackgroundColorChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            (dependencyObject as ExtendedButton)?.SetValue(HasBackgroundColorProperty, true);
        }

        /// <summary>
        /// Background hover color extension
        /// </summary>
        public static readonly DependencyProperty HasBackgroundHoverColorProperty =
            DependencyProperty.Register("HasBackgroundHoverColor", typeof(bool), typeof(ExtendedButton), new PropertyMetadata(false));

        public static readonly DependencyProperty BackgroundHoverColorProperty =
            DependencyProperty.Register("BackgroundHoverColor", typeof(Brush), typeof(ExtendedButton), new PropertyMetadata(default(Brush), BackgroundHoverColorChangedCallback));

        public bool HasBackgroundHoverColor => (bool)GetValue(HasBackgroundHoverColorProperty);

        public Brush BackgroundHoverColor
        {
            get => (Brush)GetValue(BackgroundHoverColorProperty);
            set => SetValue(BackgroundHoverColorProperty, value);
        }

        private static void BackgroundHoverColorChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            (dependencyObject as ExtendedButton)?.SetValue(HasBackgroundHoverColorProperty, true);
        }

        /// <summary>
        /// Foreground hover color extension
        /// </summary>
        public static readonly DependencyProperty HasForegroundHoverColorProperty =
            DependencyProperty.Register("HasForegroundHoverColor", typeof(bool), typeof(ExtendedButton), new PropertyMetadata(false));

        public static readonly DependencyProperty ForegroundHoverColorProperty =
            DependencyProperty.Register("ForegroundHoverColor", typeof(Brush), typeof(ExtendedButton), new PropertyMetadata(default(Brush), ForegroundHoverColorChangedCallback));

        public bool HasForegroundHoverColor => (bool)GetValue(HasForegroundHoverColorProperty);

        public Brush ForegroundHoverColor
        {
            get => (Brush)GetValue(ForegroundHoverColorProperty);
            set => SetValue(ForegroundHoverColorProperty, value);
        }

        private static void ForegroundHoverColorChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            (dependencyObject as ExtendedButton)?.SetValue(HasForegroundHoverColorProperty, true);
        }

        /// <summary>
        /// Image extension
        /// </summary>
        public static readonly DependencyProperty HasImageProperty =
            DependencyProperty.Register("HasImage", typeof(bool), typeof(ExtendedButton), new PropertyMetadata(false));

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(DrawingImage), typeof(ExtendedButton), new PropertyMetadata(default(DrawingImage), ImageChangedCallback));

        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(ExtendedButton), new PropertyMetadata(default(Thickness), ImageChangedCallback));

        public bool HasImage => (bool)GetValue(HasImageProperty);

        public DrawingImage Image
        {
            get => (DrawingImage)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        private static void ImageChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            (dependencyObject as ExtendedButton)?.SetValue(HasImageProperty, true);
        }

        public Thickness ImageMargin
        {
            get => (Thickness)GetValue(ImageMarginProperty);
            set => SetValue(ImageMarginProperty, value);
        }

        /// <summary>
        /// Image hover extension
        /// </summary>
        public static readonly DependencyProperty HasImageHoverProperty =
            DependencyProperty.Register("HasImageHover", typeof(bool), typeof(ExtendedButton), new PropertyMetadata(false));

        public static readonly DependencyProperty ImageHoverProperty =
            DependencyProperty.Register("ImageHover", typeof(DrawingImage), typeof(ExtendedButton), new PropertyMetadata(default(DrawingImage), ImageHoverChangedCallback));

        public bool HasImageHover => (bool)GetValue(HasImageHoverProperty);

        public DrawingImage ImageHover
        {
            get => (DrawingImage)GetValue(ImageHoverProperty);
            set => SetValue(ImageHoverProperty, value);
        }

        private static void ImageHoverChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            (dependencyObject as ExtendedButton)?.SetValue(HasImageHoverProperty, true);
        }

        /// <summary>
        /// Additional content extension
        /// </summary>
        public static readonly DependencyProperty Content2Property =
            DependencyProperty.Register("Content2", typeof(string), typeof(ExtendedButton), new PropertyMetadata(default(string), Content2ChangedCallBack));

        public string Content2
        {
            set => SetValue(Content2Property, value);
        }

        private static void Content2ChangedCallBack(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (!(dependencyObject as ExtendedButton is {} extendedButton)) return;

            extendedButton.Content += (string)args.NewValue;
        }
    }
}