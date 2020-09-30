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
    /// Interaction logic for AddTable.xaml
    /// </summary>
    public partial class AddTable : Page
    {
        #region 
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

       
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
        public List<M_Tables> m_Tables_items = new List<M_Tables>();
   
        

        private dynamic TableApiStringResponce,
        tableApiJOSNResponce;
        private string jsonq;
        private int CheckServerError = 0;
        public dynamic TableApiJOSNResponce { get => tableApiJOSNResponce; set => tableApiJOSNResponce = value; }

        #endregion

        public AddTable()
        {
            InitializeComponent();
            BindTableGroup();
           
         
        }

        #region Bind data
        public void BindTableGroup()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_Tables_GetData = new NpgsqlCommand("SELECT m_table_id,Name FROM  m_table  WHERE  ad_client_id = " + _clientId + "; ", connection);//
                NpgsqlDataReader _Tables_GetData = cmd_Tables_GetData.ExecuteReader();
                m_Tables_items.Add(new M_Tables()
                {
                    ID = 0,
                    Name ="---select----"


                });
                while (_Tables_GetData.Read())
                {

                    m_Tables_items.Add(new M_Tables()
                    {
                        ID = _Tables_GetData.GetInt32(0),
                        Name = _Tables_GetData.GetString(1)


                    });
                }
                
                connection.Close();

                if (m_Tables_items.Count == 0)
                {
                    MessageBox.Show("Please Add Table Group!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {

                        TableGroup_DropDown.ItemsSource = m_Tables_items;
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
                        "Error In Table Group Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }

        
        #endregion

        #region Event
        private async  void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnSave.IsEnabled = true;
                if (txtName.Text == String.Empty)
                {
                    txtName.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Name ! ");
                    return;
                }
                if (txtDescription.Text == String.Empty)
                {
                    txtDescription.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The Description ! ");
                    return;
                }

                await Task.Run(() =>
                {
                    TableApiCall();
                }

                 );



            }

            catch (Exception ex)
            {

                log.Error(" ===================  Error In Add Table  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Add Table ", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }

        }
        public void TableApiCall()
        {
          
            this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Please Wait .. "; });
                this.Dispatcher.Invoke(() =>
                {
                    dynamic TG = TableGroup_DropDown.SelectedItem as dynamic;
                    int ID = TG.ID;
                    POSTableApiModel _Table = new POSTableApiModel
                    {
                        macAddress = "8e17517b8cd41f3c",
                        clientId = _clientId,
                        orgId = _OrgId,
                        warehouseId = _Warehouse_Id,
                        userId = _UserID,
                        version = "2.0",
                        appName = "POS",
                        operation = "POSAddEditTable",
                        tableId =  0,
                        tableName = txtName.Text,
                        description = txtDescription.Text,
                        coverslevel = txtName.Text,
                        tablegroupId = ID
                    };

                    jsonq = JsonConvert.SerializeObject(_Table);
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Passing Data to Api Call"; });
                });

                try
                {
                TableApiStringResponce = PostgreSQL.ApiCallPost(jsonq);
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Checking Database Connection"; });
                    CheckServerError = 1;
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Database Connected"; });
                }
                catch
                {
                //CheckServerError = 0;
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                //log.Error("EditCategory Error|POSOrganization API Not Responding");
                //return;
                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                this.Dispatcher.Invoke(() =>
                {
                    string CoversLevel = "";

                    if (CbCoversLevel.IsChecked == true)
                    {
                        CoversLevel = "Y";
                    }
                    else
                    {
                        CoversLevel = "N";
                    }
                    dynamic TG = TableGroup_DropDown.SelectedItem as dynamic;
                    int ID = TG.ID;
                  
                    // connection.Close();
                    connection.Open();
                    NpgsqlCommand cmd_select_sequenc_no_ad_user_pos = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name='m_table';", connection);
                    NpgsqlDataReader _get__Ad_sequenc_no_ad_user_pos = cmd_select_sequenc_no_ad_user_pos.ExecuteReader();
                    if (_get__Ad_sequenc_no_ad_user_pos.Read())
                    {
                        Sequenc_id = _get__Ad_sequenc_no_ad_user_pos.GetInt32(4) + 1;
                    }
                    connection.Close();
                    connection.Open();
                    NpgsqlCommand Add_Table_List = new NpgsqlCommand("insert into m_table(ad_client_id, ad_org_id,m_table_id,created, createdby, iscoverslevel,m_table_group_id,Name,description,is_sync) " +
                        "   " + " " + "" + "VALUES('" + _clientId + "','" + _OrgId + "'," + Sequenc_id + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "','" + _UserID + "','" + CoversLevel + "','" + ID + "','" + txtName.Text + "','" + txtDescription.Text + "','0') ;", connection);
                    Add_Table_List.ExecuteNonQuery();
                    connection.Close();
                    connection.Open();
                    NpgsqlCommand cmd_update_sequenc_no_ad_user_pos = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name='m_table';", connection);
                    cmd_update_sequenc_no_ad_user_pos.ExecuteReader();
                    connection.Close();
                    MessageBox.Show("Record Save Successfully");
                    NavigationService.Navigate(new AddTable());
                });
            }
                if (CheckServerError == 1)
                {
                TableApiJOSNResponce = JsonConvert.DeserializeObject(TableApiStringResponce);

                    if (TableApiJOSNResponce.responseCode == "200")
                    {
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: loginApiResponce Responce Code 200"; });

                    int _tableId = TableApiJOSNResponce.tableId;
                       
                    NpgsqlConnection connection = new NpgsqlConnection(connstring);
                        this.Dispatcher.Invoke(() =>
                        {
                            string CoversLevel = "";
                            
                            if (CbCoversLevel.IsChecked == true)
                            {
                                CoversLevel = "Y";
                            }
                            else
                            {
                                CoversLevel = "N";
                            }
                            dynamic TG = TableGroup_DropDown.SelectedItem as dynamic;
                            int ID = TG.ID;
                       
                            // connection.Close();
                            connection.Open();
                            NpgsqlCommand Add_Table_List = new NpgsqlCommand("insert into m_table(ad_client_id, ad_org_id,m_table_id,created, createdby, iscoverslevel,m_table_group_id," +
                                "Name,description,is_sync)    " + " " + "" + "VALUES('" + _clientId + "','" + _OrgId + "','" + _tableId + "','"+ DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "','" + _UserID + "','" + CoversLevel + "','" + ID + "','" + txtName.Text + "','"+txtDescription.Text+"','1') ;", connection);
                            Add_Table_List.ExecuteNonQuery();
                            connection.Close();
                            MessageBox.Show("Record Save Successfully");
                            NavigationService.Navigate(new AddTable());
                        });

                    }
                    else if (TableApiJOSNResponce.responseCode == "106")
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            LoginProcessingText.Text = "table not Created  !";

                        });
                        return;
                    }
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

        private void CbCoversLevel_Checked(object sender, RoutedEventArgs e)
        {
            if(CbCoversLevel.IsChecked==true)
            {
               
                TableGroup_DropDown_br.Visibility = Visibility.Visible;
                lblgroup.Visibility = Visibility.Visible;
            }
          
           
        }

        private void CbCoversLevel_Unchecked(object sender, RoutedEventArgs e)
        {
            TableGroup_DropDown_br.Visibility = Visibility.Hidden;
            lblgroup.Visibility = Visibility.Hidden;
            TableGroup_DropDown.SelectedIndex = 0;
        }

        private void Addtable_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewTable());
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddTable());
        }

        #endregion
    }
}
