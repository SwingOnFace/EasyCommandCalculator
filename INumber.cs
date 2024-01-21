using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCommandCalculator
{
    public interface INumber : INumerical
    {
        public INumerical Addition(INumerical number);

        public INumerical Division(INumerical number);

        public INumerical Multiplication(INumerical number);

        public INumerical Square(INumerical number);

        public INumerical SquareRoot(INumerical number);

        public INumerical Subtraction(INumerical number);
    }
}
