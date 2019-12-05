using Dapper;
using ILEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UIR
{
    class Program
    {
        public static void buildRDPLinks()
        {




            var lines = File.ReadAllLines(@"C:\Users\alexander.higgins\Downloads\cpub-ClinicalLauncher-Alice_G3-CmsRdsh-no-signature.rdp");

            var d = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var idx = line.IndexOf(':');
                    var key = line.Substring(0, idx).Trim().Replace(" ", "%20");
                    var value = line.Substring(idx + 1).Trim().Replace(" ", "%20");
                    d.Add(key, value);
                }
            }
            var l = new UriBuilder();
            var queryFraments = d.Select(x => $"{x.Key}={x.Value}:");
            var query = string.Join("&", queryFraments);

        }

        static void saveMetaModel()
        {
            var path = @"C:\Users\alexander.higgins\source\repos\ILDisassembler\ILEngine\Models\OpCodeMetaModel.json";
            List<OpCodeMetaModel> metaModels;
          
            using (var conn = new SqlConnection("server=.;Initial Catalog=ILRuntime;Integrated Security=true;"))
            {
                metaModels = conn.Query<OpCodeMetaModel>("select * from [vwOpCodeFullMeta]").ToList();

            }
            var json = JsonConvert.SerializeObject(metaModels, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        public static void Main(string[] args)
        {
          
            //var metaModels = OpCodeMetaModel.OpCodeMetas;
            //var metaLines = string.Join("\r\n", metaModels.Select(x => x.ToString()));
            //saveMetaModel();
            //QOpCodeLearningProgram.TestExecution();
            QOpCodeLearningProgram.Run();
            Operators.AddOpTest();
            LiquisticArraySortTest.LinquisticSort();
            LiquisticArraySortTest.LinquisticSortReverse();


            LambaLoop.ForEachLoopDemo();
            LambaLoop.ForLoopDemo();
            LambaLoop.ForLoopDemo(1, 4);
            LambaLoop.CountEnumerable(1, 4);
            LambaLoop.ExpressExForEachDemo();
            var sl = new StackLoopDemo();
            while (sl.MoveNext())
                sl.Execute(sl.Current);

            var l = new List<bool>(new[] { true, false, true, false });
            dynamic enumerator = null;

            //var ex= Expression.Break

            bool loopstate = true;
            Action init = () => enumerator = l.GetEnumerator();
            Func<bool> eval = () => (loopstate = enumerator.MoveNext());
            Action body = () => Console.WriteLine(enumerator.Current);
            Action inc = () => { };

            var foreachLoop = new StackLoop(init, eval, body, inc);
            //var foreachLoop = new StackLoop(init, enumerator.MoveNext(), (Action)(() => Console.WriteLine(enumerator.Current)), (Action)(()=> { }));

            foreach (var instruction in foreachLoop)
            {
                foreachLoop.Execute(instruction);
            }


            ElementArraySiblingEnumeratorSortTest.Run();

            buildRDPLinks();
            InstructionEngine.TestLambaIterativeLoop();


            NullV3.UnitTests.Run();

            var set = new NullUnitSet();
            var setAsList = set.ToList();

            var boolset = new BoolUnitSet();
            var boolSetAsList = boolset.ToList();

            var parser = new CSParser();
            parser.ParseCsFile("class1.cs");

        }
    }


}
