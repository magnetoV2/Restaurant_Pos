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
using System.Windows.Media.Imaging;
using Restaurant_Pos.Pages.UserControls;
namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for subscription.xaml
    /// </summary>
    public partial class subscription : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string connstring = PostgreSQL.ConnectionString;
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        public subscription()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connstring);
            // connection.Close();
            connection.Open();
            //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
            NpgsqlCommand cmd_Sub_GetData = new NpgsqlCommand("select Name, to_char(subsdate, 'DD-Mon-YYYY HH12:MI:SS')As subsdate,to_char(subsenddate, 'DD-Mon-YYYY HH12:MI:SS')As  subsenddate from ad_org_subscription where ad_client_id='" + _clientId + "' and ad_org_id='"+_OrgId+"'  ; ", connection);//
            NpgsqlDataReader _Sub_GetData = cmd_Sub_GetData.ExecuteReader();
            _Sub_GetData.Read();
            lblsubinfo.Content = " subscription Name " + _Sub_GetData.GetString(0) + " subscription start date " + _Sub_GetData.GetString(1) + " subscription End date " + _Sub_GetData.GetString(2) ;
        }
    }
}
