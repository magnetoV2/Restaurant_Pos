using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant_Pos.Pages
{
    public class invoive_No
    {
        #region Global

        public static long AD_Client_ID { get; set; }
        public static long AD_ORG_ID { get; set; }
        public static long AD_USER_ID { get; set; }
        public static long AD_ROLE_ID { get; set; }
        public static string AD_ROLE_Name { get; set; }
        public static long AD_Warehouse_Id { get; set; }
       public static long AD_bpartner_Id { get; set; }
        public static string AD_UserName { get; set; }
        public static string AD_UserPassword { get; set; }
        public static double AD_SessionID { get; set; }
        
        public string connstring = PostgreSQL.ConnectionString;

        int? _InvoiceNo_ = null;
        int? _doc_no_or_error_code = null;
        private readonly string DeviceMacAdd = LoginViewModel.DeviceMacAddress();

        #endregion Global

        public invoive_No()
        {
            NpgsqlConnection connection = new NpgsqlConnection(connstring);
            connection.Open();
            NpgsqlCommand cmd_ad_user_pos_check = new NpgsqlCommand("SELECT " +
                "t1.ad_client_id, " +                                           //0
                "t1.ad_org_id, " +                                              //1
                "t1.ad_role_id, " +                                             //2
                "t1.ad_user_id," +                                              //3
                "t1.c_bpartner_id, " +                                          //4
                "t1.m_warehouse_id," +                                          //5
                "t1.name, " +                                                   //6
                "t1.password," +                                                //7
                "t1.sessionid ," +                                              //8
                "t2.name as ad_org_name, " +                                    //9     ad_org_name
                "t2.arabicname as ad_org_arabicname, " +                        //10    ad_org_arabicname
                "t2.logo as ad_org_logo, " +                                    //11    ad_org_logo
                "t2.phone as ad_org_phone, " +                                  //12    ad_org_phone
                "t2.email as ad_ord_email, " +                                  //13    ad_ord_email
                "t2.address as ad_ord_add, " +                                  //14    ad_ord_add
                "t2.city as ad_org_city, " +                                    //15    ad_org_city
                "t2.country as ad_org_country, " +                              //16    ad_org_country
                "t2.postal as ad_org_postal, " +                                //17    ad_org_postal
                "t2.weburl as ad_org_weburl , " +                               //18    ad_org_weburl
                "t2.footermessage as ad_org_footermessage, " +                  //19    ad_org_footermessage
                "t2.arabicfootermessage as ad_org_arabicfootermessage, " +      //20    ad_org_arabicfootermessage
                "t2.termsmessage as ad_ord_termsmessage, " +                    //21    ad_ord_termsmessage
                "t2.arabictermsmessage as ad_org_arabictermsmessage," +         //22    ad_org_arabictermsmessage
                "t3.name as ad_role_name, " +                                   //23    ad_role_name
                "t4.name as m_warehouse_name , " +                              //24    m_warehouse_name
                "t4.phone as m_warehouse_phone, " +                             //25    m_warehouse_phone
                "t4.city as m_warehouse_city, " +                               //26    m_warehouse_city
                "t4.warehouepricelistid as m_warehouse_warehouepricelistid, " + //27    m_warehouse_warehouepricelistid
                "t1.attribute1 as session_start_time " +                        //28
                "FROM ad_user_pos  t1,ad_org t2,ad_role t3,m_warehouse t4 " +
                "where t1.islogged = 'Y' AND t1.isactive = 'Y' AND t1.ad_org_id = t2.ad_org_id " +
                "AND t1.ad_client_id = t3.ad_client_id AND t1.ad_role_id = t3.ad_role_id " +
                "AND t1.m_warehouse_id = t4.m_warehouse_id; ", connection);
            NpgsqlDataReader _get_ad_user_pos_check = cmd_ad_user_pos_check.ExecuteReader();

            if (_get_ad_user_pos_check.Read())
            {
                AD_Client_ID = _get_ad_user_pos_check.GetInt64(0);
                AD_ORG_ID = _get_ad_user_pos_check.GetInt64(1);
                AD_ROLE_ID = _get_ad_user_pos_check.GetInt64(2);
                AD_USER_ID = _get_ad_user_pos_check.GetInt64(3);
                AD_bpartner_Id = _get_ad_user_pos_check.GetInt64(4);
                AD_Warehouse_Id = _get_ad_user_pos_check.GetInt64(5);
                AD_UserName = _get_ad_user_pos_check.GetString(6);
                AD_UserPassword = _get_ad_user_pos_check.GetString(7);
                AD_SessionID = _get_ad_user_pos_check.GetDouble(8);
               
                var Check_POS_Number_rs = RetailViewModel.Check_POS_Number(AD_UserName, AD_UserPassword, AD_Client_ID, AD_ORG_ID, AD_USER_ID, AD_bpartner_Id, AD_ROLE_ID, AD_Warehouse_Id, DeviceMacAdd);

                string _responce_code = Check_POS_Number_rs.Item3;
             //   bool _network_status_ = Check_POS_Number_rs.Item4;

                if (_responce_code == "0" || _responce_code == "200")
                {
                    _InvoiceNo_ = Check_POS_Number_rs.Item1;
                    _doc_no_or_error_code = Check_POS_Number_rs.Item2;
                }

            }
        }

        public int? invoice_no()
        {
            return _InvoiceNo_;
        }
    }

}