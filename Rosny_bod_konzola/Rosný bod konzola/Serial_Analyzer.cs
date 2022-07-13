using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosný_bod_konzola
{
    class Serial_Analyzer //1111;0 - 1024;float;float;0-1024;0-1024;0-1024\n
    {
        public int safetybit1; //safety_current_left_on
        public int safetybit2; //safety_current_right_on
        public int safetybit3; //safety_relay_1_on
        public int safetybit4; //safety_relay_2_on
        public int  LightSensorReport;
        public float EnvTempReport;
        public float EnvPressureReport;
        public float AmpSence;
        public double CoolerTempSence;
        public double CoolerVoltSence;
        public double CoolerRessSence;
        public float PT100TempSence;
        public void AnalyzeString(string MergedString)
        {
            string[] Reports = MergedString.Split(";");
            char[] tempsafetychar = Reports[0].ToCharArray(); 
            safetybit1 = (int)(tempsafetychar[0]-'0');
            safetybit2 = (int)(tempsafetychar[0] - '0');
            safetybit3 = (int)(tempsafetychar[0] - '0');
            safetybit4 = (int)(tempsafetychar[0] - '0');
            LightSensorReport = Int32.Parse(Reports[1]);
            EnvTempReport = float.Parse(Reports[2], CultureInfo.InvariantCulture);
            EnvPressureReport = float.Parse(Reports[3], CultureInfo.InvariantCulture);
            AmpSence = mapfloat(float.Parse (Reports[4])+8,0,1024,-20,20);
            CoolerVoltSence = double.Parse(Reports[5]) / 1024 * 5;// https://www.digikey.com/en/maker/projects/how-to-measure-temperature-with-an-ntc-thermistor/4a4b326095f144029df7f2eca589ca54
            CoolerRessSence = 5*25000/CoolerVoltSence - 25000;
            CoolerTempSence = Math.Round((3950*25)/(3950 + (25*Math.Log(CoolerRessSence/25))),2);
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
