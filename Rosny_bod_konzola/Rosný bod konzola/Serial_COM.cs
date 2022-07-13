using System;
using System.IO.Ports;
using System.Management;

#pragma warning disable CA1416 // Ověřit kompatibilitu platformy

namespace Rosný_bod_konzola {
    internal class Serial_COM
    {
        public string COM_Adress_PUB
        {
            get { return COM_Adress; }
        }

        private string COM_Adress;
        public SerialPort SerialLink = new SerialPort();
        public bool Error;



        public void AutodetectArduinoPort() //zdroj.: https://stackoverflow.com/questions/3293889/how-to-auto-detect-arduino-com-port
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        COM_Adress = deviceId;
                    }
                }
            }
            catch (ManagementException e)
            {
                Console.WriteLine(e);
                COM_Adress = null;
            }

            if (COM_Adress == null)
            {
                Console.WriteLine("ARDUINO NEDETEKOVÁNO");
            }
            else
            {
                Console.WriteLine(COM_Adress + " Detekováno arduino");
            }
        }

        public void Init_Port()
        {
            SerialLink.PortName = COM_Adress;
            SerialLink.BaudRate = 115200;
            SerialLink.Parity = Parity.None;
            SerialLink.DataBits = 8;
            SerialLink.StopBits = StopBits.One;
            SerialLink.WriteTimeout = 100000;
            SerialLink.ReadTimeout = 100000;
            SerialLink.RtsEnable = true;
           // SerialLink.DtrEnable = true;
            try { SerialLink.Open(); }
            catch {
                Console.WriteLine("Chyba při otevírání portu!");
            }
        }
        public void SerialComWrite(string text)
        {
            // port.RtsEnable = true;
            try
            {
                SerialLink.WriteLine(text);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
            }
        }
        public string SerialComRead() // přečte jednu řádku
        {
            try
            {
                string message = SerialLink.ReadLine();
                if (message == string.Empty)
                {
                    //Console.WriteLine("Prázdný string!");
                    return "EMPTY";
                }
                else
                {
                    //Console.WriteLine(message);
                    return message;
                }
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                return "DOSLO K CHYBE PRI CTENI STRINGU!";
            }
            //Thread.Sleep(200);
        }
    }
}