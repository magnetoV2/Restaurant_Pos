using log4net;
using Microsoft.Win32;
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
using System.Windows.Media.Imaging;

namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for AddProductCAT.xaml
    /// </summary>
    public partial class AddProductCAT : Page
    {

        #region 
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

   
      

        private string jsonq;
        private int CheckServerError = 0;
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        int _Bpartner_Id = Int32.Parse(Application.Current.Properties["BpartnerId"].ToString());
        int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());

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

        string imagePath = "";
        private dynamic AddCategoryApiStringResponce,
        addcategoryApiJOSNResponce;
        public dynamic AddCategoryApiJOSNResponce { get => addcategoryApiJOSNResponce; set => addcategoryApiJOSNResponce = value; }

        #endregion

        #region Public Function
        public AddProductCAT()
        {
            InitializeComponent();
        }
        public void ClearData()
        {
            txtCategory.Text = "";
            txtName.Text = "";
            txtSearchKey.Text = "";
            product_image.Source = null;
        }

        #endregion

        #region Event


        public void AddCategoryAPICall()
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
                    operation = "POSAddCategory",
                    categoryId = 0,
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
                string DigitalMenu = "";
                string SelfService = "";
                this.Dispatcher.Invoke(() =>
                {
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

                    //1005329
                    connection.Open();
                    NpgsqlCommand cmd_select_sequenc_no_ad_user_pos = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name='m_product_category';", connection);
                    NpgsqlDataReader _get__Ad_sequenc_no_ad_user_pos = cmd_select_sequenc_no_ad_user_pos.ExecuteReader();
                    if (_get__Ad_sequenc_no_ad_user_pos.Read())
                    {
                        Sequenc_id = _get__Ad_sequenc_no_ad_user_pos.GetInt32(4) + 1;
                    }
                    connection.Close();

                    connection.Open();
                    NpgsqlCommand INSERT_cmd_productCAT_Detail = new NpgsqlCommand("insert into m_product_category(id,ad_client_id,ad_org_id,Createdby,isactive, m_product_category_id,Name,searchkey,categoryColour,image,SelfService,DigitalMenu,is_sync)    " + " " + "" + "VALUES(" + Sequenc_id + "," + _clientId + " ," + _OrgId + " ," + _UserID + ",'Y'," + Sequenc_id + ",'" + txtName.Text + "','" + txtSearchKey.Text + "','" + txtCategory.Text + "','" + imagePath + "','" + SelfService + "','" + DigitalMenu + "','0') ;", connection);
                    INSERT_cmd_productCAT_Detail.ExecuteNonQuery();

                    ClearData();
                    connection.Close();
                    connection.Open();
                    NpgsqlCommand cmd_update_sequenc_no_ad_user_pos = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name='m_product_category';", connection);
                    cmd_update_sequenc_no_ad_user_pos.ExecuteReader();
                    connection.Close();
                    MessageBox.Show("Record Save Successfully");
                });
                //CheckServerError = 0;
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                //log.Error("AddCategory Error|POSOrganization API Not Responding");
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
                    string DigitalMenu = "";
                    string SelfService = "";
                    this.Dispatcher.Invoke(() =>
                    {
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

                        //1005329
                        connection.Open();
                        NpgsqlCommand cmd_select_sequenc_no_ad_user_pos = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name='m_product_category';", connection);
                        NpgsqlDataReader _get__Ad_sequenc_no_ad_user_pos = cmd_select_sequenc_no_ad_user_pos.ExecuteReader();
                        if (_get__Ad_sequenc_no_ad_user_pos.Read())
                        {
                            Sequenc_id = _get__Ad_sequenc_no_ad_user_pos.GetInt32(4) + 1;
                        }
                        connection.Close();

                        connection.Open();
                        NpgsqlCommand INSERT_cmd_productCAT_Detail = new NpgsqlCommand("insert into m_product_category(id,ad_client_id,ad_org_id,Createdby,isactive, m_product_category_id,Name,searchkey,categoryColour,image,SelfService,DigitalMenu,is_sync)    " + " " + "" + "VALUES(" + Sequenc_id + "," + _clientId + " ," + _OrgId + " ," + _UserID + ",'Y'," + _categoryId + ",'" + txtName.Text + "','" + txtSearchKey.Text + "','" + txtCategory.Text + "','" + imagePath + "','" + SelfService + "','" + DigitalMenu + "','1') ;", connection);
                        INSERT_cmd_productCAT_Detail.ExecuteNonQuery();

                        ClearData();
                        connection.Close();
                        connection.Open();
                        NpgsqlCommand cmd_update_sequenc_no_ad_user_pos = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name='m_product_category';", connection);
                        cmd_update_sequenc_no_ad_user_pos.ExecuteReader();
                        connection.Close();
                        MessageBox.Show("Record Save Successfully");
                    });

                }
                else if (AddCategoryApiJOSNResponce.responseCode == "106")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        LoginProcessingText.Text = "Product Category not created !";

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
                if (imagePath == "")
                {
                    btnImgUpload.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Add The Image! ");
                    return;
                }
               
                await Task.Run(() =>
                    {
                        AddCategoryAPICall();
                    });
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
            ClearData();
        }

        private void BtnImgUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFileDialog.FileName);
                string fileName = openFileDialog.SafeFileName;

                string destinationPath = "C:/pos/component/Resources/Images/ProductCAT";
                string directoryPath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

                DirectoryInfo diSource = new DirectoryInfo(directoryPath);
                DirectoryInfo diTarget = new DirectoryInfo(@destinationPath);
                Copy(diSource, diTarget, fileName.ToString());

                product_image.Source = new BitmapImage(fileUri);
                imagePath = diTarget.FullName + "/" + fileName;
                //using (System.Drawing.Image image = System.Drawing.Image.FromFile(fileUri.LocalPath.ToString()))
                //{
                //    using (MemoryStream m = new MemoryStream())
                //    {
                //        image.Save(m, image.RawFormat);
                //        byte[] imageBytes = m.ToArray();
                //        imagePath = Convert.ToBase64String(imageBytes);

                //    }
                //}
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

        private void AddProductCat_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewProductCAT());
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
