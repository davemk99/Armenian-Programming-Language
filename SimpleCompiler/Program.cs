using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            LexicalAnalyzer lexer = new LexicalAnalyzer(File.ReadAllText(args[0]));
            lexer.Analyze();
            Parser parser = new Parser(lexer,args[1],args[2]);
            parser.Parse();
            parser.Compile();
            
         
            Console.Read();
        }
    }
}
