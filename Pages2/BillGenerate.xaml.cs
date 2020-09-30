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
    /// Interaction logic for BillGenerate.xaml
    /// </summary>
    public partial class BillGenerate : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string connstring = PostgreSQL.ConnectionString;
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        string _invoiceno = "0";
        string _Orderno = "0";
        public BillGenerate(string invoice, string Order)
        {
            InitializeComponent();
            _invoiceno = invoice;
            _Orderno = Order;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataSet dss = new DataSet();

            NpgsqlConnection connection = new NpgsqlConnection(connstring);
            connection.Open();
            NpgsqlCommand cmd_c_invoice_GetData = new NpgsqlCommand("select c_invoice_id as BillNo,to_char(ic.created, 'DD-Mon-YYYY') as Date,to_char(ic.created, 'HH12:MI:SS') as Time,'T1' as tableName, grandtotal,total_items_count, total_items_count, apu.Name as userName,'CASH CUSTOMER' as CustmorName,'1' as  Covers from C_invoice ic inner join ad_user_pos apu on ic.createdby = apu.ad_user_id  WHERE  ic.c_invoice_id = '" + _invoiceno + "' and ic.ad_org_id =" + _OrgId + "  limit 1 ;", connection);//
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd_c_invoice_GetData);
            DataTable dt = new DataTable();
            da.Fill(dt);
            connection.Close();

            connection.Open();
            NpgsqlCommand cmd_Items_GetData = new NpgsqlCommand("select productname, paroductarabicname,qtyinvoiced,saleprice,uomname from C_invoiceline where c_invoice_id = '" + _invoiceno + "' And ad_org_id =" + _OrgId + "  ;", connection);//
            NpgsqlDataAdapter daa = new NpgsqlDataAdapter(cmd_Items_GetData);
            DataTable dtt = new DataTable();
            daa.Fill(dtt);
            connection.Close();
            dss.Tables.Add(dt);
            dss.Tables.Add(dtt);
         //   dss.WriteXmlSchema(@"D:\\CRBill.xml");

            CRBill _crkot = new CRBill();
            _crkot.Load(@"CRBill.rep");
            _crkot.SetDataSource(dss);
            RvKot.ViewerCore.ReportSource = _crkot;

        }
    }
}
