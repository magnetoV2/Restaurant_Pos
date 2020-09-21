using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;

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
    /// Interaction logic for PreBillGenerate.xaml
    /// </summary>
    public partial class PreBillGenerate : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string connstring = PostgreSQL.ConnectionString;
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        int? _invoiceno = 0;



        public PreBillGenerate(int? invoiceno)
        {
            //InvoiceNo = invoiceno;
            InitializeComponent();
            _invoiceno = invoiceno;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataSet ds = new DataSet();
           
            NpgsqlConnection connection = new NpgsqlConnection(connstring);
            connection.Open();
            NpgsqlCommand cmd_User_GetData = new NpgsqlCommand("select c_invoice_id as BillNo,to_char(ic.created, 'DD-Mon-YYYY') as Date,to_char(ic.created, 'HH12:MI:SS') as Time,'T1' as tableName, grandtotal,total_items_count, total_items_count, apu.Name as userName,'CASH CUSTOMER' as CustmorName from C_invoice ic inner join ad_user_pos apu on ic.createdby = apu.ad_user_id  WHERE  ic.c_invoice_id = '" + _invoiceno + "' and ic.ad_org_id =" + _OrgId + "  limit 1 ;", connection);//
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd_User_GetData);
            DataTable dt = new DataTable();
            da.Fill(dt);
            connection.Close();

            connection.Open();
            NpgsqlCommand cmd_Items_GetData = new NpgsqlCommand("select productname, paroductarabicname,qtyinvoiced,saleprice,uomname from C_invoiceline where c_invoice_id = '" + _invoiceno + "' And ad_org_id =" + _OrgId + "  ;", connection);//
            NpgsqlDataAdapter daa = new NpgsqlDataAdapter(cmd_Items_GetData);
            DataTable dtt = new DataTable();
            daa.Fill(dtt);
            connection.Close();
            ds.Tables.Add(dt);
            ds.Tables.Add(dtt);
           
            ReportDocument cryRpt = new ReportDocument();
            cryRpt.Load(@"C:\Users\Admin\Desktop\POS\Restaurant\Restaurant_Pos\CRPreBill.rpt");
            cryRpt.SetDataSource(ds);
            cryRpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, @" C:\Users\Admin\Downloads\KOTPrint\PreBill" + _invoiceno + ".pdf");
            cryRpt.Refresh();
        }

    }
}
