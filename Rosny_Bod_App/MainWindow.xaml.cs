using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rosny_Bod_App
{
    [AddINotifyPropertyChangedInterface] //Implementace propperty change pro standardní proměnné
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Proměnná definuje velikost vykreslovaného grafu, po inicializaci neměnitelná
        /// </summary>
        private const int graphsize = 200;

        /// <summary>
        /// Pomocná proměnná pro inicializaci grafů
        /// </summary>
        private const double zero = 0;

        /// <summary>
        /// List naměřených teplot z PT100
        /// </summary>
        public List<double> TemperaturesList { get; set; } = new List<double>();

        /// <summary>
        /// Kolekce dat pro binding grafů
        /// </summary>
        public SeriesCollection CollectionPT100 { get; set; } = new SeriesCollection();

        public SeriesCollection CollectionPT100_2 { get; set; } = new SeriesCollection();

        /// <summary>
        /// Seznamy bodů
        /// </summary>
        public ChartValues<ObservablePoint> PointsPT100 { get; set; }

        public ChartValues<ObservablePoint> PointsPT100_2 { get; set; }

        /// <summary>
        /// List obsahující záznam o detekci rosného bodu, teplota PT100, teplota vnější, tlak, čas, relativní vlhkost
        /// </summary>
        public ObservableCollection<DetectionRecord> Record { get; set; } = new ObservableCollection<DetectionRecord>();

        /// <summary>
        /// List záznamu o proudu
        /// </summary>
        public float[] Amperage { get; set; } = new float[100];

        /// <summary>
        /// counter limitující velikost dat v arrayi
        /// </summary>
        public int AmperageCounter { get; set; } = 0;

        /// <summary>
        /// Výstupní proměnný pro UI
        /// </summary>
        public double AmperageShow { get; set; } = 0;

        /// <summary>
        /// Vlastnosti grafů befinující jejich tvar
        /// </summary>
        public LineSeries SeriesPT100_2 { get; set; } = new LineSeries();

        public LineSeries SeriesPT100 { get; set; } = new LineSeries();

        /// <summary>
        /// Interní timer pro obnovení grafického prostředí
        /// </summary>
        public int GraphUpdatetimer { get; set; } = 0;

        /// <summary>
        /// Interní timer pro záznam do listu teplot
        /// </summary>
        public int ListUpdatetimer { get; set; } = 0;

        /// <summary>
        /// Proměnná obsahující požadavek na dosaženou teplotu - Pro regulátor
        /// </summary>
        public double RequestedValue { get; set; } = 20;

        /// <summary>
        /// Proměnná vracející string z requested value
        /// </summary>
        public string RequestedString { get; set; }

        /// <summary>
        /// Reakce regulátoru
        /// </summary>
        public double RegulatorResponse { get; set; } = 0;

        /// <summary>
        /// Reakce regulátoru v absolutní hodnotě
        /// </summary>
        public double RegulatorResponseAbs { get; set; } = 0;

        /// <summary>
        /// Konstanta kontrolující aktivitu samonaváděcího režimu
        /// </summary>
        public bool AutomodeActive { get; set; } = false;

        /// <summary>
        /// Konstanta kontrolující aktivitu vlákna
        /// </summary>
        public bool ThreadActive { get; set; } = false;

        /// <summary>
        /// Konstanta kontrolující aktivitu regulátoru
        /// </summary>
        public bool RegulatorActive { get; set; } = false;

        /// <summary>
        /// Zpětná vazba UI na manuální zadání požadované teploty
        /// </summary>
        public string RequestTemperatureManualText { get; set; }

        /// <summary>
        /// Konstanta připravující program na ukončení spojení se zařízením
        /// </summary>
        public bool ReadyForDisconnect { get; set; } = false;

        /// <summary>
        /// Konstanta kontrolující korektnost manuálně zadané hodnoty
        /// </summary>
        public bool RequestOK { get; set; } = false;

        /// <summary>
        /// Manuálně nastavitelná hodnota teploty
        /// </summary>
        public double Number = 20.12;

        /// <summary>
        /// Deklarace funkce na založení grafu
        /// </summary>
        public Graphs Drawgraphs { get; set; } = new Graphs();

        /// <summary>
        /// Deklarace funkce pro seriovou komunikaci
        /// </summary>
        public Serial_COM ComPort { get; set; } = new Serial_COM();

        /// <summary>
        /// Deklarace funkce regulátoru
        /// </summary>
        public Regulator HBridgeControl { get; set; } = new Regulator();

        /// <summary>
        /// Deklarace funkce autonavádění
        /// </summary>
        public FindMode Automode { get; set; } = new FindMode();

        /// <summary>
        /// Funkce analyzující seriový provoz
        /// </summary>
        public Serial_Analyzer Report { get; set; } = new Serial_Analyzer();

        /// <summary>
        /// časovač
        /// </summary>
        public CustomTimer Atimer { get; set; } = new CustomTimer();

        /// <summary>
        /// Proměnná držící požadavek o ukončení
        /// </summary>
        public bool StopMessurementRequest { get; set; } = false;

        /// <summary>
        /// stavy vypínání
        /// </summary>
        public int MessurementStopStates { get; set; } = 0;

        /// <summary>
        /// proměnná kontrolující správnost hesla
        /// </summary>
        public bool PasswordOk { get; set; } = false;

        /// <summary>
        /// proměnná zamikající UI
        /// </summary>
        public bool NotReadableVariables { get; set; } = false;

        /// <summary>
        /// Funkce umožňující zápis do souborů
        /// </summary>
        public File_COM Writer { get; set; } = new File_COM();

        /// <summary>
        /// Proměnná držící dnešní datum
        /// </summary>
        public string CurrentDate { get; set; } = DateTime.Now.ToString("s", new CultureInfo("en-GB")).Replace(':', '-');

        /// <summary>
        /// debug log
        /// </summary>
        ///

        public ObservableCollection<string> ObservableDevices { get; set; } = new ObservableCollection<string>();

        public bool LogON { get; set; } = false;

        public bool AlertTriggered { get; set; } = false; // přehřátí 30°C

        public bool AlertTriggered2 { get; set; } = false; // podchlazení 0°C

        public bool AlertTriggered3 { get; set; } = false; // přehřátí chladiče 40°C

        public MainWindow()
        {
            InitializeComponent();
            ComPort.AutodetectArduinoPort(); //Navaž spojení s arduinem
            ComPort.GetlistOfSerialDevices(ObservableDevices);
            Writer.create_folder("Log"); //IncomingString + ";" + HBridgeControl.SD + ";" + HBridgeControl.SI + ";" + HBridgeControl.SP + ";" + HBridgeControl.u + ";" + Report.CoolerTempSence + ";"+ Report.AmpSence
            Writer.create_folder("Records");
            Writer.Add_to("Datalog" + CurrentDate + ".csv", "Log", "Bezpečnostní bity; napětí na fotoresistoru; teplota venku; tlak venku; proud; napětí na děliči NTC; PT100; Regulátor D; Regulátor I; Regulátor P; Regulátor zásah; Teplota chladiče přepočet; Proud přepočet", true);
            Writer.Add_to("Záznam" + CurrentDate + ".csv", "Records", "Datum;Vnejsi teplota °C;Vnitrni teplota °C;Tlak hPa;Relativní vlhkost %", true);
            // Inicializace Listu teplot PT100
            TemperaturesList = Enumerable.Repeat(zero, graphsize).ToList();

            // Inicializace 2 grafů
            PointsPT100 = Drawgraphs.Create_Graph(graphsize, "Teplota");
            PointsPT100_2 = Drawgraphs.Create_Graph(graphsize, "Teplota");

            ch2.AxisY.Add(
                     new Axis
                     {
                         MinValue = 0
                     });
            ch.AxisY.Add(
                     new Axis
                     {
                         MinValue = 0
                     });

            if (!(CollectionPT100 is null))
            {
                CollectionPT100.Clear();
            }
            if (!(CollectionPT100_2 is null))
            {
                CollectionPT100_2.Clear();
            }

            SeriesPT100.Title = "Teplota";
            SeriesPT100_2.Title = "Teplota";
            SeriesPT100.Values = PointsPT100;
            SeriesPT100_2.Values = PointsPT100_2;
            CollectionPT100.Add(SeriesPT100);
            CollectionPT100_2.Add(SeriesPT100_2);

            //Binding dat z recordu
            // Auto_Messuring_Results.ItemsSource = record;

            DataContext = this;
        }

        private void Connect_to_device_Click(object sender, RoutedEventArgs e)
        {
            if (!ComPort.SerialLink.IsOpen)
            {
                ComPort.Init_Port();
            }

            if (ComPort.SerialLink.IsOpen) // Pokud došlo k úspěšnému navázání komunikace, tak pravidelně kontroluj vstupní data
            {
                ReadyForDisconnect = false;
                if (ThreadActive == false) // pokud je puštěn pouze jeden thread
                {
                    Connect_to_device.Background = Brushes.Green;
                    Disconnect_from_device.Background = Brushes.LightGray;

                    ComPort.SerialComWrite("11111111;001;001;000"); // Po připojení spusť plnou komunikaci se zařízením

                    Thread workerThread2 = new Thread(() =>
                    {
                        ThreadActive = true; //zablokování puštění dalšího threadu
                        while (ReadyForDisconnect == false)
                        {
                            string IncomingString = ComPort.SerialComRead(); // Vezmi jeden příchozí string
                            if (ComPort.unexpected_termination)
                            {
                                ComPort.SerialLink.Close();
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    Connect_to_device.Background = Brushes.LightGray;
                                    Disconnect_from_device.Background = Brushes.Red;
                                    Status.Text = "Neaktivní";
                                    Status.Background = Brushes.LightGray;
                                });
                                break;
                            }
                            Report.AnalyzeString(IncomingString); // Analyzuj ho
                            Amperage[AmperageCounter] = Report.AmpSence;
                            if (AmperageCounter > 98)
                            {
                                AmperageCounter = -1;
                                AmperageShow = Math.Round(Queryable.Average(Amperage.AsQueryable()));
                            }
                            AmperageCounter++;
                            /// Bezpečnostní funkce
                            if (Report.PT100TempSence > 40)
                            {
                                ComPort.SerialComWrite("11111111;001;001;255");
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    Status.Text = "Ochrana proti přehřátí aktivní";
                                    Status.Background = Brushes.Orange;
                                });
                                AlertTriggered = true;
                            }
                            else
                            {
                                if (AlertTriggered)
                                {
                                    ComPort.SerialComWrite("11111111;001;001;000");
                                    App.Current.Dispatcher.Invoke(() =>
                                    {
                                        Status.Text = "Neaktivní";
                                        Status.Background = Brushes.LightGray;
                                    });
                                    AlertTriggered = false;
                                }
                            }
                            if (Report.PT100TempSence < 0)
                            {
                                StopMessurementRequest = true;
                                AlertTriggered2 = true;
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    Status.Text = "Ochrana proti podchlazení aktivní";
                                    Status.Background = Brushes.Orange;
                                });
                            }
                            else
                            {
                                if (AlertTriggered2)
                                {
                                    App.Current.Dispatcher.Invoke(() =>
                                    {
                                        Status.Text = "Neaktivní";
                                        Status.Background = Brushes.LightGray;
                                    });
                                    AlertTriggered2 = false;
                                }
                            }
                            if (Report.CoolerTempSence > 60) //V tento moment už je pravděpodobně Peltierův článek poškozen
                            {
                                StopMessurementRequest = true;
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    Status.Text = "Přehřátí chladiče";
                                    Status.Background = Brushes.Red;
                                });
                                AlertTriggered3 = true;
                            }
                            else
                            {
                                if (AlertTriggered3)
                                {
                                    App.Current.Dispatcher.Invoke(() =>
                                    {
                                        Status.Text = "Neaktivní";
                                        Status.Background = Brushes.LightGray;
                                    });
                                    AlertTriggered3 = false;
                                }
                            }

                           

                            // Modifikace UI a jeho refresh
                            ListUpdatetimer++;
                            if (ListUpdatetimer > 30)
                            { // obnova záznamového listu
                                TemperaturesList.RemoveAt(0);
                                TemperaturesList.Add(Report.PT100TempSence);
                                ListUpdatetimer = 0;
                            }

                            GraphUpdatetimer++;
                            if (GraphUpdatetimer > 30) //Obnova UI
                            {
                                App.Current.Dispatcher.Invoke(() =>
                                {
                                    Auto_Messuring_Results.Items.Refresh();
                                    for (int x = 0; x < graphsize - 1; x++)
                                    {
                                        PointsPT100.RemoveAt(0);
                                        PointsPT100_2.RemoveAt(0);
                                    }
                                    for (int x = 0; x < graphsize - 1; x++)
                                    {
                                        PointsPT100.Add(new ObservablePoint(x, TemperaturesList.ElementAt(x)));
                                        PointsPT100_2.Add(new ObservablePoint(x, TemperaturesList.ElementAt(x)));
                                    }
                                    GraphUpdatetimer = 0;
                                    if (StopMessurementRequest && MessurementStopStates == 0)
                                    {
                                        ReqConfirm.IsEnabled = false;
                                        Status.Text = "Stabilizuji teplotu regulátorem";
                                        Status.Background = Brushes.LightGreen;
                                    }
                                    if (StopMessurementRequest && MessurementStopStates == 1)
                                    {
                                        Status.Text = "Chladím " + Math.Abs(60 - Atimer.difference.TotalSeconds).ToString("F2", CultureInfo.CurrentCulture) + "s";
                                        Status.Background = Brushes.LightGreen;
                                    }
                                    if (!StopMessurementRequest && !RegulatorActive && !AutomodeActive)
                                    {
                                        Status.Text = "Neaktivní";
                                        Status.Background = Brushes.LightGray;
                                        Manual_Messurement_Stop.IsEnabled = true;
                                        Auto_Messurement_Stop.IsEnabled = true;
                                        Auto_Messurement_Start.IsEnabled = true;
                                        Manual_Messurement_Start.IsEnabled = true;
                                        Request_temperature_manual.IsEnabled = true;
                                        Disconnect_from_device.IsEnabled = true;
                                        ReqConfirm.IsEnabled = true;
                                    }

                                    if (Number > 1)
                                    {
                                        if (Number < Report.EnvTempReport)
                                        {
                                            RequestTemperatureManualText = "OK potvrďte zápis";
                                            RequestOK = true;
                                        }
                                        else
                                        {
                                            RequestTemperatureManualText = "teplota > okolí";
                                            RequestOK = false;
                                        }
                                    }
                                    else
                                    {
                                        RequestTemperatureManualText = "Moc nízká teplota";
                                        RequestOK = false;
                                    }
                                });
                            }
                            if (StopMessurementRequest)
                            {
                                if (MessurementStopStates == 0)
                                {

                                    RequestedValue = Math.Round(Report.EnvTempReport);
                                    RegulatorActive = true;
                                    Automode.Update(Report.LightSensorReport, Convert.ToDouble(Report.PT100TempSence), Convert.ToDouble(Report.EnvTempReport));
                                    if (Automode.Prepare_for_shutdown(Convert.ToDouble(Report.EnvTempReport)))
                                    {
                                        RegulatorActive = false;
                                        MessurementStopStates = 1;
                                    }

                                }
                                if (MessurementStopStates == 1)
                                {
                                    ComPort.SerialComWrite("11111111;001;001;255");
                                    Atimer.start_timer();
                                    if (Atimer.timer_enlapsed(60))
                                    {
                                        MessurementStopStates = 2;
                                        Atimer.stop_timer();
                                    }
                                }
                                if (MessurementStopStates == 2)
                                {
                                    ComPort.SerialComWrite("11111111;001;001;000");
                                    Atimer.start_timer();
                                    if (Atimer.timer_enlapsed(5))
                                    {
                                        HBridgeControl.I = 0;
                                        HBridgeControl.P = 0;
                                        HBridgeControl.u = 0;
                                        HBridgeControl.D = 0;
                                        HBridgeControl.SD = HBridgeControl.D.ToString("F2", CultureInfo.CurrentCulture);
                                        HBridgeControl.SI = HBridgeControl.I.ToString("F2", CultureInfo.CurrentCulture);
                                        HBridgeControl.SP = HBridgeControl.P.ToString("F2", CultureInfo.CurrentCulture);
                                        HBridgeControl.Su = HBridgeControl.u.ToString("F2", CultureInfo.CurrentCulture);
                                        MessurementStopStates = 0;
                                        StopMessurementRequest = false;
                                        Atimer.stop_timer();
        
                                    }
                                }
                            }
                            if (AutomodeActive)
                            { //pokud je aktivní automatické řízení
                              // pravidelně aktualizuj vstupní parametry
                                Automode.Get_background(Report.LightSensorReport, Report.EnvTempReport); // Nahraj výchozí hodnoty pro autommatické řízení
                                Automode.Update(Report.LightSensorReport, Convert.ToDouble(Report.PT100TempSence), Convert.ToDouble(Report.EnvTempReport));
                                if (Automode.Dewpoint_Detect()) //pokud je detekován rosný bod, tak
                                {
                                    // Console.WriteLine("Našel jsem rosný bod");
                                    // lock (record)
                                    // {
                                    App.Current.Dispatcher.Invoke(() => //udělej záznam do tabulky record
                                    {
                                        Record.Add(new DetectionRecord(Report.PT100TempSence, Report.EnvTempReport, Report.EnvPressureReport, DateTime.Now));
                                    });
                                    // }
                                }

                                RequestedValue = Automode.Temperature_Set(); //nastavení teploty regulátoru
                                RequestedString = RequestedValue.ToString("F4", CultureInfo.InvariantCulture);//zpětná vazba uživateli
                                                                                                            //   Console.WriteLine("request" + requested_value);
                            }

                            if (RegulatorActive) //Pokud je aktivní manuální režim proveď:
                            {
                                RegulatorResponse = -Math.Round(HBridgeControl.PIDWithClamping(RequestedValue, Report.PT100TempSence));
                                //Console.WriteLine(H_Bridge_Control.u);
                                if (RegulatorResponse < 0)
                                {
                                    RegulatorResponseAbs = Math.Abs(RegulatorResponse);
                                    //   Console.Write("11111111;" + regulator_response_abs.ToString("000") + ";001;255");
                                    ComPort.SerialComWrite("11111111;" + RegulatorResponseAbs.ToString("000") + ";001;255");
                                }
                                if (RegulatorResponse == 0)
                                {
                                    ComPort.SerialComWrite("11111111;001;001;255");
                                    //       Console.Write("11111111;001;001;255");
                                }
                                if (RegulatorResponse > 0)
                                {
                                    //          Console.Write("11111111;001;" + regulator_response.ToString("000") + ";255");
                                    ComPort.SerialComWrite("11111111;001;" + RegulatorResponse.ToString("000") + ";255");
                                }
                            }
                            if (LogON)
                            {
                                string data = IncomingString + ";" + HBridgeControl.SD + ";" + HBridgeControl.SI + ";" + HBridgeControl.SP + ";" + HBridgeControl.u + ";" + Report.CoolerTempSence + ";" + Report.AmpSence;
                                Writer.Add_to("Datalog" + CurrentDate + ".csv", "Log", data, true);
                            }
                        }
                        ThreadActive = false;
                        ComPort.unexpected_termination = false;
                    });
                    workerThread2.Start();
                }
            }
        }

        private void Disconnect_from_device_Click(object sender, RoutedEventArgs e)
        {
            int x = 0;
            ReadyForDisconnect = true;

            while (true)
            {
                ComPort.SerialComWrite("11111111;001;001;000");
                if (ThreadActive == false)
                {
                    ComPort.SerialLink.Close();
                }
                if (!ComPort.SerialLink.IsOpen)
                {
                    Connect_to_device.Background = Brushes.LightGray;
                    Disconnect_from_device.Background = Brushes.Red;
                    Status.Text = "Neaktivní";
                    Status.Background = Brushes.LightGray;
                    break;
                }
                x++;
                if (x > 1000)
                {
                    ComPort.SerialLink.Close();
                    if (!ComPort.SerialLink.IsOpen)
                    {
                        Connect_to_device.Background = Brushes.LightGray;
                        Disconnect_from_device.Background = Brushes.Red;
                        Status.Text = "Neaktivní";
                        Status.Background = Brushes.LightGray;
                        break;
                    }
                };
            }
        }

        private void Manual_Messurement_Start_Click(object sender, RoutedEventArgs e)
        {
            if (!ComPort.SerialLink.IsOpen)
            {
                MessageBox.Show("Zařízení není připojeno.");
                return;
            }
            Auto_Messurement_Start.IsEnabled = false;
            Manual_Messurement_Start.IsEnabled = false;
            Disconnect_from_device.IsEnabled = false;
            Auto_Messuring_Results.IsEnabled = false;
            Status.Text = "Manuální režim aktivní";
            Status.Background = Brushes.Green;
            RegulatorActive = true;
            ComPort.SerialComWrite("11111111;001;001;255"); // Po spuštění měření pust ventilátor na plno
        }

        private void Manual_Messurement_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (!ComPort.SerialLink.IsOpen)
            {
                MessageBox.Show("Zařízení není připojeno.");
                return;
            }
            Auto_Messurement_Stop.IsEnabled = false;
            Manual_Messurement_Stop.IsEnabled = false;
            Auto_Messuring_Results.IsEnabled = true;
            if (RegulatorActive)
            {
                StopMessurementRequest = true;
                Status.Text = "Zastavuji měření";
                Status.Background = Brushes.Yellow;
            }
            else
            {
                MessageBox.Show("Není co zastavovat.");
            }
        }

        private void Request_temperature_manual_Text_Changed(object sender, TextChangedEventArgs e)
        {
            var culture = CultureInfo.InvariantCulture;
            NumberStyles styles = NumberStyles.Number;

            string requested = Request_temperature_manual.Text;
            if (!requested.Contains(","))
            {
                if (double.TryParse(requested, styles, culture, out Number))
                {
                    if (Number > 1)
                    {
                        if (Number < Report.EnvTempReport)
                        {
                            RequestTemperatureManualText = "OK Potvrďte zápis";
                            RequestOK = true;
                        }
                        else
                        {
                            RequestTemperatureManualText = "teplota > okolí";
                            RequestOK = false;
                        }
                    }
                    else
                    {
                        RequestTemperatureManualText = "Moc nízká teplota";
                        RequestOK = false;
                    }
                }
                else
                {
                    RequestTemperatureManualText = "CHYBA";
                    RequestOK = false;
                }
            }
            else
            {
                RequestTemperatureManualText = "xx.xx";
                RequestOK = false;
            }
        }

        private void ReqConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (RequestOK)
            {
                RequestedValue = Number;
            }
            else
            {
                RequestedValue = Math.Round((double)Report.EnvTempReport, 2);
            }
        }

        private void Request_temperature_manual_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var culture = CultureInfo.InvariantCulture;
            NumberStyles styles = NumberStyles.Number;

            string requested = Request_temperature_manual.Text;
            if (!requested.Contains(","))
            {
                if (double.TryParse(requested, styles, culture, out Number))
                {
                    if (Number > -2)
                    {
                        if (Number < Report.EnvTempReport)
                        {
                            RequestTemperatureManualText = "OK";
                            RequestOK = true;
                        }
                        else
                        {
                            RequestTemperatureManualText = "teplota > okolí";
                            RequestOK = false;
                        }
                    }
                    else
                    {
                        RequestTemperatureManualText = "Moc nízká teplota";
                        RequestOK = false;
                    }
                }
                else
                {
                    RequestTemperatureManualText = "CHYBA";
                    RequestOK = false;
                }
            }
            else
            {
                RequestTemperatureManualText = "xx.xx";
                RequestOK = false;
            }
        }

        private void Auto_Messurement_Start_Click(object sender, RoutedEventArgs e)
        {
            if (!ComPort.SerialLink.IsOpen)
            {
                MessageBox.Show("Zařízení není připojeno.");
                return;
            }
            Auto_Messurement_Start.IsEnabled = false;
            Manual_Messurement_Start.IsEnabled = false;
            Request_temperature_manual.IsEnabled = false;
            Disconnect_from_device.IsEnabled = false;
            Auto_Messuring_Results.IsEnabled = false;
            Status.Text = "Automatický režim aktivní";
            Status.Background = Brushes.Green;
            AutomodeActive = true;
            RegulatorActive = true;
            ComPort.SerialComWrite("11111111;001;001;255");
        }

        private void Auto_Messurement_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (!ComPort.SerialLink.IsOpen)
            {
                MessageBox.Show("Zařízení není připojeno.");
                return;
            }
            AutomodeActive = false;
            Auto_Messurement_Stop.IsEnabled = false;
            Manual_Messurement_Stop.IsEnabled = false;
            Auto_Messuring_Results.IsEnabled = true;
            if (RegulatorActive)
            {
                StopMessurementRequest = true;
                Status.Text = "Zastavuji měření";
                Status.Background = Brushes.Yellow;
            }
            else
            {
                MessageBox.Show("Není co zastavovat.");
            }
        }

        private void Password_Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (Password_box.Password.ToString() == "dew point" && (Agreement.IsChecked is true))
            {
                PasswordOk = true;
                MessageBox.Show("Heslo přijato");
            }
            else
            {
                PasswordOk = false;
                MessageBox.Show("Heslo nebylo správné");
            }
            NotReadableVariables = !PasswordOk;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            Automode.forceupdatebackground(Report.EnvTempReport);
            var culture = CultureInfo.InvariantCulture;
            NumberStyles styles = NumberStyles.Number;
            double r0 = 0;
            if (PasswordOk)
            {
                if (!r0_TextBox.Text.Contains(","))
                {
                    if (double.TryParse(r0_TextBox.Text, styles, culture, out r0))
                    {
                        HBridgeControl.r0 = r0;
                    }
                    else
                    {
                        MessageBox.Show("Nastala chyba při převodu čísla u proměnné r0.");
                    }
                }
                else
                {
                    MessageBox.Show("Proměnná r0 obsahuje ',' nahraďte ji prosím '.'");
                }

                double Ti = 0;
                if (!Ti_TextBox.Text.Contains(","))
                {
                    if (double.TryParse(Ti_TextBox.Text, styles, culture, out Ti))
                    {
                        HBridgeControl.Ti = Ti;
                    }
                    else
                    {
                        MessageBox.Show("Nastala chyba při převodu čísla u proměnné Ti.");
                    }
                }
                else
                {
                    MessageBox.Show("Proměnná Ti obsahuje ',' nahraďte ji prosím '.'");
                }

                double Td = 0;
                if (!Td_TextBox.Text.Contains(","))
                {
                    if (double.TryParse(Td_TextBox.Text, styles, culture, out Td))
                    {
                        HBridgeControl.Td = Td;
                    }
                    else
                    {
                        MessageBox.Show("Nastala chyba při převodu čísla u proměnné Td.");
                    }
                }
                else
                {
                    MessageBox.Show("Proměnná Td obsahuje ',' nahraďte ji prosím '.'");
                }

                double low_light = 0;
                if (!Low_Light_TextBox.Text.Contains(","))
                {
                    if (double.TryParse(Low_Light_TextBox.Text, styles, culture, out low_light))
                    {
                        Automode.low_light_Const = low_light;
                    }
                    else
                    {
                        MessageBox.Show("Nastala chyba při převodu čísla u proměnné Hranice chlazení.");
                    }
                }
                else
                {
                    MessageBox.Show("Proměnná Hranice chlazení obsahuje ',' nahraďte ji prosím '.'");
                }
            }
            else
            {
                MessageBox.Show("Heslo není správné!");
            }

            double high_light = 0;
            if (!High_Light_TextBox.Text.Contains(","))
            {
                if (double.TryParse(High_Light_TextBox.Text, styles, culture, out high_light))
                {
                    Automode.high_light_Const = high_light;
                }
                else
                {
                    MessageBox.Show("Nastala chyba při převodu čísla u proměnné Hranice zahřívání.");
                }
            }
            else
            {
                MessageBox.Show("Proměnná Hranice zahřívání obsahuje ',' nahraďte ji prosím '.'");
            }
            double speed = 0;
            if (!speed_TextBox.Text.Contains(","))
            {
                if (double.TryParse(speed_TextBox.Text, styles, culture, out speed))
                {
                    Automode.speed_Const = speed;
                }
                else
                {
                    MessageBox.Show("Nastala chyba při převodu čísla u proměnné Zpoždění náběhu.");
                }
            }
            else
            {
                MessageBox.Show("Proměnná Zpoždění náběhu obsahuje ',' nahraďte ji prosím '.'");
            }

            double speed2 = 0;
            if (!speed2_TextBox.Text.Contains(","))
            {
                if (double.TryParse(speed2_TextBox.Text, styles, culture, out speed2))
                {
                    Automode.speed2_Const = speed2;
                }
                else
                {
                    MessageBox.Show("Nastala chyba při převodu čísla u proměnné Zpoždění měření.");
                }
            }
            else
            {
                MessageBox.Show("Proměnná Zpoždění měření obsahuje ',' nahraďte ji prosím '.'");
            }

            double messuringstep = 0;
            if (!MessuringStep_TextBox.Text.Contains(","))
            {
                if (double.TryParse(MessuringStep_TextBox.Text, styles, culture, out messuringstep))
                {
                    Automode.messuringstep = messuringstep;
                }
                else
                {
                    MessageBox.Show("Nastala chyba při převodu čísla u proměnné Měřící krok.");
                }
            }
            else
            {
                MessageBox.Show("Proměnná Měřící krok obsahuje ',' nahraďte ji prosím '.'");
            }

            double InicialStep = 0;
            if (!InicialStep_TextBox.Text.Contains(","))
            {
                if (double.TryParse(InicialStep_TextBox.Text, styles, culture, out InicialStep))
                {
                    Automode.inicialstep = InicialStep;
                }
                else
                {
                    MessageBox.Show("Nastala chyba při převodu čísla u proměnné Měřící krok.");
                }
            }
            else
            {
                MessageBox.Show("Proměnná Měřící krok obsahuje ',' nahraďte ji prosím '.'");
            }
            MessageBox.Show("Proměnné úspěšně zapsány.");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

            /* while (true)
             {*/

            if (ThreadActive == false)
            {
                //ComPort.SerialComWrite("11111111;001;001;000");
                ComPort.SerialLink.Close();
                // break;
            }
            else {
                e.Cancel = true;
                MessageBox.Show("Zastavte měření a odpojte zařízení.");
            }

            App.Current.Dispatcher.Invoke(() =>
            {
            });
            //}
        }

        private void Export_list_Click(object sender, RoutedEventArgs e)
        {
            string data = "";
            foreach (var DetectionRecord in Record)
            {
                data += DetectionRecord.Time.ToString("f", new CultureInfo("en-GB")) + ";" + DetectionRecord.ENV_Temperature.ToString() + ";" + DetectionRecord.PT100_Temperature.ToString() + ";" + DetectionRecord.ENV_Pressure.ToString() + ";" + DetectionRecord.Humidity.ToString() + System.Environment.NewLine;
            }

            Writer.Add_to("Záznam" + CurrentDate + ".csv", "Records", data, true);
        }

        private void Remove_row_Click(object sender, RoutedEventArgs e)
        {
            int selectedRowCount = Auto_Messuring_Results.SelectedIndex;
            if (selectedRowCount > -1)
            {
                Record.RemoveAt(Auto_Messuring_Results.SelectedIndex);
            }
        }

        private void Add_row_Click(object sender, RoutedEventArgs e)
        {
            int selectedRowCount = Auto_Messuring_Results.SelectedIndex;
            if (selectedRowCount > -1)
            {
                Record.Insert(Auto_Messuring_Results.SelectedIndex, new DetectionRecord(Report.PT100TempSence, Report.EnvTempReport, Report.EnvPressureReport, DateTime.Now));
            }
            else
            {
                Record.Add(new DetectionRecord(Report.PT100TempSence, Report.EnvTempReport, Report.EnvPressureReport, DateTime.Now));
            }
        }

        private void Debug_Checked(object sender, RoutedEventArgs e)
        {
            if (Debug.IsChecked is true)
            {
                LogON = true;
            }
            else
            {
                LogON = false;
            }
        }

        private void Debug_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Debug.IsChecked is true)
            {
                LogON = true;
            }
            else
            {
                LogON = false;
            }
        }

        private void Run_fan_Click(object sender, RoutedEventArgs e)
        {
            if (!ComPort.SerialLink.IsOpen)
            {
                MessageBox.Show("Zařízení není připojeno.");
                return;
            }
            ComPort.SerialComWrite("11111111;001;001;255");
        }
    }
}
