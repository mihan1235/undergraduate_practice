using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using System.Windows;
using System.ComponentModel;

namespace undergraduate_practice
{
    public class MainViewModel: INotifyPropertyChanged

    {
        public MainViewModel(){ }

        void AddLineSeries(List<double> func_arr, string func_title, double[] t)
        {
            LineSeries LineSeries = new LineSeries();
            for (int i = 0; i < func_arr.Count(); i++)
            {
                LineSeries.Points.Add(new DataPoint(t[i], func_arr.ElementAt(i)));
            }
            LineSeries.Title = func_title;
            this.MyModel.Series.Add(LineSeries);
        }

        public void UpdateModel(List<double> g1, List<double> g2,  double[] t_arr, 
            double t_0, double t_1, double h)
        {
            this.MyModel = new PlotModel { Title = "Inverse Problem" };
            AddLineSeries(g1, "g1(t)", t_arr);
            AddLineSeries(g2, "g2(t)", t_arr);
            TestFunc((t) => t, t_0, t_1, h, "g1_test(t)");
            TestFunc((t) => 1+t, t_0, t_1, h, "g2_test(t)");
            OnPropertyChanged("MyModel");
        }

        public void TestFunc(Func<double, double> g_t_exact, 
            double t_0, double t_1, double h, string title)
        {
            //Func<double, double>  = (t) => t - Math.Pow(t, 2) / 2;
            //this.MyModel = new PlotModel { Title = "Inverse Problem" };
            this.MyModel.Series.Add(new FunctionSeries(g_t_exact, t_0, t_1,h, title));
            //OnPropertyChanged("MyModel");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
                
        }

        public PlotModel MyModel { get; private set; }

    }
}
