using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using ILEngineTests;

namespace ILEngine.Tests
{
    public static class TypeMethods
    {
        public static object Foo(this Type bar)
        {
            return null;
        }
        public static MethodInfo Method(this Action action) => action.Method;
    }
    public class MethodResolver
    {

    }

    public struct PointTest : IEquatable<PointTest>
    {
        public static PointTest Empty = new PointTest();
        public int X { get; set; }
        public int Y { get; set; }



        public static bool operator ==(PointTest a, PointTest b) => a.Equals(b);
        public static bool operator !=(PointTest a, PointTest b) => !a.Equals(b);
        public override bool Equals(object obj)
        {
            return obj is PointTest && Equals((PointTest)obj);
        }

        public bool Equals(PointTest other)
        {
            return X == other.X && Y == other.Y;
        }


        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public static bool operator <(PointTest a, PointTest b) => (((a.X * a.X) + (a.Y + a.Y)) >> 1) < (((b.X * b.X) + (b.Y + b.Y)) >> 1);
        public static bool operator >(PointTest a, PointTest b) => (((a.X * a.X) + (a.Y + a.Y)) >> 1) > (((b.X * b.X) + (b.Y + b.Y)) >> 1);
        public static bool operator <=(PointTest a, PointTest b) => a == b || a < b;
        public static bool operator >=(PointTest a, PointTest b) => a == b || a > b;
    }
    public class GraphTest
    {

        public List<PointTest> Points { get; set; }
        public GraphTest(IEnumerable<PointTest> points)
        {
            this.Points = points.ToList();
        }

    }

    public struct LineTest : IEquatable<LineTest>
    {
        public PointTest A;
        public PointTest B;
        public LineTest(PointTest a, PointTest b) { this.A = a; this.B = b; }

        public override bool Equals(object obj)
        {
            return obj is LineTest && Equals((LineTest)obj);
        }

        public bool Equals(LineTest other)
        {
            return A.Equals(other.A) &&
                   B.Equals(other.B);
        }

        public override int GetHashCode()
        {
            var hashCode = -1817952719;
            hashCode = hashCode * -1521134295 + EqualityComparer<PointTest>.Default.GetHashCode(A);
            hashCode = hashCode * -1521134295 + EqualityComparer<PointTest>.Default.GetHashCode(B);
            return hashCode;
        }

        public static bool operator ==(LineTest test1, LineTest test2)
        {
            return test1.Equals(test2);
        }

        public static bool operator !=(LineTest test1, LineTest test2)
        {
            return !(test1 == test2);
        }
    }

