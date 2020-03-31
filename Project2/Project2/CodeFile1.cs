﻿using System;
using System.Windows;
using System.Windows.Input;
namespace Petzold.HandleAnEvent
{
    class HandleAnEvent
    {
        [STAThread]
        public static void Main() 
        {
            Application app = new Application(); // инициализация объекта 
            Window win = new Window(); // инициализация нового экземпляра класса (создание окон)
            win.Title = "Handle An Event";
            win.MouseDown += WindowOnMouseDown; app.Run(win);
        }
        static void WindowOnMouseDown(object sender, MouseButtonEventArgs args)
        {
            Window win = sender as Window;
            string strMessage = string.Format("Window clicked with {0} button at point ({1})", args.ChangedButton, args.GetPosition(win));
            MessageBox.Show(strMessage, win.Title); // отображение окна сообщения 
        }
    }
}