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
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;


namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for Logout.xaml
    /// </summary>
    public partial class Logout : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        public string connstring = PostgreSQL.ConnectionString;
        public Logout()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PopupLogoutConfirm.IsOpen = true;
        }

        private void BtnPopupNo_Click(object sender, RoutedEventArgs e)
        {
            PopupLogoutConfirm.IsOpen = false;
            NavigationService.Navigate(new POSsystem());
        }

        private void BtnPopupYes_Click(object sender, RoutedEventArgs e)
        {
            PopupLogoutConfirm.IsOpen = false;
            NpgsqlConnection connection = new NpgsqlConnection(connstring);
            connection.Open();
            NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_user_session SET isactive ='N' ,endtime='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' WHERE createdby='" + _UserID + "'and  ad_client_id='"+_clientId+ "' and ad_org_id='"+_OrgId+ "' and isactive ='Y'   ;", connection);
            NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
            connection.Close();
            MessageBox.Show("Logout Successfully !! ");
            Application.Current.Properties["clientId"] = "";
            Application.Current.Properties["OrgId"] = "";
            Application.Current.Properties["UserID"] = "";
            Application.Current.Properties["SessionID"] = "";
            Application.Current.Properties["BpartnerId"] = "";
            Application.Current.Properties["WarehouseId"] = "";
            Application.Current.Properties["macAddress_post"] = "";
            Application.Current.Properties["version_post"] = "";
            Application.Current.Properties["appName_post"] = "";
           
            Environment.Exit(0);
        }
    }
}
