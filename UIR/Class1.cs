using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UIR
{
    public class NodeAbstractions
    {
        public void Unblock()
        {
            var ctx = NodeContextHelper.UniversalContext();
            var children = NodeContextHelper.GetChildContexts(ctx);
        }
    }
    public class NodeContextHelper
    {
        private static Node universalDefaultContext;
        public static Node UniversalDefaultContext()
        {
            return universalDefaultContext ?? (universalDefaultContext = UniversalContext());
        }
        public static Node UniversalContext()
        {
            return new NodeContext();
        }

        public static Node GetChildContexts(Node ctx)
        {
            Node node = null;
            //is everyting going to be static?
            // can we have a node have locals?
            return node;
            //throw new NotImplementedException();
        }
    }

    public class Node
    {
        // children, parent, sibling, index, relationship, context, value, attributes, properties,
        //single child, no child, name, id, type, system type, maps, expression, rule,
        //
        // to native type, from native type, data value, instruction,

        // Root node, what does it need that is universal?
        //   Id? Can implement NodeId.
        //  NodeType, can implement NodeType.
        //  perhaps there is an external map of these attributes.

        // Think type is all that is needed programatically, 
        // but learning algorithms might need an ID.
        //  but maybe context can retrieve id, type, etc.
        //  programtic equality>

        // Perhaps all nodes have locals but this can implementation specific.
        //  Would rather avoid initializing locals for all nodes for performance reasons.
        //  Either self learning will need to access locals or have reference
        //  static methods to access attributes for a node.

        //node could have multiple contexts. what is inheritance, what is union of applicable rules.
        // 

        public Node() { }
        public Node(dynamic node)
        {

        }

    }
    public class NodeContext : Node
    {
        //can reference other contexts, be a child context, universal context,
        // can have siblings, children,
    }
    public static class NodeExtensions
    {
        //From File is ambigious. This a binary file, csv, xml, json, source code etc?
        // "Intelligent" program will decide this and in fact "load(string)" should
        // be more approriate.
        // like wise (string fileName), AI program can decide via classification
        // if input parameter is json, csv, xml, plain text, source code, file path, uri, etc.
        // Establishing architecture pattern.
        //      Node input= new Node(parameter); <-- 
        //      Node ouput = <-- even if input is context implicit what about output.
        //          Can explicitly state expected type.
        //          Or can set a target context Node(ContextFor=Map);
        //  How about Node with dynamic constructor.
        //      Different nodes in different contexts interpet constructor types and parameters
        //  Chicken and Egg-> can hard code implementations to facilite training.
        //      but implement can create "cuts" in universality.
        // If a node or collection of nodes can represent anything then system should be able
        //  to operate solely on nodes (each representing some discrete value?)
        //  Strongly typing may be detrimental. Derived nodes can used for strong typing instead.
        //  AI system can map node to native system/lanaguge types. consider derived vs interface?

        public static Node FromFile(string fileName)
        {

            //Think this should be a static call to determine context,
            // "smart" c# implemenation resolves calling method(arg name, method name, declaring type, imported namespaces);
            // and uses these contextual clues.
            /// but now UIR is implementation specific-> but perhaps C#/NET is only a context clue
            ///     load calling exe so regardless if python/perl/assmebler?)
            var node = new Node();
            return node;
        }
    }
    public class CSParser
    {
        public Node ParseCsFile(string fileName)
        {


            //File has context of System.IO.
            //System.IO has context of Net FX.
            // Node System.IO.File has child nodes, one of which is 'ReadAllBytes'
            // 'ReadAllBytes' has context of System.IO.
            // Graph with no context should be able to learn
            // To take char[] {fileName} and convert to bytes ReadAllBytes.
            // QLearn Node(File) to make "move" readallbytes and return 
            // StackFrame with result [bytes].
            // Ultimately UIR->FromByteMap(fileName);
            var root = @"C:\Users\alexander.higgins\source\repos\ILDisassembler\UIR";
            if (!fileName.StartsWith(root)) fileName = Path.Combine(root, fileName);
            var bytes = File.ReadAllBytes(fileName);
            var graph = new Node();


            Func<byte[]> t = () => File.ReadAllBytes(fileName);
            //var lambaExpression = GetLambaExpression(() => File.ReadAllBytes(fileName));
            //Expression<Func<byte[]>> tExpression = new Expression<Func<byte[]>>(t, )


            dynamic t2 = t;
            var expression = GetExpression<dynamic>(t2);
            DynamicMetaObject metaObject = new DynamicMetaObject(t2, null);
            //var expression = GetExpression(()=> bytes);
            //var lambaGraph = new Node(() => File.ReadAllBytes(fileName));

            //node needs to have a default context.
            //can create contexts.
            // contexts may not exist and need to be learned.
            // Input graph is array of bytes.
            // Goal is to be able to universally map graph to other graph types.
            //  For example, IL, machine code, VM byte code, Code analyzer etc.
            return graph;
        }

        private dynamic GetLambaExpression(LambdaExpression lambdaExpression)
        {
            return lambdaExpression;
        }
        private object GetExpression<T>(Expression<dynamic> p)
        {
            throw new NotImplementedException();
        }

        private object GetExpression<T>(Expression p)
        {
            throw new NotImplementedException();
        }
    }
}
