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
using System.Text;
using System.Data;

namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for orderonline.xaml
    /// </summary>
    public partial class orderonline : Page
    {
        public List<m_OrderOnlineDetail> _m_OrderOnlineDetail_items = new List<m_OrderOnlineDetail>();
        public string connstring = PostgreSQL.ConnectionString;
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());

        int _sessionID = Int32.Parse(Application.Current.Properties["SessionID"].ToString());
        int _Bpartner_Id = Int32.Parse(Application.Current.Properties["BpartnerId"].ToString());
        int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());
        public long _sequenc_id { get; set; }

      //  int totalItems = 0;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public orderonline()
        {
            InitializeComponent();
            OrderDetail();
        }
        public void OrderDetail()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_OrderOnline_GetData = new NpgsqlCommand("select '#3214' as id,'Dr Aliya' as Name,'Delhi' as address,'on the Way' from m_product_category  where ad_org_id=" + _OrgId + "  ;", connection);//
                NpgsqlDataReader _cmd_OrderOnline_GetData = cmd_OrderOnline_GetData.ExecuteReader();
                _m_OrderOnlineDetail_items.Clear();

                Order_ListBox.ItemsSource = "";
               
                while (_cmd_OrderOnline_GetData.Read())
                {

                    _m_OrderOnlineDetail_items.Add(new m_OrderOnlineDetail()
                    {
                        id = _cmd_OrderOnline_GetData.GetString(0),
                        Name = _cmd_OrderOnline_GetData.GetString(1),
                        Address = _cmd_OrderOnline_GetData.GetString(2),
                        status= _cmd_OrderOnline_GetData.GetString(3)

                    });
                }

                connection.Close();

                if (_m_OrderOnlineDetail_items.Count == 0)
                {
                    MessageBox.Show("Order Online Not Found!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        Order_ListBox.ItemsSource = _m_OrderOnlineDetail_items;
                    });
                }
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Order Online  Not Found!   =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Order Online Not Found!", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }

        private void TakeAway_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new POSsystem());
        }

        private void DineIn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DineIn());
        }

        private void TxtCatSearch_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
