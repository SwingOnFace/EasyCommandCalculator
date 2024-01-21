namespace EasyCommandCalculator
{
    public static class Program
    {
        static int Main(string[] args)
        {
            
            while (true)
            {
                string? command;
                Console.Write(">> ");
                command = Console.ReadLine();
                if (command == null)
                {
                    Console.WriteLine("请输入内容。");
                    continue;
                }
                Console.WriteLine(Run(command));
            }
        }

        static string Run(string command)
        {
            Expression.Expression expression = new Expression.Expression(command);
            INumerical numerical = expression.Simplify();
            return numerical.GenerateString();
        }
    }
}