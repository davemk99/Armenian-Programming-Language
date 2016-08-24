using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    class Parser
    {
        public Collection<Token> Tokens { get; set; }
        private StringBuilder Code;
        private string AddedCode;
        private bool isParsed = false;
        public string AssemblyName { get; set; }
        public string AssemblyFolder { get; set; }
        private Dictionary<string, string> Variables;
        private bool canParsed;
        public Parser(LexicalAnalyzer lexer,string AssemblyFolder,string AssemblyName)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            canParsed = lexer.IsAnalyzed;
            Tokens = lexer.Tokens;
            Code =new StringBuilder( File.ReadAllText("template.txt"));
            Variables = new Dictionary<string, string>();
            this.AssemblyFolder = AssemblyFolder;
            this.AssemblyName = AssemblyName;
            
        }
        public void Parse()
        {

            if (canParsed)
            {
                StringBuilder str = new StringBuilder();
                
                foreach (var item in Tokens)
                {
                    if (item.TokenType.Key == "Number")
                    {
                        str.AppendLine(string.Format(CultureInfo.InvariantCulture,"double {0}={1};",item.Name,item.Value[0]));
                        Variables.Add(item.Name, "double");

                    }
                    if (item.TokenType.Key.Contains("Operator"))
                    {
                        
                        string tmp = "";
                        for (int i = 0; i < item.Value.Count; i++)
                        {
                            if (i==item.Value.Count-1)
                            {
                                tmp += string.Format(CultureInfo.InvariantCulture, "{0};", item.Value[i]);

                            }
                            else
                            {
                                tmp += string.Format(CultureInfo.InvariantCulture,"{0}{1}",item.Value[i],item.TokenType.Value);
                            }
                            
                            
                        }

                        str.AppendLine(string.Format(CultureInfo.InvariantCulture,"{0}={1}",item.Name,tmp));
                    }
                    if (item.TokenType.Key == "Output")
                    {
                        if (Variables[item.Name] == "Bool")
                        {
                            str.AppendLine(string.Format(CultureInfo.InvariantCulture, "Console.WriteLine({0});", string.Format(CultureInfo.InvariantCulture," {0}== true ? \"Ճիշտ\":\"Սխալ\"",item.Name)));
                        }

                        else
                        {
                            str.AppendLine(string.Format(CultureInfo.InvariantCulture, "Console.WriteLine({0});", item.Name));

                        }                    }
                    if (item.TokenType.Key == "Input")
                    {
                        if (Variables[item.Name] == "double")
                        {
                            str.AppendLine(string.Format(CultureInfo.InvariantCulture,"{0}=double.Parse(Console.ReadLine());",item.Name));
                            
                        }
                        if (Variables[item.Name] == "string")
                        {
                            str.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}=Console.ReadLine();", item.Name));
                        }
                    }
                    if (item.TokenType.Key == "String")
                    {
                        str.AppendLine(string.Format(CultureInfo.InvariantCulture, "string {0}=\"{1}\";", item.Name, item.Value.First()));
                        Variables.Add(item.Name, "string");
                    }
                    if (item.TokenType.Key=="Wait")
                    {
                        str.AppendLine(string.Format(CultureInfo.InvariantCulture, "Console.ReadLine();"));
                    }
                    if (item.TokenType.Key=="Bool")
                    {
                        str.AppendLine(string.Format(CultureInfo.InvariantCulture, "bool {0}={1};", item.Name, (item.Value.First() == "Ճիշտ" ? "true" : "false")));
                        Variables.Add(item.Name, "Bool");
                    }
                }
                
                isParsed = true;
                AddedCode = str.ToString(); 
            }
        }
        public void Compile()
        {

            if (isParsed)
            {
                string code = Code.Replace("insert it here", this.AddedCode).ToString();
                using (Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider())
                {
                    CompilerParameters parameters = new CompilerParameters();
                    parameters.GenerateExecutable = true;

                    parameters.OutputAssembly = Path.Combine(AssemblyFolder, AssemblyName);
                    var res = provider.CompileAssemblyFromSource(parameters, code);


                    foreach (var item in res.Errors)
                    {
                        Console.WriteLine(string.Format(CultureInfo.InvariantCulture,item.ToString()));
                    }
                }
            }
        } 
    }

}
