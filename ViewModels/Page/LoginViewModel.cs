using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Restaurant_Pos
{
    public class LoginViewModel : BaseViewModel
    {
        public static string[] _DeviceMacAddress_arr = NetworkInterface
         .GetAllNetworkInterfaces()
         .Where(nic => nic.NetworkInterfaceType != NetworkInterfaceType.Loopback && nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
         .Select(nic => nic.GetPhysicalAddress().ToString()).ToArray();

        //public static string _DeviceMacAddress = NetworkInterface
        // .GetAllNetworkInterfaces()
        // .Where(nic => nic.NetworkInterfaceType != NetworkInterfaceType.Loopback && nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
        // .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

        public static int _DeviceMacAddress_count = NetworkInterface
         .GetAllNetworkInterfaces()
         .Where(nic => nic.NetworkInterfaceType != NetworkInterfaceType.Loopback && nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
         .Select(nic => nic.GetPhysicalAddress().ToString()).Count();

        public static string DeviceMacAddress()//CPU ID
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == "")
                {
                    //Get only the first CPU's ID
                    cpuInfo = mo.Properties["processorID"].Value.ToString();
                    break;
                }
            }
            return cpuInfo;
        }
    }

    public class POSorganizationApiModel
    {
        

#pragma warning disable IDE1006 // Naming Styles
        public string remindMe { get; set; }
        public string macAddress { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string operation { get; set; }
        public string username { get; set; }
        public string password { get; set; }
       
       
        
        
        //public string userId { get; set; }
        //public string businessPartnerId { get; set; }
        //public string clientId { get; set; }
        //public string roleId { get; set; }
        //public string orgId { get; set; }
        //public string warehouseId { get; set; }
        
        
     
        
#pragma warning restore IDE1006 // Naming Styles
    }
    public class POSCustomerApiModel
    {
        public int clientId { get; set; }
        public int orgId { get; set; }
        public string macAddress { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string remindMe { get; set; }
        public string operation { get; set; }

    }
    public class M_UOM
    {
        public int UOMId { get; set; }
        public string Name { get; set; }
    }
    public class M_ItemCategory
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
    }
 
    public class M_warehouse
    {
        public int WarehouseId { get; set; }
        public int Ad_client_id { get; set; }
        public int Ad_org_id { get; set; }
        public int M_warehouse_id { get; set; }
        public string Isactive { get; set; }
        public string Isdefault { get; set; }
        public string WarehouseName { get; set; }
        public int WarehouePriceListId { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
    }
    public class M_user
    {
        public int UserID { get; set; }
        public string Name { get; set; }

    }
    
    public class M_Roles
    {
        public int RoleID { get; set; }
        public string Name { get; set; }
       
    }
    public class M_customercreditpay
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

    }
    public class M_customerpay
    {
        public int posId { get; set; }
        public int invoiceId { get; set; }
        public string documentNo { get; set; }
        public DateTime invoiceDate { get; set; }
        public double grandTotal { get; set; }
        
      


    }
    public class M_selectedcustomer
    {
        public int posId { get; set; }
        public int invoiceId { get; set; }
        public string documentNo { get; set; }
        public DateTime invoiceDate { get; set; }
        public double grandTotal { get; set; }




    }

    public class M_Tables
    {
        public int ID { get; set; }
        public string Name { get; set; }

    }
    public class M_Terminals
    {
        public int id { get; set; }
        public string Name { get; set; }

    }
    public class M_Country
    {
        public int CountryID { get; set; }
        public string Name { get; set; }

    }
    public class M_City
    {
        public int CityID { get; set; }
        public string Name { get; set; }

    }
    
     public class m_ProductList
    {
        public int id { get; set; }
        public string Name { get; set; }

    }
    public class m_ProductKey
    {
        public int id { get; set; }
        public string Name { get; set; }

    }
    

    public class m_ProductLst
    {
        public int id { get; set; }
        public string Name { get; set; }

    }


    public class m_TakeAwayProduct
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string ImgPath { get; set; }

    }
    public class m_onHoldInvoice
    {
        //c_invoice_id, grandtotal,total_items_count
        public int c_invoice_id { get; set; }
        public string grandtotal { get; set; }
        public string total_items_count { get; set; }

    }
    public class m_OrderOnlineDetail
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string status { get; set; }

    }
    
    public class m_TakeAwayProductCAT
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string ImgPath { get; set; }

    }
  
        public class m_TakeAwayProductValue
    {
        public int value { get; set; }
       

    }

    public class m_TerminalsList
    {
        public int id { get; set; }
        public string Name { get; set; }
    }
    public class m_TerminalsWise
    {
        public int id { get; set; }
        public string Name { get; set; }
    }
    public class m_ProductCATLst
    {
        public int id { get; set; }
        public string Name { get; set; }
    }
    

    public class m_TableList
    {
        public int id { get; set; }
        public string Name { get; set; }

    }
    public class m_TableWise
    {
        public int id { get; set; }
        public string Name { get; set; }

    }
  
    public class Img
    {
    
        public string imgpath { get; set; }
    }

    public class m_CustomersList
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
    
    public class UserData
    {
     
        public string UserName { get; set; }
      
    }

    public class POSCashCustomer
    {
#pragma warning disable IDE1006 // Naming Styles
        public string operation { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string remindMe { get; set; }
        public string macAddress { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string userId { get; set; }
        public string clientId { get; set; }
        public string orgId { get; set; }
        public string roleId { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    public class POSCustomers
    {
#pragma warning disable IDE1006 // Naming Styles
        public string operation { get; set; }
        public string remindMe { get; set; }
        public string macAddress { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string clientId { get; set; }
        public string orgId { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    public class POSCategory
    {
#pragma warning disable IDE1006 // Naming Styles
        public string operation { get; set; }
        public string remindMe { get; set; }
        public string macAddress { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string clientId { get; set; }
        public string orgId { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    public class POSOrderNumber
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
#pragma warning restore IDE1006 // Naming Styles
    }

    public class POSAllProducts
    {
#pragma warning disable IDE1006 // Naming Styles
        public string operation { get; set; }
        public string remindMe { get; set; }
        public string macAddress { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string clientId { get; set; }
        public string orgId { get; set; }
        public string pricelistId { get; set; }
        public string costElementId { get; set; }

        public string warehouseId { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
    public class POSAddProduct
    {
        public int clientId { get; set; }
        public int orgId { get; set; }
        public string macAddress { get; set; }
        public string version { get; set; }
        public string operation { get; set; }
        public int userId { get; set; }
        public int warehouseId { get; set; }
        public int pricelistId { get; set; }
        public string appName { get; set; }
        public int accountSchemaId { get; set; }
        public int productId { get; set; }
        public string productName { get; set; }
        public string productArabicName { get; set; }
        public string productValue { get; set; }
        public int productCategoryId { get; set; }
        public string categoryName { get; set; }
        public string purchasePrice { get; set; }
        public string costPrice { get; set; }
        public string sellingPrice { get; set; }
        public int uomId { get; set; }
        public string uomName { get; set; }
        public int costElementId { get; set; }
        public string isPriceEditable { get; set; }
        public string isBomAvailable { get; set; }
        public List<bomDetails> bomdetails = new List<bomDetails>();
       

    }
    public class POSAddUser
    {
        public string macAddress { get; set; }
        public int clientId { get; set; }
        public int orgId { get; set; }
        public int warehouseId { get; set; }
        public int userId { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string operation { get; set; }
        public int ad_user_id { get; set; }
        public string userValue { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string userNumber { get; set; }
        public string userEmail { get; set; }
        public string address1 { get; set; }
        public int city { get; set; }
        public int country { get; set; }
        public string postal { get; set; }
        public List<roleDetail> roleDetails = new List<roleDetail>();
       


    }
    public class roleDetail
    {
        public int roleId { get; set; }
        public string roleName { get; set; }

        
    }
        public class bomDetails
{
        public string sellingPrice { get; set; }
        public string categoryName { get; set; }
        public string costPrice { get; set; }
        public string isPriceEditable { get; set; }
        public int categoryId { get; set; }
        public string productValue { get; set; }
        public string uomName { get; set; }
        public string bomQty { get; set; }
        public int uomId { get; set; }
        public string productName { get; set; }
        public string purchasePrice { get; set; }
        public int productId { get; set; }

    }

        public class m_floorTables
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string tablegroup { get; set; }
        public bool coversLevel { get; set; }
        public string floor_No { get; set; }

    }
    public class POSAddCategoryApiModel
    {
        public string macAddress { get; set; }
        public int clientId { get; set; }
        public int orgId { get; set; }
        public int warehouseId { get; set; }
        public int userId { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string operation { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string description { get; set; }
    }
    public class POSCustomersApiModel
    {
        public string macAddress { get; set; }
        public int clientId { get; set; }
        public int orgId { get; set; }
        public int warehouseId { get; set; }
        public int userId { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string operation { get; set; }
        public int businessPartnerId { get; set; }
        public string customerValue { get; set; }
        public string customerName { get; set; }
        public string lastName { get; set; }
        public string customerNumber { get; set; }
        public string customerEmail { get; set; }
        public string address1  { get; set; }
        public int city { get; set; }
        public int country { get; set; }
        public string postal { get; set; }
        public string openingBalance { get; set; }
        public string creditLimit { get; set; }
        public string isVendor { get; set; }
        public string isCredit { get; set; }
        public string isCustomer { get; set; }
    }

    public class POSAddTerminalsApiModel
    {
        public string macAddress { get; set; }
        public int clientId { get; set; }
        public int orgId { get; set; }
        public int warehouseId { get; set; }
        public int userId { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string operation { get; set; }
        public int terminalId { get; set; }
        public string termnalName { get; set; }
        public string description { get; set; }
        public string isPriner { get; set; }
        public string ipaddress { get; set; }
    }
    public class POSTableApiModel
    {
        public string macAddress { get; set; }
        public int clientId { get; set; }
        public int orgId { get; set; }
        public int warehouseId { get; set; }
        public int userId { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string operation { get; set; }
        public int tableId { get; set; }
        public string tableName { get; set; }
        public string description { get; set; }
        public string coverslevel { get; set; }
        public int tablegroupId { get; set; }
    }
    public class POSCustCreditApiModel
    {
        public string macAddress { get; set; }
        public int clientId { get; set; }
        public int orgId { get; set; }
        public int warehouseId { get; set; }
        public int userId { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string operation { get; set; }
        public int businessPartnerId { get; set; }
        public string isCustomer { get; set; }
        public string isVendor { get; set; }
        
    }
    public class POSInvoicePaymentsModel
    {
        public string macAddress { get; set; }
        public int clientId { get; set; }
        public int orgId { get; set; }
        public int warehouseId { get; set; }
        public int userId { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string operation { get; set; }
        public int businessPartnerId { get; set; }
        public string isCustomer { get; set; }
        public string isVendor { get; set; }
        public string cashbookId { get; set; }
        public string tenderType { get; set; }
        public string payAmount { get; set; }
        public string description { get; set; }
        public  List<M_selectedcustomer> invoiceList = new List<M_selectedcustomer>();

    }
    //Api mode
    public class PostVendorInvoicePayments
    {
        public string macAddress { get; set; }
        public int clientId { get; set; }
        public string orgId { get; set; }
        public string warehouseId { get; set; }
        public string userId { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string operation { get; set; }
        public string businessPartnerId { get; set; }
        public string description { get; set; }
        public string cashbookId { get; set; }
        public string tenderType { get; set; }
        public string payAmount { get; set; }
        public string isCustomer { get; set; }
        public string isVendor { get; set; }
        public List<apiInvoiceList> invoiceList { get; set; }
    }

    public class apiInvoiceList
    {
        public string payAmount { get; set; }
        public string grandTotal { get; set; }
        public string invoiceDate { get; set; }
        public string posId { get; set; }
        public string invoiceId { get; set; }
        public string documentNo { get; set; }
    }


    //Api model end
    public class m_TakeAwayProductRS
    {
        public string checkbox { get; set; }
        public m_TakeAwayProductRS()
        {
            checkbox = "Hidden";
        }
        public int id { get; set; }
        public int categoryid { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int ItemCount { get; set; }
        public string KotNotes { get; set; }
        public double TotalPrice
        {
            get
            {
                double x = (ItemCount * (int)Price) - ((discountPer * (ItemCount * (int)Price)) / 100);
                return System.Math.Round(x, 2);
            }

        }
        public double discountPer
        {
            get; set;
        }
        public double discountAmt
        {
            get
            {
                return System.Math.Round(discountPer / 100 * (ItemCount * Price), 2);
            }
        }

        public string paroductarabicname { get; set; }
        public string productbarcode { get; set; }
        public string c_uom_id { get; set; }
        public string uomname { get; set; }
        public string qtyinvoiced { get; set; }
        public string qtyentered { get; set; }
        public string saleprice { get; set; }
       
        public string m_pricelist_id { get; set; }
        public string m_terminal_id { get; set; }
      
    }
 

}