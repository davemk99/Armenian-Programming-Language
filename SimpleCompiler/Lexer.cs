using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ArmenianProgrammingLanguage
{

    public class LexicalAnalyzer
    {
        private List<ITokenDefinition> tokenDefinitions;
        public Collection<Token> Tokens { get;private set; }
        public bool IsAnalyzed { get;private set; }
        
        public string SourceCode { get; private set; }
        public LexicalAnalyzer(string source)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            tokenDefinitions = new List<ITokenDefinition>();
            tokenDefinitions.Add(new BoolDefinition(@"Տրամաբանական\s+([\u0530-\u058F]+)=(Ճիշտ|Սխալ);"));
            tokenDefinitions.Add(new OutputDefinition(@"Տպել (.*);"));
            tokenDefinitions.Add(new NumberDefinition(@"Թիվ\s+([\u0530-\u058F]+)=(\d|.)+;"));
            tokenDefinitions.Add(new OperatorAssignment(@"([\u0530-\u058F]+)=([\u0530-\u058F]|\d)+;"));
            tokenDefinitions.Add(new OperatorPlusDefinition(@"([\u0530-\u058F]+)=(.+)\+(.+);"));
            tokenDefinitions.Add(new InputDefinition(@"Ներմուծել ([\u0530-\u058F]+);"));
            tokenDefinitions.Add(new OperatorMinusDefinition(@"([\u0530-\u058F]+)=(.+)\-(.+);"));
            tokenDefinitions.Add(new OperatorDivideDefinition(@"([\u0530-\u058F]+)=(.+)\/(.+);"));
            tokenDefinitions.Add(new OperatorMultiplyDefinition(@"([\u0530-\u058F]+)=(.+)\*(.+);"));
            tokenDefinitions.Add(new StringDefinition("Տող\\s+([\u0530-\u058F]+)=\"(.*)\";"));
            tokenDefinitions.Add(new WaitDefinition(@"Սպասել;"));
            Tokens = new Collection<Token>();
            SourceCode = source;
            IsAnalyzed = false;
        }
        public void Analyze()
        {

            foreach (var item in SourceCode.Split(';'))
            {
                foreach (var def in tokenDefinitions)
                {
                    var match = def.Regex.Match(item.Trim() + "; ");
                    if (match.Success&&match.Length==item.Trim().Length+1)
                    {
                        
                        Token token = new Token();
                        token.Name = match.Groups[1].Value;
                        token.TokenType = def.Type;
                        
                        for (int i = 2; i < match.Groups.Count; i++)
                        {
                            token.AddValue(match.Groups[i].Value ?? null);
                        }
                        Tokens.Add(token);
                        break;
                    }

                }
                  
            }            
            IsAnalyzed = true;
        }
    }
    public interface ITokenDefinition
    {
        Regex Regex { get; set; }
        KeyValuePair<string,string> Type { get; }
        

    }
    public class OperatorPlusDefinition:ITokenDefinition
    {
        public Regex Regex { get; set; }
        public KeyValuePair<string,string> Type { get;private set; }
        public OperatorPlusDefinition(string pattern)
        {
            Regex = new Regex(pattern);
            Type = new KeyValuePair<string, string>("Operator+", "+");
        }
    }
    public class OperatorDivideDefinition:ITokenDefinition
    {
        public Regex Regex { get; set; }
        public KeyValuePair<string,string> Type { get;private set; }
        public OperatorDivideDefinition(string pattern)
        {
            Regex = new Regex(pattern);
            Type = new KeyValuePair<string, string>("Operator/", "/");

        }
    }
    public class NumberDefinition : ITokenDefinition
    {
        public Regex Regex { get; set; }
        public KeyValuePair<string,string> Type { get;private set; }
        public NumberDefinition(string pattern)
        {
            Regex = new Regex(pattern);
            this.Type = new KeyValuePair<string, string>("Number", "Թիվ");
        }
    }
    public class OperatorAssignment:ITokenDefinition
    {
        public Regex Regex { get; set; }
        public KeyValuePair<string,string> Type { get; set; }
        public OperatorAssignment(string pattern)
        {
            Regex = new Regex(pattern);
            Type = new KeyValuePair<string, string>("Operator=", "=");
        }
    }
    public class OutputDefinition:ITokenDefinition
    {
        public Regex Regex { get; set; }
        public KeyValuePair<string,string> Type { get; private set; }
        public OutputDefinition(string pattern)
        {
            Regex = new Regex(pattern);
            Type = new KeyValuePair<string, string>("Output", "Տպել");
        }
    }
    public class BoolDefinition : ITokenDefinition
    {
        public Regex Regex { get; set; }
        public KeyValuePair<string,string> Type { get;private set; }
        public BoolDefinition(string pattern)
        {
            Regex = new Regex(pattern);
            Type = new KeyValuePair<string, string>("Bool", "Տրամաբանական");

        }
    }
    public class InputDefinition : ITokenDefinition
    {
        public Regex Regex { get; set; }
        public KeyValuePair<string,string> Type { get; set; }
        public InputDefinition(string pattern)
        {
            Regex = new Regex(pattern);
            Type = new KeyValuePair<string, string>("Input", "Ներմուծել");
        }
    }
    public class OperatorMinusDefinition : ITokenDefinition
    {
        public Regex Regex { get; set; }
        public KeyValuePair<string,string> Type { get;private set; }
        public OperatorMinusDefinition(string pattern)
        {
            Regex = new Regex(pattern);
            Type = new KeyValuePair<string, string>("Operator-", "-");
        }
    }
    public class OperatorMultiplyDefinition : ITokenDefinition
    {
        public Regex Regex { get; set; }
        public KeyValuePair<string,string> Type { get;private set; }
        public OperatorMultiplyDefinition(string pattern)
        {
            Regex = new Regex(pattern);
            Type = new KeyValuePair<string, string>("Operator*", "*");
        }
    }
    public class StringDefinition : ITokenDefinition
    {
        public Regex Regex { get; set; }
        public KeyValuePair<string,string> Type { get;private set; }
        public StringDefinition(string pattern)
        {
            Regex = new Regex(pattern);
            Type = new KeyValuePair<string, string>("String", "Տող");
        }
    }
    public class WaitDefinition : ITokenDefinition
    {
        public Regex Regex { get; set; }
        public KeyValuePair<string,string> Type { get;private set; }
        public WaitDefinition(string pattern)
        {
            Regex = new Regex(pattern);
            Type = new KeyValuePair<string, string>("Wait", "Սպասել");
        }
    }
    public class Token
    {
        public string Name { get; set; }
        public Collection<string> Value { get; }
        public void AddValue(string value)
        {
            Value.Add(value);
        }
        public Token()
        {
            Value = new Collection<string>();
        }
        public KeyValuePair<string,string> TokenType { get; set; }

    }
   
   
}
