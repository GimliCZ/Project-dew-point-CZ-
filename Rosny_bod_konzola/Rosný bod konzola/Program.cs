using System;
using System.IO.Ports;
using System.IO;
using System.Text;
#pragma warning disable CA1416 // Ověřit kompatibilitu platformy

namespace Rosný_bod_konzola
{
    internal class Program
    {
        private static void Main(/*string[] args*/)
        {
            
            bool RegulatorActive = false;
            bool RegulatorRUN = false;
            bool Autofindmode = false;
            Serial_COM ComPort = new Serial_COM(); // Detekce portu, na kterém je arduino
            Serial_Analyzer Report = new Serial_Analyzer();
            ComPort.AutodetectArduinoPort();
            ComPort.Init_Port();
            ComPort.SerialComWrite("00000000;001;001;000");
            //Console.WriteLine(ComPort.COM_Adress_PUB + "Detekováno arduino");
            //-----------------------------------------------------------------------------//
            string path = @"C:\Users\vascu\Desktop\test\report.txt";
            string path2 = @"C:\Users\vascu\Desktop\test\regulator.txt";
            string path3 = @"C:\Users\vascu\Desktop\test\detekce.txt";
            //  File.Create(path);
            // File.Create(path2);
            StreamWriter vypissouboru = new StreamWriter(path);
            StreamWriter vypisregulatoru = new StreamWriter(path2);
            StreamWriter vypisdetekce = new StreamWriter(path3);

            Regulator H_Bridge_Control = new Regulator();
            H_Bridge_Control.e = 0;
            H_Bridge_Control.r0 = 80;
            H_Bridge_Control.Ti = 14;
            H_Bridge_Control.Td = 2;
            H_Bridge_Control.umin = -10;
            H_Bridge_Control.umax = 255;
            FindMode Automode = new FindMode();


            double requested_value = 0;
            double regulator_response = 0;
            double regulator_response_abs = 0;
            int x = 0;
            while (ComPort.SerialLink.IsOpen)
            {
                x++;
                string IncomingString = ComPort.SerialComRead();
                vypissouboru.WriteLine(IncomingString, 0, IncomingString.Length);
                Report.AnalyzeString(IncomingString);

               /* if (Math.Abs(Report.PT100TempSence - Report.EnvTempReport) > 10)
                {
                    ComPort.SerialComWrite("11111111;001;001;255");
                }*/
                if (Report.PT100TempSence > 30)
                {
                    ComPort.SerialComWrite("11111111;001;001;255");
                }
                if (Report.PT100TempSence < -5)
                {
                    ComPort.SerialComWrite("11111111;001;001;255");
                }
                if (Report.CoolerTempSence > 40)
                {
                    ComPort.SerialComWrite("11111111;001;001;255");
                }

                Console.Write(Report.LightSensorReport);
                Console.Write(' ');
                Console.Write(Report.EnvTempReport);
                Console.Write(' ');
                Console.Write(Report.CoolerTempSence);
                Console.Write(' ');
                Console.Write(Report.PT100TempSence);
                Console.Write(' ');
                Console.WriteLine(Report.AmpSence);




                if (x > 100)
                { // po 100 řádcích smaž konzoli
                    Console.Clear();
                    x = 0;
                };
                string userinput = "";

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            userinput = Console.ReadLine();
                            Console.WriteLine("You pressed Enter!");
                            break;

                        default:
                            break;
                    }
                }

                if (userinput.Length == 20)
                {
                    ComPort.SerialComWrite(userinput);
                }
                