    [TestClass()]
    public class IlInstructionEngineTests
    {
        [TestMethod()]
        public void ExecuteTest()
        {

            Func<List<PointTest>> linqForEachSelectForSelectMany = () =>
            {
                var rnd = new Random();
                var range = Enumerable.Range(1, 10);
                var minValue = 5;
                var maxValue = 600;
                var points = range.Select(i =>
                {
                    var p = new PointTest { X = rnd.Next(minValue, maxValue), Y = rnd.Next(minValue, maxValue) };
                    return p;
                });

                var map = new GraphTest(points);
                Func<List<LineTest>> GetAllPoints = () =>
                {
                    var result = new List<LineTest>();
                    foreach (var p in map.Points)
                    {
                        for (var i = 0; i < map.Points.Count; i++)
                        {
                            if (p != map.Points[i])
                            {
                                var ln = new LineTest(p, map.Points[i]);
                                if (!result.Any(x => x == ln))
                                    result.Add(ln);


                            }
                        }
                    }
                    return result;

                };

                var res = GetAllPoints().SelectMany(x => new[] { x.A, x.B }).ToList();
                return res;

            };

            var engine = new ILEngine.IlInstructionEngine();

            var expected = linqForEachSelectForSelectMany();

            var IlExecuted = engine.ExecuteTyped<List<PointTest>>(linqForEachSelectForSelectMany.Method, linqForEachSelectForSelectMany);
            Assert.IsNotNull(IlExecuted);
            Assert.IsInstanceOfType(IlExecuted, typeof(List<PointTest>));
            Assert.IsTrue(IlExecuted.Count == expected.Count); // <-- TODO: this could fail if the same point is generated twice
            //Assert.IsTrue(expected.SequenceEqual(IlExecuted)); <-- points are random

            ExecuteTestInline1();
            ExecuteTestInline();


            Func<List<PointTest>> listCtor = () =>
                {
                    var pta = new PointTest() { X = 0, Y = 2 };
                    var ptb = new PointTest() { X = 2, Y = 2 };

                    var l = new List<PointTest>(new[] { pta, ptb });
                    return l;
                };


            var listCtorExpected = listCtor();

            var listCtorResult = engine.ExecuteTyped<List<PointTest>>(listCtor.Method, listCtor.Target);

            Assert.IsNotNull(listCtorResult);
            Assert.IsTrue(listCtorResult.SequenceEqual(listCtorExpected));

            Func<bool[]> linqListComp = () =>
                {

                    var pta = new PointTest() { X = 0, Y = 2 };
                    var ptb = new PointTest() { X = 2, Y = 2 };

                    var l = new List<PointTest>(new[] { pta, ptb }); //a is getting overwrote with b;

                    var cmpLtResult = l.FirstOrDefault(x => x < ptb) == pta;
                    var cmpLteResult = l.FirstOrDefault(x => x <= ptb) == pta;
                    var cmpEqResult = l.FirstOrDefault(x => x == ptb) == ptb;
                    var cmpGteResult = l.FirstOrDefault(x => x >= ptb) == ptb;
                    var cmpGtResult = l.FirstOrDefault(x => x > ptb) == PointTest.Empty;


                    return new[] { cmpLtResult, cmpLteResult, cmpEqResult, cmpGteResult, cmpGtResult };
                };

            var linqCmpExpected = linqListComp();

            var linqCmpResult = engine.ExecuteTyped<bool[]>(linqListComp.Method, linqListComp.Target);
            var cmpExpected = new bool[] { true, true, true, true, true };
            Assert.IsNotNull(linqCmpResult);
            Assert.IsTrue(linqCmpResult.Length == 5);
            Assert.IsTrue(linqCmpResult.All(x => x));
        }


        [TestMethod()]
        public void ExecuteTestInline1()
        {
            var rnd = new Random();
            var range = Enumerable.Range(1, 10);
            var minValue = 5;
            var maxValue = 600;
            var points = range.Select(i =>
            {
                var p = new PointTest { X = rnd.Next(minValue, maxValue), Y = rnd.Next(minValue, maxValue) };
                return p;
            });
            var map = new GraphTest(points);

            Func<List<LineTest>> UniqueLines = () =>
            {
                var AllPoints = new List<LineTest>();
                foreach (var p in map.Points)
                {
                    for (var i = 0; i < map.Points.Count; i++)
                    {
                        if (p != map.Points[i])
                        {
                            var ln = new LineTest(p, map.Points[i]);
                            if (!AllPoints.Any(x => x == ln))
                                AllPoints.Add(ln);


                        }
                    }
                }
                return AllPoints;
            };


           

            var engine = new ILEngine.IlInstructionEngine();

            var expected = UniqueLines();
            var IlExecuted = engine.ExecuteTyped<List<LineTest>>(UniqueLines.Method, UniqueLines.Target);// <-- this fails because the func loads a local variable as an arg even though there is no arg in the func()
            Assert.IsNotNull(IlExecuted);
            Assert.IsInstanceOfType(IlExecuted, typeof(List<LineTest>));
            Assert.IsTrue(IlExecuted.Count == expected.Count); //TODO: this can fail if same points are generated
            //Assert.IsTrue(expected.SequenceEqual(IlExecuted)); <-- //TODO: points are generated randomly
        }

