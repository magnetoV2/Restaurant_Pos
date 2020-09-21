using Barcode;
using log4net;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using Restaurant_Pos.Pages;

using Restaurant_Pos.ViewModels.Page;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToastNotifications.Messages;

namespace Restaurant_Pos
{
    public class RetailViewModel : BaseViewModel
    {
        public static bool NetworkUpStatus = NetworkInterface.GetIsNetworkAvailable();

        //To check a specific interface's status (or other info) use:
        private readonly NetworkInterface[] networkCards = NetworkInterface.GetAllNetworkInterfaces();

        public static string Product_barcode_search(string _barcode, string ad_client_id, string ad_org_id, string ad_pricelist_id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(PostgreSQL.ConnectionString);

            connection.Open();
            NpgsqlCommand cmd_clientId_Read = new NpgsqlCommand("SELECT  m_product.ad_client_id,m_product.ad_org_id,m_product. m_product_id,m_product.m_product_category_id,m_product.name,m_product.searchkey,m_product. arabicname,m_product.image," +
                "m_product.scanbyweight, m_product.scanbyprice, m_product.uomid, m_product.uomname, m_product.sopricestd, m_product.currentcostprice, m_product.attribute1, m_product_price.pricestd" +
                " FROM m_product, m_product_price, ad_sys_config" +
                " WHERE m_product.ad_client_id = " + ad_client_id + " AND m_product.ad_org_id = " + ad_org_id + " AND m_product.searchkey = '" + _barcode + "'" +
                " AND m_product.m_product_id = m_product_price.m_product_id  AND m_product_price.pricelistid = " + ad_pricelist_id + ";", connection);
            NpgsqlDataReader _clientId_Read = cmd_clientId_Read.ExecuteReader();
            if (_clientId_Read.Read())
            {
                int _ad_client_id = _clientId_Read.GetInt32(0);
                int _ad_org_id = _clientId_Read.GetInt32(1);
                int _m_product_id = _clientId_Read.GetInt32(2);
                int _m_product_category_id = _clientId_Read.GetInt32(3);
                string _name = _clientId_Read.GetString(4);
                string _searchkey = _clientId_Read.GetString(5);
                string _arabicname = _clientId_Read.GetString(6);
                string _image = _clientId_Read.GetString(7);
                string _scanbyweight = _clientId_Read.GetString(8);
                string _scanbyprice = _clientId_Read.GetString(9);
                int _uomid = _clientId_Read.GetInt32(10);
                string _uomname = _clientId_Read.GetString(11);
                int _sopricestd = _clientId_Read.GetInt32(15);
                int _currentcostprice = _clientId_Read.GetInt32(13);
                string _productMultiUOM = _clientId_Read.GetString(14);
                connection.Close();
                return @"{" +
                    "ad_client_id :" + _ad_client_id + "," +
                    "ad_org_id:" + _ad_org_id + "," +
                    "m_product_id:" + _m_product_id + "," +
                    "m_product_category_id:" + _m_product_category_id + "," +
                    "name: " + _name + "," +
                    "searchkey:'" + _searchkey + "'," +
                    "arabicname:'" + _arabicname + "'," +
                    "image:'" + _image + "'," +
                    "scanbyweight:'" + _scanbyweight + "'," +
                    "scanbyprice:'" + _scanbyprice + "'," +
                    "uomid:" + _uomid + "," +
                    "uomname:'" + _uomname + "'," +
                    "sopricestd:" + _sopricestd + "," +
                    "currentcostprice:" + _currentcostprice + "," +
                    "productMultiUOM:'" + _productMultiUOM + "'" +
                    "}";
            }
            else
            {
                connection.Close();
                return "N";
            }
        }

        public static dynamic Check_POS_Number(string AD_UserName, string AD_UserPassword, long AD_Client_ID, long AD_ORG_ID, long AD_USER_ID, long AD_bpartner_Id, long AD_ROLE_ID, long AD_Warehouse_Id, string macAddress)
        {
            dynamic POSOrderNumberApiStringResponce;
            dynamic POSOrderNumberApiJSONResponce;
            string jsonPOSOrderNumber;
            int CheckServerError;
            int invoice_number = 0;
            int document_no = 0;
            string _responce_code = "0";
            //int currentnext = 0;
            int start_no;
            int end_no;
            NpgsqlConnection connection = new NpgsqlConnection(PostgreSQL.ConnectionString);

            connection.Open();
            NpgsqlCommand cmd_c_invoic_sequenc_no = new NpgsqlCommand("SELECT start_no,end_no,doc_no FROM c_invoice_number_details where ad_client_id = " + AD_Client_ID + " AND ad_org_id = " + AD_ORG_ID + ";", connection);
            NpgsqlDataReader _get_c_invoice_number = cmd_c_invoic_sequenc_no.ExecuteReader();

            if (_get_c_invoice_number.Read())
            {
                document_no = _get_c_invoice_number.GetInt32(2);
                start_no = _get_c_invoice_number.GetInt32(0);
                end_no = _get_c_invoice_number.GetInt32(1);
                connection.Close();

                if (start_no == end_no)
                {
                    connection.Close();
                    //if (RetailPage._NetworkUpStatus != true)
                    //{
                    //    return (0, 0, "500", RetailPage._NetworkUpStatus);
                    //}

                    POSOrderNumber POSOrderNumber = new POSOrderNumber
                    {
                        operation = "POSOrderNumber",
                        username = AD_UserName,
                        password = AD_UserPassword,
                        clientId = AD_Client_ID.ToString(),
                        orgId = AD_ORG_ID.ToString(),
                        userId = AD_USER_ID.ToString(),
                        businessPartnerId = AD_bpartner_Id.ToString(),
                        roleId = AD_ROLE_ID.ToString(),
                        warehouseId = AD_Warehouse_Id.ToString(),
                        remindMe = "Y",
                        macAddress = LoginViewModel.DeviceMacAddress(),//LoginViewModel._DeviceMacAddress,
                        version = "1.0",
                        appName = "POS",
                    };

                    jsonPOSOrderNumber = JsonConvert.SerializeObject(POSOrderNumber);
                    try
                    {
                        POSOrderNumberApiStringResponce = PostgreSQL.ApiCallPost(jsonPOSOrderNumber);
                        CheckServerError = 1;
                    }
                    catch
                    {
                        CheckServerError = 0;
                    }
                    if (CheckServerError == 1)
                    {
                        POSOrderNumberApiStringResponce = PostgreSQL.ApiCallPost(jsonPOSOrderNumber);
                        POSOrderNumberApiJSONResponce = JsonConvert.DeserializeObject(POSOrderNumberApiStringResponce);

                        _responce_code = POSOrderNumberApiJSONResponce.responseCode;
                        if (POSOrderNumberApiJSONResponce.responseCode == "200")
                        {
                            string _prefix = POSOrderNumberApiJSONResponce.prefix;
                            string _startNo = POSOrderNumberApiJSONResponce.startNo;
                            invoice_number = Convert.ToInt32(POSOrderNumberApiJSONResponce.startNo);
                            string _endNo = POSOrderNumberApiJSONResponce.endNo;
                            string _docNo = POSOrderNumberApiJSONResponce.docNo;
                            document_no = Convert.ToInt32(POSOrderNumberApiJSONResponce.docNo);
                            string _incrementNo = POSOrderNumberApiJSONResponce.incrementNo;
                            string _currentNext = POSOrderNumberApiJSONResponce.currentNext;

                            connection.Close();

                            connection.Open();
                            NpgsqlCommand UPDATE_cmd_c_invoice_number_details = new NpgsqlCommand("UPDATE c_invoice_number_details SET start_no = " + _startNo + ", end_no = " + _endNo + ", updated = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' WHERE ad_client_id = " + AD_Client_ID + " AND ad_org_id = " + AD_ORG_ID + ";", connection);
                            UPDATE_cmd_c_invoice_number_details.ExecuteNonQuery();
                            connection.Close();
                        }
                        else
                        {
                         //   return (0, 0, POSOrderNumberApiJSONResponce.responseCode, RetailPage._NetworkUpStatus);
                        }
                    }
                    else
                    {
                    //    return (0, 0, "500", RetailPage._NetworkUpStatus);
                    }
                }
                else
                {
                    connection.Close();

                    connection.Open();
                    NpgsqlCommand cmd_c_invoic_sequenc_no1 = new NpgsqlCommand("SELECT start_no,end_no FROM c_invoice_number_details where ad_client_id = " + AD_Client_ID + " AND ad_org_id = " + AD_ORG_ID + "; ", connection);
                    NpgsqlDataReader _get_c_invoice_number1 = cmd_c_invoic_sequenc_no1.ExecuteReader();
                    _get_c_invoice_number1.Read();
                    invoice_number = _get_c_invoice_number1.GetInt32(0);
                    connection.Close();
                }
            }
            //currentnext = invoice_number + 1;
            //connection.Close();
            //connection.Open();
            //NpgsqlCommand cmd_update_invoic_sequenc_no = new NpgsqlCommand("UPDATE c_invoice_number_details SET start_no =" + currentnext + " where ad_client_id = " + AD_Client_ID + " AND ad_org_id = " + AD_ORG_ID + " AND ad_user_id = " + AD_USER_ID + " ; ", connection);
            //cmd_update_invoic_sequenc_no.ExecuteReader();
            connection.Close();
            Update_POS_Invoice_Number(AD_Client_ID, AD_ORG_ID, AD_USER_ID, invoice_number);
         //   return (invoice_number, document_no, _responce_code, RetailPage._NetworkUpStatus);
            return (invoice_number, document_no, _responce_code);
        }

        public static void Update_POS_Invoice_Number(long AD_Client_ID, long AD_ORG_ID, long AD_USER_ID, long up_invoice_number)
        {
            NpgsqlConnection connection = new NpgsqlConnection(PostgreSQL.ConnectionString);
            long currentnext = 0;
            currentnext = up_invoice_number + 1;
            connection.Open();
            NpgsqlCommand cmd_update_invoic_sequenc_no = new NpgsqlCommand("UPDATE c_invoice_number_details SET start_no =" + currentnext + " where ad_client_id = " + AD_Client_ID + " AND ad_org_id = " + AD_ORG_ID + " ;", connection);
            cmd_update_invoic_sequenc_no.ExecuteReader();
            connection.Close();
        }

        //public static ImageSource ToImageSource(this System.Drawing.Bitmap bmp)
        //{
        //    var handle = bmp.GetHbitmap();
        //    try
        //    {
        //        return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        //    }
        //    catch { return null; }
        //}
        //public static System.Drawing.Bitmap Base64StringToBitmap(string base64String)
        //{
        //    System.Drawing.Bitmap bmpReturn = null;
        //    //Convert Base64 string to byte[]
        //    byte[] byteBuffer = Convert.FromBase64String(base64String);
        //    MemoryStream memoryStream = new MemoryStream(byteBuffer);

        //    memoryStream.Position = 0;

        //    bmpReturn = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(memoryStream);

        //    memoryStream.Close();
        //    memoryStream = null;
        //    byteBuffer = null;

        //    return bmpReturn;
        //}
        //public static System.Drawing.Bitmap ChangeColor(System.Drawing.Bitmap scrBitmap)
        //{
        //    //You can change your new color here. Red,Green,LawnGreen any..
        //    System.Drawing.Color newColor = System.Drawing.Color.Red;
        //    System.Drawing.Color actualColor;
        //    //make an empty bitmap the same size as scrBitmap
        //    System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(scrBitmap.Width, scrBitmap.Height);
        //    for (int i = 0; i < scrBitmap.Width; i++)
        //    {
        //        for (int j = 0; j < scrBitmap.Height; j++)
        //        {
        //            //get the pixel from the scrBitmap image
        //            actualColor = scrBitmap.GetPixel(i, j);
        //            // > 150 because.. Images edges can be of low pixel colr. if we set all pixel color to new then there will be no smoothness left.
        //            if (actualColor.A > 150)
        //                newBitmap.SetPixel(i, j, newColor);
        //            else
        //                newBitmap.SetPixel(i, j, actualColor);
        //        }
        //    }
        //    return newBitmap;
        //}

        public static void Base64_Color_Change(string AD_ORG_logo)
        {
            System.Drawing.Bitmap bmpReturn = null;
            //Convert Base64 string to byte[]
            byte[] byteBuffer = Convert.FromBase64String(AD_ORG_logo);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);

            memoryStream.Position = 0;

            bmpReturn = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(memoryStream);

            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;

            //You can change your new color here. Red,Green,LawnGreen any..
            System.Drawing.Color newColor = System.Drawing.Color.Red;
            System.Drawing.Color actualColor;
            //make an empty bitmap the same size as scrBitmap
            System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(bmpReturn.Width, bmpReturn.Height);
            for (int i = 0; i < bmpReturn.Width; i++)
            {
                for (int j = 0; j < bmpReturn.Height; j++)
                {
                    //get the pixel from the bmpReturn image
                    actualColor = bmpReturn.GetPixel(i, j);
                    // > 150 because.. Images edges can be of low pixel colr. if we set all pixel color to new then there will be no smoothness left.
                    if (actualColor.A > 150)
                        newBitmap.SetPixel(i, j, newColor);
                    else
                        newBitmap.SetPixel(i, j, actualColor);
                }
            }

            var handle = newBitmap.GetHbitmap();

            Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }



