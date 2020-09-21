using System;
using System.Collections.Generic;
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
using Restaurant_Pos.Pages;


namespace Restaurant_Pos.Pages.UserControls
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
       

        public Menu()
        {
            InitializeComponent();
            
        }

      

        private void ReprintKOT_Click(object sender, RoutedEventArgs e)
        {
            ReprintKOT nextPage = new ReprintKOT();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);
            


        }

        private void ReprintOrders_Click(object sender, RoutedEventArgs e)
        {
            ReprintOrders nextPage = new ReprintOrders();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            ViewProductCAT nextPage = new ViewProductCAT();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);

            EditCategory.Background = Brushes.Blue;
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            ViewProduct nextPage = new ViewProduct();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);
        }

        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            ViewCustomers nextPage = new ViewCustomers();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            ViewUser nextPage = new ViewUser();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);
        }

        private void EditTable_Click(object sender, RoutedEventArgs e)
        {
            ViewTable nextPage = new ViewTable();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);
        }

        private void EditTerminal_Click(object sender, RoutedEventArgs e)
        {
            ViewTerminals nextPage = new ViewTerminals();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);
        }

        private void CustomerPayments_Click(object sender, RoutedEventArgs e)
        {
            customer_credit_pay nextPage = new customer_credit_pay();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);
        }

        private void VendorPayments_Click(object sender, RoutedEventArgs e)
        {
            Vendor_credit_pay nextPage = new Vendor_credit_pay();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);
        }

        private void SubscriptionInfo_Click(object sender, RoutedEventArgs e)
        {
            subscription nextPage = new subscription();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Logout nextPage = new Logout();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);
            
        }

        private void ViewOrders_Click(object sender, RoutedEventArgs e)
        {
            ViewOrder nextPage = new ViewOrder();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(nextPage);
        }
    }
}
