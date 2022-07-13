using System;
using System.Linq;

namespace Rosný_bod_konzola
{
    internal class FindMode
    {
        //public double Temperature_env = 0;
        //public double Temperature_PT100 = 0;
        public double[] Temperature_PT100_Old = new double[100];

        private double Tested_Temperature_PT100 = 0;

        //public int Light_Sence = 0;
        private int[] Light_Sence_Old = new int[5];

        public double Pressure_env = 0;

        //private bool Fastmode = false;
        public double difference = 0;

        private int x = 0; //counter

        //private int t = 0; //counter
        private int lightbackground = 0;

        private bool flip = false;

        private int t1 = 0;
        private int t2 = 0;
        private int t1time = 5;
        private bool firstdewpoint = false;
        public double Temperature_Set(double Temperature_env)
        {
            
            Tested_Temperature_PT100 = Math.Round(Temperature_env,1); // nastav výchozí testovanou hodnotu na zaokrouhlenou hodnotu vnějšího prostředí

            if (x > 99) // po naplnění průměrovacího bufferu teploty
            {
                if (Math.Abs(Tested_Temperature_PT100 - difference - Queryable.Average(Temperature_PT100_Old.AsQueryable())) < 0.03) //pokud je rozdíl žádané teploty a realné teploty nižší nežli 0.06 tak
                {
                    
                    
                        if (flip == false) {
                            t1++;
                        if (firstdewpoint)
                        {
                            t1time = 300;
                        }
                        else
                        {
                            t1time = 5;
                        }
                        if (t1 > t1time)
                        {
                           
                            difference = difference + 0.05;
                            t1 = 0;
                            t2 = 0;
                        }
                    }
                        if (flip == true)
                        {
                            t2++;
                            if (t2 > 100)
                            {
                                difference = difference - 0.05;
                              t2 = 0;
                              t1 = 0;
                            }
                        }
                      
                    
            
                    if (Tested_Temperature_PT100 < 0)
                    {
                        difference = 0;
                    }
                }
            }
            return Tested_Temperature_PT100 = Tested_Temperature_PT100 - difference;
        }

        public bool Dewpoint_Detect(int Light_Sence)
        {
            if (x > 99) // po naplnění průměrovacího bufferu světla
            {
                if (lightbackground == 0)
                {
                    lightbackground = 400;// Convert.ToInt32(Math.Round(Queryable.Average(Light_Sence_Old.AsQueryable())));
                }
                if ((Queryable.Average(Light_Sence_Old.AsQueryable()) - lightbackground) > 10 && flip == false)
                {
                    difference = difference - 0.5;
                    flip = true;
                    firstdewpoint = true;
                    return true;
                }
                if (Queryable.Average(Light_Sence_Old.AsQueryable()) - lightbackground < 3 && flip == true)
                {
                    flip = false;
                    return false;
                }
            }
            return false;
        }

        public void Update(int Light_Sence, double Temperature_PT100)
        {
            int y = 0;
            int z = 0;

            Light_Sence_Old[0] = Light_Sence;
            while (y < 4) // update světla
            {
                Light_Sence_Old[y + 1] = Light_Sence_Old[y];
                y++;
            }

            Temperature_PT100_Old[0] = Temperature_PT100;
            while (z < 99) // update světla
            {
                Temperature_PT100_Old[z + 1] = Temperature_PT100_Old[z];
                z++;
            }
            x++;
            if (x > 100)
            {
                x = 101;
            }
        }
    }
}