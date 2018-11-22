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
    using static Derivative;
    using static Rand;
    public partial class MainWindow : Window
    {
        Volter2System task;

        public static double h
        {
            get;
            private set;
        } = 0.01d;

        double RadToDeg(double rad)
        {
            return (rad * 180.0d) / Math.PI;
        }

        double DegToRad(double deg)
        {
            return (Math.PI * deg) / 180.0d;
        }

        bool bad_p_t = false;
        double delta = default(double);
        double inaccuracy = RandomDouble(-1, 1);

        void SetTaskFunctions()
        {
            double x1 = 0.0d;
            double x2 = DegToRad(90.0d);
            double a = 1;

            double f1(double x)
            {
                return Cos(x);
            }

            double f2(double x)
            {
                return Sin(x);
            }

            double Delta = f1(x1) * f2(x2) - f1(x2) * f2(x1);


            double A(double x, double t, double tau)
            {
                return FirstDerivative(f1, x + a * (t - tau), h) - FirstDerivative(f1, x - a * (t - tau), h);
            }

            double B(double x, double t, double tau)
            {
                return FirstDerivative(f2, x + a * (t - tau), h) - FirstDerivative(f2, x - a * (t - tau), h);
            }

            

            double P1(double t)
            {
                double sum = 0;
                sum += Cos(x1) * (a * t - Sin(a * t)) / (2 * Pow(a, 3));
                sum += Sin(x1) * (a * Exp(-0.5d * t) + 0.5d * Sin(a * t) - a * Cos(a * t)) / (a * (Pow(a, 2) + 0.25d));
                if (bad_p_t == true)
                {
                    sum += delta*RandomDouble(-1,1);
                }
                return sum;
            }

            double P2(double t)
            {
                double sum = 0;
                sum += Cos(x2) * (a * t - Sin(a * t)) / (2 * Pow(a, 3));
                sum += Sin(x2) * (a * Exp(-0.5d * t) + 0.5d * Sin(a * t) - a * Cos(a * t)) / (a * (Pow(a, 2) + 0.25d));
                if (bad_p_t == true)
                {
                    sum += delta* RandomDouble(-1, 1);
                }
                return sum;
            }

            task.Phi1 = (t) =>
            {
                return 1.0d / Delta * (SecondDerivative(P1, t, h) * f2(x2) -
                    SecondDerivative(P2, t, h) * f2(x1));
            };


            task.K11 = (t, tau) =>
            {
                return a / -2 * Delta * (f2(x2) * A(x1, t, tau) - f2(x1) * A(x2, t, tau));
            };

            task.K12 = (t, tau) =>
            {
                return a / -2 * Delta * (f2(x2) * B(x1, t, tau) - f2(x1) * B(x2, t, tau));
            };

            task.Phi2 = (t) =>
            {
                return 1.0d / -Delta * (SecondDerivative(P1, t, h) * f1(x2) -
                    SecondDerivative(P2, t, h) * f1(x1));
            };

            task.K21 = (t, tau) =>
            {
                return a / 2 * Delta * (f1(x2) * A(x1, t, tau) - f1(x1) * A(x2, t, tau));
            };

            task.K22 = (t, tau) =>
            {
                return a / 2 * Delta * (f1(x2) * B(x1, t, tau) - f1(x1) * B(x2, t, tau));
            };
        }


        public MainWindow()
        {
            task = new Volter2System();
            SetTaskFunctions();
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
                list_num.Add(new NumratedData(i, obj));
            }
            return list_num;
        }

        private void CountButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                double t1 = double.Parse(this.t1.Text);
                h = double.Parse(GridSpaceText.Text);
                if (checkBox1.IsChecked == true)
                {
                    delta = double.Parse(DeltaText.Text);
                    bad_p_t = true;
                }
                else
                {
                    bad_p_t = false;
                }
                task.SetTimeRange(0, t1);
                task.GridSpacing = h;
                double[] t_arr;
                task.SolveUsingRiemannSum(out List<double> g1, out List<double> g2, out t_arr);
                var Model = (MainViewModel)this.DataContext;
                Model.UpdateModel(g1, g2, t_arr, task.T0, task.T1);
                t_array.ItemsSource = ConvertToNumerated(t_arr.ToList());
                g1_array.ItemsSource = ConvertToNumerated(g1);
                g2_array.ItemsSource = ConvertToNumerated(g2);
            }
            catch
            {

            }
        }
    }
}
