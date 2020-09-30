using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using Restaurant_Pos.Mail;

using System;
using System.Web;
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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;


namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for ViewOrder.xaml
    /// </summary>
    public partial class ViewOrder : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string connstring = PostgreSQL.ConnectionString;
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
   //  int _invoiceno = Int32.Parse(Application.Current.Properties["invoiceno"].ToString());
        public ViewOrder()
        {
            InitializeComponent();
            refreshdata();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
         
            e.Row.Header = (e.Row.GetIndex()).ToString();
        }
        public void refreshdata()
        {
            DataSet ViewOrderds = new DataSet();

            NpgsqlConnection connection = new NpgsqlConnection(connstring);
            connection.Open();
            NpgsqlCommand cmd_ViewOrder_GetData = new NpgsqlCommand("select kl.c_invoice_id ,kl.c_invoice_id as orderid,kl.kotno as c_kotline_id,to_char(ic.created, 'DD-Mon-YYYY HH12:MI:SS')As Date , apu.Name, icl.productname, icl.qtyinvoiced, icl.saleprice, is_onhold, is_canceled, is_completed from c_kotline kl  inner  join C_invoice ic on kl.c_invoice_id=ic.c_invoice_id inner join C_invoiceLine icl on ic.c_invoice_id = icl.c_invoice_id inner join ad_user_pos apu on ic.createdby = apu.ad_user_id where  icl.productname=kl.productname group by ic.c_invoice_id ,icl.c_invoice_id ,ic.createdby,apu.ad_user_id,kl.c_invoice_id ,kl.kotno,apu.Name,icl.productname,icl.qtyinvoiced, icl.saleprice, is_onhold,is_canceled, is_completed, kl.m_terminal_id", connection);//            
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd_ViewOrder_GetData);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ViewOrderds.Tables.Add(dt);
            if (ViewOrderds.Tables[0].Rows.Count > 0)
            {
                DataGrdViewOrder.ItemsSource = ViewOrderds.Tables[0].DefaultView;
            }
            connection.Close();
            
        }
    }
}
