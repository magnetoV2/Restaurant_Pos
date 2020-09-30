using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace Restaurant_Pos
{
    public class PostgreSQL
    {
        /// <summary>
        /// Default Base Values
        /// 
        /// 
       // public static string connstring = PostgreSQL.ConnectionString;
        /// </summary>
        private static string  Name = ConfigurationManager.AppSettings.Get("ServerName");
        private static string  Port = ConfigurationManager.AppSettings.Get("ServerPort");
        private static string  UserID = ConfigurationManager.AppSettings.Get("ServerUserID");
        private static string  Password = ConfigurationManager.AppSettings.Get("ServerPassword");
        private static string Database = ConfigurationManager.AppSettings.Get("Database");
        public static string ServerName = Name, ServerPort = Port, ServerUserID = UserID, ServerPassword = Password, Databasename = Database,
        //   ConnectionString = DBConnection(PostgreSQL.ServerName, PostgreSQL.ServerPort, PostgreSQL.ServerUserID, PostgreSQL.ServerPassword, PostgreSQL.Databasename),
         ConnectionString = $"Server={ServerName};Port={ServerPort};User Id={ServerUserID};Password={ServerPassword};Database={Databasename};";
       

        /// <summary>
        /// For All  API Calls in Application
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string ApiCallPost(string json)
        {
            var api_url = Get_App_setting();
            //string _api_url = $"http://{api_url.servername}:{api_url.serverport}{api_url.api_url}";
            string _api_url = $"{api_url.api_url}";
            WebRequest request = WebRequest.Create(_api_url);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            request.Timeout = 200000; 
            // Create POST data and convert it to a byte array.
            //string postData = "This is a test that posts this string to a Web server.";
            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/json";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();
            try
            {
                if (ConnectionString != null)
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
                    {
                        connection.Open();
                        dynamic ProductApiStringResponce = JObject.Parse(responseFromServer);
                        string command = "insert into audit values('" + json + "','" + ProductApiStringResponce.responseCode + "')";
                        NpgsqlCommand INSERT_c_invoicepaymentdetails = new NpgsqlCommand(command, connection);
                        INSERT_c_invoicepaymentdetails.ExecuteNonQuery();
                       
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }

            return responseFromServer;
        }

        public static dynamic DBConnection()
        {
            //string ConnectionString = "Server=localhost; Port=5432; User Id=adempieres; Password=adempiere; Database=pos;";
            //string ConnectionString = "Server=localhost; Port=5432; User Id=adempieres; Password=adempiere; Database=postgres;";

            try
            {
                NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Success open postgreSQL connection.");
                    return "200";
                }
            }
            catch (NpgsqlException ex)
            {
                //dynamic message = JsonConvert.DeserializeObject(ex.Data.ToString());

                return ex.Data.ToString();
            }
            return "false";
        }

        //public static string DBConnection(string ServerName, string ServerPort, string ServerUserID, string ServerPassword, string Databasename)
        //{
        //    return $"Server={ServerName};Port={ServerPort};User Id={ServerUserID};Password={ServerPassword};Database={Databasename};";
        //}

        public static (string servername, string serverport, string api_url, string server_local_name, string server_local_port, string server_local_userid, string server_local_password, string server_local_dbname, string display_language, string on_printer) Get_App_setting()
        {
            string _servername
                    , _serverport
                    , _api_url
                    , _server_local_name
                    , _server_local_port
                    , _server_local_userid
                    , _server_local_password
                    , _server_local_dbname
                    , _display_language
                    , _on_printer;
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
                connection.Open();
                NpgsqlCommand cmd_retail_app_setting_GetData = new NpgsqlCommand("SELECT " +
                    "id, servername, serverport, api_url, server_local_name, server_local_port,server_local_userid, " +
                    "server_local_password, server_local_dbname, display_language, on_printer  FROM ad_setting;", connection);
                NpgsqlDataReader _retail_app_setting_GetData = cmd_retail_app_setting_GetData.ExecuteReader();
                _retail_app_setting_GetData.Read();
                _servername = _retail_app_setting_GetData.GetString(1);
                _serverport = _retail_app_setting_GetData.GetString(2);
                _api_url = _retail_app_setting_GetData.GetString(3);
                _server_local_name = _retail_app_setting_GetData.GetString(4);
                _server_local_port = _retail_app_setting_GetData.GetString(5);
                _server_local_userid = _retail_app_setting_GetData.GetString(6);
                _server_local_password = _retail_app_setting_GetData.GetString(7);
                _server_local_dbname = _retail_app_setting_GetData.GetString(8);
                _display_language = _retail_app_setting_GetData.GetString(9);
                _on_printer = _retail_app_setting_GetData.GetString(10);
                connection.Close();
            }
            catch (Exception)
            {
                throw;
            }

            

            //string DBConnection = "Server=" + _server_local_name + ";" +
            //    "Port=" + _server_local_port + ";" +
            //    "User Id=" + _server_local_userid + ";" +
            //    "Password=" + _server_local_password + ";" +
            //    "Database=" + _server_local_dbname + ";";
            //return (
            //servername,//Item1
            //serverport,//Item2
            //api_url,//Item3
            //server_local_name,//Item4
            //server_local_port,//Item5
            //server_local_userid,//Item6
            //server_local_password,//Item7
            //server_local_dbname,//Item8
            //display_language,//Item9
            //on_printer,//Item10
            //DBConnection//Item11
            //);

            //return (
            //servername,//Item1
            //serverport,//Item2
            //api_url,//Item3
            //display_language,//Item4
            //on_printer,//Item5
            //DBConnection//Item6
            //);

            return (
                servername: _servername,
                serverport: _serverport,
                api_url: _api_url,
                server_local_name: _server_local_name,
                server_local_port: _server_local_port,
                server_local_userid: _server_local_userid,
                server_local_password: _server_local_password,
                server_local_dbname: _server_local_dbname,
                display_language: _display_language,
                on_printer: _on_printer
                );
        }
    }
}