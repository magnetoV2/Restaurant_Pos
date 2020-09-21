using System.Collections.ObjectModel;

namespace Restaurant_Pos
{
    public class UICombobox
    {
        private ObservableCollection<WarehouseLoginComboBox> _Retail_Warehouse;

        public ObservableCollection<WarehouseLoginComboBox> Retail_Warehouse
        {
            get { return _Retail_Warehouse; }
            set { _Retail_Warehouse = value; }
        }

        private WarehouseLoginComboBox _sRetail_Warehouse;

        public WarehouseLoginComboBox SRetail_Warehouse
        {
            get { return _sRetail_Warehouse; }
            set { _sRetail_Warehouse = value; }
        }
    }
}