        public static string ConvertNumbertoWords(long number)
        {
            if (number == 0) return "ZERO";
            if (number < 0) return "minus " + ConvertNumbertoWords(Math.Abs(number));
            string words = "";
            if ((number / 1000000) > 0)
            {
                words += ConvertNumbertoWords(number / 100000) + " LAKES ";
                number %= 1000000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000) + " THOUSAND ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWords(number / 100) + " HUNDRED ";
                number %= 100;
            }
            //if ((number / 10) > 0)  
            //{  
            // words += ConvertNumbertoWords(number / 10) + " RUPEES ";  
            // number %= 10;  
            //}  
            if (number > 0)
            {
                if (words != "") words += "AND ";
                var unitsMap = new[]
                {
            "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN"
        };
                var tensMap = new[]
                {
            "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY"
        };
                if (number < 20) words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0) words += " " + unitsMap[number % 10];
                }
            }
            return words;
        }
        private static List<OrderDetails> OrderDetails_Print = new List<OrderDetails>();
        private static List<OrderHeaderSheeetBill> OrderHeaderSheet_Print = new List<OrderHeaderSheeetBill>();
        private static List<OrderDetailSheeetBill> OrderDetailsSheet_Print = new List<OrderDetailSheeetBill>();
        public static void Get_invoice_And_print(string invoice_no, Boolean Duplicate)
        {
            #region Get Data To Print
            OrderDetails_Print.Clear();
            string customer_No = "";
            NpgsqlConnection connection = new NpgsqlConnection(PostgreSQL.ConnectionString);
            //Getting Last 5 Invoice Which is Complected and Not Posted
            connection.Open();
            NpgsqlCommand cmd_c_invoic_iPaymentD = new NpgsqlCommand("SELECT t1.c_invoice_id, t1.ad_client_id, t1.ad_org_id,t1.ad_user_id," +
                "t1.ad_role_id,t1.documentno, t1.m_warehouse_id, t1.c_bpartner_id, t1.qid, t1.mobilenumber, t1.discounttype, t1.discountvalue, " +
                "t1.grandtotal, t1.orderid, t1.reason as invoice_reason,t1.created, t1.grandtotal_round_off, t1.total_items_count, t1.balance, t1.change, t1.lossamount, " +
                "t1.extraamount,t2.cash, t2.card, t2.exchange, t2.redemption, t2.iscomplementary, t2.iscredit,t2.c_invoice_id as name_id, '8767889809'as mobile_numbler,'dd' as reason,t3.name,t4.name as m_warehouse_name,t3.searchkey,t1.is_return  " +
                "FROM c_invoice t1 ,c_invoicepaymentdetails t2,c_bpartner t3,m_warehouse t4 " +
               // "WHERE is_completed = 'Y' AND t1.c_invoice_id = t2.c_invoice_id AND t1.c_bpartner_id = t3.c_bpartner_id AND t1.m_warehouse_id=t4.m_warehouse_id AND t1.c_invoice_id = " + invoice_no + ";", connection);
                ";", connection);

            NpgsqlDataReader _get_c_invoic_iPaymentD = cmd_c_invoic_iPaymentD.ExecuteReader();
            if (_get_c_invoic_iPaymentD.HasRows == false)
            {
                Console.WriteLine("Error");
                log.Info("Invoice Not Found ID:" + invoice_no);
                connection.Close();
                return;
            }
            log.Info("Invoice ID:" + invoice_no);
            _get_c_invoic_iPaymentD.Read();
            Invoice_Post Invoice_Print = new Invoice_Post()
            {
                c_invoice_id = _get_c_invoic_iPaymentD.GetDouble(0).ToString(),
                ad_client_id = _get_c_invoic_iPaymentD.GetDouble(1).ToString(),
                ad_org_id = _get_c_invoic_iPaymentD.GetDouble(2).ToString(),
                ad_user_id = _get_c_invoic_iPaymentD.GetDouble(3).ToString(),
                ad_role_id = _get_c_invoic_iPaymentD.GetDouble(4).ToString(),
                documentno = _get_c_invoic_iPaymentD.GetString(5),
                m_warehouse_id = _get_c_invoic_iPaymentD.GetDouble(6).ToString(),
                c_bpartner_id = _get_c_invoic_iPaymentD.GetDouble(7).ToString(),
                qid = _get_c_invoic_iPaymentD.GetString(8),
                mobilenumber = _get_c_invoic_iPaymentD.GetString(9),
                discounttype = _get_c_invoic_iPaymentD.GetDouble(10).ToString(),
                discountvalue = _get_c_invoic_iPaymentD.GetDouble(11).ToString(),
                grandtotal = _get_c_invoic_iPaymentD.GetDouble(12).ToString(),
                orderid = _get_c_invoic_iPaymentD.GetDouble(13).ToString(),
                invoice_reason = _get_c_invoic_iPaymentD.GetString(14),
                created = _get_c_invoic_iPaymentD.GetDateTime(15).ToString("yyyy-MM-dd HH:mm:ss"),
                grandtotal_round_off = _get_c_invoic_iPaymentD.GetDouble(16).ToString(),
                total_items_count = _get_c_invoic_iPaymentD.GetDouble(17).ToString(),
                balance = _get_c_invoic_iPaymentD.GetDouble(18).ToString(),
                change = _get_c_invoic_iPaymentD.GetDouble(19).ToString(),
                lossamount = _get_c_invoic_iPaymentD.GetDouble(20).ToString(),
                extraamount = _get_c_invoic_iPaymentD.GetDouble(21).ToString(),
                cash = _get_c_invoic_iPaymentD.GetDouble(22).ToString(),
                card = _get_c_invoic_iPaymentD.GetDouble(23).ToString(),
                exchange = _get_c_invoic_iPaymentD.GetDouble(24).ToString(),
                redemption = _get_c_invoic_iPaymentD.GetDouble(25).ToString(),
                iscomplementary = _get_c_invoic_iPaymentD.GetString(26),
                iscredit = _get_c_invoic_iPaymentD.GetString(27),
                name_id = _get_c_invoic_iPaymentD.GetString(28),
                mobile_numbler = _get_c_invoic_iPaymentD.GetString(29),
                reason = _get_c_invoic_iPaymentD.GetString(30),
                c_bpartner_name = _get_c_invoic_iPaymentD.GetString(31),
                m_warehouse_name = _get_c_invoic_iPaymentD.GetString(32),
                is_return = _get_c_invoic_iPaymentD.GetString(34)
            };
            customer_No = _get_c_invoic_iPaymentD.GetString(33);
            connection.Close();

            #region Posting Each Invoice to Sever and Updating the is_posted flag to 'Y' 
            connection.Open();
            NpgsqlCommand cmd_c_invoic_c_invoiceline = new NpgsqlCommand("SELECT " +
                "t1.m_product_id," +          //0
                "t1.productname," +           //1
                "t1.paroductarabicname," +    //2
                "t1.productbarcode," +        //3
                "t1.c_uom_id," +              //4
                "t1.uomname," +               //5
                "t1.qtyinvoiced," +           //6
                "t1.qtyentered," +            //7
                "t1.saleprice," +             //8
                "t1.costprice," +             //9
                "t1.discounttype," +          //10
                "t1.discountvalue," +         //11
                "t1.linetotalamt," +          //12
                "t1.pricelistid," +           //13
                "t1.islinediscounted," +      //14
                "t2.m_product_category_id," + //15
                "t2.searchkey  " +//16 
                "FROM c_invoiceline t1 , m_product t2 " +
                "WHERE t1.m_product_id = t2.m_product_id " +
                "AND t1.ad_client_id = " + Invoice_Print.ad_client_id + " " +
                "AND t1.ad_org_id =" + Invoice_Print.ad_org_id + "  " +
                "AND t1.c_invoice_id = " + Invoice_Print.c_invoice_id + " " +
                "AND t1.ad_user_id = " + Invoice_Print.ad_user_id + " ;", connection);
            NpgsqlDataReader _get_c_invoic_c_invoiceline = cmd_c_invoic_c_invoiceline.ExecuteReader();

            while (_get_c_invoic_c_invoiceline.Read())
            {
                double _dicount = _get_c_invoic_c_invoiceline.GetDouble(11),
                discountType = _get_c_invoic_c_invoiceline.GetDouble(10),
                _price = _get_c_invoic_c_invoiceline.GetDouble(12),
                _actualPrice = _get_c_invoic_c_invoiceline.GetDouble(8), _qty = _get_c_invoic_c_invoiceline.GetInt32(7);
                string _discountType, _dicountPercent, _discountAmount;
                if (discountType == 0)
                {
                    _discountType = "P";
                    _dicountPercent = _dicount.ToString();
                    _discountAmount = Math.Round((((_actualPrice * _qty) * _dicount) / 100), 2).ToString();
                }
                else
                {
                    _discountType = "A";
                    _dicountPercent = Math.Round(((_dicount / _price) * 100), 2).ToString();
                    _discountAmount = _dicount.ToString();
                }
                OrderDetails_Print.Add(new OrderDetails()
                {
                    isExists = "N",
                    KotLineID = "0",
                    description = "",
                    uomId = _get_c_invoic_c_invoiceline.GetInt32(4).ToString(),
                    uomName = _get_c_invoic_c_invoiceline.GetString(5),
                    productUOMValue = _get_c_invoic_c_invoiceline.GetString(5),
                    actualPrice = _get_c_invoic_c_invoiceline.GetDouble(8).ToString(),
                    costPrice = _get_c_invoic_c_invoiceline.GetInt32(9).ToString(),
                    orderedQty = _get_c_invoic_c_invoiceline.GetInt32(6).ToString(),
                    qty = _get_c_invoic_c_invoiceline.GetInt32(7).ToString(),
                    discountType = _discountType,
                    dicountPercent = _dicountPercent,
                    discountAmount = _discountAmount,
                    price = _price.ToString(),
                    productId = _get_c_invoic_c_invoiceline.GetInt32(0).ToString(),
                    productName = _get_c_invoic_c_invoiceline.GetString(1),
                    productArabicName = _get_c_invoic_c_invoiceline.GetString(2),
                    productCategoryId = _get_c_invoic_c_invoiceline.GetDouble(15).ToString(),
                    productSearchKey = _get_c_invoic_c_invoiceline.GetString(16),
                });
            }

            connection.Close();
            string user_name;
            connection.Open();
            NpgsqlCommand cmd_c_invoic_ad_user_pos = new NpgsqlCommand("SELECT " +
                "name " +             //0
                "FROM ad_user_pos " +
                "WHERE ad_client_id = " + Invoice_Print.ad_client_id + " AND ad_org_id = " + Invoice_Print.ad_org_id + " AND ad_user_id = " + Invoice_Print.ad_user_id + " ; ", connection);
            NpgsqlDataReader _get_c_invoic_ad_user_pos = cmd_c_invoic_ad_user_pos.ExecuteReader();
            _get_c_invoic_ad_user_pos.Read();
            user_name = _get_c_invoic_ad_user_pos.GetString(0);

            connection.Close();
            string ad_org_name, ad_org_arabicname, ad_org_logo, ad_org_phone, ad_org_email, ad_org_address, ad_org_city, ad_org_country, ad_org_postal, ad_org_weburl, ad_org_footermessage, ad_org_arabicfootermessage,
                ad_org_termsmessage, ad_org_arabictermsmessage, ad_org_displayimage;
            connection.Open();
            NpgsqlCommand cmd_ad_org = new NpgsqlCommand("SELECT " +
                "name, " +                  //0
                "arabicname, " +            //1
                "logo, " +                  //2
                "phone, " +                 //3
                "email, " +                 //4
                "address, " +               //5
                "city," +                   //6
                "country, " +               //7
                "postal, " +                //8
                "weburl, " +                //9
                "footermessage, " +         //10
                "arabicfootermessage, " +   //11
                "termsmessage, " +          //12
                "arabictermsmessage, " +    //13
                "displayimage " +           //14
                "FROM ad_org " +            //15
                "WHERE ad_client_id = " + Invoice_Print.ad_client_id + " AND ad_org_id = " + Invoice_Print.ad_org_id + "; ", connection);
            NpgsqlDataReader _get_ad_org = cmd_ad_org.ExecuteReader();
            _get_ad_org.Read();
            ad_org_name = _get_ad_org.GetString(0);
            ad_org_arabicname = _get_ad_org.GetString(1);
            ad_org_logo = _get_ad_org.GetString(2);
            ad_org_phone = _get_ad_org.GetString(3);
            ad_org_email = _get_ad_org.GetString(4);
            ad_org_address = _get_ad_org.GetString(5);
            ad_org_city = _get_ad_org.GetString(6);
            ad_org_country = _get_ad_org.GetString(7);
            ad_org_postal = _get_ad_org.GetString(8);
            ad_org_weburl = _get_ad_org.GetString(9);
            ad_org_footermessage = _get_ad_org.GetString(10);
            ad_org_arabicfootermessage = _get_ad_org.GetString(11);
            ad_org_termsmessage = _get_ad_org.GetString(12);
            ad_org_arabictermsmessage = _get_ad_org.GetString(13);
            ad_org_displayimage = _get_ad_org.GetString(14);
            connection.Close();
            #endregion Posting Each Invoice to Sever and Updating the is_posted flag to 'Y'

            connection.Close();

            #endregion Get Data To Print

            //--------------------------------------------------------------------------------------------------------------------------------//
            //-----------------------------                          PRINT FUNCTION                    ---------------------------------------//
            //--------------------------------------------------------------------------------------------------------------------------------//

            #region Print
            string billFmt = "";
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var BillFormat = configuration.AppSettings.Settings["BillFormat"];
            if (BillFormat == null)
            {
                billFmt = "NormalBill";
            }
            else
            {
                billFmt = BillFormat.Value;
            }
            if (billFmt == "A4" || billFmt == "A5")
            {
                LocalReport localreport = new LocalReport();
                if(billFmt=="A5")
                {
                    localreport.ReportPath = "InvoiceA5.rdlc";// @"C:\Zearo\zearo-retail-windows-pos-master\zearo-retail-windows-pos-master\Restaurant_Pos\Invoice.rdlc";
                }
                if (billFmt == "A4")
                {
                    localreport.ReportPath = "Invoice.rdlc";// @"C:\Zearo\zearo-retail-windows-pos-master\zearo-retail-windows-pos-master\Restaurant_Pos\Invoice.rdlc";
                }

                OrderHeaderSheet_Print.Clear();
                string ftaddress = "";
                if(ad_org_postal!="")
                {
                    ftaddress = "Po.No:" + ad_org_postal;
                }
                if(Invoice_Print.m_warehouse_name!="")
                {
                    if (ftaddress != "")
                        ftaddress += ", " + Invoice_Print.m_warehouse_name;
                    else
                        ftaddress +=  Invoice_Print.m_warehouse_name;
                }
                if(ad_org_city != "")
                {
                    if (ftaddress != "")
                        ftaddress += ", " + ad_org_city;
                    else
                        ftaddress += ad_org_city; 
                }
                if(ad_org_phone!="")
                {
                   if(ftaddress!="") 
                        ftaddress += ", Phone:" + ad_org_phone; 
                   else 
                        ftaddress += "Phone:" + ad_org_phone; 
                }
                ftaddress += ", Fax:" + "-";
                if (ad_org_email != "")
                {
                    if (ftaddress != "") 
                        ftaddress += ", email:" + ad_org_email; 
                    else 
                        ftaddress += "email:"+ad_org_email; 
                }
                string orgaddress = "";
                if(ad_org_postal!="")
                {
                    orgaddress = "Po.No: " + ad_org_postal;
                }
                if(ad_org_address != "")
                {
                    if(orgaddress!="") 
                        orgaddress += ", " + ad_org_address;
                    else
                        orgaddress += ad_org_address;
                }
                if(ad_org_city != "")
                {
                    if (orgaddress != "")
                        orgaddress += ", " + ad_org_city;
                    else
                        orgaddress += ad_org_city;
                }
                string amtword = ConvertNumbertoWords(long.Parse(Invoice_Print.grandtotal));
                OrderHeaderSheet_Print.Add(new OrderHeaderSheeetBill()
                {
                    orgname = ad_org_name,
                    logo = ad_org_logo,
                    wareHouse = Invoice_Print.m_warehouse_name,
                    address = orgaddress,
                    city = ad_org_city,
                    postalCode = ad_org_postal,
                    phoneNumber = ad_org_phone,
                    fax = "-",
                    attn = "-",
                    dnNo = "-",
                    lpoReference = "-",
                    invoiceNo = Invoice_Print.c_invoice_id,
                    date = Convert.ToDateTime(Invoice_Print.created).ToString("dd-MMM-yyyy"),
                    orderReference = "-",
                    paymentTerm = "Immediate",
                    grandTotal = Convert.ToDecimal(Invoice_Print.grandtotal).ToString("0.00"),
                    customerName = "Cash Customer",
                    customerNo = customer_No,
                    amountwords = amtword + " ONLY",
                    email=ad_org_email,
                    footaddress = ftaddress,
                    arabicName=""

                });
                int sno = 1;
                OrderDetailsSheet_Print.Clear();
                foreach (var lineitem in OrderDetails_Print)
                {
                    OrderDetailsSheet_Print.Add(new OrderDetailSheeetBill()
                    {
                        sno = sno.ToString(),
                         searchKey = lineitem.productSearchKey,
                        discription=  string.IsNullOrEmpty(lineitem.productArabicName) ? lineitem.productName : lineitem.productName + "  |  " + lineitem.productArabicName,
                   // discription =lineitem.productName+"  |  "+ lineitem.productArabicName,
                         uom =lineitem.productUOMValue,
                         qty =lineitem.qty,
                         unitPrice=Convert.ToDouble(lineitem.actualPrice).ToString("0.00"),
                         discount= Convert.ToDouble(lineitem.discountAmount).ToString("0.00"),
                        total = Convert.ToDouble((Convert.ToInt64(lineitem.qty) * Convert.ToDouble(lineitem.price))).ToString("0.00"),
                        invoiceNo= Invoice_Print.c_invoice_id,
                        arabicName=lineitem.productArabicName
                    });
                    sno++;

                }

                //DataSet dtHeader = new DataSet();
                localreport.DataSources.Clear();

                string jsondetails = Newtonsoft.Json.JsonConvert.SerializeObject(OrderDetailsSheet_Print);
                DataTable pDdtdetails = JsonConvert.DeserializeObject<DataTable>(jsondetails);
                //dtDetails.Tables.Add(pDdtdetails);
                ReportDataSource reportDataSourcedetails = new ReportDataSource();

                reportDataSourcedetails.Name = "Ds_Details"; // Name of the DataSet we set in .rdlc
                reportDataSourcedetails.Value = pDdtdetails;
                localreport.Refresh();
                localreport.DataSources.Add(reportDataSourcedetails);



                string json = Newtonsoft.Json.JsonConvert.SerializeObject(OrderHeaderSheet_Print);
                DataTable pDt = JsonConvert.DeserializeObject<DataTable>(json);
                //dtHeader.Tables.Add(pDt);
                ReportDataSource reportDataSource = new ReportDataSource(); 
                reportDataSource.Name = "DS_HEader"; // Name of the DataSet we set in .rdlc
                reportDataSource.Value = pDt;
                localreport.DataSources.Add(reportDataSource); 

              //  DataSet dtDetails = new DataSet(); 
              
                localreport.Print();
                return;
            }

            #region Public
            string formattedVal = "";

            string baseFont = "./Resources/Fonts/GothamRounded/#GothamRounded-Book";
            PrintDialog printDlg = new PrintDialog();
            FlowDocument doc = new FlowDocument();
            Span s = new Span();

            #endregion Public

            if (billFmt == "ShortBill")
            {
             

                Paragraph Invoice_Header = new Paragraph();
                    s = new Span(new Run(""));
                    s.Inlines.Add(new LineBreak()); 
                    Invoice_Header.Inlines.Add(s);

                    string Cashier = ad_org_name;
                    if (Duplicate)
                    {
                        s = new Span(new Run("Duplicate Copy"));
                        s.Inlines.Add(new LineBreak());
                        s.FontSize = 16;
                        Invoice_Header.Inlines.Add(s);
                    }
                    else
                    {
                    if (Invoice_Print.iscomplementary == "Y" || Invoice_Print.iscredit == "Y")
                    {
                        if (Invoice_Print.iscomplementary == "Y")
                        {
                            Cashier = Cashier + " - Complementary Bill";
                            formattedVal = ConfigurationManager.AppSettings.Get("lblComplement");

                        }
                        if (Invoice_Print.iscredit == "Y")
                        {
                            Cashier = Cashier + " - Credit Bill";
                            formattedVal = ConfigurationManager.AppSettings.Get("lblCreditBill");
                            //s = new Span(new Run("Credit Bill"));
                            //s.Inlines.Add(new LineBreak());
                            //s.FontSize = 16;
                            //Invoice_Header.Inlines.Add(s);
                            //formattedVal = ConfigurationManager.AppSettings.Get("lblCreditBill");
                            //s = new Span(new Run(formattedVal));
                            //s.Inlines.Add(new LineBreak());
                            //s.FontSize = 16;
                            //Invoice_Header.Inlines.Add(s);
                        }
                    }
                    else if (Invoice_Print.is_return == "Y")
                    {
                        Cashier = Cashier + " - Credit Memo";
                        formattedVal = ConfigurationManager.AppSettings.Get("lblCreditMemo");
                    }
                    else
                    {
                        Cashier = Cashier + " - Cash Bill";
                        formattedVal = ConfigurationManager.AppSettings.Get("lblCashBill");
                         
                    }
                    }

                    Paragraph Invoice_cashier_header = new Paragraph();
                    s = new Span(new Run("")); 
                    Invoice_cashier_header.Inlines.Add(s);
                    s = new Span(new Run(""));
                    s.Inlines.Add(new LineBreak());
                    Invoice_cashier_header.Inlines.Add(s);
                    Invoice_cashier_header.Inlines.Add(s);
                    s = new Span(new Run(Cashier));
                    s.Inlines.Add(new LineBreak());
                    Invoice_cashier_header.Inlines.Add(s);
                    s = new Span(new Run(formattedVal));
                    s.Inlines.Add(new LineBreak());
                    Invoice_cashier_header.Inlines.Add(s);
                    Invoice_cashier_header.FontSize = 14;
                    Invoice_cashier_header.FontStyle = FontStyles.Normal;
                    Invoice_cashier_header.TextAlignment = System.Windows.TextAlignment.Center;
                    Invoice_cashier_header.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                    Invoice_cashier_header.Margin = new Thickness(0);
                    doc.Blocks.Add(Invoice_cashier_header);

                Table oTable = new Table();

                // Append the table to the document
                oTable.CellSpacing = 0;
                oTable.Padding = new Thickness(0);
                oTable.FontSize = 14;
                oTable.FontStyle = FontStyles.Normal;
                oTable.TextAlignment = System.Windows.TextAlignment.Center;
                oTable.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                oTable.Margin = new Thickness(0);   
                doc.Blocks.Add(oTable);

                

                // Create 5 columns and add them to the table's Columns collection.
                 
                // Create and add an empty TableRowGroup Rows.

                oTable.RowGroups.Add(new TableRowGroup()); 

                // Add the table head row.

                oTable.RowGroups[0].Rows.Add(new TableRow());

                //Configure the table head row

                TableRow currentRow = oTable.RowGroups[0].Rows[0];

                // Add the header row with content,
                Paragraph pBillno = new Paragraph();
                pBillno.TextAlignment = TextAlignment.Left;
                string Bill_No = "Bill No : " + invoice_no;
                s = new Span(new Run(Bill_No));
                pBillno.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pBillno)); 
                Paragraph pdate = new Paragraph();
                pdate.TextAlignment = TextAlignment.Right;
                string Date = "Date : " + DateTime.Now.ToString("dd/MM/yyyy");
                s = new Span(new Run(Date));
                pdate.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pdate)); 
                oTable.RowGroups[0].Rows.Add(new TableRow());

                currentRow = oTable.RowGroups[0].Rows[1]; 

                //Add the country name in the first cell
                Paragraph pName = new Paragraph();
                pName.TextAlignment = TextAlignment.Left;
                string Cashiername = "Cashier  : " + user_name; 
                s = new Span(new Run(Cashiername));
                pName.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pName)); 


                Paragraph pTime = new Paragraph();
                pTime.TextAlignment = TextAlignment.Right;
                string Time = "Time : " + DateTime.Now.ToShortTimeString()+"     ";
                s = new Span(new Run(Time));
                pTime.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pTime)); 
                 
                

                Paragraph Invoice_Header_Lines = new Paragraph(); 

                s = new Span(new Run("-----------------------------------------------"));//47
                     s.Inlines.Add(new LineBreak());
                     Invoice_Header_Lines.Inlines.Add(s);
                     s = new Span(new Run("Qty\t\t Item\t\t  Price"));
                     s.Inlines.Add(new LineBreak());
                     Invoice_Header_Lines.Inlines.Add(s);
                     formattedVal = ConfigurationManager.AppSettings.Get("lblNoOfItems") + "\t" + ConfigurationManager.AppSettings.Get("lblItem") + "\t\t" + ConfigurationManager.AppSettings.Get("lblPrice");
                     s = new Span(new Run(formattedVal));
                     s.Inlines.Add(new LineBreak());
                     Invoice_Header_Lines.Inlines.Add(s);
                     s = new Span(new Run("-----------------------------------------------"));//47
                                                                                              //s.Inlines.Add(new LineBreak());
                     Invoice_Header_Lines.Inlines.Add(s);

                    //Give style and formatting to paragraph content.
                    Invoice_Header_Lines.FontSize = 14;
                    Invoice_Header_Lines.FontStyle = FontStyles.Normal;
                    Invoice_Header_Lines.TextAlignment = System.Windows.TextAlignment.Left;
                    Invoice_Header_Lines.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                    Invoice_Header_Lines.Margin = new Thickness(0);  
                    doc.Blocks.Add(Invoice_Header_Lines);

                    #region OrderList

                    double subtotal = 0;
                double subtotal_dis = 0;

                Section OrderDetails_sec = new Section();
                Table dtable = new Table();
                
                    TableColumn tc = new TableColumn();

                    tc.Width = new System.Windows.GridLength(30);
                    dtable.Columns.Add(tc);

                TableColumn tc1 = new TableColumn();
                    tc1.Width = new System.Windows.GridLength(180);
                    dtable.Columns.Add(tc1);

                TableColumn tc2 = new TableColumn();
                    tc2.Width = new System.Windows.GridLength(50);
                    dtable.Columns.Add(tc2); 

                dtable.CellSpacing = 0;
                dtable.Padding = new Thickness(0);
                dtable.FontSize = 14;
                dtable.FontStyle = FontStyles.Normal;
                dtable.TextAlignment = System.Windows.TextAlignment.Center;
                dtable.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                dtable.Margin = new Thickness(0);
                doc.Blocks.Add(dtable);
                dtable.RowGroups.Add(new TableRowGroup());

                int row = 0;
                OrderDetails_Print.ForEach(x =>
                {
                    dtable.RowGroups[0].Rows.Add(new TableRow());
                    currentRow = dtable.RowGroups[0].Rows[row];
                     
                    Paragraph pqty = new Paragraph();
                    pqty.TextAlignment = TextAlignment.Left;
                    string qty = x.qty;
                    s = new Span(new Run(qty));
                    pqty.Inlines.Add(s);
                    currentRow.Cells.Add(new TableCell(pqty));
                     
                    Paragraph pitem = new Paragraph();
                    pitem.TextAlignment = TextAlignment.Left;
                    string productName = SerialPort.Truncate(x.productName, 25);
                    s = new Span(new Run(productName));
                    pitem.Inlines.Add(s);
                    currentRow.Cells.Add(new TableCell(pitem));

                    // string price = Convert.ToDouble(x.price).ToString("0.00");
                    string price = (Convert.ToDouble(x.qty) * Convert.ToDouble(x.actualPrice)).ToString("0.00");
                     Paragraph pprice = new Paragraph();
                    pprice.TextAlignment = TextAlignment.Right; 
                    s = new Span(new Run(price));
                    pprice.Inlines.Add(s);
                    currentRow.Cells.Add(new TableCell(pprice));
                    subtotal += (Convert.ToDouble(x.actualPrice) * Convert.ToDouble(x.qty));
                   // subtotal += Convert.ToDouble(x.actualPrice) * Convert.ToDouble(x.qty);
                     subtotal_dis += (Convert.ToDouble(x.actualPrice) * Convert.ToDouble(x.qty)) - Convert.ToDouble(x.price);
                    row++;
                    //Paragraph OrderDetails_price = new Paragraph();
                    ////int _p = Convert.ToDouble(x.price).ToString("0.00").Length;
                    ////string price = Convert.ToDouble(x.price).ToString("0.00");
                    //string price = (Convert.ToDouble(x.qty) * Convert.ToDouble(x.actualPrice)).ToString("0.00");
                    //string lineitem = x.qty + "   " + SerialPort.Truncate(x.productName, 25);
                    //int _p = price.Length;


                    ////  int a = print_spacing - _p;
                    //string rightval = price.PadLeft(76 - (lineitem.Length * 2) - _p);
                    ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));

                    //s = new Span(new Run(x.qty + "   " + SerialPort.Truncate(x.productName, 25) + rightval));//.PadLeft(a)
                    //                                                                                         //s.Inlines.Add(new LineBreak());
                    //OrderDetails_price.Inlines.Add(s);
                    //OrderDetails_price.Margin = new Thickness(0);
                    //OrderDetails_price.FontSize = 14;
                    //OrderDetails_price.FontStyle = FontStyles.Normal;
                    //OrderDetails_price.TextAlignment = TextAlignment.Left;
                    //OrderDetails_price.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                    //OrderDetails_sec.Blocks.Add(OrderDetails_price);
                    //subtotal += Convert.ToDouble(x.actualPrice) * Convert.ToDouble(x.qty);
                    //subtotal_dis += (Convert.ToDouble(x.actualPrice) * Convert.ToDouble(x.qty)) - Convert.ToDouble(x.price);
                });

                // doc.Blocks.Add(OrderDetails_sec);
                Paragraph Invoice_Header_Lines_1 = new Paragraph();
                    s = new Span(new Run("-----------------------------------------------"));//47
                    Invoice_Header_Lines_1.Inlines.Add(s);
                    s.Inlines.Add(new LineBreak());  
                    Invoice_Header_Lines_1.Inlines.Add(s);
                    Invoice_Header_Lines_1.FontSize = 14;
                    Invoice_Header_Lines_1.FontStyle = FontStyles.Normal;
                    Invoice_Header_Lines_1.TextAlignment = System.Windows.TextAlignment.Left;
                    Invoice_Header_Lines_1.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                    Invoice_Header_Lines_1.Margin = new Thickness(0);
                    doc.Blocks.Add(Invoice_Header_Lines_1);
                 

                Paragraph Grand_total = new Paragraph();
                s = new Span(new Run("Discounts (D+R)  " + Convert.ToDouble(subtotal_dis + Convert.ToDouble(Invoice_Print.lossamount)).ToString("0.00") + "  "));//discount and roundoff  
                s.Inlines.Add(new LineBreak());
                Grand_total.Inlines.Add(s);
                s = new Span(new Run(ConfigurationManager.AppSettings.Get("lblDiscount") + "             "));
                s.Inlines.Add(new LineBreak());
                Grand_total.Inlines.Add(s);
                s = new Span(new Run("Grand Total  "+ Convert.ToDouble(subtotal- subtotal_dis - Convert.ToDouble(Invoice_Print.lossamount)).ToString("0.00")+"  "));//less with roundoff with total
                    s.Inlines.Add(new LineBreak());
                    Grand_total.Inlines.Add(s);
                    s = new Span(new Run(ConfigurationManager.AppSettings.Get("lblGrandTotal")+"             "));
                    s.Inlines.Add(new LineBreak());
                    Grand_total.Inlines.Add(s);
                    s = new Span(new Run("-----------------------------------------------"));//47
                    s.Inlines.Add(new LineBreak());
                    Grand_total.Inlines.Add(s);
                    Grand_total.FontSize = 14;
                    Grand_total.FontStyle = FontStyles.Normal;
                    Grand_total.TextAlignment = System.Windows.TextAlignment.Right;
                    Grand_total.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                    Grand_total.Margin = new Thickness(0);
                    doc.Blocks.Add(Grand_total);
                    #endregion OrderList

                    Paragraph Invoice_cashier_footer = new Paragraph();  
                    Invoice_cashier_footer.Inlines.Add(s);
                    string footer_val = "Powered by qsale.qa";
                    s = new Span(new Run(footer_val));
                    s.Inlines.Add(new LineBreak());
                    Invoice_cashier_footer.Inlines.Add(s);
                    Invoice_cashier_footer.FontSize = 14;
                    Invoice_cashier_footer.FontStyle = FontStyles.Normal;
                    Invoice_cashier_footer.TextAlignment = System.Windows.TextAlignment.Center;
                    Invoice_cashier_footer.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                    Invoice_cashier_footer.Margin = new Thickness(0);
                    doc.Blocks.Add(Invoice_cashier_footer); 

            }
            if (billFmt == "NormalBill")
            {
                #region Invoice Header

                Paragraph Invoice_Header = new Paragraph();
                byte[] binaryData = Convert.FromBase64String(ad_org_logo);

                BitmapImage ad_org_logo_bit = new BitmapImage();
                try
                {
                    ad_org_logo_bit.BeginInit();
                    ad_org_logo_bit.StreamSource = new MemoryStream(binaryData);
                    ad_org_logo_bit.EndInit();
                }
                catch (Exception)
                {

                }
                Image logo = new Image() { Source = ad_org_logo_bit };
                logo.Height = 60;
                logo.Width = 150;
                s = new Span(new Run(""));
                s.Inlines.Add(new LineBreak());
                s.Inlines.Add(new LineBreak());
                Invoice_Header.Inlines.Add(logo);
                Invoice_Header.Inlines.Add(s);

                if (ad_org_arabicname != String.Empty)
                {
                    s = new Span(new Run(ad_org_arabicname));
                    s.Inlines.Add(new LineBreak());
                    s.FontWeight = FontWeights.Bold;
                    Invoice_Header.Inlines.Add(s);
                }
                s = new Span(new Run(ad_org_name));
                s.Inlines.Add(new LineBreak());
                s.FontWeight = FontWeights.Bold;
                Invoice_Header.Inlines.Add(s);

                s = new Span(new Run(Invoice_Print.m_warehouse_name + " - " + ad_org_city));
                s.Inlines.Add(new LineBreak());
                Invoice_Header.Inlines.Add(s);
                s = new Span(new Run(" Tel: " + ad_org_phone));
                s.Inlines.Add(new LineBreak());
                Invoice_Header.Inlines.Add(s);
                if (Duplicate)
                {
                    s = new Span(new Run("Duplicate Copy"));
                    s.Inlines.Add(new LineBreak());
                    s.FontSize = 16;
                    Invoice_Header.Inlines.Add(s);
                }
                else
                {
                    if (Invoice_Print.iscomplementary == "Y" || Invoice_Print.iscredit == "Y")
                    {
                        if (Invoice_Print.iscomplementary == "Y")
                        {
                            s = new Span(new Run("Complementary Bill"));
                            s.Inlines.Add(new LineBreak());
                            s.FontSize = 16;
                            Invoice_Header.Inlines.Add(s);
                            formattedVal = ConfigurationManager.AppSettings.Get("lblComplement");
                            s = new Span(new Run(formattedVal));
                            s.Inlines.Add(new LineBreak());
                            s.FontSize = 16;
                            Invoice_Header.Inlines.Add(s);
                        }
                        if (Invoice_Print.iscredit == "Y")
                        {
                            s = new Span(new Run("Credit Bill"));
                            s.Inlines.Add(new LineBreak());
                            s.FontSize = 16;
                            Invoice_Header.Inlines.Add(s);
                            formattedVal = ConfigurationManager.AppSettings.Get("lblCreditBill");
                            s = new Span(new Run(formattedVal));
                            s.Inlines.Add(new LineBreak());
                            s.FontSize = 16;
                            Invoice_Header.Inlines.Add(s);
                        }
                    }
                    else if(Invoice_Print.is_return=="Y")
                    {
                        s = new Span(new Run("Credit Memo"));
                        s.Inlines.Add(new LineBreak());
                        s.FontSize = 16;
                        Invoice_Header.Inlines.Add(s);
                        formattedVal = ConfigurationManager.AppSettings.Get("lblCreditMemo");
                        s = new Span(new Run(formattedVal));
                        s.Inlines.Add(new LineBreak());
                        s.FontSize = 16;
                        Invoice_Header.Inlines.Add(s);
                    }
                    else
                    {
                        s = new Span(new Run("Cash Bill"));
                        s.Inlines.Add(new LineBreak());
                        s.FontSize = 16;
                        Invoice_Header.Inlines.Add(s);
                        formattedVal = ConfigurationManager.AppSettings.Get("lblCashBill");
                        s = new Span(new Run(formattedVal));
                        s.Inlines.Add(new LineBreak());
                        s.FontSize = 16;
                        Invoice_Header.Inlines.Add(s);
                    }
                }

                Invoice_Header.Inlines.Add(s);
                Invoice_Header.FontSize = 14;
                Invoice_Header.FontStyle = FontStyles.Normal;
                Invoice_Header.TextAlignment = TextAlignment.Center;
                Invoice_Header.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                doc.Blocks.Add(Invoice_Header);

                #endregion Invoice Header

                #region Invoice Header Lines

                Paragraph Invoice_Header_Lines = new Paragraph();

                Table oTable = new Table();

                // Append the table to the document
                oTable.CellSpacing = 0;
                oTable.Padding = new Thickness(0);
                oTable.FontSize = 14;
                oTable.FontStyle = FontStyles.Normal;
                oTable.TextAlignment = System.Windows.TextAlignment.Center;
                oTable.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                oTable.Margin = new Thickness(0);
                doc.Blocks.Add(oTable);
                 
                oTable.RowGroups.Add(new TableRowGroup()); 
                oTable.RowGroups[0].Rows.Add(new TableRow()); 

                TableRow currentRow = oTable.RowGroups[0].Rows[0];

                // Add the header row with content,
                Paragraph pBillno = new Paragraph();
                pBillno.TextAlignment = TextAlignment.Left;
                string Bill_No = "Bill No : " + invoice_no;
                s = new Span(new Run(Bill_No));
                pBillno.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pBillno));
                Paragraph pdate = new Paragraph();
                pdate.TextAlignment = TextAlignment.Right;
                string Date = "Date : " + DateTime.Now.ToString("dd/MM/yyyy");
                s = new Span(new Run(Date));
                pdate.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pdate));
                oTable.RowGroups[0].Rows.Add(new TableRow());

                currentRow = oTable.RowGroups[0].Rows[1];

                //Add the country name in the first cell
                Paragraph pName = new Paragraph();
                pName.TextAlignment = TextAlignment.Left;
                string Cashiername = "Cashier  : " + user_name;
                s = new Span(new Run(Cashiername));
                pName.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pName));


                Paragraph pTime = new Paragraph();
                pTime.TextAlignment = TextAlignment.Right;
                string Time = "Time : " + DateTime.Now.ToShortTimeString() + "     "; 
                s = new Span(new Run(Time));
                pTime.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pTime));



                //string Bill_No = "Bill No   : " + invoice_no;
                //int Bill_No_l = Bill_No.Length;
                //string Date = "Date : " + DateTime.Now.ToString("dd/MM/yyyy");
                //int Date_l = Date.Length;
                //int Bill_No_Date_sp = 64 - (Bill_No_l + Date_l);
                //if (invoice_no.Length == 7)
                //    Bill_No_Date_sp = 28;
                //if (invoice_no.Length == 8)
                //    Bill_No_Date_sp = 26;
                //if (invoice_no.Length == 9)
                //    Bill_No_Date_sp = 24;
                //if (invoice_no.Length == 10)
                //    Bill_No_Date_sp = 22;
                //s = new Span(new Run(Bill_No + Date.PadLeft(28)));
                ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
                //s.Inlines.Add(new LineBreak());
                //Invoice_Header_Lines.Inlines.Add(s);

                //string Cashier = "Cashier  : " + user_name;
                //int Cashier_l = Cashier.Length;
                //string Time = "Time : " + DateTime.Now.ToShortTimeString();
                //int Time_l = Time.Length;
                //int Cashier_Time_sp = 68 - (Cashier_l + Time_l);
                //s = new Span(new Run(Cashier + Time.PadLeft(28)));
                ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
                //s.Inlines.Add(new LineBreak());
                //Invoice_Header_Lines.Inlines.Add(s);
                 string Customername = "Customer Name  : " + Invoice_Print.c_bpartner_name;

                 s = new Span(new Run(Customername));
                ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
                 s.Inlines.Add(new LineBreak());
                 Invoice_Header_Lines.Inlines.Add(s);
                s = new Span(new Run("-----------------------------------------------"));//47
                s.Inlines.Add(new LineBreak());
                Invoice_Header_Lines.Inlines.Add(s);
                s = new Span(new Run("Item\t\t\t\t  Price"));
                s.Inlines.Add(new LineBreak());
                Invoice_Header_Lines.Inlines.Add(s);
                formattedVal = ConfigurationManager.AppSettings.Get("lblItem") + "\t\t\t\t" + ConfigurationManager.AppSettings.Get("lblPrice");
                s = new Span(new Run(formattedVal));
                s.Inlines.Add(new LineBreak());
                Invoice_Header_Lines.Inlines.Add(s);
                s = new Span(new Run("-----------------------------------------------"));//47
                                                                                         //s.Inlines.Add(new LineBreak());
                Invoice_Header_Lines.Inlines.Add(s);

                //Give style and formatting to paragraph content.
                Invoice_Header_Lines.FontSize = 14;
                Invoice_Header_Lines.FontStyle = FontStyles.Normal;
                Invoice_Header_Lines.TextAlignment = System.Windows.TextAlignment.Left;
                Invoice_Header_Lines.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                Invoice_Header_Lines.Margin = new Thickness(0);
                doc.Blocks.Add(Invoice_Header_Lines);
               // s = new Span(new Run("-----------------------------------------------"));//47
               // Invoice_Header_Lines.Inlines.Add(s);
                doc.Blocks.Add(Invoice_Header_Lines);

                #endregion Invoice Header Lines

                #region OrderList

                double subtotal = 0;
                double subtotal_dis = 0;
                Section OrderDetails_sec = new Section();
                OrderDetails_Print.ForEach(x =>
                {
                    Paragraph OrderDetails = new Paragraph();

                    s = new Span(new Run(x.productSearchKey));
                    s.Inlines.Add(new LineBreak());
                    OrderDetails.Inlines.Add(s);
                    s = new Span(new Run(x.productName));

                    if (x.productArabicName != String.Empty )
                    {
                        if(  x.productArabicName != " ")
                        {
                            s.Inlines.Add(new LineBreak());
                            OrderDetails.Inlines.Add(s);
                            s = new Span(new Run(x.productArabicName));
                            //s.Inlines.Add(new LineBreak());

                        }

                    }
                    OrderDetails.Inlines.Add(s);
                    OrderDetails.Margin = new Thickness(0);
                    OrderDetails.FontSize = 14;
                    OrderDetails.FontStyle = FontStyles.Normal;
                    OrderDetails.TextAlignment = TextAlignment.Left;
                    OrderDetails.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                    OrderDetails_sec.Blocks.Add(OrderDetails);

                    Paragraph OrderDetails_price = new Paragraph();
                    int print_spacing = 20;
                    //int _p = Convert.ToDouble(x.price).ToString("0.00").Length;
                    //string price = Convert.ToDouble(x.price).ToString("0.00");
                    int _p = (Convert.ToDouble(x.qty) * Convert.ToDouble(x.actualPrice)).ToString("0.00").Length;
                    string price = (Convert.ToDouble(x.qty) * Convert.ToDouble(x.actualPrice)).ToString("0.00");

                    int a = print_spacing - _p;
                    s = new Span(new Run(x.qty + " " + x.uomName + " x " + Convert.ToDouble(x.actualPrice).ToString("0.00") + "  " + price.PadLeft(a)));//.PadLeft(a)
                                                                                                                                                        //s.Inlines.Add(new LineBreak());
                    OrderDetails_price.Inlines.Add(s);
                    OrderDetails_price.Margin = new Thickness(0);
                    OrderDetails_price.FontSize = 14;
                    OrderDetails_price.FontStyle = FontStyles.Normal;
                    OrderDetails_price.TextAlignment = TextAlignment.Right;
                    OrderDetails_price.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                    OrderDetails_sec.Blocks.Add(OrderDetails_price);
                    subtotal += Convert.ToDouble(x.actualPrice) * Convert.ToDouble(x.qty);
                    subtotal_dis += (Convert.ToDouble(x.actualPrice) * Convert.ToDouble(x.qty)) - Convert.ToDouble(x.price);
                });
                doc.Blocks.Add(OrderDetails_sec);

                #endregion OrderList

                #region Payment_Details

                Paragraph Payment_Details = new Paragraph();
                s = new Span(new Run("-----------------------------------------------"));//47
                s.Inlines.Add(new LineBreak());
                Payment_Details.Inlines.Add(s);
                int Payment_Details_sp = 40;
                string LRM = ((char)0x200E).ToString();  // This is a LRM
                if (Invoice_Print.iscomplementary != "Y" && Invoice_Print.iscredit != "Y")
                {
                    string Sub_Total = "Sub Total";
                    int Sub_Total_l = Sub_Total.Length;
                    //string Sub_Total_val = Convert.ToDouble(Invoice_Print.grandtotal_round_off).ToString("0.00");
                    string Sub_Total_val = subtotal.ToString("0.00");
                    int Sub_Total_val_l = Sub_Total_val.Length;
                    int Sub_Total_sp = Payment_Details_sp - (Sub_Total_l + Sub_Total_val_l) - 6;
                    s = new Span(new Run(Sub_Total + Sub_Total_val.PadLeft(Sub_Total_sp)));
                    //s = new Span(new Run("Sub Total" + (Convert.ToDouble(Invoice_Print.grandtotal_round_off).ToString("0.00")).PadLeft(20)));
                    s.Inlines.Add(new LineBreak());
                    Payment_Details.Inlines.Add(s);
                    // formattedVal = ConfigurationManager.AppSettings.Get("lblSubTotal"); 
                    Sub_Total = ConfigurationManager.AppSettings.Get("lblSubTotal");
                    s = new Span(new Run(Sub_Total + LRM + Sub_Total_val.PadLeft(Sub_Total_sp)));
                    //s = new Span(new Run("Sub Total" + (Convert.ToDouble(Invoice_Print.grandtotal_round_off).ToString("0.00")).PadLeft(20)));
                    s.Inlines.Add(new LineBreak());
                    Payment_Details.Inlines.Add(s);

                    if (subtotal_dis > 0)
                    {
                        string Discount = "Discount";
                        int Discount_l = Discount.Length;
                        //string Discount_val = Convert.ToDouble(Invoice_Print.grandtotal_round_off).ToString("0.00");
                        string Discount_val = subtotal_dis.ToString("-0.00");
                        int Discount_val_l = Discount_val.Length;
                        int Discount_sp = Payment_Details_sp - (Discount_l + Discount_val_l) - 6;
                        s = new Span(new Run(Discount + Discount_val.PadLeft(Discount_sp)));
                        //s = new Span(new Run("Sub Total" + (Convert.ToDouble(Invoice_Print.grandtotal_round_off).ToString("0.00")).PadLeft(20)));
                        s.Inlines.Add(new LineBreak());
                        Payment_Details.Inlines.Add(s);

                        Discount = ConfigurationManager.AppSettings.Get("lblDiscount");
                        s = new Span(new Run(Discount + LRM + Discount_val.PadLeft(Discount_sp)));
                        //s = new Span(new Run("Sub Total" + (Convert.ToDouble(Invoice_Print.grandtotal_round_off).ToString("0.00")).PadLeft(20)));
                        s.Inlines.Add(new LineBreak());
                        Payment_Details.Inlines.Add(s);
                    }

                    if (Convert.ToDouble(Invoice_Print.lossamount) > 0)
                    {
                        string Rounding_Discount = "Rounding Discount";
                        int Rounding_Discount_l = Rounding_Discount.Length;
                        string Rounding_Discount_val = "-" + Convert.ToDouble(Invoice_Print.lossamount).ToString("0.00");
                        int Rounding_Discount_val_l = Rounding_Discount_val.Length;
                        int Rounding_Discount_sp = Payment_Details_sp - (Rounding_Discount_l + Rounding_Discount_val_l) + 3;
                        s = new Span(new Run(Rounding_Discount + Rounding_Discount_val.PadLeft(Rounding_Discount_sp)));
                        //s = new Span(new Run("Rounding Discount" + (Convert.ToDouble(Invoice_Print.lossamount).ToString("-0.00")).PadLeft(20)));
                        s.Inlines.Add(new LineBreak());
                        Payment_Details.Inlines.Add(s);
                        Rounding_Discount = ConfigurationManager.AppSettings.Get("lblDiscount");
                        s = new Span(new Run(Rounding_Discount + LRM + Rounding_Discount_val.PadLeft(Rounding_Discount_sp)));
                        //s = new Span(new Run("Rounding Discount" + (Convert.ToDouble(Invoice_Print.lossamount).ToString("-0.00")).PadLeft(20)));
                        s.Inlines.Add(new LineBreak());
                        Payment_Details.Inlines.Add(s);
                    }

                }
                string Grand_Total = "Grand Total";
                int Grand_Total_l = Grand_Total.Length;
                string Grand_Total_val = Convert.ToDouble(Invoice_Print.grandtotal).ToString("0.00");
                int Grand_Total_val_l = Grand_Total_val.Length;
                int Grand_Total_sp = Payment_Details_sp - (Grand_Total_l + Grand_Total_val_l) - 4;
                s = new Span(new Run(Grand_Total + Grand_Total_val.PadLeft(Grand_Total_sp)));
                //s = new Span(new Run("Grand Total" + (Convert.ToDouble(Invoice_Print.grandtotal).ToString("0.00")).PadLeft(20)));
                s.Inlines.Add(new LineBreak());
                Payment_Details.Inlines.Add(s);
                Grand_Total = ConfigurationManager.AppSettings.Get("lblGrandTotal");
                s = new Span(new Run(Grand_Total + LRM + Grand_Total_val.PadLeft(Grand_Total_sp)));
                //s = new Span(new Run("Grand Total" + (Convert.ToDouble(Invoice_Print.grandtotal).ToString("0.00")).PadLeft(20)));
                s.Inlines.Add(new LineBreak());
                Payment_Details.Inlines.Add(s);
                s = new Span(new Run("-----------------------------------------------"));//47
                                                                                         //s.Inlines.Add(new LineBreak());
                Payment_Details.Inlines.Add(s);
                Payment_Details.Margin = new Thickness(0);
                Payment_Details.FontSize = 14;
                Payment_Details.FontWeight = FontWeights.Bold;
                Payment_Details.FontStyle = FontStyles.Normal;
                Payment_Details.TextAlignment = TextAlignment.Right;
                Payment_Details.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                doc.Blocks.Add(Payment_Details);

                #endregion Payment_Details

                #region item_qy

                Paragraph item_qy = new Paragraph();
                s = new Span(new Run("No of Items : " + OrderDetails_Print.Count + "\t" + "Total Quantity : " + Invoice_Print.total_items_count));//47
                                                                                                                                                  // s.Inlines.Add(new LineBreak());
                item_qy.Inlines.Add(s);
                item_qy.Margin = new Thickness(0);
                item_qy.FontSize = 14;
                item_qy.FontStyle = FontStyles.Normal;
                item_qy.TextAlignment = TextAlignment.Center;
                item_qy.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                doc.Blocks.Add(item_qy);
                Paragraph item_qy1 = new Paragraph();
                s = new Span(new Run(ConfigurationManager.AppSettings.Get("lblNoOfItems") + LRM + "  : " + OrderDetails_Print.Count + "\t" + ConfigurationManager.AppSettings.Get("lblTotalQuantity") + LRM + " : " + Invoice_Print.total_items_count));//47
                s.Inlines.Add(new LineBreak());
                item_qy1.Inlines.Add(s);
                item_qy1.Margin = new Thickness(0);
                item_qy1.FontSize = 14;
                item_qy1.FontStyle = FontStyles.Normal;
                item_qy1.TextAlignment = TextAlignment.Center;
                item_qy1.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                item_qy1.Inlines.Add(s);
                doc.Blocks.Add(item_qy1);
                #endregion item_qy

                if (Invoice_Print.is_return != "Y")
                {
                    #region Payment_Method

                    int Payment_Method_sp = 66;
                    Paragraph Payment_Method = new Paragraph();
                    if (Invoice_Print.iscomplementary != "Y" && Invoice_Print.iscredit != "Y")
                    {
                        if (Invoice_Print.cash != "0")
                        {
                            string cash = "Cash";
                            int cash_l = cash.Length;
                            string cash_val = Convert.ToDouble(Invoice_Print.cash).ToString("0.00");
                            int cash_val_l = cash_val.Length;
                            int cash_sp = Payment_Method_sp - (cash_l + cash_val_l) + 1;
                            s = new Span(new Run(cash + cash_val.PadLeft(cash_sp)));
                            Payment_Method.Inlines.Add(s);
                            cash = ConfigurationManager.AppSettings.Get("lblCash");
                            cash_l = cash.Length;
                            cash_val = Convert.ToDouble(Invoice_Print.cash).ToString("0.00");
                            cash_val_l = cash_val.Length;
                            cash_sp = 67 - (cash_l + cash_val_l) + 1;
                            s = new Span(new Run(cash + LRM + cash_val.PadLeft(cash_sp)));
                            s.Inlines.Add(new LineBreak());
                            Payment_Method.Inlines.Add(s);
                        }
                        if (Invoice_Print.card != "0")
                        {
                            string card = "Card";
                            int card_l = card.Length;
                            string card_val = Convert.ToDouble(Invoice_Print.card).ToString("0.00");
                            int card_val_l = card_val.Length;
                            int card_sp = Payment_Method_sp - (card_l + card_val_l) + 1;
                            s = new Span(new Run(card + card_val.PadLeft(card_sp)));
                            Payment_Method.Inlines.Add(s);
                            card = ConfigurationManager.AppSettings.Get("lblCard");
                            card_l = card.Length + 4;
                            card_val_l = card_val.Length;
                            card_sp = 67 - (card_l + card_val_l) + 1;
                            s = new Span(new Run(card + card_val.PadLeft(card_sp)));
                            s = new Span(new Run(card + LRM + card_val.PadLeft(card_sp)));
                            s.Inlines.Add(new LineBreak());
                            Payment_Method.Inlines.Add(s);
                        }
                        if (Invoice_Print.redemption != "0")
                        {
                            string redemption = "Redemption";
                            int redemption_l = redemption.Length;
                            string redemption_val = Convert.ToDouble(Invoice_Print.redemption).ToString("0.00");
                            int redemption_val_l = redemption_val.Length;
                            int redemption_sp = Payment_Method_sp - (redemption_l + redemption_val_l) - 5;
                            s = new Span(new Run(redemption + redemption_val.PadLeft(redemption_sp)));
                            Payment_Method.Inlines.Add(s);
                            redemption = ConfigurationManager.AppSettings.Get("lblRedeem");
                            s = new Span(new Run(redemption + LRM + redemption_val.PadLeft(redemption_sp)));
                            s.Inlines.Add(new LineBreak());
                            Payment_Method.Inlines.Add(s);
                        }
                        if (Invoice_Print.exchange != "0")
                        {
                            string exchange = "Exchange";
                            int exchange_l = exchange.Length;
                            string exchange_val = Convert.ToDouble(Invoice_Print.exchange).ToString("0.00");
                            int exchange_val_l = exchange_val.Length;
                            int exchange_sp = Payment_Method_sp - (exchange_l + exchange_val_l) - 3;
                            s = new Span(new Run(exchange + exchange_val.PadLeft(exchange_sp)));
                            Payment_Method.Inlines.Add(s);
                            exchange = ConfigurationManager.AppSettings.Get("lblExchange");
                            s = new Span(new Run(exchange + LRM + exchange_val.PadLeft(exchange_sp)));
                            s.Inlines.Add(new LineBreak());
                            Payment_Method.Inlines.Add(s);
                        }
                        s = new Span(new Run("-----------------------------------------------"));//47
                        s.Inlines.Add(new LineBreak());
                        Payment_Method.Inlines.Add(s);

                        string Paid = "Paid";
                        int Paid_l = Paid.Length;
                        string Paid_val = (Convert.ToDouble(Invoice_Print.cash) + Convert.ToDouble(Invoice_Print.card) + Convert.ToDouble(Invoice_Print.redemption) + Convert.ToDouble(Invoice_Print.exchange)).ToString("0.00");
                        int Paid_val_l = Paid_val.Length;
                        int Paid_sp = 67 - (Paid_l + Paid_val_l) + 1;
                        s = new Span(new Run(Paid + Paid_val.PadLeft(Paid_sp)));
                        Payment_Method.Inlines.Add(s);
                        int Paid_sp1 = 66 - (Paid_l + Paid_val_l) + 1;
                        Paid = ConfigurationManager.AppSettings.Get("lblPaid");
                        s = new Span(new Run(Paid + LRM + Paid_val.PadLeft(Paid_sp1)));
                        s.Inlines.Add(new LineBreak());
                        Payment_Method.Inlines.Add(s);

                        string change = "Change";
                        int change_l = change.Length;
                        string change_val = Convert.ToDouble(Invoice_Print.change).ToString("0.00");
                        int change_val_l = change_val.Length;
                        int change_sp = 66 - (change_l + change_val_l) - 2;
                        s = new Span(new Run(change + change_val.PadLeft(change_sp)));
                        Payment_Method.Inlines.Add(s);
                        change = ConfigurationManager.AppSettings.Get("lblChange");
                        change_l = change.Length - 3;
                        change_val = Convert.ToDouble(Invoice_Print.change).ToString("0.00");
                        change_val_l = change_val.Length;
                        change_sp = 67 - (change_l + change_val_l) - 2;
                        s = new Span(new Run(change + LRM + change_val.PadLeft(change_sp)));
                        s.Inlines.Add(new LineBreak());
                        Payment_Method.Inlines.Add(s);
                    }
                    else
                    {
                        s = new Span(new Run("-----------------------------------------------"));//47
                        s.Inlines.Add(new LineBreak());
                        Payment_Method.Inlines.Add(s);

                        if (Invoice_Print.iscomplementary == "Y")
                        {
                            string Complementary = "COMPLEMENTARY";
                            s = new Span(new Run("-------------" + Complementary + "-------------"));//15 
                            Payment_Method.Inlines.Add(s);
                            Complementary = ConfigurationManager.AppSettings.Get("lblComplement");
                            s = new Span(new Run("-------------" + Complementary + LRM + "-------------"));//15
                            s.Inlines.Add(new LineBreak());
                            Payment_Method.Inlines.Add(s);
                        }
                        if (Invoice_Print.iscredit == "Y")
                        {
                            string credit = "CREDIT";
                            s = new Span(new Run("-------------------" + credit + "--------------------"));//15 
                            credit = ConfigurationManager.AppSettings.Get("lblComplement");
                            s = new Span(new Run("-------------------" + credit + LRM + "--------------------"));//15
                            s.Inlines.Add(new LineBreak());
                            Payment_Method.Inlines.Add(s);
                        }
                    }

                    s = new Span(new Run("-----------------------------------------------"));//47
                    s.Inlines.Add(new LineBreak());
                    Payment_Method.Inlines.Add(s);
                    Payment_Method.Margin = new Thickness(0);
                    Payment_Method.FontSize = 14;
                    Payment_Method.FontStyle = FontStyles.Normal;
                    Payment_Method.TextAlignment = TextAlignment.Left;
                    Payment_Method.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                    doc.Blocks.Add(Payment_Method);

                    #endregion Payment_Method
                }

                #region Summary

                Paragraph Summary = new Paragraph();
                if (ad_org_footermessage != String.Empty)
                {
                    s = new Span(new Run(ad_org_footermessage));
                    Summary.Inlines.Add(s);
                    s.Inlines.Add(new LineBreak());
                    s = new Span(new Run(ConfigurationManager.AppSettings.Get("lblThanks")));
                    s.Inlines.Add(new LineBreak());
                    Summary.Inlines.Add(s);
                }

                //if (ad_org_arabicfootermessage != String.Empty)
                //{
                //    s = new Span(new Run(ad_org_arabicfootermessage));
                //    s.Inlines.Add(new LineBreak());
                //    Summary.Inlines.Add(s);
                //}
                if (ad_org_termsmessage != String.Empty)
                {
                    s = new Span(new Run(ad_org_termsmessage));
                    s.Inlines.Add(new LineBreak());
                    Summary.Inlines.Add(s);
                }
                if (ad_org_arabictermsmessage != String.Empty)
                {
                    s = new Span(new Run(ad_org_arabictermsmessage));
                    s.Inlines.Add(new LineBreak());
                    Summary.Inlines.Add(s);
                }
                Summary.Margin = new Thickness(0);
                Summary.FontSize = 14;
                Summary.FontStyle = FontStyles.Normal;
                Summary.TextAlignment = TextAlignment.Center;
                Summary.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                doc.Blocks.Add(Summary);

                #endregion Summary

                #region barcode

                Paragraph barcode = new Paragraph();
                /////////////////////////////////////
                // Encode The Data
                /////////////////////////////////////
                Barcodes bb = new Barcodes();
                bb.BarcodeType = Barcodes.BarcodeEnum.Code39;
                bb.Data = invoice_no;
                bb.CheckDigit = Barcodes.YesNoEnum.Yes;
                bb.encode();
                int thinWidth;
                int thickWidth;
                thinWidth = 2;
                thickWidth = 2 * thinWidth;
                string outputString = bb.EncodedData;
                string humanText = bb.HumanText;

                Canvas c = new Canvas();
                c.Margin = new Thickness(0);
                c.Height = 60;
                /////////////////////////////////////
                // Draw The Barcode
                /////////////////////////////////////
                int len = outputString.Length;
                int currentPos = 10;
                int currentTop = 10;
                int currentColor = 0;
                if (invoice_no.Length >= 10)
                {
                    currentPos = 30;
                    thinWidth = 1;
                    thickWidth = 2;
                }
                for (int i = 0; i < len; i++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Height = 40;
                    if (currentColor == 0)
                    {
                        currentColor = 1;
                        rect.Fill = new SolidColorBrush(Colors.Black);
                    }
                    else
                    {
                        currentColor = 0;
                        rect.Fill = new SolidColorBrush(Colors.White);
                    }
                    Canvas.SetLeft(rect, currentPos);
                    Canvas.SetTop(rect, currentTop);

                    if (outputString[i] == 't')
                    {
                        rect.Width = thinWidth;
                        currentPos += thinWidth;
                    }
                    else if (outputString[i] == 'w')
                    {
                        rect.Width = thickWidth;
                        currentPos += thickWidth;
                    }
                    c.Children.Add(rect);
                }
                /////////////////////////////////////
                // Add the Human Readable Text
                /////////////////////////////////////
                TextBlock tb = new TextBlock();
                tb.Text = humanText;
                tb.FontSize = 16;
                tb.FontWeight = FontWeights.Bold;
                tb.FontFamily = new FontFamily("Courier New");
                Rect rx = new Rect(0, 0, 0, 0);
                tb.Arrange(rx);
                Canvas.SetLeft(tb, (currentPos - tb.ActualWidth) / 2);
                Canvas.SetTop(tb, currentTop + 51);
                c.Children.Add(tb);
                barcode.Inlines.Add(c);
                //s = new Span(new Run(" "));//47
                //s.Inlines.Add(new LineBreak());
                //s.Inlines.Add(new LineBreak());
                //barcode.Inlines.Add(s);
                barcode.Margin = new Thickness(0);
                barcode.FontSize = 14;
                barcode.FontStyle = FontStyles.Normal;
                barcode.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                doc.Blocks.Add(barcode);

                #endregion barcode

                Paragraph invoice_barcode = new Paragraph();
                //s = new Span(new Run("*" + invoice_no + "*"));//47
                s = new Span(new Run(" "));//47
                s.Inlines.Add(new LineBreak());
                s.Inlines.Add(new LineBreak());
                invoice_barcode.Inlines.Add(s);
                s = new Span(new Run("."));//47
                invoice_barcode.Inlines.Add(s);
                invoice_barcode.Margin = new Thickness(0);
                invoice_barcode.FontSize = 14;
                invoice_barcode.FontStyle = FontStyles.Normal;
                invoice_barcode.TextAlignment = TextAlignment.Center;
                invoice_barcode.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                doc.Blocks.Add(invoice_barcode);
            }
               
                   
          

            doc.Name = "InvoiceNo_" + invoice_no;
            doc.PageWidth = 270;
            doc.PagePadding = new Thickness(1);

            IDocumentPaginatorSource idpSource = doc;
            try
            {

                SerialPort.OpenDrawer();
                printDlg.PrintDocument(idpSource.DocumentPaginator, $"InvoiceNo_{invoice_no}");
            }
            catch
            {
                NotifierViewModel.Notifier.ShowError(" Printer Not Connected");
                OrderDetails_Print.Clear();
                return;
            }
            #endregion Print

            //--------------------------------------------------------------------------------------------------------------------------------//
            //-----------------------------                          PRINT FUNCTION END                ---------------------------------------//
            //--------------------------------------------------------------------------------------------------------------------------------//

            //Clearing Memory
            OrderDetails_Print.Clear();
        }
         //ekta
        public static void Get_SalesSummery_And_print(string Session_Start, string Sessionformattedstartdate, string longSessionEnd, string Session_End, Boolean Duplicate, string txt_Purchases, string txt_Expenses, string AD_Client_ID, string AD_ORG_ID, string AD_Warehouse_Id, string AD_USER_ID, string openingBalance,
            string SessionClose_500x_total, string SessionClose_100x_total, string SessionClose_50x_total, string SessionClose_10x_total, string SessionClose_5x_total, string SessionClose_1x_total, string SessionClose_50dx_total, string SessionClose_25dx_total, string SessionClose_grand_total, string saleType, string onlyTotal, string SessionClose_500x_input, string SessionClose_100x_input, string SessionClose_50x_input, string SessionClose_10x_input, string SessionClose_5x_input, string SessionClose_1x_input, string SessionClose_50dx_input, string SessionClose_25dx_input)
        {

            if (SessionClose_500x_input == "") { SessionClose_500x_input = "0"; }

            if (SessionClose_100x_input == "") { SessionClose_100x_input = "0"; } 
            if (SessionClose_50x_input == "") { SessionClose_50x_input = "0"; } 
            if (SessionClose_10x_input == "") { SessionClose_10x_input = "0"; }
            if (SessionClose_5x_input == "") { SessionClose_5x_input = "0"; } 
            if(SessionClose_1x_input=="")  { SessionClose_1x_input = "0"; }
            if(SessionClose_50dx_input=="")  { SessionClose_50dx_input = "0"; }
            if (SessionClose_25dx_input=="") { SessionClose_25dx_input = "0"; }

            NpgsqlConnection connection = new NpgsqlConnection(PostgreSQL.ConnectionString);
            //Getting Last 5 Invoice Which is Complected and Not Posted
            connection.Open();
            NpgsqlCommand cmd_c_invoic_SalesSummery = new NpgsqlCommand("select COALESCE(sum(h.grandtotal),0)  totalsales,COALESCE(sum(d.cash),0) Cash,COALESCE(sum(d.card),0) Bank,COALESCE(sum(d.exchange),0) exchange, " +
            " COALESCE(sum(d.redemption),0)   redemption, COALESCE(sum(h.openingbalance),0) as openingbalance, h.ad_client_id, h.ad_org_id,h.m_warehouse_id,h.ad_user_id from c_invoice h, c_invoicepaymentdetails d " +
            "where  is_completed = 'Y' AND iscredit = 'N' AND iscomplementary='N' AND h.c_invoice_id = d.c_invoice_id AND  h.ad_client_id=d.ad_client_id "+
            "AND h.ad_org_id = d.ad_org_id  AND h.session_id = " + Session_Start + " group by h.ad_client_id, h.ad_org_id,h.m_warehouse_id,h.ad_user_id; ", connection);

            NpgsqlDataReader _get_c_invoic_SalesSummery = cmd_c_invoic_SalesSummery.ExecuteReader();
            Sales_Summery sales_Summery;
            if (_get_c_invoic_SalesSummery.HasRows == false)
            {
                // Console.WriteLine("Error");
                log.Info("Session Not Found ID:" + Session_Start);
                connection.Close();


                sales_Summery = new Sales_Summery()
                {
                    totalSales = "0.00",
                    cash = "0.00",
                    bank = "0.00",
                    exchange = "0.00",
                    redemption = "0.00",
                    openingBalance = openingBalance,
                    ad_client_Id = AD_Client_ID,
                    ad_org_id = AD_ORG_ID,
                    warehouse_Id = AD_Warehouse_Id,
                    ad_user_Id = AD_USER_ID,
                };

            }
            else
            {


                _get_c_invoic_SalesSummery.Read();
                sales_Summery = new Sales_Summery()
                {
                    totalSales = Convert.ToDouble(_get_c_invoic_SalesSummery.GetDouble(0)).ToString("0.00"),
                  //  cash = _get_c_invoic_SalesSummery.GetDouble(1).ToString(),
                    bank = _get_c_invoic_SalesSummery.GetDouble(2).ToString(),
                    exchange = _get_c_invoic_SalesSummery.GetDouble(3).ToString(),
                    redemption = _get_c_invoic_SalesSummery.GetDouble(4).ToString(),
                    openingBalance = Convert.ToDouble(_get_c_invoic_SalesSummery.GetDouble(5)).ToString("0.00"),
                    ad_client_Id = _get_c_invoic_SalesSummery.GetInt64(6).ToString(),
                    ad_org_id = _get_c_invoic_SalesSummery.GetInt64(7).ToString(),
                    warehouse_Id = _get_c_invoic_SalesSummery.GetInt64(8).ToString(),
                    ad_user_Id = _get_c_invoic_SalesSummery.GetInt64(9).ToString(),
                };
                connection.Close();
            }
            string name = "";
            string ad_org_name = "";
            string ad_org_logo = "";
            string warehouse = "";
            string password = "";
            connection.Close();
            //MessageBox.Show(sales_Summery.openingBalance);
            connection.Open();
            NpgsqlCommand cmd_c_invoic_ad_user_pos = new NpgsqlCommand("SELECT " +
                "name,password " +             //0
                "FROM ad_user_pos " +
                "WHERE ad_client_id = " + sales_Summery.ad_client_Id + " AND ad_org_id = " + sales_Summery.ad_org_id + " AND ad_user_id = " + sales_Summery.ad_user_Id + " ; ", connection);
            NpgsqlDataReader _get_c_invoic_ad_user_pos = cmd_c_invoic_ad_user_pos.ExecuteReader();

            _get_c_invoic_ad_user_pos.Read();
            name = _get_c_invoic_ad_user_pos.GetString(0);
            password = _get_c_invoic_ad_user_pos.GetString(1);
            connection.Close();
            ////credit,complement,discount,
            string credit = "0.00", complement = "0.00", discount = "0.00",sale_return="0.00",Dis_round_off="0.00";
            connection.Open();
            cmd_c_invoic_SalesSummery = new NpgsqlCommand("select COALESCE(sum(d.cash),0) Cash" +
           "  from c_invoice h, c_invoicepaymentdetails d " +
           "where  is_completed = 'Y' AND iscredit = 'N' AND iscomplementary='N' AND is_return='N'  AND h.c_invoice_id = d.c_invoice_id   AND  h.ad_client_id=d.ad_client_id " +
            "AND h.ad_org_id = d.ad_org_id AND h.session_id = " + Session_Start + ";", connection);
            _get_c_invoic_SalesSummery = cmd_c_invoic_SalesSummery.ExecuteReader();
            if (_get_c_invoic_SalesSummery.HasRows == true)
            {
                _get_c_invoic_SalesSummery.Read();
               sales_Summery.cash = Convert.ToDouble(_get_c_invoic_SalesSummery.GetDouble(0)).ToString("0.00");

            }
            connection.Close();

            connection.Open();
            cmd_c_invoic_SalesSummery = new NpgsqlCommand("select COALESCE(sum(d.cash),0) Cash" +
           "  from c_invoice h, c_invoicepaymentdetails d " +
           "where  is_completed = 'Y' AND iscredit = 'Y' AND iscomplementary='N' AND h.c_invoice_id = d.c_invoice_id  AND  h.ad_client_id=d.ad_client_id " +
            "AND h.ad_org_id = d.ad_org_id AND h.session_id = " + Session_Start + ";", connection);
            _get_c_invoic_SalesSummery = cmd_c_invoic_SalesSummery.ExecuteReader();
            if (_get_c_invoic_SalesSummery.HasRows == true)
            {
                _get_c_invoic_SalesSummery.Read();
                credit = Convert.ToDouble(_get_c_invoic_SalesSummery.GetDouble(0)).ToString("0.00");

            }
            connection.Close();

            connection.Open();
            cmd_c_invoic_SalesSummery = new NpgsqlCommand("select  COALESCE(sum(d.cash),0) Cash" +
           "  from c_invoice h, c_invoicepaymentdetails d " +
           "where  is_completed = 'Y' AND iscredit = 'N' AND iscomplementary='Y' AND h.c_invoice_id = d.c_invoice_id  AND  h.ad_client_id=d.ad_client_id " +
            "AND h.ad_org_id = d.ad_org_id AND h.session_id = " + Session_Start + ";", connection);
            _get_c_invoic_SalesSummery = cmd_c_invoic_SalesSummery.ExecuteReader();
            if (_get_c_invoic_SalesSummery.HasRows == true)
            {
                _get_c_invoic_SalesSummery.Read();
                complement = Convert.ToDouble(_get_c_invoic_SalesSummery.GetDouble(0)).ToString("0.00");
            }
            connection.Close();

            connection.Open();
            cmd_c_invoic_SalesSummery = new NpgsqlCommand("select COALESCE(sum(h.discountvalue),0) Cash" +
           "  from c_invoice h, c_invoicepaymentdetails d " +
           "where  is_completed = 'Y' AND iscredit = 'N' AND iscomplementary='N' AND h.c_invoice_id = d.c_invoice_id  AND  h.ad_client_id=d.ad_client_id " +
            "AND h.ad_org_id = d.ad_org_id AND h.session_id = " + Session_Start + ";", connection);
            _get_c_invoic_SalesSummery = cmd_c_invoic_SalesSummery.ExecuteReader();
            if (_get_c_invoic_SalesSummery.HasRows == true)
            {
                _get_c_invoic_SalesSummery.Read();
                //  if (_get_c_invoic_SalesSummery.GetString(0) != null)
                discount = Convert.ToDouble(_get_c_invoic_SalesSummery.GetDouble(0)).ToString("0.00");
            }
            connection.Close();

            connection.Open();
            cmd_c_invoic_SalesSummery = new NpgsqlCommand("select COALESCE(sum(h.lossamount),0) Cash" +
           "  from c_invoice h, c_invoicepaymentdetails d " +
           "where  is_completed = 'Y' AND iscredit = 'N' AND iscomplementary='N' AND h.c_invoice_id = d.c_invoice_id  AND  h.ad_client_id=d.ad_client_id " +
            "AND h.ad_org_id = d.ad_org_id AND h.session_id = " + Session_Start + ";", connection);
            _get_c_invoic_SalesSummery = cmd_c_invoic_SalesSummery.ExecuteReader();
            if (_get_c_invoic_SalesSummery.HasRows == true)
            {
                _get_c_invoic_SalesSummery.Read();
                //  if (_get_c_invoic_SalesSummery.GetString(0) != null)
                Dis_round_off = Convert.ToDouble(_get_c_invoic_SalesSummery.GetDouble(0)).ToString("0.00");
            }
            connection.Close();

            connection.Open();
            cmd_c_invoic_SalesSummery = new NpgsqlCommand("select  COALESCE(sum(d.cash),0) Cash" +
           "  from c_invoice h, c_invoicepaymentdetails d " +
           "where  is_completed = 'Y' AND is_return='Y' AND h.c_invoice_id = d.c_invoice_id  AND  h.ad_client_id=d.ad_client_id " +
            "AND h.ad_org_id = d.ad_org_id AND h.session_id = " + Session_Start + ";", connection);
            _get_c_invoic_SalesSummery = cmd_c_invoic_SalesSummery.ExecuteReader();
            if (_get_c_invoic_SalesSummery.HasRows == true)
            {
                _get_c_invoic_SalesSummery.Read();
                sale_return = Convert.ToDouble(_get_c_invoic_SalesSummery.GetDouble(0)).ToString("0.00");
            }
            connection.Close();

            //////////////
            connection.Open();
            NpgsqlCommand cmd_ad_org = new NpgsqlCommand("SELECT " +
                 "name, " +                  //0
                 "arabicname, " +            //1
                 "logo, " +                  //2
                 "phone, " +                 //3
                 "email, " +                 //4
                 "address, " +               //5
                 "city," +                   //6
                 "country, " +               //7
                 "postal, " +                //8
                 "weburl, " +                //9
                 "footermessage, " +         //10
                 "arabicfootermessage, " +   //11
                 "termsmessage, " +          //12
                 "arabictermsmessage, " +    //13
                 "displayimage " +           //14
                 "FROM ad_org " +            //15
                 "WHERE ad_client_id = " + sales_Summery.ad_client_Id + " AND ad_org_id = " + sales_Summery.ad_org_id + "; ", connection);
            NpgsqlDataReader _get_ad_org = cmd_ad_org.ExecuteReader();
            _get_ad_org.Read();
            ad_org_name = _get_ad_org.GetString(0);
            ad_org_logo = _get_ad_org.GetString(2);
            connection.Close();
            connection.Open();
            NpgsqlCommand cmd_warehouse = new NpgsqlCommand("select name from m_warehouse where m_warehouse_id = " + sales_Summery.warehouse_Id + "; ", connection);
            NpgsqlDataReader _get_warehouse = cmd_warehouse.ExecuteReader();
            _get_warehouse.Read();
            warehouse = _get_warehouse.GetString(0);
            connection.Close();
            //--------------------------------------------------------------------------------------------------------------------------------//
            //-----------------------------                          PRINT FUNCTION                    ---------------------------------------//
            //--------------------------------------------------------------------------------------------------------------------------------//
            #region Public
            /// string formattedVal = "";

            string baseFont = "./Resources/Fonts/GothamRounded/#GothamRounded-Book";
            PrintDialog printDlg = new PrintDialog();
            FlowDocument doc = new FlowDocument();
            Span s = new Span();

            #endregion Public
            #region Invoice Header

            Paragraph Invoice_Header = new Paragraph();
            byte[] binaryData = Convert.FromBase64String(ad_org_logo);

            BitmapImage ad_org_logo_bit = new BitmapImage();
            try
            {
                ad_org_logo_bit.BeginInit();
                ad_org_logo_bit.StreamSource = new MemoryStream(binaryData);
                ad_org_logo_bit.EndInit();
            }
            catch (Exception)
            {

            }
            Image logo = new Image() { Source = ad_org_logo_bit };
            logo.Height = 60;
            logo.Width = 150;
            s = new Span(new Run(""));
            s.Inlines.Add(new LineBreak());
            Invoice_Header.Inlines.Add(logo);
            Invoice_Header.Inlines.Add(s);


            s = new Span(new Run(ad_org_name));
            s.Inlines.Add(new LineBreak());
            s.FontWeight = FontWeights.Bold;
            Invoice_Header.Inlines.Add(s);
            Invoice_Header.TextAlignment = System.Windows.TextAlignment.Center;
            Invoice_Header.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
            Invoice_Header.Margin = new Thickness(0);
            doc.Blocks.Add(Invoice_Header); 
            #endregion Invoice Header



            #region Invoice Header Lines

            Paragraph Invoice_Header_Lines = new Paragraph();
            int Invoice_Header_Lines_sp = 60;
            string Date = "    Session Close Report on " + DateTime.Now.ToString("dd/MM/yyyy");
            int Date_l = Date.Length;
            int Bill_No_Date_sp = Invoice_Header_Lines_sp - (Date_l);
            s = new Span(new Run(Date.PadLeft(Bill_No_Date_sp)));
            //s = new Span(new Run("Bill No : " + invoice_no + "\t" + "       Date : " + DateTime.Now.ToString("dd/MM/yyyy")));
            s.Inlines.Add(new LineBreak());
            Invoice_Header_Lines.Inlines.Add(s);
            s = new Span(new Run("-----------------------------------------------"));//47
            s.Inlines.Add(new LineBreak());
            Invoice_Header_Lines.Inlines.Add(s);
            s = new Span(new Run("                    Session Summary"));
             
            s.Inlines.Add(new LineBreak());
            Invoice_Header_Lines.Inlines.Add(s);
            s = new Span(new Run("-----------------------------------------------"));//47
            //s.Inlines.Add(new LineBreak());
            Invoice_Header_Lines.Inlines.Add(s);


            string Cashier = "Cashier Name :   ";
            int Cashier_l = Cashier.Length;
            string Time = name;
            int Time_l = Time.Length;
            int Cashier_Time_sp = Invoice_Header_Lines_sp - (Cashier_l + Time_l);
            s = new Span(new Run(Cashier + Time));
            //s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            s.Inlines.Add(new LineBreak());
            Invoice_Header_Lines.Inlines.Add(s);

            Cashier = "WareHouse     :   ";
            Cashier_l = Cashier.Length;
            Time = warehouse;
            Time_l = Time.Length;
            Cashier_Time_sp = Invoice_Header_Lines_sp - (Cashier_l + Time_l);
            s = new Span(new Run(Cashier + Time));
            //s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            s.Inlines.Add(new LineBreak());
            Invoice_Header_Lines.Inlines.Add(s);

            Cashier = "Session Start   :   ";
            Cashier_l = Cashier.Length;
            Time = Sessionformattedstartdate;
            Time_l = Time.Length;
            Cashier_Time_sp = Invoice_Header_Lines_sp - (Cashier_l + Time_l);
            s = new Span(new Run(Cashier + Time));
            //s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            s.Inlines.Add(new LineBreak());
            Invoice_Header_Lines.Inlines.Add(s);

            Cashier = "Session End     :  ";
            Cashier_l = Cashier.Length;
            Time = Session_End;
            Time_l = Time.Length;
            Cashier_Time_sp = Invoice_Header_Lines_sp - (Cashier_l + Time_l);
            s = new Span(new Run(Cashier + Time));
            //s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            s.Inlines.Add(new LineBreak());
            Invoice_Header_Lines.Inlines.Add(s);

            //Give style and formatting to paragraph content.
            Invoice_Header_Lines.FontSize = 14;
            Invoice_Header_Lines.FontStyle = FontStyles.Normal;
            Invoice_Header_Lines.TextAlignment = System.Windows.TextAlignment.Left;
            Invoice_Header_Lines.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
            Invoice_Header_Lines.Margin = new Thickness(0);
            doc.Blocks.Add(Invoice_Header_Lines);

            s = new Span(new Run("-----------------------------------------------"));//47
            s.Inlines.Add(new LineBreak());
            Invoice_Header_Lines.Inlines.Add(s);
            s = new Span(new Run("                      Sales Smmary"));
            s.Inlines.Add(new LineBreak());
            Invoice_Header_Lines.Inlines.Add(s);
            s = new Span(new Run("-----------------------------------------------"));//47
            //s.Inlines.Add(new LineBreak());
            Invoice_Header_Lines.Inlines.Add(s);
            #endregion Invoice Header Lines
            doc.Blocks.Add(Invoice_Header_Lines);
            


            Table oTable = new Table();

            // Append the table to the document
            oTable.CellSpacing = 0;
            oTable.Padding = new Thickness(0);
            oTable.FontSize = 14;
            oTable.FontStyle = FontStyles.Normal;
            oTable.TextAlignment = System.Windows.TextAlignment.Center;
            oTable.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
            oTable.Margin = new Thickness(0); 

            oTable.RowGroups.Add(new TableRowGroup());
             
            oTable.RowGroups[0].Rows.Add(new TableRow());  
            TableRow currentRow = oTable.RowGroups[0].Rows[0]; 
            Paragraph pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
            string leftval = "Opening Balance (A)";
            string rightval = Convert.ToDouble(sales_Summery.openingBalance).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
            Paragraph prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right; 
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));


            oTable.RowGroups[0].Rows.Add(new TableRow());
              currentRow = oTable.RowGroups[0].Rows[1];
              pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
              leftval = "Total Sales (B)";
              rightval = Convert.ToDouble(sales_Summery.totalSales).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
              prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));





             int Invoice_Lines_sp = 64; 
            //Cashier = "Opening Balance (A)";
            //Cashier_l = Cashier.Length;
            //Time = Convert.ToDouble(sales_Summery.openingBalance).ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //int len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            //// s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //string rightval = Time.PadLeft(38 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //Invoice_Header_Lines.Inlines.Add(s);

            //Cashier = "Total Sales (B)";
            //Cashier_l = Cashier.Length;
            //Time = Convert.ToDouble(sales_Summery.totalSales).ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            //// s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //rightval = Time.PadLeft(48 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //Invoice_Header_Lines.Inlines.Add(s);

            oTable.RowGroups[0].Rows.Add(new TableRow());
            currentRow = oTable.RowGroups[0].Rows[2];
            pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
            leftval = "Cash (C)";
            rightval = Convert.ToDouble(sales_Summery.cash).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
            prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));

            //Cashier = "Cash (C)";
            //Cashier_l = Cashier.Length;
            //Time = Convert.ToDouble(sales_Summery.cash).ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            //// s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //rightval = Time.PadLeft(57 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //Invoice_Header_Lines.Inlines.Add(s);

            oTable.RowGroups[0].Rows.Add(new TableRow());
            currentRow = oTable.RowGroups[0].Rows[3];
            pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
            leftval = "Bank (D)";
            rightval = Convert.ToDouble(sales_Summery.bank).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
            prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));

            //Cashier = "Bank (D)";
            //Cashier_l = Cashier.Length;
            //Time = Convert.ToDouble(sales_Summery.bank).ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            //// s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //rightval = Time.PadLeft(57 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //Invoice_Header_Lines.Inlines.Add(s);

            oTable.RowGroups[0].Rows.Add(new TableRow());
            currentRow = oTable.RowGroups[0].Rows[4];
            pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
            leftval = "Credit (E)";
            rightval = Convert.ToDouble(credit).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
            prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));

            //Cashier = "Credit (E)";
            //Cashier_l = Cashier.Length;
            //Time = Convert.ToDouble(credit).ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            //// s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //rightval = Time.PadLeft(56 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //Invoice_Header_Lines.Inlines.Add(s);

            oTable.RowGroups[0].Rows.Add(new TableRow());
            currentRow = oTable.RowGroups[0].Rows[5];
            pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
            leftval = "Discount (F)";
            rightval = Convert.ToDouble(discount).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
            prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));

            //Cashier = "Discount (F)";
            //Cashier_l = Cashier.Length;
            //Time = Convert.ToDouble(discount).ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            //rightval = Time.PadLeft(51 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //Invoice_Header_Lines.Inlines.Add(s);

            oTable.RowGroups[0].Rows.Add(new TableRow());
            currentRow = oTable.RowGroups[0].Rows[6];
            pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
            leftval = "RoundOff(K)";
            rightval = Convert.ToDouble(Dis_round_off).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
            prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));

            //Cashier = "RoundOff(K)";
            //Cashier_l = Cashier.Length;
            //Time = Convert.ToDouble(Dis_round_off).ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            //rightval = Time.PadLeft(51 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //Invoice_Header_Lines.Inlines.Add(s);

            oTable.RowGroups[0].Rows.Add(new TableRow());
            currentRow = oTable.RowGroups[0].Rows[7];
            pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
            leftval = "Complement (G)";
            rightval = Convert.ToDouble(complement).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
            prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));

            //Cashier = "Complement (G)";
            //Cashier_l = Cashier.Length;
            //Time = Convert.ToDouble(complement).ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            ////s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //rightval = Time.PadLeft(44 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //Invoice_Header_Lines.Inlines.Add(s);
            oTable.RowGroups[0].Rows.Add(new TableRow());
            currentRow = oTable.RowGroups[0].Rows[8];
            pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
            leftval = "Return (R)";
            rightval = Convert.ToDouble(sale_return).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
            prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));

            //Cashier = "Return (R)";
            //Cashier_l = Cashier.Length;
            //Time = Convert.ToDouble(sale_return).ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            ////s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //rightval = Time.PadLeft(54 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //Invoice_Header_Lines.Inlines.Add(s);



            string Purchase = "0.00";
            string Expenses = "0.00";
            if (txt_Purchases != "")
            {
                Purchase = Convert.ToDouble(txt_Purchases).ToString("0.00");
            }
            if (txt_Expenses != "")
            {
                Expenses = Convert.ToDouble(txt_Expenses).ToString("0.00");
            }

            oTable.RowGroups[0].Rows.Add(new TableRow());
            currentRow = oTable.RowGroups[0].Rows[9];
            pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
            leftval = "Purchase (H)";
            rightval = Convert.ToDouble(Purchase).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
            prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));
            //Cashier = "Purchase (H)";
            //Cashier_l = Cashier.Length;
            //Time = Convert.ToDouble(Purchase).ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            //// s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //rightval = Time.PadLeft(50 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //Invoice_Header_Lines.Inlines.Add(s);
            oTable.RowGroups[0].Rows.Add(new TableRow());
            currentRow = oTable.RowGroups[0].Rows[10];
            pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
            leftval = "Expense (I)";
            rightval = Convert.ToDouble(Expenses).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
            prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
             currentRow.Cells.Add(new TableCell(prightval));
            doc.Blocks.Add(oTable);
            //currentRow.Cells.Add(new TableCell(prightval));
            //Cashier = "Expense (I)";
            //Cashier_l = Cashier.Length;
            //Time = Convert.ToDouble(Expenses).ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            ////s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //rightval = Time.PadLeft(53 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //Invoice_Header_Lines.Inlines.Add(s);

            Paragraph Invoice_Header_Lines_ftr_line = new Paragraph();
            double actualcash = (Convert.ToDouble(sales_Summery.openingBalance)) + (Convert.ToDouble(sales_Summery.cash));
            double exp = Convert.ToDouble(Purchase) + Convert.ToDouble(Expenses);
            s = new Span(new Run("-----------------------------------------------"));//47
           // s.Inlines.Add(new LineBreak());
            Invoice_Header_Lines_ftr_line.Inlines.Add(s);
            Invoice_Header_Lines_ftr_line.FontSize = 14;
            Invoice_Header_Lines_ftr_line.FontStyle = FontStyles.Normal;
            Invoice_Header_Lines_ftr_line.TextAlignment = System.Windows.TextAlignment.Left;
            Invoice_Header_Lines_ftr_line.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
            Invoice_Header_Lines_ftr_line.Margin = new Thickness(0);
            doc.Blocks.Add(Invoice_Header_Lines_ftr_line);




              oTable = new Table();

            // Append the table to the document
            oTable.CellSpacing = 0;
            oTable.Padding = new Thickness(0);
            oTable.FontSize = 14;
            oTable.FontStyle = FontStyles.Normal;
            oTable.TextAlignment = System.Windows.TextAlignment.Center;
            oTable.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
            oTable.Margin = new Thickness(0);

            oTable.RowGroups.Add(new TableRowGroup());

            oTable.RowGroups[0].Rows.Add(new TableRow());
              currentRow = oTable.RowGroups[0].Rows[0];
              pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
              leftval = "Settlement Cash";
            double settlement = Convert.ToDouble(actualcash - exp - Convert.ToDouble(Dis_round_off) - Convert.ToDouble(sale_return));
            rightval = Convert.ToDouble(settlement).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
              prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));
            doc.Blocks.Add(oTable);

            //Cashier = "Settlement Cash";
            //Cashier_l = Cashier.Length;
            //double settlement = Convert.ToDouble(actualcash - exp - Convert.ToDouble(Dis_round_off) - Convert.ToDouble(sale_return));
            //Time = settlement.ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            //rightval = Time.PadLeft(44 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //Invoice_Header_Lines_ftr.Inlines.Add(s);
            Paragraph aftsettlinerows = new Paragraph();
            Cashier = "(J=A+C - H-I-R-K)                    ";
            Cashier_l = Cashier.Length;
            Time = "";
            Time_l = Time.Length;
            Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            s.Inlines.Add(new LineBreak());
            aftsettlinerows.Inlines.Add(s);
            s.Inlines.Add(new LineBreak());
            aftsettlinerows.Inlines.Add(s); 

            s = new Span(new Run("-----------------------------------------------"));//47
            s.Inlines.Add(new LineBreak());
            aftsettlinerows.Inlines.Add(s);
            s = new Span(new Run("              Current Cashier Cash Reciept"));
            s.Inlines.Add(new LineBreak());
            aftsettlinerows.Inlines.Add(s); 
            s = new Span(new Run("-----------------------------------------------"));//47
            //s.Inlines.Add(new LineBreak());
            aftsettlinerows.Inlines.Add(s);

            s = new Span(new Run("Cash On Hand"));
            s.Inlines.Add(new LineBreak()); 
            aftsettlinerows.Inlines.Add(s);

            aftsettlinerows.FontSize = 14;
            aftsettlinerows.FontStyle = FontStyles.Normal;
            aftsettlinerows.TextAlignment = System.Windows.TextAlignment.Left;
            aftsettlinerows.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
            aftsettlinerows.Margin = new Thickness(0);
            doc.Blocks.Add(aftsettlinerows);


            double total_Denomination = 0;
            if (saleType == "Denomination")
            {
  
                oTable = new Table();

                // Append the table to the document
                oTable.CellSpacing = 0;
                oTable.Padding = new Thickness(0);
                oTable.FontSize = 14;
                oTable.FontStyle = FontStyles.Normal;
                oTable.TextAlignment = System.Windows.TextAlignment.Center;
                oTable.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                oTable.Margin = new Thickness(0);

                oTable.RowGroups.Add(new TableRowGroup());

                oTable.RowGroups[0].Rows.Add(new TableRow());
                currentRow = oTable.RowGroups[0].Rows[0];
                pleftval = new Paragraph();
                pleftval.TextAlignment = TextAlignment.Left;
                leftval = "500  x " + SessionClose_500x_input;
                rightval = Convert.ToDouble(SessionClose_500x_total).ToString("0.00 ");

                s = new Span(new Run(leftval));
                pleftval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pleftval));
                prightval = new Paragraph();
                prightval.TextAlignment = TextAlignment.Right;
                s = new Span(new Run(rightval));
                prightval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(prightval));


                oTable.RowGroups[0].Rows.Add(new TableRow());
                currentRow = oTable.RowGroups[0].Rows[1];
                pleftval = new Paragraph();
                pleftval.TextAlignment = TextAlignment.Left;
                leftval = "100  x " + SessionClose_100x_input;
                rightval = Convert.ToDouble(SessionClose_100x_total).ToString("0.00 ");

                s = new Span(new Run(leftval));
                pleftval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pleftval));
                prightval = new Paragraph();
                prightval.TextAlignment = TextAlignment.Right;
                s = new Span(new Run(rightval));
                prightval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(prightval));
 

                oTable.RowGroups[0].Rows.Add(new TableRow());
                currentRow = oTable.RowGroups[0].Rows[2];
                pleftval = new Paragraph();
                pleftval.TextAlignment = TextAlignment.Left;
                leftval = "50    x " + SessionClose_50x_input;
                rightval = Convert.ToDouble(SessionClose_50x_total).ToString("0.00 ");

                s = new Span(new Run(leftval));
                pleftval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pleftval));
                prightval = new Paragraph();
                prightval.TextAlignment = TextAlignment.Right;
                s = new Span(new Run(rightval));
                prightval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(prightval));
 

                oTable.RowGroups[0].Rows.Add(new TableRow());
                currentRow = oTable.RowGroups[0].Rows[3];
                pleftval = new Paragraph();
                pleftval.TextAlignment = TextAlignment.Left;
                leftval = "10    x " + SessionClose_10x_input;
                rightval = Convert.ToDouble(SessionClose_10x_total).ToString("0.00 ");

                s = new Span(new Run(leftval));
                pleftval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pleftval));
                prightval = new Paragraph();
                prightval.TextAlignment = TextAlignment.Right;
                s = new Span(new Run(rightval));
                prightval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(prightval));
 
                oTable.RowGroups[0].Rows.Add(new TableRow());
                currentRow = oTable.RowGroups[0].Rows[4];
                pleftval = new Paragraph();
                pleftval.TextAlignment = TextAlignment.Left;
                leftval = "5      x " + SessionClose_5x_input;
                rightval = Convert.ToDouble(SessionClose_5x_total).ToString("0.00 ");

                s = new Span(new Run(leftval));
                pleftval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pleftval));
                prightval = new Paragraph();
                prightval.TextAlignment = TextAlignment.Right;
                s = new Span(new Run(rightval));
                prightval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(prightval));

               
                oTable.RowGroups[0].Rows.Add(new TableRow());
                currentRow = oTable.RowGroups[0].Rows[5];
                pleftval = new Paragraph();
                pleftval.TextAlignment = TextAlignment.Left;
                leftval = "1      x " + SessionClose_1x_input;
                rightval = Convert.ToDouble(SessionClose_1x_total).ToString("0.00 ");

                s = new Span(new Run(leftval));
                pleftval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pleftval));
                prightval = new Paragraph();
                prightval.TextAlignment = TextAlignment.Right;
                s = new Span(new Run(rightval));
                prightval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(prightval));

                

                oTable.RowGroups[0].Rows.Add(new TableRow());
                currentRow = oTable.RowGroups[0].Rows[6];
                pleftval = new Paragraph();
                pleftval.TextAlignment = TextAlignment.Left;
                leftval = "0.50 x " + SessionClose_50dx_input;
                rightval = Convert.ToDouble(SessionClose_50dx_total).ToString("0.00 ");

                s = new Span(new Run(leftval));
                pleftval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pleftval));
                prightval = new Paragraph();
                prightval.TextAlignment = TextAlignment.Right;
                s = new Span(new Run(rightval));
                prightval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(prightval));

               

                oTable.RowGroups[0].Rows.Add(new TableRow());
                currentRow = oTable.RowGroups[0].Rows[7];
                pleftval = new Paragraph();
                pleftval.TextAlignment = TextAlignment.Left;
                leftval = "0.25 x " + SessionClose_25dx_input;
                rightval = Convert.ToDouble(SessionClose_25dx_total).ToString("0.00 ");

                s = new Span(new Run(leftval));
                pleftval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(pleftval));
                prightval = new Paragraph();
                prightval.TextAlignment = TextAlignment.Right;
                s = new Span(new Run(rightval));
                prightval.Inlines.Add(s);
                currentRow.Cells.Add(new TableCell(prightval)); 

                doc.Blocks.Add(oTable);
                Paragraph aftdinomination = new Paragraph();
                s = new Span(new Run("--------------------------------------------"));//47 
                aftdinomination.Inlines.Add(s);
                doc.Blocks.Add(aftdinomination);
                total_Denomination = (Convert.ToDouble(SessionClose_500x_total) + Convert.ToDouble(SessionClose_100x_total) + Convert.ToDouble(SessionClose_50x_total) + Convert.ToDouble(SessionClose_10x_total) + Convert.ToDouble(SessionClose_5x_total) + Convert.ToDouble(SessionClose_1x_total) + Convert.ToDouble(SessionClose_50dx_total) + Convert.ToDouble(SessionClose_25dx_total));
            }
            if (saleType == "OnlyTotal")
            {
                total_Denomination = Convert.ToDouble(onlyTotal);
            }

            oTable = new Table();

            // Append the table to the document
            oTable.CellSpacing = 0;
            oTable.Padding = new Thickness(0);
            oTable.FontSize = 14;
            oTable.FontStyle = FontStyles.Normal;
            oTable.TextAlignment = System.Windows.TextAlignment.Center;
            oTable.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
            oTable.Margin = new Thickness(0);

            oTable.RowGroups.Add(new TableRowGroup());

            oTable.RowGroups[0].Rows.Add(new TableRow());
            currentRow = oTable.RowGroups[0].Rows[0];
            pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
            leftval = "Total Cash";
            rightval = Convert.ToDouble(total_Denomination).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
            prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));
            doc.Blocks.Add(oTable); 
            Paragraph afttotalcashline = new Paragraph();
            s = new Span(new Run("--------------------------------------------"));//47 
            afttotalcashline.Inlines.Add(s);
            doc.Blocks.Add(afttotalcashline);
            double balance = 0;
            if (settlement < 0)
            {
                balance = Convert.ToDouble(total_Denomination) + settlement;
            }
            else
            {
                balance = Convert.ToDouble(total_Denomination) - settlement;
            }

            if (balance < 0)
            {
                Cashier = "Shortage";
            }
            else
            {
                Cashier = "Excess ";
            }
            oTable = new Table();

            // Append the table to the document
            oTable.CellSpacing = 0;
            oTable.Padding = new Thickness(0);
            oTable.FontSize = 14;
            oTable.FontStyle = FontStyles.Normal;
            oTable.TextAlignment = System.Windows.TextAlignment.Center;
            oTable.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
            oTable.Margin = new Thickness(0);

            oTable.RowGroups.Add(new TableRowGroup());

            oTable.RowGroups[0].Rows.Add(new TableRow());
            currentRow = oTable.RowGroups[0].Rows[0];
            pleftval = new Paragraph();
            pleftval.TextAlignment = TextAlignment.Left;
            leftval = Cashier;
            rightval = Convert.ToDouble(balance).ToString("0.00 ");

            s = new Span(new Run(leftval));
            pleftval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(pleftval));
            prightval = new Paragraph();
            prightval.TextAlignment = TextAlignment.Right;
            s = new Span(new Run(rightval));
            prightval.Inlines.Add(s);
            currentRow.Cells.Add(new TableCell(prightval));
            doc.Blocks.Add(oTable);
            //Cashier_l = Cashier.Length;
            //Time = Convert.ToDouble(balance).ToString("0.00");
            //Time_l = Time.Length;
            //Cashier_Time_sp = Invoice_Lines_sp - (Cashier_l + Time_l);
            //len = (Cashier + Time.PadLeft(Time_l + Cashier_Time_sp)).Length;
            ////s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //rightval = Time.PadLeft(60 - Time_l);
            ////  s = new Span(new Run(Cashier + Time.PadLeft(Cashier_Time_sp)));
            //s = new Span(new Run(Cashier + rightval));
            ////s = new Span(new Run("Cashier : " + user_name + "\t" + "Time : " + DateTime.Now.ToShortTimeString()));
            //s.Inlines.Add(new LineBreak());
            //aftdinomination.Inlines.Add(s);
             
            doc.Name = "Sales_Summary" + Session_Start;
            doc.PageWidth = 270;
            doc.PagePadding = new Thickness(1);
            #region Call Sales Summary API
            string DeviceMacAdd = LoginViewModel.DeviceMacAddress();
            Object SessionCloseJSON =
                           new JObject(
                           new JProperty("operation", "PostSalesSummary"),
                       new JProperty("macAddress", DeviceMacAdd),
                       new JProperty("username", name),
                       new JProperty("password", password),
                       new JProperty("clientId", sales_Summery.ad_client_Id),
                       new JProperty("orgId", sales_Summery.ad_org_id),
                       new JProperty("warehouseId", sales_Summery.warehouse_Id),
                       new JProperty("userId", sales_Summery.ad_user_Id),
                       new JProperty("startTime", Session_Start),
                       new JProperty("endTime", longSessionEnd),
                       new JProperty("openingBalance", sales_Summery.openingBalance),
                       new JProperty("totalSales", Convert.ToDouble(sales_Summery.totalSales).ToString("0.00") ),
                       new JProperty("cash", Convert.ToDouble(sales_Summery.cash).ToString("0.00")),
                       new JProperty("card", Convert.ToDouble(sales_Summery.bank).ToString("0.00") ),
                       new JProperty("credit", credit),
                       new JProperty("discount", discount),
                       new JProperty("complementary", complement),
                       new JProperty("purchase", Purchase),
                       new JProperty("expense", Expenses),
                       new JProperty("settlementCash", actualcash - exp)
                     );

            log.Info("-------------------------");
            log.Info(SessionCloseJSON.ToString());
            log.Info("-------------------------");

            //Sessions.Sales_Summary_APICall(SessionCloseJSON.ToString());

            #endregion Call Sales Summary API
            IDocumentPaginatorSource idpSource = doc;
            try
            {

                SerialPort.OpenDrawer();
                printDlg.PrintDocument(idpSource.DocumentPaginator, $"InvoiceNo_{34}");
            }
            catch
            {
                NotifierViewModel.Notifier.ShowError(" Printer Not Connected");
                OrderDetails_Print.Clear();
                return;
            }
            //Clearing Memory
            OrderDetails_Print.Clear();
        }
        public static void Pring_No_of_Barcode(string count,string barCode_No,string productName,string prodPrice,string prodUOM)
        {
            

            //--------------------------------------------------------------------------------------------------------------------------------//
            //-----------------------------                          PRINT FUNCTION                    ---------------------------------------//
            //--------------------------------------------------------------------------------------------------------------------------------//

            #region Print

            
            string baseFont = "./Resources/Fonts/GothamRounded/#GothamRounded-Book";
            PrintDialog printDlg = new PrintDialog();
            FlowDocument doc = new FlowDocument();
           
          
              
        for(int cnt=1; cnt <= Convert.ToInt32(count); cnt++)
            {
                Span s = new Span();
                Paragraph ItemDetails = new Paragraph();
                s = new Span(new Run(productName));//47
                                                   // s.Inlines.Add(new LineBreak());
                ItemDetails.Inlines.Add(s);
                ItemDetails.Margin = new Thickness(0);
                ItemDetails.FontSize = 14;
                ItemDetails.FontStyle = FontStyles.Normal;
                ItemDetails.TextAlignment = TextAlignment.Center;
                ItemDetails.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                doc.Blocks.Add(ItemDetails);
                #region barcode

                Paragraph barcode = new Paragraph();
                /////////////////////////////////////
                // Encode The Data
                /////////////////////////////////////
                Barcodes bb = new Barcodes();
                bb.BarcodeType = Barcodes.BarcodeEnum.Code39;
                bb.Data = barCode_No;
                bb.CheckDigit = Barcodes.YesNoEnum.Yes;
                bb.encode();
                int thinWidth;
                int thickWidth; 
                string outputString = bb.EncodedData;
                string humanText = bb.HumanText;

                Canvas c = new Canvas();
                c.Margin = new Thickness(0);
                c.Height = 60;
                /////////////////////////////////////
                // Draw The Barcode
                /////////////////////////////////////
                int len = outputString.Length;

                int currentPos = 10;
                int currentTop = 10;

                thinWidth = 2;
                thickWidth = 2 * thinWidth;

                if(barCode_No.Length> 8)
                {
                    if (  barCode_No.Length <=12)
                    {
                        currentPos = 50;
                    }
                    if (barCode_No.Length > 12 && barCode_No.Length <= 15)
                    {
                        currentPos = 30;
                    }
                    thinWidth = 1;
                    thickWidth = 2;
                }
                int currentColor = 0;
                for (int i = 0; i < len; i++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Height = 40;
                    if (currentColor == 0)
                    {
                        currentColor = 1;
                        rect.Fill = new SolidColorBrush(Colors.Black);
                    }
                    else
                    {
                        currentColor = 0;
                        rect.Fill = new SolidColorBrush(Colors.White);
                    }
                    Canvas.SetLeft(rect, currentPos);
                    Canvas.SetTop(rect, currentTop-5);

                    if (outputString[i] == 't')
                    {
                        rect.Width = thinWidth;
                        currentPos += thinWidth;
                    }
                    else if (outputString[i] == 'w')
                    {
                        rect.Width = thickWidth;
                        currentPos += thickWidth;
                    }
                    c.Children.Add(rect);
                }
                /////////////////////////////////////
                // Add the Human Readable Text
                /////////////////////////////////////
                //TextBlock tb = new TextBlock();
                //tb.Text = humanText; 
                //tb.FontSize = 16; 
                //tb.FontWeight = FontWeights.Bold;
                //tb.FontFamily = new FontFamily("Courier New");
                //Rect rx = new Rect(0, 0, 0, 0);
                //tb.Arrange(rx);
                //Canvas.SetLeft(tb, (currentPos - tb.ActualWidth) / 2);
                //Canvas.SetTop(tb, currentTop + 37);
                //c.Children.Add(tb);
                 barcode.Inlines.Add(c);
                s = new Span(new Run(" "));//47
                // s.Inlines.Add(new LineBreak());
                //s.Inlines.Add(new LineBreak());
                barcode.Inlines.Add(s);
                barcode.Margin = new Thickness(0);
                barcode.FontSize = 14;
                barcode.FontStyle = FontStyles.Normal;
                barcode.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                doc.Blocks.Add(barcode);

                s = new Span(new Run(humanText));//47
                Paragraph humanbarcode = new Paragraph();                                   // s.Inlines.Add(new LineBreak());
                humanbarcode.Inlines.Add(s);
                humanbarcode.Margin = new Thickness(0);
                humanbarcode.FontSize = 14;
                humanbarcode.FontStyle = FontStyles.Normal;
                humanbarcode.TextAlignment = TextAlignment.Center;
                humanbarcode.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                doc.Blocks.Add(humanbarcode);


               

                #endregion barcode
                s = new Span(new Run(prodPrice+" / "+prodUOM));//47
                Paragraph Itemprice = new Paragraph();                                   // s.Inlines.Add(new LineBreak());
                Itemprice.Inlines.Add(s);
                Itemprice.Margin = new Thickness(0);
                Itemprice.FontSize = 14;
                Itemprice.FontStyle = FontStyles.Normal;
                Itemprice.TextAlignment = TextAlignment.Center;
                Itemprice.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                doc.Blocks.Add(Itemprice);
               
                Paragraph invoice_barcode = new Paragraph();
                //s = new Span(new Run("*" + invoice_no + "*"));//47
                s = new Span(new Run(" "));//47
                s.Inlines.Add(new LineBreak()); 
                invoice_barcode.Inlines.Add(s);
                s = new Span(new Run("Qsale.Qa"));//47
                invoice_barcode.Inlines.Add(s);
                invoice_barcode.Margin = new Thickness(0);
                invoice_barcode.FontSize = 14;
                invoice_barcode.FontStyle = FontStyles.Normal;
                invoice_barcode.TextAlignment = TextAlignment.Center;
                invoice_barcode.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), baseFont);
                doc.Blocks.Add(invoice_barcode);
                Paragraph Line = new Paragraph();

                s = new Span(new Run("--------------------------------------------"));//47
                s.Inlines.Add(new LineBreak());
                Line.Inlines.Add(s);
                doc.Blocks.Add(Line);
            }
            doc.Name = "BarcodeNo_" + barCode_No;
            doc.PageWidth = 270;
            doc.PagePadding = new Thickness(1);
           
            IDocumentPaginatorSource idpSource = doc;
            try
            {

                SerialPort.OpenDrawer();
                printDlg.PrintDocument(idpSource.DocumentPaginator, $"BarcodeNo_{barCode_No}");
            }
            catch
            {
                NotifierViewModel.Notifier.ShowError(" Printer Not Connected");
                OrderDetails_Print.Clear();
                return;
            }
            #endregion Print

            //--------------------------------------------------------------------------------------------------------------------------------//
            //-----------------------------                          PRINT FUNCTION END                ---------------------------------------//
            //--------------------------------------------------------------------------------------------------------------------------------//

            //Clearing Memory
            OrderDetails_Print.Clear();
        }
    }

    public class Product
    {
        public string Product_Name { get; set; }
        public string Product_ID { get; set; }
        public double Discount { get; set; }
        public double Quantity { get; set; }
        public string Price { get; set; }
        public string Amount { get; set; }
        public string Iteam_Barcode { get; set; }//Product_Barcode_Searchkey
        public string Percentpercentage_OR_Price { get; set; }
        public string Ad_client_id { get; set; }
        public string Ad_org_id { get; set; }
        public string Product_category_id { get; set; }
        public string Product_Arabicname { get; set; }
        public string Product_Image { get; set; }
        public string Scanby_Weight { get; set; }
        public string Scanby_Price { get; set; }
        public string Uom_Id { get; set; }
        public string Uom_Name { get; set; }
        public string Sopricestd { get; set; }
        public string Current_costprice { get; set; }
        public string Is_productMultiUOM { get; set; }
        public string Line_Discount { get; set; } = "N";
    }

    public class Product_Hold
    {
        public string C_invoice_id { get; set; }
        public string Ad_client_id { get; set; }
        public string Ad_org_id { get; set; }
        public string Ad_role_id { get; set; }
        public string Ad_user_id { get; set; }
        public string Documentno { get; set; }
        public string M_warehouse_id { get; set; }
        public string C_bpartner_id { get; set; }
        public string Qid { get; set; }
        public string Mobilenumber { get; set; }
        public string Discounttype { get; set; }
        public string Discountvalue { get; set; }
        public string Grandtotal { get; set; }
        public string Orderid { get; set; }
        public string Reason { get; set; }
        public string Is_posted { get; set; }
        public string Is_onhold { get; set; }
        public string Is_canceled { get; set; }
        public string Is_completed { get; set; }
        public string Grandtotal_round_off { get; set; }
        public string Total_items_count { get; set; }
        //public string Product_Name              { get; set; }
        //public string Product_ID                { get; set; }
        //public double Discount                  { get; set; }
        //public double Quantity                  { get; set; }
        //public string Price                     { get; set; }
        //public string Amount                    { get; set; }
        //public string Iteam_Barcode             { get; set; }//Product_Barcode_Searchkey
        //public string Percentpercentage_OR_Price{ get; set; }
        //public string Product_category_id       { get; set; }
        //public string Product_Arabicname        { get; set; }
        //public string Product_Image             { get; set; }
        //public string Scanby_Weight             { get; set; }
        //public string Scanby_Price              { get; set; }
        //public string Uom_Id                    { get; set; }
        //public string Uom_Name                  { get; set; }
        //public string Sopricestd                { get; set; }
        //public string Current_costprice         { get; set; }
        //public string Is_productMultiUOM        { get; set; }
        //public string Line_Discount             { get; set; } = "N";
    }
    public class invoiceList
    {
        public string invoiceId { get; set; }
        public string documentNo { get; set; }
        public string posId { get; set; }
        public string invoiceDate { get; set; }
        public string grandTotal { get; set; }
        public string lineCount { get; set; }

        public string payAmount { get; set; }
    }

        public class Product_Uom
    {
        public string barCode { get; set; }
        public string productName { get; set; }
        public string uomType { get; set; }
        public string categoryid { get; set; }
        public string categoryName { get; set; }
        public string uomid { get; set; }
        public string uomValue { get; set; }
        public string salesPrice { get; set; }
        public string purchasePrice { get; set; }
        public string currency { get; set; }
        public string costprice { get; set; }
        public string productId { get; set; }
        public string noofpcs { get; set; }

    }
    public class Product_Uom_Erp
    {

        public string sellingPrice { get; set; }
        public string categoryName { get; set; }
        public string costPrice { get; set; }
        public string isPriceEditable { get; set; }
        public string categoryId { get; set; }
        public string productValue { get; set; }
        public string uomName { get; set; }
        public string bomQty { get; set; }
        public string uomId { get; set; }
        public string productName { get; set; }
        public string purchasePrice { get; set; }
        public string productId { get; set; }

    }
    public class SessionCart
    {
#pragma warning disable IDE1006 // Naming Styles
        public string operation { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string userId { get; set; }
        public string businessPartnerId { get; set; }
        public string clientId { get; set; }
        public string roleId { get; set; }
        public string orgId { get; set; }
        public string warehouseId { get; set; }
        public string remindMe { get; set; }
        public string macAddress { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string sessionId { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    public class OrderHeaders
    {
#pragma warning disable IDE1006 // Naming Styles
        public string isReturned { get; set; } = "N";
        public int clientId { get; set; }
        public int orgId { get; set; }
        public int warehouseId { get; set; }
        public int userId { get; set; }
        public int salesrepId { get; set; }
        public int giftAmount { get; set; }
        public int periodId { get; set; }
        public int currencyId { get; set; }
        public int cashbookId { get; set; }
        public int paymentTermId { get; set; }
        public int pricelistId { get; set; }
        public int adTableId { get; set; }
        public string accountSchemaId { get; set; }
        public string createdDate { get; set; }
        public int posId { get; set; }
        public string docNo { get; set; }
        public int refInvoiceId { get; set; } = 0;
        public int totalLines { get; set; }
        public string qid { get; set; }
        public int warehouseNo { get; set; } = 0;
        public string customerName { get; set; }
        public string creditName { get; set; }
        public int businessPartnerId { get; set; }
        public int mobilenumber { get; set; }
        public double totalAmount { get; set; } = 0;
        public double cashAmount { get; set; } = 0;
        public double cardAmount { get; set; } = 0;
        public double exchangeAmount { get; set; } = 0;
        public double redemptionAmount { get; set; } = 0;
        public double paidAmount { get; set; } = 0;
        public double dueAmount { get; set; } = 0;
        public double lossAmount { get; set; } = 0;
        public double extraAmount { get; set; } = 0;
        public string IsCash { get; set; } = "N";
        public string IsCard { get; set; } = "N";
        public string isComplement { get; set; } = "N";
        public string isCredit { get; set; } = "N";
#pragma warning restore IDE1006 // Naming Styles
    }
    public class Sales_Summery
    {
#pragma warning disable IDE1006 // Naming Styles
        public string totalSales { get; set; }
        public string cash { get; set; }
        public string bank { get; set; }
        public string exchange { get; set; }
        public string redemption { get; set; }
        public string openingBalance { get; set; }
        public string ad_client_Id { get; set; }
        public string ad_org_id { get; set; }
        public string warehouse_Id { get; set; }
        public string ad_user_Id { get; set; }
    }
    public class Invoice_Post
    {
#pragma warning disable IDE1006 // Naming Styles

        public string c_invoice_id { get; set; }
        public string ad_client_id { get; set; }
        public string ad_org_id { get; set; }
        public string ad_user_id { get; set; }
        public string ad_role_id { get; set; }
        public string documentno { get; set; }
        public string m_warehouse_id { get; set; }
        public string m_warehouse_name { get; set; }
        public string c_bpartner_id { get; set; }
        public string qid { get; set; }
        public string mobilenumber { get; set; }
        public string discounttype { get; set; }
        public string discountvalue { get; set; }
        public string grandtotal { get; set; }
        public string orderid { get; set; }
        public string invoice_reason { get; set; }
        public string created { get; set; }
        public string grandtotal_round_off { get; set; }
        public string total_items_count { get; set; }
        public string balance { get; set; }
        public string change { get; set; }
        public string lossamount { get; set; }
        public string extraamount { get; set; }
        public string cash { get; set; }
        public string card { get; set; }
        public string exchange { get; set; }
        public string redemption { get; set; }
        public string iscomplementary { get; set; }
        public string iscredit { get; set; }
        public string name_id { get; set; }
        public string mobile_numbler { get; set; }
        public string reason { get; set; }
        public string c_bpartner_name { get; set; }
        public string c_status { get; set; }
        public string is_return { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
    public class OrderHeaderSheeetBill
    {
        public string logo { get; set; }
        public string orgname { get; set; }
        public string wareHouse { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string phoneNumber { get; set; }
        public string fax { get; set; }
        public string attn { get; set; }
        public string dnNo { get; set; }
        public string lpoReference { get; set; }
        public string invoiceNo { get; set; }
        public string date { get; set; }
        public string orderReference { get; set; }
        public string paymentTerm { get; set; }
        public string grandTotal { get; set; }
        public string customerName { get; set; }
        public string customerNo { get; set; }
        public string amountwords { get; set; }
        public string email { get; set; }
        public string footaddress { get; set; }
        public string arabicName { get; set; }
    }
    public class OrderDetailSheeetBill
    {
        public string sno { get; set; }
        public string searchKey { get; set; }
        public string discription { get; set; }
        public string uom { get; set; }
        public string qty { get; set; }
        public string unitPrice { get; set; }
        public string discount { get; set; }
        public string total { get; set; }
        public string invoiceNo { get; set; }
        public string arabicName { get; set; }
    }

     public class OrderDetails
    {
#pragma warning disable IDE1006 // Naming Styles
        public string costPrice { get; set; }                           //288.00,
        public string orderedQty { get; set; }                          //1,
        public string dicountPercent { get; set; }                      // 0,
        public string qty { get; set; }                                 // 1,
        public string isExists { get; set; }                            //"N",
        public string uomId { get; set; }                               //1000217,
        public string uomName { get; set; }
        public string discountAmount { get; set; }                      //"20.00",
        public string productId { get; set; }                           //1019713,
        public string actualPrice { get; set; }                         //861,
        public string KotLineID { get; set; }                           //0,
        public string price { get; set; }                               //841,
        public string discountType { get; set; }                        //"P",
        public string productUOMValue { get; set; }                     //"PCS",
        public string description { get; set; }                         //"",
        public string productName { get; set; }                         //"ESLABONDEXX SKIN TOTAL BODY LOTION 250ml",
        public string productArabicName { get; set; }
        public string productSearchKey { get; set; }
        public string productCategoryId { get; set; }                   //1000405
#pragma warning restore IDE1006 // Naming Styles
    }

    public class View_Order_Completed
    {
#pragma warning disable IDE1006 // Naming Styles
        public string c_invoice_id { get; set; }
        public string invoice_id { get; set; }
        public string ad_client_id { get; set; }
        public string ad_org_id { get; set; }
        public string ad_role_id { get; set; }
        public string ad_user_id { get; set; }
        public string documentno { get; set; }
        public string m_warehouse_id { get; set; }
        public string c_bpartner_id { get; set; }
        public string qid { get; set; }
        public string mobilenumber { get; set; }
        public string discounttype { get; set; }
        public string discountvalue { get; set; }
        public string grandtotal { get; set; }
        public string orderid { get; set; }
        public string reason { get; set; }
        public string created { get; set; }
        public string is_posted { get; set; }
        public string grandtotal_round_off { get; set; }
        public string total_items_count { get; set; }
        public string items_count { get; set; }
        public string grandtotal_tx { get; set; }
        public string Status { get; set; }
        public string completedStatus { get; set; }
        public string StrikeThrough { get; set; }
        public string balance { get; set; }
        public string change { get; set; }
        public string lossamount { get; set; }
        public string extraamount { get; set; }
        public string c_bpartner_name { get; set; }
        public string searchkey { get; set; }
        public string pricelistid { get; set; }
        public string iscredit { get; set; }
        public string creditamount { get; set; }
        public string isdefault { get; set; }
        public string iscashcustomer { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
    public class SearchAndSelectViewModel
    {
        private ICollectionView products;
        private Product selectedProduct;

        public SearchAndSelectViewModel()
        {
            var myProducts = new List<Product>();

            myProducts.Add(new Product() { Product_Name = "Asparagus" });
            myProducts.Add(new Product() { Product_Name = "Broccoli" });
            myProducts.Add(new Product() { Product_Name = "Chard" });
            myProducts.Add(new Product() { Product_Name = "Dandelion" });
            myProducts.Add(new Product() { Product_Name = "Endive" });
            myProducts.Add(new Product() { Product_Name = "Fennel" });
            myProducts.Add(new Product() { Product_Name = "Jicama" });
            myProducts.Add(new Product() { Product_Name = "Kale" });
            myProducts.Add(new Product() { Product_Name = "Lettuce" });
            myProducts.Add(new Product() { Product_Name = "Potatoes" });
            myProducts.Add(new Product() { Product_Name = "Rhubarb" });

            this.products = System.Windows.Data.CollectionViewSource.GetDefaultView(myProducts);
        }

        public ICollectionView Products
        {
            get
            {
                return this.products;
            }
        }

        public Product SelectedProduct
        {
            get
            {
                return this.selectedProduct;
            }
            set
            {
                if (this.selectedProduct != value)
                {
                    this.selectedProduct = value;
                }
            }
        }

    }
    public class TestVM : INotifyPropertyChanged
    {
        public TestVM()
        {
            var values = new List<TestNameValue>();
            var valuesstr = new List<object>();
            for (int cntb = 0; cntb <= 304; cntb++)
            {
                values.Add(new TestNameValue("train" + cntb, cntb));
                values.Add(new TestNameValue("train1" + cntb, cntb));
                values.Add(new TestNameValue("train2" + cntb, cntb));
                values.Add(new TestNameValue("name" + cntb, cntb));
                values.Add(new TestNameValue("driving mode" + cntb, cntb));
                values.Add(new TestNameValue("spees" + cntb, cntb));

                values.Add(new TestNameValue("trdweain" + cntb, cntb));
                values.Add(new TestNameValue("ewetrain1" + cntb, cntb));
                values.Add(new TestNameValue("ewetrain2" + cntb, cntb));
                values.Add(new TestNameValue("ename" + cntb, cntb));
                values.Add(new TestNameValue("dfriving mode" + cntb, cntb));
                values.Add(new TestNameValue("sdpeeews" + cntb, cntb));

                valuesstr.Add("str" + cntb);
            }
            Values = values;
            Valuesstr = valuesstr;
        }

        private List<TestNameValue> _values;
        public List<TestNameValue> Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
                OnPropertyChanged("Values");
            }
        }

        private List<object> _valuesstr;
        public List<object> Valuesstr
        {
            get
            {
                return _valuesstr;
            }
            set
            {
                _valuesstr = value;
                OnPropertyChanged("Valuesstr");
            }
        }

        private string _seltext;
        public string Text
        {
            get
            {
                return _seltext;
            }
            set
            {
                _seltext = value;
                OnPropertyChanged("Text");
            }
        }

        private object _selItem;
        public object SelectedItm
        {
            get
            {
                return _selItem;
            }
            set
            {
                _selItem = value;
                OnPropertyChanged("SelectedItm");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class TestNameValue
    {
        public TestNameValue()
        {
        }

        public TestNameValue(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}