                if (userinput.Contains("Fan")) // Fan 0 - 255
                {
                    int temp = (userinput[5] - '0') * 100 + (userinput[6] - '0') * 10 + (userinput[7] - '0');
                    if (temp > 255)
                    {
                        Console.WriteLine("Platný rozsah je 0 - 255");
                    }
                    else
                    {
                    }
                }
                if (userinput.Contains("regulatorON")) // Fan 0 - 255
                {
                    if (RegulatorActive == false)
                    {
                        RegulatorActive = true;
                    }

                }
                if (userinput.Contains("regulatorOFF")) // Fan 0 - 255
                {
                    if (RegulatorActive == true)
                    {
                        RegulatorActive = false;
                        RegulatorRUN = false;
                        ComPort.SerialComWrite("11111111;001;001;255");
                    }
                }
                if (userinput.Contains("FinddewpointOFF"))
                {
                    Autofindmode = false;
                    RegulatorRUN = false;
                    RegulatorActive = false;
                    ComPort.SerialComWrite("11111111;001;001;255");
                    System.Threading.Thread.Sleep(20);
                }
                if (userinput.Contains("FinddewpointON"))
                {
                    Autofindmode = true;
                    RegulatorRUN = true;
                }
                if (RegulatorActive == true && userinput.Length == 5)
                {
                    RegulatorRUN = true;
                    requested_value = (userinput[0] - '0') * 10 + (userinput[1] - '0') + (userinput[3] - '0')*0.1 + (userinput[4] - '0')*0.01;
                    H_Bridge_Control.PIDWithClamping(-requested_value, -Report.PT100TempSence);
                   // Console.Write(H_Bridge_Control.u);
                }
                
                if (userinput.Contains("exit"))
                {
                    vypissouboru.Close();
                    vypisregulatoru.Close();
                    vypisdetekce.Close();
                    ComPort.SerialComWrite("11111111;001;001;000");
                    System.Threading.Thread.Sleep(10000);
                    System.Environment.Exit(0);
                }
           
                if (Autofindmode)
                {
                    if (Automode.Dewpoint_Detect(Report.LightSensorReport)) //pokud je detekován rosný bod, tak
                    {
                        Console.WriteLine("Našel jsem rosný bod");
                       
                    }
                    
                    requested_value = Automode.Temperature_Set(Report.EnvTempReport);
                    Console.WriteLine("request" + requested_value);

                    Automode.Update(Report.LightSensorReport, Convert.ToDouble(Report.PT100TempSence));
                }
                if (RegulatorRUN == true)
                {
                    //var watch = System.Diagnostics.Stopwatch.StartNew();
                    regulator_response = -Math.Round(H_Bridge_Control.PIDWithClamping(requested_value, Report.PT100TempSence));
                    // watch.Stop();
                    // var elapsedMs = watch.ElapsedMilliseconds;
                    if (regulator_response < 0)
                    {
                        regulator_response_abs = Math.Abs(regulator_response);
                        Console.Write("11111111;" + regulator_response_abs.ToString("000") + ";001;255");
                        ComPort.SerialComWrite("11111111;" + regulator_response_abs.ToString("000") + ";001;255");

                    }
                    if (regulator_response == 0)
                    {
                        ComPort.SerialComWrite("11111111;001;001;255");
                        Console.Write("11111111;001;001;255");
                    }
                    if (regulator_response > 0)
                    {
                        Console.Write("11111111;001;" + regulator_response.ToString("000") + ";255");
                        ComPort.SerialComWrite("11111111;001;" + regulator_response.ToString("000") + ";255");

                    }
                    vypisregulatoru.Write(H_Bridge_Control.e + ";" + H_Bridge_Control.u + ";" + requested_value + ";" + H_Bridge_Control.I + ";" + Report.PT100TempSence, 0);
                    if (Automode.Dewpoint_Detect(Report.LightSensorReport))
                    {
                        vypisregulatoru.Write(";*\n");
                        vypisdetekce.WriteLine(Report.PT100TempSence + ";" + Report.EnvTempReport + ";" + Report.EnvPressureReport);

                    }
                    else
                    {
                        vypisregulatoru.Write("\n");
                    }

                }

            }
        }

        /*     public static bool SerialComStart(SerialPort port) //metoda navázání komunikace
             {
                 try
                 {
                     port.Open();
                 }
                 catch
                 {
                     Console.WriteLine("Chyba při otevírání portu!");
                     return false;
                 }
                 return true;
             }*/

        private void ArduinoControlFunction(SerialPort port)
        {
        }
    }
}