        [TestMethod()]
        public void ExecuteTestInline()
        {

            Func<List<PointTest>> linqForEachSelectForSelectMany = () =>
            {
                var rnd = new Random();
                var range = Enumerable.Range(1, 10);
                var minValue = 5;
                var maxValue = 600;
                var points = range.Select(i =>
                {
                    var p = new PointTest { X = rnd.Next(minValue, maxValue), Y = rnd.Next(minValue, maxValue) };
                    return p;
                });

                var map = new GraphTest(points);

                var AllPoints = new List<LineTest>();
                foreach (var p in map.Points)
                {
                    for (var i = 0; i < map.Points.Count; i++)
                    {
                        if (p != map.Points[i])
                        {
                            var ln = new LineTest(p, map.Points[i]);
                            if (!AllPoints.Any(x => x == ln))
                                AllPoints.Add(ln);


                        }
                    }
                }
                //return result;



                var res = AllPoints.SelectMany(x => new[] { x.A, x.B }).ToList();
                return res;

            };

            var engine = new ILEngine.IlInstructionEngine();

            var expected = linqForEachSelectForSelectMany();

            var IlExecuted = engine.ExecuteTyped<List<PointTest>>(linqForEachSelectForSelectMany.Method, engine);
            Assert.IsNotNull(IlExecuted);
            Assert.IsInstanceOfType(IlExecuted, typeof(List<PointTest>));
            Assert.IsTrue(IlExecuted.Count == expected.Count);  //TODO: this can fail if same points are generated
                                                                // Assert.IsTrue(expected.SequenceEqual(IlExecuted));  //TODO: points are generated randomly

        }

        //private MethodInfo Resolve(Action actionResolver)
        //{
        //    return actionResolver.Method;
        //}
        //private MethodInfo Resolve(Func<Action> actionResolver)
        //{
        //    return actionResolver().Method;
        //}
        private MethodInfo Resolve(Action action, params Type[] args)
        {
            var baseMethod = action.Method;
            var declaringType = baseMethod.DeclaringType;
            var method = declaringType.GetMethod(baseMethod.Name, args);

            return method;
        }


        private MethodInfo ResolveGenericOverload(Func<Action> actionResolver, params Type[] types) => Resolve(actionResolver(), types);



        //private MethodInfo Resolve<T>(Action action) => Resolve(action, typeof(T));


        private MethodInfo Resolve<T>(Func<Action<T>> action) => action().Method;
        private MethodInfo Resolve<T1, T2>(Func<Action> actionResolver) => ResolveGenericOverload(actionResolver, typeof(T1), typeof(T2));

        private MethodInfo Resolve<T1, T2, T3>(Func<Action> actionResolver) => ResolveGenericOverload(actionResolver, typeof(T1), typeof(T2), typeof(T3));

        private MethodInfo Resolve<T1, T2, T3, T4>(Func<Action> actionResolver) => ResolveGenericOverload(actionResolver, typeof(T1), typeof(T2), typeof(T3), typeof(T4));

