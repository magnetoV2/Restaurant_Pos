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
    /// Interaction logic for UserDetails.xaml
    /// </summary>
    public partial class AddUser : Page
    {
        #region 
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        int _pricelistId = Int32.Parse(Application.Current.Properties["pricelistId"].ToString());
        int _costElementId = Int32.Parse(Application.Current.Properties["costElementId"].ToString());
      
        public string connstring = PostgreSQL.ConnectionString;

        public List<M_warehouse> m_warehouse_items = new List<M_warehouse>();
        public List<M_Roles> m_Roles_items = new List<M_Roles>();
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
        //private dynamic UserApiStringResponce,
        // userApiJOSNResponce;
        //public dynamic UserApiJOSNResponce { get => userApiJOSNResponce; set => userApiJOSNResponce = value; }
        //private string jsonq;
        //private int CheckServerError = 0;
        string imagePath = "";
        #endregion
        #region Public Function
        public AddUser()
        {
            InitializeComponent();
            BindWareHouse();
            BindRole();
            BindCountry();
        }
        public void ClearData()
        {
            txtQID.Text = "";
            txtname.Text = "";
            txtPassword.Text = "";
            txtUserName.Text = "";
            txtAddress.Text = "";
            txtEmail.Text = "";
            txtMobile.Text = "";
            txtZipCode.Text = "";
            imagePath = "";
        }
        #endregion

        #region validation
        private void TxtQID_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtQID.Text = Regex.Replace(txtQID.Text, "[^0-9]+", "");
           
        }

        private void TxtMobile_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtMobile.Text = Regex.Replace(txtMobile.Text, "[^0-9]+", "");
          
        }

        private void TxtZipCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtZipCode.Text = Regex.Replace(txtZipCode.Text, "[^0-9]+", "");
           
        }
    private void TxtEmail_TextChanged(object sender, TextChangedEventArgs e)
    {
            txtEmail.Text = Regex.Replace(txtEmail.Text, "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])? ", "");
         
        }
        #endregion

        #region Bind Data Function


        public void BindWareHouse()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_warehouseDetails_GetData = new NpgsqlCommand("SELECT  name,m_warehouse_id FROM m_warehouse  WHERE ad_org_id =" + _OrgId + " AND ad_client_id =" + _clientId + " And m_warehouse_id !=1 ;", connection);//
                NpgsqlDataReader _warehouseDetails_GetData = cmd_warehouseDetails_GetData.ExecuteReader();

                while (_warehouseDetails_GetData.Read())
                {
                    m_warehouse_items.Add(new M_warehouse()
                    {
                        WarehouseName = _warehouseDetails_GetData.GetString(0),
                        WarehouseId = _warehouseDetails_GetData.GetInt32(1)

                    });
                }
                connection.Close();
                if (m_warehouse_items.Count == 0)
                {
                    MessageBox.Show("Please Add Warehouse!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        Warehouse_DropDown.ItemsSource = m_warehouse_items;
                    });
                }

             
               
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Login POS  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Warehouse Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }

        public void BindRole()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_Roles_GetData = new NpgsqlCommand("SELECT Name,ad_role_id FROM ad_role  WHERE  ad_client_id = " + _clientId + "; ", connection);//
                NpgsqlDataReader _Roles_GetData = cmd_Roles_GetData.ExecuteReader();

                while (_Roles_GetData.Read())
                {
                    m_Roles_items.Add(new M_Roles()
                    {
                        Name = _Roles_GetData.GetString(0),
                        RoleID = _Roles_GetData.GetInt32(1)
                    });
                }
                connection.Close();
                if (m_Roles_items.Count == 0)
                {
                    MessageBox.Show("Please Add Roles!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        Role_DropDown.ItemsSource = m_Roles_items;
                    });
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
                m_City_items.Clear();
                City_DropDown.ItemsSource = "";
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
                    return;
                }
                return;
            }




        }

        #endregion
        #region call api POSAddUser
        //public void UserAPICall()
        //{

        //    this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Please Wait .. "; });
        //    this.Dispatcher.Invoke(() =>
        //    {

        //        NpgsqlConnection connection = new NpgsqlConnection(connstring);
        //        dynamic WH = Warehouse_DropDown.SelectedItem as dynamic;
        //        int WarehouseId = WH.WarehouseId;

        //        dynamic RD = Role_DropDown.SelectedItem as dynamic;
        //        int Roleid = RD.RoleID;
        //        string RoleName = RD.Name;

        //        dynamic CD = Country_DropDown.SelectedItem as dynamic;
        //        int CountryID = CD.CountryID;

        //        dynamic City = City_DropDown.SelectedItem as dynamic;
        //        int CityID = City.CityID;

        //        roleDetail _roleDetail = new roleDetail
        //        {
        //            roleId= Roleid,
        //            roleName= RoleName
        //        };
        //            POSAddUser _AddUser = new POSAddUser
        //        {
        //            macAddress = "8e17517b8cd41f3c",
        //            clientId = _clientId,
        //            orgId = _OrgId,
        //            warehouseId = WarehouseId,
        //            userId = _UserID,
        //            version = "2.0",
        //            appName = "POS",
        //            operation = "POSAddEditUser",
        //            ad_user_id = 0,
        //            userValue = txtQID.Text,
        //            name = txtname.Text,
        //            username = txtUserName.Text,
        //            password = txtPassword.Text,
        //            userNumber = txtMobile.Text,
        //            userEmail = txtEmail.Text,
        //            address1 = txtAddress.Text,
        //            city = CityID,
        //            country = CountryID,
        //            postal = txtZipCode.Text,
                   
        //            };
        //        _AddUser.roleDetails.Add(_roleDetail);
             

        //        jsonq = JsonConvert.SerializeObject(_AddUser);
        //        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Passing Data to Api Call"; });
        //    });

        //    try
        //    {
        //        UserApiStringResponce = PostgreSQL.ApiCallPost(jsonq);
        //        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
        //        CheckServerError = 1;
        //        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
        //    }
        //    catch
        //    {
        //        CheckServerError = 0;
        //        this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
        //        log.Error("Product Error|POSOrganization API Not Responding");
        //        return;
        //    }
        //    if (CheckServerError == 1)
        //    {
        //        UserApiJOSNResponce = JsonConvert.DeserializeObject(UserApiStringResponce);

        //        if (UserApiJOSNResponce.responseCode == "200")
        //        {
        //            //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: loginApiResponce Responce Code 200"; });

        //            int _ad_user_id = UserApiJOSNResponce.ad_user_id;
                  
        //            NpgsqlConnection connection = new NpgsqlConnection(connstring);
        //            this.Dispatcher.Invoke(() =>
        //            {
                      
        //                connection.Open();

        //                dynamic WH = Warehouse_DropDown.SelectedItem as dynamic;
        //                int WarehouseId = WH.WarehouseId;

        //                dynamic RD = Role_DropDown.SelectedItem as dynamic;
        //                int Roleid = RD.RoleID;

        //                dynamic CD = Country_DropDown.SelectedItem as dynamic;
        //                int CountryID = CD.CountryID;

        //                dynamic City = City_DropDown.SelectedItem as dynamic;
        //                int CityID = City.CityID;

                       

        //                connection.Open();
        //                NpgsqlCommand INSERT_cmd_User_Detail = new NpgsqlCommand("INSERT INTO ad_user_pos (ad_user_pos_id,ad_client_id,ad_org_id,QID,name,username,password,mobile,email,address,m_warehouse_id,ad_role_id,createdBy,Updatedby,islogged, isactive,ZipCode,City,CountryID,ad_user_id,imgpath) " + " " + "VALUES(" + _ad_user_id + "," + _clientId + " ," + _OrgId + " ," + txtQID.Text + ",'" + txtname.Text + "','" + txtUserName.Text + "','" + txtPassword.Text + "','" + txtMobile.Text + "','" + txtEmail.Text + "','" + txtAddress.Text + "','" + WarehouseId + "','" + Roleid + "'," + _UserID + "," + _UserID + ",'Y','Y','" + txtZipCode.Text + "','" + CountryID + "','" + CityID + "','" + _ad_user_id + "','" + imagePath + "') ;", connection);
        //                INSERT_cmd_User_Detail.ExecuteNonQuery();
        //                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: ad_client_id  new data  Inserted"; });
        //                ClearData();
        //                connection.Close();
                      
        //                MessageBox.Show("Record Save Successfully");
        //            });

        //        }
        //        else if (UserApiJOSNResponce.responseCode == "106")
        //        {
        //            this.Dispatcher.Invoke(() =>
        //            {
        //                LoginProcessingText.Text = "Product not created   !";

        //            });
        //            return;
        //        }
        //    }
        //}


        #endregion
        #region Event

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
           
            ClearData();
        }
        public  void SaveUserDetail()
        {
            try
            {
                btnSave.IsEnabled = true;

                if (txtname.Text == String.Empty)
                {
                    txtname.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Name ! ");
                    return;
                }
                if (txtUserName.Text == String.Empty)
                {
                    txtname.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The User Name ! ");
                    return;
                }
                if (txtQID.Text == String.Empty)
                {
                    txtQID.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The txtQID ! ");
                    return;
                }
                if (txtPassword.Text == String.Empty)
                {
                    txtPassword.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Password ! ");
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
                if (!Regex.IsMatch(txtEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
                {
                    MessageBox.Show("Please Enter Valid Email ! ");
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
                NpgsqlConnection connection = new NpgsqlConnection(connstring);


            

                dynamic WH = Warehouse_DropDown.SelectedItem as dynamic;
                int WarehouseId = WH.WarehouseId;

                dynamic RD = Role_DropDown.SelectedItem as dynamic;
                int Roleid = RD.RoleID;

                dynamic CD = Country_DropDown.SelectedItem as dynamic;
                int CountryID = CD.CountryID;

                dynamic City = City_DropDown.SelectedItem as dynamic;
                int CityID = City.CityID;

                connection.Open();
                NpgsqlCommand cmd_select_sequenc_no_ad_user_pos = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'ad_user_pos';", connection);
                NpgsqlDataReader _get__Ad_sequenc_no_ad_user_pos = cmd_select_sequenc_no_ad_user_pos.ExecuteReader();

                if (_get__Ad_sequenc_no_ad_user_pos.Read())
                {
                    Sequenc_id = _get__Ad_sequenc_no_ad_user_pos.GetInt32(4) + 1;
                }
                connection.Close();

                connection.Open();
                NpgsqlCommand INSERT_cmd_User_Detail = new NpgsqlCommand("INSERT INTO ad_user_pos (ad_user_pos_id,ad_client_id,ad_org_id,QID,name,username,password,mobile,email,address,m_warehouse_id,ad_role_id,createdBy,islogged, isactive,ZipCode,City,CountryID,ad_user_id,imgpath) " + " " + "VALUES(" + Sequenc_id + "," + _clientId + " ," + _OrgId + " ," + txtQID.Text + ",'" + txtname.Text + "','" + txtUserName.Text + "','" + txtPassword.Text + "','" + txtMobile.Text + "','" + txtEmail.Text + "','" + txtAddress.Text + "','" + WarehouseId + "','" + Roleid + "'," + _UserID + ",'Y','Y','" + txtZipCode.Text + "','" + CountryID + "','" + CityID + "','" + Sequenc_id + "','" + imagePath + "') ;", connection);
                INSERT_cmd_User_Detail.ExecuteNonQuery();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: ad_client_id  new data  Inserted"; });
                ClearData();
                connection.Close();
                connection.Open();
                NpgsqlCommand cmd_update_sequenc_no_ad_user_pos = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'ad_user_pos';", connection);
                cmd_update_sequenc_no_ad_user_pos.ExecuteReader();
                connection.Close();

                MessageBox.Show("Record Save Successfully");

            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Insert User Detail  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Insert User Detail", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }

        }
       
        private void Country_DropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic CD = Country_DropDown.SelectedItem as dynamic;
            int CountryID = CD.CountryID;
            BindCity(CountryID);
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveUserDetail();
        }
        private void BtnImgUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFileDialog.FileName);
                string fileName = openFileDialog.SafeFileName;

                string destinationPath = "C:/pos/component/Resources/Images/UsersImg";
                string directoryPath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

                DirectoryInfo diSource = new DirectoryInfo(directoryPath);
                DirectoryInfo diTarget = new DirectoryInfo(@destinationPath);
                Copy(diSource, diTarget, fileName.ToString());

                product_image.Source = new BitmapImage(fileUri);
                imagePath = diTarget.FullName + "/" + fileName;
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







        #endregion

        private void Adduser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewUser());
        }

        private void Grid_KeyUp_1(object sender, KeyEventArgs e)
        {

        }
    }
}

