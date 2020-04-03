﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Petzold.GetMedieval
{
    public class MedievalButton : Control
    {
        FormattedText formtxt; //элемент управления для рисования текста
        bool isMouseReallyOver;

        public static readonly DependencyProperty TextProperty; //свойство (стили, наследование)
        public static readonly RoutedEvent KnockEvent; //идентификация (событие и его характеристики) и ниже
        public static readonly RoutedEvent PreviewKnockEvent;

        static MedievalButton()
        {
            // запись свойства зависимости
            TextProperty =
            DependencyProperty.Register("Text", typeof(string),
            typeof(MedievalButton),
            new FrameworkPropertyMetadata(" ", FrameworkPropertyMetadataOptions.AffectsMeasure));

            // запись routed events.
            KnockEvent =
            EventManager.RegisterRoutedEvent("Knock", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(MedievalButton));

            PreviewKnockEvent =
            EventManager.RegisterRoutedEvent("PreviewKnock",
            RoutingStrategy.Tunnel,
            typeof(RoutedEventHandler), typeof(MedievalButton));
        }
        //открытый интерфейс к свойству зависимости
        public string Text
        {
            set { SetValue(TextProperty, value == null ? " " : value); }
            get { return (string)GetValue(TextProperty); }
        }
        // открытый интерфейс к routed events.
        public event RoutedEventHandler Knock
        {
            add { AddHandler(KnockEvent, value); }
            remove { RemoveHandler(KnockEvent, value); }
        }
        public event RoutedEventHandler PreviewKnock
        {
            add { AddHandler(PreviewKnockEvent, value); }
            remove { RemoveHandler(PreviewKnockEvent, value); }
        }
        //MeasureOverride вызывается, когда размер кнопки может измениться
        protected override Size MeasureOverride(Size sizeAvailable)
        {
            formtxt = new FormattedText(
            Text, CultureInfo.CurrentCulture, FlowDirection,
            new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
            FontSize, Foreground);

            //учитывать Padding при расчете размера 
            Size sizeDesired = new Size(Math.Max(48, formtxt.Width) + 4,
            formtxt.Height + 4);
            sizeDesired.Width += Padding.Left + Padding.Right;
            sizeDesired.Height += Padding.Top + Padding.Bottom;

            return sizeDesired;
        }
        // OnRender вызывается, чтобы перерисовать кнопку 
        protected override void OnRender(DrawingContext dc)
        {
            //определение цвета фона
            Brush brushBackground = SystemColors.ControlBrush;

            if (isMouseReallyOver && IsMouseCaptured)
                brushBackground = SystemColors.ControlDarkBrush;

            //определить ширину пера
            Pen pen = new Pen(Foreground, IsMouseOver ? 2 : 1);

            //нарисовать заполненный прямоугольник с закругленными углами
            dc.DrawRoundedRectangle(brushBackground, pen,
            new Rect(new Point(0, 0), RenderSize), 4, 4);

            //определение цвета переднего плана 
            formtxt.SetForegroundBrush(
            IsEnabled ? Foreground : SystemColors.ControlDarkBrush);

            //определение начала текста 
            Point ptText = new Point(2, 2);

            switch (HorizontalContentAlignment)
            {
                case HorizontalAlignment.Left:
                    ptText.X += Padding.Left;
                    break;

                case HorizontalAlignment.Right:
                    ptText.X += RenderSize.Width - formtxt.Width - Padding.Right;
                    break;

                case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch:
                    ptText.X += (RenderSize.Width - formtxt.Width -
                    Padding.Left - Padding.Right) / 2;
                    break;
            }
            switch (VerticalContentAlignment)
            {
                case VerticalAlignment.Top:
                    ptText.Y += Padding.Top;
                    break;

                case VerticalAlignment.Bottom:
                    ptText.Y +=
                    RenderSize.Height - formtxt.Height - Padding.Bottom;
                    break;

                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                    ptText.Y += (RenderSize.Height - formtxt.Height -
                    Padding.Top - Padding.Bottom) / 2;
                    break;
            }

            //рисование текста
            dc.DrawText(formtxt, ptText);
        }
        protected override void OnMouseEnter(MouseEventArgs args)
        {
            base.OnMouseEnter(args);
            InvalidateVisual();
        }
        protected override void OnMouseLeave(MouseEventArgs args)
        {
            base.OnMouseLeave(args);
            InvalidateVisual();
        }
        protected override void
            OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args); //определить, если мышка сдвинулась внутрь или наружу           
            Point pt = args.GetPosition(this);
            bool isReallyOverNow = (pt.X >= 0 &&  pt.X < ActualWidth &&  pt.Y >= 0 &&  pt.Y < ActualHeight);
            if (isReallyOverNow != isMouseReallyOver)             {
                isMouseReallyOver = isReallyOverNow;
                InvalidateVisual();
            }
        }         //начало запуска Knock.        
        protected override void  OnMouseLeftButtonDown(MouseButtonEventArgs args)         {
            base.OnMouseLeftButtonDown(args);
            CaptureMouse();
            InvalidateVisual();
            args.Handled = true;
        }        // это запускает Knock  
        protected override void  OnMouseLeftButtonUp(MouseButtonEventArgs args)         {
            base.OnMouseLeftButtonUp(args);
            if (IsMouseCaptured)             {
                if (isMouseReallyOver)                 {
                    OnPreviewKnock();
                    OnKnock();
                }
                args.Handled = true;
                Mouse.Capture(null);
            }
        }         //если потерять захват мыши (внутри или снаружи), перерисовать      
        protected override void OnLostMouseCapture (MouseEventArgs args)         {
            base.OnLostMouseCapture(args);
            InvalidateVisual();
        }         //пробел на клавиатуре или Enter так же запускает кнопку       
        protected override void OnKeyDown (KeyEventArgs args)         {
            base.OnKeyDown(args);
            if (args.Key == Key.Space || args.Key  == Key.Enter)
                args.Handled = true;
        }
        protected override void OnKeyUp (KeyEventArgs args)         {
            base.OnKeyUp(args);
            if (args.Key == Key.Space || args.Key  == Key.Enter)             {
                OnPreviewKnock();
                OnKnock();
                args.Handled = true;
            }

        }         //метод OnKnock вызывает событие Knock     
        protected virtual void OnKnock()         {
            RoutedEventArgs argsEvent = new  RoutedEventArgs();
            argsEvent.RoutedEvent = MedievalButton .PreviewKnockEvent;
            argsEvent.Source = this;
            RaiseEvent(argsEvent);
        }         //метод OnPreviewKnock вызывает событие PreviewKnock    
        protected virtual void OnPreviewKnock()         {
            RoutedEventArgs argsEvent = new  RoutedEventArgs();
            argsEvent.RoutedEvent = MedievalButton .KnockEvent;
            argsEvent.Source = this;
            RaiseEvent(argsEvent);
        }
    }
} 
