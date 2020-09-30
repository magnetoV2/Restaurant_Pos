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
    /// Interaction logic for customer_credit_pay.xaml
    /// </summary>
    public partial class customer_credit_pay : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        int _Bpartner_Id = Int32.Parse(Application.Current.Properties["BpartnerId"].ToString());
        int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());
        public List<M_customercreditpay> _m_customercreditpayitems = new List<M_customercreditpay>();
        public List<M_customerpay> _m_customerpayitems = new List<M_customerpay>();
        public string connstring = PostgreSQL.ConnectionString;
        private dynamic CustCreditApiStringResponce,
        custcreditApiJOSNResponce;
        public dynamic CustCreditApiJOSNResponce { get => custcreditApiJOSNResponce; set => custcreditApiJOSNResponce = value; }
        private string jsonq;
        private int CheckServerError = 0;
        double itemsRate = 0;
        public List<M_selectedcustomer> _invoicelist = new List<M_selectedcustomer>();
       
        public customer_credit_pay()
        {
            InitializeComponent();
            BindCustomer();
         //   BindCustomerCredit();
          //  BindCustomerPay();
            BindCustomer();
        }

        private void Product_Search_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_CreditPay_GetData = new NpgsqlCommand("select c_bpartner_id,name,image from c_bpartner  WHERE  ad_client_id = " + _clientId + " and name like '"+ product_Search.Text + "%' and iscustomer='Y'  ; ", connection);//
                NpgsqlDataReader _CreditPay_GetData = cmd_CreditPay_GetData.ExecuteReader();
                _m_customercreditpayitems.Clear();
                CustPayment_ListBox.ItemsSource = "";
                while (_CreditPay_GetData.Read())
                {
                    _m_customercreditpayitems.Add(new M_customercreditpay()
                    {
                        id = int.Parse(_CreditPay_GetData["c_bpartner_id"].ToString()),
                        Name = _CreditPay_GetData["name"].ToString(),
                        Image = _CreditPay_GetData["image"].ToString()

                    });
                }
                connection.Close();

                if (_m_customercreditpayitems.Count == 0)
                {
                    MessageBox.Show("Please Add Customer!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        CustPayment_ListBox.ItemsSource = _m_customercreditpayitems;
                    });
                }


            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Customer  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Customer Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }

                return;
            }
        }
        public void BindCustomer()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_CreditPay_GetData = new NpgsqlCommand("select c_bpartner_id,name,image from c_bpartner  WHERE  ad_client_id = " + _clientId + " And iscustomer='Y'; ", connection);//
                NpgsqlDataReader _CreditPay_GetData = cmd_CreditPay_GetData.ExecuteReader();
                _m_customercreditpayitems.Clear();
                CustPayment_ListBox.ItemsSource = "";
                while (_CreditPay_GetData.Read())
                {
                    _m_customercreditpayitems.Add(new M_customercreditpay()
                    {
                        id = int.Parse(_CreditPay_GetData["c_bpartner_id"].ToString()),
                        Name = _CreditPay_GetData["name"].ToString(),
                        Image = _CreditPay_GetData["image"].ToString()
                      

                    });
                }
                connection.Close();

                if (_m_customercreditpayitems.Count == 0)
                {
                    MessageBox.Show("Please Add Customer!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        CustPayment_ListBox.ItemsSource = _m_customercreditpayitems;
                    });
                }


            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Customer  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Customer Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void BindCustomerPay()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_CustPay_GetData = new NpgsqlCommand("select C_Invoice_id, created, CONCAT(total_items_count, ' Items') as itemcount, grandtotal from c_invoice WHERE  ad_client_id = " + _clientId + " limit 5; ", connection);//
                NpgsqlDataReader _CustPay_GetData = cmd_CustPay_GetData.ExecuteReader();
                _m_customerpayitems.Clear();
                CustPaymentBal_ListBox.ItemsSource = "";
                while (_CustPay_GetData.Read())
                {
                    _m_customerpayitems.Add(new M_customerpay()
                    {
                        invoiceId = _CustPay_GetData.GetInt32(0),
                        invoiceDate = _CustPay_GetData.GetDateTime(1),
                        documentNo = _CustPay_GetData.GetString(2),
                        grandTotal = _CustPay_GetData.GetDouble(3),

                    });
                }
                connection.Close();

                if (_m_customerpayitems.Count == 0)
                {
                    MessageBox.Show("Please Add CustomerCredit Pay!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        CustPaymentBal_ListBox.ItemsSource = _m_customerpayitems;
                    });
                }


            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Customer  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Customer credit pay Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void BindCustomerCredit(int _BpartnerId)
        {
            this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Please Wait .. "; });
            this.Dispatcher.Invoke(() =>
            {
                POSCustCreditApiModel CustCreditDetail = new POSCustCreditApiModel
                {
                    macAddress = "8e17517b8cd41f3c",
                    clientId = _clientId,
                    orgId = _OrgId,
                    warehouseId = _Warehouse_Id,
                    userId = _UserID,
                    version = "2.0",
                    appName = "POS",
                    operation = "GetCreditInvoices",
                    businessPartnerId = _BpartnerId,
                    isCustomer = "Y",
                    isVendor ="N",
                  

                };

                jsonq = JsonConvert.SerializeObject(CustCreditDetail);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Passing Data to Api Call"; });
            });

            try
            {
                CustCreditApiStringResponce = PostgreSQL.ApiCallPost(jsonq);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
                CheckServerError = 1;
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
            }
            catch
            {
                CheckServerError = 0;
                this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                log.Error("Terminals Error|POSOrganization API Not Responding");
                return;
            }
            if (CheckServerError == 1)
            {
                CustCreditApiJOSNResponce = JsonConvert.DeserializeObject(CustCreditApiStringResponce);

                if (CustCreditApiJOSNResponce.responseCode == "200")
                {
                    JArray _invoiceListAccessDetails =  CustCreditApiJOSNResponce.invoiceList;
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: loginApiResponce Responce Code 200"; });
                    NpgsqlConnection connection = new NpgsqlConnection(connstring);
                    connection.Open();
                    foreach (dynamic _m_invoiceList in _invoiceListAccessDetails)
                    {
                        _m_customerpayitems.Add(new M_customerpay()
                        {
                            invoiceId = _m_invoiceList.invoiceId,
                            invoiceDate = _m_invoiceList.invoiceDate,
                            documentNo = _m_invoiceList.documentNo,
                            grandTotal = _m_invoiceList.grandTotal,

                        });
                    }
                    CustPaymentBal_ListBox.ItemsSource = _m_customerpayitems;


                    MessageBox.Show("Record Edit Successfully !");
                    connection.Close();
                

                }
                else if (CustCreditApiJOSNResponce.responseCode == "106")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        LoginProcessingText.Text = "CustCredit not Edit  !";

                    });
                    return;
                }
            }

           


        }
        private void CustPayment_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic CPL = CustPayment_ListBox.SelectedItem as dynamic;
            if(CPL==null)
            {

            }
            else
            {
                int _BpartnerId = CPL.id;
                // BindCustomerCredit(_BpartnerId);
                BindCustomerPay();
            }
           

        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool? isChecked = (sender as CheckBox).IsChecked;
            string documentNo = (sender as CheckBox).ToolTip.ToString();
         
            dynamic SelectedItem = _m_customerpayitems.Find(i => i.documentNo == documentNo);
            M_selectedcustomer x = new M_selectedcustomer {
                documentNo = SelectedItem.documentNo,
                posId= SelectedItem.posId,
                grandTotal= SelectedItem.grandTotal,
                invoiceId = SelectedItem.invoiceId,
                invoiceDate= SelectedItem.invoiceDate
            };
            int SelectedItemIndex = _m_customerpayitems.FindIndex(i => i.documentNo == documentNo);
           // double itemsRate = 0;
            if (isChecked == true)
             itemsRate += SelectedItem.grandTotal;
            if (isChecked == false)
             itemsRate -= SelectedItem.grandTotal;
            lblableAmt.Content = "QR"+ itemsRate;
            txtBal.Text = itemsRate.ToString();
            lblPaymentAmt.Content = txtBal.Text;
            

            _invoicelist.Add(x);
            spPayment.Visibility = Visibility.Visible;
        }
        #region Event  Payment
        private void One_Click(object sender, RoutedEventArgs e)
        {
            txtBal.Text += "1";
        }

        private void Two_Click(object sender, RoutedEventArgs e)
        {
            txtBal.Text += "2";
        }

        private void Three_Click(object sender, RoutedEventArgs e)
        {
            txtBal.Text += "3";
        }

        private void Four_Click(object sender, RoutedEventArgs e)
        {
            txtBal.Text += "4";
        }

        private void Mul_Click(object sender, RoutedEventArgs e)
        {
            int total = txtBal.Text.Length;
            if (total > 0)
            {
                total = total - 1;
                string bal = txtBal.Text.Substring(0, total);
                txtBal.Text = bal;
            }
            else
            {
                txtBal.Text = "";
            }

           
        }

        private void Five_Click(object sender, RoutedEventArgs e)
        {
            txtBal.Text += "5";
        }

        private void Six_Click(object sender, RoutedEventArgs e)
        {
            txtBal.Text += "6";
        }

        private void Eight_Click(object sender, RoutedEventArgs e)
        {
            txtBal.Text += "8";
        }

        private void Ziro_Click(object sender, RoutedEventArgs e)
        {
            txtBal.Text += "0";
        }

        private void Nine_Click(object sender, RoutedEventArgs e)
        {
            txtBal.Text += "9";
        }

        private void Point_Click(object sender, RoutedEventArgs e)
        {
            txtBal.Text += ".";
        }

      
        private void Seven_Click(object sender, RoutedEventArgs e)
        {
            txtBal.Text += "7";
        }

        #endregion

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Please Wait .. "; });
            this.Dispatcher.Invoke(() =>
            {

                POSInvoicePaymentsModel _CustInvoicePayments = new POSInvoicePaymentsModel
                {

                    macAddress = "8e17517b8cd41f3c",
                    clientId = _clientId,
                    orgId = _OrgId,
                    warehouseId = _Warehouse_Id,
                    userId = _UserID,
                    version = "2.0",
                    appName = "POS",
                    operation = "PostCreditInvoicePayments",
                    businessPartnerId = _Bpartner_Id,
                    cashbookId = "1000021",
                    payAmount = txtBal.Text,
                    tenderType = "X",
                    description = txtBal.Text + " QAR Paid",
                    isCustomer = "Y",
                    isVendor = "N",
                    invoiceList=_invoicelist
                 
                };
            
               
             


                jsonq = JsonConvert.SerializeObject(_CustInvoicePayments);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Passing Data to Api Call"; });
            });

            try
            {
                CustCreditApiStringResponce = PostgreSQL.ApiCallPost(jsonq);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
                CheckServerError = 1;
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
            }
            catch
            {
                CheckServerError = 0;
                this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                log.Error("Terminals Error|POSOrganization API Not Responding");
                return;
            }
            if (CheckServerError == 1)
            {
                CustCreditApiJOSNResponce = JsonConvert.DeserializeObject(CustCreditApiStringResponce);

                if (CustCreditApiJOSNResponce.responseCode == "200")
                {
                    JArray _invoiceListAccessDetails = CustCreditApiJOSNResponce.invoiceList;
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: loginApiResponce Responce Code 200"; });
                    NpgsqlConnection connection = new NpgsqlConnection(connstring);
                    connection.Open();
                    foreach (dynamic _m_invoiceList in _invoiceListAccessDetails)
                    {
                        _m_customerpayitems.Add(new M_customerpay()
                        {
                            invoiceId = _m_invoiceList.invoiceId,
                            invoiceDate = _m_invoiceList.invoiceDate,
                            documentNo = _m_invoiceList.documentNo,
                            grandTotal = _m_invoiceList.grandTotal,

                        });
                    }
                    CustPaymentBal_ListBox.ItemsSource = _m_customerpayitems;


                    MessageBox.Show(" Pay Successfully !");
                    connection.Close();


                }
                else if (CustCreditApiJOSNResponce.responseCode == "106")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        LoginProcessingText.Text = "Pay not Successfully  !";

                    });
                    return;
                }
            }


        }

    }
}
