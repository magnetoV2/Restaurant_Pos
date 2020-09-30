using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using Restaurant_Pos.Mail;
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
using Restaurant_Pos.ViewModels.Page;
using CrystalDecisions.CrystalReports.Engine;


namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for POSsystem.xaml
    /// </summary>
    public partial class POSsystem : Page
    {
        #region 
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());

        string selectedItemForEditNotes = "";

        string _macAddress_post = (Application.Current.Properties["macAddress_post"].ToString());
        string _version_post = (Application.Current.Properties["version_post"].ToString());
        string _appName_post = (Application.Current.Properties["appName_post"].ToString());

        int _sessionID = Int32.Parse(Application.Current.Properties["SessionID"].ToString());
        int _Bpartner_Id = Int32.Parse(Application.Current.Properties["BpartnerId"].ToString());
        int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());
        public long _sequenc_id { get; set; }
        public List<m_CustomersList> m_CustomersList_items = new List<m_CustomersList>();
        DateTime invoiceDatetime;
        int? splitinvoiceNo = 0;
        int totalItems = 0;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<m_onHoldInvoice> _m_onHoldInvoice = new List<m_onHoldInvoice>();
        public List<m_TakeAwayProduct> _m_TakeAwayProduct = new List<m_TakeAwayProduct>();
        public List<m_TakeAwayProductCAT> m_TakeAwayProductCAT = new List<m_TakeAwayProductCAT>();
        public List<m_TakeAwayProductRS> _m_TakeAwayProductRS = new List<m_TakeAwayProductRS>();
        public List<m_TakeAwayProductValue> _m_TakeAwayProductValue = new List<m_TakeAwayProductValue>();
        public List<m_floorTables> _m_floor2Tables = new List<m_floorTables>();
        public List<m_floorTables> _m_floor1Tables = new List<m_floorTables>();
        public List<m_TakeAwayProductRS> splitbill = new List<m_TakeAwayProductRS>();
        public List<OrderDetails> _orderDetails = new List<OrderDetails>();
        public string connstring = PostgreSQL.ConnectionString;
        public string selectItemForLineDiscount = null;
        double payableAmount = 0;
        double paidCash = 0;
        bool totalDiscountApplied = false;
        float totalAmount = 0;
        string OrderCancelreason = null;
        //int invoiceno = 0;
        int Product_ListBoxSelect = 0;
        
        //  int splitInvoice = 0;
        int? invoiceno = 0;
        int selectedTable = 0;
        public int Sequenc_id { get; set; }
        public int Sequenc_kotid { get; set; }
        public int Sequenc_kotlineid { get; set; }
        int kotno = 0;
        #endregion

        #region Public Function
        public POSsystem()
        {
            InitializeComponent();
            BindTakeAwayProductCAT();
            calculateSum();
            CustomersList();
            double height = SystemParameters.PrimaryScreenHeight - 250;
                Product_ListBox.Height =height ;
        }
        #endregion


        #region Bind Data Function
        public void CustomersList()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_CustomersList_GetData = new NpgsqlCommand("select c_bpartner_id,name from  c_bpartner  where ad_org_id=" + _OrgId + "  and iscustomer='Y' ;", connection);//
                NpgsqlDataReader _CustomersList_GetData = cmd_CustomersList_GetData.ExecuteReader();
                m_CustomersList_items.Clear();
                Customer_DropDown.ItemsSource = "";

                while (_CustomersList_GetData.Read())
                {
                    m_CustomersList_items.Add(new m_CustomersList()
                    {
                        id = int.Parse(_CustomersList_GetData["c_bpartner_id"].ToString()),
                        Name = _CustomersList_GetData["name"].ToString(),

                    });
                }

                connection.Close();

                if (m_CustomersList_items.Count == 0)
                {
                    MessageBox.Show("Customers Is Not Found!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        Customer_DropDown.ItemsSource = m_CustomersList_items;
                    });
                }


            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Customers Is Not Found!  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Customers Is Not Found!", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }

        public void BindTakeAwayProduct(int ID)
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_TakeAwayProduct_GetData = new NpgsqlCommand("select m_product_id,name,image from M_product  where ad_org_id=" + _OrgId + " and m_product_category_id='" + ID + "'  ;", connection);//
                NpgsqlDataReader _TakeAwayProduct_GetData = cmd_TakeAwayProduct_GetData.ExecuteReader();
                _m_TakeAwayProduct.Clear();
                productCat_ListBox.ItemsSource = "";
                while (_TakeAwayProduct_GetData.Read())
                {

                    _m_TakeAwayProduct.Add(new m_TakeAwayProduct()
                    {
                        id = int.Parse(_TakeAwayProduct_GetData["m_product_id"].ToString()),
                        Name = _TakeAwayProduct_GetData["name"].ToString(),
                        ImgPath = _TakeAwayProduct_GetData["image"].ToString()                        
                    });
                }

                connection.Close();

                if (_m_TakeAwayProduct.Count == 0)
                {
                    MessageBox.Show("Please Add Product !");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        productCat_ListBox.ItemsSource = _m_TakeAwayProduct;
                    });
                }
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Product   =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Product  Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }

        public void BindTakeAwayProductCAT()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_TakeAwayProductCAT_GetData = new NpgsqlCommand("select c.m_product_category_id,c.name,c.image,count(p.m_product_category_id) from  m_product_category c left join m_product p on c.m_product_category_id=p.m_product_category_id where c.ad_org_id=" + _OrgId + " group by c.name,c.m_product_category_id,c.image ;", connection);//
                NpgsqlDataReader _TakeAwayProductCAT_GetData = cmd_TakeAwayProductCAT_GetData.ExecuteReader();               
                m_TakeAwayProductCAT.Clear();
                _m_TakeAwayProduct.Clear();
                Product_ListBox.ItemsSource = "";
                productCat_ListBox.ItemsSource = "";
                while (_TakeAwayProductCAT_GetData.Read())
                {

                    m_TakeAwayProductCAT.Add(new m_TakeAwayProductCAT()
                    {
                        id = int.Parse(_TakeAwayProductCAT_GetData["m_product_category_id"].ToString()),
                        Name = _TakeAwayProductCAT_GetData["name"].ToString(),
                        ImgPath = _TakeAwayProductCAT_GetData["image"].ToString(),                     
                        count= _TakeAwayProductCAT_GetData["count"].ToString()
                    });
                }

                connection.Close();

                if (m_TakeAwayProductCAT.Count == 0)
                {
                    MessageBox.Show("Product Category Not Found!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        Product_ListBox.ItemsSource = m_TakeAwayProductCAT.OrderBy(item => item.Name);
                    });
                }
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Product Category Not Found!   =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Product Category Not Found!", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }

        public void BindTakeAwayProductRS(int ID)
        {
            int index = _m_TakeAwayProductRS.FindIndex((x) => x.id == ID);
            if (index < 0)
            {
                try
                {

                    NpgsqlConnection connection = new NpgsqlConnection(connstring);
                    connection.Open();
                    NpgsqlCommand cmd_TakeAwayProductCAT_GetData = new NpgsqlCommand("select m_product_id,name,currentcostprice, barcode,arabicname,uomid,uomname,purchaseprice,m_terminal_id from m_product  where ad_org_id=" + _OrgId + " and m_product_id='" + ID + "' ;", connection);//
                    NpgsqlDataReader _TakeAwayProductCAT_GetData = cmd_TakeAwayProductCAT_GetData.ExecuteReader();

                    price_ListBox.ItemsSource = "";
                    while (_TakeAwayProductCAT_GetData.Read())
                    {


                        _m_TakeAwayProductRS.Add(new m_TakeAwayProductRS()
                        {
                            id = int.Parse(_TakeAwayProductCAT_GetData["m_product_id"].ToString()),
                            Name = _TakeAwayProductCAT_GetData["name"].ToString(),
                            Price = int.Parse(_TakeAwayProductCAT_GetData["currentcostprice"].ToString()),
                            ItemCount = 1,

                            productbarcode = _TakeAwayProductCAT_GetData["barcode"].ToString(),

                            paroductarabicname = _TakeAwayProductCAT_GetData["arabicname"].ToString(),
                            c_uom_id = _TakeAwayProductCAT_GetData["uomid"].ToString(),
                            uomname = _TakeAwayProductCAT_GetData["uomname"].ToString(),
                            qtyinvoiced = "1",
                            qtyentered = "1",
                            saleprice = _TakeAwayProductCAT_GetData["currentcostprice"].ToString(),

                            m_pricelist_id = "1",
                            m_terminal_id = _TakeAwayProductCAT_GetData["m_terminal_id"].ToString()
                        });

                    }

                    connection.Close();

                    if (_m_TakeAwayProductRS.Count == 0)
                    {
                        MessageBox.Show("Product Category Rate Not Found!");

                        return;

                    }
                    else
                    {


                        this.Dispatcher.Invoke(() =>
                        {

                            price_ListBox.ItemsSource = _m_TakeAwayProductRS;
                        });
                    }
                }
                catch (Exception ex)
                {

                    log.Error(" ===================  Error In Product Category Not Found!   =========================== ");
                    log.Error(DateTime.Now.ToString());
                    log.Error(ex.ToString());
                    log.Error(" ===================  End of Error  =========================== ");
                    if (MessageBox.Show(ex.ToString(),
                            "Error In Product Category Rates Not Found!", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                    {
                        return;
                    }
                    return;
                }
            }
            else
            {
                _m_TakeAwayProductRS[index].ItemCount++;
                if (_m_TakeAwayProductRS.Count == 0)
                {
                    MessageBox.Show("Product Category Rate Not Found!");

                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        price_ListBox.ItemsSource = _m_TakeAwayProductRS;
                    });
                    price_ListBox.Items.Refresh();
                }


            }



        }

        #endregion

        #region Event Popup Payment
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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            paymentPopup.IsOpen = false;
        }



        private void TxtBal_KeyUp(object sender, KeyEventArgs e)
        {
            decimal BalAmt = int.Parse(txtBal.Text) - int.Parse(PayableAmt.Content.ToString());
            BalanceAmt.Content = BalAmt;
        }
        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            paymentPopup.IsOpen = false;
            //paymentPopup.Visibility = Visibility.Collapsed;
            try
            {
                if (lblInvoiceNo.Content != null)
                {


                    using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
                    {
                        dynamic CD = Customer_DropDown.SelectedItem as dynamic;
                        int CustomerID = CD.id;
                        connection.Open();
                        NpgsqlCommand INSERT_c_invoicepaymentdetails = new NpgsqlCommand("INSERT INTO c_invoicepaymentdetails(" +
                                                        " ad_client_id, ad_org_id, c_invoice_id," +
                                                        "cash, card, exchange, redemption, iscomplementary, iscredit, createdby, updatedby)" +
                                                        "VALUES(" + _clientId + "," + _OrgId + "," + lblInvoiceNo.Content + "," + payableAmount + ",0,0,0,'N','N'," + _UserID + "," + _UserID + "); ", connection);
                        INSERT_c_invoicepaymentdetails.ExecuteNonQuery();
                        connection.Close();
                        connection.Open();
                        NpgsqlCommand c_invoiceCust = new NpgsqlCommand("update  c_invoice set c_bpartner_id='" + CustomerID + "',is_onhold='N' where c_invoice_id=" + lblInvoiceNo.Content + "", connection);
                        c_invoiceCust.ExecuteNonQuery();
                        connection.Close();
                        //NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + _sequenc_id + " WHERE name = 'c_invoicepaymentdetails';", connection);
                        //NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                        MessageBox.Show("cash paymen completed successfully");
                        if (onHoldOrders.Visibility == Visibility.Visible)
                            Recall();
                        paidCash = payableAmount;
                        try
                        {
                            OrdersApicall("POSOrderRelease");
                            using (NpgsqlConnection connection3 = new NpgsqlConnection(connstring))
                            {
                                connection.Open();
                                NpgsqlCommand c_invoiceSync = new NpgsqlCommand("update  c_invoice set is_onsync='Y' where c_invoice_id=" + lblInvoiceNo.Content + "", connection);
                                c_invoiceSync.ExecuteNonQuery();

                            }
                        }
                        catch
                        {
                            using (NpgsqlConnection connection3 = new NpgsqlConnection(connstring))
                            {
                                connection.Open();
                                NpgsqlCommand c_invoiceSync = new NpgsqlCommand("update  c_invoice set is_onsync='N' where c_invoice_id=" + lblInvoiceNo.Content + "", connection);
                                c_invoiceSync.ExecuteNonQuery();

                            }
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Please Generated Pre Bill !!");
                }
            }
            catch
            {
                MessageBox.Show("Data Not inserted !!");
            }


        }
        #endregion

        #region Event Popup Notes
        private void BtnPopupNotesVancel_Click(object sender, RoutedEventArgs e)
        {
            MyPopupNotes.IsOpen = false;
        }


        private void BtnPopupNotesSubmit_Click(object sender, RoutedEventArgs e)
        {

            MyPopupNotes.IsOpen = false;
            m_TakeAwayProductRS SelectedItem = _m_TakeAwayProductRS.Find(i => i.Name == selectedItemForEditNotes);
            int SelectedItemIndex = _m_TakeAwayProductRS.FindIndex(i => i.Name == selectedItemForEditNotes);
            _m_TakeAwayProductRS.RemoveAt(SelectedItemIndex);
            SelectedItem.KotNotes = txtnotes.Text;
            _m_TakeAwayProductRS.Insert(SelectedItemIndex, SelectedItem);
        }
        private void BtnNotesEdit_Click(object sender, RoutedEventArgs e)
        {          
            selectedItemForEditNotes = (sender as Button).ToolTip.ToString();                     
            MyPopupNotes.IsOpen = true;
            lblItemName.Content = "items";
          
        }
        #endregion

        #region Event Line Discount


        private void MyPopupLineDiscSubmit_Click(object sender, RoutedEventArgs e)
        {
            MyPopupLineDisc.IsOpen = false;
            if (!totalDiscountApplied)
            {
                m_TakeAwayProductRS SelectedItem = _m_TakeAwayProductRS.Find(i => i.Name == selectItemForLineDiscount);
                int SelectedItemIndex = _m_TakeAwayProductRS.FindIndex(i => i.Name == selectItemForLineDiscount);
                _m_TakeAwayProductRS.RemoveAt(SelectedItemIndex);
                if (qar_line_radio.IsChecked == true && float.Parse(discount.Text) < SelectedItem.Price)
                {
                    SelectedItem.discountPer = Math.Round((float.Parse(discount.Text) / SelectedItem.Price) * 100, 2);
                }
                else if (per_line_radio.IsChecked == true)
                {
                    SelectedItem.discountPer = Math.Round(float.Parse(discount.Text), 2);
                }
                _m_TakeAwayProductRS.Insert(SelectedItemIndex, SelectedItem);
                price_ListBox.Items.Refresh();
                calculateSum();

                this.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Total discount already applied please set it to 0");
                this.Visibility = Visibility.Visible;
            }

        }

        private void BtnMyPopupLineDiscCancel_Click(object sender, RoutedEventArgs e)
        {
            MyPopupLineDisc.IsOpen = false;
            this.Visibility = Visibility.Visible;
        }

        private void BtnPer_Click(object sender, RoutedEventArgs e)
        {
            //   this.Visibility = Visibility.Collapsed;
            MyPopupLineDisc.IsOpen = true;            
            string ItemName = (sender as Button).ToolTip.ToString();           
            lblitemName.Content = ItemName;
            selectItemForLineDiscount = ItemName;

        }

        private void BtnDec_Click(object sender, RoutedEventArgs e)
        {
            if (lblInvoiceNo.Content == null || lblInvoiceNo.Content.ToString() == "")
            {
                string ItemName = (sender as Button).ToolTip.ToString();

                m_TakeAwayProductRS SelectedItem = _m_TakeAwayProductRS.Find(i => i.Name == ItemName);
                int SelectedItemIndex = _m_TakeAwayProductRS.FindIndex(i => i.Name == ItemName);

                _m_TakeAwayProductRS.RemoveAt(SelectedItemIndex);

                if (SelectedItem.ItemCount > 1)
                {
                    SelectedItem.ItemCount -= 1;

                    _m_TakeAwayProductRS.Insert(SelectedItemIndex, SelectedItem);

                }
                price_ListBox.Items.Refresh();

                calculateSum();
            }
            else
            {
                MessageBox.Show("Can Not Modify, Bill Already Generated");
            }
        }

        private void BtnInc_Click(object sender, RoutedEventArgs e)
        {
            if (lblInvoiceNo.Content == null || lblInvoiceNo.Content.ToString() == "")

            {
                string ItemName = (sender as Button).ToolTip.ToString();
                m_TakeAwayProductRS SelectedItem = _m_TakeAwayProductRS.Find(i => i.Name == ItemName);
                int SelectedItemIndex = _m_TakeAwayProductRS.FindIndex(i => i.Name == ItemName);
                _m_TakeAwayProductRS.RemoveAt(SelectedItemIndex);

                SelectedItem.ItemCount += 1;

                _m_TakeAwayProductRS.Insert(SelectedItemIndex, SelectedItem);
                price_ListBox.Items.Refresh();
                calculateSum();
            }
            else
            {
                MessageBox.Show("Can Not Modify, Bill Already Generated");
            }
        }

        #endregion

        #region Event Total Discount

        private void BtnMyPopupTotalDiscOK_Click(object sender, RoutedEventArgs e)
        {
            if (float.Parse(txtOpeningBalanceDisc.Text) > 0)
                totalDiscountApplied = true;
            else
            {
                totalDiscountApplied = false;
            }

            MyPopupTotalDisc.IsOpen = false;
            double discountPer = 0;


            if (qar_total_radio.IsChecked == true)
            {
                discountPer = Math.Round(float.Parse(txtOpeningBalanceDisc.Text) / float.Parse(payableAmount.ToString()) * 100, 2);
            }
            else if (per_total_radio.IsChecked == true)
            {
                discountPer = float.Parse(txtOpeningBalanceDisc.Text);
            }

            foreach (m_TakeAwayProductRS element in _m_TakeAwayProductRS)
            {

                element.discountPer = discountPer;
            }
            price_ListBox.Items.Refresh();
            calculateSum();
            //MyPopupLineDisc.IsOpen = false;

            this.Visibility = Visibility.Visible;


        }

        private void BtnMyPopupTotalDiscCancel_Click(object sender, RoutedEventArgs e)
        {
            MyPopupTotalDisc.IsOpen = false;
        }



        #endregion

        #region Pre Bill
        private void PreBill_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                DataSet ds = new DataSet();
                connection.Open();
                NpgsqlCommand cmd_Prebill_GetData = new NpgsqlCommand("select c_invoice_id as BillNo,to_char(ic.created, 'DD-Mon-YYYY') as Date,to_char(ic.created, 'HH12:MI:SS') as Time,'T1' as tableName, grandtotal,total_items_count, total_items_count, apu.Name as userName,'CASH CUSTOMER' as CustmorName from C_invoice ic inner join ad_user_pos apu on ic.createdby = apu.ad_user_id  WHERE  ic.c_invoice_id = '" + invoiceno + "' and ic.ad_org_id =" + _OrgId + "  limit 1 ;", connection);//
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd_Prebill_GetData);
                DataTable dtPrebill = new DataTable();
                da.Fill(dtPrebill);
                connection.Close();

                connection.Open();
                NpgsqlCommand cmd_Items_GetData = new NpgsqlCommand("select productname, paroductarabicname,qtyinvoiced,saleprice,uomname from C_invoiceline where c_invoice_id = '" + invoiceno + "' And ad_org_id =" + _OrgId + "  ;", connection);//
                NpgsqlDataAdapter daa = new NpgsqlDataAdapter(cmd_Items_GetData);
                DataTable dtt = new DataTable();
                daa.Fill(dtt);
                connection.Close();
                ds.Tables.Add(dtPrebill);
                ds.Tables.Add(dtt);

                ReportDocument cryRpt = new ReportDocument();
                cryRpt.Load(@"C:\pos\component\Resources\reportFormats\CRPreBill.rpt");
                cryRpt.SetDataSource(ds);
                cryRpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, @" C:\pos\component\Resources\PreBill" + invoiceno + ".pdf");
                cryRpt.Refresh();
                MessageBox.Show("prebill genrated successfully");
                #endregion
                //  NavigationService.Navigate(new PreBillGenerate(invoiceno));
            }
            catch
            {

            }


        }



        #region Split Bill
        private void Spit_Back_Click(object sender, RoutedEventArgs e)
        {
            _m_TakeAwayProductRS.ForEach((x) => x.checkbox = "Hidden");
            price_ListBox.Items.Refresh();
            cancel_Order.Visibility = Visibility.Visible;
            KOT.Visibility = Visibility.Visible;
            Spit_Back.Visibility = Visibility.Hidden;
            Pre_bil.Visibility = Visibility.Visible;
            HR.Visibility = Visibility.Visible;
            completed.Visibility = Visibility.Visible;
            split.Visibility = Visibility.Hidden;
            SplitInvoiceNo.Visibility = Visibility.Hidden;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool? isChecked = (sender as CheckBox).IsChecked;
            string itemName = (sender as CheckBox).ToolTip.ToString();

            m_TakeAwayProductRS SelectedItem = _m_TakeAwayProductRS.Find(i => i.Name == itemName);
            int SelectedItemIndex = _m_TakeAwayProductRS.FindIndex(i => i.Name == itemName);
            if (isChecked == true)
                splitbill.Add(SelectedItem);

            if (isChecked == false)
                splitbill.RemoveAt(splitbill.FindIndex(i => i.Name == itemName));

        }

        private void Split_Click(object sender, RoutedEventArgs e)
        {


            try
            {

                using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
                {
                    connection.Open();
                    if (lblInvoiceNo.Content == null)
                    {
                        MessageBox.Show("Please Generate the Pri Bill !!");
                    }
                    else
                    {

                        //NpgsqlCommand cmd_Count_GetData = new NpgsqlCommand("select count(*) from c_invoicepaymentdetails where c_invoice_id='" + lblInvoiceNo.Content + "' And  ad_org_id=" + _OrgId + "  ;", connection);//
                        //NpgsqlDataReader _Count_GetData = cmd_Count_GetData.ExecuteReader();
                        //_Count_GetData.Read();
                        //if (_Count_GetData.GetInt32(0) > 0)
                        //{



                        connection.Close();
                        connection.Open();
                        var dateTimeOffset = DateTimeOffset.Parse(DateTime.Now.ToUniversalTime().ToString(), null);
                        var dt = (dateTimeOffset.DateTime);
                        NpgsqlCommand cmd = null;
                        double totalDiscount = 0;
                        double splitPayableamount = 0;
                        double splitItemCount = 0;
                        //Updating  c_invoiceline table
                        foreach (m_TakeAwayProductRS element in splitbill)
                        {
                            double discountValue = element.Price * element.ItemCount - element.TotalPrice;
                            totalDiscount += discountValue;
                            splitPayableamount += element.TotalPrice;
                            splitItemCount += element.ItemCount;
                            string c_invoiceline = "update c_invoiceline set c_invoice_id = " + splitinvoiceNo + " where c_invoice_id =" + lblInvoiceNo.Content + " and productname ='" + element.Name + "'";

                            cmd = new NpgsqlCommand(c_invoiceline, connection);
                            int rowInvoice = cmd.ExecuteNonQuery();
                        }

                        //Updating and adding record to c_invoice table
                        string c_invoice_Update = "update c_invoice set discountvalue= discountvalue - " + totalDiscount + ",grandtotal=grandtotal- " + splitPayableamount + ",grandtotal_round_off=grandtotal_round_off - " + splitPayableamount + ",total_items_count= total_items_count- " + splitItemCount + " where c_invoice_id=" + invoiceno + "";
                        cmd = new NpgsqlCommand(c_invoice_Update, connection);
                        cmd.ExecuteNonQuery();
                        string command_c_invoice = "insert into c_invoice(c_invoice_id,ad_client_id,ad_org_id,created,createdby,ad_user_id,mobile,discounttype,discountvalue,grandtotal,reason,grandtotal_round_off,total_items_count,balance,order_type,m_warehouse_id,c_bpartner_id,session_id)" +
                                               "OVERRIDING SYSTEM VALUE values" +
                                                 "('" + splitinvoiceNo + "'," + _clientId + "," + _OrgId + ",'" + dt + "'," +
                                                 "" + _UserID + "," + _UserID + ",0000000,1," + totalDiscount + "," +
                                                 splitPayableamount + ",'" + txtReason.Text + "'," + splitPayableamount + "," +
                                                 splitItemCount +
                                                 ",0,'Take away'," + _Warehouse_Id + "," + _Bpartner_Id + "," + _sessionID + ")";
                        cmd = new NpgsqlCommand(command_c_invoice, connection);
                        cmd.ExecuteNonQuery();

                        //updating and adding record to Payment table

                        //INSERT_c_invoicepaymentdetails.ExecuteNonQuery();

                        string c_invoicepaymentdetails_update = "update c_invoicepaymentdetails set cash= cash-" + splitPayableamount + "where c_invoice_id = " + lblInvoiceNo.Content + "";
                        cmd = new NpgsqlCommand(c_invoicepaymentdetails_update, connection);
                        cmd.ExecuteNonQuery();

                        string c_invoicepaymentdetails_insert = "INSERT INTO c_invoicepaymentdetails(" +
                                                      " ad_client_id, ad_org_id, c_invoice_id," +
                                                    "cash, card, exchange, redemption, iscomplementary, iscredit, createdby, updatedby)" +
                                                  "VALUES(" + _clientId + "," + _OrgId + "," + splitinvoiceNo + "," + splitPayableamount + ",0,0,0,'N','N'," + _UserID + "," + _UserID + ")";
                        cmd = new NpgsqlCommand(c_invoicepaymentdetails_insert, connection);
                        cmd.ExecuteNonQuery();


                        //END  updating and adding record to Payment table
                        MessageBox.Show("Split Successful");
                        _m_TakeAwayProductRS.ForEach((x) => x.checkbox = "Hidden");
                        price_ListBox.Items.Refresh();
                        cancel_Order.Visibility = Visibility.Visible;
                        KOT.Visibility = Visibility.Visible;
                        Spit_Back.Visibility = Visibility.Hidden;
                        Pre_bil.Visibility = Visibility.Visible;
                        HR.Visibility = Visibility.Visible;
                        completed.Visibility = Visibility.Visible;
                        split.Visibility = Visibility.Hidden;
                        //}
                        //else
                        //{
                        //    MessageBox.Show("Payment is Not Successful !!");
                        //}
                    }


                }

            }
            catch
            {
                MessageBox.Show("Split Not Successful !");
            }

        }

        private void Split1_Click(object sender, RoutedEventArgs e)
        {
            _m_TakeAwayProductRS.ForEach((x) => x.checkbox = "Visible");
            price_ListBox.Items.Refresh();
            // checkboxVisible = "Visible";
            //checkBoxsplitBill.Visibility = "Visible";
            cancel_Order.Visibility = Visibility.Hidden;
            KOT.Visibility = Visibility.Hidden;

            Pre_bil.Visibility = Visibility.Hidden;
            HR.Visibility = Visibility.Hidden;
            completed.Visibility = Visibility.Hidden;
            Spit_Back.Visibility = Visibility.Visible;
            split.Visibility = Visibility.Visible;
            SplitInvoiceNo.Visibility = Visibility.Visible;
            invoive_No p = new invoive_No();
            splitinvoiceNo = p.invoice_no();
            SplitInvoiceNo.Content = "Split Invoice No-" + splitinvoiceNo.ToString();
        }

        #endregion

        #region order Cancel
        private void PopupCancelOrderConfirm_Click(object sender, RoutedEventArgs e)
        {
            PopupCancelOrder.IsOpen = false;
            PopupCancelOrderReason.IsOpen = true;
        }

        private void CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (lblInvoiceNo.Content != null)
            {
                //   this.Visibility = Visibility.Collapsed;
                PopupCancelOrder.IsOpen = true;
            }
            else
            {
                MessageBox.Show("Please Generate the Pri Bill !!");
                return;
            }
        }

        private void PopupCancelReasonConfirm_Click(object sender, RoutedEventArgs e)
        {
            OrderCancelreason = null;
            PopupCancelOrderReason.IsOpen = false;
            if (rWrongOrder.IsChecked == true)
            {
                OrderCancelreason = " Wrong order";
            }

            else if (rCustomerLeft.IsChecked == true)
            {
                OrderCancelreason = "Customer Left";
            }

            if (OrderCancelreason != null)
            {
                PopupSecurityConfirmation.IsOpen = true;
            }
            else
            {
                MessageBox.Show("Please select The Reson");
                PopupCancelOrderReason.IsOpen = true;
            }
        }

        private void PopupSecurityConfirmationCancel_Click(object sender, RoutedEventArgs e)
        {
            PopupCancelOrderReason.IsOpen = false;
        }

        private void PopupSecurityConfirmationConfirm_Click(object sender, RoutedEventArgs e)
        {
            string securityCode = txtsecurityCode.Text;

            if (securityCode == "2020")
            {
                PopupSecurityConfirmation.IsOpen = false;
                try
                {
                    var dateTimeOffset = DateTimeOffset.Parse(DateTime.Now.ToUniversalTime().ToString(), null);
                    var dt = (dateTimeOffset.DateTime);
                    using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
                    {
                        connection.Open();
                        string command_c_invoice = "update  c_invoice set updated='" + dt + "',updatedby=" + _UserID + ",is_canceled='y',reason='" + OrderCancelreason + "' where c_invoice_id='" + lblInvoiceNo.Content + "'";
                        NpgsqlCommand cmd = new NpgsqlCommand(command_c_invoice, connection);
                        cmd.ExecuteNonQuery();
                    }
                    _m_TakeAwayProductRS.Clear();
                    calculateSum();
                    price_ListBox.Items.Refresh();
                    this.Visibility = Visibility.Visible;
                    MessageBox.Show("Order Deleted");

                    try
                    {
                        OrdersApicall("POSOrderCancel");
                        using (NpgsqlConnection connection3 = new NpgsqlConnection(connstring))
                        {
                            connection3.Open();
                            NpgsqlCommand c_invoiceSync = new NpgsqlCommand("update  c_invoice set is_onsync='Y' where c_invoice_id=" + invoiceno + "", connection3);
                            c_invoiceSync.ExecuteNonQuery();

                        }
                    }
                    catch
                    {
                        using (NpgsqlConnection connection3 = new NpgsqlConnection(connstring))
                        {
                            connection3.Open();
                            NpgsqlCommand c_invoiceSync = new NpgsqlCommand("update  c_invoice set is_onsync='N' where c_invoice_id=" + invoiceno + "", connection3);
                            c_invoiceSync.ExecuteNonQuery();

                        }
                    }
                }
                catch
                {

                }
            }

        }

        private void PopupCancelOrderCancel_Click(object sender, RoutedEventArgs e)
        {
            PopupCancelOrder.IsOpen = false;
            //PopupCancelOrderReason.IsOpen = true;
        }

        #endregion
        #region Hold And ReCall
        private void HR_Click(object sender, RoutedEventArgs e)
        {

            if (_m_TakeAwayProductRS.Count > 0)
            {
                Hold();


            }
            else if (_m_TakeAwayProductRS.Count <= 0)
            {
                Recall();
            }


        }
        private void Recall()
        {
            productCat_ListBox.Visibility = Visibility.Hidden;
            Product_ListBox.Visibility = Visibility.Hidden;
            onHoldOrders.Visibility = Visibility.Visible;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
                {
                    connection.Open();
                    string command_c_invoice = "select c_invoice_id, grandtotal,total_items_count from c_invoice where is_onhold='Y' and is_completed='N'";
                    NpgsqlCommand cmd = new NpgsqlCommand(command_c_invoice, connection);
                    NpgsqlDataReader _holdInvoiceData = cmd.ExecuteReader();
                    _m_onHoldInvoice.Clear();
                    while (_holdInvoiceData.Read())
                    {
                        _m_onHoldInvoice.Add(new m_onHoldInvoice()
                        {
                            c_invoice_id = _holdInvoiceData.GetInt32(0),
                            grandtotal = "QR " + _holdInvoiceData.GetString(1),
                            total_items_count = _holdInvoiceData.GetString(2) + " Items",
                        });
                    }
                    onHoldOrders.ItemsSource = _m_onHoldInvoice;
                    onHoldOrders.Items.Refresh();
                }
            }

            catch
            {

            }

        }

        private void Hold()
        {
            if (lblInvoiceNo.Content != null)
            {
                try
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
                    {
                        connection.Open();
                        var dateTimeOffset = DateTimeOffset.Parse(DateTime.Now.ToUniversalTime().ToString(), null);
                        var dt = (dateTimeOffset.DateTime);

                        string command_c_invoice = "update c_invoice set is_onhold='Y' where c_invoice_id=" + lblInvoiceNo.Content + "";
                        NpgsqlCommand cmd = new NpgsqlCommand(command_c_invoice, connection);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Status updated to Hold");
                    _m_TakeAwayProductRS.Clear();
                    calculateSum();
                    price_ListBox.Items.Refresh();
                }
                catch
                {

                }
            }
            else
            {
                MessageBox.Show("Pre Bill not generated yet");
            }
        }

        private void OnHoldOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic holdInvoiceNo = onHoldOrders.SelectedItem as dynamic;
            int id = holdInvoiceNo.c_invoice_id;
            if (id > 0)
            {
                lblInvoiceNo.Content = id;
                try
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
                    {

                        connection.Open();
                        string command_c_invoice = "select productname,qtyinvoiced,saleprice,linetotalamt,discountvalue from c_invoiceline where c_invoice_id=" + id + "";
                        NpgsqlCommand cmd = new NpgsqlCommand(command_c_invoice, connection);
                        NpgsqlDataReader _HoldOrders_GetData = cmd.ExecuteReader();

                        price_ListBox.ItemsSource = "";
                        _m_TakeAwayProductRS.Clear();
                        while (_HoldOrders_GetData.Read())
                        {
                            _m_TakeAwayProductRS.Add(new m_TakeAwayProductRS()
                            {
                                Name = _HoldOrders_GetData.GetString(0),
                                Price = _HoldOrders_GetData.GetInt32(2),
                                ItemCount = _HoldOrders_GetData.GetInt32(1),
                                discountPer = Math.Round(float.Parse(_HoldOrders_GetData.GetInt32(4).ToString()) / _HoldOrders_GetData.GetInt32(2) * 100, 2)
                            });
                        }
                        price_ListBox.ItemsSource = _m_TakeAwayProductRS;
                        calculateSum();
                        price_ListBox.Items.Refresh();
                    }
                    // MessageBox.Show("Status updated to Hold");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Selected invoice not found");
            }
        }
        #endregion

        #region Event
        private void Order_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new orderonline());
        }



        private void TxtTotalDiscountPer_Click(object sender, RoutedEventArgs e)
        {
            MyPopupTotalDisc.IsOpen = true;
            billAmt.Content = lblRS.Content;
            lblbillNo.Content = lblInvoiceNo.Content;
        }



        private void TxtCatSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (Product_ListBoxSelect > 0)
                {


                    dynamic PC = Product_ListBox.SelectedItem as dynamic;
                    int id = PC.id;

                    if (txtCatSearch.Text.Trim() != null)
                    {
                        NpgsqlConnection connection = new NpgsqlConnection(connstring);
                        connection.Open();

                        NpgsqlCommand cmd_TakeAwayProductCAT_GetData = new NpgsqlCommand("select m_product_id,name,image from m_product  where ad_org_id=" + _OrgId + " And m_product_category_id='" + id + "' And upper(name) like '%" + txtCatSearch.Text.Trim().ToUpper() + "%' ;", connection);//
                        NpgsqlDataReader _TakeAwayProductCAT_GetData = cmd_TakeAwayProductCAT_GetData.ExecuteReader();
                        m_TakeAwayProductCAT.Clear();
                        // _m_TakeAwayProduct.Clear();
                        //  Product_ListBox.ItemsSource = "";
                        productCat_ListBox.ItemsSource = "";
                        while (_TakeAwayProductCAT_GetData.Read())
                        {

                            m_TakeAwayProductCAT.Add(new m_TakeAwayProductCAT()
                            {
                                id = int.Parse(_TakeAwayProductCAT_GetData["m_product_id"].ToString()),
                                Name = _TakeAwayProductCAT_GetData["name"].ToString(),
                                ImgPath = _TakeAwayProductCAT_GetData["image"].ToString()

                            });
                        }

                        connection.Close();

                        if (m_TakeAwayProductCAT.Count == 0)
                        {
                            MessageBox.Show("Product  Not Found!");

                            return;

                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {

                                productCat_ListBox.ItemsSource = m_TakeAwayProductCAT;
                            });
                        }
                    }
                    else
                    {
                        BindTakeAwayProduct(id);
                    }
                }
                else
                {
                    MessageBox.Show("Please Select Product Category !");
                }


            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Product Category Not Found!   =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Product Category Not Found!", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }
        }
        private void KOT_Click(object sender, RoutedEventArgs e)
        {

            invoive_No p = new invoive_No();
            invoiceno = p.invoice_no();

            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connstring);

                connection.Open();
                var dateTimeOffset = DateTimeOffset.Parse(DateTime.Now.ToUniversalTime().ToString(), null);
                var dt = (dateTimeOffset.DateTime);

                string command_c_invoice = "insert into c_invoice(c_invoice_id,ad_client_id,ad_org_id,created,createdby,ad_user_id,mobile,discounttype,discountvalue,grandtotal,reason,grandtotal_round_off,total_items_count,balance,order_type,m_warehouse_id,c_bpartner_id,session_id)" +
                                                               "OVERRIDING SYSTEM VALUE values('" + invoiceno + "'," + _clientId + "," + _OrgId + ",'" + dt + "'," + _UserID + "," + _UserID + ",0000000,1," + (totalAmount - payableAmount) + "," + payableAmount + ",'" + txtReason.Text + "'," + payableAmount + "," + totalItems + ",0,'Dine In'," + _Warehouse_Id + "," + _Bpartner_Id + "," + _sessionID + ")";
                NpgsqlCommand cmd = new NpgsqlCommand(command_c_invoice, connection);
                cmd.ExecuteNonQuery();
                foreach (m_TakeAwayProductRS element in _m_TakeAwayProductRS)
                {
                    double discountValue = element.Price * element.ItemCount - element.TotalPrice;
                    string c_invoiceline = "insert into c_invoiceline(ad_client_id,ad_org_id,created,createdby,ad_user_id,c_invoice_id,productname,saleprice,qtyinvoiced,discountvalue,linetotalamt,m_table_id)" +
                    " values(" + _clientId + "," + _OrgId + ",'" + dt + "'," + _UserID + "," + _UserID + "," + invoiceno + ",'" + element.Name + "'," + element.Price + "," + element.ItemCount + "," + discountValue + "," + element.TotalPrice + ",'" + selectedTable + "')";
                    cmd = new NpgsqlCommand(c_invoiceline, connection);
                    int rowInvoice = cmd.ExecuteNonQuery();
                }
                connection.Close();
                lblInvoiceNo.Content = invoiceno;
                try
                {
                    OrdersApicall("POSOrderDrafted");
                    using (NpgsqlConnection connection3 = new NpgsqlConnection(connstring))
                    {
                        connection3.Open();
                        NpgsqlCommand c_invoiceSync = new NpgsqlCommand("update  c_invoice set is_onsync='Y' where c_invoice_id=" + invoiceno + "", connection3);
                        c_invoiceSync.ExecuteNonQuery();

                    }
                }
                catch
                {
                    using (NpgsqlConnection connection3 = new NpgsqlConnection(connstring))
                    {
                        connection3.Open();
                        NpgsqlCommand c_invoiceSync = new NpgsqlCommand("update  c_invoice set is_onsync='N' where c_invoice_id=" + invoiceno + "", connection3);
                        c_invoiceSync.ExecuteNonQuery();

                    }
                }
            }

            catch
            {

            }

            try
            {
                if (lblInvoiceNo.Content != null)
                {

                    int kotid = 0;
                    using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
                    {
                        connection.Open();
                        NpgsqlCommand cmd_select_sequenc_no_ad_user_pos = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'c_kot';", connection);
                        NpgsqlDataReader _get__Ad_sequenc_no_ad_user_pos = cmd_select_sequenc_no_ad_user_pos.ExecuteReader();
                        if (_get__Ad_sequenc_no_ad_user_pos.Read())
                        {
                            Sequenc_kotid = _get__Ad_sequenc_no_ad_user_pos.GetInt32(4) + 1;
                            kotid = Sequenc_kotid;

                        }

                        connection.Close();
                        connection.Open();
                        var dateTimeOffset = DateTimeOffset.Parse(DateTime.Now.ToUniversalTime().ToString(), null);
                        var dt = (dateTimeOffset.DateTime);

                        string command_c_Kot = "insert into c_kot( c_kot_id,ad_client_id,ad_org_id,ad_user_id,m_warehouse_id,m_table_id,c_invoice_id,m_terminal_id,orderby,order_type,kot_type,delivery_type,isprinted,grandtotal_round_off,covers_count, createdby,updatedby)" +
                                      "OVERRIDING SYSTEM VALUE values('" + Sequenc_kotid + "'," + _clientId + "," + _OrgId + ",'" + _UserID + "'," + _Warehouse_Id + "," + selectedTable + ",'" + lblInvoiceNo.Content + "',1,'" + _UserID + "','C','C','C','Y'," + payableAmount + ",'2'," + _UserID + "," + _UserID + ")";
                        NpgsqlCommand cmd = new NpgsqlCommand(command_c_Kot, connection);
                        cmd.ExecuteNonQuery();
                        connection.Close();
                        connection.Open();
                        NpgsqlCommand cmd_update_sequenc_no_ad_user_pos = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_kotid + " WHERE name = 'c_kot';", connection);
                        cmd_update_sequenc_no_ad_user_pos.ExecuteReader();
                        connection.Close();

                        foreach (m_TakeAwayProductRS element in _m_TakeAwayProductRS)
                        {
                            int _kotLineNo = 0;

                            connection.Open();
                            NpgsqlCommand cmd_terminal_Kot = new NpgsqlCommand("select count (*) as totalrow  FROM c_kotline where m_terminal_id='" + element.m_terminal_id + "' and c_invoice_id='" + lblInvoiceNo.Content + "';", connection);
                            Npgsql.NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd_terminal_Kot);
                            DataTable terminaldt = new DataTable();
                            da.Fill(terminaldt);
                            connection.Close();

                            if (terminaldt.Rows[0]["totalrow"].ToString() != "0")
                            {

                            }
                            else
                            {
                                connection.Open();
                                NpgsqlCommand cmd_select_sequenc_Kotno = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'kotno';", connection);
                                NpgsqlDataReader _select_sequenc_Kotno = cmd_select_sequenc_Kotno.ExecuteReader();
                                if (_select_sequenc_Kotno.Read())
                                {
                                    Sequenc_id = _select_sequenc_Kotno.GetInt32(4) + 1;
                                    kotno = Sequenc_id;
                                }

                                connection.Close();
                                connection.Open();
                                NpgsqlCommand cmd_update_Kot = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'kotno';", connection);
                                cmd_update_Kot.ExecuteReader();
                                connection.Close();
                            }

                            connection.Open();
                            NpgsqlCommand cmd_select_sequenc_no_Kotno = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'c_kotline';", connection);
                            NpgsqlDataReader _select_sequenc_no_Kotno = cmd_select_sequenc_no_Kotno.ExecuteReader();
                            if (_select_sequenc_no_Kotno.Read())
                            {
                                Sequenc_kotlineid = _select_sequenc_no_Kotno.GetInt32(4) + 1;
                                _kotLineNo = Sequenc_kotlineid;

                            }

                            connection.Close();

                            connection.Open();
                            string c_invoiceline = "insert into c_kotline(c_kotline_id,ad_client_id,ad_org_id,c_kot_id,createdby,updatedby,m_product_id,Productname,paroductarabicname,productbarcode,c_uom_id,uomname,qtyinvoiced,qtyentered,saleprice,costprice,linetotalamt,m_pricelist_id,m_terminal_id,m_table_id,kotnotes,c_invoice_id,ad_user_id,kotno)" +
                            " values('" + Sequenc_kotlineid + "'," + _clientId + "," + _OrgId + ",'" + kotid + "'," + _UserID + "," + _UserID + "," + element.id + ",'" + element.Name + "','" + element.paroductarabicname + "','" + element.productbarcode + "','" + element.c_uom_id + "','" + element.uomname + "'," + element.ItemCount + "," + element.ItemCount + "," + element.saleprice + "," + element.Price + "," + element.TotalPrice + "," + element.m_pricelist_id + "," + element.m_terminal_id + ",'" + selectedTable + "','Na','" + lblInvoiceNo.Content + "','" + _UserID + "','" + kotno + "')";
                            cmd = new NpgsqlCommand(c_invoiceline, connection);

                            int rowInvoice = cmd.ExecuteNonQuery();
                            connection.Close();
                            connection.Open();
                            NpgsqlCommand cmd_update_KotLine = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_kotlineid + " WHERE name = 'c_kotline';", connection);
                            cmd_update_KotLine.ExecuteReader();
                            connection.Close();



                        }
                        connection.Open();
                        NpgsqlCommand cmd_select_Kotno = new NpgsqlCommand("select DISTINCT kotno from c_kotline where c_invoice_id='" + lblInvoiceNo.Content + "';", connection);
                        Npgsql.NpgsqlDataAdapter da_select_Kotno = new NpgsqlDataAdapter(cmd_select_Kotno);
                        DataTable dt_select_Kotno = new DataTable();
                        da_select_Kotno.Fill(dt_select_Kotno);
                        if (dt_select_Kotno.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt_select_Kotno.Rows.Count; i++)
                            {
                                reportview(lblInvoiceNo.Content.ToString(), int.Parse(dt_select_Kotno.Rows[i]["kotno"].ToString()));
                            }
                        }


                        connection.Close();
                        MessageBox.Show("Kot is  generated !!");
                    }
                }
                else
                {
                    MessageBox.Show("Please  generated Pre Bill !!");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                MessageBox.Show("Kot is Not generated !!");
               
            }

        }
        private void reportview(string _invoiceno, int _kotno)
        {
            DataSet ds = new DataSet();

            NpgsqlConnection connection = new NpgsqlConnection(connstring);
            connection.Open();
            NpgsqlCommand cmd_select_sequenc_no_ad_user_pos = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'KotGenerate';", connection);
            NpgsqlDataReader _get__Ad_sequenc_no_ad_user_pos = cmd_select_sequenc_no_ad_user_pos.ExecuteReader();

            if (_get__Ad_sequenc_no_ad_user_pos.Read())
            {
                Sequenc_id = _get__Ad_sequenc_no_ad_user_pos.GetInt32(4) + 1;
            }
            connection.Close();



            connection.Open();
            NpgsqlCommand cmd_User_GetData = new NpgsqlCommand("select c_invoice_id as BillNo,to_char(ck.created, 'DD-Mon-YYYY') as Date,to_char(ck.created, 'HH12:MI:SS') as Time,'Na' as tableName, " + _kotno + " as KotNo,'Neelam' as OrderBy from c_kot ck  WHERE  ck.c_invoice_id = '" + _invoiceno + "'   limit 1 ;", connection);//
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd_User_GetData);
            DataTable dt = new DataTable();
            da.Fill(dt);
            connection.Close();

            connection.Open();
            NpgsqlCommand cmd_Items_GetData = new NpgsqlCommand("select productname, paroductarabicname from c_kotline where c_invoice_id = '" + _invoiceno + "' and kotno='" + _kotno + "';", connection);//
            NpgsqlDataAdapter daa = new NpgsqlDataAdapter(cmd_Items_GetData);
            DataTable dtt = new DataTable();
            daa.Fill(dtt);
            connection.Close();
            ds.Tables.Add(dt);
            ds.Tables.Add(dtt);




            ReportDocument cryRpt = new ReportDocument();
            cryRpt.Load(@"C:\pos\component\Resources\reportFormats\CRKot.rpt");
            cryRpt.SetDataSource(ds);
            //   cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "Crystal");
            cryRpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, @"C:\pos\component\Resources\reports\Kot" + Sequenc_id + "_" + _kotno + ".pdf");
            cryRpt.Refresh();



            connection.Open();
            NpgsqlCommand cmd_update_sequenc_no_ad_user_pos = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'KotGenerate';", connection);
            cmd_update_sequenc_no_ad_user_pos.ExecuteReader();
            connection.Close();
        }



        private void Completed_Click(object sender, RoutedEventArgs e)
        {
            if (lblInvoiceNo.Content != null)
            {
                try

                {

                    using (NpgsqlConnection connection = new NpgsqlConnection(connstring))
                    {
                        connection.Open();
                        NpgsqlCommand cmd_Count_GetData = new NpgsqlCommand("select count(*) from c_invoicepaymentdetails where c_invoice_id='" + lblInvoiceNo.Content + "' And  ad_org_id=" + _OrgId + "  ;", connection);//
                        NpgsqlDataReader _Count_GetData = cmd_Count_GetData.ExecuteReader();
                        _Count_GetData.Read();
                        //   connection.Close();
                        if (_Count_GetData.GetInt32(0) > 0)
                        {
                            connection.Close();

                            connection.Open();
                            NpgsqlCommand cmd_update_c_invoice = new NpgsqlCommand("update c_invoice set is_completed='Y' where c_invoice_id='" + lblInvoiceNo.Content + "' ;", connection);
                            cmd_update_c_invoice.ExecuteNonQuery();
                            connection.Close();

                            MessageBox.Show("Status updated to complete");
                            _m_TakeAwayProductRS.Clear();
                            calculateSum();
                            price_ListBox.Items.Refresh();

                            NavigationService.Navigate(new POSsystem());
                            NavigationService.Navigate(new BillGenerate(invoiceno.ToString(), "0"));
                        }
                        else
                        {
                            MessageBox.Show("Payment is Not Successful !!");
                        }
                    }

                }



                catch
                {
                    MessageBox.Show("Payment is Not Successful !!");
                }
            }
            else
            {
                MessageBox.Show("Invoice number not genetated yet ");
            }
        }
        public void calculateSum()
        {
            payableAmount = 0;
            totalItems = 0;
            totalAmount = 0;
            double total_discount_amount = 0;
            int itemTypes = 0;
            foreach (m_TakeAwayProductRS element in _m_TakeAwayProductRS)
            {
                itemTypes++;
                totalItems += element.ItemCount;
                payableAmount += element.TotalPrice;
                totalAmount += element.Price * element.ItemCount;
                total_discount_amount += element.discountAmt;
            }

            lblRS.Content = "QR " + String.Format("{0:N}", payableAmount);
            lblitems.Content = totalItems.ToString() + " ITEMS";
            DiscountAmt.Content = "QR " + String.Format("{0:N}", totalAmount - payableAmount);
            txtTotalDiscountPer.Content = "QR " + String.Format("{0:N}", total_discount_amount).ToString();

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Product_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            dynamic PC = Product_ListBox.SelectedItem as dynamic;
            if (PC == null)
            {

            }
            else
            {
                Product_ListBoxSelect = PC.id;
                int id = PC.id;
                BindTakeAwayProduct(id);
            }


        }

        private void ProductCat_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic PC = productCat_ListBox.SelectedItem as dynamic;
            if (PC == null)
            {

            }
            else
            {
                int id = PC.id;
                string Name = PC.Name;
                if (lblInvoiceNo.Content == null || lblInvoiceNo.Content.ToString() == "")
                {
                    BindTakeAwayProductRS(id);
                    selectedItemForEditNotes = PC.Name;
                }
                else
                {
                    MessageBox.Show("Can Not Modify, Bill Already Generated");
                }
                calculateSum();
            }


        }



        private void POS_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new POSsystem());
        }

        private void DineIn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DineIn());

        }




        #endregion

        #region Popup_KeyUp
        private void GridpaymentPopup_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().ToUpper() == "F10")
            {
                BtnComplete_Click(sender, e);
            }
            if (e.Key.ToString().ToUpper() == "ESCAPE")
            {
                BtnClose_Click(sender, e);
            }
        }

        private void GridMyPopupNotes_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().ToUpper() == "F4")
            {
                BtnPopupNotesSubmit_Click(sender, e);
            }
            if (e.Key.ToString().ToUpper() == "ESCAPE")
            {
                BtnPopupNotesVancel_Click(sender, e);
            }
        }

        private void GridMyPopupLineDisc_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().ToUpper() == "ENTER")
            {
                MyPopupLineDiscSubmit_Click(sender, e);
            }
            if (e.Key.ToString().ToUpper() == "ESCAPE")
            {
                BtnMyPopupLineDiscCancel_Click(sender, e);
            }
        }

        private void GridMyPopupTotalDisc_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().ToUpper() == "ENTER")
            {
                BtnMyPopupTotalDiscOK_Click(sender, e);
            }
            if (e.Key.ToString().ToUpper() == "ESCAPE")
            {
                BtnMyPopupTotalDiscCancel_Click(sender, e);
            }
        }

        private void GridMyPopupOrderReason_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().ToUpper() == "F4")
            {
                PopupCancelReasonConfirm_Click(sender, e);
            }
            if (e.Key.ToString().ToUpper() == "ESCAPE")
            {
                PopupCancelOrderCancel_Click(sender, e);
            }
        }

        private void GridMyPopupSecurityConfirmation_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().ToUpper() == "F4")
            {
                PopupSecurityConfirmationConfirm_Click(sender, e);
            }
            if (e.Key.ToString().ToUpper() == "ESCAPE")
            {
                PopupSecurityConfirmationCancel_Click(sender, e);
            }
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key.ToString().ToUpper() == "F9")
                {


                    paymentPopup.IsOpen = true;
                    TotalAmt.Content = "QR " + String.Format("{0:N}", totalAmount);
                    PayableAmt.Content = "QR " + String.Format("{0:N}", payableAmount);
                    BalanceAmt.Content = "QR " + String.Format("{0:N}", payableAmount);
                    txtBal.Text = payableAmount.ToString();



                }
                if (e.Key.ToString().ToUpper() == "F5")
                {
                    MyPopupNotes.IsOpen = true;
                    lblItemName.Content = "items";
                }
                //if (e.Key.ToString().ToUpper() == "Ctrl+D")
                //{
                //    TxtTotalDiscountPer_Click(sender, e);
                //}
                if (e.Key == Key.D && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    
                    MyPopupTotalDisc.IsOpen = true;
                    billAmt.Content = lblRS.Content;
                }
                if (e.Key.ToString().ToUpper() == "F3")
                {
                    CancelOrder_Click(sender, e);
                }
                if (e.Key.ToString().ToUpper() == "F7")
                {
                    KOT_Click(sender, e);
                }
                if (e.Key.ToString().ToUpper() == "F12")
                {
                    PreBill_Click(sender, e);
                }
                if (e.Key.ToString().ToUpper() == "F8")
                {
                    HR_Click(sender, e);
                }
                if (e.Key.ToString().ToUpper() == "F10")
                {
                    Split_Click(sender, e);
                }
                if (e.Key.ToString().ToUpper() == "F6")
                {
                    txtCatSearch.Focus();
                }               
                if (e.Key == Key.S && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {

                    Split1_Click(sender, e);
                }
                if (e.Key == Key.T && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {

                    MyPopupTotalDisc.IsOpen = true;
                    billAmt.Content = lblRS.Content;
                }
                if (e.Key == Key.I && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                {

                    DineIn_Click(sender, e);
                }
                if (e.Key == Key.O && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                {

                    Order_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
              
            }

        }

        #endregion

        public void OrdersApicall(string operation)
        {
            List<PaymentDetails> _paymentDetails = new List<PaymentDetails>();
            _paymentDetails.Add(new PaymentDetails()
            {
                amount = "0",
                paymenttype = "EXCHANGE"
            });
            _paymentDetails.Add(new PaymentDetails()
            {
                amount = paidCash.ToString(),
                paymenttype = "CASH",
            });
            _paymentDetails.Add(new PaymentDetails()
            {
                amount = "0",
                paymenttype = "CARD"
            });
            _paymentDetails.Add(new PaymentDetails()
            {
                amount = "0",
                paymenttype = "LOYALTY"
            });
            OrderHeaders _orderHeader = new OrderHeaders();
            _orderHeader.posId = 0;

            _orderHeader.clientId = _clientId;
            _orderHeader.orgId = _OrgId;
            _orderHeader.userId = _UserID;
            _orderHeader.warehouseId = _Warehouse_Id;
            _orderHeader.accountSchemaId = 1000029.ToString();
            _orderHeader.paymentTermId = 1000014;
            _orderHeader.pricelistId = 1000032;
            _orderHeader.adTableId = 318;
            _orderHeader.periodId = 1000721;
            _orderHeader.currencyId = 293;
            _orderHeader.cashbookId = 1000021;
            _orderHeader.salesrepId = _UserID;
            _orderHeader.businessPartnerId = _Bpartner_Id;
            _orderHeader.createdDate = invoiceDatetime.ToString();
            _orderHeader.customerName = "CASH CUSTOMER";
            _orderHeader.isReturned = "N";
            _orderHeader.docNo = "";
            _orderHeader.creditName = "";
            _orderHeader.mobilenumber = 0;
            _orderHeader.qid = "";
            _orderHeader.cashAmount = payableAmount;
            _orderHeader.cardAmount = 0;
            _orderHeader.exchangeAmount = 0;
            _orderHeader.giftAmount = 0;
            _orderHeader.redemptionAmount = 0;
            _orderHeader.dueAmount = 0;
            _orderHeader.lossAmount = 0;
            _orderHeader.extraAmount = 0;
            _orderHeader.totalAmount = payableAmount;
            _orderHeader.refInvoiceId = 0;
            _orderHeader.totalLines = _m_TakeAwayProductRS.Count;
            _orderHeader.IsCash = "Y";
            _orderHeader.IsCard = "N";
            _orderHeader.paidAmount = payableAmount;
            _orderHeader.warehouseNo = _Warehouse_Id;

            POSOrderDrafted _POSOrderDrafted = new POSOrderDrafted();

            _POSOrderDrafted.macAddress = _macAddress_post;
            _POSOrderDrafted.clientId = _clientId.ToString();
            _POSOrderDrafted.orgId = _OrgId.ToString();
            _POSOrderDrafted.warehouseId = _Warehouse_Id.ToString();
            _POSOrderDrafted.userId = _UserID.ToString();
            _POSOrderDrafted.version = _version_post.ToString();
            _POSOrderDrafted.appName = _appName_post;
            _POSOrderDrafted.operation = operation;
            _POSOrderDrafted.authorizedBy = _UserID.ToString();
            _POSOrderDrafted.sessionId = _sessionID.ToString();
            _POSOrderDrafted.reason = OrderCancelreason;
            _POSOrderDrafted.OrderHeaders = _orderHeader;
            _POSOrderDrafted.OrderDetails = _orderDetails;
            _POSOrderDrafted.PaymentDetails = _paymentDetails;

            object jsonq = JsonConvert.SerializeObject(_POSOrderDrafted);


            var loginApiStringResponce = PostgreSQL.ApiCallPost(jsonq.ToString());
            MessageBox.Show("Sync to Master successfull");


        }

    }
}