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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Web.UI.WebControls;
using System.Data;


namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for ViewCustomers.xaml
    /// </summary>
    public partial class ViewCustomers : Page
    {
        #region 
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        int _Bpartner_Id = Int32.Parse(Application.Current.Properties["BpartnerId"].ToString());
        int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());
        int page_no = 0;
        int itemPerpage = 20;
        public string connstring = PostgreSQL.ConnectionString;
        string imagePath = "";


        public List<m_CustomersList> m_CustomersList_items = new List<m_CustomersList>();
        public List<M_City>  _m_City_items = new List<M_City>();

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
   
        private dynamic CustomersApiStringResponce,
        customersApiJOSNResponce;
        public dynamic CustomersApiJOSNResponce { get => customersApiJOSNResponce; set => customersApiJOSNResponce = value; }
        private string jsonq;
        private int CheckServerError = 0;

        #endregion
        public ViewCustomers()
        {
            InitializeComponent();
            Enabled();
            CustomersList();
            BindCity(1);
            var x=Customer_ListBox.SelectedItem;
            
            //  BindBpartner();
            // Customer_ListBox.Loaded += Customer_ListBox_Loaded;
        }
        public void Enabled()
        {
            txtName.IsEnabled = false;
            txtQID.IsEnabled = false;
            txtMobile.IsEnabled = false;
            txtEmail.IsEnabled = false;
            txtAddress.IsEnabled = false;
            PartnerGroup_DropDown.IsEnabled = false;
            btn_edit.IsEnabled = false;
            City_DropDown.IsEnabled = false;
            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;

            btnupload.Visibility = Visibility.Hidden;

        }
        #region Bind Data
       
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
                m_CustomersList_items.Add(new m_CustomersList()
                {
                    id = 0,
                    Name = "All",

                });
                while (_CustomersList_GetData.Read())
                {
                    m_CustomersList_items.Add(new m_CustomersList()
                    {
                        id = Int32.Parse(_CustomersList_GetData["c_bpartner_id"].ToString()),
                        Name = _CustomersList_GetData["name"].ToString()

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
        public void CustomersWise(int id)
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_Se_GetData = new NpgsqlCommand("select c_bpartner_id as id,name,image from  c_bpartner  where ad_org_id=" + _OrgId + " And c_bpartner_id='" + id + "' and iscustomer='Y' ;", connection);//
                Npgsql.NpgsqlDataAdapter dacustwise = new NpgsqlDataAdapter(cmd_Se_GetData);
                DataTable dtcustwise = new DataTable();
                dacustwise.Fill(dtcustwise);
                connection.Close();
                Customer_ListBox.ItemsSource = "";
                if (dtcustwise.Rows.Count > 0)
                {

                    Customer_ListBox.ItemsSource = dtcustwise.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add customer list!");
                    return;
                }

            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In customer Is Not Found!  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In customer Is Not Found!", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }



        }
        public void CustomersLst(int pageNo)
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
             
                NpgsqlCommand cmd_Search_GetData = new NpgsqlCommand("select c_bpartner_id as id,name,image from  c_bpartner  where ad_org_id=" + _OrgId + "  and iscustomer='Y' order by name offset '" + pageNo * itemPerpage + "' ROWS FETCH NEXT '" + itemPerpage + "'  ROWS ONLY  ;", connection);//
                Npgsql.NpgsqlDataAdapter dacust = new NpgsqlDataAdapter(cmd_Search_GetData);
                DataTable dtcust = new DataTable();
                dacust.Fill(dtcust);
                connection.Close();
                Customer_ListBox.ItemsSource = "";
                if (dtcust.Rows.Count > 0)
                {

                    Customer_ListBox.ItemsSource = dtcust.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add customer list!");
                    return;
                }
               
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In customer Is Not Found!  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In customer Is Not Found!", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }



        }
        public void Search(string Key)
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_Search_GetData = new NpgsqlCommand("select c_bpartner_id as id,name,image from  c_bpartner  where ad_org_id=" + _OrgId + " and iscustomer='Y' And upper(name) LIKE  '" + Key.ToUpper() + "%' ;", connection);//
                Npgsql.NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd_Search_GetData);
                DataTable dt = new DataTable();
                da.Fill(dt);
                connection.Close();
                Customer_ListBox.ItemsSource = "";
                if (dt.Rows.Count > 0)
                {

                    Customer_ListBox.ItemsSource = dt.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add customer list!");
                    return;
                }
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In customer Is Not Found!  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In customer Is Not Found!", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void BindCity(int CID)
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_City_GetData = new NpgsqlCommand("select city_id,city_name from city where country_ID='" + CID + "'; ", connection);//
                NpgsqlDataReader _City_GetData = cmd_City_GetData.ExecuteReader();

                while (_City_GetData.Read())
                {
                     _m_City_items.Add(new M_City()
                    {
                        CityID = Int32.Parse(_City_GetData["city_id"].ToString()),
                        Name = _City_GetData["city_name"].ToString()

                    });
                }
                connection.Close();


                this.Dispatcher.Invoke(() =>
                {

                    City_DropDown.ItemsSource =  _m_City_items;
                });
                if ( _m_City_items.Count == 0)
                {
                    MessageBox.Show("Please Add City!");

                    return;

                }
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In City  Not Bind  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In City Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }

      

     


        #endregion

        #region Event
        private void Customer_DropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic CD = Customer_DropDown.SelectedValue as dynamic;
            int id = CD.id;
            if (Customer_DropDown.SelectedIndex <= 0)
            {
                CustomersLst(0);
            }
            else
            {
                CustomersWise(id);
            }
        }

        private void Btn_edit_Click(object sender, RoutedEventArgs e)
        {
            txtName.IsEnabled = true;
            txtQID.IsEnabled = true;
            txtMobile.IsEnabled = true;
            txtEmail.IsEnabled = true;
            txtAddress.IsEnabled = true;
            PartnerGroup_DropDown.IsEnabled = true;
            City_DropDown.IsEnabled = true;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            btnupload.Visibility = Visibility.Visible;
        }

        private void Product_Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (product_Search.Text != null)
            {
               Search(product_Search.Text);
            }
            else
            {
                CustomersLst(0);
            }
        }

        private void Customer_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            dynamic CL = Customer_ListBox.SelectedItem as dynamic;
            if (CL!=null)
            {
                dynamic id = CL[0];
                btn_edit.IsEnabled = true;
                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_list_GetData = new NpgsqlCommand("select Qid,name,emailaddress,address,mobile_number,c_bpartner_id,city,image from  c_bpartner  where ad_org_id=" + _OrgId + " And c_bpartner_id='" + id + "'  ;", connection);//
                Npgsql.NpgsqlDataAdapter dalist = new NpgsqlDataAdapter(cmd_list_GetData);
                DataTable dtlist = new DataTable();
                dalist.Fill(dtlist);
                connection.Close();

                if (dtlist.Rows.Count > 0)
                {
                    txtQID.Text = dtlist.Rows[0]["Qid"].ToString();
                    txtName.Text = dtlist.Rows[0]["name"].ToString();
                    txtEmail.Text = dtlist.Rows[0]["emailaddress"].ToString();
                    txtAddress.Text = dtlist.Rows[0]["address"].ToString();
                    txtMobile.Text = dtlist.Rows[0]["mobile_number"].ToString();
                    txtCustID.Text = dtlist.Rows[0]["c_bpartner_id"].ToString();
                    int City_SelectedIndex = _m_City_items.FindIndex(i => i.Name == dtlist.Rows[0]["city"].ToString().Trim());
                    City_DropDown.SelectedIndex = City_SelectedIndex;
                    editimage.Source = new BitmapImage(new Uri(dtlist.Rows[0]["image"].ToString(), UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Please Add customer list!");
                    return;
                }

            }

        }
        public void AddCustomersAPICall()
        {
            this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Please Wait .. "; });
            this.Dispatcher.Invoke(() =>
            {
                dynamic CD = City_DropDown.SelectedItem as dynamic;
                int cityId = CD.CityID;


                //dynamic PD = PartnerGroup_DropDown.SelectedItem as dynamic;
                //int PartnerGroupID = PD.id;

                POSCustomersApiModel CustomersDetail = new POSCustomersApiModel
                {
                    macAddress = "8e17517b8cd41f3c",
                    clientId = _clientId,
                    orgId = _OrgId,
                    warehouseId = _Warehouse_Id,
                    userId = _UserID,
                    version = "2.0",
                    appName = "POS",
                    operation = "POSAddEditCustomer",
                    businessPartnerId = int.Parse(txtCustID.Text),
                    customerValue = txtQID.Text,
                    customerName = txtName.Text,
                    lastName = txtName.Text,
                    customerNumber = txtMobile.Text,
                    customerEmail = txtEmail.Text,
                    address1 = txtAddress.Text,
                    city = cityId,
                    country = 0,
                    postal = "0",
                    openingBalance = "0",
                    creditLimit = "100",
                    isVendor = "N",
                    isCredit = "Y",
                    isCustomer = "Y",

                };

                jsonq = JsonConvert.SerializeObject(CustomersDetail);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Passing Data to Api Call"; });
            });

            try
            {
                CustomersApiStringResponce = PostgreSQL.ApiCallPost(jsonq);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
                CheckServerError = 1;
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
            }
            catch
            {
                NpgsqlConnection connection = new NpgsqlConnection(connstring);

                this.Dispatcher.Invoke(() =>
                {
                    connection.Open();

                    //dynamic PG = PartnerGroup_DropDown.SelectedItem as dynamic;
                    //int PartnerGroupID = PG.id;

                    dynamic City = City_DropDown.SelectedItem as dynamic;
                    string CityID = City.Name;


                    ComboBoxItem typeItem = (ComboBoxItem)PartnerGroup_DropDown.SelectedItem;
                    string PartnerGroup = typeItem.Content.ToString();
                    NpgsqlCommand Update_cmd_Cust_Detail = new NpgsqlCommand("Update c_bpartner set qid='" + txtQID.Text + "',name='" + txtName.Text + "',emailaddress='" + txtEmail.Text + "',address='" + txtAddress.Text + "',city='" + CityID + "',updatedby='" + _UserID + "',updated='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "', mobile_number='" + txtMobile.Text + "',is_sync='0',image='"+ imagePath + "' ,c_bpartnergroup_name='" + PartnerGroup + "' where c_bpartner_id='" + txtCustID.Text + "' and ad_org_id=" + _OrgId + ";", connection);//
                    Update_cmd_Cust_Detail.ExecuteNonQuery();
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: ad_client_id  new data  Inserted"; });

                    connection.Close();

                    MessageBox.Show("Record Update Successfully");
                    NavigationService.Navigate(new ViewCustomers());
                });
                //CheckServerError = 0;
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                //log.Error("AddCategory Error|POSOrganization API Not Responding");
                //return;
            }
            if (CheckServerError == 1)
            {
                CustomersApiJOSNResponce = JsonConvert.DeserializeObject(CustomersApiStringResponce);

                if (CustomersApiJOSNResponce.responseCode == "200")
                {
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: loginApiResponce Responce Code 200"; });

                    int _businessPartnerId = CustomersApiJOSNResponce.businessPartnerId;

                    NpgsqlConnection connection = new NpgsqlConnection(connstring);

                    this.Dispatcher.Invoke(() =>
                    {
                        connection.Open();

                        //dynamic PG = PartnerGroup_DropDown.SelectedItem as dynamic;
                        //int PartnerGroupID = PG.id;

                        dynamic City = City_DropDown.SelectedItem as dynamic;
                        string CityID = City.Name;

                        ComboBoxItem typeItem = (ComboBoxItem)PartnerGroup_DropDown.SelectedItem;
                        string PartnerGroup = typeItem.Content.ToString();

                        NpgsqlCommand Update_cmd_Cust_Detail = new NpgsqlCommand("Update c_bpartner set qid='" + txtQID.Text+ "',name='"+txtName.Text+ "',emailaddress='"+txtEmail.Text+ "',address='"+txtAddress.Text+ "',city='"+CityID+ "',updatedby='"+_UserID+ "',updated='"+ DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "',c_bpartnergroup_name='" + PartnerGroup + "', mobile_number='" + txtMobile.Text+ "',is_sync='1',c_bpartner_id='"+_businessPartnerId+ "',id='" + _businessPartnerId + "' ,image='" + imagePath + "'   where c_bpartner_id='" + txtCustID.Text + "' and ad_org_id=" + _OrgId + ";", connection);//
                        Update_cmd_Cust_Detail.ExecuteNonQuery();
                       
                        connection.Close();
                       
                        MessageBox.Show("Record Update Successfully");
                        NavigationService.Navigate(new ViewCustomers());
                    });

                }
                else if (CustomersApiJOSNResponce.responseCode == "106")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        LoginProcessingText.Text = "Customer not created or Updated  !";

                    });
                    return;
                }
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (txtName.Text == String.Empty)
                {
                    txtName.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Name ! ");
                    return;
                }

                if (txtQID.Text == String.Empty)
                {
                    txtQID.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The txtQID ! ");
                    return;
                }

                if (txtAddress.Text == String.Empty)
                {
                    txtAddress.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Address ! ");
                    return;
                }
                if (txtEmail.Text == String.Empty)
                {
                    txtEmail.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Email ! ");
                    return;
                }
              
                if (txtMobile.Text == String.Empty)
                {
                    txtMobile.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Mobile No. ! ");
                    return;
                }
           
                await Task.Run(() => {
                    AddCustomersAPICall();
                });

            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In  POS  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error Record Not Save", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }
        }

        private void Btn_Add_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCustomers());
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().ToUpper() == "F4")
            {
                BtnSave_Click(sender, e);
            }
            if (e.Key.ToString().ToUpper() == "ESCAPE")
            {
                BtnCancel_Click(sender, e);
            }
        }
     

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new ViewCustomers());
        }
        #endregion
        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            
            if (page_no > 0)
                page_no -= 1;
            CustomersLst(page_no);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
           
            page_no += 1;
            CustomersLst(page_no);
        }
        private void Btnupload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFileDialog.FileName);
                string fileName = openFileDialog.SafeFileName;

                string destinationPath = "C:/pos/component/Resources/Images/CustomersImg";
                string directoryPath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

                DirectoryInfo diSource = new DirectoryInfo(directoryPath);
                DirectoryInfo diTarget = new DirectoryInfo(@destinationPath);
                Copy(diSource, diTarget, fileName.ToString());

                editimage.Source = new BitmapImage(fileUri);
                imagePath = diTarget.FullName + "/" + fileName;
            }
        }
        public static void Copy(DirectoryInfo source, DirectoryInfo target, string fileName)
        {

            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            try
            {
                foreach (FileInfo fi in source.GetFiles())
                {
                    if (fi.Name == fileName)
                        fi.CopyTo(System.IO.Path.Combine(target.ToString(), fi.Name), false);
                }

            }
            catch
            {

            }


        }

    }
}
