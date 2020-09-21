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

namespace Restaurant_Pos.Pages
{
    /// <summary>
    /// Interaction logic for ViewTerminals.xaml
    /// </summary>
    public partial class ViewTerminals : Page
    {
        #region 
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        int _Bpartner_Id = Int32.Parse(Application.Current.Properties["BpartnerId"].ToString());
        int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<m_TerminalsList> _m_TerminalsList = new List<m_TerminalsList>();
        public List<m_TerminalsWise> _m_TerminalsWise = new List<m_TerminalsWise>();

        public string connstring = PostgreSQL.ConnectionString;
        private dynamic TerminalsApiStringResponce,
        terminalsApiJOSNResponce;
        public dynamic TerminalsApiJOSNResponce { get => terminalsApiJOSNResponce; set => terminalsApiJOSNResponce = value; }
        private string jsonq;
        private int CheckServerError = 0;
     
        #endregion

        #region Public Function
        public ViewTerminals()
        {
            InitializeComponent();
            BindTerminalsList();
            Enabled();


        }
        public void Enabled()
        {
            txtDescription.IsEnabled = false;
            txtName.IsEnabled = false;
            btn_Cancel.Visibility = Visibility.Hidden;
            btn_Save.Visibility = Visibility.Hidden;
            txtIPAddress.IsEnabled = false;


            btn_edit.IsEnabled = false;
        }
        public void ClearData()
        {
            txtName.Text = "";
            txtDescription.Text = "";
        }
        #endregion

        #region Bind Data Function
        public void BindTerminalsList()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_TerminalsList_GetData = new NpgsqlCommand("select m_terminal_id,name from  m_terminal where ad_org_id=" + _OrgId + " ;", connection);//
                NpgsqlDataReader _TerminalsList_GetData = cmd_TerminalsList_GetData.ExecuteReader();
                _m_TerminalsList.Clear();
                
                    _m_TerminalsList.Add(new m_TerminalsList()
                    {

                        id = 0,
                        Name = "All"


                    });
                    while (_TerminalsList_GetData.Read())
                    {

                        _m_TerminalsList.Add(new m_TerminalsList()
                        {

                            id = Int32.Parse(_TerminalsList_GetData["m_terminal_id"].ToString()),
                            Name = _TerminalsList_GetData["name"].ToString()


                        });
                    }

                    connection.Close();
                if (_m_TerminalsList.Count == 0)
                {

                    MessageBox.Show("Please Add Terminals!");

                    return;

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Terminals_DropDown.ItemsSource = _m_TerminalsList;

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
                        "Error In Terminals Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void BindTerminalsLst()
        {

            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_Terminals_GetData = new NpgsqlCommand("select m_terminal_id,name from  m_terminal where ad_org_id=" + _OrgId + "  ;", connection);//
                NpgsqlDataAdapter daTerminals = new NpgsqlDataAdapter(cmd_Terminals_GetData);
                DataTable dtTer = new DataTable();
                daTerminals.Fill(dtTer);
                connection.Close();
                Terminals_ListBox.ItemsSource = "";
                if (dtTer.Rows.Count > 0)
                {

                    Terminals_ListBox.ItemsSource = dtTer.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add Product Terminals!");
                    return;
                }
              
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Terminals List  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Terminals List Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }




        }
        public void BindTerminalsWise(int id)
        {
         try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_TerminalsWise_GetData = new NpgsqlCommand("select m_terminal_id,name from  m_terminal where ad_org_id=" + _OrgId + "  And m_terminal_id='" + id + "' ;", connection);//
                NpgsqlDataAdapter daTerminalsWise = new NpgsqlDataAdapter(cmd_TerminalsWise_GetData);
                DataTable dtTerminalsWise = new DataTable();
                daTerminalsWise.Fill(dtTerminalsWise);
                connection.Close();
                Terminals_ListBox.ItemsSource = "";
                if (dtTerminalsWise.Rows.Count > 0)
                {

                    Terminals_ListBox.ItemsSource = dtTerminalsWise.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add Terminals!");
                    return;
                }

            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error Terminals Wise List  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Terminals Wise List  Not Bind", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }
        }
        public void BindTerminals(string Search_Key)
        {
            try
            {

                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_Tr_GetData = new NpgsqlCommand("select m_terminal_id,name from  m_terminal where ad_org_id=" + _OrgId + "  And upper(name) like '" + Search_Key.ToUpper() + "%' ;", connection);//
                NpgsqlDataAdapter daTr = new NpgsqlDataAdapter(cmd_Tr_GetData);
                DataTable dtTr = new DataTable();
                daTr.Fill(dtTr);
                connection.Close();
                Terminals_ListBox.ItemsSource = "";
                if (dtTr.Rows.Count > 0)
                {

                    Terminals_ListBox.ItemsSource = dtTr.AsDataView();
                }
                else
                {
                    MessageBox.Show("Please Add Terminals!");
                    return;
                }
             
        }
            
            catch (Exception ex)
            {

                log.Error(" ===================  Error Terminals Is Not Found  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Terminals Is Not Found!", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }
        }

        #endregion

        #region Event
        private void Terminals_Search_KeyUp(object sender, KeyEventArgs e)
        {
            if(Terminals_Search.Text!=null )
            {
                string Search_Key = Terminals_Search.Text;
                BindTerminals(Search_Key);
            }
           else
            {
                BindTerminalsLst();
            }
        }

        private void Terminals_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          
            dynamic TList = Terminals_ListBox.SelectedItem as dynamic;
            if(TList==null)
            { 
             }
            else
            {
                dynamic id = TList[0];

                btn_edit.IsEnabled = true;
                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();
                NpgsqlCommand cmd_T_GetData = new NpgsqlCommand("select name,description,IPAddress from  m_terminal where ad_org_id=" + _OrgId + "  And m_terminal_id='" + id + "'  ;", connection);//
                NpgsqlDataAdapter daT = new NpgsqlDataAdapter(cmd_T_GetData);
                DataTable dtT = new DataTable();
                daT.Fill(dtT);
                connection.Close();

                if (dtT.Rows.Count > 0)
                {
                    txtName.Text = dtT.Rows[0]["name"].ToString();
                    txtDescription.Text = dtT.Rows[0]["description"].ToString();
                    txtIPAddress.Text = dtT.Rows[0]["IPAddress"].ToString();
                    txtTerminalsID.Text = id.ToString();

                }
                else
                {
                    MessageBox.Show("Please Add Terminals!");
                    return;
                }
            }
           

          
        }

