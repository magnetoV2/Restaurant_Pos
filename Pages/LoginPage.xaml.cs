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
using Restaurant_Pos.Pages.UserControls;

namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Public Function
        //int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        //int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        //int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());

        //int _sessionID = Int32.Parse(Application.Current.Properties["SessionID"].ToString());
        //int _Bpartner_Id = Int32.Parse(Application.Current.Properties["BpartnerId"].ToString());
        //int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());
        private dynamic loginApiStringResponce,
        loginApiJOSNResponce,

        POSCashCustomerApiStringResponce,
        POSCashCustomerApiJSONResponce,
        POSCustomerApiStringResponce,
        POSCustomerApiJSONResponce,
        POSCategoryApiStringResponce,
        POSCategoryApiJSONResponce,
        POSOrderNumberApiStringResponce,
        POSOrderNumberApiJSONResponce,
        POSAllProductsApiStringResponce,
        POSAllProductsApiJSONResponce;

        private string jsonq;
        private int CheckServerError = 0;

        //private readonly string DeviceMacAdd = LoginViewModel.CPU_ID();
        //private string DeviceMacAdd = LoginViewModel._DeviceMacAddress;
        //private string[] DeviceMacAdd_arr = LoginViewModel._DeviceMacAddress_arr;
        //private int DeviceMacAdd_count = LoginViewModel._DeviceMacAddress_count;
        //private string CPU_Mac_ID = LoginViewModel.CPU_ID();
        public string connstring = PostgreSQL.ConnectionString;
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

        public dynamic LoginApiJOSNResponce { get => loginApiJOSNResponce; set => loginApiJOSNResponce = value; }

        public List<M_warehouse> m_warehouse_items = new List<M_warehouse>();
        private string jsonPOSCustomers;
        private string jsonPOSCashCustomer;
        private string jsonPOSCategory;
        private string jsonPOSOrderNumber;
        private string jsonPOSAllProducts;

        private string servername;
        private string serverport;
        //  private string api_url;
        private string server_local_name;
        private string server_local_port;
        // private string server_local_userid;
        //  private string server_local_password;
        //  private string server_local_dbname;
        // private string display_language;
        // private string on_printer;


        #endregion Public Function

        public LoginPage()
        {
            try
            {
                InitializeComponent();

                NavigationCommands.BrowseBack.InputGestures.Clear();
                NavigationCommands.BrowseForward.InputGestures.Clear();
                //  macAddress.Text = LoginViewModel.DeviceMacAddress();
                Warehouse_selection = 0;

                //  ServerConfigration();
                LoadPreviousLoggedUser();
                Keyboard.Focus(Login_Password);
                Login_Password.Focus();
                //  wherehousedata();



            }
            catch (Exception ex)
            {
                log.Error(" ===================  Error In Login POS  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Login POS", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;

            }

        }
        private string _username, _password;

        private void GridPopupCreateNewSession_KeyUp(object sender, KeyEventArgs e)
        {
            int ascii = e.Key.ToString().ToUpper()[0];
            
            if (e.Key.ToString().ToUpper() == "F4")
            {
                BtnStart_Click(sender, e);
            }
            else if(ascii>65 && ascii<90)
            {
              
                txtopeningbalance.Text = "0";
                txtopeningbalance.SelectionStart = 1;
            }
            else if((e.Key.Equals(Key.Back) || e.Key.Equals(Key.Delete)) && txtopeningbalance.Text.Length == 0)
            {
                txtopeningbalance.Text = "0";
                txtopeningbalance.SelectionStart = 1;
            }
            else
            {
                if (txtopeningbalance.Text == string.Empty)
                {
                    MessageBox.Show("Please Enter The Opening Balance !");
                    return;
                }
            }
        }

        private void Txtopeningbalance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Back) && txtopeningbalance.Text.Length==1) 
            {
                txtopeningbalance.Text = "0";
            }
            else
            {
                return;
            }
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (txtopeningbalance.Text != string.Empty)
                {
                    PopupCreateNewSession.IsOpen = false;
                    await Task.Run(() =>
                    {
                        LoginAPICall();
                    });
                }
                else
                {

                    MessageBox.Show("Please Enter The Opening Balance !");
                }

            }
            catch (Exception)
            {
                // MessageBox.Show("Opening balance is Not successfully Save!!");
                return;
            }
        }
        public void OpeningSessionSave()
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                string command= "INSERT INTO ad_user_session(ad_client_id,ad_org_id,isActive,created,createdby,updated,updatedby,session_id,openingbalance)" +
                " VALUES(" + AD_Client_ID + " ," + AD_ORG_ID + " ,'Y','" + 
                DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "'," + AD_USER_ID + ",'" +
                DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "',"+ AD_USER_ID + ",'0','" + 
                txtopeningbalance.Text + "')" ;
                NpgsqlCommand INSERT_cmd_Start = new NpgsqlCommand(command, connection);
                INSERT_cmd_Start.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Opening balance is successfully Save!!");

            }
            catch (Exception)
            {

                MessageBox.Show("Opening balance is Not successfully Save!!");
                return;
            }
        }



        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                this.Dispatcher.Invoke(() =>
                {

                    Login_Page.Opacity = 50;
                    Login_Page.IsEnabled = false;
                });
                _username = Login_Email.Text;
                _password = Login_Password.Password;

                PopupCreateNewSession.IsOpen = true;



            }
            catch (Exception ex)
            {
                log.Error(" ===================  Error In Login POS  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Retail POS", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;

            }
        }

        private void Login_Password_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key != Key.Enter)
            //{
            //    return;
            //}
            //try
            //{
            //    this.Dispatcher.Invoke(() =>
            //    {

            //        Login_Page.Opacity = 50;
            //        Login_Page.IsEnabled = false;
            //    });
            //    _username = Login_Email.Text;
            //    _password = Login_Password.Password;
            //    PopupCreateNewSession.IsOpen = true;
            //}
            //catch (Exception ex)
            //{
            //    log.Error(" ===================  Error In Login POS  =========================== ");
            //    log.Error(DateTime.Now.ToString());
            //    log.Error(ex.ToString());
            //    log.Error(" ===================  End of Error  =========================== ");
            //    if (MessageBox.Show(ex.ToString(),
            //            "Error In Retail POS", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
            //    {
            //        return;
            //    }
            //    return;

            //}
        }

        /// <summary>
        /// Call the function if new user is logged in
        /// Call the function if new user is logged in
        /// </summary>
        public void LoginAPICall()
        {
            var setting = PostgreSQL.Get_App_setting();
            servername = setting.servername;
            serverport = setting.serverport;
            server_local_name = setting.server_local_name;
            server_local_port = setting.server_local_port;
            string StrServerName = Application.Current.Properties["ServerName"].ToString();
            string StrPortNo = Application.Current.Properties["PortNo"].ToString();
            if (_username == String.Empty || _password == String.Empty)
            {
                this.Dispatcher.Invoke(() =>
                {
                    LoginProcessingText.Text = "The Fields Cannot Be Empty!!!";
                    //  LoginProgressBar.Visibility = Visibility.Hidden;
                    Login_Page.Opacity = 100;
                    Login_Page.IsEnabled = true;
                    if (_username == String.Empty)
                    {
                        Login_Email.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    }
                    if (_password == String.Empty)
                    {
                        Login_Password.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    }
                });
                return;
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    Login_Email.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartBlueBrush);
                    Login_Password.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartBlueBrush);
                });
            }
            string _username_post = _username;//Login_Email.Text;
            string _password_post = _password;//Login_Password.Password;
            string _remindMe_post = "N";

            string _macAddress_post = "8e17517b8cd41f3c";
            // string _macAddress_post = LoginViewModel.DeviceMacAddress();//LoginViewModel._DeviceMacAddress;
            string _version_post = "1.6";
            string _appName_post = "POS";
            //22-08-2020
            Application.Current.Properties["macAddress_post"] = _macAddress_post;
            Application.Current.Properties["version_post"] = _version_post;
            Application.Current.Properties["appName_post"] = _appName_post;
            #region LoginApi POSOrganizationPostgreSQL

            if (Warehouse_selection == 0)
            {
                this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Please Wait .. "; });

                POSorganizationApiModel loginDetail = new POSorganizationApiModel
                {
                    remindMe = _remindMe_post,
                    macAddress = _macAddress_post,
                    version = _version_post,
                    appName = _appName_post,
                    operation = "POSOrganization",
                    username = _username_post,
                    password = _password_post,

                };

                jsonq = JsonConvert.SerializeObject(loginDetail);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Passing Data to Api Call"; });
                try
                {
                    loginApiStringResponce = PostgreSQL.ApiCallPost(jsonq);

                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
                    CheckServerError = 1;
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
                }
                catch
                {
                    if (StrServerName == server_local_name && StrPortNo == server_local_port)
                    {
                        NpgsqlConnection connection = new NpgsqlConnection(connstring);
                        connection.Open();
                        this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Fetching Client Details .. "; });
                        NpgsqlCommand cmd_select_user_no = new NpgsqlCommand(" select Name,Password,ad_client_id,ad_org_id,ad_user_id,c_bpartner_id,m_warehouse_id,ad_role_id,sessionid from ad_user_pos;", connection);
                        NpgsqlDataReader _select_user_no = cmd_select_user_no.ExecuteReader();
                        string UserName = "";
                        string Password = "";
                        if (_select_user_no.Read())
                        {
                            UserName = _select_user_no.GetString(0);
                            Password = _select_user_no.GetString(1);
                            string _ad_client_id = _select_user_no.GetString(2);
                            string _ad_org_id = _select_user_no.GetString(3);
                            string _ad_user_id = _select_user_no.GetString(4);
                            string _c_bpartner_id = _select_user_no.GetString(5);
                            string _m_warehouse_id = _select_user_no.GetString(6);
                            string _ad_role_id = _select_user_no.GetString(7);
                            string _sessionid = _select_user_no.GetString(8);

                            Application.Current.Properties["UserID"] = _ad_user_id;
                            Application.Current.Properties["clientId"] = _ad_client_id;
                            Application.Current.Properties["OrgId"] = _ad_org_id;
                            Application.Current.Properties["BpartnerId"] = _c_bpartner_id;
                            Application.Current.Properties["WarehouseId"] = _m_warehouse_id;
                            Application.Current.Properties["SessionID"] = _sessionid;
                            Application.Current.Properties["pricelistId"] = "0";
                            Application.Current.Properties["costElementId"] = "0";



                            AD_Client_ID = int.Parse(_ad_client_id);
                            AD_ORG_ID = int.Parse(_ad_org_id);
                            AD_USER_ID = int.Parse(_ad_user_id);
                        }

                        connection.Close();

                        if (_username_post == UserName && _password_post == Password)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                OpeningSessionSave();
                                Login_Page.Opacity = 100;
                                Login_Page.IsEnabled = true;
                                LoginProcessingText.Text = "";
                            //  LoginProgressBar.Visibility = Visibility.Hidden;

                            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Retail;
                            });
                        }

                    }
                    else
                    {
                        MessageBox.Show("Please Enter Local Server Name And Port No. ");
                        this.Dispatcher.Invoke(() =>
                        {
                            NavigationService.Navigate(new ServerConfig());
                        });
                    }
                    //CheckServerError = 0;
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; LoginProgressBar.Visibility = Visibility.Hidden; });
                    //log.Error("Login Error|POSOrganization API Not Responding");
                    //this.Dispatcher.Invoke(() =>
                    //{
                    //    LoginProgressBar.Visibility = Visibility.Hidden;
                    //    Login_Page.Opacity = 100;
                    //    Login_Page.IsEnabled = true;
                    //});

                    //return;
                }
                if (CheckServerError == 1)
                {

                    if (StrServerName == servername && StrPortNo == serverport)
                    {
                        LoginApiJOSNResponce = JsonConvert.DeserializeObject(loginApiStringResponce);
                        NpgsqlConnection connection = new NpgsqlConnection(connstring);

                        if (LoginApiJOSNResponce.responseCode == "200")
                        {
                            //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: loginApiResponce Responce Code 200"; });

                            connection.Open();

                            int _userId = LoginApiJOSNResponce.userId;                                      //1005329
                            AD_USER_ID = _userId;
                            Application.Current.Properties["UserID"] = _userId.ToString();
                            int _clientId = LoginApiJOSNResponce.clientId;

                            Application.Current.Properties["clientId"] = _clientId.ToString();
                            //1000049,
                            AD_Client_ID = _clientId;
                            int _businessPartnerId = LoginApiJOSNResponce.businessPartnerId;                // 1011081,
                            string isRetail = LoginApiJOSNResponce.businessPartnerId;                       // "N"
                            JArray _orgDetails = LoginApiJOSNResponce.orgDetails;
                            JArray _warehouseDetails = LoginApiJOSNResponce.warehouseDetails;
                            JArray _roleDetails = LoginApiJOSNResponce.roleDetails;
                            JArray _roleAccessDetails = LoginApiJOSNResponce.roleAccessDetails;

                            //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking ad_client_id table"; });

                            NpgsqlCommand cmd_clientId_Read = new NpgsqlCommand("SELECT  ad_client_id FROM ad_client WHERE ad_client_id = " + _clientId + " ;", connection);
                            NpgsqlDataReader _clientId_Read = cmd_clientId_Read.ExecuteReader();
                            if (_clientId_Read.Read() && _clientId_Read.HasRows == true)
                            {
                                int check = _clientId_Read.FieldCount;
                                connection.Close();
                                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: ad_client_id  table Present"; });
                            }
                            else
                            {
                                connection.Close();

                                connection.Open();
                                this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Fetching Client Details .. "; });
                                NpgsqlCommand cmd_select_sequenc_no = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'ad_client';", connection);
                                NpgsqlDataReader _get__Ad_sequenc_no = cmd_select_sequenc_no.ExecuteReader();

                                if (_get__Ad_sequenc_no.Read())
                                {
                                    Sequenc_id = _get__Ad_sequenc_no.GetInt32(4) + 1;
                                }
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand INSERT_cmd_ad_client = new NpgsqlCommand("INSERT INTO ad_client(id, ad_client_id, createdby, updatedby) VALUES(" + Sequenc_id + "," + _clientId + " ," + _userId + " ," + _userId + ") ;", connection);
                                INSERT_cmd_ad_client.ExecuteNonQuery();
                                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: ad_client_id  new data  Inserted"; });
                                connection.Close();




                                connection.Open();
                                NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'ad_client';", connection);
                                NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                                connection.Close();
                            }
                            foreach (dynamic _m_orgDetails in _orgDetails)
                            {
                                int orgId = _m_orgDetails.orgId;
                                Application.Current.Properties["OrgId"] = orgId.ToString();
                                AD_ORG_ID = orgId;

                                string orgName = _m_orgDetails.orgName ?? null;                         //"Gulf Shell",
                                string isdefault = _m_orgDetails.isdefault;                             //"Y",
                                string showImage = _m_orgDetails.showImage;                             //"Y",
                                string orgImage = _m_orgDetails.orgImage;                               //Base64
                                string orgPhone = _m_orgDetails.orgPhone;                               //"+97450777822",
                                string orgEmail = _m_orgDetails.orgEmail;                               //"gulfshell@gmail.com",
                                string orgAddress = _m_orgDetails.orgAddress;                           //"AL KHARAITIYAT",
                                string orgCity = _m_orgDetails.orgCity;                                 //"DOHA",
                                string orgCountry = _m_orgDetails.orgCountry;                           //"QATAR",
                                string orgReceiptfootermsg = _m_orgDetails.orgReceiptfootermsg;         //"Thank You! Visit Again!!",
                                string orgbgImage = _m_orgDetails.orgbgImage;                           //base64
                                string arabicName = _m_orgDetails.orgArabic;
                                string postal = "";
                                string arabicfootermessage = "";
                                string termsmessage = "";
                                string arabictermsmessage = "";
                                string weburl = "";
                                connection.Open();
                                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking ad_org table"; });
                                NpgsqlCommand cmd_orgDetails = new NpgsqlCommand("SELECT  id FROM ad_org WHERE ad_org_id = " + orgId + " AND ad_client_id =" + _clientId + " ;", connection);
                                NpgsqlDataReader _orgDetails_Read = cmd_orgDetails.ExecuteReader();
                                if (_orgDetails_Read.Read() && _orgDetails_Read.HasRows == true)
                                {
                                    int check = _orgDetails_Read.FieldCount;
                                    connection.Close();
                                    connection.Open();

                                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: ad_org  table Present"; });
                                    NpgsqlCommand UPDATE_cmd_orgDetails = new NpgsqlCommand("UPDATE  ad_org " +
                                    "SET id = " +
                                    orgId + ", ad_client_id = " +
                                    _clientId + ", ad_org_id = " +
                                    orgId + ", createdby = " +
                                    _userId + ", updatedby =" +
                                    _userId + ",name = '" +
                                    orgName + "', arabicname = '" +
                                    arabicName + "',logo = '" +
                                    orgImage + "',phone= '" +
                                    orgPhone + "', email = '" +
                                    orgEmail + "', address = '" +
                                    orgAddress + "', city = '" +
                                    orgCity + "', country = '" +
                                    orgCountry + "', postal = '" +
                                    postal + "', weburl = '" +
                                    weburl + "',  footermessage = '" +
                                    orgReceiptfootermsg + "', arabicfootermessage = '" +
                                    arabicfootermessage + "', termsmessage = '" +
                                    termsmessage + "', arabictermsmessage = '" +
                                    arabictermsmessage + "', displayimage = '" +
                                    showImage + "' ,updated = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' WHERE ad_client_id = " + _clientId + " AND ad_org_id = " + orgId + " ;", connection);
                                    UPDATE_cmd_orgDetails.ExecuteNonQuery();
                                    connection.Close();
                                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Updated ad_org table"; });
                                }
                                else
                                {
                                    connection.Close();

                                    connection.Open();

                                    NpgsqlCommand cmd_select_sequenc_no = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'ad_org';", connection);
                                    NpgsqlDataReader _get__Ad_sequenc_no = cmd_select_sequenc_no.ExecuteReader();

                                    if (_get__Ad_sequenc_no.Read())
                                    {
                                        Sequenc_id = _get__Ad_sequenc_no.GetInt32(4) + 1;
                                    }
                                    connection.Close();
                                    this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Fetching Organization Details .. "; });
                                    connection.Open();
                                    NpgsqlCommand INSERT_cmd_orgDetails = new NpgsqlCommand("INSERT INTO ad_org(" +
                                    "id, ad_client_id, ad_org_id,  createdby," +
                                    "updatedby, " + "name" + ", arabicname, logo, phone, email, address, city," +
                                    "country, postal, weburl, footermessage, arabicfootermessage," +
                                    "termsmessage, arabictermsmessage, displayimage)" +
                                    "VALUES(" +
                                    Sequenc_id + ", " +
                                    _clientId + ", " +
                                    orgId + ", " +
                                    _userId + "," +
                                    _userId + ", '" +
                                    orgName + "', '" +
                                    arabicName + "', '" +
                                    orgImage + "', '" +
                                    orgPhone + "','" +
                                    orgEmail + "', '" +
                                    orgAddress + "', '" +
                                    orgCity + "','" +
                                    orgCountry + "','" +
                                    postal + "', '" +
                                    weburl + "','" +
                                    orgReceiptfootermsg + "','" +
                                    arabicfootermessage + "','" +
                                    termsmessage + "','" +
                                    arabictermsmessage + "','" +
                                    showImage + "');", connection);
                                    INSERT_cmd_orgDetails.ExecuteNonQuery();
                                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Inserted ad_org table"; });
                                    connection.Close();

                                    connection.Open();
                                    NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'ad_org';", connection);
                                    NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                                    connection.Close();
                                }
                            }
                            foreach (dynamic _m_warehouseDetails in _warehouseDetails)
                            {
                                Application.Current.Properties["WarehouseId"] = _m_warehouseDetails.warehouseId.ToString();
                                int warehouseId = _m_warehouseDetails.warehouseId;                      //": 1000103,
                                string warehouseName = _m_warehouseDetails.warehouseName;               //": "Al Kharaitiyat",
                                string isdefault = _m_warehouseDetails.isdefault;                       //": "Y",
                                int warehouePriceListId = _m_warehouseDetails.warehouePriceListId;      //": 1000135,
                                string city = _m_warehouseDetails.city;                                 //": "Doha",
                                string orgId = _m_warehouseDetails.orgId;                               //": "1000070"
                                string phone = "";

                                connection.Open();
                                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking m_warehouse table"; });
                                NpgsqlCommand cmd_warehouseDetails_Read = new NpgsqlCommand("SELECT  * FROM m_warehouse WHERE ad_org_id = " + orgId + " AND ad_client_id =" + _clientId + " AND m_warehouse_id = " + warehouseId + ";", connection);//
                                NpgsqlDataReader _warehouseDetails_Read = cmd_warehouseDetails_Read.ExecuteReader();

                                if (_warehouseDetails_Read.Read() && _warehouseDetails_Read.HasRows == true)
                                {
                                    int check = _warehouseDetails_Read.FieldCount;
                                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: m_warehouse table Present"; });
                                    connection.Close();
                                }
                                else
                                {
                                    connection.Close();

                                    connection.Open();

                                    NpgsqlCommand cmd_select_sequenc_no = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'm_warehouse';", connection);
                                    NpgsqlDataReader _get__Ad_sequenc_no = cmd_select_sequenc_no.ExecuteReader();

                                    if (_get__Ad_sequenc_no.Read())
                                    {
                                        Sequenc_id = _get__Ad_sequenc_no.GetInt32(4) + 1;
                                    }
                                    connection.Close();
                                    this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Fetching WareHouse Details .. "; });
                                    connection.Open();
                                    NpgsqlCommand INSERT_cmd_warehouseDetails = new NpgsqlCommand("INSERT INTO m_warehouse(id, ad_client_id, ad_org_id, m_warehouse_id,createdby, updatedby, name, phone, city, warehouepricelistid) VALUES("
                                                                                            + Sequenc_id + "," + _clientId + " ," + orgId + " ," + warehouseId + "," + _userId + "," + _userId + ",'" + warehouseName + "','" + phone + "','" + city + "'," + warehouePriceListId + ") ;", connection);
                                    INSERT_cmd_warehouseDetails.ExecuteNonQuery();
                                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Inserted new m_warehouse row"; });
                                    connection.Close();

                                    connection.Open();
                                    NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'm_warehouse';", connection);
                                    NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                                    connection.Close();
                                }
                            }
                            foreach (dynamic _m_roleDetails in _roleDetails)
                            {
                                int roleId = _m_roleDetails.roleId;                                     // 1000328,
                                string roleName = _m_roleDetails.roleName;                              // "Cashier",
                                string isdefault = _m_roleDetails.isdefault;                            // "Y"
                                connection.Open();
                                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking ad_role table"; });
                                NpgsqlCommand cmd_roleDetails = new NpgsqlCommand("SELECT  id FROM ad_role WHERE ad_client_id =" + _clientId + " AND ad_role_id = " + roleId + ";", connection);
                                NpgsqlDataReader _roleDetails_Read = cmd_roleDetails.ExecuteReader();
                                if (_roleDetails_Read.Read() && _roleDetails_Read.HasRows == true)
                                {
                                    int check = _roleDetails_Read.FieldCount;
                                    connection.Close();
                                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: ad_role table Present"; });
                                }
                                else
                                {
                                    connection.Close();

                                    connection.Open();

                                    NpgsqlCommand cmd_select_sequenc_no = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'ad_role';", connection);
                                    NpgsqlDataReader _get__Ad_sequenc_no = cmd_select_sequenc_no.ExecuteReader();

                                    if (_get__Ad_sequenc_no.Read())
                                    {
                                        Sequenc_id = _get__Ad_sequenc_no.GetInt32(4) + 1;
                                    }
                                    connection.Close();

                                    connection.Open();
                                    NpgsqlCommand INSERT_roleDetails = new NpgsqlCommand("INSERT INTO ad_role(id, ad_client_id, ad_role_id,createdby, updatedby, " + "name" + ", isdefault) VALUES("
                                                                                           + Sequenc_id + "," + _clientId + " ," + roleId + "," + _userId + "," + _userId + ",'" + roleName + "','" + isdefault + "') ;", connection);
                                    INSERT_roleDetails.ExecuteNonQuery();
                                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Inserted new ad_role table row"; });
                                    connection.Close();

                                    connection.Open();
                                    NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'ad_role';", connection);
                                    NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                                    connection.Close();
                                }
                            }
                            foreach (dynamic _m_roleAccessDetails in _roleAccessDetails)
                            {
                                string roleId = _m_roleAccessDetails.roleId;                              // "1000328",
                                int orgId = _m_roleAccessDetails.orgId;                                   // 1000070
                                connection.Open();
                                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking ad_role_orgaccess table"; });
                                NpgsqlCommand cmd_roleDetails = new NpgsqlCommand("SELECT  id FROM ad_role_orgaccess WHERE  ad_client_id =" + _clientId + "  AND ad_org_id =" + orgId + " AND ad_role_id = " + roleId + ";", connection);
                                NpgsqlDataReader _roleAccessDetails_Read = cmd_roleDetails.ExecuteReader();
                                if (_roleAccessDetails_Read.Read() && _roleAccessDetails_Read.HasRows == true)
                                {
                                    int check = _roleAccessDetails_Read.FieldCount;
                                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process:  ad_role_orgaccess table Present"; });
                                    connection.Close();
                                }
                                else
                                {
                                    connection.Close();

                                    connection.Open();

                                    NpgsqlCommand cmd_select_sequenc_no = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'ad_role_orgaccess';", connection);
                                    NpgsqlDataReader _get__Ad_sequenc_no = cmd_select_sequenc_no.ExecuteReader();

                                    if (_get__Ad_sequenc_no.Read())
                                    {
                                        Sequenc_id = _get__Ad_sequenc_no.GetInt32(4) + 1;
                                    }
                                    connection.Close();

                                    connection.Open();
                                    NpgsqlCommand INSERT_roleDetails = new NpgsqlCommand("INSERT INTO ad_role_orgaccess(id, ad_client_id,ad_org_id, ad_role_id,createdby, updatedby) VALUES("
                                                                                           + Sequenc_id + "," + _clientId + " ," + orgId + "," + roleId + "," + _userId + "," + _userId + ") ;", connection);
                                    INSERT_roleDetails.ExecuteNonQuery();
                                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Inserted new ad_role_orgaccess table row"; });

                                    connection.Close();

                                    connection.Open();

                                    NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'ad_role_orgaccess';", connection);
                                    NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                                    connection.Close();
                                }
                            }
                        }
                        else if (LoginApiJOSNResponce.responseCode == "102")
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                LoginProcessingText.Text = "Login Failed! Please Try Again";

                                Login_Page.Opacity = 100;
                                Login_Page.IsEnabled = true;
                            });
                            return;
                        }
                        else if (LoginApiJOSNResponce.responseCode == "301")
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                LoginProcessingText.Text = "Login Failed! This Device Is Not Registered";

                                Login_Page.Opacity = 100;
                                Login_Page.IsEnabled = true;
                            });
                            return;
                        }
                        else if (LoginApiJOSNResponce.responseCode == "101")
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                LoginProcessingText.Text = "Login Failed! User Not Registered";

                                Login_Page.Opacity = 100;
                                Login_Page.IsEnabled = true;
                            });
                            return;
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                            log.Error("Login Error: POSOrganization");
                            log.Error("Server Responce:" + LoginApiJOSNResponce);

                            this.Dispatcher.Invoke(() =>
                            {

                                Login_Page.Opacity = 100;
                                Login_Page.IsEnabled = true;
                            });
                            return;
                        }

                        #region Get All WarehouseDetails and return

                        connection.Close();
                        connection.Open();
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                        NpgsqlCommand cmd_warehouseDetails_GetData = new NpgsqlCommand("SELECT  id,ad_client_id,ad_org_id,m_warehouse_id,isactive,isdefault,Name ,warehouepricelistid,city,phone FROM m_warehouse WHERE ad_org_id = " + AD_ORG_ID + " AND ad_client_id =" + AD_Client_ID + ";", connection);//
                        NpgsqlDataReader _warehouseDetails_GetData = cmd_warehouseDetails_GetData.ExecuteReader();

                        while (_warehouseDetails_GetData.Read())
                        {
                            m_warehouse_items.Add(new M_warehouse()
                            {
                                WarehouseId = _warehouseDetails_GetData.GetInt32(0),
                                Ad_client_id = _warehouseDetails_GetData.GetInt32(1),
                                Ad_org_id = _warehouseDetails_GetData.GetInt32(2),
                                M_warehouse_id = _warehouseDetails_GetData.GetInt32(3),
                                Isactive = _warehouseDetails_GetData.GetString(4),
                                Isdefault = _warehouseDetails_GetData.GetString(5),
                                WarehouseName = _warehouseDetails_GetData.GetString(6),
                                WarehouePriceListId = _warehouseDetails_GetData.GetInt32(7),
                                City = _warehouseDetails_GetData.GetString(8),
                                Phone = _warehouseDetails_GetData.GetString(9)
                            });
                        }
                        connection.Close();
                        Warehouse_selection = 1;
                        //testing
                        double check_warehouse_id = LoadPreviousLoggedUserWarehouse();
                        if (check_warehouse_id != 0)
                        {
                            var m_warehouse_index = m_warehouse_items.IndexOf(m_warehouse_items.Where(x => x.M_warehouse_id == check_warehouse_id).FirstOrDefault());
                            if (m_warehouse_index >= 0)
                            {
                                var m_warehouse_index_val = m_warehouse_items[m_warehouse_index];
                                m_warehouse_items.RemoveAt(m_warehouse_index);
                                m_warehouse_items.Insert(0, m_warehouse_index_val);
                            }
                        }
                        m_warehouse_items.ToList().ForEach(x =>
                        {
                            Console.WriteLine(x.M_warehouse_id + "-" + x.WarehouseName);
                        });
                        //testing END

                        this.Dispatcher.Invoke(() =>
                        {

                            Warehouse_DropDown.ItemsSource = m_warehouse_items;
                            Warehouse_DropDown_br.Visibility = Visibility.Visible;
                            Warehouse_DropDown.Visibility = Visibility.Visible;

                            LoginProcessingText.Text = "";
                            Login_Page.Opacity = 100;
                            Login_Page.IsEnabled = true;
                            btnLogin.IsDefault = true;


                        });
                        if (m_warehouse_items.Count == 0)
                        {
                            LoginProcessingText.Text = "Please Add Warehouse!";
                            return;

                        }

                        #endregion Get All WarehouseDetails and return
                        this.Dispatcher.Invoke(() =>
                        {
                            OpeningSessionSave();
                        });
                    }
                    else
                    {
                        MessageBox.Show("Please Enter The Server Name And Port No.");
                        this.Dispatcher.Invoke(() =>
                        {


                            NavigationService.Navigate(new ServerConfig());
                        });

                    }
                }
            }

            #endregion LoginApi POSOrganization

            //Checking if Warehouse is selected
            if (WarehouseId_Selected != 0 && Warehouse_selection == 1)
            {
                this.Dispatcher.Invoke(() =>
                {

                    Login_Page.Opacity = 50;
                    Login_Page.IsEnabled = false;
                });
                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                //GET ROLE FOR USER
                connection.Open();
                NpgsqlCommand cmd_GetRole_ID = new NpgsqlCommand("SELECT " +
                    "ad_role_orgaccess.ad_client_id," +
                    "ad_role_orgaccess.ad_org_id," +
                    "ad_role_orgaccess.ad_role_id," +
                    "ad_role.isdefault," +
                    "ad_role.name " +
                    "FROM ad_role_orgaccess " +
                    "INNER JOIN ad_role ON ad_role_orgaccess.ad_role_id = ad_role.ad_role_id " +
                    "WHERE ad_role_orgaccess.ad_client_id = " + AD_Client_ID + " AND ad_role_orgaccess.ad_org_id = " + AD_ORG_ID + " AND ad_role.isdefault = 'Y' LIMIT 1;", connection);
                NpgsqlDataReader _GetRole_ID = cmd_GetRole_ID.ExecuteReader();
                if (_GetRole_ID.Read() && _GetRole_ID.HasRows == true)
                {
                    AD_ROLE_ID = _GetRole_ID.GetInt32(2);
                    connection.Close();

                    #region cash coustomer/sys config

                    POSCashCustomer SYS_CONFIG = new POSCashCustomer
                    {
                        operation = "POSCashCustomer",
                        username = _username_post,
                        password = _password_post,
                        remindMe = _remindMe_post,
                        macAddress = _macAddress_post,
                        version = _version_post,
                        appName = _appName_post,
                        userId = AD_USER_ID.ToString(),
                        clientId = AD_Client_ID.ToString(),
                        orgId = AD_ORG_ID.ToString(),
                        roleId = AD_ROLE_ID.ToString()
                    };
                    jsonPOSCashCustomer = JsonConvert.SerializeObject(SYS_CONFIG);
                    this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Please Wait ..  "; });
                    try
                    {
                        POSCashCustomerApiStringResponce = PostgreSQL.ApiCallPost(jsonPOSCashCustomer);
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
                        CheckServerError = 1;
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
                    }
                    catch
                    {
                        CheckServerError = 0;
                        this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                        log.Error("Login Error|POSCashCustomer API Not Responding");
                        this.Dispatcher.Invoke(() =>
                        {

                            Login_Page.Opacity = 100;
                            Login_Page.IsEnabled = true;
                        });

                        return;
                    }
                    if (CheckServerError == 1)
                    {
                        POSCashCustomerApiJSONResponce = JsonConvert.DeserializeObject(POSCashCustomerApiStringResponce);

                        if (POSCashCustomerApiJSONResponce.responseCode == "200")
                        {

                            //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: POSCashCustomerApiResponce Responce Code 200"; });
                            string _cashBookId = POSCashCustomerApiJSONResponce.cashBookId;
                            string _periodId = POSCashCustomerApiJSONResponce.periodId;
                            string _paymentTermId = POSCashCustomerApiJSONResponce.paymentTermId;
                            string _adTableId = POSCashCustomerApiJSONResponce.adTableId;
                            string _accountSchemaId = POSCashCustomerApiJSONResponce.accountSchemaId;
                            string _discPercent = POSCashCustomerApiJSONResponce.discPercent;
                            string _currencyId = POSCashCustomerApiJSONResponce.currencyId;
                            string _isDiscount = POSCashCustomerApiJSONResponce.isDiscount;
                            string _costElementId = POSCashCustomerApiJSONResponce.costElementId;
                            Sys_config_costElementId = _costElementId;
                            Application.Current.Properties["costElementId"] = Sys_config_costElementId;

                            string _currencyCode = POSCashCustomerApiJSONResponce.currencyCode;
                            string _paymentRule = POSCashCustomerApiJSONResponce.paymentRule;
                            string _printSalesSummary = POSCashCustomerApiJSONResponce.printSalesSummary;
                            string _printPreBill = POSCashCustomerApiJSONResponce.printPreBill;
                            string _showComplement = POSCashCustomerApiJSONResponce.showComplement;
                            string _customerName = POSCashCustomerApiJSONResponce.customerName;
                            string _pricelistId = POSCashCustomerApiJSONResponce.pricelistId;
                            if (string.IsNullOrEmpty(_pricelistId))
                                _pricelistId = "0";
                            Sys_config_pricelistId = _pricelistId;
                            Application.Current.Properties["pricelistId"] = Sys_config_pricelistId;

                            string _businessPartnerId = POSCashCustomerApiJSONResponce.businessPartnerId;
                            Sys_config_businessPartnerId = _businessPartnerId;
                            Application.Current.Properties["BpartnerId"] = _businessPartnerId;
                            string _printCount = POSCashCustomerApiJSONResponce.printCount; //"printCount": "1",
                            string _custprefix = POSCashCustomerApiJSONResponce.prefix; //"prefix": "TEL"
                            JArray _msrCodeDetails = POSCashCustomerApiJSONResponce.msrCodeDetails;
                            JArray _msruomDetails = POSCashCustomerApiJSONResponce.uomDetails;

                            //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking ad_client_id table"; });

                            connection.Open();
                            NpgsqlCommand cmd_ad_sys_config_check = new NpgsqlCommand("SELECT  id FROM ad_sys_config WHERE ad_client_id = " + AD_Client_ID + " AND ad_org_id = " + AD_ORG_ID + " AND ad_user_id=" + AD_USER_ID + ";", connection);
                            NpgsqlDataReader _ad_sys_config_Read = cmd_ad_sys_config_check.ExecuteReader();

                            if (_ad_sys_config_Read.Read() && _ad_sys_config_Read.HasRows == true)
                            {
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand cmd_Up_ad_sys_config = new NpgsqlCommand("UPDATE ad_sys_config SET isdiscount = '" + _isDiscount + "', discpercent = " + _discPercent + ", costelementid = '" + _costElementId + "', currencyid = " + _currencyId + ", currencycode = '" + _currencyCode + "', cashbookid = " + _cashBookId + ", periodid = " + _periodId + ", paymenttermid = " + _paymentTermId + ", adtableid = " + _adTableId + ", accountschemaid = " + _accountSchemaId + ", paymentrule = '" + _paymentRule + "', printsalessummary = '" + _printSalesSummary + "', printprebill = '" + _printPreBill + "', showcomplement = '" + _showComplement + "' ,updated = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") +
                                "',c_bpartner_id = " + _businessPartnerId + ",pricelistid = " + _pricelistId + "  WHERE ad_client_id = " + AD_Client_ID + " AND ad_org_id = " + AD_ORG_ID + " AND ad_user_id=" + AD_USER_ID + ";", connection);
                                NpgsqlDataReader _Up_ad_sys_config_Read = cmd_Up_ad_sys_config.ExecuteReader();
                                connection.Close();
                            }
                            else
                            {
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand cmd_select_sequenc_no = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'ad_sys_config';", connection);
                                NpgsqlDataReader _get__Ad_sequenc_no = cmd_select_sequenc_no.ExecuteReader();

                                if (_get__Ad_sequenc_no.Read())
                                {
                                    Sequenc_id = _get__Ad_sequenc_no.GetInt32(4) + 1;
                                }
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand INSERT_cmd_ad_sys_config = new NpgsqlCommand("INSERT INTO ad_sys_config( " +
                                    "id, ad_client_id, ad_org_id, createdby, updatedby, isdiscount, discpercent, " +
                                    "costelementid, currencyid, currencycode, cashbookid,periodid, paymenttermid, " +
                                    "adtableid, accountschemaid, paymentrule, printsalessummary, printprebill, showcomplement," +
                                    "ad_user_id,ad_role_id,pricelistid,c_bpartner_id)" +
                                    " VALUES(" +
                                    Sequenc_id + "," + AD_Client_ID + "," + AD_ORG_ID + "," + AD_USER_ID + "," + AD_USER_ID + ",'" + _isDiscount + "'," + _discPercent + ",'" + _costElementId + "'," + _currencyId + ",'" + _currencyCode + "'," + _cashBookId + "," + _periodId + "," + _paymentTermId + "," + _adTableId + "," + _accountSchemaId + ",'" + _paymentRule + "','" + _printSalesSummary + "','" + _printPreBill + "','" + _showComplement + "'," + AD_USER_ID + "," + AD_ROLE_ID + "," + _pricelistId + "," + _businessPartnerId + ");", connection);
                                INSERT_cmd_ad_sys_config.ExecuteNonQuery();
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'ad_sys_config';", connection);
                                NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                                connection.Close();
                            }
                            // inserting Cash Customer for sys connfig----------------------------------------------------------------------------------------------------------------------------------

                            #region inserting Cash Customer for sys

                            connection.Open();
                            NpgsqlCommand cmd_c_bpartner_check = new NpgsqlCommand("SELECT  id FROM c_bpartner WHERE ad_client_id = " + AD_Client_ID + "AND ad_org_id = " + AD_ORG_ID + "AND c_bpartner_id =" + _businessPartnerId + " ;", connection);
                            NpgsqlDataReader __c_bpartner_Read = cmd_c_bpartner_check.ExecuteReader();

                            if (__c_bpartner_Read.Read() && __c_bpartner_Read.HasRows == true)
                            {
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand cmd_Up_ad_sys_config = new NpgsqlCommand("UPDATE c_bpartner SET name = '" + _customerName + "',updated = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") +
                                "',iscustomer='Y',isvendor='N' WHERE ad_client_id = " + AD_Client_ID + "AND ad_org_id = " + AD_ORG_ID + "AND c_bpartner_id =" + _businessPartnerId + " ;", connection);
                                NpgsqlDataReader _Up_ad_sys_config_Read = cmd_Up_ad_sys_config.ExecuteReader();
                                connection.Close();
                            }
                            else
                            {
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand cmd_select_sequenc_no = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'c_bpartner';", connection);
                                NpgsqlDataReader _get__Ad_sequenc_no = cmd_select_sequenc_no.ExecuteReader();

                                if (_get__Ad_sequenc_no.Read())
                                {
                                    Sequenc_id = _get__Ad_sequenc_no.GetInt32(4) + 1;
                                }
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand INSERT_cmd_c_bpartner = new NpgsqlCommand("INSERT INTO c_bpartner(" +
                                    "id, ad_client_id, ad_org_id, c_bpartner_id," +
                                    "createdby, updatedby, name, searchkey, pricelistid," +
                                    "iscredit, creditamount, iscashcustomer,iscustomer,isvendor)" +
                                    "VALUES(" + Sequenc_id + "," + AD_Client_ID + "," + AD_ORG_ID + "," + _businessPartnerId + "," +
                                    AD_USER_ID + "," + AD_USER_ID + ",'" + _customerName + "'," + _businessPartnerId + "," + _pricelistId + ",'N',0,'Y','Y','N');", connection);
                                INSERT_cmd_c_bpartner.ExecuteNonQuery();
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'c_bpartner';", connection);
                                NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                                connection.Close();
                            }

                            #endregion inserting Cash Customer for sys

                            //END--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                            if (_msrCodeDetails != null)
                            {
                                foreach (dynamic _m_msrCodeDetails in _msrCodeDetails)
                                {
                                    string msrName = _m_msrCodeDetails.msrName;          //"msrName": "MSR Code",
                                    string msrCode = _m_msrCodeDetails.msrCode;         //"msrCode": "102030",
                                    int userId = _m_msrCodeDetails.userId;              //"userId": 1005327

                                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking ee_msrmanager table"; });

                                    connection.Open();
                                    NpgsqlCommand cmd_msrCodeDetails = new NpgsqlCommand("SELECT id FROM ee_msrmanager WHERE msr_Code = '" + msrCode + "' AND ad_client_id =" + AD_Client_ID + " AND ad_org_id = " + AD_ORG_ID + ";", connection);
                                    NpgsqlDataReader _orgDetails_Read = cmd_msrCodeDetails.ExecuteReader();
                                    if (_orgDetails_Read.Read() && _orgDetails_Read.HasRows == true)
                                    {
                                        connection.Close();
                                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: ee_msrmanager Present"; });
                                    }
                                    else
                                    {
                                        connection.Close();

                                        connection.Open();
                                        NpgsqlCommand cmd_select_sequenc_no = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'ee_msrmanager';", connection);
                                        NpgsqlDataReader _get__Ad_sequenc_no = cmd_select_sequenc_no.ExecuteReader();

                                        if (_get__Ad_sequenc_no.Read())
                                        {
                                            Sequenc_id = _get__Ad_sequenc_no.GetInt32(4) + 1;
                                        }
                                        connection.Close();

                                        connection.Open();
                                        NpgsqlCommand INSERT_cmd_ad_sys_config = new NpgsqlCommand("INSERT INTO ee_msrmanager(" +
                                                                    "id, ad_client_id, ad_org_id, ad_user_id, createdby, updatedby,name, msr_code)" +
                                                                    "VALUES(" + Sequenc_id + "," + AD_Client_ID + "," + AD_ORG_ID + "," + userId + "," + AD_USER_ID + "," + AD_USER_ID + ",'" + msrName + "'," + msrCode + "); ", connection);
                                        INSERT_cmd_ad_sys_config.ExecuteNonQuery();
                                        connection.Close();

                                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Inserted ad_org table"; });

                                        connection.Open();
                                        NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'ee_msrmanager';", connection);
                                        NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                                        connection.Close();
                                    }
                                }
                            }
                            if (_msruomDetails != null)
                            {
                                NpgsqlConnection connection1 = new NpgsqlConnection(connstring);
                                connection1.Open();
                                NpgsqlCommand productuom_mstr = new NpgsqlCommand("SELECT c_uom_id FROM m_product_units where ad_client_id = '" + AD_Client_ID + "' and ad_org_id='" + AD_ORG_ID + "';", connection1);
                                NpgsqlDataReader _get__productuom_mstr = productuom_mstr.ExecuteReader();

                                if (_get__productuom_mstr.Read() && _get__productuom_mstr.HasRows == true)
                                {
                                    connection1.Close();
                                }
                                else
                                {
                                    connection1.Close();
                                    foreach (dynamic _m_msruomDetails in _msruomDetails)
                                    {
                                        string uomId = _m_msruomDetails.uomId;
                                        string uomName = _m_msruomDetails.uomName;

                                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking ee_msrmanager table"; });

                                        connection.Open();
                                        NpgsqlCommand cmd_msruomDetails = new NpgsqlCommand("insert into m_product_units(ad_client_id,ad_org_id,c_uom_id,name) values (" + AD_Client_ID + "," + AD_ORG_ID + "," + uomId + ",'" + uomName + "');", connection);
                                        cmd_msruomDetails.ExecuteNonQuery();
                                        connection.Close();
                                    }
                                }
                            }

                        }
                        else
                        {
                            this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                            log.Error("Login Error: POSCashCustomer");
                            log.Error("Server Responce:" + POSCashCustomerApiJSONResponce);
                            this.Dispatcher.Invoke(() =>
                            {

                                Login_Page.Opacity = 100;
                                Login_Page.IsEnabled = true;
                            });
                            return;
                        }
                    }

                    #endregion cash coustomer/sys config
                }
                else
                {
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "No Role assess for the User"; LoginProgressBar.Visibility = Visibility.Hidden; });
                    this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Error! User Dosen't Have A Defaulf Role. Contact Admin"; });
                    log.Error("Login Error");
                    log.Error("User Dosen't Have A Defaulf Role Or Table Is Empty");
                    connection.Close();
                    this.Dispatcher.Invoke(() =>
                    {

                        Login_Page.Opacity = 100;
                        Login_Page.IsEnabled = true;
                    });
                    return;
                }

                #region Getting ALL businessPartner for the orgination

                POSCustomers BpabusinessPartner = new POSCustomers
                {
                    operation = "POSCustomers",
                    remindMe = _remindMe_post,
                    macAddress = _macAddress_post,
                    version = _version_post,
                    appName = _appName_post,
                    clientId = AD_Client_ID.ToString(),
                    orgId = AD_ORG_ID.ToString(),
                };

                jsonPOSCustomers = JsonConvert.SerializeObject(BpabusinessPartner);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Passing Data to Api Call"; });
                try
                {
                    POSCustomerApiStringResponce = PostgreSQL.ApiCallPost(jsonPOSCustomers);
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
                    CheckServerError = 1;
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
                }
                catch
                {
                    CheckServerError = 0;
                    this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                    log.Error("Login Error|POSCustomers API Not Responding");
                    this.Dispatcher.Invoke(() =>
                    {

                        Login_Page.Opacity = 100;
                        Login_Page.IsEnabled = true;
                    });
                    return;
                }
                if (CheckServerError == 1)
                {
                    POSCustomerApiJSONResponce = JsonConvert.DeserializeObject(POSCustomerApiStringResponce);

                    if (POSCustomerApiJSONResponce.responseCode == "200")
                    {
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: POSCustomerApiResponce Responce Code 200"; });
                        JArray _customers = POSCustomerApiJSONResponce.customers;

                        // inserting businessPartner ----------------------------------------------------------------------------------------------------------------------------------

                        #region Business Partner

                        foreach (dynamic m_customers in _customers)
                        {
                            string _businessPartnerId = m_customers.businessPartnerId;
                            string _customerName = m_customers.customerName;
                            string _customerValue = m_customers.customerValue;
                            string _pricelistId = m_customers.pricelistId;
                            if (string.IsNullOrEmpty(_pricelistId))
                                _pricelistId = "0";
                            string _isCredit = m_customers.isCredit;
                            string _creditLimit = m_customers.creditLimit;
                            string _isCustomer = m_customers.isCustomer;
                            string _isVendor = m_customers.isVendor;
                            connection.Open();
                            NpgsqlCommand cmd_c_bpartner_check = new NpgsqlCommand("SELECT  id FROM c_bpartner WHERE ad_client_id = " + AD_Client_ID + " AND ad_org_id = " + AD_ORG_ID +
                                " AND c_bpartner_id =" + _businessPartnerId + ";", connection);
                            NpgsqlDataReader __c_bpartner_Read = cmd_c_bpartner_check.ExecuteReader();

                            if (__c_bpartner_Read.Read() && __c_bpartner_Read.HasRows == true)
                            {
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand cmd_Up_ad_sys_config = new NpgsqlCommand("UPDATE c_bpartner SET " +
                                    "name = '" + _customerName + "',pricelistid =" + _pricelistId + ",iscredit ='" + _isCredit + "',creditamount =" + _creditLimit + ",updated = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") +
                                "',iscustomer='" + _isCustomer + "',isvendor='" + _isVendor + "' WHERE ad_client_id = " + AD_Client_ID + "AND ad_org_id = " + AD_ORG_ID +
                                "AND c_bpartner_id =" + _businessPartnerId + "AND searchkey = '" + _customerValue + "' ; ", connection);
                                NpgsqlDataReader _Up_ad_sys_config_Read = cmd_Up_ad_sys_config.ExecuteReader();
                                connection.Close();
                            }
                            else
                            {
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand cmd_select_sequenc_no = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'c_bpartner';", connection);
                                NpgsqlDataReader _get__Ad_sequenc_no = cmd_select_sequenc_no.ExecuteReader();

                                if (_get__Ad_sequenc_no.Read())
                                {
                                    Sequenc_id = _get__Ad_sequenc_no.GetInt32(4) + 1;
                                }
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand INSERT_cmd_c_bpartner = new NpgsqlCommand("INSERT INTO c_bpartner(" +
                                    "id, ad_client_id, ad_org_id, c_bpartner_id," +
                                    "createdby, updatedby, name, searchkey, pricelistid," +
                                    "iscredit, creditamount,iscustomer,isvendor)" +
                                    "VALUES(" + Sequenc_id + "," + AD_Client_ID + "," + AD_ORG_ID + "," + _businessPartnerId + "," +
                                    AD_USER_ID + "," + AD_USER_ID + ",'" + _customerName + "','" + _customerValue + "'," + _pricelistId + ",'" + _isCredit + "'," + _creditLimit + ",'" + _isCustomer + "','" + _isVendor + "');", connection);
                                INSERT_cmd_c_bpartner.ExecuteNonQuery();
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'c_bpartner';", connection);
                                NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                                connection.Close();
                            }
                        }

                        #endregion Business Partner

                        //END--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                        log.Error("Login Error: POSCustomers");
                        log.Error("Server Responce:" + POSCustomerApiJSONResponce);
                        this.Dispatcher.Invoke(() =>
                        {

                            Login_Page.Opacity = 100;
                            Login_Page.IsEnabled = true;
                        });

                        return;
                    }
                }

                #endregion Getting ALL businessPartner for the orgination

                #region Getting ALL Product CATEGORY for the orgination and client

                POSCategory Product_Category = new POSCategory
                {
                    operation = "POSCategory",
                    remindMe = _remindMe_post,
                    macAddress = _macAddress_post,
                    version = _version_post,
                    appName = _appName_post,
                    clientId = AD_Client_ID.ToString(),
                    orgId = AD_ORG_ID.ToString(),
                };

                jsonPOSCategory = JsonConvert.SerializeObject(Product_Category);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Passing Data to Api Call"; });
                try
                {
                    POSCategoryApiStringResponce = PostgreSQL.ApiCallPost(jsonPOSCategory);
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
                    CheckServerError = 1;
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
                }
                catch
                {
                    CheckServerError = 0;
                    this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                    log.Error("Login Error|POSCategory API Not Responding");
                    this.Dispatcher.Invoke(() =>
                    {

                        Login_Page.Opacity = 100;
                        Login_Page.IsEnabled = true;
                    });
                    return;
                }
                if (CheckServerError == 1)
                {
                    POSCategoryApiJSONResponce = JsonConvert.DeserializeObject(POSCategoryApiStringResponce);

                    if (POSCategoryApiJSONResponce.responseCode == "200")
                    {
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: POSCategoryApiResponce Responce Code 200"; });
                        JArray _category = POSCategoryApiJSONResponce.category;

                        // inserting CATEGORY ----------------------------------------------------------------------------------------------------------------------------------

                        #region CATEGORY

                        foreach (dynamic m_category in _category)
                        {
                            string _categoryId = m_category.categoryId;
                            string _categoryName = m_category.categoryName;
                            string _categoryValue = m_category.categoryValue;
                            string _categoryNameArabic = m_category.categoryNameArabic;
                            string _categoryImage = m_category.categoryImage;

                            connection.Open();
                            NpgsqlCommand cmd_m_product_category_check = new NpgsqlCommand("SELECT  id FROM m_product_category WHERE ad_client_id = " + AD_Client_ID + "AND ad_org_id = " + AD_ORG_ID +
                                "AND m_product_category_id =" + _categoryId + " ; ", connection);
                            NpgsqlDataReader _m_product_category_Read = cmd_m_product_category_check.ExecuteReader();

                            if (_m_product_category_Read.Read() && _m_product_category_Read.HasRows == true)
                            {
                                connection.Close();

                                //connection.Open();
                                //NpgsqlCommand cmd_Up_ad_sys_config = new NpgsqlCommand("UPDATE m_product_category SET " +
                                //    "name = '" + _customerName + "',pricelistid =" + _pricelistId + ",iscredit ='" + _isCredit + "',creditamount =" + _creditLimit + ",updated = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") +
                                //"WHERE c_bpartner_id =" + __c_bpartner_Read.GetInt32(0) + ";", connection);
                                //NpgsqlDataReader _Up_ad_sys_config_Read = cmd_Up_ad_sys_config.ExecuteReader();
                                //connection.Close();
                            }
                            else
                            {
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand cmd_select_sequenc_no = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'm_product_category';", connection);
                                NpgsqlDataReader _get__Ad_sequenc_no = cmd_select_sequenc_no.ExecuteReader();

                                if (_get__Ad_sequenc_no.Read())
                                {
                                    Sequenc_id = _get__Ad_sequenc_no.GetInt32(4) + 1;
                                }
                                connection.Close();
                                this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Fetching Product Details .. "; });
                                connection.Open();
                                string INSERT_cmd_m_product_category_string = "INSERT INTO m_product_category(" +
                                    "id, ad_client_id, ad_org_id, m_product_category_id," +
                                    "createdby, updatedby, name, searchkey, arabicname," +
                                    "image)" +
                                    "VALUES(" + Sequenc_id + "," + AD_Client_ID + "," + AD_ORG_ID + "," + _categoryId + "," +
                                    AD_USER_ID + "," + AD_USER_ID + ",@_categoryName,@_categoryValue,@_categoryNameArabic,@_categoryImage);";

                                NpgsqlCommand INSERT_cmd_m_product_category = new NpgsqlCommand(INSERT_cmd_m_product_category_string, connection);
                                INSERT_cmd_m_product_category.Parameters.AddWithValue("@_categoryName", _categoryName);
                                INSERT_cmd_m_product_category.Parameters.AddWithValue("@_categoryValue", _categoryValue);
                                INSERT_cmd_m_product_category.Parameters.AddWithValue("@_categoryNameArabic", _categoryNameArabic ?? " ");
                                INSERT_cmd_m_product_category.Parameters.AddWithValue("@_categoryImage", _categoryImage ?? " ");
                                INSERT_cmd_m_product_category.ExecuteNonQuery();
                                connection.Close();

                                connection.Open();
                                NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'm_product_category';", connection);
                                NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                                connection.Close();
                            }
                        }

                        #endregion CATEGORY

                        //END--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                        log.Error("Login Error: POSCategory");
                        log.Error("Server Responce:" + POSCategoryApiJSONResponce);
                        this.Dispatcher.Invoke(() =>
                        {

                            Login_Page.Opacity = 100;
                            Login_Page.IsEnabled = true;
                        });
                        return;
                    }
                }

                #endregion Getting ALL Product CATEGORY for the orgination and client

                #region POSOrderNumber
                string _prefix = "";
                string _startNo = "";
                string _endNo = "";
                //Change mani startNo
                string _docNo = "";
                string _incrementNo = "";
                string _currentNext = "";
                int start_no = 0;
                int end_no = 0;
                POSOrderNumber POSOrderNumber = new POSOrderNumber
                {
                    //Change mani
                    // operation = "POSOrderNumber",
                    operation = "POSDeviceOrderNumber",
                    username = _username_post,
                    password = _password_post,
                    clientId = AD_Client_ID.ToString(),
                    orgId = AD_ORG_ID.ToString(),
                    userId = AD_USER_ID.ToString(),
                    businessPartnerId = Sys_config_businessPartnerId.ToString(),
                    roleId = AD_ROLE_ID.ToString(),
                    warehouseId = WarehouseId_Selected.ToString(),
                    remindMe = _remindMe_post,
                    macAddress = _macAddress_post,
                    version = _version_post,
                    appName = _appName_post,
                };

                jsonPOSOrderNumber = JsonConvert.SerializeObject(POSOrderNumber);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Passing Data to Api Call"; });
                try
                {
                    POSOrderNumberApiStringResponce = PostgreSQL.ApiCallPost(jsonPOSOrderNumber);
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
                    CheckServerError = 1;
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
                }
                catch
                {
                    CheckServerError = 0;
                    this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                    log.Error("Login Error|POSOrderNumber API Not Responding");
                    this.Dispatcher.Invoke(() =>
                    {

                        Login_Page.Opacity = 100;
                        Login_Page.IsEnabled = true;
                    });
                    return;
                }
                if (CheckServerError == 1)
                {
                    POSOrderNumberApiJSONResponce = JsonConvert.DeserializeObject(POSOrderNumberApiStringResponce);

                    if (POSOrderNumberApiJSONResponce.responseCode == "200")
                    {
                        _prefix = POSOrderNumberApiJSONResponce.prefix;
                        _startNo = POSOrderNumberApiJSONResponce.startNo;
                        _endNo = POSOrderNumberApiJSONResponce.endNo;
                        //Change mani startNo
                        _docNo = POSOrderNumberApiJSONResponce.startNo;
                        _incrementNo = POSOrderNumberApiJSONResponce.incrementNo;
                        _currentNext = POSOrderNumberApiJSONResponce.currentNext;

                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking ad_client_id table"; });

                    }
                    else
                    {
                        this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                        log.Error("Login Error: POSOrderNumber");
                        log.Error("Server Responce:" + POSOrderNumberApiJSONResponce);
                        this.Dispatcher.Invoke(() =>
                        {

                            Login_Page.Opacity = 100;
                            Login_Page.IsEnabled = true;
                        });
                        return;
                    }
                }
                connection.Open();
                NpgsqlCommand cmd_c_invoice_number_details_check = new NpgsqlCommand("SELECT  * FROM c_invoice_number_details WHERE ad_client_id = " + AD_Client_ID + " AND ad_org_id = " + AD_ORG_ID + ";", connection);
                NpgsqlDataReader _c_invoice_number_details_Read = cmd_c_invoice_number_details_check.ExecuteReader();

                if (_c_invoice_number_details_Read.Read() && _c_invoice_number_details_Read.HasRows == true)
                {
                    connection.Close();

                    if (start_no == end_no || start_no < Convert.ToInt64(_startNo))
                    {


                        //connection.Open();
                        //NpgsqlCommand UPDATE_cmd_c_invoice_number_details = new NpgsqlCommand("UPDATE c_invoice_number_details SET start_no = " + _startNo + ", end_no = " + _endNo + ", updated = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' WHERE ad_client_id = " + AD_Client_ID + " AND ad_org_id = " + AD_ORG_ID + ";", connection);
                        //UPDATE_cmd_c_invoice_number_details.ExecuteNonQuery();
                        //connection.Close(); 
                    }

                }
                else
                {
                    connection.Close();
                    connection.Open();
                    NpgsqlCommand cmd_select_sequenc_no = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'c_invoice_number_details';", connection);
                    NpgsqlDataReader _get__Ad_sequenc_no = cmd_select_sequenc_no.ExecuteReader();

                    if (_get__Ad_sequenc_no.Read())
                    {
                        Sequenc_id = _get__Ad_sequenc_no.GetInt32(4) + 1;
                    }
                    connection.Close();

                    connection.Open();
                    NpgsqlCommand INSERT_cmd_c_invoice_number_details = new NpgsqlCommand("INSERT INTO c_invoice_number_details(c_invoice_number_details_id, ad_client_id, ad_org_id, createdby, updatedby, ad_user_id,name, start_no,end_no, increment_val, m_warehouse_id, doc_no, macaddr,currentnext)" +
                                                                                                "VALUES(" + Sequenc_id + "," + AD_Client_ID + "," + AD_ORG_ID + "," + AD_USER_ID + "," + AD_USER_ID + "," + AD_USER_ID + ",'" + _prefix + "'," + _startNo + "," + _endNo + "," + _incrementNo + "," + WarehouseId_Selected + "," + _docNo + ",'" + LoginViewModel.DeviceMacAddress() + "'," + _currentNext + "); ", connection);
                    INSERT_cmd_c_invoice_number_details.ExecuteNonQuery();
                    connection.Close();

                    connection.Open();
                    NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'c_invoice_number_details';", connection);
                    NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                    connection.Close();

                }

                #endregion POSOrderNumber

                #region All Product from the Orgination and Client for the a warehouse


                connection.Open();
                NpgsqlCommand cmd_m_product_check = new NpgsqlCommand("SELECT  * FROM m_product WHERE ad_client_id = " + AD_Client_ID + " AND ad_org_id = " + AD_ORG_ID + " ; ", connection);
                NpgsqlDataReader _m_product_Read = cmd_m_product_check.ExecuteReader();

                if (_m_product_Read.Read() && _m_product_Read.HasRows == true)
                {
                    connection.Close();
                }
                else
                {
                    #region Inserting Product if not exits

                    POSAllProducts Product = new POSAllProducts
                    {
                        operation = "POSAllProducts",
                        remindMe = _remindMe_post,
                        macAddress = _macAddress_post,
                        version = _version_post,
                        appName = _appName_post,
                        clientId = AD_Client_ID.ToString(),
                        orgId = AD_ORG_ID.ToString(),
                        pricelistId = Sys_config_pricelistId.ToString(),
                        costElementId = Sys_config_costElementId.ToString(),
                        warehouseId = WarehouseId_Selected.ToString()
                    };

                    jsonPOSAllProducts = JsonConvert.SerializeObject(Product);
                    this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Please Wait ..."; });
                    try
                    {
                        POSAllProductsApiStringResponce = PostgreSQL.ApiCallPost(jsonPOSAllProducts);
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
                        CheckServerError = 1;
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
                    }
                    catch
                    {
                        CheckServerError = 0;
                        this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                        log.Error("Login Error|POSAllProducts API Not Responding");
                        this.Dispatcher.Invoke(() =>
                        {

                            Login_Page.Opacity = 100;
                            Login_Page.IsEnabled = true;
                        });
                        return;
                    }
                    if (CheckServerError == 1)
                    {
                        POSAllProductsApiJSONResponce = JsonConvert.DeserializeObject(POSAllProductsApiStringResponce);

                        if (POSAllProductsApiJSONResponce.responseCode == "200")
                        {
                            //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: POSAllProductsApiResponce Responce Code 200"; });
                            JArray _products = POSAllProductsApiJSONResponce.products;
                            int _products_count = _products.Count();
                            string _itemCount = POSAllProductsApiJSONResponce.itemCount ?? "0";

                            connection.Close();
                            connection.Open();
                            NpgsqlCommand UPDATE_cmd_m_warehouse = new NpgsqlCommand("UPDATE m_warehouse SET attribute1 ='" + _itemCount + "' WHERE ad_client_id = " + AD_Client_ID + "  and ad_org_id = " + AD_ORG_ID + " and m_warehouse_id = " + WarehouseId_Selected + "; ", connection);
                            UPDATE_cmd_m_warehouse.ExecuteNonQuery();
                            connection.Close();

                            foreach (dynamic m_product in _products)
                            {
                                string _productId = m_product.productId;
                                string _productName = m_product.productName;
                                string _productValue = m_product.productValue;
                                string _categoryId = m_product.categoryId;
                                string _productArabicName = m_product.productArabicName ?? " ";
                                string _description = m_product.description;
                                string _scanbyWeight = m_product.scanbyWeight;
                                string _scanbyPrice = m_product.scanbyPrice;
                                string _ispriceEditable = m_product.isPriceEditable;
                                string _isquick = m_product.isQuick;
                                string _productUOMId = m_product.productUOMId;
                                string _productUOMValue = m_product.productUOMValue;
                                string _isBomAvailable = m_product.isBomAvailable;
                                string _sellingPrice = m_product.sellingPrice;
                                string _costprice = m_product.costprice;
                                string _productMultiUOM = m_product.productMultiUOM;
                                JArray _productMultiImage = m_product.productMultiImage;
                                JArray _productPriceArray = m_product.productPriceArray;
                                JArray _productsUOMArray = m_product.productsUOMArray;
                                JArray _productsBOMArray = m_product.bomDetails;
                                int img_count = _productMultiImage.Count;
                                string _product_image = " ";
                                if (img_count > 0)
                                {
                                    _product_image = _productMultiImage[0]["productImage"].ToString();
                                }

                                connection.Close();

                                connection.Open();
                                NpgsqlCommand cmd_select_sequenc_no = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'm_product';", connection);
                                NpgsqlDataReader _get__Ad_sequenc_no = cmd_select_sequenc_no.ExecuteReader();

                                if (_get__Ad_sequenc_no.Read())
                                {
                                    Sequenc_id = _get__Ad_sequenc_no.GetInt32(4) + 1;
                                }
                                connection.Close();
                                connection.Open();
                                NpgsqlCommand cmd_product_exist_m_produc = new NpgsqlCommand("SELECT m_product_id from m_product where m_product_id='" + _productId + "';", connection);
                                NpgsqlDataReader _get__product_exist_m_produc = cmd_product_exist_m_produc.ExecuteReader();


                                if (!_get__product_exist_m_produc.Read())
                                {
                                    this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Fetching Products Details .. "; });
                                    connection.Close();
                                    connection.Open();
                                    NpgsqlCommand INSERT_cmd_m_product = new NpgsqlCommand("INSERT INTO m_product(id, ad_client_id, ad_org_id, m_product_id, m_product_category_id, createdby, updatedby, name, searchkey, arabicname, image, scanbyweight, scanbyprice, uomid, uomname, sopricestd, currentcostprice,ispriceeditable,isquick, attribute1,attribute2,purchaseprice)VALUES(@id, @ad_client_id, @ad_org_id, @m_product_id, @m_product_category_id, @createdby, @updatedby, @name, @searchkey, @arabicname, @image, @scanbyweight, @scanbyprice, @uomid, @uomname, @sopricestd, @currentcostprice,@isPriceEditable,@isquick, @attribute1, @attribute2,@purchaseprice)", connection);

                                    INSERT_cmd_m_product.Parameters.AddWithValue("@id", Sequenc_id);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@ad_client_id", AD_Client_ID);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@ad_org_id", AD_ORG_ID);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@m_product_id", Convert.ToInt32(_productId));
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@m_product_category_id", Convert.ToInt32(_categoryId));
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@createdby", Convert.ToInt32(AD_USER_ID));
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@updatedby", Convert.ToInt32(AD_USER_ID));
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@name", _productName);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@searchkey", _productValue);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@arabicname", _productArabicName);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@image", _product_image);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@scanbyweight", _scanbyWeight);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@scanbyprice", _scanbyPrice);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@uomid", Convert.ToInt32(_productUOMId));
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@uomname", _productUOMValue);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@sopricestd", Convert.ToDouble(_sellingPrice));
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@currentcostprice", Convert.ToDouble(_costprice));
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@isPriceEditable", _ispriceEditable);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@isquick", _isquick);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@attribute1", _productMultiUOM);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@attribute2", WarehouseId_Selected);
                                    INSERT_cmd_m_product.Parameters.AddWithValue("@purchaseprice", Convert.ToDouble(_costprice));
                                    INSERT_cmd_m_product.ExecuteNonQuery();
                                    connection.Close();

                                    connection.Open();
                                    NpgsqlCommand cmd_update_sequenc_no = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'm_product';", connection);
                                    NpgsqlDataReader _update__Ad_sequenc_no = cmd_update_sequenc_no.ExecuteReader();
                                    connection.Close();
                                }
                                else
                                {
                                    connection.Close();
                                }
                                foreach (dynamic m_productPriceArray in _productPriceArray)
                                {
                                    string _productId_PriceArray = m_productPriceArray.productId;
                                    string _pricelistId_PriceArray = m_productPriceArray.pricelistId;
                                    if (string.IsNullOrEmpty(_pricelistId_PriceArray))
                                        _pricelistId_PriceArray = "0";
                                    string _pricelistName_PriceArray = m_productPriceArray.pricelistName;
                                    string _priceStd_PriceArray = m_productPriceArray.priceStd;
                                    if (string.IsNullOrEmpty(_priceStd_PriceArray))
                                        _priceStd_PriceArray = "0";
                                    connection.Close();

                                    connection.Open();
                                    NpgsqlCommand cmd_select_sequenc_no_m_product_price = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'm_product_price';", connection);
                                    NpgsqlDataReader _get__Ad_sequenc_no_m_product_price = cmd_select_sequenc_no_m_product_price.ExecuteReader();

                                    if (_get__Ad_sequenc_no_m_product_price.Read())
                                    {
                                        Sequenc_id = _get__Ad_sequenc_no_m_product_price.GetInt32(4) + 1;
                                    }
                                    connection.Close();

                                    connection.Open();
                                    NpgsqlCommand INSERT_cmd_m_product_price = new NpgsqlCommand("INSERT INTO m_product_price(id, ad_client_id, ad_org_id, m_product_id, pricelistid, pricestd,createdby,updatedby, name)" +
                                                                                            "VALUES(" + Sequenc_id + "," + AD_Client_ID + "," + AD_ORG_ID + ",'" + _productId_PriceArray + "','" + _pricelistId_PriceArray + "','" + _priceStd_PriceArray + "'," + AD_USER_ID + "," + AD_USER_ID + ",'" + _pricelistName_PriceArray + "'); ", connection);
                                    INSERT_cmd_m_product_price.ExecuteNonQuery();
                                    connection.Close();

                                    connection.Open();
                                    NpgsqlCommand cmd_update_sequenc_no_m_product_price = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'm_product_price';", connection);
                                    cmd_update_sequenc_no_m_product_price.ExecuteReader();
                                    connection.Close();
                                }
                                if (_productMultiUOM == "Y")
                                {
                                    foreach (dynamic m_productsUOMArray in _productsUOMArray)
                                    {
                                        string _uomId_UOMArray = m_productsUOMArray.uomId;
                                        string _uomValue_UOMArray = m_productsUOMArray.uomValue;
                                        string _uomConvRate_UOMArray = m_productsUOMArray.uomConvRate;

                                        connection.Close();

                                        connection.Open();
                                        NpgsqlCommand cmd_select_sequenc_no_m_product_price = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'm_product_uom';", connection);
                                        NpgsqlDataReader _get__Ad_sequenc_no_m_product_price = cmd_select_sequenc_no_m_product_price.ExecuteReader();

                                        if (_get__Ad_sequenc_no_m_product_price.Read())
                                        {
                                            Sequenc_id = _get__Ad_sequenc_no_m_product_price.GetInt32(4) + 1;
                                        }
                                        connection.Close();

                                        connection.Open();
                                        NpgsqlCommand INSERT_cmd_m_product_price = new NpgsqlCommand("INSERT INTO m_product_uom(ad_client_id, ad_org_id, createdby,updatedby, m_product_id, c_uom_id, uomvalue, uomconvrate)" +
                                                                                                        " VALUES(" + AD_Client_ID + "," + AD_ORG_ID + "," + AD_USER_ID + "," + AD_USER_ID + "," + _productId + "," + _uomId_UOMArray + ",'" + _uomValue_UOMArray + "'," + _uomConvRate_UOMArray + ");", connection);
                                        INSERT_cmd_m_product_price.ExecuteNonQuery();
                                        connection.Close();

                                        connection.Open();
                                        NpgsqlCommand cmd_update_sequenc_no_m_product_price = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'm_product_uom';", connection);
                                        cmd_update_sequenc_no_m_product_price.ExecuteReader();
                                        connection.Close();
                                    }
                                }
                                if (_isBomAvailable == "Y")
                                {
                                    foreach (dynamic m_productsBOMArray in _productsBOMArray)
                                    {
                                        string _BOMproductId = m_productsBOMArray.productId;
                                        string _BOMproductName = m_productsBOMArray.productName;
                                        string _BOMproductValue = m_productsBOMArray.productValue;
                                        string _BOMcategoryId = m_productsBOMArray.categoryId;
                                        string _BOMcategoryName = m_productsBOMArray.categoryName;
                                        string _BOMcategoryVAlue = m_productsBOMArray.categoryValue;
                                        string _BOMproductArabicName = m_productsBOMArray.productArabicName ?? " ";
                                        string _BOMdescription = m_productsBOMArray.description;
                                        string _BOMscanbyweight = m_productsBOMArray.scanbyWeight;
                                        string _BOMscanbyPrice = m_productsBOMArray.scanbyPrice;
                                        string _BOMisPriceEditable = m_productsBOMArray.isPriceEditable;
                                        string _BOMproductUOMId = m_productsBOMArray.productUOMId;
                                        string _BOMproductUOMValue = m_productsBOMArray.productUOMValue;
                                        string _BOMQty = m_productsBOMArray.bomQty;
                                        string _BOMsellingPrice = m_productsBOMArray.sellingPrice;
                                        string _BOMcostPrice = m_productsBOMArray.costPrice;
                                        JArray _BOMproductMultiImage = m_productsBOMArray.productMultiImage;
                                        img_count = _BOMproductMultiImage.Count;
                                        string _BOMproduct_image = " ";

                                        if (img_count > 0)
                                        {
                                            _BOMproduct_image = _BOMproductMultiImage[0]["productImage"].ToString();
                                        }
                                        connection.Close();
                                        connection.Open();
                                        NpgsqlCommand cmd_select_sequenc_no_m_product_price = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'm_product_uom';", connection);
                                        NpgsqlDataReader _get__Ad_sequenc_no_m_product_price = cmd_select_sequenc_no_m_product_price.ExecuteReader();

                                        if (_get__Ad_sequenc_no_m_product_price.Read())
                                        {
                                            Sequenc_id = _get__Ad_sequenc_no_m_product_price.GetInt32(4) + 1;
                                        }
                                        connection.Close();
                                        connection.Open();
                                        cmd_product_exist_m_produc = new NpgsqlCommand("SELECT m_product_id from m_product where m_product_id='" + _BOMproductId + "';", connection);
                                        _get__product_exist_m_produc = cmd_product_exist_m_produc.ExecuteReader();

                                        if (!_get__product_exist_m_produc.Read())
                                        {
                                            connection.Close();
                                            connection.Open();
                                            this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Fetching Product UOM Details .. "; });

                                            NpgsqlCommand INSERT_cmd_m_product_BOM = new NpgsqlCommand("INSERT INTO m_product(id, ad_client_id, ad_org_id, m_product_id, m_product_category_id, createdby, updatedby, name, searchkey, arabicname, image, scanbyweight, scanbyprice, uomid, uomname, sopricestd, currentcostprice,ispriceeditable,isquick, attribute1,attribute2,purchaseprice)VALUES(@id, @ad_client_id, @ad_org_id, @m_product_id, @m_product_category_id, @createdby, @updatedby, @name, @searchkey, @arabicname, @image, @scanbyweight, @scanbyprice, @uomid, @uomname, @sopricestd, @currentcostprice,@isPriceEditable,@isquick, @attribute1, @attribute2,@purchaseprice)", connection);

                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@id", Sequenc_id);
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@ad_client_id", AD_Client_ID);
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@ad_org_id", AD_ORG_ID);
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@m_product_id", Convert.ToInt32(_BOMproductId));
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@m_product_category_id", Convert.ToInt32(_BOMcategoryId));
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@createdby", Convert.ToInt32(AD_USER_ID));
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@updatedby", Convert.ToInt32(AD_USER_ID));
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@name", _BOMproductName);
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@searchkey", _BOMproductValue);
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@arabicname", _BOMproductArabicName);
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@image", _BOMproduct_image);
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@scanbyweight", _BOMscanbyweight);
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@scanbyprice", _BOMscanbyPrice);
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@uomid", Convert.ToInt32(_BOMproductUOMId));
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@uomname", _BOMproductUOMValue);
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@sopricestd", Convert.ToDouble(_BOMsellingPrice));
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@currentcostprice", Convert.ToDouble(_BOMcostPrice));
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@isPriceEditable", _BOMisPriceEditable);
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@isquick", "N");
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@attribute1", "N");
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@attribute2", WarehouseId_Selected);
                                            INSERT_cmd_m_product_BOM.Parameters.AddWithValue("@purchaseprice", Convert.ToDouble(_costprice));
                                            INSERT_cmd_m_product_BOM.ExecuteNonQuery();
                                            connection.Close();
                                        }
                                        else
                                        {
                                            connection.Close();
                                        }

                                        connection.Open();
                                        NpgsqlCommand INSERT_cmd_m_product_price = new NpgsqlCommand("INSERT INTO m_product_bom( ad_client_id,ad_org_id, m_product_id,no_of_pcs,  m_parent_product_id)" +
                                                                                                        " VALUES(" + AD_Client_ID + "," + AD_ORG_ID + "," + _BOMproductId + "," + _BOMQty + "," + _productId + ");", connection);
                                        INSERT_cmd_m_product_price.ExecuteNonQuery();
                                        connection.Close();

                                        connection.Open();
                                        NpgsqlCommand cmd_update_sequenc_no_m_product_price = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'm_product_uom';", connection);
                                        cmd_update_sequenc_no_m_product_price.ExecuteReader();
                                        connection.Close();
                                    }
                                }
                                connection.Open();
                                NpgsqlCommand command = new NpgsqlCommand("select productId  from m_pos_sequence WHERE clientId=" + AD_Client_ID + " and orgId=" + AD_ORG_ID + "", connection);
                                NpgsqlDataReader _get_cmd_barcode = command.ExecuteReader();
                                if (!_get_cmd_barcode.HasRows)
                                {
                                    connection.Close();
                                    connection.Open();
                                    NpgsqlCommand INSERT_cmd_pos_sequence = new NpgsqlCommand("INSERT INTO m_pos_sequence(orgid,clientid,categoryid,productid,bomproductid,uomid,bpid,posid)" +
                                     " VALUES(" + AD_ORG_ID + "," + AD_Client_ID + ",0,0,0,0,0,0)", connection);
                                    INSERT_cmd_pos_sequence.ExecuteNonQuery();
                                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: ad_client_id  new data  Inserted"; });
                                    connection.Close();

                                }
                                else
                                {
                                    connection.Close();
                                }

                            }
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                            log.Error("Login Error: POSAllProducts");
                            log.Error("Server Responce:" + POSOrderNumberApiJSONResponce);
                            this.Dispatcher.Invoke(() =>
                            {

                                Login_Page.Opacity = 100;
                                Login_Page.IsEnabled = true;
                            });
                            return;
                        }
                    }

                    #endregion Inserting Product if not exits
                }

                #endregion All Product from the Orgination and Client for the a warehouse

                #region Save Current UserDetails

                connection.Close();

                connection.Open();
                NpgsqlCommand cmd_ad_user_pos_check = new NpgsqlCommand("SELECT sessionid, ad_client_id, ad_org_id,name,password,islogged,isactive FROM ad_user_pos where name = '" + _username_post + "' AND password = '" + _password_post + "' AND ad_client_id = " + AD_Client_ID + "AND ad_org_id = " + AD_ORG_ID + " ; ", connection);
                NpgsqlDataReader _get_ad_user_pos_check = cmd_ad_user_pos_check.ExecuteReader();

                if (_get_ad_user_pos_check.Read())
                {
                    double AD_SessionID = _get_ad_user_pos_check.GetDouble(0);
                    Application.Current.Properties["SessionID"] = _get_ad_user_pos_check.GetDouble(0).ToString();
                    connection.Close();
                    //Updating All Users to N
                    //ekta
                    //connection.Open();
                    //NpgsqlCommand cmd_ad_user_pos_All = new NpgsqlCommand("select update_ad_user_pos();", connection);
                    //cmd_ad_user_pos_All.ExecuteReader();
                    //connection.Close();
                    //ekta
                    connection.Open();
                    NpgsqlCommand cmd_User_Login = new NpgsqlCommand("UPDATE ad_user_pos SET ad_role_id= " + AD_ROLE_ID + ", c_bpartner_id=" + Sys_config_businessPartnerId + ", m_warehouse_id=" + WarehouseId_Selected + ", islogged='Y', isactive='Y'  WHERE ad_client_id = " + AD_Client_ID + "  and ad_org_id = " + AD_ORG_ID + " and ad_user_id = " + AD_USER_ID + "; ", connection);
                    cmd_User_Login.ExecuteReader();

                    connection.Close();
                }
                else
                {
                    connection.Close();
                    connection.Open();
                    NpgsqlCommand cmd_select_sequenc_no_ad_user_pos = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name = 'ad_user_pos';", connection);
                    NpgsqlDataReader _get__Ad_sequenc_no_ad_user_pos = cmd_select_sequenc_no_ad_user_pos.ExecuteReader();

                    if (_get__Ad_sequenc_no_ad_user_pos.Read())
                    {
                        Sequenc_id = _get__Ad_sequenc_no_ad_user_pos.GetInt32(4) + 1;
                    }
                    connection.Close();

                    connection.Open();
                    NpgsqlCommand Get_previous_cmd_ad_user_pos = new NpgsqlCommand("SELECT ad_user_pos_id, ad_user_id, islogged FROM ad_user_pos where islogged = 'Y';", connection);
                    NpgsqlDataReader _getGet_previous_cmd_ad_user_pos = Get_previous_cmd_ad_user_pos.ExecuteReader();
                    if (_getGet_previous_cmd_ad_user_pos.Read())
                    {
                        int previous_id = _getGet_previous_cmd_ad_user_pos.GetInt32(0);
                        int previous_ad_user_id = _getGet_previous_cmd_ad_user_pos.GetInt32(1);

                        connection.Close();

                        connection.Open();
                        NpgsqlCommand UPDATE_previous_ad_user_pos = new NpgsqlCommand("UPDATE ad_user_pos " +
                            "SET islogged='N', isactive='N'  WHERE ad_user_pos_id = " + previous_id + " and ad_user_id =" + previous_ad_user_id + " ;", connection);
                        UPDATE_previous_ad_user_pos.ExecuteNonQuery();
                        connection.Close();
                    }
                    connection.Close();

                    connection.Open();
                    NpgsqlCommand INSERT_cmd_ad_user_pos = new NpgsqlCommand("INSERT INTO ad_user_pos(ad_user_pos_id, ad_client_id, ad_org_id, ad_role_id, c_bpartner_id,ad_user_id," +
                    "m_warehouse_id, name, password, islogged, isactive, createdby, updatedby)" +
                    "VALUES(" + Sequenc_id + "," + AD_Client_ID + "," + AD_ORG_ID + "," + AD_ROLE_ID + "," + Sys_config_businessPartnerId + "," + AD_USER_ID + "," + WarehouseId_Selected + ",'" + _username_post + "','" + _password_post + "','Y','Y'," + AD_USER_ID + "," + AD_USER_ID + "); ", connection);
                    INSERT_cmd_ad_user_pos.ExecuteNonQuery();
                    connection.Close();

                    connection.Open();
                    NpgsqlCommand cmd_update_sequenc_no_ad_user_pos = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name = 'ad_user_pos';", connection);
                    cmd_update_sequenc_no_ad_user_pos.ExecuteReader();
                    connection.Close();
                }

                #endregion Save Current UserDetails


                this.Dispatcher.Invoke(() =>
                {
                    Login_Page.Opacity = 100;
                    Login_Page.IsEnabled = true;
                    LoginProcessingText.Text = "";

                    ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Retail;
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {

                    Login_Page.Opacity = 100;
                    Login_Page.IsEnabled = true;
                    LoginProcessingText.Text = "Please Select Warehouse to Proceed";
                });
            }


        }


        /// <summary>
        /// Selecting the Warehouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WarehouseId_Selected = ((M_warehouse)((System.Windows.Controls.Primitives.Selector)e.Source).SelectedItem).M_warehouse_id;
        }
        private static string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private static void SetSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var entry = configuration.AppSettings.Settings[key];
            if (entry == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddUser());
        }


        /// <summary>
        /// Used for Database Connection and Other Api Details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>




        private void LoadPreviousLoggedUser()
        {
            NpgsqlConnection connection = new NpgsqlConnection(PostgreSQL.ConnectionString);
            //Getting Last Logged User Details
            connection.Open();
            NpgsqlCommand cmd_PreviousLoggedUser = new NpgsqlCommand("SELECT " +
                "t1.ad_client_id, " +             //0
                "t1.ad_org_id," +                 //1
                "t1.m_warehouse_id, " +           //2
                "t1.name, " +                     //3
                "t1.password, " +                 //4
                "t1.islogged, " +                 //5
                "t1.isactive," +                  //6
                "t2.m_warehouse_id , " +          //7
                "t2.name as m_warehouse_name  " + //8
                "FROM ad_user_pos t1 , m_warehouse t2 where t1.m_warehouse_id = t2.m_warehouse_id and t1.islogged = 'Y'; ", connection);

            NpgsqlDataReader _get_PreviousLoggedUser = cmd_PreviousLoggedUser.ExecuteReader();
            if (_get_PreviousLoggedUser.HasRows == true && _get_PreviousLoggedUser.Read())
            {
                Login_Email.Text = _get_PreviousLoggedUser.GetString(3);
                Login_Password.Password = String.Empty;
                connection.Close();
            }
            else
            {
                connection.Close();
            }
        }

        private static dynamic LoadPreviousLoggedUserWarehouse()
        {
            NpgsqlConnection connection = new NpgsqlConnection(PostgreSQL.ConnectionString);
            //Getting Last Logged User Details
            connection.Open();
            NpgsqlCommand cmd_PreviousLoggedUser = new NpgsqlCommand("SELECT " +
                "t1.ad_client_id, " +             //0
                "t1.ad_org_id," +                 //1
                "t1.m_warehouse_id, " +           //2
                "t1.name, " +                     //3
                "t1.password, " +                 //4
                "t1.islogged, " +                 //5
                "t1.isactive," +                  //6
                "t2.m_warehouse_id , " +          //7
                "t2.name as m_warehouse_name  " + //8
                "FROM ad_user_pos t1 , m_warehouse t2 where t1.m_warehouse_id = t2.m_warehouse_id and t1.islogged = 'Y'; ", connection);

            NpgsqlDataReader _get_PreviousLoggedUser = cmd_PreviousLoggedUser.ExecuteReader();
            if (_get_PreviousLoggedUser.HasRows == true && _get_PreviousLoggedUser.Read())
            {
                double M_Warehouse_ID = _get_PreviousLoggedUser.GetInt32(2);
                connection.Close();
                return M_Warehouse_ID;
            }
            else
            {
                connection.Close();
                return 0;
            }
        }

        private void TestMail_Click(object sender, RoutedEventArgs e)
        {
            //  string JSON = "{\"operation\": \"GetSalesSummary\",\"username\": \"gulfcashier\",\"password\": \"123456\",\"clientId\": 1000049,\"orgId\": 1000070,\"userId\": 1005329,\"roleId\": 1000328,\"sessionId\": 1001902.0,\"businessPartnerId\": 1011080,\"warehouseId\": 1000103,\"cashbookId\": 1000092,\"startTime\": 1556549080000,\"endTime\": 1556549333146,\"startTime_Atc\": \"29/04/2019 08:14:40\",\"endTime_Atc\": \"29/04/2019 08:18:53\",\"SyncedTime\": 0,\"showImage\": \"N\",\"macAddress\": \"BFEBFBFF000906EA\",\"version\": 1.0,\"appName\": \"POS\",\"remindMe\": \"Y\",\"UserName\": \"gulfcashier\",\"AD_ORG_logo\": \"/9j/4QAYRXhpZgAASUkqAAgAAAAAAAAAAAAAAP/sABFEdWNreQABAAQAAAA8AAD/4QMvaHR0cDov\r\nL25zLmFkb2JlLmNvbS94YXAvMS4wLwA8P3hwYWNrZXQgYmVnaW49Iu+7vyIgaWQ9Ilc1TTBNcENl\r\naGlIenJlU3pOVGN6a2M5ZCI/PiA8eDp4bXBtZXRhIHhtbG5zOng9ImFkb2JlOm5zOm1ldGEvIiB4\r\nOnhtcHRrPSJBZG9iZSBYTVAgQ29yZSA1LjYtYzEzOCA3OS4xNTk4MjQsIDIwMTYvMDkvMTQtMDE6\r\nMDk6MDEgICAgICAgICI+IDxyZGY6UkRGIHhtbG5zOnJkZj0iaHR0cDovL3d3dy53My5vcmcvMTk5\r\nOS8wMi8yMi1yZGYtc3ludGF4LW5zIyI+IDxyZGY6RGVzY3JpcHRpb24gcmRmOmFib3V0PSIiIHht\r\nbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyIgeG1sbnM6eG1wTU09Imh0dHA6\r\nLy9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUu\r\nY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bXA6Q3JlYXRvclRvb2w9IkFkb2JlIFBo\r\nb3Rvc2hvcCBDQyAyMDE3IChXaW5kb3dzKSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDo4RDEy\r\nRkE0MkFDRTYxMUU4OTBGOUEzNzRBNDcyNThFNSIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDo4\r\nRDEyRkE0M0FDRTYxMUU4OTBGOUEzNzRBNDcyNThFNSI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJl\r\nZjppbnN0YW5jZUlEPSJ4bXAuaWlkOjhEMTJGQTQwQUNFNjExRTg5MEY5QTM3NEE0NzI1OEU1IiBz\r\ndFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjhEMTJGQTQxQUNFNjExRTg5MEY5QTM3NEE0NzI1OEU1\r\nIi8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQg\r\nZW5kPSJyIj8+/+4ADkFkb2JlAGTAAAAAAf/bAIQABgQEBAUEBgUFBgkGBQYJCwgGBggLDAoKCwoK\r\n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\",\"AD_ORG_arabicname\": \"\",\"AD_ORG_name\": \"Gulf Shell\",\"AD_Warehouse_Name\": \"Al Kharaitiyat\",\"AD_ORG_city\": \"DOHA\",\"AD_ORG_phone\": \"+97450777822\",\"Denominations\": [{\"total\": 11500.0,\"count\": 23.0,\"type\": \"riyal\",\"name\": 500    },{\"total\": 400.0,\"count\": 4.0,\"type\": \"riyal\",\"name\": 100    },{\"total\": 250.0,\"count\": 5.0,\"type\": \"riyal\",\"name\": 50    },{\"total\": 0.0,\"count\": 0.0,\"type\": \"riyal\",\"name\": 10    },{\"total\": 30.0,\"count\": 6.0,\"type\": \"riyal\",\"name\": 5    },{\"total\": 5.0,\"count\": 5.0,\"type\": \"riyal\",\"name\": 1    },{\"total\": 1.5,\"count\": 3.0,\"type\": \"dirhams\",\"name\": 50    },{\"total\": 0.75,\"count\": 3.0,\"type\": \"dirhams\",\"name\": 25    },{\"total\": 2.0,\"count\": 1,\"type\": \"cash\",\"name\": 2    },{\"total\": 0,\"count\": 1,\"type\": \"complement\",\"name\": 1    },{\"total\": 2.0,\"count\": 0,\"type\": \"total\",\"name\": 0    }]}";

            // Sessions.Sales_Summary_APICall(JSON);
            //ErrorMail.InvoicePostingLog();
        }

    }
}