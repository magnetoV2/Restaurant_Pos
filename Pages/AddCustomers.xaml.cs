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


namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for AddCustomers.xaml
    /// </summary>
    public partial class AddCustomers : Page
    {
        #region 
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        int _Bpartner_Id = Int32.Parse(Application.Current.Properties["BpartnerId"].ToString());
        int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());

        public string connstring = PostgreSQL.ConnectionString;

 
        public List<M_City> m_City_items = new List<M_City>();


        public List<M_Country> m_Country_items = new List<M_Country>();
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
        string imagePath = "";
        private dynamic CustomersApiStringResponce,
        customersApiJOSNResponce;
        public dynamic CustomersApiJOSNResponce { get => customersApiJOSNResponce; set => customersApiJOSNResponce = value; }
        private string jsonq;
        private int CheckServerError = 0;

        #endregion
        public AddCustomers()
        {
            InitializeComponent();
          //  BindBpartner();
            BindCountry();
        }

        #region Bind Data
        public void BindCountry()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_Country_GetData = new NpgsqlCommand("select Country_ID,Country_Name from Country; ", connection);//
                NpgsqlDataReader _Country_GetData = cmd_Country_GetData.ExecuteReader();

                while (_Country_GetData.Read())
                {
                    m_Country_items.Add(new M_Country()
                    {
                        CountryID = _Country_GetData.GetInt32(0),
                        Name = _Country_GetData.GetString(1)

                    });
                }
                connection.Close();


                this.Dispatcher.Invoke(() =>
                {

                    Country_DropDown.ItemsSource = m_Country_items;
                });
                if (m_Country_items.Count == 0)
                {
                    MessageBox.Show("Please Add Country!");

                    return;

                }
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Login POS  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Roles Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
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
                NpgsqlCommand cmd_City_GetData = new NpgsqlCommand("select City_ID,City_Name from City where Country_ID='" + CID + "'; ", connection);//
                NpgsqlDataReader _City_GetData = cmd_City_GetData.ExecuteReader();

                while (_City_GetData.Read())
                {
                    m_City_items.Add(new M_City()
                    {
                        CityID = _City_GetData.GetInt32(0),
                        Name = _City_GetData.GetString(1)

                    });
                }
                connection.Close();


                this.Dispatcher.Invoke(() =>
                {

                    City_DropDown.ItemsSource = m_City_items;
                });
                if (m_City_items.Count == 0)
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
                    Environment.Exit(0);
                }
                Environment.Exit(0);
            }




        }

        //public void BindBpartner()
        //{

        //    try
        //    {

        //        NpgsqlConnection connection = new NpgsqlConnection(connstring);
        //        // connection.Close();
        //        connection.Open();
        //        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
        //        NpgsqlCommand cmd_Bpartner_GetData = new NpgsqlCommand("select c_bpartner_id,Name from c_bpartner  WHERE ad_org_id =" + _OrgId + " AND ad_client_id =" + _clientId + " ;", connection);//
        //        NpgsqlDataReader _Bpartner_GetData = cmd_Bpartner_GetData.ExecuteReader();

        //        while (_Bpartner_GetData.Read())
        //        {
        //            m_Bpartner_items.Add(new M_Bpartner()
        //            {
        //                id = _Bpartner_GetData.GetInt32(0),
        //                Name = _Bpartner_GetData.GetString(1)
                     

        //            });
        //        }
        //        connection.Close();
        //        if (m_Bpartner_items.Count == 0)
        //        {
        //            MessageBox.Show("Please Add Bpartner!");

        //            return;

        //        }
        //        else
        //        {
        //            this.Dispatcher.Invoke(() =>
        //            {

        //                PartnerGroup_DropDown.ItemsSource = m_Bpartner_items;
        //            });
        //        }



        //    }
        //    catch (Exception ex)
        //    {

        //        log.Error(" ===================  Error In Login POS  =========================== ");
        //        log.Error(DateTime.Now.ToString());
        //        log.Error(ex.ToString());
        //        log.Error(" ===================  End of Error  =========================== ");
        //        if (MessageBox.Show(ex.ToString(),
        //                "Error In Bpartner Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
        //        {
        //            return;
        //        }
        //        return;
        //    }




        //}

        #endregion

        #region validation
        private void TxtQID_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtQID.Text = Regex.Replace(txtQID.Text, "[^0-9]+", "");

        }
        private void TxtZipCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtZipCode.Text = Regex.Replace(txtZipCode.Text, "[^0-9]+", "");

        }
        #endregion
        private void Country_DropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic CD = Country_DropDown.SelectedItem as dynamic;
            int CountryID = CD.CountryID;
            BindCity(CountryID);
        }


        public void AddCustomersAPICall()
        {
            
            
            this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Please Wait .. "; });
            this.Dispatcher.Invoke(() =>
            {
                dynamic CD = City_DropDown.SelectedItem as dynamic;
                int cityId = CD.CityID;

                dynamic CUD = Country_DropDown.SelectedItem as dynamic;
                int country = CUD.CountryID;

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
                    businessPartnerId =-1,
                    customerValue = txtQID.Text,
                    customerName = txtname.Text,
                    lastName = txtname.Text,
                    customerNumber = txtMobile.Text,
                    customerEmail =txtEmail.Text,
                    address1 = txtAddress.Text,
                    city = cityId,
                    country = country,
                    postal = txtZipCode.Text,
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
              //  CustomersApiStringResponce = "";
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

                        //  dynamic PG = PartnerGroup_DropDown.SelectedItem as dynamic;
                        //  int PartnerGroupID = PG.id;

                        dynamic CD = Country_DropDown.SelectedItem as dynamic;
                        string CountryID = CD.Name;

                        dynamic City = City_DropDown.SelectedItem as dynamic;
                        string CityID = City.Name;
                        ComboBoxItem typeItem = (ComboBoxItem)PartnerGroup_DropDown.SelectedItem;
                        string PartnerGroup = typeItem.Content.ToString();
                        NpgsqlCommand cmd_select_sequenc_no_ad_user_pos = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'm_customer';", connection);
                        NpgsqlDataReader _get__Ad_sequenc_no_ad_user_pos = cmd_select_sequenc_no_ad_user_pos.ExecuteReader();
                        if (_get__Ad_sequenc_no_ad_user_pos.Read())
                        {
                            Sequenc_id = _get__Ad_sequenc_no_ad_user_pos.GetInt32(4) + 1 *-1;
                        }
                        connection.Close();

                        connection.Open();

                        NpgsqlCommand INSERT_cmd_Cust_Detail = new NpgsqlCommand("INSERT INTO c_bpartner(id,qid,name,emailaddress,address,city,country,zipcode,createdby,ad_client_id,ad_org_id, c_bpartner_id, mobile_number,image,is_sync,c_bpartnergroup_name,iscustomer,isvendor) " + " " + "VALUES('"+Sequenc_id+"','" + txtQID.Text + "' ,'" + txtname.Text + "' ,'" + txtEmail.Text + "','" + txtAddress.Text + "','" + CityID + "','" + CountryID + "','" + txtZipCode.Text + "','" + _UserID + "','" + _clientId + "','" + _OrgId + "','"+ Sequenc_id + "','" + txtMobile.Text + "','" + imagePath + "','0','"+ PartnerGroup + "','Y','N') ;", connection);
                        INSERT_cmd_Cust_Detail.ExecuteNonQuery();
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: ad_client_id  new data  Inserted"; });

                        connection.Close();
                        connection.Open();
                        NpgsqlCommand cmd_update_sequenc_no_ad_user_pos = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'm_customer';", connection);
                        cmd_update_sequenc_no_ad_user_pos.ExecuteReader();
                        connection.Close();
                        MessageBox.Show("Record Save Successfully");
                        NavigationService.Navigate(new AddCustomers());
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
                      //  connection.Open();

                      //  dynamic PG = PartnerGroup_DropDown.SelectedItem as dynamic;
                      //  int PartnerGroupID = PG.id;

                        dynamic CD = Country_DropDown.SelectedItem as dynamic;
                        string CountryID = CD.Name;

                        dynamic City = City_DropDown.SelectedItem as dynamic;
                        string CityID = City.Name;
                        ComboBoxItem typeItem = (ComboBoxItem)PartnerGroup_DropDown.SelectedItem;
                        string PartnerGroup = typeItem.Content.ToString();
                        
                        connection.Open();
                        NpgsqlCommand INSERT_cmd_Cust_Detail = new NpgsqlCommand("INSERT INTO c_bpartner(id,qid,name,emailaddress,address,city,country,zipcode,createdby,ad_client_id,ad_org_id, c_bpartner_id, mobile_number,image,is_sync,c_bpartnergroup_name,iscustomer,isvendor) " + " " + "VALUES('"+ _businessPartnerId + "','" + txtQID.Text + "' ,'" + txtname.Text + "' ,'" + txtEmail.Text + "','" + txtAddress.Text + "','" + CityID + "','" + CountryID + "','" + txtZipCode.Text + "','" + _UserID + "','" + _clientId + "','" + _OrgId + "','"+ _businessPartnerId + "','" + txtMobile.Text + "','" + imagePath + "','0','" + PartnerGroup + "','Y','N') ;", connection);
                        INSERT_cmd_Cust_Detail.ExecuteNonQuery();
                       
                        connection.Close();
                       
                        MessageBox.Show("Record Save Successfully");
                        NavigationService.Navigate(new AddCustomers());
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
                btnSave.IsEnabled = false;
                if (txtname.Text == String.Empty)
                {
                    txtname.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
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
                if (txtZipCode.Text == String.Empty)
                {
                    txtZipCode.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Zip Code ! ");
                    return;
                }
                if (txtMobile.Text == String.Empty)
                {
                    txtMobile.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Mobile No. ! ");
                    return;
                }
                if (imagePath == String.Empty)
                {
                    btnImgUpload.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Upload The Image ! ");
                    return;
                }
                await Task.Run(()=>{
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
        
        private void BtnImgUpload_Click(object sender, RoutedEventArgs e)
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

                product_image.Source = new BitmapImage(fileUri);
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

        private void AddCust_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewCustomers());
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCustomers());
        }
    }
}
