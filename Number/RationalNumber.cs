using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCommandCalculator
{
    public class RationalNumber : INumber
    {
        int numerator, denominator;

        public RationalNumber()
        {
            this.numerator = 0;
            this.denominator = 1;
        }
        public RationalNumber(RationalNumber number)
        {
            this.numerator = number.numerator;
            this.denominator = number.denominator;
            this.Simplify();
        }
        public RationalNumber(int numerator, int denominator) 
        {
            this.numerator = numerator;
            this.denominator = denominator;
            this.Simplify();
        }
        public RationalNumber(float number)
        {
            const float error = 0.001f;
            while (true)
            {
                float val = numerator / (float)denominator;
                if (MathF.Abs(val - number) < error)
                {
                    this.Simplify();
                    return;
                }
                else if (val > number)
                {
                    denominator++;
                }
                else
                {
                    numerator++;
                }
            }//水多了加面，面多了加水
        }
        public RationalNumber(int number)
        {
            numerator = number;
            denominator = 1;
        }

        public static RationalNumber Zero
        {
            get
            {
                return new RationalNumber(0, 1);
            }
        }
        public static RationalNumber One
        {
            get
            {
                return new RationalNumber(1, 1);
            }
        }

        public void Simplify()
        {
            int gcd = GCD(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;
        }

        private static int LCM(int a, int b)
        {
            return Math.Abs(a * b) / GCD(a, b);
        }
        private static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static explicit operator int(RationalNumber number)
        {
            return number.numerator / number.denominator;
        }
        public static explicit operator float(RationalNumber number)
        {
            return number.numerator / (float)number.denominator;
        }
        public static explicit operator double(RationalNumber number)
        {
            return number.numerator / (double)number.denominator;
        }
        public static explicit operator string(RationalNumber number)
        {
            return (number.numerator).ToString() + "/" + (number.denominator).ToString();
        }

        public static bool operator ==(RationalNumber number1, RationalNumber number2)
        {
            number1.Simplify();
            number2.Simplify();
            if ((number1.numerator == number2.numerator) && (number1.numerator == number2.denominator))
            {
                return true;
            }
            return false;
        }
        public static bool operator ==(RationalNumber number1, int number2)
        {
            return (int)number1 == number2;
        }
        public static bool operator !=(RationalNumber number1, RationalNumber number2)
        {
            number1.Simplify();
            number2.Simplify();
            if ((number1.numerator != number2.numerator) || (number1.numerator != number2.denominator))
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(RationalNumber number1, int number2)
        {
            return (int)number1 != number2;
        }
        public override bool Equals(object? obj)
        {
            if (obj is RationalNumber other)
            {
                if ((this.numerator == other.numerator) && (this.numerator == other.denominator))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        public override int GetHashCode()
        {
            // 使用XOR操作符组合多个散列码
            return numerator.GetHashCode() ^ denominator.GetHashCode();
        }
        public static bool operator >(RationalNumber a, RationalNumber b)
        {
            // 先将两个分数转换为相同的分母
            int commonDenominator = LCM(a.denominator, b.denominator);
            int aNumerator = a.numerator * (commonDenominator / a.denominator);
            int bNumerator = b.numerator * (commonDenominator / b.denominator);

            // 然后比较分子
            return aNumerator > bNumerator;
        }
        public static bool operator <(RationalNumber a, RationalNumber b)
        {
            // 先将两个分数转换为相同的分母
            int commonDenominator = LCM(a.denominator, b.denominator);
            int aNumerator = a.numerator * (commonDenominator / a.denominator);
            int bNumerator = b.numerator * (commonDenominator / b.denominator);

            // 然后比较分子
            return aNumerator < bNumerator;
        }

        public static RationalNumber Addition(RationalNumber number1, RationalNumber number2)
        {
            int temp1 = number1.denominator;
            int temp2 = number2.denominator;

            int gcd = GCD(temp1, temp2);
            int a = temp1 / gcd;
            int b = temp2 / gcd;

            int numerator1 = number1.numerator * b;
            int numerator2 = number2.numerator * a;

            RationalNumber res = new RationalNumber(numerator1 + numerator2, a * b * gcd);
            res.Simplify();
            return res;
        }


        public static RationalNumber Division(RationalNumber number1, RationalNumber number2)
        {
            return new RationalNumber(number1.numerator * number2.denominator, number1.denominator * number2.numerator);
        }

        public static RationalNumber Multiplication(RationalNumber number1, RationalNumber number2)
        {
            return new RationalNumber(number1.numerator * number2.numerator, number1.denominator * number2.denominator);
        }

        public static RationalNumber Square(RationalNumber number1, RationalNumber number2)
        {
            if(number2 == 0)
            {
                return One;
            }
            if(number2.denominator == 1)
            {
                RationalNumber temp = new RationalNumber(number1);

                temp.numerator = (int)Math.Pow(number1.numerator, Math.Abs(number2.numerator));
                temp.denominator = (int)Math.Pow(number1.denominator, Math.Abs(number2.numerator));

                if(number2 < Zero)
                {
                    (temp.numerator, temp.denominator) = (temp.denominator, temp.numerator);
                }

                return temp;
            }
            else
            {
                throw new NotImplementedException("This calculator currently does not support non integer powers.");
            }
        }

        public static RationalNumber SquareRoot(RationalNumber number1, RationalNumber number2)
        {
            throw new NotImplementedException();
        }

        public static RationalNumber Subtraction(RationalNumber number1, RationalNumber number2)
        {
            int temp1 = number1.denominator;
            int temp2 = number2.denominator;

            int gcd = GCD(temp1, temp2);
            int a = temp1 / gcd;
            int b = temp2 / gcd;

            int numerator1 = number1.numerator * b;
            int numerator2 = number2.numerator * a;

            RationalNumber res = new RationalNumber(numerator1 - numerator2, a * b * gcd);
            res.Simplify();
            return res;
        }


        #region 实现INumber接口
        INumerical INumber.Addition(INumerical number)
        {
            if (number is RationalNumber rationalNumber)
            {
                return Addition(this, rationalNumber);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        INumerical INumber.Division(INumerical number)
        {
            if (number is RationalNumber rationalNumber)
            {
                return Division(this, rationalNumber);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        INumerical INumber.Multiplication(INumerical number)
        {
            if (number is RationalNumber rationalNumber)
            {
                return Multiplication(this, rationalNumber);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        INumerical INumber.Square(INumerical number)
        {
            if (number is RationalNumber rationalNumber)
            {
                return Square(this, rationalNumber);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        INumerical INumber.SquareRoot(INumerical number)
        {
            if (number is RationalNumber rationalNumber)
            {
                return SquareRoot(this, rationalNumber);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        INumerical INumber.Subtraction(INumerical number)
        {
            if (number is RationalNumber rationalNumber)
            {
                return Subtraction(this, rationalNumber);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public string GenerateString()
        {
            if(denominator == 1)
            {
                return numerator.ToString();
            }
            else
            {
                return "(" + numerator.ToString() + "/" + denominator.ToString() + ")";
            }
        }

        INumerical INumerical.Simplify()
        {
            this.Simplify();
            return this;
        }
        #endregion
    }
}
