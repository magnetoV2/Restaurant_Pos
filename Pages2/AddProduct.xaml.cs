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
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Page
    {
        #region 
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        int _Bpartner_Id = Int32.Parse(Application.Current.Properties["BpartnerId"].ToString());
        int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());
        int _pricelistId = Int32.Parse(Application.Current.Properties["pricelistId"].ToString());
        int _costElementId = Int32.Parse(Application.Current.Properties["costElementId"].ToString());
        List<M_UOM> m_UOM_items = new List<M_UOM>();
        List<M_ItemCategory> m_ItemCategory_items = new List<M_ItemCategory>();
        public List<M_Terminals> m_Terminals_items = new List<M_Terminals>();
        public string connstring = PostgreSQL.ConnectionString;
       
        List<String> imagePath = new List<String>();
        public int Sequenc_id { get; set; }
        public int ProductSequenc_id { get; set; }
        private dynamic ProductApiStringResponce,
        productApiJOSNResponce;
        public dynamic ProductApiJOSNResponce { get => productApiJOSNResponce; set => productApiJOSNResponce = value; }
        private string jsonq;
        private int CheckServerError = 0;
        #endregion

        #region Public Function
        public AddProduct()
        {
            InitializeComponent();
            BindUOM();
            BindItemCategory();
            BindTerminal();
        }
        public void ClearData()
        {
            txtBarCode.Text = "";
            txtName.Text = "";
            txtMaximum.Text = "";
            txtMinimum.Text = "";
            txtPurchasePrice.Text = "";
            txtSalePrice.Text = "";
            product_image.Source = null;

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
                NpgsqlCommand cmd_Terminal_GetData = new NpgsqlCommand("SELECT m_terminal_id,Name FROM  m_terminal  WHERE  ad_client_id = " + _clientId + "; ", connection);//
                NpgsqlDataReader _Terminal_GetData = cmd_Terminal_GetData.ExecuteReader();
                while (_Terminal_GetData.Read())
                {

                    m_Terminals_items.Add(new M_Terminals()
                    {
                        id = _Terminal_GetData.GetInt32(0),
                        Name = _Terminal_GetData.GetString(1)


                    });
                }

                connection.Close();

                if (m_Terminals_items.Count == 0)
                {
                    MessageBox.Show("Please Add Terminals!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        terminal_DropDown.ItemsSource = m_Terminals_items;
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
                        ItemId = _ItemCategory_GetData.GetInt32(0),
                        Name = _ItemCategory_GetData.GetString(1)
                    });
                }
                connection.Close();

                if (m_ItemCategory_items.Count == 0)
                {
                    MessageBox.Show("Please Add UOM!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        ItemCategory_DropDown.ItemsSource = m_ItemCategory_items;
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
                    Environment.Exit(0);
                }
                Environment.Exit(0);
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
                        UOMId = _UOM_GetData.GetInt32(0),
                        Name = _UOM_GetData.GetString(1)
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

                        UOM_DropDown.ItemsSource = m_UOM_items;
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
                    Environment.Exit(0);
                }
                Environment.Exit(0);
            }




        }

        #endregion
        #region call api POSAddProduct
        public void ProductAPICall()
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
                productValue = txtBarCode.Text,
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
                productValue = txtBarCode.Text,
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
                    dynamic UC = UOM_DropDown.SelectedItem as dynamic;
                    int UOMId = UC.UOMId;
                    string UOMName = UC.Name;
                    dynamic IC = ItemCategory_DropDown.SelectedItem as dynamic;
                    int ItemId = IC.ItemId;
                    string CategoryName = IC.Name;
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

                    connection.Open();
                    NpgsqlCommand cmd_select_sequenc_no_ad_user_pos = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name='m_product_bom';", connection);
                    NpgsqlDataReader _get__Ad_sequenc_no_ad_user_pos = cmd_select_sequenc_no_ad_user_pos.ExecuteReader();
                    if (_get__Ad_sequenc_no_ad_user_pos.Read())
                    {
                        Sequenc_id = _get__Ad_sequenc_no_ad_user_pos.GetInt32(4) + 1;
                    }
                    connection.Close();
                    connection.Open();
                    NpgsqlCommand cmd_select_sequenc_no_ad_product= new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name='m_product';", connection);
                    NpgsqlDataReader _select_sequenc_no_ad_product = cmd_select_sequenc_no_ad_product.ExecuteReader();
                    if (_select_sequenc_no_ad_product.Read())
                    {
                        ProductSequenc_id = _select_sequenc_no_ad_product.GetInt32(4) + 1;
                    }
                    connection.Close();

                    connection.Open();
                    NpgsqlCommand INSERT_cmd_Product_Detail = new NpgsqlCommand("insert into m_product(id,ad_client_id,ad_org_id,m_product_id,Createdby,isactive, m_product_category_id, Name, barcode, uomid, uomname, image, sellonline, purchaseprice, Currentcostprice, max_qty, min_qty,PriceEditable,is_sync,m_terminal_id)    " + " " + "" + "" +
                                                                                                  "VALUES(" + ProductSequenc_id + "," + _clientId + " ," + _OrgId + " ," + ProductSequenc_id + "," + _UserID + ",'Y','" + ItemId + "','" + txtName.Text + "','" + txtBarCode.Text + "','" + UOMId + "','" + UOMName + "','" + imagePath[0].ToString() + "','" + SellOnline + "','" + txtPurchasePrice.Text + "','" + txtSalePrice.Text + "','" + txtMaximum.Text + "','" + txtMinimum.Text + "','" + PriceEditable + "','0','" + terminalID + "') ;", connection);
                    INSERT_cmd_Product_Detail.ExecuteNonQuery();
                    connection.Close();
                    connection.Open();

                 foreach ( string  image in imagePath)
                    {
                        {
                            

                            string command = "insert into m_product_images(ad_client_id,ad_org_id,createdby,updatedby,image_url,m_product_id) VALUES(" + _clientId + " ," + _OrgId + " ," + _UserID + "," + _UserID + ",'" + image + "'," + ProductSequenc_id + ")";
                            NpgsqlCommand cmd_product_image = new NpgsqlCommand(command, connection);
                            cmd_product_image.ExecuteNonQuery();
                        }

                    }
                    connection.Close();
                    connection.Open();
                    NpgsqlCommand INSERT_cmd_BomProduct_Detail = new NpgsqlCommand("insert into m_product_bom(m_product_bom_id,ad_client_id,ad_org_id,m_product_id,no_of_pcs,m_parent_product_id,is_sync)    " + " " + "" + "" +
                                                                                                  "VALUES(" + Sequenc_id + "," + _clientId + " ," + _OrgId + " ," + ProductSequenc_id + "," + UOMId + "," + ProductSequenc_id + ",'0') ;", connection);
                    INSERT_cmd_BomProduct_Detail.ExecuteNonQuery();

                    connection.Close();
                    connection.Open();
                    NpgsqlCommand cmd_update_sequenc_no_ad_user_pos = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name='m_product_bom';", connection);
                    cmd_update_sequenc_no_ad_user_pos.ExecuteReader();
                    connection.Close();

                    connection.Open();
                    NpgsqlCommand cmd_update_sequenc_no_m_product = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name='m_product';", connection);
                    cmd_update_sequenc_no_m_product.ExecuteReader();
                    connection.Close();
                    ClearData();

                    MessageBox.Show("Record Save Successfully");
                    NavigationService.Navigate(new AddProduct());
                });
                //CheckServerError = 0;
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                //log.Error("Product Error|POSOrganization API Not Responding");
                //return;
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
                    connection.Open();
                    this.Dispatcher.Invoke(() =>
                    {
                        dynamic UC = UOM_DropDown.SelectedItem as dynamic;
                        int UOMId = UC.UOMId;
                        string UOMName = UC.Name;
                        dynamic IC = ItemCategory_DropDown.SelectedItem as dynamic;
                        int ItemId = IC.ItemId;
                        string CategoryName = IC.Name;
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

                       
                        NpgsqlCommand cmd_select_sequenc_no_ad_user_pos = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name='m_product_bom';", connection);
                        NpgsqlDataReader _get__Ad_sequenc_no_ad_user_pos = cmd_select_sequenc_no_ad_user_pos.ExecuteReader();
                        if (_get__Ad_sequenc_no_ad_user_pos.Read())
                        {
                            Sequenc_id = _get__Ad_sequenc_no_ad_user_pos.GetInt32(4) + 1;
                        }
                        connection.Close();

                        connection.Open();
                        NpgsqlCommand INSERT_cmd_Product_Detail = new NpgsqlCommand("insert into m_product(id,ad_client_id,ad_org_id,m_product_id,Createdby,isactive, m_product_category_id, Name, barcode, uomid, uomname, image, sellonline, purchaseprice, Currentcostprice, max_qty, min_qty,PriceEditable,is_sync,m_terminal_id)    " + " " + "" + "" +
                                                                                                      "VALUES(" + _productId + "," + _clientId + " ," + _OrgId + " ," + _productId + "," + _UserID + ",'Y','" + ItemId + "','" + txtName.Text + "','" + txtBarCode.Text + "','" + UOMId + "','" + UOMName + "','" + imagePath[0] + "','" + SellOnline + "','" + txtPurchasePrice.Text + "','" + txtSalePrice.Text + "','" + txtMaximum.Text + "','" + txtMinimum.Text + "','" + PriceEditable + "','1','" + terminalID + "') ;", connection);
                        INSERT_cmd_Product_Detail.ExecuteNonQuery();
                        connection.Close();
                        connection.Open();

                        foreach (string image in imagePath)
                        {
                            {
                                

                                string command = "insert into m_product_images(ad_client_id,ad_org_id,createdby,updatedby,image_url,m_product_id) VALUES(" + _clientId + " ," + _OrgId + " ," + _UserID + "," + _UserID + ",'" + image + "'," + ProductSequenc_id + ")";
                                NpgsqlCommand cmd_product_image = new NpgsqlCommand(command, connection);
                                cmd_product_image.ExecuteNonQuery();
                            }

                        }

                        connection.Close();
                        connection.Open();
                        NpgsqlCommand INSERT_cmd_BomProduct_Detail = new NpgsqlCommand("insert into m_product_bom(m_product_bom_id,ad_client_id,ad_org_id,m_product_id,no_of_pcs,m_parent_product_id,is_sync)    " + " " + "" + "" +
                                                                                                      "VALUES(" + Sequenc_id + "," + _clientId + " ," + _OrgId + " ," + _productId + "," + UOMId + "," + _productId + ",'1') ;", connection);
                        INSERT_cmd_BomProduct_Detail.ExecuteNonQuery();

                        connection.Close();
                        connection.Open();
                        NpgsqlCommand cmd_update_sequenc_no_ad_user_pos = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name='m_product_bom';", connection);
                        cmd_update_sequenc_no_ad_user_pos.ExecuteReader();
                        connection.Close();
                        ClearData();

                        MessageBox.Show("Record Save Successfully");
                        NavigationService.Navigate(new AddProduct());
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
        private async void Btnsave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnsave.IsEnabled = false;
                if(txtBarCode.Text == String.Empty)
                {
                    txtBarCode.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
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
                if (imagePath.Count==0)
                {
                    btnupload.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Add the image ! ");
                    return;
                }

                await Task.Run(() =>
                {
                    ProductAPICall();
                }

                 );

            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Insert Product Category Detail  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Insert Product Category Detail", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }
        }
        
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddProduct());
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
                

                product_image.Source = new BitmapImage(fileUri);

                
            
             
                        // Convert byte[] to Base64 String
          //   string base64String = Convert.ToBase64String(directoryPath);
                        
               
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

        #region validation
        private void TxtBarCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBarCode.Text = Regex.Replace(txtBarCode.Text, "[^0-9]+", "");
        }

        private void TxtPurchasePrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPurchasePrice.Text = Regex.Replace(txtPurchasePrice.Text, "[^0-9]+", "");
        }

        private void TxtMinimum_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtMinimum.Text = Regex.Replace(txtMinimum.Text, "[^0-9]+", "");
            
        }

        private void TxtMaximum_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtMaximum.Text = Regex.Replace(txtMaximum.Text, "[^0-9]+", "");
            
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
           

                if (e.Key.ToString().ToUpper() == "F4")
                {
                    Btnsave_Click(sender, e);
                }
                if (e.Key.ToString().ToUpper() == "ESCAPE")
                {
                    BtnCancel_Click(sender, e);
               


            }
        }

        private void AddPro_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewProduct());
        }

        private void TxtSalePrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtSalePrice.Text = Regex.Replace(txtSalePrice.Text, "[^0-9]+", "");
            
        }
        #endregion
    }
}
