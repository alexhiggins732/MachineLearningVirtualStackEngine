using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UIR
{
    public class Operators
    {
        public static void AddOpTest()
        {
            const int testCount = 10000000;

            for (var k = 0; k < 10; k++)
            {


                int a32 = 1;
                int b32 = 2;

                AdditionOperand<int> a = a32;
                AdditionOperand<int> b = b32;
                var result = a + b;

                var swIL = System.Diagnostics.Stopwatch.StartNew();
                for (var i = 0; i < testCount; i++)
                {
                    result = a32 + b32;
                }
                swIL.Stop();
                var elapsedIL = swIL.Elapsed;


                var sw = System.Diagnostics.Stopwatch.StartNew();
                for (var i = 0; i < testCount; i++)
                {
                    result = a + b;
                }
                sw.Stop();
                var elapsed = sw.Elapsed;


                var swCtor = System.Diagnostics.Stopwatch.StartNew();
                for (var i = 0; i < testCount; i++)
                {
                    result = (AdditionOperand<int>)a32 + (AdditionOperand<int>)b32;
                }
                swCtor.Stop();
                var elapsedCtor = swCtor.Elapsed;


                Func<int, int, int> opAdd = (x, y) => x + y;
                var swFunc = System.Diagnostics.Stopwatch.StartNew();
                for (var i = 0; i < testCount; i++)
                {
                    result = opAdd(a32, b32);
                }
                swFunc.Stop();
                var elapsedFunc = swFunc.Elapsed;




                var swDyn = System.Diagnostics.Stopwatch.StartNew();
                for (var i = 0; i < 10000000; i++)
                {
                    result = dynamicAdd(a32, b32);
                }
                swDyn.Stop();
                var elapsedDyn = swDyn.Elapsed;


                Console.WriteLine($"Compiled: {sw.Elapsed}");
                Console.WriteLine($"Ctor    : {swCtor.Elapsed}");
                Console.WriteLine($"IL      : {swIL.Elapsed}");
                Console.WriteLine($"Func    : {swFunc.Elapsed}");
                Console.WriteLine($"Dyna    : {swDyn.Elapsed}");
            }
        }

        static dynamic dynamicAdd(dynamic a, dynamic b) => a + b;
    }

    public class AdditionOperand<T>
    {
        private static readonly Func<T, T, T> op;
        public T Value { get; private set; }
        public AdditionOperand(T value) => this.Value = value;
        static AdditionOperand()
        {
            try
            {
                ParameterExpression left = Expression.Parameter(typeof(T), "left");
                ParameterExpression right = Expression.Parameter(typeof(T), "right");
                op = Expression.Lambda<Func<T, T, T>>(Expression.Add(left, right), left, right).Compile();
            }
            catch (InvalidOperationException)
            {
                //Eat the exception, no + operator defined :(
            }
        }

        public static implicit operator AdditionOperand<T>(T value) => new AdditionOperand<T>(value);
        public static implicit operator T(AdditionOperand<T> operand) => operand.Value;
        public static AdditionOperand<T> operator +(AdditionOperand<T> leftOperand, AdditionOperand<T> rightOperand)
        {
            if (op != null)
            {
                return new AdditionOperand<T>(op(leftOperand.Value, rightOperand.Value));
            }
            return null;
        }

    }

    public class OperatorDefinition
    {
        public string Expression { get; set; }
        public string Name { get; set; }
        public string AlternativeName { get; set; }
        public string Description { get; set; }

        public OperatorDefinition(string expression, string Name, string altName, string description, int arity)
        {
            this.Expression = (expression + "").Trim();
            this.Name = (Name + "").Trim();
            this.AlternativeName = (altName + "").Trim();
            this.Description = (description + "").Trim();
        }
    }
    public class BuiltInOperators : List<OperatorDefinition>
    {
        //https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/ms229032(v=vs.100)
        public static BuiltInOperators GetBuiltinOperatorDefinitions()
        {
            return new BuiltInOperators
            {
                new OperatorDefinition("implicit <type>", "op_Implicit ", "To<type> or From<type>", "Implicit conversion of operand to <type>", 1),
                new OperatorDefinition("explicit <type>", "op_Explicit ", "To<type> or From<type>", "Explicit conversion of operand to <type>", 1),

                new OperatorDefinition("+", "op_Addition" ,"Add", "Binary Addition", 2),
                new OperatorDefinition("-", "op_Subtraction", "Subtract", "Binary Subtraction", 2),
                new OperatorDefinition("*", "op_Multiply", "Multiply" ,"Binary Multiplication", 2),
                new OperatorDefinition("/", "op_Division", "Divide", "Binary Division", 2),

                new OperatorDefinition("%", "op_Modulus", "Mod", "Binary Modulus", 2),
                new OperatorDefinition("^", "op_ExclusiveOr", "Xor", "Binary Exclusive Or", 2),
                new OperatorDefinition("&", "op_BitwiseAnd", "And", "Binary And", 2),
                new OperatorDefinition("|", "op_BitwiseOr", "Or","Binary Or", 2),

                new OperatorDefinition("&&", "op_LogicalAnd", "And", "Logical And", 2),
                new OperatorDefinition("||", "op_LogicalOrr", "Or","Logical Or", 2),

                new OperatorDefinition("=", "op_Assign", "Assign", "Assignment", 2),

                new OperatorDefinition("<<", "op_LeftShift", "LeftShift", "Left Shift", 2),
                new OperatorDefinition(">>", "op_RightShift", "RightShift", "Right Shift", 2),

                // NOT Assigned  RightShift, op_SignedRightShift
                // NOT Assigned  LeftShift, op_SignedLeftShift

                new OperatorDefinition("==", "op_Equality" ,"Equals", "Equality", 2),
                new OperatorDefinition(">", "op_GreaterThan", "CompareTo", "Comparison", 2),
                new OperatorDefinition("<", "op_LessThan",  "CompareTo", "Comparison", 2),
                new OperatorDefinition("!=", "op_Inequality" ,"Equals", "Equality", 2),
                new OperatorDefinition(">=", "op_GreaterThanOrEqual", "CompareTo", "Equality", 2),
                new OperatorDefinition("<=", "op_LessThanOrEqual", "CompareTo", "Equality", 2),

                new OperatorDefinition("*=", "op_MultiplicationAssignment", "Multiply" ,"Multiplication Assignment", 2),
                new OperatorDefinition("-=", "op_SubtractionAssignment", "Subtract", "Subtraction Assignment", 2),
                new OperatorDefinition("^=", "op_ExclusiveOrAssignment", "Xor", "Exclusive Or Assignment", 2),
                new OperatorDefinition("<<=", "op_LeftShiftAssignment", "LeftShift", "Left Shift Assignment", 2),
                new OperatorDefinition("%=", "op_ModulusAssignment", "Mod", "Modulus Assignment", 2),
                new OperatorDefinition("+=", "op_AdditionAssignment", "Add", "Addition Assignment", 2),

                new OperatorDefinition("&=", "op_BitwiseAndAssignment", "And", "Bitwise And Assignment", 2),
                new OperatorDefinition("|=", "op_BitwiseOrAssignment", "Or", "Bitwise Or Assignment", 2),

                new OperatorDefinition(",", "op_comma", "Comma", "Comma", 2),

                new OperatorDefinition("/=", "op_DivisionAssignment", "Divide", "Division Assignment", 2),



                new OperatorDefinition("--", "op_Decrement", "Decrement" ,"Decrement Operand", 1),
                new OperatorDefinition("++", "op_Increment", "Increment" ,"Increments Operand", 1),

               new OperatorDefinition("-", "op_UnaryNegation",  "Negate", "Negates Operand", 1),
                new OperatorDefinition("+", "op_UnaryPlus", "Plus", "Makes operand positive", 1),


                new OperatorDefinition("~", "op_OnesComplement",  "OnesComplement", "Performs Ones Complent", 1),

                new OperatorDefinition(">>=", "op_RightShiftAssignment", "RightShift", "Right Shift Assignment", 2),
                new OperatorDefinition("!", "op_LogicalNot" ,"Not", "Logical Not", 1),
                // not built-in (already used for comparison A!=B; but maybe useful; can also be accomplished with &= !other
                //new OperatorDefinition("!=", "op_LogicalNotAssignment" ,"Not", "Logical Not", 1),











                //new OperatorDefinition("true", "op_True", "   "),
               // new OperatorDefinition("false", "op_False", "   "),
            };
        }
    }
}
