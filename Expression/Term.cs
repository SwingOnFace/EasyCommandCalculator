using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCommandCalculator
{
    public class Term : INumerical
    {
        public List<(INumerical number, INumerical power)> contents;

        public Term()
        {
            this.contents = new();
        }
        public Term(List<(INumerical number, INumerical power)> contents)
        {
            this.contents = contents;
        }

        public int lastContentIndex
        {
            get
            {
                return this.contents.Count - 1;
            }
        }

        public string GenerateString()
        {
            string res = String.Empty;
            foreach(var item in this.contents)
            {
                res += item.number.GenerateString();
                if(!RationalNumber.One.Equals(item.power))
                {
                    res += "^" + item.power.GenerateString();
                }
                res += "*";
            }
            res = res[..^1];
            return res;
        }

        public INumerical Simplify()
        {
            List<(INumerical number, INumerical power)> newContents = new();

            INumber number = RationalNumber.One;
            foreach(var item in contents)
            {
                (INumerical number, INumerical power) simplifiedItem = (item.number.Simplify(), item.power.Simplify());
                if (simplifiedItem.number is RationalNumber && simplifiedItem.power is RationalNumber)
                {
                    number = (INumber)number.Multiplication(RationalNumber.Square((RationalNumber)simplifiedItem.number, (RationalNumber)simplifiedItem.power));
                }
                else if (simplifiedItem.number is Expression.Expression && (simplifiedItem.power is RationalNumber))
                {
                    newContents.Add(simplifiedItem);
                }
                else
                {
                    newContents.Add(simplifiedItem);
                }
            }

            if(newContents.Count > 0)
            {
                return (INumerical)new Term(newContents);
            }
            else
            {
                return (INumerical)number;
            }
        }
    }
}