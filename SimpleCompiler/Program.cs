using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmenianProgrammingLanguage
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {


                LexicalAnalyzer lexer = new LexicalAnalyzer(File.ReadAllText(args[0]));
                lexer.Analyze();
                Parser parser = new Parser(lexer, args[1], args[2]);
                parser.Parse();
                parser.Compile();


                Console.Read();
            }
            else
            {
                Console.WriteLine("You will run executable with parameters FileName OutputFolder and OutputExecutable seperated with space");
                Console.ReadLine();
            }
        }
    }
}
