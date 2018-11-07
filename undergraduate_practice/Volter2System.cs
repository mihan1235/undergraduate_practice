using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace undergraduate_practice
{
    public delegate double function1(double op1);
    public delegate double function2(double op1, double op2);

    /*
     * \left\{
     * \begin{aligned}
     * & g_1(t) = \int_0^t K_{11}(t,\tau)g_1(\tau) \diff \tau + \int_0^t K_{12}(t,\tau)
     * g_2(\tau) \diff \tau + \phi_1(t)\\
     * & g_2(t) = \int_0^t K_{21}(t,\tau) g_1(\tau) \diff \tau + \int_0^t K_{22}(t,\tau)
     * g_2(\tau) \diff \tau + \phi_2(t)\\
     * \end{aligned}
     * \right.
     * 
     * K_{11}, K_{22}, K_{21}, K{22}, \phi_1, \phi_2 are given.
     * 
     * we need to find g_1(t),g_2(t)
     * 
     * left rectangle formula:
     * 
     * g_1(t_0) = \phi_1(0)
     * g_2(t_0) = \phi_2(0)
     * 
     * \left\{
     * \begin{aligned}
     * & g_1(t_i) = \int_0^{t_i} K_{11} (t_i, \tau) g_1(\tau) \diff \tau + \int_0^{t_i} 
     * K_{12}(t_i,\tau) g_2(\tau) \diff \tau + \phi_1(t_i)\\
     * & g_2(t_i) = \int_0^{t_i} K_{21} (t_i, \tau) g_1(\tau) \diff \tau + \int_0^{t_i} 
     * K_{22}(t_i,\tau) g_2(\tau) \diff \tau + \phi_2(t_i)\\
     * \end{aligned}
     * \right.
     * 
     * \tiled g_1(t), \tilde g_2(t) are approximated functions
     * 
     * \left\{
     * \begin{aligned}
     * & \tilde g_1(t_i) = \sum_{k=0}^{i-1} K_{11} (t_i, t_k) \tilde g_1(t_k) h + \sum_{k=0}^{i-1} 
     * K_{12}(t_i,t_k) \tilde g_2(t_k) h + \phi_1(t_i)\\
     * & \tilde g_2(t_i) = \sum_{k=0}^{i-1} K_{21} (t_i, t_k) \tilde g_1(t_k) h + \sum_{k=0}^{i-1} 
     * K_{22}(t_i,t_k) \tilde g_2(t_k) h + \phi_2(t_i)\\
     * \end{aligned}
     * \right.
     */

    public class Volter2System
    {
        double h = 0.02f;
        public double GridSpacing
        {
            get
            {
                return h;
            }
            set
            {
                h = value;
            }
        }

        public double T0 { get; private set; } = 2.0f;
        public double T1 { get; private set; } = 3.0f;
        public void SetTimeRange(double a, double b)
        {
            this.T0 = a;
            this.T1 = b;
        }

        public function2 K11
        {
            get;
            set;
        }

        public function2 K12
        {
            get;
            set;
        }

        public function2 K21
        {
            get;
            set;
        }

        public function2 K22
        {
            get;
            set;
        }

        public function1 Phi1
        {
            get;
            set;
        }

        public function1 Phi2
        {
            get;
            set;
        }


        int num;

        double[] Make_t_Array()
        {
            double[] t = new double[(int)((T1 - T0) / h + 1)];
            //Console.WriteLine(a);
            //Console.WriteLine(b);
            num = (int)((T1 - T0) / h + 1);
            //Console.WriteLine(num);
            double tmp = T0;
            for (int i = 0; i < num; i = i + 1)
            {

                t[i] = tmp;
                tmp += h;
            }
            return t;
        }

        public void SolveUsingRiemannSum(out List<double> g_1, out List<double> g_2, out double[] t)
        {
            t = Make_t_Array();
            g_1 = new List<double>();
            g_2 = new List<double>();

            g_1.Add(Phi1(t[0]));
            g_2.Add(Phi2(t[0]));

            double A(double t_i)
            {
                return 1 - h * K11(t_i, t_i);
            }

            double B(double t_i)
            {
                return - h * K12(t_i, t_i);
            }

            double C(double t_i)
            {
                return - h * K21(t_i, t_i);
            }

            double D(double t_i)
            {
                return 1 - h * K22(t_i, t_i);
            }

            double E(int i, ref List<double> g1, ref List<double> g2, ref double[] t_arr)
            {
                double sum = 0;
                for (int j = 1; j <= i - 1; j++)
                {
                    sum += h * K11(t_arr[i], t_arr[j]) * g1.ElementAt(j);
                    sum += h * K12(t_arr[i], t_arr[j]) * g2.ElementAt(j);
                }
                sum += Phi1(t_arr[i]);
                return sum;
            }

            double F(int i, ref List<double> g1, ref List<double> g2, ref double[] t_arr)
            {
                double sum = 0;
                for (int j = 1; j <= i - 1; j++)
                {
                    sum += h * K21(t_arr[i], t_arr[j]) * g1.ElementAt(j);
                    sum += h * K22(t_arr[i], t_arr[j]) * g2.ElementAt(j);
                }
                sum += Phi2(t_arr[i]);
                return sum;
            }

            for (int i = 1; i < num; i = i + 1)
            {
                double E_i = E(i, ref g_1, ref g_2, ref t);
                double F_i = F(i, ref g_1, ref g_2, ref t);
                double A_i = A(t[i]);
                double B_i = B(t[i]);
                double C_i = C(t[i]);
                double D_i = D(t[i]);
                double Delta_i = A_i * D_i - C_i * B_i;
                double g1i = (E_i*D_i - F_i * B_i) / Delta_i;
                g_1.Add(g1i);
                double g2i = (E_i * C_i - F_i * A_i) / -Delta_i;
                g_2.Add(g2i);
            }
        }

        public void SolveUsingRiemannSum_old(out List<double> g_1, out List<double> g_2, out double[] t)
        {
            t = Make_t_Array();
            g_1 = new List<double>();
            g_2 = new List<double>();

            g_1.Add(Phi1(t[0]));
            g_2.Add(Phi2(t[0]));

            for (int i = 1; i < num; i = i + 1)
            {
                double sum_11 = 0;

                for (int k = 0; k <= i - 1; k = k + 1)
                {
                    sum_11 += K11(t[i], t[k]) * g_1.ElementAt(k) * h;
                }

                double sum_12 = 0;

                for (int k = 0; k <= i - 1; k++)
                {
                    sum_12 += K12(t[i], t[k]) * g_2.ElementAt(k) * h;
                }

                g_1.Add(sum_11 + sum_12 + Phi1(t[i]));

                double sum_21 = 0;

                for (int k = 0; k <= i - 1; k++)
                {
                    sum_21 += K21(t[i], t[k]) * g_1.ElementAt(k) * h;
                }

                double sum_22 = 0;

                for (int k = 0; k <= i - 1; k++)
                {
                    sum_22 += K22(t[i], t[k]) * g_2.ElementAt(k) * h;
                }

                g_2.Add(sum_21 + sum_22 + Phi2(t[i]));
            }
        }
    }
}
