using log4net;
using Npgsql;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;




namespace Restaurant_Pos.Pages
{

    /// <summary>
    /// Interaction logic for KotGenerate.xaml
    /// </summary>
    public partial class KotGenerate : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string connstring = PostgreSQL.ConnectionString;
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        string _invoiceno = "0";
        int _kotno =0;
        public int Sequenc_id { get; set; }
        string pagename = "";
        public KotGenerate(string invoiceno,int Kot,string pgname)
        {
            InitializeComponent();
            _invoiceno = invoiceno;
            _kotno = Kot;
            pagename = pgname;
            reportview();
        }
    
        private void reportview()
        {
            DataSet ds = new DataSet();

            NpgsqlConnection connection = new NpgsqlConnection(connstring);
           
            connection.Open();
            NpgsqlCommand cmd_User_GetData = new NpgsqlCommand("select c_invoice_id as BillNo,to_char(ck.created, 'DD-Mon-YYYY') as Date,to_char(ck.created, 'HH12:MI:SS') as Time,mt.name as tableName, " + _kotno + " as KotNo,'Neelam' as OrderBy from c_kot ck inner join m_table mt on mt.m_table_id = ck.m_table_id  WHERE  ck.c_invoice_id = '" + _invoiceno + "' and ck.ad_org_id =" + _OrgId + "   limit 1 ;", connection);//
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd_User_GetData);
            DataTable dt = new DataTable();
            da.Fill(dt);
            connection.Close();

            connection.Open();
            NpgsqlCommand cmd_Items_GetData = new NpgsqlCommand("select productname, paroductarabicname from c_kotline where c_invoice_id = '" + _invoiceno + "' And ad_org_id =" + _OrgId + " and kotno='" + _kotno + "';", connection);//
            NpgsqlDataAdapter daa = new NpgsqlDataAdapter(cmd_Items_GetData);
            DataTable dtt = new DataTable();
            daa.Fill(dtt);
            connection.Close();
            ds.Tables.Add(dt);
            ds.Tables.Add(dtt);



            if (pagename == "POS")
            {

                ReportDocument cryRpt = new ReportDocument();
                cryRpt.Load(@"C:\Users\Admin\Desktop\POS\Restaurant\Restaurant_Pos\CRKot.rpt");
                cryRpt.SetDataSource(ds);
                //   cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "Crystal");
                cryRpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, @" C:\Users\Admin\Downloads\KOTPrint\Kot" + Sequenc_id + "_" + _kotno + ".pdf");
                cryRpt.Refresh();

            }
            else
            {
                CRKot _crkot = new CRKot();
                _crkot.Load(@"CRKot.rep");
                _crkot.SetDataSource(ds);
                RvKot.ViewerCore.ReportSource = _crkot;
                //  RvKot.Refresh();
            }
          
        }
       
     
    }

        
    }

