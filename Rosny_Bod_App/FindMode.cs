using PropertyChanged;
using System;
using System.Linq;

namespace Rosny_Bod_App
{
    [AddINotifyPropertyChangedInterface]
    public class FindMode
    {/// <summary>
     /// Proměnná držící posledních 100 hodnot teploty z PT100
     /// </summary>
        public double[] Temperature_PT100_Old { get; set; } = new double[100];

        /// <summary>
        /// Hledaná teplota
        /// </summary>
        private double Tested_Temperature_PT100 { get; set; } = 0;

        /// <summary>
        /// Záznam posledních 5 hodnot z fotoresistoru
        /// </summary>
        private int[] Light_Sence_Old { get; set; } = new int[5];

        /// <summary>
        /// Konstanta obsahující rozdíl mezi testovanou hodnotou a realnou hodnotou
        /// </summary>
        public double difference { get; set; } = 0;

        /// <summary>
        /// Konstanta čekání naplnění arraye Temperature_PT100_Old
        /// </summary>
        private int x { get; set; } = 0; //counter

        /// <summary>
        /// Konstanta držící současný stav fotoresistoru
        /// </summary>
        public double lightbackground { get; set; } = 0;

        /// <summary>
        /// Proměnná ovlivňující stoupání / klesání teploty
        /// </summary>
        private bool flip { get; set; } = false;

        /// <summary>
        /// Counter držící zahřívací čas
        /// </summary>
        private int t1 { get; set; } = 0;

        /// <summary>
        ///  Counter držící chladící čas
        /// </summary>
        private int t2 { get; set; } = 0;

        /// <summary>
        /// Couner resetu na okolní teplotu
        /// </summary>
        private double t3 { get; set; } = 0;

        /// <summary>
        /// konstanta ovlivňující rychlost poklesu teploty
        /// </summary>
        private double t1time { get; set; } = 10;

        /// <summary>
        /// Rosný bod?
        /// </summary>
        private bool dewpoint { get; set; } = false;

        /// <summary>
        /// Rychlost pádu teploty za tick (Počáteční)
        /// </summary>
        public double speed_Const { get; set; } = 10;

        /// <summary>
        /// Rychlost pádu teploty za tick (Měřící)
        /// </summary>
        public double speed2_Const { get; set; } = 300;

        /// <summary>
        /// Offset od světelného pozadí - zahřívání
        /// </summary>
        public double low_light_Const { get; set; } = 10;

        /// <summary>
        /// Offset od světelného pozadí - chlazení
        /// </summary>
        public double high_light_Const { get; set; } = 30;

        /// <summary>
        /// Počáteční krok
        /// </summary>
        public double inicialstep { get; set; } = 0.3;

        /// <summary>
        /// Měřící krok
        /// </summary>
        public double messuringstep { get; set; } = 0.05;

        /// <summary>
        /// Krok °C
        /// </summary>
        public double step { get; set; } = 0.05;

        /// <summary>
        /// Proměnná blokující vícenásobné nahrání pozadí
        /// </summary>
        public bool once { get; set; } = false;

        /// <summary>
        /// Proměnná obsahující pozadí + offset
        /// </summary>
        public double light_low { get; set; }

        /// <summary>
        /// Proměnná obsahující pozadí + offset
        /// </summary>
        public double light_high { get; set; }

        /// <summary>
        /// Výchozí teplota
        /// </summary>
        public double inicial_temperature { get; set; }

        /// <summary>
        /// Proměnná počítající ustálení
        /// </summary>
        private int stabilization_counter { get; set; } = 0;

        private double temperaturecheck;

        public void Get_background(int Light_Sence, double Temperature_env)
        {
            if (once == false)
            {
                lightbackground = Convert.ToDouble(Light_Sence); //měřené pozadí
                light_low = lightbackground + low_light_Const; // hodnota navrácení se k ochlazování
                light_high = lightbackground + high_light_Const; // hodnota navráceníse k ohřívání
                inicial_temperature = Math.Round(Temperature_env, 1); //výchozí teplota okolního prostředí
                if (inicial_temperature == 0)
                {
                    once = false;
                }
                else
                {
                    once = true;
                }
            }
        }

