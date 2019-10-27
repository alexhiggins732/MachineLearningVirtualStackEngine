using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIR
{
    class Logic
    {
    }

    namespace NullV3
    {

        public static class UnitTests
        {
            public static void Run()
            {
                var Null = NullUnit.Null;
                var False = NullUnit.False;
                var True = NullUnit.True;


                var nullEqualsNull = Null == (Null as NullUnit?).Value;
                var nullEqualsFalse = Null == (False as NullUnit?).Value;
                var nullEqualsTrue = Null == (True as NullUnit?).Value;

                var falseEqualsNull = False == (Null as NullUnit?).Value;
                var falseEqualsFalse = False == (False as NullUnit?).Value;
                var falseEqualsTrue = False == (True as NullUnit?).Value;

                var trueEqualsNull = True == (Null as NullUnit?).Value;
                var trueEqualsFalse = True == (False as NullUnit?).Value;
                var trueEqualsTrue = True == (True as NullUnit?).Value;

                var nullHashCode = Null.GetHashCode();
                var falseHashCode = False.GetHashCode();
                var trueHashCode = True.GetHashCode();

                var l_1 = new List<NullUnit>
                {
                    Null, False, True,
                    nullEqualsNull, nullEqualsFalse, nullEqualsTrue,
                    falseEqualsNull, falseEqualsFalse, falseEqualsTrue,
                    trueEqualsNull, trueEqualsFalse,  trueEqualsTrue,
                };

                var distinctUnits = l_1.Distinct().ToList();
                var sortedUnits = l_1.OrderBy(x => x).ToList();

                var d = new List<object>();
                d.Add(false);
                d.Add(null);
                d.Add(true);
                var d_dis = d.Distinct().ToList();
                d.Clear();
                d.AddRange(l_1.Cast<object>());

                d.Sort();


            }

            public struct Unit { }
            public struct NullUnit : IEquatable<NullUnit>, IComparable, IComparable<NullUnit>
            {
                public static readonly NullUnit Null = new NullUnit(); //Ideally this is null
                public static readonly NullUnit False = new NullUnit(Null); //Ideally new NullUnit(new NullUnit()).. NullUnit with value= NullUnit with value= null
                public static readonly NullUnit True = new NullUnit(False); //Ideally new NullUnit(new NullUnit(New NullUnit()))


                //We could take this path
                public static readonly NullUnit NotNull = new NullUnit(Null); //Ideally this is null

                public static readonly NullUnit FalseFromNull = Null; //Ideally this is null
                public static readonly NullUnit TrueFromNotNull = NotNull; //Ideally this is null

                // But if we do there is no map mapping for a nullable boolean. So any logic rules built on this system would not be able to represent the concept of null or nothing.
                // Hmmm->  When Exists(Anything)=False is equivalent to nothing or null.  
                //  In other words nothing exists which is False statement, eg Exists(Anything)=False)==False because exists(nothing)=false

                // Exists(nothing)=false implies something exists, existing(nothing)=true implies the concept that "nothing exists".
                //  Supposed nothing exists. That implies there is nothing for all of infinity in all directions across all dimensions.  
                //      Otherwise, if we where to find anything, anywhere then we have something and something does not equal nothing.
                //      Therefore nothing does not exist, so exists(nothing) logically = false because nothing does not exist.
                //      But exists(false)=true because false does exist which we can prove because exists(nothing)=false <==>  exists(exists(nothing))=exists(false)=true

                //  Can something not exists, or exists(notexists). True
                // logic short circuits -> exists(false)=false how can false not exist if false was passed as parameter, 

                public static readonly NullUnit NotExists = Null; //exists(nothing)
                public static readonly NullUnit Exists = NotNull; //exists(exists(nothing))

                //even in programming, null is actually a null pointer, and a nil pointer is a false statement, eg 1 > 2.
                //Null pointer is a special reserved VALUE of a pointer (0) which points to invalid memory location. 
                //A pointer of any type has such a reserved value (char, int, etc). Void is Type as opposed to a value.

                private object value;
                public NullUnit(object value)
                {
                    this.value = value;
                }
                private NullUnit unitValue => (value is NullUnit) ? (NullUnit)value : Null;
                public bool IsNull => value == null;
                public bool IsFalse => value != null && unitValue.value == null;
                public bool IsTrue => value != null && unitValue.value != null;

                public override bool Equals(object obj)
                {
                    return obj is NullUnit && Equals((NullUnit)obj);
                }

                public bool Equals(NullUnit other)
                {
                    return (IsNull || other.IsNull) ? false : (IsTrue == other.IsTrue);
                }

                public override int GetHashCode()
                {
                    return IsNull ? -1 : (IsFalse ? 0 : 1);
                }

                public override string ToString()
                {
                    var result =
                    $"{nameof(NullUnit)}.{(IsNull ? "Null" : IsTrue.ToString())}";
                    return result;
                }

                public int CompareTo(object obj)
                {
                    if (!(obj is NullUnit))
                        throw new NotImplementedException();
                    return CompareTo((NullUnit)obj);
                }

                public int CompareTo(NullUnit other)
                {
                    return GetHashCode().CompareTo(other.GetHashCode());
                }

                public static NullUnit operator ==(NullUnit x, NullUnit y)
                {
                    if (x.IsNull || y.IsNull) return Null;
                    return x.IsTrue == y.IsTrue ? True : False;
                }

                // Inequality operator. Returns Null if either operand is Null, otherwise
                // returns True or False.

                public static NullUnit operator !=(NullUnit x, NullUnit y)
                {
                    if (x.IsNull || y.IsNull) return Null;
                    return x.IsTrue != y.IsTrue ? True : False;
                }



            }
        }
    }
    namespace NullV2
    {
        public struct Unit { }
        public struct NullUnit : IEquatable<NullUnit>
        {
            public static readonly NullUnit Null = new NullUnit(); //Ideally this is null
            public static readonly NullUnit False = new NullUnit(new Unit()); //Ideally new NullUnit(new NullUnit()).. NullUnit with value= NullUnit with value= null
            public static readonly NullUnit True = new NullUnit(new NullUnit()); //Ideally new NullUnit(new NullUnit(New NullUnit()))
            private object value;
            public NullUnit(object value)
            {
                this.value = value;
            }
            public bool IsNull => value == null;
            public bool IsFalse => value is Unit;
            public bool IsTrue => value is NullUnit;

            public override bool Equals(object obj)
            {
                return obj is NullUnit && Equals((NullUnit)obj);
            }

            public bool Equals(NullUnit other)
            {
                return (IsNull || other.IsNull) ? false : (IsTrue == other.IsTrue);
            }

            public override int GetHashCode()
            {
                return IsNull ? 0 : (IsFalse ? -1 : 1);
            }

            public override string ToString() => $"{nameof(NullUnit)}.{(IsNull ? "Null" : IsTrue.ToString())}";

            public static NullUnit operator ==(NullUnit x, NullUnit y)
            {
                if (x.IsNull || y.IsNull) return Null;
                return x.IsTrue == y.IsTrue ? True : False;
            }

            // Inequality operator. Returns Null if either operand is Null, otherwise
            // returns True or False.

            public static NullUnit operator !=(NullUnit x, NullUnit y)
            {
                if (x.IsNull || y.IsNull) return Null;
                return x.IsTrue != y.IsTrue ? True : False;
            }



        }
        public struct BoolUnit
        {
            public static readonly BoolUnit Null = new BoolUnit(NullUnit.Null);
            public static readonly BoolUnit False = new BoolUnit(NullUnit.False);
            public static readonly BoolUnit True = new BoolUnit(NullUnit.True);

            NullUnit value;
            public BoolUnit(NullUnit value)
            {
                this.value = value;
            }


            public bool IsNull { get { return value.IsNull; } }

            public bool IsFalse { get { return value.IsFalse; } }

            public bool IsTrue { get { return value.IsTrue; } }

            // Implicit conversion from bool to BoolUnit. Maps true to BoolUnit.True and
            // false to BoolUnit.False.

            public static implicit operator BoolUnit(bool x)
            {
                return x ? True : False;
            }

            // Explicit conversion from BoolUnit to bool. Throws an exception if the
            // given BoolUnit is Null, otherwise returns true or false.

            public static explicit operator bool(BoolUnit x)
            {
                if (x.value.IsNull) throw new InvalidOperationException();
                return x.value.IsTrue;
            }

            // Equality operator. Returns Null if either operand is Null, otherwise
            // returns True or False.

            public static BoolUnit operator ==(BoolUnit x, BoolUnit y)
            {
                //v1:
                //  if (x.value.IsNull || y.value.IsNull) return Null;
                //  return new BoolUnit(x.value == y.value);

                //v2:
                //  if (x.value.IsNull || y.value.IsNull) return Null;
                //    (x.value.IsTrue == y.value.IsTrue) ? True : False;

                //v3: //TODO: profile effect of new object allocation:
                //  return new BoolUnit(x.value == y.value);

                if (x.IsNull || y.IsNull) return Null;
                return x.IsTrue == y.IsTrue ? True : False;
            }

            // Inequality operator. Returns Null if either operand is Null, otherwise
            // returns True or False.

            public static BoolUnit operator !=(BoolUnit x, BoolUnit y)
            {
                if (x.IsNull || y.IsNull) return Null;
                return x.IsTrue != y.IsTrue ? True : False;
            }

            // Logical negation operator. Returns True if the operand is False, Null
            // if the operand is Null, or False if the operand is True.

            public static BoolUnit operator !(BoolUnit x)
            {
                return x.IsTrue ? False : (x.IsFalse ? True : Null);
            }

            // Logical AND operator. Returns False if either operand is False,
            // otherwise Null if either operand is Null, otherwise True.

            public static BoolUnit operator &(BoolUnit x, BoolUnit y)
            {
                return (x.IsNull || y.IsNull) ? Null : (x.IsFalse || y.IsFalse ? False : True);
            }

            // Logical OR operator. Returns True if either operand is True, otherwise
            // Null if either operand is Null, otherwise False.

            public static BoolUnit operator |(BoolUnit x, BoolUnit y)
            {
                return (x.IsNull || y.IsNull) ? Null : (x.IsTrue || y.IsTrue ? True : False);
            }

            // Definitely true operator. Returns true if the operand is True, false
            // otherwise.

            public static bool operator true(BoolUnit x)
            {
                return x.value.IsTrue;
            }

            // Definitely false operator. Returns true if the operand is False, false
            // otherwise.

            public static bool operator false(BoolUnit x)
            {
                return x.value.IsFalse;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is BoolUnit)) return false;
                return (this == ((BoolUnit)obj)).IsTrue;

            }

            public override int GetHashCode()
            {
                return value.IsNull ? 0 : (value.IsFalse ? -1 : 1);
            }

            public override string ToString()
            {
                if (value.IsTrue) return $"{nameof(BoolUnit)}.True";
                if (value.IsFalse) return $"{nameof(BoolUnit)}.False";
                return $"{nameof(BoolUnit)}.Null";
            }

        }
    }
    public class NullUnitContext
    {
        Unit Evaluate() => Unit.Null;
    }

    public class NullUnitSet : IEnumerable<Unit>
    {
        public IEnumerator<Unit> GetEnumerator() => new Enumerator();
        //{
        //    //var l = new List<Unit> { Unit.Null };
        //    //var en = l.GetEnumerator();
        //    //var ein = (IEnumerator)en;
        //    //ein.Reset();
        //    //return l.GetEnumerator();
        //}

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



        /// <summary>
        ///  Enumerates the elements of the Null Unit Set
        /// </summary>
        public struct Enumerator : IEnumerator<Unit>, IDisposable, IEnumerator
        {
            /// <summary>
            ///  Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            ///  The element in the NullUnitSet at the current position of the enumerator.
            /// </returns>
            public Unit Current { get; private set; }

            /// <summary>
            ///  Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            ///  The element in the NullUnitSet at the current position of the enumerator.
            /// </returns>
            object IEnumerator.Current => Current;

            /// <summary>
            /// Releases all resources used by the NullUnitSet Enumerator.
            /// </summary> 
            public void Dispose() { }

            /// <summary>
            /// Advances the enumerator to the next element of the NullUnitSet.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            /// 
            private bool empty;
            public bool MoveNext() => !empty && (empty = true);


            /// <summary>
            ///   Sets the enumerator to its initial position, which is before the first element  in the collection.
            /// </summary>
            public void Reset() => Current = new Unit();

        }

    }

    public class BoolUnitContext
    {
        BoolUnit Evaluate() => false;
        BoolUnit Evaluate(Unit unit) => true;
    }

    public struct Unit
    {
        public static readonly Unit Null = new Unit();
        private object value;
        public Unit(object value)
        {
            this.value = value;
        }
        public bool IsNull => value == null;
        public bool IsNotNull => !IsNull;

        public override string ToString() => $"{nameof(Unit)}.{(IsNull ? "Null" : "NotNull")}";

    }

    public struct BoolUnit
    {

        // The three possible nameof(BoolUnit) values.

        public static readonly BoolUnit Null = new BoolUnit(0);
        public static readonly BoolUnit False = new BoolUnit(-1);
        public static readonly BoolUnit True = new BoolUnit(1);

        // Private field that stores -1, 0, 1 for False, Null, True.

        sbyte value;

        // Private instance constructor. The value parameter must be -1, 0, or 1.

        BoolUnit(int value)
        {
            this.value = (sbyte)value;
        }

        // Properties to examine the value of a BoolUnit. Return true if this
        // BoolUnit has the given value, false otherwise.

        public bool IsNull { get { return value == 0; } }

        public bool IsFalse { get { return value < 0; } }

        public bool IsTrue { get { return value > 0; } }

        // Implicit conversion from bool to BoolUnit. Maps true to BoolUnit.True and
        // false to BoolUnit.False.

        public static implicit operator BoolUnit(bool x)
        {
            return x ? True : False;
        }

        // Explicit conversion from BoolUnit to bool. Throws an exception if the
        // given BoolUnit is Null, otherwise returns true or false.

        public static explicit operator bool(BoolUnit x)
        {
            if (x.value == 0) throw new InvalidOperationException();
            return x.value > 0;
        }

        // Equality operator. Returns Null if either operand is Null, otherwise
        // returns True or False.

        public static BoolUnit operator ==(BoolUnit x, BoolUnit y)
        {
            if (x.value == 0 || y.value == 0) return Null;
            return x.value == y.value ? True : False;
        }

        // Inequality operator. Returns Null if either operand is Null, otherwise
        // returns True or False.

        public static BoolUnit operator !=(BoolUnit x, BoolUnit y)
        {
            if (x.value == 0 || y.value == 0) return Null;
            return x.value != y.value ? True : False;
        }

        // Logical negation operator. Returns True if the operand is False, Null
        // if the operand is Null, or False if the operand is True.

        public static BoolUnit operator !(BoolUnit x)
        {
            return new BoolUnit(-x.value);
        }

        // Logical AND operator. Returns False if either operand is False,
        // otherwise Null if either operand is Null, otherwise True.

        public static BoolUnit operator &(BoolUnit x, BoolUnit y)
        {
            return new BoolUnit(x.value < y.value ? x.value : y.value);
        }

        // Logical OR operator. Returns True if either operand is True, otherwise
        // Null if either operand is Null, otherwise False.

        public static BoolUnit operator |(BoolUnit x, BoolUnit y)
        {
            return new BoolUnit(x.value > y.value ? x.value : y.value);
        }

        // Definitely true operator. Returns true if the operand is True, false
        // otherwise.

        public static bool operator true(BoolUnit x)
        {
            return x.value > 0;
        }

        // Definitely false operator. Returns true if the operand is False, false
        // otherwise.

        public static bool operator false(BoolUnit x)
        {
            return x.value < 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BoolUnit)) return false;
            return value == ((BoolUnit)obj).value;
        }

        public override int GetHashCode()
        {
            return value;
        }

        public override string ToString()
        {
            if (value > 0) return $"{nameof(BoolUnit)}.True";
            if (value < 0) return $"{nameof(BoolUnit)}.False";
            return $"{nameof(BoolUnit)}.Null";
        }

    }


    public class BoolUnitSet : IEnumerable<BoolUnit>
    {
        public IEnumerator<BoolUnit> GetEnumerator()
        {
            var l = new List<BoolUnit> { false, true };
            return l.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <summary>
        ///  Enumerates the elements of the BoolUnitSet
        /// </summary>
        public struct Enumerator : IEnumerator<BoolUnit>, IDisposable, IEnumerator
        {
            /// <summary>
            ///  Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            ///  The element in the BoolUnitSet at the current position of the enumerator.
            /// </returns>
            public BoolUnit Current { get; private set; }

            /// <summary>
            ///  Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            ///  The element in the NullUnitSet at the current position of the enumerator.
            /// </returns>
            object IEnumerator.Current => Current;

            /// <summary>
            /// Releases all resources used by the BoolUnitSet Enumerator.
            /// </summary> 
            public void Dispose() { }

            /// <summary>
            /// Advances the enumerator to the next element of the BoolUnitSet.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            /// 
            public bool MoveNext() => ((Current.IsNull && (Current = false).IsFalse)) || (Current.IsFalse && (Current = true).IsTrue);


            /// <summary>
            ///   Sets the enumerator to its initial position, which is before the first element  in the collection.
            /// </summary>
            public void Reset() => Current = BoolUnit.Null;

        }

    }

}
