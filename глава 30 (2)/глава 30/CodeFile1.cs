﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
namespace Petzold.EnlargeButtonWithTimer
{
    public class EnlargeButtonWithTimer : Window // объявление класса
    {
        const double initFontSize = 12;
        const double maxFontSize = 48;
        Button btn;
        [STAThread]
        public static void Main()
        {
            Application app = new Application(); // инициализация объекта
            app.Run(new EnlargeButtonWithTimer()); // создание кнопки
        }
        public EnlargeButtonWithTimer()
        {
            Title = "Enlarge Button with Timer";
            btn = new Button();
            btn.Content = "Expanding Button";
            btn.FontSize = initFontSize;
            btn.HorizontalAlignment = HorizontalAlignment.Center; // горизонтальное выравнивание
            btn.VerticalAlignment = VerticalAlignment.Center; // вертикальное выравнивание 
            btn.Click += ButtonOnClick; Content = btn;
        }
        void ButtonOnClick(object sender, RoutedEventArgs args) // описание кнопки 
        {
            DispatcherTimer tmr = new DispatcherTimer(); // таймер
            tmr.Interval = TimeSpan.FromSeconds(0.1);
            tmr.Tick += TimerOnTick; tmr.Start();
        }
        void TimerOnTick(object sender, EventArgs args)
        {
            btn.FontSize += 2;
            if (btn.FontSize >= maxFontSize)
            {
                btn.FontSize = initFontSize;
                (sender as DispatcherTimer).Stop();
            }
        }
    }
} 
