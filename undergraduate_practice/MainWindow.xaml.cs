using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Math;

namespace undergraduate_practice
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Volter2System task;
        public MainWindow()
        {
            task = new Volter2System();
            task.Phi1 = (t) =>
            {
                return -Pow(t, 3.0f) / 6.0f - Pow(t, 2.0f) / 2.0f;
            };


            task.K11 = (t, tau) =>
            {
                return t - tau;
            };

            task.K12 = (t, tau) =>
            {
                return 1.0f;
            };

            task.Phi2 = (t) =>
            {
                //return t+1-5/3*Pow(t,3)-3/2*Pow(t,2);
                return -13.0f / 6.0f * Pow(t, 3.0f) - Pow(t, 2.0f) + t + 1.0f;
            };

            task.K21 = (t, tau) =>
            {
                return 2.0f * (t + tau);
            };

            task.K22 = (t, tau) =>
            {
                return t;
            };
            InitializeComponent();
        }


        private void CountButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                double t0 = double.Parse(this.t0.Text);
                double t1 = double.Parse(this.t1.Text);
                double h = double.Parse(GridSpaceText.Text);
                task.SetTimeRange(t0, t1);
                task.GridSpacing = h;
                double[] t_arr;
                task.SolveUsingRiemannSum(out List<double> g1, out List<double> g2, out t_arr);
                var Model = (MainViewModel)this.DataContext;
                Model.UpdateModel(g1, g2, t_arr, task.T0, task.T1, task.GridSpacing);
                t_array.ItemsSource = t_arr;
                g1_array.ItemsSource = g1;
                g2_array.ItemsSource = g2;
            }
            catch{

            }
        }
    }
}