        private MethodInfo Resolve<T1, T2, T3, T4, T5>(Func<Action> actionResolver) => ResolveGenericOverload(actionResolver, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

        private MethodInfo Resolve<T1, T2, T3, T4, T5, T6>(Func<Action> actionResolver) => ResolveGenericOverload(actionResolver, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

        private MethodInfo Resolve<T1, T2, T3, T4, T5, T6, T7>(Func<Action> actionResolver) => ResolveGenericOverload(actionResolver, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

        private MethodInfo Resolve<T1, T2, T3, T4, T5, T6, T7, T8>(Func<Action> actionResolver) => ResolveGenericOverload(actionResolver, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));


        private MethodInfo ResolveFn<TResult>(Func<TResult> funcResolver) => ResolveGenericFnOverload(funcResolver, typeof(TResult));
        private MethodInfo ResolveFn<T1, TResult>(Func<T1, TResult> funcResolver) => ResolveGenericFnOverload(funcResolver, typeof(T1), typeof(TResult));
        private MethodInfo ResolveFn<T1, T2, TResult>(Func<T1, T2, TResult> funcResolver) => ResolveGenericFnOverload(funcResolver, typeof(T1), typeof(T2), typeof(TResult));
        private MethodInfo ResolveFn<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> funcResolver) => ResolveGenericFnOverload(funcResolver, typeof(T1), typeof(T2), typeof(T3), typeof(TResult));
        private MethodInfo ResolveFn<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> funcResolver) => ResolveGenericFnOverload(funcResolver, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(TResult));
        private MethodInfo ResolveFn<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> funcResolver) => ResolveGenericFnOverload(funcResolver, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(TResult));
        private MethodInfo ResolveFn<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> funcResolver) => ResolveGenericFnOverload(funcResolver, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(TResult));
        private MethodInfo ResolveFn<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> funcResolver) => ResolveGenericFnOverload(funcResolver, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(TResult));
        private MethodInfo ResolveFn<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> funcResolver) => ResolveGenericFnOverload(funcResolver, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(TResult));
        private MethodInfo ResolveGenericFnOverload(Delegate funcResolver, params Type[] types)
        {
            var resolver = funcResolver.Method;
            if (resolver.IsAssembly) return resolver;
            var method = resolver;// (MethodInfo)resolver.Method;
            var declaringType = method.DeclaringType;

            var result = declaringType.GetMethod(method.Name, types.Take(types.Length - 1).ToArray());//, types.Skip(1).ToArray());
            return result;

        }

        public object RtHandle() => ArgTest(__arglist(1));
        public object[] ArgTest(__arglist)
        {

            var args = new ArgIterator(__arglist);
            var result = new object[] { args.GetRemainingCount() };
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = TypedReference.ToObject(args.GetNextArg());
            }
            return result;


        }

        private object targtest(Func<RuntimeTypeHandle> argTest)
        {
            throw new NotImplementedException();
        }


        [TestMethod()]
        public void ExecuteTypedTest()
        {




            var engine = new IlInstructionEngine();



            var rwl_0 = Resolve(Console.WriteLine);

            engine.ExecuteTyped(rwl_0);

            Assert.ThrowsException<TargetParameterCountException>(() => engine.ExecuteTyped(rwl_0, "someArg"));

            var rwl_2 = Resolve<string>(() => Console.WriteLine);
            Assert.ThrowsException<TargetParameterCountException>(() => engine.ExecuteTyped(rwl_2));

            engine.ExecuteTyped(rwl_2, $"Executed: " + nameof(rwl_2));

            var rwl_3 = Resolve<string, object>(() => Console.WriteLine);
            Assert.ThrowsException<TargetParameterCountException>(() => engine.ExecuteTyped(rwl_3));
            Assert.ThrowsException<TargetParameterCountException>(() => engine.ExecuteTyped(rwl_3, "Executed: {0}"));
            engine.ExecuteTyped(rwl_3, "Executed: {0}", nameof(rwl_3));
            Assert.ThrowsException<TargetParameterCountException>(() => engine.ExecuteTyped(rwl_3, "Executed: {0}", nameof(rwl_3), "somearg"));


            var rwl_4 = Resolve<string, object, object>(() => Console.WriteLine);
            Assert.ThrowsException<TargetParameterCountException>(() => engine.ExecuteTyped(rwl_4));
            Assert.ThrowsException<TargetParameterCountException>(() => engine.ExecuteTyped(rwl_4, "Executed: {0} {1}"));
            Assert.ThrowsException<TargetParameterCountException>(() => engine.ExecuteTyped(rwl_4, "Executed: {0} {1}", nameof(rwl_4)));
            engine.ExecuteTyped(rwl_4, "Executed: {0} {1}", nameof(rwl_4), "arg1");



            var rwl_5 = Resolve<string, object, object, object>(() => Console.WriteLine);
            Assert.ThrowsException<TargetParameterCountException>(() => engine.ExecuteTyped(rwl_5));
            Assert.ThrowsException<TargetParameterCountException>(() => engine.ExecuteTyped(rwl_5, "Executed: {0} {1}"));
            Assert.ThrowsException<TargetParameterCountException>(() => engine.ExecuteTyped(rwl_5, "Executed: {0} {1}", nameof(rwl_5)));
            Assert.ThrowsException<TargetParameterCountException>(() => engine.ExecuteTyped(rwl_5, "Executed: {0} {1}", nameof(rwl_5), "arg2"));
            engine.ExecuteTyped(rwl_5, "Executed: {0} {1} {2}", nameof(rwl_5), "arg1", "arg2");



            var rwl_6 = Resolve<string, object, object, object, object>(() => Console.WriteLine);
            Assert.ThrowsException<NotSupportedException>(() =>
            engine.ExecuteTyped(rwl_6, "Executed: {0} {1} {2} {3}", nameof(rwl_6), "arg1", "arg2", "arg3")
            );


            var rwl_7 = Resolve<string, object[]>(() => Console.WriteLine);
            engine.ExecuteTyped(rwl_7, "Executed: {0} {1} {2} {3} {4}", new object[] { nameof(rwl_7), "arg1", "arg2", "arg3", "arg4" });
            engine.ExecuteTyped(rwl_7, "Executed: {0} {1} {2} {3} {4}", nameof(rwl_7), "arg1", "arg2", "arg3", "arg4");




        }


        public class NetIOTestClass
        {

            public static string DLGoogleStatic()
            {
                var wc = new System.Net.WebClient();
                var s = wc.DownloadString("http://www.google.com");
                return s;
            }

            private string targetUrl = "";
            private char[] targetUrlChars;
            public NetIOTestClass() : this("http://google.com") { }


            public NetIOTestClass(string target)
            {
                this.targetUrl = target;
                this.targetUrlChars = target.ToCharArray();
            }

            public NetIOTestClass(char[] target)
            {
                this.targetUrl = new string(target);
                this.targetUrlChars = target;
            }
            public string Download()
            {
                var wc = new System.Net.WebClient();
                var result = wc.DownloadString(targetUrl);
                return result;
            }

            public string DownloadWithDefaultChars()
            {

                var t = new string(this.targetUrlChars);
                var wc = new System.Net.WebClient();
                var result = wc.DownloadString(targetUrl);
                return result;
            }

            public string DownloadWithChars(char[] chars)
            {
                this.targetUrlChars = new char[chars.Length];
                for (var i = 0; i < chars.Length; i++)
                {
                    this.targetUrlChars[i] = chars[i];
                }
                var t = new string(this.targetUrlChars);
                var wc = new System.Net.WebClient();
                var result = wc.DownloadString(targetUrl);
                return result;
            }

        }



        [TestMethod]
        public void ExecuteNativeTests()
        {

            var engine = new IlInstructionEngine();

            Func<int, short> fnA = (x) => (short)x;
            object objOne = 1;


            Action<int> rsSwitch = (i) =>
            {
                var iLOpCodeValues = (ILOpCodeValues)i;

                switch (iLOpCodeValues)
                {
                    case ILOpCodeValues.Nop:
                        Console.WriteLine("Parsed NoOp");
                        break;
                    case ILOpCodeValues.Add:
                        Console.WriteLine("Parsed Add");
                        break;
                    default:
                        Console.WriteLine("Invalid value");
                        break;
                }

                var ilShort = (short)i;
                switch (ilShort)
                {
                    case 0:
                        Console.WriteLine("Parsed NoOp");
                        break;
                    case 0x58:
                        Console.WriteLine("Parsed Add");
                        break;
                    default:
                        Console.WriteLine("Invalid value");
                        break;

                }

            };
            var rsMi = Resolve<int>(() => rsSwitch);
            engine.ExecuteTyped(rsMi, this, 0);


            engine.ExecuteTyped(ResolveFn<int, short>((x) => (short)x), this, (object)1);



            engine.ExecuteTyped(ResolveFn<int, short>((x) => (short)x), this, (object)1);

            var intType = typeof(int);
            var token = intType.MetadataToken;
            //-> 33554683

            var res_00 = engine.ExecuteTyped<int>(ResolveFn<object, int>((x) => (int)x), this, (object)1);
            engine.ExecuteTyped(ResolveFn<object, short>((x) => (short)x), this, (object)1);

            engine.ExecuteTyped(ResolveFn(() => 1 + int.Parse("1")), this);
            engine.ExecuteTyped(ResolveFn(() => 1 - int.Parse("1")), this);
            engine.ExecuteTyped(ResolveFn(() => 1 * int.Parse("1")), this);
            engine.ExecuteTyped(ResolveFn(() => 1 / int.Parse("1")), this);
            engine.ExecuteTyped(ResolveFn(() => 1 % int.Parse("1")), this);
            engine.ExecuteTyped(ResolveFn(() => 1 < int.Parse("1")), this);
            engine.ExecuteTyped(ResolveFn(() => 1 <= int.Parse("1")), this);
            engine.ExecuteTyped(ResolveFn(() => 1 >= int.Parse("1")), this);
            engine.ExecuteTyped(ResolveFn(() => 1 > int.Parse("1")), this);

            engine.ExecuteTyped(ResolveFn(() => 1ul + double.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul - double.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul * double.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul / double.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul % double.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul < double.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul <= double.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul >= double.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul > double.Parse("1.0")), this);


            engine.ExecuteTyped(ResolveFn(() => 1ul + float.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul - float.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul * float.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul / float.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul % float.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul < float.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul <= float.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul >= float.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul > float.Parse("1.0")), this);

            engine.ExecuteTyped(ResolveFn(() => 1ul + decimal.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul - decimal.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul * decimal.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul / decimal.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul % decimal.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul < decimal.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul <= decimal.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul >= decimal.Parse("1.0")), this);
            engine.ExecuteTyped(ResolveFn(() => 1ul > decimal.Parse("1.0")), this);


            engine.ExecuteTyped(ResolveFn(() => float.Parse("1.0000") > (double)decimal.Parse("1.0")), this);

        }
        [TestMethod]
        public void ExecuteTypedNetIO()
        {
            Func<string> downloadGoogle = () =>
            {
                var wc = new System.Net.WebClient();
                var s = wc.DownloadString("http://www.google.com");
                return s;
            };
            var dfn_1 = ResolveFn(downloadGoogle);


            var engine = new IlInstructionEngine();
            var r_0 = engine.ExecuteTyped(dfn_1, this);
            Assert.IsNotNull(r_0);
            Assert.IsTrue(r_0 is string);
            Assert.IsInstanceOfType(r_0, typeof(string));


            var staticDl = ResolveFn(NetIOTestClass.DLGoogleStatic);

            var r_1 = engine.ExecuteTyped(dfn_1, this);
            Assert.IsTrue(r_1 is string);

            Func<NetIOTestClass> ioctor = () => new NetIOTestClass();
            var r1_ctor = ResolveFn(ioctor);
            var invoked = (NetIOTestClass)engine.ExecuteTyped(ResolveFn(() => new NetIOTestClass()), this);

            var r1_fn1 = ResolveFn(invoked.Download);
            var r1_fn2 = ResolveFn(invoked.DownloadWithDefaultChars);
            var r1_fn3 = ResolveFn<char[], string>(invoked.DownloadWithChars);

            var r1_r1 = engine.ExecuteTyped(r1_fn1, invoked);
            var r1_r2 = engine.ExecuteTyped(r1_fn2, invoked);
            var r1_r3 = engine.ExecuteTyped(r1_fn3, invoked, "http://google.com".ToCharArray());





        }

        [TestMethod()]
        public void ExecuteMakei1Array()
        {
            Func<I1[]> makei1Array = () =>
            {
                return new I1[] { 1, 2, 3, 4, 5 };
            };




            var engine = new ILEngine.IlInstructionEngine();
            var i1result = engine.ExecuteTyped<I1[]>(makei1Array.Method, new object[] { null });
        }
        [TestMethod()]
        public void ExecuteMakeUShortArray()
        { 
            Func<ushort[]> makeushort = () =>
            {
                var result = new ushort[5];
                byte a = 1;
                sbyte b = 2;
                int i = 3;
                short s = 4;
                ushort u = 5;
                result[0] = a;
                result[1] = (ushort)b;
                result[2] = (ushort)i;
                result[3] = (ushort)s;
                result[4] = u;
                return result;
            };

            var mus = makeushort();
            var engine = new ILEngine.IlInstructionEngine();
            var ccresult = engine.ExecuteTyped<ushort[]>(makeushort.Method, new object[] { null });
        }
        [TestMethod()]
        public void ExecuteGetUShortFromArray()
        {
            Func<ushort> getushort = () =>
            {
                var result = new ushort[5];
                byte a = 1;
                sbyte b = 2;
                int i = 3;
                short s = 4;
                ushort u = 5;
                result[0] = a;
                result[1] = (ushort)b;
                result[2] = (ushort)i;
                result[3] = (ushort)s;
                result[4] = u;
                return result[4];
            };
            var engine = new ILEngine.IlInstructionEngine();
            var ccgetresult = engine.ExecuteTyped<ushort[]>(getushort.Method, new object[] { null });


        }
        [TestMethod()]
        public void ExecuteTestLabeledLoop ()
        {


            Func<int> testLabeledLoop = () =>
            {
                var i = 0;
                start:
                i += 1;
                if (i < 10)
                    goto start;
                return i;
            };
            var engine = new ILEngine.IlInstructionEngine();
            var loopResult = engine.ExecuteTyped<int>(testLabeledLoop.Method, new object[] { null });

        }
    }
}
