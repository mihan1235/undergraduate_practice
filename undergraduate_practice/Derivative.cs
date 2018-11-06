using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace undergraduate_practice
{
    public  class Derivative
    {
        public delegate double tf(double op);

        public static double FirstDerivative(tf f,double x, double h)
        {
            //return (f(x + h) - f(x)) / h;
            return 1 / (6 * h) * (-11 * f(x) + 18 * f(x + h) - 9 * f(x + 2 * h) + 2 * f(x + 3 * h));
        }

        public static double SecondDerivative(tf f, double x, double h)
        {
            //return (f(x) - 2*f(x+h) + f(x + 2*h)) / Math.Pow(h, 2);
            return 1 / Math.Pow(h, 2) * (2*f(x) - 5 *f(x+h) + 4*f(x+2*h)-f(x+3*h));
        }
    }
}