        public double Temperature_Set()
        {
            Tested_Temperature_PT100 = inicial_temperature; // nastav výchozí testovanou hodnotu na zaokrouhlenou hodnotu vnějšího prostředí

            if (x > 99) // po naplnění průměrovacího bufferu teploty
            {
                if (Math.Abs(Tested_Temperature_PT100 - difference - Queryable.Average(Temperature_PT100_Old.AsQueryable())) < 0.05) //pokud je rozdíl žádané teploty a realné teploty nižší nežli 0.03 tak
                {
                    if (flip == false) // ochlazování
                    {
                        t1++; //counter posunu teploty při chlazení
                        if (Light_Sence_Old[0] - Light_Sence_Old[4] > 2) { // pokud detekuješ velkou změnu světla, tak počkej s krokem
                            t1 = 0;
                        }
                        if (dewpoint)
                        {
                            t1time = speed2_Const;//udělej pokles teploty každých n tiků - standardní měření
                            step = messuringstep;
                        }
                        else
                        {
                            t1time = speed_Const;//-náběh zařízení
                            if (Light_Sence_Old[0] > light_low)
                            {
                                step = messuringstep;
                            }
                            else
                            {
                                step = inicialstep;
                            }
                        }
                        if (t1 > t1time)
                        {
                            difference += step;
                            t1 = 0;
                            t2 = 0;
                        }
                        temperaturecheck = Tested_Temperature_PT100;
                    }
                    if (flip == true) // zahřívání
                    {
                        t2++;
                        if (t2 > 100)
                        {
                            difference -= step;
                            t2 = 0;
                            t1 = 0;
                        }
                        /*  if (temperaturecheck+2< Tested_Temperature_PT100) //pokud při zahřívání zjistíš, že
                          {
                              Get_background(Queryable.Min(Light_Sence_Old.AsQueryable()),Queryable.Min(Temperature_PT100_Old.AsQueryable()));
                          }*/
                    }
                    if (Tested_Temperature_PT100 < 0 && flip == false) //Pojistka, která by neměla nastat - Pokud se zařízení podchladí pod 0, tak se vrať na výchozí hodnotu teploty
                    {
                        difference = 0;
                    }
                }
            }
            return Tested_Temperature_PT100 -= difference;
        }

        public bool Prepare_for_shutdown(double Temp_Env) // navedení teploty tařízení do stabilní polohy
        {
            difference = 0;
            if (Math.Abs(Queryable.Average(Temperature_PT100_Old.AsQueryable()) - Temp_Env) < 0.2)
            {
                stabilization_counter++;
            }
            if (stabilization_counter > 300)
            {
                return true;
            }
            else { return false; }
        }

        public bool Dewpoint_Detect()
        {
            if (x > 99) // po naplnění průměrovacího bufferu světla
            {
                /* if (lightbackground == 0)
                 {
                     lightbackground = 400;// Convert.ToInt32(Math.Round(Queryable.Average(Light_Sence_Old.AsQueryable())));
                 }*/

                if ((Queryable.Average(Light_Sence_Old.AsQueryable()) - light_high) > 3 && flip == false)
                {
                    flip = true;
                    dewpoint = true;
                    return true;
                }
                if ((Queryable.Average(Light_Sence_Old.AsQueryable()) - light_low) < 3 && flip == true)
                {
                    flip = false;
                    return false;
                }
            }
            return false;
        }

        public void Update(int Light_Sence, double Temperature_PT100, double Temp_Env)
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
            while (z < 99) // update teploty
            {
                Temperature_PT100_Old[z + 1] = Temperature_PT100_Old[z];
                z++;
            }
            x++;
            if (x > 100)
            {
                x = 101;
            }
            if (Math.Abs(inicial_temperature - Temp_Env) > 2)// Pokud se vnější teplota změní o 2°C, tak resetuj hledání
            {
                difference = 0;
                flip = false;
                dewpoint = false;
                t3++;
                t2 = 0; // vyneguj veškeré změny
                t1 = 0;
                if (t3 > 50000) // po stabilizaci
                {
                    once = false;
                    Get_background(Light_Sence, Temp_Env);
                }
            }
        }
        public void forceupdatebackground(double Temperature_env) {
            light_low = lightbackground + low_light_Const; // hodnota navrácení se k ochlazování
            light_high = lightbackground + high_light_Const; // hodnota navráceníse k ohřívání
            inicial_temperature = Math.Round(Temperature_env, 1);
        }
    }
}