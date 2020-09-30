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

namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for ViewTable.xaml
    /// </summary>
    public partial class ViewTable : Page
    {
        #region 

        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        int _Bpartner_Id = Int32.Parse(Application.Current.Properties["BpartnerId"].ToString());
        int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());
        DataRowView TL ;
        dynamic id = 0;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        public List<m_TableList> _m_TableList = new List<m_TableList>();
        public List<m_TableWise> _m_TableWise = new List<m_TableWise>();
        public List<M_Tables> m_Tables_items = new List<M_Tables>();
      
        private dynamic TableApiStringResponce,
         tableApiJOSNResponce;
        private string jsonq;
        private int CheckServerError = 0;
        public dynamic TableApiJOSNResponce { get => tableApiJOSNResponce; set => tableApiJOSNResponce = value; }
        

        public string connstring = PostgreSQL.ConnectionString;
        #endregion
        public ViewTable()
        {
            InitializeComponent();
            BindTableLst();
            BindTableGroup();
            Enabled();
          

        }
        public void Enabled()
        {
            btn_edit.IsEnabled = false;
            txtName.IsEnabled = false;
            txtDescription.IsEnabled = false;
            CbCoversLevel.IsEnabled = false;
            TableGroup_DropDown.IsEnabled = false;
            btn_Cancel.Visibility = Visibility.Hidden;
            btn_Save.Visibility = Visibility.Hidden;
         
        }
        #region Bind Data Function
        public void BindTableLst()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_TableLst_GetData = new NpgsqlCommand("select m_table_id,name from  m_table  where ad_org_id=" + _OrgId + "order by name ;", connection);//
                NpgsqlDataReader _TableLst_GetData = cmd_TableLst_GetData.ExecuteReader();
                _m_TableWise.Clear();
                Table_DropDown.ItemsSource = "";
                _m_TableWise.Add(new m_TableWise()
                {
                    id = 0,
                    Name = "All"


                });
                while (_TableLst_GetData.Read())
                {

                    _m_TableWise.Add(new m_TableWise()
                    {
                        id = Int32.Parse(_TableLst_GetData["m_table_id"].ToString()),
                        Name = _TableLst_GetData["name"].ToString()


                    });
                }

                connection.Close();

                if (_m_TableWise.Count == 0)
                {
                    MessageBox.Show("Please Add Table!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Table_DropDown.ItemsSource = _m_TableWise;

                    });
                }


            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Table List  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Table List Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
      
        public void BindTableList()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_TableList_GetData = new NpgsqlCommand("select m_table_id,name from  m_table  where ad_org_id=" + _OrgId + "order by name ;", connection);//
                NpgsqlDataAdapter daTableList = new NpgsqlDataAdapter(cmd_TableList_GetData);
                DataTable dtTableList = new DataTable();
                daTableList.Fill(dtTableList);
                connection.Close();
                Table_ListBox.ItemsSource = "";
                if (dtTableList.Rows.Count > 0)
                {

                    Table_ListBox.ItemsSource = dtTableList.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add Table List !");
                    return;
                }
              
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Table List  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Table List Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void BindTableWise(int id)
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_Table_GetData = new NpgsqlCommand("select m_table_id,name from  m_table  where ad_org_id=" + _OrgId + " and m_table_id='"+ id + "' ;", connection);//
                NpgsqlDataAdapter daTable = new NpgsqlDataAdapter(cmd_Table_GetData);
                DataTable dtTable = new DataTable();
                daTable.Fill(dtTable);
                connection.Close();
                Table_ListBox.ItemsSource = "";
                if (dtTable.Rows.Count > 0)
                {

                    Table_ListBox.ItemsSource = dtTable.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add Table List !");
                    return;
                }

                


            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Table List  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Table List Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void BindTableGroup()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                // connection.Close();
                connection.Open();
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                NpgsqlCommand cmd_Tables_GetData = new NpgsqlCommand("SELECT m_table_id,name FROM  m_table  WHERE  ad_client_id = " + _clientId + ";  ", connection);//
                NpgsqlDataReader _Tables_GetData = cmd_Tables_GetData.ExecuteReader();
                m_Tables_items.Add(new M_Tables()
                {
                    ID = 0,
                    Name = "---select----"


                });
                while (_Tables_GetData.Read())
                {

                    m_Tables_items.Add(new M_Tables()
                    {
                        ID =Int32.Parse(_Tables_GetData["m_table_id"].ToString()),
                        Name = _Tables_GetData["name"].ToString()


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


        public void BindTableSearch(string Search_Key)
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_TableSearch_GetData = new NpgsqlCommand("select m_table_id,name from  m_table  where ad_org_id=" + _OrgId + " and upper(name) like '" + Search_Key.ToUpper() + "%' ;", connection);//
                NpgsqlDataAdapter daTableSearch = new NpgsqlDataAdapter(cmd_TableSearch_GetData);
                DataTable dtTableSearch = new DataTable();
                daTableSearch.Fill(dtTableSearch);
                connection.Close();
                Table_ListBox.ItemsSource = "";
                if (dtTableSearch.Rows.Count > 0)
                {

                    Table_ListBox.ItemsSource = dtTableSearch.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add Table List !");
                    return;
                }
            


            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Table List  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Table List Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
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
            if(product_Search.Text!=null)
            {
                BindTableSearch(product_Search.Text);
            }
           else
            {
                BindTableList();
            }
        }

        private void Table_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TL = Table_ListBox.SelectedItem as DataRowView;
            if (TL==null)
            {
                
            }
            else
            {

          
          
             id = TL[0];
            dynamic Name = TL[1];
            btn_edit.IsEnabled = true;

            NpgsqlConnection connection = new NpgsqlConnection(connstring);
            // connection.Close();
            connection.Open();
            //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
            NpgsqlCommand cmd_Tbl_GetData = new NpgsqlCommand("select name,description,m_table_group_id,iscoverslevel,m_table_id from  m_table  where m_table_id=" + id + " and ad_org_id=" + _OrgId + "  ;", connection);//
            NpgsqlDataAdapter daTbl = new NpgsqlDataAdapter(cmd_Tbl_GetData);
            DataTable dtTbl = new DataTable();
            daTbl.Fill(dtTbl);
            connection.Close();
            
            if (dtTbl.Rows.Count > 0)
            {

                txtName.Text = dtTbl.Rows[0]["name"].ToString();
                txtDescription.Text = dtTbl.Rows[0]["description"].ToString();
                int Table_SelectedItemIndex = m_Tables_items.FindIndex(i => i.ID == Int32.Parse(dtTbl.Rows[0]["m_table_group_id"].ToString()));
                TableGroup_DropDown.SelectedIndex = Table_SelectedItemIndex;

               
                    if (dtTbl.Rows[0]["iscoverslevel"].ToString() == "Y")
                    {
                        CbCoversLevel.IsChecked = true;
                        TableGroup_DropDown_br.Visibility = Visibility.Visible;
                        lblgroup.Visibility = Visibility.Visible;

                    }
                    else
                    {
                        CbCoversLevel.IsChecked = false;
                        TableGroup_DropDown_br.Visibility = Visibility.Hidden;
                        lblgroup.Visibility = Visibility.Hidden;
                    }
                
                txttableid.Text = dtTbl.Rows[0]["m_table_id"].ToString();
            }
            else
            {
                MessageBox.Show("Please Add Product Category!");
                return;
            }
            }
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewTable());
        }

        private async void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
                    EditTableApiCall();
                }

              );
               
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

        public void EditTableApiCall()
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
                    tableId = 0,
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
                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                this.Dispatcher.Invoke(() =>
                {
                    dynamic TG = TableGroup_DropDown.SelectedItem as dynamic;
                    int ID = TG.ID;
                   
                    string CoversLevel = "";

                    if (CbCoversLevel.IsChecked == true)
                    {
                        CoversLevel = "Y";
                    }
                    else
                    {
                        CoversLevel = "N";
                    }
                    
                    connection.Open();
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                    NpgsqlCommand Edit_Table_List = new NpgsqlCommand("Update m_table set Name='" + txtName.Text + "', description='" + txtDescription.Text + "', iscoverslevel='" + CoversLevel + "',updatedby=" + _UserID + " ,m_table_group_id='" + ID + "',Updated='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "',is_sync='0' where m_table_id='" + txttableid.Text + "' and ad_org_id=" + _OrgId + ";", connection);//
                    Edit_Table_List.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("Record Edit Successfully");

                    NavigationService.Navigate(new ViewTable());
                });
                //CheckServerError = 0;
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                //log.Error("EditCategory Error|POSOrganization API Not Responding");
                //return;
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
                        dynamic TG = TableGroup_DropDown.SelectedItem as dynamic;
                        int ID = TG.ID;
                       
                        string CoversLevel = "";

                        if (CbCoversLevel.IsChecked == true)
                        {
                            CoversLevel = "Y";
                        }
                        else
                        {
                            CoversLevel = "N";
                        }
                       
                        connection.Open();
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Getting All Warehouse Present"; });
                        NpgsqlCommand Edit_Table_List = new NpgsqlCommand("Update m_table set Name='" + txtName.Text + "', description='" + txtDescription.Text + "', iscoverslevel='" + CoversLevel + "',updatedby=" + _UserID + " ,m_table_group_id='" + ID + "',Updated='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "',is_sync='1',m_table_id='"+_tableId+ "'  where m_table_id='" + txttableid.Text + "' and ad_org_id=" + _OrgId + ";", connection);//
                        Edit_Table_List.ExecuteNonQuery();
                        connection.Close();
                        MessageBox.Show("Record Edit Successfully");

                        NavigationService.Navigate(new ViewTable());
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

        private void Btn_edit_Click(object sender, RoutedEventArgs e)
        {
            txtName.IsEnabled = true;
            txtDescription.IsEnabled = true;
            CbCoversLevel.IsEnabled = true;
            TableGroup_DropDown.IsEnabled = true;
            btn_Cancel.Visibility = Visibility.Visible;
            btn_Save.Visibility = Visibility.Visible;
            
        }

        private void Btn_Add_Category_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddTable());
        }

    

        private void Grdtable_KeyUp(object sender, KeyEventArgs e)
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

        private void CbCoversLevel_Checked(object sender, RoutedEventArgs e)
        {
            TableGroup_DropDown_br.Visibility = Visibility.Visible;
            lblgroup.Visibility = Visibility.Visible;
        }

        private void CbCoversLevel_Unchecked(object sender, RoutedEventArgs e)
        {
            TableGroup_DropDown_br.Visibility = Visibility.Hidden;
            lblgroup.Visibility = Visibility.Hidden;
            TableGroup_DropDown.SelectedIndex = 0;
        }

        private void Table_DropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic PC = Table_DropDown.SelectedItem as dynamic;
            int id = PC.id;
            if (Table_DropDown.SelectedIndex <= 0)
            {
                BindTableList();
            }
            else
            {
                BindTableWise(id);
            }
        }
        #endregion
    }
}
