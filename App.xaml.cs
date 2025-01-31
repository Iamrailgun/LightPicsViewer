﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace lpv
{
  /// <summary>
  /// Логика взаимодействия для App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      if (e.Args.Length > 0)
      {
        Application.Current.Resources.Add("args", e.Args);
      }
      else
      {
        Application.Current.Shutdown();
      }
    }
  }
}
