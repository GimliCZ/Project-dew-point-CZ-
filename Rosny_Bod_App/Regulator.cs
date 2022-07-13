using PropertyChanged;
using System;
using System.Globalization;

namespace Rosny_Bod_App
{
    [AddINotifyPropertyChangedInterface]
    public class Regulator
    {
        //Deklarace konstant regulátoru
        /*  H_Bridge_Control.e = 0;
              H_Bridge_Control.r0 = 80;
              H_Bridge_Control.Ti = 14;
              H_Bridge_Control.Td = 0.002;
              H_Bridge_Control.umin = -10;
              H_Bridge_Control.umax = 255;*/

        /// <summary>
        /// Konstanta obsahující odchylku od regulovaných hodnot
        /// </summary>
        public double e { get; set; } = 0;

        /// <summary>
        /// Proporcionální konstanta závislá na čase
        /// </summary>
        public double r0 { get; set; } = 80;

        /// <summary>
        /// Derivační konstanta závislá na čase
        /// </summary>
        public double Td { get; set; } = 0.002;

        /// <summary>
        /// Integrační konstanta závislá na čase
        /// </summary>
        public double Ti { get; set; } = 14;

        /// <summary>
        /// Akční zásah regulátoru
        /// </summary>
        public double u { get; set; }

        /// <summary>
        /// string u
        /// </summary>
        public string Su { get; set; }

        /// <summary>
        /// Minimální Akční zásah
        /// </summary>
        public double umin { get; set; } = -10;

        /// <summary>
        /// Maximální akční zásah
        /// </summary>
        public double umax { get; set; } = 255;

        /// <summary>
        /// Požadovaná hodnota
        /// </summary>
        public string B_requested_value { get; set; }

        /// <summary>
        /// Stará integrační proměnná
        /// </summary>
        private double oldI { get; set; }

        /// <summary>
        /// Stará odchylka
        /// </summary>
        private double olde { get; set; }

        /// <summary>
        /// Integrační zásah
        /// </summary>
        public double I { get; set; }

        /// <summary>
        /// Proporcionální zásah
        /// </summary>
        public double P { get; set; }

        /// <summary>
        /// Derivační zásah
        /// </summary>
        public double D { get; set; }

        /// <summary>
        /// čas cyklu [ms]
        /// </summary>
        public TimeSpan Ts { get; set; }

        /// <summary>
        /// String I
        /// </summary>
        public string SI { get; set; }

        /// <summary>
        /// String P
        /// </summary>
        public string SP { get; set; }

        /// <summary>
        /// String D
        /// </summary>
        public string SD { get; set; }

        /// <summary>
        /// String Ts
        /// </summary>
        public string STs { get; set; }

        /// <summary>
        /// Čas minulého cyklu
        /// </summary>
        private DateTime Timeold { get; set; }

        /// <summary>
        /// Čas současného cyklu
        /// </summary>
        private DateTime Time { get; set; }

        public Regulator()
        {
            Timeold = DateTime.MinValue;
        }

        public double PIDWithClamping(double RequestedValue, double RealValue)
        {
            B_requested_value = RequestedValue.ToString();
            e = RealValue - RequestedValue;
            Time = DateTime.Now;
            if (Timeold == DateTime.MinValue)
            {
                I = 100;
                P = r0 * e;
                D = 0;
            }
            else
            {
                Ts = Time - Timeold;
                if (Ti == 0)
                {
                    I = 0;
                }
                else
                {
                    I = (r0 * Ts.Milliseconds / 1000) / (2 * Ti) * (e + olde) + oldI;
                }
                P = r0 * e;
                D = (r0 * Td) / (Ts.Milliseconds * 0.0001) * (e - olde);
            }
            if (I + P + D > umax)
            {
                I = oldI;
                u = umax;
            }
            else if (I + P + D < umin)
            {
                I = oldI;
                u = umin;
            }
            else
            {
                u = I + P + D;
            }
            oldI = I;
            olde = e;
            Timeold = Time;
            if (double.IsNaN(u))
            {
                u = 0;
            }
            SD = D.ToString("F2", CultureInfo.CurrentCulture);
            SI = I.ToString("F2", CultureInfo.CurrentCulture);
            SP = P.ToString("F2", CultureInfo.CurrentCulture);
            Su = u.ToString("F2", CultureInfo.CurrentCulture);
            STs = Ts.Milliseconds.ToString(CultureInfo.CurrentCulture);

            return u;
        }
    }
}