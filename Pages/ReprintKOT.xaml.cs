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
    /// Interaction logic for ReprintKOT.xaml
    /// </summary>
    public partial class ReprintKOT : Page
    {
        public ReprintKOT()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtinvoice.Text == String.Empty)
            {
                txtinvoice.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                MessageBox.Show("Please Enter The Invoice No! ");
                return;
            }
            if (txtKotNo.Text == String.Empty)
            {
                txtKotNo.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                MessageBox.Show("Please Enter The KOT No! ");
                return;
            }
            if (txtinvoice.Text== String.Empty && txtKotNo.Text== String.Empty)
            {
                MessageBox.Show("Please Fill The Textbox");
                return;
            }
            else
            {
                NavigationService.Navigate(new KotGenerate(txtinvoice.Text, Int32.Parse(txtKotNo.Text), "Reprint"));
            }
           
        }

      

        private void Txtinvoice_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtinvoice.Text = Regex.Replace(txtinvoice.Text, "[^0-9]+", "");
        }

        private void TxtKotNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtKotNo.Text = Regex.Replace(txtKotNo.Text, "[^0-9]+", "");
        }
    }
}
