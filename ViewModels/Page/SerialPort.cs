using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Restaurant_Pos.ViewModels.Page
{
    static class SerialPort
    {
       private static string  PortName = ConfigurationManager.AppSettings.Get("PortName");
        private static string BaudRate = ConfigurationManager.AppSettings.Get("BaudRate");
        private static string DataBits = ConfigurationManager.AppSettings.Get("DataBits");
        
        private static string CashDrawPortName = ConfigurationManager.AppSettings.Get("CashDrawPortName");
        private static string CashDrawBaudRate = ConfigurationManager.AppSettings.Get("CashDrawBaudRate");
        private static string CashDrawDataBits = ConfigurationManager.AppSettings.Get("CashDrawDataBits");
       // private static System.IO.Ports.SerialPort serialPort1 = new System.IO.Ports.SerialPort();
        public  static void display(string Line1,string Line2,string Line3,string Line4)
        {
            System.IO.Ports.SerialPort serialPort1 = new System.IO.Ports.SerialPort();
            try
            {
                
                serialPort1.PortName = PortName;
                serialPort1.BaudRate = Convert.ToInt32(BaudRate); 
                serialPort1.DataBits = Convert.ToInt32(DataBits);
                serialPort1.Parity = Parity.None;
                serialPort1.StopBits = StopBits.One;
                serialPort1.DtrEnable = true;
                serialPort1.RtsEnable = true;  
                    serialPort1.Open();
                    serialPort1.Write(new byte[] { 0x0C }, 0, 1); 
                   // byte[] data = Encoding.ASCII.GetBytes(Line1 + space1 + Line2); // your byte data;
                    serialPort1.Write(Line1  + Line2);

                    //Goto Bottem Line
                    //serialPort1.Write(new byte[] { 0x0A, 0x0D }, 0, 2); 
                    //byte[] data1 = Encoding.ASCII.GetBytes(Line3 + space2 + Line4); // your byte data;  
                     serialPort1.Write(Line3 + Line4); 

                
                serialPort1.Close();
                serialPort1.Dispose();
                serialPort1 = null; 
            }
            catch (Exception )
            {
                serialPort1.Close();
                serialPort1.Dispose();
                serialPort1 = null;
                //MessageBox.Show(ex.ToString() + "----"+ PortName+"----"+ BaudRate.ToString()+"----"+ DataBits.ToString());

            }
        }
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "..";
        }
         
          
 

        public static void OpenDrawer()
        {
            System.IO.Ports.SerialPort serialPort1 = new System.IO.Ports.SerialPort();
            serialPort1.PortName = CashDrawPortName;
            serialPort1.Encoding = Encoding.ASCII;
            serialPort1.BaudRate = Convert.ToInt32(CashDrawBaudRate);
            serialPort1.Parity = System.IO.Ports.Parity.None;
            serialPort1.DataBits = Convert.ToInt32(CashDrawDataBits);
            serialPort1.StopBits = System.IO.Ports.StopBits.One;
            serialPort1.DtrEnable = true;
            try
            {
                serialPort1.Open();
                serialPort1.Write(Char.ConvertFromUtf32(27) + char.ConvertFromUtf32(64));
                serialPort1.Write(char.ConvertFromUtf32(27) +
                char.ConvertFromUtf32(112) +
                char.ConvertFromUtf32(0) +
                char.ConvertFromUtf32(5) +
                char.ConvertFromUtf32(5));
                serialPort1.Close();
                serialPort1.Dispose();
                serialPort1 = null;
            }
            catch (Exception )
            {
                serialPort1.Close();
                serialPort1.Dispose();
                serialPort1 = null;
                // MessageBox.Show(ex.ToString() + "----" + CashDrawPortName + "----" + CashDrawBaudRate.ToString() + "----" + CashDrawDataBits.ToString());

            }
        }

    }
}
