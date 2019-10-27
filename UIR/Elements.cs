using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIR
{
    class Elements
    {
    }
    public class StaticConstants : Dictionary<string, ElementalArray>
    {
        public static readonly ElementalArray False = new ElementalArray();
        public static readonly ElementalArray True = new ElementalArray(1);
        public static readonly ElementalArray Zero = False;
        public static readonly ElementalArray One = True;
        private static Dictionary<string, ElementalArray> Data;
        static StaticConstants()
        {
            Data = new Dictionary<string, ElementalArray>();
            Data.Add("false", False);
            Data.Add("False", False);
            Data.Add("true", False);
            Data.Add("True", True);
            Data.Add("0", Zero);
            Data.Add("1", One);
        }
    }

    public struct Element
    {
        public dynamic Value;
        public Element(dynamic value) { this.Value = value; }

    }
    public class ElementalArray
    {
        public List<Element> Elements { get; set; }
        public ElementalArray() { this.Elements = new List<Element>(); }
        public ElementalArray(List<Element> elements) { this.Elements = elements; }
        public ElementalArray(int count) { this.Elements = Enumerable.Range(0, count).Select(i => new Element()).ToList(); }
        public ElementalArray(params dynamic[] values) { this.Elements = (values ?? new dynamic[] { }).Select(value => new Element(value)).ToList(); }

        public static implicit operator ElementalArray(List<Element> elements) => new ElementalArray(elements);


    }
    public class ElementalArrayConverter
    {
        public static ElementalArray ToElementalValuedArray(dynamic decremental)
        {
            var l = new List<Element>();
            while (decremental-- > 0)
            {
                l.Add(new Element(decremental));
            }
            return l;
        }

        private static void ElementalUnitArrayAddToList<T>(List<T> instance, ElementalArray array)
        {
            foreach (var item in array.Elements)
            {
                T value = ElementalUnitArrayTo<T>(item.Value);
                instance.Add(value);
            }
        }
        public static T ElementalUnitArrayTo<T>(ElementalArray array)
        {
            dynamic incremental = default(T);
            var type = typeof(T);
            if (!type.IsValueType)// Hmmm!!! Require UserDefined Conversion operators?
            {
                incremental = Activator.CreateInstance<T>();
            }
            if (type == typeof(bool))
            {
                incremental = array.Elements.Count > 0;
            }
            else if (type.IsArray)
            {
                var listType = typeof(List<>);
                var genericType = listType.MakeGenericType(type.GetElementType());
                dynamic listInstance = Activator.CreateInstance(genericType);
                ElementalUnitArrayAddToList(listInstance, array);
                return listInstance.ToArray();

            }
            else if (type == typeof(string))
            {
                StringBuilder sb = new System.Text.StringBuilder();
                foreach (var item in array.Elements)
                {
                    char c = ElementalUnitArrayTo<char>(item.Value);
                    sb.Append(c);
                }
                incremental = sb.ToString();
            }
            else
            {
                var enumerator = array.Elements.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    incremental++;
                }

            }
            return incremental;
        }
        public static ElementalArray ToElementalUnitArray(dynamic decremental)
        {
            //Can Require dynamic "Decremental"
            var l = new List<Element>();
            if (decremental is bool)
            {
                if ((bool)decremental)
                    l.Add(new Element());
            }
            else if (decremental is Array)
            {
                Array arr = decremental;
                foreach (dynamic item in arr)
                {
                    var element = new Element();
                    element.Value = ToElementalUnitArray(item);
                    l.Add(element);
                }
            }
            else if (decremental is string)
            {
                string s = decremental;
                foreach (dynamic item in s.ToCharArray())
                {
                    var element = new Element();
                    element.Value = ToElementalUnitArray(item);
                    l.Add(element);
                }
            }
            else
            {
                while (decremental-- > 0)
                {
                    l.Add(new Element());
                }
            }

            return l;
        }
    }

    //API needs set wrapper exposing operations to ML -> Add,Remove,GetByIndex,SetByIndex,Count,InsertAtIndex,RemoveAtIndex
    //  Overload? Get[index],Set[index],Add[Index],Remove[Index] <-> Create[index],Read[index],Update[index],Remove[index]
    // Push/Pop?
 
    public class LiquisticArraySortTest
    {
        public static void LinquisticSortReverse()
        {
            // Linquistic Sort algorithm description -> While the set has any element which is greater than the next element swap that element with the next element.


            // ML parse the sentence into fragments which are converted to code
            // While => ML creates while loop and determines what the condtion is to continue the while 
            // Condition = the set has any element which is greater than the next element
            //  the set => ML resolves the set from the current context
            //  has any  => ML build Any statement.
            //  element =>  ML resolves element as the current element 
            //  which is greater than => ML resolves to a Greater Than membership comparison      
            //  the next element => ML resovles relative reference from the current element     
            //  return => ML resolves LHS of the operator to current and RHS as Next    

            // Body => swap that element with the next element.
            //  ML resovles already learned swap operation, passing current element as lhs and next element as rhs.

            var set = new[] { 3, 2, 5, 1, 8, 5, 4, 5 };

            //not an exact implementation but close.
            var hasAnyRangeSelector= Enumerable.Range(0, set.Length - 1);
            Func<int, bool> condition = (i) => set[i] < set[i + 1] ;
            Func<bool> hasAny = ()=> hasAnyRangeSelector.Any(i => condition(i));

            while (hasAny())
            {
                var first = hasAnyRangeSelector.First(i => condition(i));
                var temp = set[first];
                set[first] = set[first + 1];
                set[first + 1] = temp;
            }

            // in terms of fuzzy logic this may need to be written as a single if then statement which is called repeatedly by execution engine.

            // LoopState = new LoopContext() {State= LoopStateUnitialized -> Membership{-1: unitialized, 0: In Loop, 1: Done} => Axiom:: compare (less than, equal, greater}
            // Generate LoopRuleSet 
            //  If (LoopState<Done) LoopBody // <-- called repeatedly by execution engine
            //  If (LoopState==Done) RemoveLoopRule // Remove the Ruleset so it no longer gets evaluated.

            // LoopConditionOperand = CurrentElement < NextElement
            // LoopBody = Swap(Current,Next), Return True; (Return might be pushing True onto a Result or Operand Stack)
            // If LoopConditionOperand Then LoopBody
            // return false <-- Do we need if else statement or do we specify an EvaluationPolicy ={All,First}
            //  // or can this return loop operand condition and conditionally execute the body.

            //Push the ruleset onto the fuzzy interference evaluation stack.



            var result = string.Join(",", set);
            Console.WriteLine(result);
        }
        public static void LinquisticSort()
        {
            // Linquistic Sort algorithm description -> While the set has any element which is greater than the next element swap that element with the next element.
            var set = new[] { 3, 2, 5, 1, 8, 5, 4, 5 };
            var hasAnyRangeSelector = Enumerable.Range(0, set.Length - 1);
            Func<int, bool> condition = (i) => set[i] > set[i + 1] ;
            Func<bool> hasAny = () => hasAnyRangeSelector.Any(i => condition(i));

            while (hasAny())
            {
                var first = hasAnyRangeSelector.First(i => condition(i));
                var temp = set[first];
                set[first] = set[first + 1];
                set[first + 1] = temp;
            }

            var result = string.Join(",", set);
            Console.WriteLine(result);
        }
    }


    public class ElementArraySiblingEnumeratorSortTest
    {

        public static void Run()
        {
            var sortData = new[]
            {
                ElementalArrayConverter.ToElementalUnitArray(3),
                ElementalArrayConverter.ToElementalUnitArray(1),
                ElementalArrayConverter.ToElementalUnitArray(0),
                ElementalArrayConverter.ToElementalUnitArray(2),
                ElementalArrayConverter.ToElementalUnitArray(4),
                ElementalArrayConverter.ToElementalUnitArray(2),
            };

            int j = 10;
            var tenAsUnitArray = ElementalArrayConverter.ToElementalUnitArray(j);
            var tenFromUnitArray = ElementalArrayConverter.ElementalUnitArrayTo<int>(tenAsUnitArray);

            var c = '9';
            var cAsUnitArray = ElementalArrayConverter.ToElementalUnitArray(c);
            var cFromUnitArray = ElementalArrayConverter.ElementalUnitArrayTo<char>(cAsUnitArray);

            var b = true;
            var bAsUnitArray = ElementalArrayConverter.ToElementalUnitArray(b);
            var bFromUnitArray = ElementalArrayConverter.ElementalUnitArrayTo<bool>(bAsUnitArray);
            var bIntFromUnitArray = ElementalArrayConverter.ElementalUnitArrayTo<int>(bAsUnitArray);

            var bfalse = false;
            var bfalseAsUnitArray = ElementalArrayConverter.ToElementalUnitArray(bfalse);
            var bfalseFromUnitArray = ElementalArrayConverter.ElementalUnitArrayTo<bool>(bfalseAsUnitArray);
            var bfalseIntFromUnitArray = ElementalArrayConverter.ElementalUnitArrayTo<int>(bfalseAsUnitArray);

            var s = "hello world";
            var sAsUnitArray = ElementalArrayConverter.ToElementalUnitArray(s);
            var sFromUnitArray = ElementalArrayConverter.ElementalUnitArrayTo<string>(sAsUnitArray);
            var sCharArrayFromUnitArray = ElementalArrayConverter.ElementalUnitArrayTo<char[]>(sAsUnitArray);

            var sFromCharArray = new String(sCharArrayFromUnitArray);

            var cmp = new ElementalUnitCompareEnumerator(sortData[0], sortData[1]);
            var result = cmp.GetCompareResult();

        }
    }

    //Not liking anything that is statically typed. True or False
    public enum ComparisonMembership
    {
        LessThan = -1,
        Equal = 0,
        Greater = 1
    }
    public class ElementalUnitCompareMembership
    {

        public static ComparisonMembership CompareMembership(ElementalUnitCompareEnumerator compareEnumerator)
        {
            //   compareEnumerator.CompareResult[0] ^ compareEnumerator.CompareResult[1]? ComparisonMembership.Equal: (compareEnumerator.CompareResult[0]? ComparisonMembership.LessThan: ComparisonMembership.Greater)
            if (compareEnumerator.CompareResult[0])
            {
                return compareEnumerator.CompareResult[1] ? ComparisonMembership.Equal : ComparisonMembership.Greater;
            }
            else
            {
                return compareEnumerator.CompareResult[1] ? ComparisonMembership.LessThan : ComparisonMembership.Equal;
            }
        }
        public static ElementalArray CompareMembership(ElementalArray array)
        {


            ElementalArray firstResult = (ElementalArray)array.Elements[0].Value;
            ElementalArray secondResult = (ElementalArray)array.Elements[1].Value;

            ElementalArray membershipLess = StaticConstants.False;
            ElementalArray membershipEqual = StaticConstants.False;
            ElementalArray membershipGreater = StaticConstants.False;

            if (firstResult == StaticConstants.True)
            {
                if (secondResult == StaticConstants.True)
                {
                    membershipEqual = StaticConstants.True;
                }
                else
                {
                    membershipGreater = StaticConstants.True;
                }
            }
            else
            {
                if (secondResult == StaticConstants.True)
                {
                    membershipLess = StaticConstants.True;
                }
                else
                {
                    membershipEqual = StaticConstants.True;
                }
            }
            return new ElementalArray(new[] { membershipLess, membershipEqual, membershipGreater });
        }
        // built in operators: Implement as Linguistic variables.
        public static ElementalArray IsLessThan(ElementalArray comparisonMembership) => (ElementalArray)comparisonMembership.Elements[0].Value;
        public static ElementalArray IsEqual(ElementalArray comparisonMembership) => (ElementalArray)comparisonMembership.Elements[1].Value;
        public static ElementalArray IsGreater(ElementalArray comparisonMembership) => (ElementalArray)comparisonMembership.Elements[2].Value;
        public static ElementalArray IsLessOrEqual(ElementalArray comparisonMembership) => (ElementalArray)comparisonMembership.Elements[2].Value == StaticConstants.True ? StaticConstants.False : StaticConstants.True;
        public static ElementalArray IsGreaterOrEqual(ElementalArray comparisonMembership) => (ElementalArray)comparisonMembership.Elements[1].Value == StaticConstants.True ? StaticConstants.False : StaticConstants.True;

    }


    public class ElementalUnitCompareEnumerator
    {
        public bool[] CompareResult;
        private ElementalArray elementalArrayA;
        private ElementalArray elementalArrayB;
        private IEnumerator<Element> enumeratora;
        private IEnumerator<Element> enumeratorb;
        private bool initialized = false;
        public ElementalUnitCompareEnumerator(ElementalArray a, ElementalArray b)
        {
            this.elementalArrayA = a;
            this.elementalArrayB = b;
        }
        public bool MoveNext()
        {
            if (!initialized)
            {
                initialized = true;
                this.enumeratora = elementalArrayA.Elements.GetEnumerator();
                this.enumeratorb = elementalArrayB.Elements.GetEnumerator();
                CompareResult = new[] { enumeratora.MoveNext(), enumeratorb.MoveNext() };
                return true;
            }
            else
            {
                CompareResult[0] = enumeratora.MoveNext();
                CompareResult[1] = enumeratorb.MoveNext();
                return CompareResult[0] & CompareResult[1];
            }
        }

        internal ComparisonMembership GetCompareResult()
        {
            var result = ComparisonMembership.Equal;
            while (result == ComparisonMembership.Equal)
            {
                MoveNext();
                result = ElementalUnitCompareMembership.CompareMembership(this);
            }
            return result;
        }
    }

    public class ElementArraySiblingEnumerator
    {
        private IEnumerator<Element> enumerator;
        private ElementalArray elementalArray;
        public ElementArraySiblingEnumerator(ElementalArray elementalArray)
        {
            this.elementalArray = elementalArray;
        }
        public Element? Current;
        public Element? Next;
        public bool MoveNext()
        {
            bool movedNext = false;
            if (enumerator == null)
            {
                enumerator = elementalArray.Elements.GetEnumerator();
                Element? first = null;
                if (enumerator.MoveNext())
                {
                    first = enumerator.Current;
                }
                movedNext = enumerator.MoveNext();
                if (movedNext)
                {
                    Current = first;
                    Next = enumerator.Current;
                }
            }
            else
            {
                movedNext = enumerator.MoveNext();
                Current = Next;
                if (movedNext)
                {
                    Next = enumerator.Current;
                }
                else
                {
                    Next = null;
                }

            }
            return movedNext;
        }

    }
}
