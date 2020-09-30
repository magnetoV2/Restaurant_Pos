using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Restaurant_Pos.Pages.UserControls
{
    /// <summary>
    /// Interaction logic for PosHeaderMenu.xaml
    /// </summary>
    public partial class PosHeaderMenu : UserControl
    {
        public PosHeaderMenu()
        {
            InitializeComponent();
            stMenu.Width = SystemParameters.VirtualScreenWidth;
        }

        private void Btnmenu_Click(object sender, RoutedEventArgs e)
        {
            if (MenuPopup.IsOpen == true)
            {

                MenuPopup.IsOpen = false;
            }
            else
            {
                MenuPopup.IsOpen = true;
            }
        }

        static string progFiles = @"C:\Program Files\Common Files\Microsoft Shared\ink";
        string onScreenKeyboardPath = System.IO.Path.Combine(progFiles, "TabTip.exe");
        Process p;
        private void Keyboard_plysical_Click(object sender, RoutedEventArgs e)
        {
            p.Close();
            Process[] oskProcessArray = Process.GetProcessesByName("TabTip");
            foreach (Process onscreenProcess in oskProcessArray)
            {
                onscreenProcess.Kill();
            }

        }

        private void Keyboard_virtual_Click(object sender, RoutedEventArgs e)
        {

            Process[] oskProcessArray = Process.GetProcessesByName("TabTip");
            foreach (Process onscreenProcess in oskProcessArray)
            {
                onscreenProcess.Kill();
            }

            p = System.Diagnostics.Process.Start(onScreenKeyboardPath);

        }
    }
}
