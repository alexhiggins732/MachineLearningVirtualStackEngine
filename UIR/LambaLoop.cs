using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UIR
{
    public class FuzzyElementWiseUnitCompareLoop
    {
        public FuzzyElementWiseUnitCompareLoop(ElementalArray lhs, ElementalArray rhs)
        {
            // if GetLoopControlMembership(ElementWiseUnitComparer)
            //      => Comparer.MovedNext And ComparisonResult = Membership.Equal
            //  ComparisonResult = UnitEnumeratorCompare(lhs.Enumerator, rhs.Enumerator)
            //      => {lhs.Current ^ rhs.Current} ? Membership.Equal : (lhs.Current ? Membership.Greater: Membership.Less);

            /* Fuzzy Rules:
               if LoopStateNone Then Init
               if LoopStateInit Then Loop
               if LoopStateLoop Then Evaluate
               if LoopStateEvaluate and Evaluate() Then LoopAction
               if LoopStateLoopBody Then LoopAction();
               if LoopStateDone Then LoopExit();
             */
             /*
              If NotLoopInitialized then Initialize
              If InLoopBody Then ExecuteLoop()
              --- or  --
              If Not LoopFinished Then ExecuteLoop()
              --- Generic rule
              If Not ReachedTarget Then Move()

              */

            dynamic LoopState = null;

       

            dynamic LoopStateNone = null;
            dynamic LoopStateInit = null;
            dynamic LoopStateLoop = null;
            dynamic LoopStateEvaluate = null;
            dynamic LoopStateLoopAction= null;

            dynamic LoopInit = null;
            dynamic Loop= null;
            dynamic Evaluate = null;
            dynamic LoopAction = null ;
            var loopActions = new Dictionary<dynamic, dynamic>
            {
                {LoopStateNone, LoopInit },
                {LoopStateInit, Loop },
                {LoopStateLoop , Evaluate },
                {LoopStateEvaluate , LoopAction },
                {LoopStateLoopAction , Loop },
            };

            Func<dynamic, dynamic> GetMembership = (state) =>
             {
                 return loopActions[state];
             };
            var loopMembership = GetMembership(LoopState);

        }
    }

    public class LambaLoop
    {
        public static Expression ForEach(Expression collection, ParameterExpression loopVar, Expression loopContent)
        {
            var elementType = loopVar.Type;
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

            var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
            var getEnumeratorCall = Expression.Call(collection, enumerableType.GetMethod("GetEnumerator"));
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);

            // The MoveNext method's actually on IEnumerator, not IEnumerator<T>
            var moveNextCall = Expression.Call(enumeratorVar, typeof(System.Collections.IEnumerator).GetMethod("MoveNext"));

            var breakLabel = Expression.Label("LoopBreak");

            var loop = Expression.Block(new[] { enumeratorVar },
                enumeratorAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(moveNextCall, Expression.Constant(true)),
                        Expression.Block(new[] { loopVar },
                            Expression.Assign(loopVar, Expression.Property(enumeratorVar, "Current")),
                            loopContent
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
        }

        public static void ForEachLoopDemo()
        {
            var collection = Expression.Parameter(typeof(List<string>), "collection");
            var loopVar = Expression.Parameter(typeof(string), "loopVar");
            var loopBody = Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), loopVar);
            var loop = ForEach(collection, loopVar, loopBody);
            var compiled = Expression.Lambda<Action<List<string>>>(loop, collection).Compile();
            compiled(new List<string>() { "a", "b", "c" });
        }


        /// <summary>
        /// Lamba loop equivalent of for (loopVar = initValue; condition; increment) {  loopContent     }
        /// </summary>
        /// <param name="loopVar"></param>
        /// <param name="initValue"></param>
        /// <param name="condition"></param>
        /// <param name="increment"></param>
        /// <param name="loopContent"></param>
        /// <returns></returns>
        public static Expression For(ParameterExpression loopVar, Expression initValue, Expression condition, Expression increment, Expression loopContent)
        {
            var initAssign = Expression.Assign(loopVar, initValue);

            var breakLabel = Expression.Label("LoopBreak");

            var loop = Expression.Block(new[] { loopVar },
                initAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        condition,
                        Expression.Block(
                            loopContent,
                            increment
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
        }


        public static void ForLoopDemo()
        {

            var loopVar = Expression.Variable(typeof(int), "loopVar");
            var initValue = Expression.Assign(loopVar, Expression.Constant(0));
            var condition = Expression.LessThan(loopVar, Expression.Constant(10));
            var increment = Expression.PostIncrementAssign(loopVar);
            var loopBody = Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(int) }), loopVar);
            var loop = For(loopVar, initValue, condition, increment, loopBody);
            //var compiled = Expression.Lambda<Action<List<string>>>(loop, collection).Compile();
            var compiled = Expression.Lambda<Action>(loop).Compile();
            compiled();
            //compiled(new List<string>() { "a", "b", "c" });
        }

        public static void ForLoopDemo(int loopStart = 0, int loopEnd = 10)
        {

            var loopVar = Expression.Parameter(typeof(int), "loopVar");
            var initValue = Expression.Assign(loopVar, Expression.Constant(loopStart));
            var condition = Expression.LessThan(loopVar, Expression.Constant(loopEnd));
            var increment = Expression.PostIncrementAssign(loopVar);
            var loopBody = Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(int) }), loopVar);
            var loop = For(loopVar, initValue, condition, increment, loopBody);
            //var compiled = Expression.Lambda<Action<List<string>>>(loop, collection).Compile();
            var compiled = Expression.Lambda<Action>(loop).Compile();
            compiled();
            //compiled(new List<string>() { "a", "b", "c" });
        }

        public static void CountEnumerable(int loopStart = 0, int loopEnd = 10)
        {
            //var loopVar = Expression.Parameter(typeof(int), "loopVar");
            //var initValue = Expression.Assign(loopVar, Expression.Constant(loopStart));
            //var condition = Expression.LessThan(loopVar, Expression.Constant(loopEnd));
            //var increment = Expression.PostIncrementAssign(loopVar);
            //var loopBody = Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(int) }), loopVar);
            //var loop = For(loopVar, initValue, condition, increment, loopBody);
            //var ret = Expression.Lambda<int>(loopVar);
            //var loopExpressions = ((BlockExpression)loop).Expressions.ToList();
            //loopExpressions.Add(ret);
            //var retBlock = Expression.Block(loopExpressions);
            //var compiled = Expression.Lambda<Func<int>>(retBlock).Compile();
            //compiled();
            ////compiled(new List<string>() { "a", "b", "c" });
        }

        public static void LambaLoopDemoWIP()
        {
            var state = Expression.Variable(typeof(int), "state");
            var stateStart = Expression.Constant(0);
            var stateEnd = Expression.Constant(10);
            var init = Expression.Assign(state, stateStart);
            var eval = Expression.IsTrue(Expression.LessThan(state, stateEnd));
            var inc = Expression.Increment(state);

            //var bwrite= Expression.Invoke()
            // var instructions = new List<Expression>(new[] { }
            var body = Expression.Block(new[] { state }, eval);
            var loop = Expression.Loop(body);

            var instructions = new Expression[] { init, loop };
            var block = Expression.Block(new[] { state }, init, loop);

            var compiled = Expression.Lambda<Action>(block).Compile();
            compiled();
        }

        public static void ExpressExForEachDemo()
        {
            var coll = new List<string>(new[] { "a", "b", "c" });
            var collection = Expression.Parameter(typeof(List<string>), "collection");
            var loopVar = Expression.Parameter(typeof(string), "loopVar");
            //var loopBody = Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), loopVar);
            var loopBody = Expression.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expression.Variable(typeof(string), "enumerator"));
            var exp = ExpressionEx.ForEach<string>(collection, loopBody);
            var compiled = Expression.Lambda<Action>(exp, collection).Compile();
            compiled();
        }
    }

    static partial class ExpressionEx
    {
        public static Expression ForEach<TSource>(Expression enumerable, Expression loopContent)
        {
            var enumerableType = enumerable.Type;
            var getEnumerator = enumerableType.GetMethod("GetEnumerator");
            if (getEnumerator is null)
                getEnumerator = typeof(IEnumerable<>).MakeGenericType(typeof(TSource)).GetMethod("GetEnumerator");
            var enumeratorType = getEnumerator.ReturnType;
            var enumerator = Expression.Variable(enumeratorType, "enumerator");

            return Expression.Block(new[] { enumerator },
                Expression.Assign(enumerator, Expression.Call(enumerable, getEnumerator)),
                EnumerationLoop(enumerator, loopContent));
        }

        public static Expression ForEach<TSource>(Expression enumerable, ParameterExpression loopVar, Expression loopContent)
        {
            var enumerableType = enumerable.Type;
            var getEnumerator = enumerableType.GetMethod("GetEnumerator");
            if (getEnumerator is null)
                getEnumerator = typeof(IEnumerable<>).MakeGenericType(typeof(TSource)).GetMethod("GetEnumerator");
            var enumeratorType = getEnumerator.ReturnType;
            var enumerator = Expression.Variable(enumeratorType, "enumerator");

            return Expression.Block(new[] { enumerator },
                Expression.Assign(enumerator, Expression.Call(enumerable, getEnumerator)),
                EnumerationLoop(enumerator,
                    Expression.Block(new[] { loopVar },
                        Expression.Assign(loopVar, Expression.Property(enumerator, "Current")),
                        loopContent)));
        }

        static Expression EnumerationLoop(ParameterExpression enumerator, Expression loopContent)
        {
            var loop = While(
                Expression.Call(enumerator, typeof(System.Collections.IEnumerator).GetMethod("MoveNext")),
                loopContent);

            var enumeratorType = enumerator.Type;
            if (typeof(IDisposable).IsAssignableFrom(enumeratorType))
                return Using(enumerator, loop);

            if (!enumeratorType.IsValueType)
            {
                var disposable = Expression.Variable(typeof(IDisposable), "disposable");
                return Expression.TryFinally(
                    loop,
                    Expression.Block(new[] { disposable },
                        Expression.Assign(disposable, Expression.TypeAs(enumerator, typeof(IDisposable))),
                        Expression.IfThen(
                            Expression.NotEqual(disposable, Expression.Constant(null)),
                            Expression.Call(disposable, typeof(IDisposable).GetMethod("Dispose")))));
            }

            return loop;
        }

        public static Expression Using(ParameterExpression variable, Expression content)
        {
            var variableType = variable.Type;

            if (!typeof(IDisposable).IsAssignableFrom(variableType))
                throw new Exception($"'{variableType.FullName}': type used in a using statement must be implicitly convertible to 'System.IDisposable'");

            var getMethod = typeof(IDisposable).GetMethod("Dispose");

            if (variableType.IsValueType)
            {
                return Expression.TryFinally(
                    content,
                    Expression.Call(Expression.Convert(variable, typeof(IDisposable)), getMethod));
            }

            if (variableType.IsInterface)
            {
                return Expression.TryFinally(
                    content,
                    Expression.IfThen(
                        Expression.NotEqual(variable, Expression.Constant(null)),
                        Expression.Call(variable, getMethod)));
            }

            return Expression.TryFinally(
                content,
                Expression.IfThen(
                    Expression.NotEqual(variable, Expression.Constant(null)),
                    Expression.Call(Expression.Convert(variable, typeof(IDisposable)), getMethod)));
        }

        public static Expression While(Expression loopCondition, Expression loopContent)
        {
            var breakLabel = Expression.Label();
            return Expression.Loop(
                Expression.IfThenElse(
                    loopCondition,
                    loopContent,
                    Expression.Break(breakLabel)),
                breakLabel);
        }
    }
}
