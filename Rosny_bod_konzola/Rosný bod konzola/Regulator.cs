using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosný_bod_konzola
{
    class Regulator
    {
        public double e;
        public double r0;
        public double Td;
        public double Ti;
        public double u;
        public double umin;
        public double umax;
        private double oldI;
        private double olde;
        public double I;
        private double P;
        private double D;
        private TimeSpan Ts;
        private DateTime Timeold;
        private DateTime Time;
        

        public Regulator()
        {
            Timeold = DateTime.UnixEpoch;
        }


        public double PIDWithClamping(double RequestedValue, double RealValue)
        {
            
            e = RealValue - RequestedValue;
            Time = DateTime.Now;
            if (Timeold == DateTime.UnixEpoch)
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
                    I = (r0 * Ts.Milliseconds / 1000) / (2 * Ti) * (e + olde)+oldI;
                }
                P = r0 * e;
                D = (r0 * Td / Ts.Milliseconds/1000) * (e - olde);
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
            if (double.IsNaN(u)) {
                u = 0;
            }
            return u;

        }
    }
}
