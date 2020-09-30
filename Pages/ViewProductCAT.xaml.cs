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
using System.Diagnostics;
using System.Web.UI.WebControls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;

namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for ProductCAT.xaml
    /// </summary>
    public partial class ViewProductCAT : Page
    {
        #region 

        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        int _Bpartner_Id = Int32.Parse(Application.Current.Properties["BpartnerId"].ToString());
        int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public List<m_ProductCATLst> _m_ProductCATLst = new List<m_ProductCATLst>();
        private dynamic AddCategoryApiStringResponce,
        addcategoryApiJOSNResponce;
        public dynamic AddCategoryApiJOSNResponce { get => addcategoryApiJOSNResponce; set => addcategoryApiJOSNResponce = value; }
        private string jsonq;
        private int CheckServerError = 0;

        public string connstring = PostgreSQL.ConnectionString;
        string imagePath = "";
        #endregion

        #region Public Function
        public ViewProductCAT()
        {
            InitializeComponent();
            BindProductCATLst();
            EnableData();
           
            
        }
        public void EnableData()
        {
            txtName.IsEnabled = false;
            txtSearchKey.IsEnabled = false;
            txtCategory.IsEnabled = false;
            CBL_DigitalMenu.IsEnabled = false;
            Self_Service.IsEnabled = false;
            btn_edit.IsEnabled = false;

            btn_Cancel.Visibility = Visibility.Hidden;

            btn_Save.Visibility = Visibility.Hidden;
            btnupload.Visibility = Visibility.Hidden;
        }
        #endregion


        #region validation
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtName.Text = Regex.Replace(txtName.Text, "[^a-zA-Z ]+", "");

        }
        
        #endregion

        #region Bind Data Function
        public void BindProductCATLst()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_ProductCATLst_GetData = new NpgsqlCommand("select m_product_category_id,name from m_product_category  where ad_org_id=" + _OrgId + "order by name ;", connection);//
                NpgsqlDataReader _ProductCATLst_GetData = cmd_ProductCATLst_GetData.ExecuteReader();
                _m_ProductCATLst.Clear();
                _m_ProductCATLst.Add(new m_ProductCATLst()
                {
                    id = 0,
                    Name = "All"


                });
                while (_ProductCATLst_GetData.Read())
                {

                    _m_ProductCATLst.Add(new m_ProductCATLst()
                    {
                        id = int.Parse(_ProductCATLst_GetData["m_product_category_id"].ToString()),
                        Name = _ProductCATLst_GetData["name"].ToString()


                    });
                }

                connection.Close();

                if (_m_ProductCATLst.Count == 0)
                {
                    MessageBox.Show("Please Add Product Category!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ProductCAT_DropDown.ItemsSource = _m_ProductCATLst;

                    });
                }


            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Product Category  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Product Category Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void BindProductCATList()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_ProductCATList_GetData = new NpgsqlCommand("select m_product_category_id as id,name,image from m_product_category  where ad_org_id=" + _OrgId + " order by name ;", connection);//
                NpgsqlDataAdapter daPCList = new NpgsqlDataAdapter(cmd_ProductCATList_GetData);
                DataTable dtPCList = new DataTable();
                daPCList.Fill(dtPCList);
                connection.Close();
                ProductCD_ListBox.ItemsSource = "";
                if (dtPCList.Rows.Count > 0)
                {

                    ProductCD_ListBox.ItemsSource = dtPCList.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add Product Category!");
                    return;
                }
              
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Product Category  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Product Category Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }

        public void BindProductCATWise(dynamic id)
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_ProductCATWise_GetData = new NpgsqlCommand("select m_product_category_id as id,name,image  from m_product_category  where ad_org_id=" + _OrgId + " And m_product_category_id='" + id + "' ;", connection);//
                NpgsqlDataAdapter daPCWise = new NpgsqlDataAdapter(cmd_ProductCATWise_GetData);
                DataTable dtPCWise = new DataTable();
                daPCWise.Fill(dtPCWise);
                connection.Close();
                ProductCD_ListBox.ItemsSource = "";
                if (dtPCWise.Rows.Count > 0)
                {

                    ProductCD_ListBox.ItemsSource = dtPCWise.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add Product Category!");
                    return;
                }
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Product Category  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Product Category Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }

        public void BindProductCAT(string Search_Key)
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_ProductCAT_GetData = new NpgsqlCommand("select m_product_category_id as id,Name,image as Path from m_product_category  where ad_org_id=" + _OrgId + " And upper(name) like '" + Search_Key.ToUpper() + "%' ;", connection);//
                NpgsqlDataAdapter daPC = new NpgsqlDataAdapter(cmd_ProductCAT_GetData);
                DataTable dtPC = new DataTable();
                daPC.Fill(dtPC);
                connection.Close();
                ProductCD_ListBox.ItemsSource = "";
                if (dtPC.Rows.Count > 0)
                {
                    
                    ProductCD_ListBox.ItemsSource = dtPC.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add Product Category!");
                    return;
                }

            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Product Category  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Product Category Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        #endregion

        #region Event
        private void Product_Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (product_Search.Text != null)
            {
                string Search_Key = product_Search.Text;
                BindProductCAT(Search_Key);
            }
            else
            {
                BindProductCATList();
            }

        }

        private void ProductCD_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic PC = ProductCD_ListBox.SelectedItem as dynamic;
            if(PC==null)
            {

            }
            else
            {
                dynamic id = PC[0];
                dynamic Name = PC[1];
                btn_edit.IsEnabled = true;

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_ProductCAT_GetData = new NpgsqlCommand("select m_product_category_id,Name,searchkey,categoryColour,DigitalMenu,selfService,image from m_product_category  where m_product_category_id=" + id + " and ad_org_id=" + _OrgId + " and Name='" + Name + "' ;", connection);//
                Npgsql.NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd_ProductCAT_GetData);
                DataTable dt = new DataTable();
                da.Fill(dt);
                connection.Close();
                if (dt.Rows.Count > 0)
                {
                    txtProductCATID.Text = dt.Rows[0]["m_product_category_id"].ToString();
                    txtName.Text = dt.Rows[0]["Name"].ToString();
                    txtSearchKey.Text = dt.Rows[0]["searchkey"].ToString();
                    txtCategory.Text = dt.Rows[0]["categoryColour"].ToString();

                    if (dt.Rows[0]["DigitalMenu"].ToString() == "Y")
                    {
                        CBL_DigitalMenu.IsChecked = true;
                    }
                    else
                    {
                        CBL_DigitalMenu.IsChecked = false;
                    }

                    if (dt.Rows[0]["selfService"].ToString() == "Y")
                    {
                        Self_Service.IsChecked = true;
                    }
                    else
                    {
                        Self_Service.IsChecked = false;
                    }
                    try
                    {
                        editimage.Source = new BitmapImage(new Uri(dt.Rows[0]["image"].ToString(), UriKind.Absolute));

                    }
                    catch
                    {
                        editimage.Source = null;

                    }
                }
            }
          




        }

        private void ProductCAT_DropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic PC = ProductCAT_DropDown.SelectedItem as dynamic;
            int id = PC.id;
            if (ProductCAT_DropDown.SelectedIndex <= 0)
            {
                BindProductCATList();
            }
            else
            {
                BindProductCATWise(id);
            }
        }


        private void Btn_edit_Click(object sender, RoutedEventArgs e)
        {
            txtName.IsEnabled = true;
            txtSearchKey.IsEnabled = true;
            txtCategory.IsEnabled = true;
            CBL_DigitalMenu.IsEnabled = true;
            Self_Service.IsEnabled = true;
            btn_Cancel.Visibility = Visibility.Visible;
            btn_Save.Visibility = Visibility.Visible;
            btnupload.Visibility = Visibility.Visible;
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewProductCAT());
        }

        private async void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtSearchKey.Text == String.Empty)
                {
                    txtSearchKey.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The SearchKey ! ");
                    return;
                }
                if (txtName.Text == String.Empty)
                {
                    txtName.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Name ! ");
                    return;
                }
                if (txtCategory.Text == String.Empty)
                {
                    txtCategory.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Category! ");
                    return;
                }
                await Task.Run(() =>
                {
                    EditCategoryAPICall();
                }

                );

            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Update Product Category  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Edit Product Category ", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }
        }
        public void EditCategoryAPICall()
        {
            this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Please Wait .. "; });
            this.Dispatcher.Invoke(() =>
            {
                POSAddCategoryApiModel AddCategoryDetail = new POSAddCategoryApiModel
                {
                    macAddress = "8e17517b8cd41f3c",
                    clientId = _clientId,
                    orgId = _OrgId,
                    warehouseId = _Warehouse_Id,
                    userId = _UserID,
                    version = "2.0",
                    appName = "POS",
                    operation = "POSEditCategory",
                    categoryId = Int32.Parse(txtProductCATID.Text),
                    categoryName = txtName.Text,
                    description = txtCategory.Text,
                };

                jsonq = JsonConvert.SerializeObject(AddCategoryDetail);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Passing Data to Api Call"; });
            });

            try
            {
                AddCategoryApiStringResponce = PostgreSQL.ApiCallPost(jsonq);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
                CheckServerError = 1;
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
            }
            catch
            {
                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                this.Dispatcher.Invoke(() =>
                {
                    string DigitalMenu = "";
                    string SelfService = "";
                    if (CBL_DigitalMenu.IsChecked == true)
                    {
                        DigitalMenu = "Y";
                    }
                    else
                    {
                        DigitalMenu = "N";
                    }
                    if (Self_Service.IsChecked == true)
                    {
                        SelfService = "Y";
                    }
                    else
                    {
                        SelfService = "N";
                    }

                    // connection.Close();
                    connection.Open();
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                    NpgsqlCommand Edit_ProductCAT_List = new NpgsqlCommand("Update m_product_category set Name='" + txtName.Text + "', searchkey='" + txtSearchKey.Text + "', categoryColour='" + txtCategory.Text + "',updatedby=" + _UserID + " ,DigitalMenu='" + DigitalMenu + "',selfService='" + SelfService + "',is_sync='0' ,image='"+imagePath+"'  where m_product_category_id='" + txtProductCATID.Text + "' and ad_org_id=" + _OrgId + ";", connection);//
                    Edit_ProductCAT_List.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("Record Edit Successfully");
                    NavigationService.Navigate(new ViewProductCAT());
                });
                //CheckServerError = 0;
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                //log.Error("EditCategory Error|POSOrganization API Not Responding");
                //return;
            }
            if (CheckServerError == 1)
            {
                AddCategoryApiJOSNResponce = JsonConvert.DeserializeObject(AddCategoryApiStringResponce);

                if (AddCategoryApiJOSNResponce.responseCode == "200")
                {
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: loginApiResponce Responce Code 200"; });

                    int _categoryId = AddCategoryApiJOSNResponce.categoryId;
                    string _image = AddCategoryApiJOSNResponce.categoryImage;
                    NpgsqlConnection connection = new NpgsqlConnection(connstring);
                    this.Dispatcher.Invoke(() =>
                    {
                        string DigitalMenu = "";
                        string SelfService = "";
                        if (CBL_DigitalMenu.IsChecked == true)
                        {
                            DigitalMenu = "Y";
                        }
                        else
                        {
                            DigitalMenu = "N";
                        }
                        if (Self_Service.IsChecked == true)
                        {
                            SelfService = "Y";
                        }
                        else
                        {
                            SelfService = "N";
                        }

                        // connection.Close();
                        connection.Open();
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                        NpgsqlCommand Edit_ProductCAT_List = new NpgsqlCommand("Update m_product_category set Name='" + txtName.Text + "', searchkey='" + txtSearchKey.Text + "', categoryColour='" + txtCategory.Text + "',updatedby=" + _UserID + " ,DigitalMenu='" + DigitalMenu + "',selfService='" + SelfService + "',image='" + imagePath + "',m_product_category_id='" + _categoryId + "',is_sync='1'  where m_product_category_id='" + txtProductCATID.Text + "' and ad_org_id=" + _OrgId + ";", connection);//
                        Edit_ProductCAT_List.ExecuteNonQuery();
                        connection.Close();
                        MessageBox.Show("Record Edit Successfully");
                        NavigationService.Navigate(new ViewProductCAT());
                    });

                }
                else if (AddCategoryApiJOSNResponce.responseCode == "106")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        LoginProcessingText.Text = "Product Category not edited  !";

                    });
                    return;
                }
            }
        }

    

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().ToUpper() == "F4")
            {
                Btn_Save_Click(sender, e);
            }
            if (e.Key.ToString().ToUpper() == "ESCAPE")
            {
                Btn_Cancel_Click(sender, e);
            }
        }

        private void Btn_Add_Category_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddProductCAT());
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

        #endregion
    }
}
