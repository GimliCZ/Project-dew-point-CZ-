using PropertyChanged;
using System;
using System.Globalization;

namespace Rosny_Bod_App
{
    [AddINotifyPropertyChangedInterface]
    public class Serial_Analyzer
    {
        public int safetybit1 { get; set; } //safety_current_left_on
        public int safetybit2 { get; set; } //safety_current_right_on
        public int safetybit3 { get; set; } //safety_relay_1_on
        public int safetybit4 { get; set; } //safety_relay_2_on
        public int LightSensorReport { get; set; }
        public float EnvTempReport { get; set; }
        public float EnvPressureReport { get; set; }
        public float AmpSence { get; set; }
        public double CoolerTempSence { get; set; }
        public double CoolerVoltSence { get; set; }
        public string CoolerTemoSencetext { get; set; }
        public float PT100TempSence { get; set; }

        public void AnalyzeString(string MergedString)
        {
            string[] Reports = MergedString.Split(';');
            char[] tempsafetychar = Reports[0].ToCharArray();
            safetybit1 = (int)(tempsafetychar[0] - '0');
            safetybit2 = (int)(tempsafetychar[0] - '0');
            safetybit3 = (int)(tempsafetychar[0] - '0');
            safetybit4 = (int)(tempsafetychar[0] - '0');
            LightSensorReport = Int32.Parse(Reports[1]);
            EnvTempReport = float.Parse(Reports[2], CultureInfo.InvariantCulture);
            EnvPressureReport = float.Parse(Reports[3], CultureInfo.InvariantCulture)+5;
            AmpSence = mapfloat(float.Parse(Reports[4]) + 27, 0, 1024, -20, 20);
            CoolerVoltSence = double.Parse(Reports[5]) / 1024 * 5;//
                                                                  // CoolerRessSence = 5 * 25000 / CoolerVoltSence - 25000;
            CoolerTempSence = CoolerVoltSence * 24.436;//Math.Round((3950 * 25) / (3950 + (25 * Math.Log(CoolerRessSence / 25))), 2);
            CoolerTemoSencetext = CoolerTempSence.ToString("F2", CultureInfo.CurrentCulture);
            //(Math.Log((double.Parse(Reports[5])) / 100000) * 5693 + 79000+ 11926 * Math.PI);
            PT100TempSence = float.Parse(Reports[6], CultureInfo.InvariantCulture);
        }

        public float mapfloat(float x, float y, float z, float a, float b)
        {
            float temp = (x - y) * (b - a) / (z - y) + a;
            return temp;
        }
    }
}