using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for ReprintOrders.xaml
    /// </summary>
    public partial class ReprintOrders : Page
    {
        public ReprintOrders()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(txtinvoice.Text== String.Empty)
            {
                txtinvoice.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                MessageBox.Show("Please Enter The Invoice No! ");
              
                return;

            }
            if (txtorderNo.Text == String.Empty)
            {
                txtorderNo.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                MessageBox.Show("Please Enter The Order No! ");
                return;
            }
            if (txtinvoice.Text == String.Empty && txtorderNo.Text == String.Empty)
            {
            }
            else
            {
                NavigationService.Navigate(new BillGenerate(txtinvoice.Text, txtorderNo.Text));
            }
               
        }

        private void Txtinvoice_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtinvoice.Text = Regex.Replace(txtinvoice.Text, "[^0-9]+", "");
        }

        private void TxtorderNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtorderNo.Text = Regex.Replace(txtorderNo.Text, "[^0-9]+", "");
        }
    }
}
