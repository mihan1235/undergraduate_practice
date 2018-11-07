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
                //return -Pow(t, 3.0f) / 6.0f - Pow(t, 2.0f) / 2.0f;
                double sum = 0;
                double t0 = task.T0;
                sum += -Pow(t, 3.0f) / 6.0f - Pow(t, 2.0f) / 2.0f + t / 2.0f * Pow(t0, 2.0f);
                sum += -Pow(t0, 3.0f) / 3.0f + Pow(t0, 2.0f) / 2.0f + t0;
                return sum;
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
                double sum = 0;
                double t0 = task.T0;
                sum += -13.0f / 6.0f * Pow(t, 3.0f) - Pow(t, 2.0f) + 3.0f / 2.0f * t * Pow(t0, 2.0f);
                sum += t * t0 + t + 2.0f / 3.0f * Pow(t0, 3.0f) + 1.0f;
                return sum;
                //return -13.0f / 6.0f * Pow(t, 3.0f) - Pow(t, 2.0f) + t + 1.0f;
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

        public class NumratedData
        {
            public int num
            {
                get;
                set;
            }
            public double obj
            {
                get;
                set;
            }
            public NumratedData(int num, double obj)
            {
                this.num = num;
                this.obj = obj;
            }
            public override string ToString()
            {
                return num.ToString() + ": " + obj.ToString();
            }
        }

        List<NumratedData> ConvertToNumerated(List<double> list)
        {
            List<NumratedData> list_num = new List<NumratedData>();
            int i = 0;
            foreach (var obj in list)
            {
                i++;
                list_num.Add(new NumratedData(i,obj));
            }
            return list_num;
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
                List<double> g1;
                List<double> g2;
                if (checkBox1.IsChecked == true)
                {
                    task.SolveUsingRiemannSum_old(out g1, out g2, out t_arr);
                }
                else
                {
                    task.SolveUsingRiemannSum(out g1, out g2, out t_arr);
                }
                
                var Model = (MainViewModel)this.DataContext;
                Model.UpdateModel(g1, g2, t_arr, task.T0, task.T1, task.GridSpacing);
                t_array.ItemsSource = ConvertToNumerated(t_arr.ToList());
                g1_array.ItemsSource = ConvertToNumerated(g1);
                g2_array.ItemsSource = ConvertToNumerated(g2);
            }
            catch{

            }
        }
    }
}
