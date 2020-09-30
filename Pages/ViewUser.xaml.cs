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
using System.Windows.Media.Imaging;
using Restaurant_Pos.Pages.UserControls;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace Restaurant_Pos.Pages
{

    /// <summary>
    /// Interaction logic for ViewUser.xaml
    /// </summary>
    public partial class ViewUser : Page
    {
        #region 
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      

        public List<M_warehouse> m_warehouse_items = new List<M_warehouse>();
        public List<M_user> m_user_items = new List<M_user>();
        public List<M_Roles> m_Roles_items = new List<M_Roles>();
        public string connstring = PostgreSQL.ConnectionString;
        string imagePath = "";
        #endregion

        #region Public Function

        public ViewUser()
        {
            InitializeComponent();

          
            //BindWareHouse();
            TextEnabled();
            BindUser();
            BindWareHouse();
            BindRole();
           

        }
        public void TextEnabled()
        {
            txtQID.IsEnabled = false;
            txtName.IsEnabled = false;
            txtUserName.IsEnabled = false;
            txtPassword.IsEnabled = false;
            txtMobile.IsEnabled = false;
            txtEmail.IsEnabled = false;
            txtAddress.IsEnabled = false;
            Warehouse_DropDown.IsEnabled = false;
            Role_DropDown.IsEnabled = false;
            btn_edit.IsEnabled = false;
            btnCancel.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Hidden;
            btnupload.Visibility = Visibility.Hidden;

        }
        #endregion

        #region validation
        private void txtQID_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtQID.Text = Regex.Replace(txtQID.Text, "[^0-9]+", "");

        }
        private void txtMobile_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtMobile.Text = Regex.Replace(txtMobile.Text, "[^0-9]+", "");

        }
        private void TxtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtEmail.Text = Regex.Replace(txtEmail.Text, " ", "");

        }
        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtUserName.Text = Regex.Replace(txtUserName.Text, " ", "");

        }
        private void txtAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtAddress.Text = Regex.Replace(txtAddress.Text, " ", "");

        }
        #endregion

        #region Bind Data Function
        public void BindRole()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_Roles_GetData = new NpgsqlCommand("SELECT name,ad_role_id FROM ad_role  WHERE  ad_client_id = " + _clientId + "; ", connection);//
                NpgsqlDataReader _Roles_GetData = cmd_Roles_GetData.ExecuteReader();

                while (_Roles_GetData.Read())
                {
                    m_Roles_items.Add(new M_Roles()
                    {
                        Name = _Roles_GetData["name"].ToString()  ,
                        RoleID = Int32.Parse(_Roles_GetData["ad_role_id"].ToString())
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

                log.Error(" ===================  Error In Roles  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Roles Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                Environment.Exit(0);
            }




        }
        public void BindWareHouse()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_warehouseDetails_GetData = new NpgsqlCommand("SELECT  name,m_warehouse_id FROM m_warehouse  WHERE ad_org_id =" + _OrgId + " AND ad_client_id =" + _clientId + " order by id;", connection);//
                NpgsqlDataReader _warehouseDetails_GetData = cmd_warehouseDetails_GetData.ExecuteReader();

                while (_warehouseDetails_GetData.Read())
                {
                    m_warehouse_items.Add(new M_warehouse()
                    {
                        WarehouseName = _warehouseDetails_GetData["name"].ToString(),
                        WarehouseId= Int32.Parse(_warehouseDetails_GetData["m_warehouse_id"].ToString())
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
        public void BindUser()
        {
          

                try
                {

                    NpgsqlConnection connection = new NpgsqlConnection(connstring);
                    // connection.Close();
                    connection.Open();
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                    NpgsqlCommand cmd_User_GetData = new NpgsqlCommand("select ad_user_id,name from ad_user_pos  WHERE ad_org_id =" + _OrgId + " order by name ;", connection);//
                    NpgsqlDataReader _cmd_User_GetData = cmd_User_GetData.ExecuteReader();
                m_user_items.Add(new M_user()
                {
                    UserID = 0,
                    Name = "All"
                });
                while (_cmd_User_GetData.Read())
                    {
                        m_user_items.Add(new M_user()
                        {
                            UserID = Int32.Parse(_cmd_User_GetData["ad_user_id"].ToString()),
                            Name = _cmd_User_GetData["name"].ToString()
                        });
                    }
                    connection.Close();

                if (m_user_items.Count == 0)
                {
                    MessageBox.Show("Please Add User!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        User_DropDown.ItemsSource = m_user_items;
                    });

                }
            }
               
                catch (Exception ex)
                {

                    log.Error(" ===================  Error In User =========================== ");
                    log.Error(DateTime.Now.ToString());
                    log.Error(ex.ToString());
                    log.Error(" ===================  End of Error  =========================== ");
                    if (MessageBox.Show(ex.ToString(),
                            "Error In User Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                    {
                        return;
                    }
                    return;
                }




            
        }
        public void BindUserList()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_User_GetData = new NpgsqlCommand("select ad_user_id,name,imgpath  from ad_user_pos  where ad_org_id=" + _OrgId+ " order by name;", connection);//
                NpgsqlDataAdapter daUser = new NpgsqlDataAdapter(cmd_User_GetData);
                DataTable dtUser = new DataTable();
                daUser.Fill(dtUser);
                connection.Close();
                User_ListBox.ItemsSource = "";
                if (dtUser.Rows.Count > 0)
                {

                    User_ListBox.ItemsSource = dtUser.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add users!");
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
                        "Error In Users Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void BindUserWise(int ID)
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_Usr_GetData = new NpgsqlCommand("select ad_user_id,name,imgpath from ad_user_pos  where ad_org_id=" + _OrgId + " And ad_user_id="+ID+" ;", connection);//
                NpgsqlDataAdapter daUsr = new NpgsqlDataAdapter(cmd_Usr_GetData);
                DataTable dtUsr = new DataTable();
                daUsr.Fill(dtUsr);
                connection.Close();
                User_ListBox.ItemsSource = "";
                if (dtUsr.Rows.Count > 0)
                {

                    User_ListBox.ItemsSource = dtUsr.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add users!");
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
                        "Error In Users Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void UserSearch(string Key)
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_U_GetData = new NpgsqlCommand("select ad_user_id,name,imgpath from ad_user_pos  where ad_org_id=" + _OrgId + " And upper(name) LIKE  '" + Key.ToUpper()  + "%' ;", connection);//
                NpgsqlDataAdapter daU = new NpgsqlDataAdapter(cmd_U_GetData);
                DataTable dtU = new DataTable();
                daU.Fill(dtU);
                connection.Close();
                User_ListBox.ItemsSource = "";
                if (dtU.Rows.Count > 0)
                {

                    User_ListBox.ItemsSource = dtU.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add users!");
                    return;
                }
              
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In User Is Not Found!  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Users Is Not Found!", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        #endregion

        #region Event
        private void User_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
               
                dynamic UL = User_ListBox.SelectedItem as dynamic;
                if (UL == null)
                {

                }
                else
                {
                    dynamic id = UL[0];
                    dynamic Name = UL[1];
                    btn_edit.IsEnabled = true;

                    NpgsqlConnection connection = new NpgsqlConnection(connstring);
                    connection.Open();
                    NpgsqlCommand cmd_Userlist_GetData = new NpgsqlCommand("select aup.QID,aup.name,aup.username,aup.password,aup.mobile,aup.email,aup.address,aup.m_warehouse_id,ar.Name as roleName,aup.imgpath,aup.ad_user_id from ad_user_pos aup inner join ad_role ar on ar.ad_role_id=aup.ad_role_id  where aup.ad_user_id=" + id + " and aup.ad_org_id=" + _OrgId + " and aup.Name='" + Name + "' ;", connection);//
                    NpgsqlDataAdapter daUserlist = new NpgsqlDataAdapter(cmd_Userlist_GetData);
                    DataTable dtUserlist = new DataTable();
                    daUserlist.Fill(dtUserlist);
                    connection.Close();

                    if (dtUserlist.Rows.Count > 0)
                    {

                        txtQID.Text = dtUserlist.Rows[0]["QID"].ToString();
                        txtName.Text = dtUserlist.Rows[0]["name"].ToString();
                        txtUserName.Text = dtUserlist.Rows[0]["username"].ToString();
                        txtPassword.Text = dtUserlist.Rows[0]["password"].ToString();
                        txtMobile.Text = dtUserlist.Rows[0]["mobile"].ToString();
                        txtEmail.Text = dtUserlist.Rows[0]["email"].ToString();
                        txtAddress.Text = dtUserlist.Rows[0]["address"].ToString();
                        Warehouse_DropDown.SelectedItem = dtUserlist.Rows[0]["m_warehouse_id"].ToString();
                        int Warehouse_SelectedItemIndex = m_warehouse_items.FindIndex(i => i.WarehouseId == Int32.Parse(dtUserlist.Rows[0]["m_warehouse_id"].ToString()));
                        Warehouse_DropDown.SelectedIndex = Warehouse_SelectedItemIndex;
                        int Roles_SelectedItemIndex = m_Roles_items.FindIndex(i => i.Name == dtUserlist.Rows[0]["roleName"].ToString());
                        Role_DropDown.SelectedIndex = Roles_SelectedItemIndex;
                        string imgpath = dtUserlist.Rows[0]["imgpath"].ToString();
                        txtUserID.Text = dtUserlist.Rows[0]["ad_user_id"].ToString();
                        editimage.Source = new BitmapImage(new Uri(dtUserlist.Rows[0]["imgpath"].ToString(), UriKind.Relative));
                   


                    }
                    else
                    {
                        MessageBox.Show("Please Add users!");
                        return;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please Add users!");
                return;
            }
        
        }

      

        private void Product_Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (product_Search.Text != null)
            {
                UserSearch(product_Search.Text);
            }
            else
            {
                BindUserList();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtName.Text == String.Empty)
                {
                    txtName.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Name ! ");
                    return;
                }
                if (txtUserName.Text == String.Empty)
                {
                    txtUserName.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
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

                if (txtMobile.Text == String.Empty)
                {
                    txtMobile.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Mobile No. ! ");
                    return;
                }
                dynamic WH = Warehouse_DropDown.SelectedItem as dynamic;
                int WarehouseId = WH.WarehouseId;
                dynamic RD = Role_DropDown.SelectedItem as dynamic;
                int RoleID = RD.RoleID;
                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand Edit_User_List = new NpgsqlCommand("Update ad_user_pos set QID=" + txtQID.Text + ", name='" + txtName.Text + "', username='" + txtUserName.Text + "', password='" + txtPassword.Text + "', mobile='" + txtMobile.Text + "', email='" + txtEmail.Text + "', address='" + txtAddress.Text + "', m_warehouse_id='" + WarehouseId + "', ad_role_id='" + RoleID + "' ,updatedby=" + _UserID + ",imgpath='" + imagePath + "'   where ad_user_id='" + txtUserID.Text + "' and ad_org_id=" + _OrgId + ";", connection);//
                Edit_User_List.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Record Edit Successfully");
                NavigationService.Navigate(new ViewUser());
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Update User Detail  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Update User Detail ", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }
            
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
          
            NavigationService.Navigate(new ViewUser());
        }

        private void Btn_edit_Click(object sender, RoutedEventArgs e)
        {
            //MainTab.Items.Clear(); //Clear previous Items in the user controls which is my tabItems
            //var userControls = new AllSidemenu();
            //_tabUserPage = new TabItem { Content = userControls };
            //MainTab.Items.Add(_tabUserPage); // Add User Controls
            //MainTab.Items.Refresh();
            txtQID.IsEnabled = true;
            txtName.IsEnabled = true;
            txtUserName.IsEnabled = true;
            txtPassword.IsEnabled = true;
            txtMobile.IsEnabled = true;
            txtEmail.IsEnabled = true;
            txtAddress.IsEnabled = true;
            Warehouse_DropDown.IsEnabled = true;
            Role_DropDown.IsEnabled = true;
            btnCancel.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Visible;
            btnupload.Visibility = Visibility.Visible;
        }

        private void Btn_Add_Category_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddUser());
        }

        private void User_DropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic UD = User_DropDown.SelectedValue as dynamic;
            int id = UD.UserID;
            if (User_DropDown.SelectedIndex <= 0)
            {
                BindUserList();
            }
            else
            {
                BindUserWise(id);
            }
        }



        #endregion

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

        private void Btnupload_Click(object sender, RoutedEventArgs e)
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
