using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace ILEngineTests
{
    [ExcludeFromCodeCoverage]
    public class ConvertWip
    {


        public struct I1
        {
            public sbyte value; public override string ToString() => $"{value}";
            public static implicit operator sbyte(I1 value) => value.value;
            public static implicit operator I1(sbyte value) => new I1 { value = value };
        }
        public struct I2 { public short value; public override string ToString() => $"{value}"; }
        public struct I4 { public int value; public override string ToString() => $"{value}"; }
        public struct I8 { public long value; public override string ToString() => $"{value}"; }
        public struct U1 { public byte value; public override string ToString() => $"{value}"; }
        public struct U2 { public short value; public override string ToString() => $"{value}"; }
        public struct U4 { public uint value; public override string ToString() => $"{value}"; }
        public struct U8 { public ulong value; public override string ToString() => $"{value}"; }
        public struct R32 { public float value; public override string ToString() => $"{value}"; }
        public struct R64 { public float value; public override string ToString() => $"{value}"; }
        public struct D { public decimal value; public override string ToString() => $"{value}"; }

        public class TypePtrConvert
        {
            public static I2 ToI2(I1 value)
            {
                unsafe { return *(I2*)&value; }
            }
            public static I4 ToI4(I1 value)
            {
                unsafe { return *(I4*)&value; }
            }
            public static I8 ToI8(I1 value)
            {
                unsafe { return *(I8*)&value; }
            }
            public static U1 ToU1(I1 value)
            {
                unsafe { return *(U1*)&value; }
            }
            public static U2 ToU2(I1 value)
            {
                unsafe { return *(U2*)&value; }
            }
            public static U4 ToU4(I1 value)
            {
                unsafe { return *(U4*)&value; }
            }
            public static U8 ToU8(I1 value)
            {
                unsafe { return *(U8*)&value; }
            }
            public static R32 ToR32(I1 value)
            {
                return new R32 { value = value.value };
            }
            public static R64 ToR64(I1 value)
            {
                return new R64 { value = value.value };
            }
            public static D ToD(I1 value)
            {
                return new D { value = value.value };
            }

            public static I1 ToI1(R32 value)
            {
                unsafe { return *(I1*)&value; }
            }


            public static I8 ToI8(R32 value)
            {
                unsafe { return *(I8*)&value; }
            }
        }

        public class ObjectStackTests
        {
            class Unboxer<T>
            {
                Type TFrom;
                public Unboxer() => TFrom = typeof(T);
                public T Unbox(object obj)
                {
                    if (!(obj is T))
                    {
                        var msg = string.Format("Object is not of the type {0}.", this.TFrom.FullName);
                        throw new ArgumentException(msg, "obj");
                    }

                    // Can throw exception, it's ok.
                    return (T)obj;
                }
            }

            static public T? DynamicCast<T>(dynamic src) where T : struct
            {
                T result = default(T);
                try { result = (T)src; }
                catch { }
                return result;
            }
            static public object DynamicCastObject(object src, Type type)
            {
                var result = typeof(ObjectStackTests)
                    .GetMethod("DynamicCast")
                    .MakeGenericMethod(new[] { type })
                    .Invoke(null, new[] { src });
                return result;

            }

            public interface IArray
            {
                object this[int index] { get; set; }
                void SetValue(object value, int index);
                object GetValue(int index);
            }
            public interface IArray<T> : IArray
            {
                new T this[int index] { get; set; }
                new void SetValue(object value, int index); // => this[index] = (T)value;

                new T GetValue(int index);
            }
            public class ArrayBase<T> : IArray<T>
            {
                protected T[] data;
                private readonly Type typeOfT = typeof(T);
                //private readonly T defaultT = default(T);
                private bool iConvertible = (default(T) is IConvertible);
                private TypeCode typeCode = Convert.GetTypeCode(default(T));
                public ArrayBase(int length) => this.data = new T[length];
                public ArrayBase(T[] data) => this.data = (T[])data.Clone();
                object IArray.this[int index]
                {
                    get => this[index];
                    set
                    {
                        if (iConvertible && value is IConvertible && !(value is T))
                        {

                            data[index] = (T)Convert.ChangeType(value, typeCode);
                        }
                        else
                        {
                            data[index] = (T)value;
                        }

                    }
                }

                public T this[int index] { get => data[index]; set => data[index] = value; }
                public void SetValue(object value, int index) => this[index] = (T)value;
                public T GetValue(int index) => data[index];

                object IArray.GetValue(int index) => GetValue(index);

            }
            public class I8Array : ArrayBase<sbyte>
            {

                public I8Array(int length) : this(new sbyte[length]) { }
                public I8Array(sbyte[] value) : base(value) { }
            }

            public class GenericConverter
            {
                public static sbyte ConvertU1ToI1(byte instance) => (sbyte)instance;
                public static sbyte ConvertU2ToI1(ushort instance) => (sbyte)instance;
                public static sbyte ConvertU4ToI1(uint instance) => (sbyte)instance;
                public static sbyte ConvertU8ToI1(ulong instance) => (sbyte)instance;
                public static sbyte ConvertI1ToI1(ushort instance) => (sbyte)instance;

            }

            public class SByteArray : IArray
            {
                public sbyte[] data;
                private static readonly Type type = typeof(SByteArray);
                private static Dictionary<Type, System.Reflection.MethodInfo> converters;
                static SByteArray()
                {
                    var methods = type
                        .GetMethods(BindingFlags.Public | BindingFlags.Static).ToArray();
                    var methodTypes = methods.Select(x => x.GetParameters().First().ParameterType);

                    converters = type
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Where(x => x.Name == nameof(ConvertFrom))
                        .ToDictionary(x => x.GetParameters()
                        .First().ParameterType, x => x);
                }

                public SByteArray(int length) : this(new sbyte[length]) { }
                public SByteArray(sbyte[] value) => data = (sbyte[])value.Clone();
                public sbyte this[int index] { get => data[index]; set => data[index] = value; }

                public static sbyte ConvertFrom(byte instance) => (sbyte)instance;
                public static sbyte ConvertFrom(ushort instance) => (sbyte)instance;
                public static sbyte ConvertFrom(uint instance) => (sbyte)instance;
                public static sbyte ConvertFrom(ulong instance) => (sbyte)instance;

                public static sbyte ConvertFrom(sbyte instance) => (sbyte)instance;
                public static sbyte ConvertFrom(short instance) => (sbyte)instance;
                public static sbyte ConvertFrom(int instance) => (sbyte)instance;
                public static sbyte ConvertFrom(long instance) => (sbyte)instance;

                public static sbyte ConvertFrom(float instance) => (sbyte)instance;
                public static sbyte ConvertFrom(double instance) => (sbyte)instance;
                public static sbyte ConvertFrom(decimal instance) => (sbyte)instance;
                public static sbyte ConvertFrom(string instance) => sbyte.Parse(instance);

                object IArray.this[int index]
                {
                    get
                    {
                        return data[index];
                    }
                    set
                    {

                        var valueType = value.GetType();
                        var t = Convert.ToSByte(value);// won't handle op_implicit, op_explicit. won't allow for unchecked.
                        var typeCode = Convert.GetTypeCode(value);
                        switch (typeCode)
                        {
                            case TypeCode.Int32:
                                data[index] = (sbyte)(int)value;
                                return;
                            case TypeCode.Byte:
                                data[index] = (sbyte)(byte)value;
                                return;
                            case TypeCode.SByte:
                                data[index] = (sbyte)value;
                                return;
                            case TypeCode.Int16:
                                data[index] = (sbyte)(short)value;
                                return;
                            case TypeCode.Int64:
                                data[index] = (sbyte)(long)value;
                                return;
                            case TypeCode.UInt16:
                                data[index] = (sbyte)(ushort)value;
                                return;
                            case TypeCode.UInt32:
                                data[index] = (sbyte)(uint)value;
                                return;
                            case TypeCode.UInt64:
                                data[index] = (sbyte)(ulong)value;
                                return;
                            case TypeCode.Single:
                                data[index] = (sbyte)(float)value;
                                return;
                            case TypeCode.Double:
                                data[index] = (sbyte)(double)value;
                                return;
                            case TypeCode.Decimal:
                                data[index] = (sbyte)(decimal)value;
                                return;
                            case TypeCode.Boolean:
                                bool val = (bool)value;
                                data[index] = (sbyte)(val ? 1 : 0);
                                return;
                            default: throw new NotImplementedException();


                        }
                    }
                }
                public void SetValue(object value, int index) => this[index] = (sbyte)value;
                public object GetValue(int index) => this[index];

            }
            public class DynamicArray
            {
                private Array array;
                private Type elementType;
                private System.Reflection.MethodInfo converter;
                public DynamicArray(int length, Type ElementType)
                {
                    this.elementType = ElementType;
                    this.array = Array.CreateInstance(ElementType, length);
                    this.converter = typeof(ObjectStackTests)
                   .GetMethod("DynamicCast")
                   .MakeGenericMethod(new[] { elementType });
                    //.MakeGenericMethod(new[] { ElementType });



                }
                public void Set(object element, int index) => array.SetValue(converter.Invoke(null, new[] { element }), index);
            }
            public static void Run()
            {
                var array = Array.CreateInstance(typeof(byte), 5);

                Stack<object> stack = new Stack<object>();
                stack.Push(1);
                stack.Push((byte)1);
                //stack.Push((short)257);
                var setMethod = array.GetType().GetMethod(nameof(array.SetValue), new[] { typeof(object), typeof(int) });


                stack.Push(1);
                stack.Push((short)1);

                setMethod.Invoke(array, new[] { Convert.ChangeType(stack.Pop(), typeof(byte)), 0 });
                dynamic dynarr = array;


                var dynArray = new DynamicArray(5, typeof(byte));

                var byteUnboxer = new Unboxer<byte>();
                var intUnboxer = new Unboxer<int>();

                var boxedByte = (object)(byte)1;
                var unboxedByte = byteUnboxer.Unbox(boxedByte);



                stack.Push(new SByteArray(5));



                var iArray = stack.Pop();
                var boxedOverFlow = stack.Pop();
                var boxedShort = stack.Pop();
                var boxedInt = stack.Pop();

                ((IArray)iArray)[0] = (byte)boxedByte;
                ((IArray)iArray)[1] = boxedOverFlow;
                ((IArray)iArray)[2] = boxedOverFlow;


                dynArray.Set(boxedOverFlow, 0);








                var dncast = DynamicCastObject(boxedOverFlow, typeof(byte));


                dynarr[0] = (dynamic)dncast;



                var overflowUnboxed = byteUnboxer.Unbox(boxedOverFlow);


                var boxedIntType = boxedInt.GetType();
                var unboxedInt = Convert.ChangeType(boxedInt, boxedIntType);

                var boxShortType = boxedShort.GetType();
                var unboxedShort = Convert.ChangeType(boxedShort, boxShortType);


                var arrayType = array.GetType();
                var arrayElementType = arrayType.GetElementType();
                array.SetValue(unboxedShort, 0);

                // Convert.ChangeType limitiations:
                //  1) Doesn't allow ignoring overflow, (byte)(int)(257)  while Convert.ChangeType((int)257, typeof(byte)); throws exception;
                //  2) Only works for IConvertible.
                var unboxedIntToTargetType = Convert.ChangeType(boxedInt, arrayElementType);
                var unboxedShortToTargetType = Convert.ChangeType(boxedShort, arrayElementType);


                var converter = TypeDescriptor.GetConverter(arrayElementType);
                unchecked
                {
                    var unboxedOverFlow = Convert.ChangeType(boxedOverFlow, arrayElementType);
                    array.SetValue(unboxedOverFlow, 2);
                    if (converter.CanConvertFrom(boxedIntType))
                    {
                        array.SetValue(converter.ConvertFrom(unboxedOverFlow), 0);
                    }

                }


                if (converter.CanConvertFrom(boxedIntType))
                    array.SetValue(converter.ConvertFrom(boxedInt), 0);

                array.SetValue(unboxedShortToTargetType, 1);
                //


            }
        }
    }
}
