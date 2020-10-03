using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using Restaurant_Pos.Mail;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text;
using System.Data;
using System.Windows.Media.Imaging;
using Restaurant_Pos.Pages;
using System.Windows.Navigation;
using Microsoft.Reporting.WinForms;

namespace Restaurant_Pos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow()
        {
            InitializeComponent();
            //log4net.Config.XmlConfigurator.Configure();
            DataContext = new WindowViewModel(this);
          
        }

        public ReportViewer _reportviewer { get; internal set; }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            // Application activated
            this.Focusable = true;
            this.Focus();
        }

        private void Window_Deactivated(object sender, System.EventArgs e)
        {
            // Application deactivated

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var key = (e.Key == Key.System ? e.SystemKey : e.Key);

            if (key == Key.I && (Keyboard.Modifiers & (ModifierKeys.Alt)) == (ModifierKeys.Alt))
            // if (e.Key == Key.I && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
            {
                MainFrame.Navigate(new DineIn());
            }
            if (key == Key.O && (Keyboard.Modifiers & (ModifierKeys.Alt)) == (ModifierKeys.Alt))
                //if (e.Key == Key.O && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
            {

                MainFrame.Navigate(new orderonline());
            }

        }

    }
}
