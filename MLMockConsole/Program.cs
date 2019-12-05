using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MLMockConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CategorizeCharacters();
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            var enumTypes = EnumFinder.GetLoadedEnums();
            var scalarDateParts = ScalarDatePartInputs();
            var app = new MLConsoleApp();
            app.Run();

        }


        private static void CategorizeCharacters()
        {
            var charLookup = Enumerable.Range(0, 65536).Select(i => (char)((ushort)i)).ToLookup(c => char.GetUnicodeCategory(c), c => c);

            var json = JsonConvert.SerializeObject(charLookup);
            var categoryKeys = charLookup.Select(x => x.Key).ToArray();
            var d = new Dictionary<string, List<char>>();
            foreach(var key in categoryKeys)
            {
                List<char> categoryChars = charLookup[key].ToList();
                d.Add(key.ToString(), categoryChars);
            }
            var json2 = JsonConvert.SerializeObject(d, Formatting.Indented);
            System.IO.File.WriteAllText("char-categories.json", json2);
        }
        private static void GenerateAlphaNumericTokens()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("public class AlphaUpperTokens {");

            List<char> allTokens = new List<char>();
            for(var c= 'A'; c <= 'Z'; c++)
            {
                allTokens.Add(c);
                sb.AppendLine($"public const char {c} = '{c}';");
            }

            sb.AppendLine($"public const string AllTokens = \"{new string(allTokens.ToArray())}\";");
            sb.AppendLine("}");

            allTokens.Clear();
            sb.AppendLine("public class AlphaLowerTokens {");
            for (var c = 'a'; c <= 'z'; c++)
            {
                allTokens.Add(c);
                sb.AppendLine($"public const char {c} = '{c}';");
            }
            sb.AppendLine($"public const string AllTokens = \"{new string(allTokens.ToArray())}\";");
            sb.AppendLine("}");


            allTokens.Clear();
            sb.AppendLine("public class DigitTokens {");
            for (var c = '0'; c <= '9'; c++)
            {
                allTokens.Add(c);
                sb.AppendLine($"public const char {c} = '{c}';");
            }
            sb.AppendLine($"public const string AllTokens = \"{new string(allTokens.ToArray())}\";");
            sb.AppendLine("}");

            var code = sb.ToString();
        }


        /// <summary>
        /// TODO monitor assembly loads and add them to ML context.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            var loaded = args.LoadedAssembly;

        }

        static List<string> ScalarDatePartInputs()
        {
            var dateParts = new[] { "second", "minute", "hour", "day", "month", "year" };//, "century", "millenium" }

            var datePartTemplates = new[] { "What {0} is it", "what is the current {0}" };
            var scalarDatePartInputs = datePartTemplates.SelectMany(fmt => dateParts.Select(dp => string.Format(fmt, dp))).ToList();
            return scalarDatePartInputs;
        }
        static List<string> CollectionDatePartInputs()
        {

            //what are the days of the week, months of the year
            var dateParts = new[] { "seconds of the hour", "minute", "hour", "day", "month", "year" };//, "century", "millenium" }

            var datePartTemplates = new[] { "What are the", "what is the current {0}" };
            var scalarDatePartInputs = datePartTemplates.SelectMany(fmt => dateParts.Select(dp => string.Format(fmt, dp))).ToList();
            return scalarDatePartInputs;
        }

        static void MsilDateTimeTests()
        {
            var t = typeof(DateTime);
            var fullName = t.FullName;//need to be able to tokenize


            //Assembly Name, Type Name, Member Name, Member Binding Flags,  
            var resolvedAssembly = Assembly.Load("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            var resolvedType = resolvedAssembly.GetType("System.DateTime");
            var nowMethod = resolvedType.GetProperty("Now", BindingFlags.Public | BindingFlags.Static);
            var res = nowMethod.GetValue(null);
          
        }

        // useful who, what, when, where, why how.
        // Interfaces for mock testing, Environment.
        //Engine => indexes assemblies, types, members potentially msil as encountered. Optionally, assembly binaries and config files.
        static void MsilDateTimeTestInline()
        {
            var n = DateTime.Now;
            var tc= Environment.TickCount;
        }


        public static void RunSamples()
        {
            // enumerate lists -> default context standard out. As operator
            // can definte common question templates up front.
            // eliminating common stop words will be problematic, need to context on singular vs plural, congugations of "is,be,are", future, present past, etc.
            //      can we learn classifications.

            //tokens = {List|What[is|are] (is singleton of an collection, are plural)
            // collections=> 
            //      initialize a generic iterator
            //      use ML to learn how to resolve collection source. might require building code (eg parsing paged list off web site, a pdf, or downloading a csv file)
            //          => random Q, Genetic could potential "eventually" parse a list.
            //              decided cache retention policy, source identifier and parsing hints.
            //how vs how many.

          

            var inputs = new[]
            {
                "List the days of the week",
                "List the months of the year",
                "list the players on the eagles", //-> "(list)[resolved action] (the players)[dynamic] (on the eagles)[collection to resolve]"
                "List the SqlDbTypes",
                "List the Http Request Headers",
                "What are the hours of the day",
                "How many the hours are in a day", // how many
                "list the numbers between 2 and 10", // range, range counts.
                "What are the numbers between 5 and 7",
                "What time is it", // It is {DateTime.Now}
                "What is the time", // the time is {DateTime.Now}
            };

            // list hints: when to save, purge. Consider hit count, cost.
            //  members of congress or a team might be expensive and error prone to reproduce. URI identifier and sequence hit. 
            // strongly types enums might require file uri to assemlby with an 'enum' hint.

            var asClause = new[]
            {
                "",
                "as Json",
                "as comma seperated string", // delimiters (comma, pipe, tab)
                "to file",
                "as xml",
                "as datatable",
                "as database",

            };
        }
    }

    public class EnumFinder
    {
        public static List<Type> GetLoadedEnums()
        {
            List<Type> result = new List<Type>();



            var enums = FindEnums(AppDomain.CurrentDomain.GetAssemblies);

            return result;
        }

        private static List<Type> FindEnums(Func<IEnumerable<Assembly>> p)
        {
            var all = p().SelectMany(a => a.ExportedTypes).Where(t => t.IsEnum).OrderBy(t=> t.FullName).ToArray();
            var tt = all.Where(x => x.Name == "TokenType").ToList();
            var wsType = NLP.TokenType.Text;

            var all2 = p().SelectMany(a => a.ExportedTypes).Where(t => t.IsEnum).OrderBy(t => t.FullName).ToArray();
            var tt2 = all2.Where(x => x.Name == "TokenType").ToList();
            return all.ToList();
        }
    }


    internal class MLConsoleApp
    {
        public MLConsoleApp()
        {
        }

        internal void Run()
        {
            while (true)
            {
                var input = Console.ReadLine();
                ProcessInput(input);
            }
        }

        private void ProcessInput(string input)
        {
            var result = new ClassificationResult(input);

        }
    }

    internal class ClassificationResult
    {
        public string ResultText;
        private string input;


        public ClassificationResult(string input)
        {
            this.input = input;
            var tokens = NLP.TokenReader.Tokenize(input);
            if (tokens.Count == 0)
            {
                ResultText = "Emtpy";
            }
            else
            {
                if (tokens.HasText)
                {
                    var ResultText = tokens.TextTokens.First();
                }
            }
        }
    }





    
 

}
