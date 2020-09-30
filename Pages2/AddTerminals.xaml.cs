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
    /// Interaction logic for AddTerminals.xaml
    /// </summary>
    public partial class AddTerminals : Page
    {
        #region public variable
        int _clientId = Int32.Parse(Application.Current.Properties["clientId"].ToString());
        int _OrgId = Int32.Parse(Application.Current.Properties["OrgId"].ToString());
        int _UserID = Int32.Parse(Application.Current.Properties["UserID"].ToString());
        int _Warehouse_Id = Int32.Parse(Application.Current.Properties["WarehouseId"].ToString());
        public int Sequenc_id { get; set; }
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string connstring = PostgreSQL.ConnectionString;
        private dynamic TerminalsApiStringResponce,
        terminalsApiJOSNResponce;
        public dynamic TerminalsApiJOSNResponce { get => terminalsApiJOSNResponce; set => terminalsApiJOSNResponce = value; }
        private string jsonq;
        private int CheckServerError = 0;
        #endregion
        public AddTerminals()
        {
            InitializeComponent();
        }
        public void ClearData()
        {
            txtName.Text = "";
            txtDescription.Text = "";
        }
        private async void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btn_Save.IsEnabled = true;
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
                    TerminalsAPICall();
                }
              );


            }
            catch (Exception ex)
            {

                log.Error(" ===================  Error In Insert Terminals Detail  =========================== ");
                log.Error(DateTime.Now.ToString());
                log.Error(ex.ToString());
                log.Error(" ===================  End of Error  =========================== ");
                if (MessageBox.Show(ex.ToString(),
                        "Error In Insert Terminals Detail", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
                return;
            }
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            ClearData();
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

        private void Addterminal_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ViewTerminals());
        }

        public void TerminalsAPICall()
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
                    NpgsqlCommand cmd_select_sequenc_no_ad_user_pos = new NpgsqlCommand("SELECT ad_sequence_id,name, incrementno, startno,currentnext FROM ad_sequence where name='m_kitchen';", connection);
                    NpgsqlDataReader _get__Ad_sequenc_no_ad_user_pos = cmd_select_sequenc_no_ad_user_pos.ExecuteReader();
                    if (_get__Ad_sequenc_no_ad_user_pos.Read())
                    {
                        Sequenc_id = _get__Ad_sequenc_no_ad_user_pos.GetInt32(4) + 1;
                    }
                    connection.Close();
                    connection.Open();
                    NpgsqlCommand INSERT_cmd_Terminals_Detail = new NpgsqlCommand("insert into m_terminal(m_terminal_id,ad_client_id,ad_org_id,Createdby,isactive, name, Description,IPAddress,is_sync)    " + " " + "" + "" +
                                                                                                  "VALUES(" + Sequenc_id + "," + _clientId + " ," + _OrgId + " ," + _UserID + ",'Y','" + txtName.Text + "','" + txtDescription.Text + "','" + txtIPAddress.Text + "','0') ;", connection);
                    INSERT_cmd_Terminals_Detail.ExecuteNonQuery();
                    //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: ad_client_id  new data  Inserted"; });
                    ClearData();
                    connection.Close();
                    connection.Open();
                    NpgsqlCommand cmd_update_sequenc_no_ad_user_pos = new NpgsqlCommand("UPDATE ad_sequence SET currentnext =" + Sequenc_id + " WHERE name='m_kitchen';", connection);
                    cmd_update_sequenc_no_ad_user_pos.ExecuteReader();
                    connection.Close();

                    MessageBox.Show("Record Save Successfully !");
                    NavigationService.Navigate(new AddTerminals());
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
                        NpgsqlCommand INSERT_cmd_Terminals_Detail = new NpgsqlCommand("insert into m_terminal(m_terminal_id,ad_client_id,ad_org_id,Createdby,isactive, name, Description,IPAddress,is_sync)    " + " " + "" + "" +
                                                                                                      "VALUES(" + _Terminals + "," + _clientId + " ," + _OrgId + " ," + _UserID + ",'Y','" + txtName.Text + "','" + txtDescription.Text + "','" + txtIPAddress.Text + "','1') ;", connection);
                        INSERT_cmd_Terminals_Detail.ExecuteNonQuery();
                        //this.Dispatcher.Invoke(() => { LoginProcessingText.Text = "Process: ad_client_id  new data  Inserted"; });
                        ClearData();
                        connection.Close();

                        MessageBox.Show("Record Save Successfully !");
                        NavigationService.Navigate(new AddTerminals());
                    });

                }
                else if (TerminalsApiJOSNResponce.responseCode == "106")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        LoginProcessingText.Text = "Terminals not updated  !";

                    });
                    return;
                }
            }
        }

    }
}
