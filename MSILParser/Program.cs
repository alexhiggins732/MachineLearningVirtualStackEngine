
using Antlr.Runtime;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTokenStream = Antlr4.Runtime.CommonTokenStream;
using ICharStream = Antlr4.Runtime.ICharStream;

namespace MSILParser
{
    class Program
    {

        public static void Main(string[] args)
        {
            var path = @"C:\Users\alexander.higgins\source\repos\ILDisassembler\MSILParser\IlDisassembler.il";
            var text = File.ReadAllText(path);

            var stream = new Antlr4.Runtime.AntlrInputStream(text);
            var lexer = new MSILLexer((ICharStream)stream);

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new MSILParser(tokenStream);

            parser.BuildParseTree = true;

            var ctx = parser.decls();

            MSILParser.StartContext start = parser.start();

            var classDecs = parser.start();

            var vistor = new MsilVisitor();
            vistor.Visit(start);
            var decl = parser.decl();
            vistor.Visit(decl);
            //parser.d

        }
    }
    public class MsilVisitor : MSILBaseVisitor<object>
    {
        public override object Visit([NotNull] IParseTree tree)
        {
            if (tree is MSILParser.StartContext)
            {
                var sc = (MSILParser.StartContext)tree;
                var children = sc.children;
                foreach (var child in children)
                {
                    if (child is MSILParser.DeclsContext)
                    {
                        var dcs = (MSILParser.DeclsContext)child;
                        Visit(dcs);
                    }
                }
            }
            return base.Visit(tree);
        }
    }

}
