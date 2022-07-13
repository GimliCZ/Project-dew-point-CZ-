using LiveCharts;
using LiveCharts.Defaults;

namespace Rosny_Bod_App
{
    public class Graphs
    {/// <summary>
     /// počítadlo požadovaných proměnných
     /// </summary>
        private int x = 0;

        public ChartValues<ObservablePoint> Create_Graph(int Number_of_points, string name)
        {
            x = 1;
            ChartValues<ObservablePoint> Temp3 = new ChartValues<ObservablePoint>();

            while (x < Number_of_points)
            {
                Temp3.Add(new ObservablePoint(x, 0));
                x++;
            }
            return Temp3;
        }
    }
}