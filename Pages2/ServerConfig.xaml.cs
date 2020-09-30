using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;

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
using Restaurant_Pos.Pages.UserControls;

namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for ServerConfig.xaml
    /// </summary>
    public partial class ServerConfig : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region 

        //private dynamic loginApiStringResponce,
        //loginApiJOSNResponce,
        //POSCashCustomerApiStringResponce,
        //POSCashCustomerApiJSONResponce,
        //POSCustomerApiStringResponce,
        //POSCustomerApiJSONResponce,
        //POSCategoryApiStringResponce,
        //POSCategoryApiJSONResponce,
        //POSOrderNumberApiStringResponce,
        //POSOrderNumberApiJSONResponce,
        //POSAllProductsApiStringResponce,
        //POSAllProductsApiJSONResponce;

        //private string jsonq;
        //private int CheckServerError = 0;
        //private readonly string DeviceMacAdd = LoginViewModel.CPU_ID();
        //private string DeviceMacAdd = LoginViewModel._DeviceMacAddress;
        //private string[] DeviceMacAdd_arr = LoginViewModel._DeviceMacAddress_arr;
        //private int DeviceMacAdd_count = LoginViewModel._DeviceMacAddress_count;
        //private string CPU_Mac_ID = LoginViewModel.CPU_ID();
        public string connstring = PostgreSQL.ConnectionString;
        public int Warehouse_selection { get; set; }

        public int WarehouseId_Selected { get; set; }
        public string Sys_config_pricelistId { get; set; }

        public string Sys_config_costElementId { get; set; }
        public string Sys_config_businessPartnerId { get; set; }

        public int AD_Client_ID { get; set; }
        public int AD_ORG_ID { get; set; }
        public int AD_USER_ID { get; set; }
        public int AD_ROLE_ID { get; set; }
        public int Sequenc_id { get; set; }

       // public dynamic LoginApiJOSNResponce { get => loginApiJOSNResponce; set => loginApiJOSNResponce = value; }

        public List<M_warehouse> m_warehouse_items = new List<M_warehouse>();
        //private string jsonPOSCustomers;
        //private string jsonPOSCashCustomer;
        //private string jsonPOSCategory;
        //private string jsonPOSOrderNumber;
        //private string jsonPOSAllProducts;

        private string servername;
        private string serverport;

      

        private string api_url;
        private string server_local_name;
        private string server_local_port;
        private string server_local_userid;
        private string server_local_password;
        private string server_local_dbname;
        private string display_language;
        private string on_printer;

        #endregion Public Function
        #region Public Function
        public ServerConfig()
        {
            InitializeComponent();
            rbhttp.IsSealed.ToString();
            
        }
        #endregion

        #region Event
        private void ServerConfig_grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().ToUpper() == "F4")
            {
                BtnConnect_Click( sender, e);
            }
         
        }
     
     
        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    var setting = PostgreSQL.Get_App_setting();
                    servername = setting.servername;
                    serverport = setting.serverport;
                    api_url = setting.api_url;
                    server_local_name = setting.server_local_name;
                    server_local_port = setting.server_local_port;
                    server_local_userid = setting.server_local_userid;
                    server_local_password = setting.server_local_password;
                    server_local_dbname = setting.server_local_dbname;
                    display_language = setting.display_language;
                    on_printer = setting.on_printer;
                });
         
                Application.Current.Properties["ServerName"] = txtWebAddress.Text;
                Application.Current.Properties["PortNo"] = txtPortNo.Text;
                if (txtWebAddress.Text == server_local_name && txtPortNo.Text == server_local_port)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        NavigationService.Navigate(new LoginPage());
                    } );
                }
                if (txtWebAddress.Text == servername && txtPortNo.Text == serverport)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        NavigationService.Navigate(new LoginPage());
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error(" ===================  Error In Login POS  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Server Configration", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;

            }
        }
    
    }
    #endregion
}
