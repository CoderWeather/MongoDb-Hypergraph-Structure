﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace HyperGraphSharp.WPFExtensions.AttachedBehaviors
{
    public static class DragBehavior
    {
        #region PropertyChanged callbacks

        private static void OnIsDragEnabledPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var element = obj as FrameworkElement;
            FrameworkContentElement? contentElement = null;
            if (element == null)
            {
                contentElement = obj as FrameworkContentElement;
                if (contentElement == null)
                    return;
            }

            if (e.NewValue is bool == false)
                return;

            if ((bool) e.NewValue)
            {
                //register the event handlers
                if (element != null)
                {
                    //registering on the FrameworkElement
                    element.MouseLeftButtonDown += OnDragStarted;
                    element.MouseLeftButtonUp += OnDragFinished;
                }
                else
                {
                    //registering on the FrameworkContentElement
                    Debug.Assert(contentElement != null, nameof(contentElement) + " != null");
                    contentElement.MouseLeftButtonDown += OnDragStarted;
                    contentElement.MouseLeftButtonUp += OnDragFinished;
                }

                Debug.WriteLine("DragBehavior registered.", "WPFExt");
            }
            else
            {
                //unregister the event handlers
                if (element != null)
                {
                    //unregistering on the FrameworkElement
                    element.MouseLeftButtonDown -= OnDragStarted;
                    element.MouseLeftButtonUp -= OnDragFinished;
                }
                else
                {
                    //unregistering on the FrameworkContentElement
                    Debug.Assert(contentElement != null, nameof(contentElement) + " != null");
                    contentElement.MouseLeftButtonDown -= OnDragStarted;
                    contentElement.MouseLeftButtonUp -= OnDragFinished;
                }

                Debug.WriteLine("DragBehavior unregistered.", "WPFExt");
            }
        }

        #endregion

        private static void OnDragStarted(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as DependencyObject;
            //we are starting the drag
            Debug.Assert(obj != null, nameof(obj) + " != null");
            SetIsDragging(obj, true);

            var pos = e.GetPosition((IInputElement) obj);

            //save the position of the mouse to the start position
            SetOriginalX(obj, pos.X);
            SetOriginalY(obj, pos.Y);

            Debug.WriteLine("Drag started on object: " + obj, "WPFExt");

            //capture the mouse
            if (obj is FrameworkElement element)
            {
                element.CaptureMouse();
                element.MouseMove += OnDragging;
            }
            else
            {
                if (!(obj is FrameworkContentElement contentElement))
                    throw new ArgumentException(
                        "The control must be a descendent of the FrameworkElement or FrameworkContentElement!");
                contentElement.CaptureMouse();
                contentElement.MouseMove += OnDragging;
            }

            e.Handled = true;
        }

        private static void OnDragFinished(object sender, MouseButtonEventArgs e)
        {
            var obj = (DependencyObject) sender;
            SetIsDragging(obj, false);
            obj.ClearValue(OriginalXPropertyKey);
            obj.ClearValue(OriginalYPropertyKey);

            Debug.WriteLine("Drag finished on object: " + obj, "WPFExt");

            //we finished the drag, release the mouse
            if (sender is FrameworkElement element)
            {
                element.MouseMove -= OnDragging;
                element.ReleaseMouseCapture();
            }
            else
            {
                if (!(sender is FrameworkContentElement contentElement))
                    throw new ArgumentException(
                        "The control must be a descendent of the FrameworkElement or FrameworkContentElement!");
                contentElement.MouseMove -= OnDragging;
                contentElement.ReleaseMouseCapture();
            }

            e.Handled = true;
        }

        private static void OnDragging(object sender, MouseEventArgs e)
        {
            var obj = sender as DependencyObject;
            Debug.Assert(obj != null, nameof(obj) + " != null");
            if (!GetIsDragging(obj))
                return;

            var pos = e.GetPosition(obj as IInputElement);
            var horizontalChange = pos.X - GetOriginalX(obj);
            var verticalChange = pos.Y - GetOriginalY(obj);

            if (double.IsNaN(GetX(obj)))
                SetX(obj, 0);
            if (double.IsNaN(GetY(obj)))
                SetY(obj, 0);

            //move the object
            SetX(obj, GetX(obj) + horizontalChange);
            SetY(obj, GetY(obj) + verticalChange);

            e.Handled = true;
        }

        #region Attached DPs

        public static readonly DependencyProperty IsDragEnabledProperty =
            DependencyProperty.RegisterAttached("IsDragEnabled", typeof(bool), typeof(DragBehavior),
                new UIPropertyMetadata(false, OnIsDragEnabledPropertyChanged));

        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.RegisterAttached("IsDragging",
            typeof(bool), typeof(DragBehavior), new UIPropertyMetadata(false));

        public static readonly DependencyProperty XProperty = DependencyProperty.RegisterAttached("X", typeof(double),
            typeof(DragBehavior),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty YProperty = DependencyProperty.RegisterAttached("Y", typeof(double),
            typeof(DragBehavior),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private static readonly DependencyPropertyKey OriginalXPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("OriginalX", typeof(double), typeof(DragBehavior),
                new UIPropertyMetadata(0.0));

        private static readonly DependencyPropertyKey OriginalYPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("OriginalY", typeof(double), typeof(DragBehavior),
                new UIPropertyMetadata(0.0));

        #endregion

        #region Get/Set method for Attached Properties

        public static bool GetIsDragEnabled(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsDragEnabledProperty);
        }

        public static void SetIsDragEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragEnabledProperty, value);
        }

        public static bool GetIsDragging(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsDraggingProperty);
        }

        public static void SetIsDragging(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDraggingProperty, value);
        }

        public static double GetX(DependencyObject obj)
        {
            return (double) obj.GetValue(XProperty);
        }

        public static void SetX(DependencyObject obj, double value)
        {
            obj.SetValue(XProperty, value);
        }

        public static double GetY(DependencyObject obj)
        {
            return (double) obj.GetValue(YProperty);
        }

        public static void SetY(DependencyObject obj, double value)
        {
            obj.SetValue(YProperty, value);
        }

        private static double GetOriginalX(DependencyObject obj)
        {
            return (double) obj.GetValue(OriginalXPropertyKey.DependencyProperty);
        }

        private static void SetOriginalX(DependencyObject obj, double value)
        {
            obj.SetValue(OriginalXPropertyKey, value);
        }

        private static double GetOriginalY(DependencyObject obj)
        {
            return (double) obj.GetValue(OriginalYPropertyKey.DependencyProperty);
        }

        private static void SetOriginalY(DependencyObject obj, double value)
        {
            obj.SetValue(OriginalYPropertyKey, value);
        }

        #endregion
    }
}