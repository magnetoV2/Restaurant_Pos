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
using System.Data;

namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class ViewProduct : Page
    {

        #region 
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());
       
        int _pricelistId = Int32.Parse(Application.Current.Properties["pricelistId"].ToString());
        int _costElementId = Int32.Parse(Application.Current.Properties["costElementId"].ToString());
        List<M_UOM> m_UOM_items = new List<M_UOM>();
        List<M_ItemCategory> m_ItemCategory_items = new List<M_ItemCategory>();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //public List<m_Product> _Product_items = new List<m_Product>();
        public List<M_Terminals> _m_Terminals_items = new List<M_Terminals>();
        public List<m_ProductList> m_ProductList_items = new List<m_ProductList>();


        public string connstring = PostgreSQL.ConnectionString;

        private dynamic ProductApiStringResponce,
        productApiJOSNResponce;
        public dynamic ProductApiJOSNResponce { get => productApiJOSNResponce; set => productApiJOSNResponce = value; }
        private string jsonq;
        private int CheckServerError = 0;
        List<String> imagePath = new List<String>();
        #endregion

        #region Public Function
        public ViewProduct()
        {
            InitializeComponent();
            BindProduct();
            Enabled();
            BindUOM();
            BindItemCategory();
            BindTerminal();
        }
        public void Enabled()
        {
            txtbarcode.IsEnabled = false;
            txtMaximum.IsEnabled = false;
            txtMinimum.IsEnabled = false;
            txtName.IsEnabled = false;
            txtPurchasePrice.IsEnabled = false;
            txtSalePrice.IsEnabled = false;
            UOM_DropDown.IsEnabled = false;
            ItemCategory_DropDown.IsEnabled = false;
            cblPriceEditable.IsEnabled = false;
            cblSellOnline.IsEnabled = false;
            btn_edit.IsEnabled = false;
            btn_Cancel.Visibility = Visibility.Hidden;
            btn_Save.Visibility = Visibility.Hidden;
            terminal_DropDown.IsEnabled = false;
            btnupload.Visibility = Visibility.Hidden;
            uploadText.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Bind Data Function
        public void BindTerminal()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_Terminal_GetData = new NpgsqlCommand("SELECT m_terminal_id,name FROM  m_terminal  WHERE  ad_client_id = " + _clientId + "; ", connection);//
                NpgsqlDataReader _Terminal_GetData = cmd_Terminal_GetData.ExecuteReader();
                while (_Terminal_GetData.Read())
                {

                    _m_Terminals_items.Add(new M_Terminals()
                    {
                        id = int.Parse(_Terminal_GetData["m_terminal_id"].ToString()),
                        Name = _Terminal_GetData["name"].ToString()


                    });
                }

                connection.Close();

                if (_m_Terminals_items.Count == 0)
                {
                    MessageBox.Show("Please Add Terminals!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        terminal_DropDown.ItemsSource = _m_Terminals_items.OrderBy(item => item.Name);
                    });
                }


            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Terminals  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In  Terminals Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void BindItemCategory()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_ItemCategory_GetData = new NpgsqlCommand("select m_product_category_id,name from  m_product_category  WHERE ad_org_id =" + _OrgId + " AND ad_client_id =" + _clientId + " ;", connection);//
                NpgsqlDataReader _ItemCategory_GetData = cmd_ItemCategory_GetData.ExecuteReader();

                while (_ItemCategory_GetData.Read())
                {
                    m_ItemCategory_items.Add(new M_ItemCategory()
                    {
                        ItemId = int.Parse(_ItemCategory_GetData["m_product_category_id"].ToString()),
                        Name = _ItemCategory_GetData["name"].ToString()
                    });
                }
                connection.Close();
                if (m_ItemCategory_items.Count == 0)
                {
                    MessageBox.Show("Please Add Item Category!");

                    return;

                }
                else
                {

                    this.Dispatcher.Invoke(() =>
                    {

                        ItemCategory_DropDown.ItemsSource = m_ItemCategory_items.OrderBy(item => item.Name);
                    });
                }
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Item Category POS  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Item Category Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void BindUOM()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_UOM_GetData = new NpgsqlCommand("select c_uom_id,name  from  m_product_uom  WHERE ad_org_id =" + _OrgId + " AND ad_client_id =" + _clientId + " ;", connection);//
                NpgsqlDataReader _UOM_GetData = cmd_UOM_GetData.ExecuteReader();

                while (_UOM_GetData.Read())
                {
                    m_UOM_items.Add(new M_UOM()
                    {
                        UOMId = Int32.Parse(_UOM_GetData["c_uom_id"].ToString()),
                        Name = _UOM_GetData["name"].ToString()
                    });
                }
                connection.Close();

                if (m_UOM_items.Count == 0)
                {
                    MessageBox.Show("Please Add UOM!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        UOM_DropDown.ItemsSource = m_UOM_items.OrderBy(item => item.Name);
                    });
                }
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In UOM POS  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In UOM Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void BindProduct()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_ProductList_GetData = new NpgsqlCommand("select m_product_id,Name from m_product  where ad_org_id=" + _OrgId + " order by Name ;", connection);//
                NpgsqlDataReader _ProductList_GetData = cmd_ProductList_GetData.ExecuteReader();
                m_ProductList_items.Add(new m_ProductList()
                {
                    id = 0,
                    Name = "All"


                });
                while (_ProductList_GetData.Read())
                {

                    m_ProductList_items.Add(new m_ProductList()
                    {
                        id = Int32.Parse(_ProductList_GetData["m_product_id"].ToString()),
                        Name = _ProductList_GetData["Name"].ToString()


                    });
                }

                connection.Close();

                if (m_ProductList_items.Count == 0)
                {
                    MessageBox.Show("Please Add Product !");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        Product_DropDown.ItemsSource = m_ProductList_items;
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
        public void BindProductList()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_Product_GetData = new NpgsqlCommand("select m_product_id,name,image from m_product  where ad_org_id=" + _OrgId + " order by name ;", connection);//
                NpgsqlDataAdapter daProduct = new NpgsqlDataAdapter(cmd_Product_GetData);
                DataTable dtProduct = new DataTable();
                daProduct.Fill(dtProduct);
                connection.Close();
                Product_ListBox.ItemsSource = "";
                if (dtProduct.Rows.Count > 0)
                {

                    Product_ListBox.ItemsSource = dtProduct.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add Product !");
                    return;
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
        public void BindProductWise(int id)
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_ProductWise_GetData = new NpgsqlCommand("select m_product_id,Name,image from m_product  where ad_org_id=" + _OrgId + " And m_product_id='" + id + "' ;", connection);//
                NpgsqlDataAdapter daProductwise = new NpgsqlDataAdapter(cmd_ProductWise_GetData);
                DataTable dtProductwise = new DataTable();
                daProductwise.Fill(dtProductwise);
                connection.Close();
                Product_ListBox.ItemsSource = "";
                if (dtProductwise.Rows.Count > 0)
                {

                    Product_ListBox.ItemsSource = dtProductwise.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add Product !");
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
        public void BindProductLst(string Search_Key)
        {
            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_ProductList_GetData = new NpgsqlCommand("select m_product_id,Name,image from m_product  where ad_org_id=" + _OrgId + " and upper(name) Like '" + Search_Key.ToUpper() + "%' ;", connection);//
                NpgsqlDataAdapter daProductList = new NpgsqlDataAdapter(cmd_ProductList_GetData);
                DataTable dtProductList = new DataTable();
                daProductList.Fill(dtProductList);
                connection.Close();
                Product_ListBox.ItemsSource = "";
                if (dtProductList.Rows.Count > 0)
                {

                    Product_ListBox.ItemsSource = dtProductList.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add Product !");
                    return;
                }


            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In ProductList Is Not Found!   =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In ProductList Is Not Found!", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }
        }
        #endregion
        #region call api POSAddProduct
        public void EditProductAPICall()
        {

            this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Please Wait .. "; });
            this.Dispatcher.Invoke(() =>
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                string PriceEditable = "";
                string SellOnline = "";
                if (cblPriceEditable.IsChecked == true)
                {
                    PriceEditable = "Y";
                }
                else
                {
                    PriceEditable = "N";

                }
                if (cblSellOnline.IsChecked == true)
                {
                    SellOnline = "Y";
                }
                else
                {
                    SellOnline = "N";
                }
                dynamic UC = UOM_DropDown.SelectedItem as dynamic;
                int UOMId = UC.UOMId;
                string UOMName = UC.Name;
                dynamic IC = ItemCategory_DropDown.SelectedItem as dynamic;
                int ItemId = IC.ItemId;
                string CategoryName = IC.Name;

                bomDetails _bomDetails = new bomDetails
                {
                    sellingPrice = txtSalePrice.Text,
                    categoryName = txtName.Text,
                    costPrice = txtPurchasePrice.Text,
                    isPriceEditable = PriceEditable,
                    categoryId = ItemId,
                    productValue = txtbarcode.Text,
                    uomName = UOMName,
                    bomQty = "0",
                    uomId = UOMId,
                    productName = txtName.Text,
                    purchasePrice = txtPurchasePrice.Text,
                    productId = -1
                };
                POSAddProduct _AddProduct = new POSAddProduct
                {
                    clientId = _clientId,
                    orgId = _OrgId,
                    macAddress = "8e17517b8cd41f3c",
                    version = "2.0",
                    operation = "POSAddProduct",
                    userId = _UserID,
                    warehouseId = _Warehouse_Id,
                    pricelistId = _pricelistId,
                    appName = "POS",
                    accountSchemaId = _OrgId,
                    productId = -1,
                    productName = txtName.Text,
                    productArabicName = txtName.Text,
                    productValue = txtbarcode.Text,
                    productCategoryId = ItemId,
                    categoryName = CategoryName,
                    purchasePrice = txtPurchasePrice.Text,
                    costPrice = txtPurchasePrice.Text,
                    sellingPrice = txtSalePrice.Text,
                    uomId = UOMId,
                    uomName = UOMName,
                    costElementId = _costElementId,
                    isPriceEditable = PriceEditable,
                    isBomAvailable = "Y",
                };

                _AddProduct.bomdetails.Add(_bomDetails);

                jsonq = JsonConvert.SerializeObject(_AddProduct);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Passing Data to Api Call"; });
            });

            try
            {
                ProductApiStringResponce = PostgreSQL.ApiCallPost(jsonq);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
                CheckServerError = 1;
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
            }
            catch
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                this.Dispatcher.Invoke(() =>
                {
                    dynamic UD = UOM_DropDown.SelectedItem as dynamic;
                    int UOMId = UD.UOMId;
                    string Name = UD.Name;
                    dynamic IC = ItemCategory_DropDown.SelectedItem as dynamic;
                    int ItemId = IC.ItemId;
                    dynamic PL = Product_ListBox.SelectedItem as dynamic;
                    dynamic id = PL[0];
                    dynamic Ter = terminal_DropDown.SelectedItem as dynamic;
                    int terminalID = Ter.id; 
                    string PriceEditable = "";
                    string SellOnline = "";
                    if (cblPriceEditable.IsChecked == true)
                    {
                        PriceEditable = "Y";
                    }
                    else
                    {
                        PriceEditable = "N";
                    }
                    if (cblSellOnline.IsChecked == true)
                    {
                        SellOnline = "Y";
                    }
                    else
                    {
                        SellOnline = "N";
                    }

                    // connection.Close();

                    connection.Open();
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                    NpgsqlCommand Edit_Product_List = new NpgsqlCommand("update m_product set m_product_category_id='" + ItemId + "', Name='" + txtName.Text + "', barcode='" + txtbarcode.Text + "', uomid='" + UOMId + "', uomname='" + Name + "', sellonline='" + SellOnline + "',PriceEditable='" + PriceEditable + "', purchaseprice='" + txtPurchasePrice.Text + "', Currentcostprice='" + txtSalePrice.Text + "', max_qty='" + txtMaximum.Text + "', min_qty='" + txtMinimum.Text + "' ,is_sync='0',updatedby=" + _UserID + ",m_terminal_id='" + terminalID + "',image='"+imagePath+"'  where m_product_id='" + id + "' and ad_org_id=" + _OrgId + ";", connection);//
                    Edit_Product_List.ExecuteNonQuery();
                    connection.Close();
                    if (imagePath.Count > 0)
                    {
                        foreach (string image in imagePath)
                        {
                            {
                                if (connection.State == ConnectionState.Closed)
                                    connection.Open();
                                string command = "insert into m_product_images(ad_client_id,ad_org_id,createdby,updatedby,image_url,m_product_id) VALUES(" + _clientId + " ," + _OrgId + " ," + _UserID + "," + _UserID + ",'" + image + "'," + id + ")";
                                NpgsqlCommand cmd_product_image = new NpgsqlCommand(command, connection);
                                cmd_product_image.ExecuteNonQuery();
                                connection.Close();
                            }

                        }
                    }
                    MessageBox.Show("Record Update Successfully");
                    NavigationService.Navigate(new ViewProduct());
                });
                CheckServerError = 0;
                //    this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                //    log.Error("Product Error|POSOrganization API Not Responding");
                //    return;
            }
            if (CheckServerError == 1)
            {
                ProductApiJOSNResponce = JsonConvert.DeserializeObject(ProductApiStringResponce);

                if (ProductApiJOSNResponce.responseCode == "200")
                {
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: loginApiResponce Responce Code 200"; });

                    int _productId = ProductApiJOSNResponce.productId;
                    string _image = ProductApiJOSNResponce.productImage;
                    NpgsqlConnection connection = new NpgsqlConnection(connstring);
                    this.Dispatcher.Invoke(() =>
                    {
                        dynamic UD = UOM_DropDown.SelectedItem as dynamic;
                        int UOMId = UD.UOMId;
                        string Name = UD.Name;
                        dynamic IC = ItemCategory_DropDown.SelectedItem as dynamic;
                        int ItemId = IC.ItemId;
                        dynamic PL = Product_ListBox.SelectedItem as dynamic;
                        dynamic id = PL[0];
                        dynamic Ter = terminal_DropDown.SelectedItem as dynamic;
                        int terminalID = Ter.id;
                        string PriceEditable = "";
                        string SellOnline = "";
                        if (cblPriceEditable.IsChecked == true)
                        {
                            PriceEditable = "Y";
                        }
                        else
                        {
                            PriceEditable = "N";
                        }
                        if (cblSellOnline.IsChecked == true)
                        {
                            SellOnline = "Y";
                        }
                        else
                        {
                            SellOnline = "N";
                        }

                        // connection.Close();
                        connection.Open();
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                        NpgsqlCommand Edit_Product_List = new NpgsqlCommand("update m_product set m_product_category_id='" + ItemId + "', Name='" + txtName.Text + "', barcode='" + txtbarcode.Text + "', uomid='" + UOMId + "', uomname='" + Name + "', sellonline='" + SellOnline + "',PriceEditable='" + PriceEditable + "', purchaseprice='" + txtPurchasePrice.Text + "', Currentcostprice='" + txtSalePrice.Text + "', max_qty='" + txtMaximum.Text + "', min_qty='" + txtMinimum.Text + "' ,is_sync='1',m_product_id='" + _productId + "',updatedby=" + _UserID + " ,m_terminal_id='" + terminalID + "',image='" + imagePath + "'   where m_product_id='" + txtProductID.Text + "' and ad_org_id=" + _OrgId + ";", connection);//
                        Edit_Product_List.ExecuteNonQuery();
                        connection.Close();
                        if (imagePath.Count > 0)
                        {
                            foreach (string image in imagePath)
                            {
                                {
                                    if (connection.State == ConnectionState.Closed)
                                        connection.Open();
                                    string command = "insert into m_product_images(ad_client_id,ad_org_id,createdby,updatedby,image_url,m_product_id) VALUES(" + _clientId + " ," + _OrgId + " ," + _UserID + "," + _UserID + ",'" + image + "'," + _productId + ")";
                                    NpgsqlCommand cmd_product_image = new NpgsqlCommand(command, connection);
                                    cmd_product_image.ExecuteNonQuery();
                                    connection.Close();
                                }

                            }
                        }
                        MessageBox.Show("Record Update Successfully");
                        NavigationService.Navigate(new ViewProduct());
                    });

                }
                else if (ProductApiJOSNResponce.responseCode == "106")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        LoginProcessingText.Text = "Product not created   !";

                    });
                    return;
                }
            }
        }


        #endregion
        #region Event
        private void Product_Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (product_Search.Text != null)
            {
                string Search_Key = product_Search.Text;
                BindProductLst(Search_Key);
            }
            else
            {
                BindProductList();
            }

        }

        private void Product_DropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            dynamic PC = Product_DropDown.SelectedItem as dynamic;
            int id = PC.id;
            if (Product_DropDown.SelectedIndex <= 0)
            {
                BindProductList();
            }
            else
            {
                BindProductWise(id);
            }
        }

        private void Btn_edit_Click(object sender, RoutedEventArgs e)
        {
            txtbarcode.IsEnabled = true;
            txtMaximum.IsEnabled = true;
            txtMinimum.IsEnabled = true;
            txtName.IsEnabled = true;
            txtPurchasePrice.IsEnabled = true;
            txtSalePrice.IsEnabled = true;
            UOM_DropDown.IsEnabled = true;
            ItemCategory_DropDown.IsEnabled = true;
            cblPriceEditable.IsEnabled = true;
            cblSellOnline.IsEnabled = true;
            btn_Cancel.Visibility = Visibility.Visible;
            btn_Save.Visibility = Visibility.Visible;
            terminal_DropDown.IsEnabled = true;
            uploadText.Visibility = Visibility.Visible;

            btnupload.Visibility = Visibility.Visible;
        }

        private void Btn_Add_Category_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddProduct());

        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewProduct());
        }

        private async void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtbarcode.Text == String.Empty)
                {
                    txtbarcode.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The BarCode ! ");
                    return;
                }
                if (txtName.Text == String.Empty)
                {
                    txtName.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Name ! ");
                    return;
                }
                if (txtMaximum.Text == String.Empty)
                {
                    txtMaximum.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Maximum Qty! ");
                    return;
                }
                if (txtMinimum.Text == String.Empty)
                {
                    txtMinimum.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Minimum Qty! ");
                    return;
                }
                if (txtPurchasePrice.Text == String.Empty)
                {
                    txtPurchasePrice.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Purchase Price ! ");
                    return;
                }
                if (txtSalePrice.Text == String.Empty)
                {
                    txtSalePrice.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Sale Price ! ");
                    return;
                }

                await Task.Run(() =>
                {
                    EditProductAPICall();
                }

                                );


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


        private void Product_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                dynamic PL = Product_ListBox.SelectedItem as dynamic;
                if(PL==null)
                {

                }
                else
                {
                    dynamic id = PL[0];
                    dynamic Name = PL[1];
                    btn_edit.IsEnabled = true;

                    NpgsqlConnection connection = new NpgsqlConnection(connstring);
                    // connection.Close();
                    connection.Open();
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                    NpgsqlCommand cmd_list_GetData = new NpgsqlCommand("select barcode,Name,m_product_category_id,uomid,purchaseprice, Currentcostprice, max_qty, min_qty,Sellonline,PriceEditable,m_terminal_id as id,image from m_product   where m_product_id=" + id + " and ad_org_id=" + _OrgId + " and Name='" + Name + "' ;", connection);//
                    NpgsqlDataAdapter dalist = new NpgsqlDataAdapter(cmd_list_GetData);
                    DataTable dtlist = new DataTable();
                    dalist.Fill(dtlist);
                    connection.Close();

                    if (dtlist.Rows.Count > 0)
                    {

                        txtbarcode.Text = dtlist.Rows[0]["barcode"].ToString();
                        txtName.Text = dtlist.Rows[0]["Name"].ToString();
                        int ItemCategory_SelectedItemIndex = m_ItemCategory_items.FindIndex(i => i.ItemId == Int32.Parse(dtlist.Rows[0]["m_product_category_id"].ToString()));
                        ItemCategory_DropDown.SelectedIndex = ItemCategory_SelectedItemIndex;
                        int UOM_SelectedItemIndex = m_UOM_items.FindIndex(i => i.UOMId == Int32.Parse(dtlist.Rows[0]["uomid"].ToString()));
                        UOM_DropDown.SelectedIndex = UOM_SelectedItemIndex;
                        txtPurchasePrice.Text = dtlist.Rows[0]["purchaseprice"].ToString();
                        txtSalePrice.Text = dtlist.Rows[0]["Currentcostprice"].ToString();
                        txtMaximum.Text = dtlist.Rows[0]["max_qty"].ToString();
                        txtMinimum.Text = dtlist.Rows[0]["min_qty"].ToString();


                        if (dtlist.Rows[0]["Sellonline"].ToString() == "Y")
                        {
                            cblSellOnline.IsChecked = true;
                        }
                        else
                        {
                            cblSellOnline.IsChecked = false;
                        }


                        if (dtlist.Rows[0]["PriceEditable"].ToString() == "Y")
                        {
                            cblPriceEditable.IsChecked = true;
                        }
                        else
                        {
                            cblSellOnline.IsChecked = false;
                        }

                        int Terminal_SelectedItemIndex = _m_Terminals_items.FindIndex(i => i.id == int.Parse(dtlist.Rows[0]["id"].ToString()));
                        terminal_DropDown.SelectedIndex = Terminal_SelectedItemIndex;
                        txtProductID.Text = id.ToString();
                        editimage.Source = new BitmapImage(new Uri(dtlist.Rows[0]["image"].ToString(), UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("Please Add Product !");
                        return;
                    }

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


        #endregion

        #region validation

        private void Txtbarcode_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtbarcode.Text = Regex.Replace(txtbarcode.Text, "[^0-9]+", "");
        }

        private void TxtPurchasePrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPurchasePrice.Text = Regex.Replace(txtPurchasePrice.Text, "[^0-9.]+", "");
        }

        private void TxtSalePrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtSalePrice.Text = Regex.Replace(txtSalePrice.Text, "[^0-9]+", "");
        }

        private void TxtMinimum_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtMinimum.Text = Regex.Replace(txtMinimum.Text, "[^0-9]+", "");
        }

        private void TxtMaximum_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtMaximum.Text = Regex.Replace(txtMaximum.Text, "[^0-9]+", "");
        }
        #endregion

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

        private void Btnupload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {  //openFileDialog.se
                Uri fileUri = new Uri(openFileDialog.FileName);
                string[] filePaths = openFileDialog.FileNames;

                string destinationPath = "C:/pos/component/Resources/Images/ProductImg";
                string directoryPath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

                DirectoryInfo diSource = new DirectoryInfo(directoryPath);
                DirectoryInfo diTarget = new DirectoryInfo(@destinationPath);
                foreach (string path in filePaths)
                {
                    var Filename = System.IO.Path.GetFileName(path);
                    Copy(diSource, diTarget, Filename.ToString());
                    imagePath.Add(diTarget.FullName + "/" + Filename);
                }

                editimage.Source = new BitmapImage(fileUri);
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