        private void Terminals_DropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic PC = Terminals_DropDown.SelectedItem as dynamic;
            int id = PC.id;
            if (Terminals_DropDown.SelectedIndex <= 0)
            {
                BindTerminalsLst();
            }
            else
            {
                BindTerminalsWise(id);
            }
        }

        private void Btn_edit_Click(object sender, RoutedEventArgs e)
        {
            txtDescription.IsEnabled = true;
            txtName.IsEnabled = true;
            btn_Cancel.Visibility = Visibility.Visible;
            btn_Save.Visibility = Visibility.Visible;
            txtIPAddress.IsEnabled = true;

        }

        private void Btn_Add_Terminals_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddTerminals());
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewTerminals());
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
                if (txtIPAddress.Text == String.Empty)
                {
                    txtIPAddress.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(CoustomColors.CartRedBrush);
                    MessageBox.Show("Please Enter The IPAddress ! ");
                    return;
                }
                await Task.Run(() =>
                {
                    EditTerminalsAPICall();
                }

              );
             
             
            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Edit Terminals  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Edit Terminals", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
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

        public void EditTerminalsAPICall()
        {

            this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Please Wait .. "; });
            this.Dispatcher.Invoke(() =>
            {
                POSAddTerminalsApiModel AddTerminalsDetail = new POSAddTerminalsApiModel
                {
                    macAddress = "8e17517b8cd41f3c",
                    clientId = _clientId,
                    orgId = _OrgId,
                    warehouseId = _Warehouse_Id,
                    userId = _UserID,
                    version = "2.0",
                    appName = "POS",
                    operation = "POSAddEditTerminal",
                    terminalId = 0,
                    termnalName = txtName.Text,
                    description = txtDescription.Text,
                    isPriner = "Y",
                    ipaddress = txtIPAddress.Text,

                };

                jsonq = JsonConvert.SerializeObject(AddTerminalsDetail);
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: Passing Data to Api Call"; });
            });

            try
            {
                TerminalsApiStringResponce = PostgreSQL.ApiCallPost(jsonq);
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
                    NpgsqlCommand cmd_update_Terminals = new NpgsqlCommand("update m_terminal set  name='" + txtName.Text + "', Description='" + txtDescription.Text + "',IPAddress='" + txtIPAddress.Text + "',is_sync='0',updatedby=" + _UserID + " where m_terminal_id='" + txtTerminalsID.Text + "' and ad_org_id='" + _OrgId + "';", connection);
                    cmd_update_Terminals.ExecuteReader();
                    ClearData();
                    connection.Close();

                    MessageBox.Show("Record Edit Successfully !");
                    NavigationService.Navigate(new ViewTerminals());
                });
                //CheckServerError = 0;
                //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Server Error! Contact Admin"; });
                //log.Error("Terminals Error|POSOrganization API Not Responding");
                //return;
            }
            if (CheckServerError == 1)
            {
                TerminalsApiJOSNResponce = JsonConvert.DeserializeObject(TerminalsApiStringResponce);

                if (TerminalsApiJOSNResponce.responseCode == "200")
                {
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: loginApiResponce Responce Code 200"; });

                    int _Terminals = TerminalsApiJOSNResponce.terminalId;

                    NpgsqlConnection connection = new NpgsqlConnection(connstring);
                    this.Dispatcher.Invoke(() =>
                    {

                        connection.Open();
                        NpgsqlCommand cmd_update_Terminals = new NpgsqlCommand("update m_terminal set  name='" + txtName.Text + "', Description='" + txtDescription.Text + "',IPAddress='" + txtIPAddress.Text + "',is_sync='1',m_terminal_id='"+_Terminals+ "' ,updatedby=" + _UserID + " where m_terminal_id='" + txtTerminalsID.Text + "' and ad_org_id='" + _OrgId + "';", connection);
                        cmd_update_Terminals.ExecuteReader();
                        ClearData();
                        connection.Close();

                        MessageBox.Show("Record Edit Successfully !");
                        NavigationService.Navigate(new ViewTerminals());
                    });

                }
                else if (TerminalsApiJOSNResponce.responseCode == "106")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        LoginProcessingText.Text = "Terminals not Edit  !";

                    });
                    return;
                }
            }
        }
        #endregion
    }
}
