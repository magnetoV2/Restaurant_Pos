using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant_Pos.ViewModels.Page
{
    class cartOrders
    {
    }

    class POSOrderDrafted
    {
        public string macAddress { get; set; }
        public string clientId { get; set; }
        public string orgId { get; set; }
        public string warehouseId { get; set; }
        public string userId { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string operation { get; set; }
        public string authorizedBy { get; set; }
        public string sessionId { get; set; }
        public string reason { get; set; }
        public OrderHeaders OrderHeaders;
        public List<OrderDetails> OrderDetails { get; set; }
        public List<PaymentDetails> PaymentDetails { get; set; }
    }


   class PaymentDetails
    {
     public string amount { get; set; }
      public string paymenttype { get; set; }
    }


}

