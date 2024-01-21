using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EasyCommandCalculator.Expression
{
    public class Expression : INumerical
    {
        public enum Operation
        {
            Empty,
            Addition,
            Multiplication,
            Division,
            Square,
            SquareRoot,
            Subtraction,
        }

        public static Dictionary<Operation, string> operationToString = new Dictionary<Operation, string>
        {
            {Operation.Empty, "" },
            {Operation.Addition, "+" },
            {Operation.Multiplication, "*" },
            {Operation.Division, "/" },
            {Operation.Square, "^" },
            {Operation.SquareRoot, "sqrt" },
            {Operation.Subtraction, "-" },

        };
        public static Dictionary<string, Operation> stringToOperation = operationToString.ToDictionary(kv => kv.Value, kv => kv.Key);

        public List<(Operation operation, INumerical number)> contents;

        public Expression()
        {
            this.contents = new();
        }
        public Expression(List<(Operation operation, INumerical number)> content)
        {
            this.contents = content;
        }

        #region 字符串转表达式

        private bool IsNumber(char c)
        {
            if ((c >= '0' && c <= '9') || c == '.')
            {
                return true;
            }
            return false;
        }
        private bool IsChanged(string current, char c)
        {
            if(current == String.Empty)
            {
                return false;
            }
            if(IsNumber(c) ^ IsNumber(current[0]))
            {
                return true;
            }
            if (!IsNumber(current[0]) && //不是数字
                (current[0] < 'a' || current[0] > 'z'))//不是字母
            {
                return true;
            }
            return false;
        }
        private (Operation operation, INumerical number) lastContent
        {
            get
            {
                try
                {
                    return contents[contents.Count - 1];
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    return (Operation.Empty, RationalNumber.Zero);
                }
            }
            set
            {
                try
                {
                    contents[contents.Count - 1] = value;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }
        }
        private void MultiplyLastContent(INumerical number)
        {
            Term term;
            if (lastContent.number is not Term)
            {
                term = new Term(new List<(INumerical number, INumerical power)>
                {
                    (lastContent.number, new RationalNumber(1)),
                });
            }
            else
            {
                term = (Term)lastContent.number;
            }
            term.contents.Add((number, new RationalNumber(1)));
            lastContent = (lastContent.operation, (INumerical)term);
        }
        private void SquareLastContent(INumerical power)
        {
            Term term;
            if (lastContent.number is not Term)
            {
                term = new Term(new List<(INumerical number, INumerical power)>
                {
                    (lastContent.number, power),
                });
            }
            else
            {
                term = (Term)lastContent.number;
                var lastTermContent = term.contents[term.lastContentIndex];

                if (RationalNumber.One.Equals(lastTermContent.power))
                {
                    lastTermContent.power = power;
                }
                else if (lastTermContent.power is INumber number)
                {
                    lastTermContent.power = number.Addition(power);
                }
                else
                {
                    throw new NotImplementedException("Currently, multiplication of expressions in power is not supported.");
                }

                term.contents[term.lastContentIndex] = lastTermContent;
                lastContent = (lastContent.operation, (INumerical)term);
            }
        }

        public Expression(string input)
        {
            this.contents = new();

            input = input.Replace(" ", "");
            input = input.Replace("（", "(");
            input += " ";

            Operation lastOperation = Operation.Empty;
            string current = String.Empty;
            string bracketed = String.Empty;
            bool lastCharIsNumber = false;
            int level = 0;//当前左右包了几个括号
            for (int i = 0; i < input.Length; i++)
            {
                bool ThisCharIsNumber = IsNumber(input[i]);
                if (IsChanged(current, input[i]))//如果上个不是数字而这个是，或者上个是这个不是，就认为遍历完了当前数字或符号
                {
                    //计算括号
                    if (current == "(")
                    {
                        level++;
                    }
                    else if (current == ")")
                    {
                        level--;
                    }

                    if (level > 0)
                    {
                        while (level > 0)//括号内打包送走
                        {
                            try
                            {
                                if (input[i] == '(')
                                {
                                    level++;
                                }
                                else if (input[i] == ')')
                                {
                                    level--;
                                }
                                bracketed += input[i];
                            }
                            catch (Exception e)
                            {
                                Console.Error.WriteLine(e);
                                return;
                            }
                            i++;
                        }
                        if (bracketed.Length > 1)//确保其不为空（上面运行完后会残留一个右括号
                        {
                            bracketed = bracketed[..^1];

                            //出了括号之后处理括号内
                            Expression innerExpression = new Expression(bracketed);
                            if (lastOperation == Operation.Empty)//如果前面没有写运算符
                            {
                                if (contents.Count == 0)//如果前面没东西，说明这是个废括号，用加法存储
                                {
                                    contents.Add((Operation.Addition, innerExpression));
                                }
                                else//那就是与前面相乘
                                {
                                    MultiplyLastContent(innerExpression);
                                }
                            }
                            else
                            {
                                if(lastOperation == Operation.Multiplication)
                                {
                                    MultiplyLastContent(innerExpression);
                                }
                                else if (lastOperation == Operation.Division)
                                {
                                    MultiplyLastContent(innerExpression);
                                    SquareLastContent(new RationalNumber(-1, 0));
                                }
                                contents.Add((lastOperation, innerExpression));
                            }

                            bracketed = String.Empty;
                        }
                    }
                    else
                    {
                        if (IsNumber(current[0]))//如果是数字
                        {
                            RationalNumber number = new RationalNumber(float.Parse(current));

                            if (lastOperation == Operation.Empty)//如果前面没有写运算符
                            {
                                if (contents.Count == 0)//如果前面没东西，说明这是算式中第一个数字，用加法存储
                                {
                                    contents.Add((Operation.Addition, number));
                                }
                                else//那就是与前面相乘
                                {
                                    MultiplyLastContent(number);
                                }
                            }
                            else
                            {
                                if (lastOperation == Operation.Multiplication)
                                {
                                    MultiplyLastContent(number);
                                }
                                else if (lastOperation == Operation.Square)
                                {
                                    SquareLastContent(number);
                                }
                                else if (lastOperation == Operation.Division)
                                {
                                    MultiplyLastContent(number);
                                    SquareLastContent(new RationalNumber(-1, 0));
                                }
                                else
                                {
                                    contents.Add((lastOperation, number));
                                }
                            }
                            lastOperation = Operation.Empty;
                        }
                        else
                        {
                            lastOperation = stringToOperation[current];
                        }
                    }

                    current = String.Empty;
                }
                current += input[i];
                lastCharIsNumber = ThisCharIsNumber;
            }

        }
        #endregion

        public INumerical Simplify()
        {
            List<(Operation operation, INumerical number)> newContents = new();
            INumber number = RationalNumber.Zero;

            foreach(var item in contents)
            {
                (Operation operation, INumerical number) sinplifiedItem = (item.operation, item.number.Simplify());
                if (sinplifiedItem.number is RationalNumber)
                {
                    switch (sinplifiedItem.operation)
                    {
                        case Operation.Addition:
                            number = (INumber)number.Addition(sinplifiedItem.number);
                            break;
                        case Operation.Subtraction:
                            number = (INumber)number.Subtraction(sinplifiedItem.number);
                            break;
                        case Operation.Multiplication:
                            number = (INumber)number.Multiplication(sinplifiedItem.number);
                            break;
                        case Operation.Division:
                            number = (INumber)number.Division(sinplifiedItem.number);
                            break;
                        case Operation.Square:
                            number = (INumber)number.Square(sinplifiedItem.number);
                            break;
                        default:
                            throw new Exception("no operation");
                    }
                }
                else
                {
                    newContents.Add(sinplifiedItem);
                }
            }

            if(newContents.Count == 0)
            {
                return (INumerical)number;
            }
            else
            {
                return (INumerical)new Expression(newContents);
            }
        }

        public string GenerateString()
        {
            string res = String.Empty;
            for(int i = 0;i < contents.Count;i++)
            {
                if (!(i == 0 && contents[i].operation == Operation.Addition))
                {
                    res += operationToString[contents[i].operation];
                }
                res += contents[i].number.GenerateString();
            }
            return res;
        }
    }
}