using Newtonsoft.Json;
using NLP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RednaConsole
{
    class Program
    {
        static ILogger logger = LoggerFactory.GetLogger<Program>();
        static void Main(string[] args)
        {
            //DictionarySampleTest.RunTests();
            LoggerFactory.RegisterLogger(ConsoleLogger.Instance);


            logger.Log("Hello");
            ProcessIO();
        }

        static Redna instance = new Redna();
        private static void ProcessIO()
        {
            while (true)
            {

                Console.Write("\r\n>>");
                var input = Console.ReadLine();
                ProcessInput(input);
            }
        }

        private static void ProcessInput(string input)
        {
            instance.ProcessInput(input);
            //throw new NotImplementedException();
        }
    }

    public class Redna
    {
        ILogger logger;
        public Redna()
        {
            this.logger = LoggerFactory.GetLogger<Redna>();
            //var nlp= new N
        }
        public void ProcessInput(string input)
        {
            logger.LogDebug($"ProcessInput:'{input}'");
            TokenCollection collection = input;
            var sentenceBuilder = new SentenceBuilder();
            foreach (var token in collection.TextTokens)
            {
                string nomalized = token.Normalize();
                var def = EnglishDictionary.Lookup(nomalized);
                if (def == null)
                {
                    logger.LogWarn($"Defintion for {nomalized} not found");
                }
                else
                {
                    logger.LogDebug($"Defintion for {nomalized}: {def.ToJson()}");
                    sentenceBuilder.Add(def);
                }
            }

            SentenceExecutionOptions executionOptions = sentenceBuilder.BuildOptions();
            if (executionOptions.IsExecutable())
            {
                executionOptions.ResolveAndExecute(sentenceBuilder);
            }
        }
    }

    public class SentenceExecutionOptions
    {
        ILogger logger;

        public List<dynamic> Executables = new List<dynamic>();
        public SentenceExecutionOptions()
        {
            this.logger = LoggerFactory.GetLogger<SentenceExecutionOptions>();
        }
        public bool IsExecutable() => Executables.Count > 0;


        public void ResolveAndExecute(SentenceBuilder builder)
        {
            logger.LogWarn($"{nameof(SentenceExecutionOptions)}.{nameof(ResolveAndExecute)} is only a prototype");

            foreach (var executable in this.Executables)
            {
                var name = executable.ToString().ToLower();
                switch (name)
                {
                    case "list":
                        logger.LogInfo($"{nameof(SentenceExecutionOptions)}.{nameof(ResolveAndExecute)} resolved {name}");
                        var resolutionTarget = builder.Words.Where(x => x.Word != "list" && x.Word != "the" && x.Word != "what" && x.Word != "are").ToList();
                        var target = ResolveTarget(resolutionTarget);
                        if (target == null)
                        {
                            logger.LogWarn($"{nameof(SentenceExecutionOptions)}.{nameof(ResolveAndExecute)} failed to resolve '{string.Join(", ", resolutionTarget.Select(x => x.Word))}'");
                        }
                        else
                        {
                            ListTarget(target);
                        }
                        break;

                    default:
                        logger.LogWarn($"{nameof(SentenceExecutionOptions)}.{nameof(ResolveAndExecute)} failed to resolve executable {name}");
                        break;
                }
            }
        }

        private void ListTarget(object target)
        {
            logger.LogWarn($"{nameof(SentenceExecutionOptions)}.{nameof(ListTarget)} is only a prototype");
            if (target is Type)
            {
                var t = (Type)target;
                if (t.IsEnum)
                {
                    var names = Enum.GetNames(t);
                    Console.WriteLine(string.Join(", ", names));

                }
                else
                {
                    logger.LogWarn($"{nameof(SentenceExecutionOptions)}.{nameof(ListTarget)} is missing list instructions for {t}");
                }
            }
            else
            {
                logger.LogWarn($"{nameof(SentenceExecutionOptions)}.{nameof(ListTarget)} is missing list instructions for {target}");
            }


        }

        private object ResolveTarget(List<DictionaryEntry> resolutionTarget)
        {
            logger.LogWarn($"{nameof(SentenceExecutionOptions)}.{nameof(ResolveTarget)}  is only a prototype");
            var words = resolutionTarget.Select(x => x.Word.ToLower()).ToList();
            bool containsDay = words.Contains("day");
            bool containsDays = words.Contains("days");

            if (((words.Contains("day") || words.Contains("days")) && words.Contains("week")) || words.Contains("weekday") || words.Contains("weekdays"))
            {
                return typeof(DayOfWeek);
            }
            if (((words.Contains("months") || words.Contains("month")) && words.Contains("year")) || words.Contains("month") || words.Contains("months"))
            {
                return typeof(MonthOfYear);
            }


            return null;
        }

        public void AddExecutable(dynamic item)
        {
            logger.LogWarn($"{nameof(SentenceExecutionOptions)}.{nameof(AddExecutable)} is only a prototype");
            Executables.Add(item);
        }
    }

    public static class StringExtensions
    {
        public static string ToJson(this object value) => value.ToJson(Formatting.None);
        public static string ToJsonIndented(this object value) => value.ToJson(Formatting.Indented);

        public static string ToJson(this object value, Formatting formatting) => JsonConvert.SerializeObject(value, formatting);
    }

    public enum MonthOfYear
    {

        January = 1,

        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
    public class SentenceBuilder
    {
        public List<DictionaryEntry> Words = new List<DictionaryEntry>();
        ILogger logger;
        public SentenceBuilder()
        {
            this.logger = LoggerFactory.GetLogger<SentenceBuilder>();

        }


        public void Add(DictionaryEntry def)
        {
            Words.Add(def);
        }

        public SentenceExecutionOptions BuildOptions()
        {
            var result = new SentenceExecutionOptions();

            logger.LogWarn($"{nameof(BuildOptions)} is only a prototype");

            var listIndex = Words.IndexOf(x => x.Word == "list");
            var theIndex = Words.IndexOf(x => x.Word == "the");
            var whatIndex = Words.IndexOf(x => x.Word == "what");
            var areIndex = Words.IndexOf(x => x.Word == "are");
            if (
                ((listIndex > -1 && theIndex > -1) && theIndex == listIndex + 1)
                || (whatIndex > -1 && whatIndex + 1 == areIndex && areIndex + 1 == theIndex)
            )
            {
                result.Executables.Add("List");
            }
            else
            {
                logger.LogWarn($"{nameof(BuildOptions)} did not find an executable phrase");
            }
            return result;
        }
    }

    public static class ListExtensions
    {
        public static int IndexOf<T>(this List<T> list, Func<T, bool> filter) => list.IndexOf(list.FirstOrDefault(filter));
        public static bool Contains<T>(this List<T> list, Func<T, bool> filter) => list.IndexOf(list.FirstOrDefault(filter)) > -1;
    }

}
