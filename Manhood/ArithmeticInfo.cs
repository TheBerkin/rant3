using System;
using System.Text;
using Manhood.Arithmetic;

namespace Manhood
{
    internal class ArithmeticInfo
    {
        private readonly bool _givesOutput;
        private readonly string _input;

        public ArithmeticInfo(string input, bool givesOutput)
        {
            _input = input;
            _givesOutput = givesOutput;
        }

        public static bool TryParse(Scanner scanner, out ArithmeticInfo output)
        {
            output = null;
            if (!scanner.Eat("(")) return false;
            bool givesOutput = !scanner.Eat("@");
            bool escapeNext = false;
            int balance = 1;
            var sb = new StringBuilder();
            while (balance > 0 && !scanner.EndOfString)
            {
                char c = scanner.ReadRawChar();
                if (!escapeNext)
                {
                    switch (c)
                    {
                        case '\\':
                            escapeNext = true;
                            sb.Append(c);
                            continue;
                        case '(':
                            balance++;
                            break;
                        case ')':
                            balance--;
                            if (balance == 0)
                            {
                                output = new ArithmeticInfo(sb.ToString(), givesOutput);
                                return true;
                            }
                            break;
                    }
                }
                escapeNext = false;
                sb.Append(c);
            }
            if (balance > 0)
            {
                throw new FormatException("Too many opening parentheses in arithmetic expression.");
            }
            return true;
        }

        public void Evaluate(Interpreter ii)
        {
            if (_givesOutput)
            {
                ii.Write(Parser.Calculate(ii, ii.Evaluate(_input)));
            }
            else
            {
                foreach (var expr in ii.Evaluate(_input).Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    Parser.Calculate(ii, expr);
                }
            }
        }
    }
}