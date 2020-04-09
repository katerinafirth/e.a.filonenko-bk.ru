﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
public class DumpContentPropertyAttributes
{
    [STAThread]
    public static void Main()
    {
        UIElement dummy1 = new UIElement(); //инициализация объекта
        FrameworkElement dummy2 = new FrameworkElement(); // SortedList для хранения класса и свойства контента
        SortedList<string, string> listClass = new SortedList<string, string>(); // Строка форматирования
        string strFormat = "{0,-35}{1}"; // Loop through the loaded assemblies.
        foreach (AssemblyName asmblyname in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
        { // перебор типов
            foreach (Type type in Assembly.Load(asmblyname).GetTypes())
            { //перебор пользовательских атрибутов. // (установить аргумент в 'ложь' только для не наследуемого!)
                foreach (object obj in type.GetCustomAttributes(typeof(ContentPropertyAttribute), true))
                {
                    // добавление в список, если ContentPropertyAttribute
                    if (type.IsPublic && obj as ContentPropertyAttribute != null)
                        listClass.Add(type.Name, (obj as ContentPropertyAttribute).Name);
                }
            }
        } //показать результаты
        Console.WriteLine(strFormat, "Class", "Content Property");
        Console.WriteLine(strFormat, "-----", "----------------");
        foreach (string strClass in listClass.Keys)
            Console.WriteLine(strFormat, strClass, listClass[strClass]);
    }
}