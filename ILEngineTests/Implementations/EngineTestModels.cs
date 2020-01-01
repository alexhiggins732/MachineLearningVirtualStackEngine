using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.Tests
{
    public class SourceClass
    {
        public virtual string TypeName => nameof(SourceClass);
        public int Value;
        public SourceClass(int value) => this.Value = value;
        public static implicit operator DestClass(SourceClass source) => new DestClass(source.Value);
        public int ftnTest() => 1;
    }
    public class DerivedSource : SourceClass
    {
        public override string TypeName => nameof(DerivedSource);
        public DerivedSource(int value) : base(value)
        {
        }
    }

    public class DestClass : IEquatable<DestClass>
    {
        public int Value;
        public string TypeName = nameof(DestClass);
        public DestClass(int value) => this.Value = value;
        public DestClass() => this.Value = 0;
        public static bool operator ==(DestClass a, DestClass b) => a?.Value == b?.Value;
        public static bool operator !=(DestClass a, DestClass b) => a?.Value != b?.Value;

        public override bool Equals(object obj)
        {
            return Equals(obj as DestClass);
        }

        public bool Equals(DestClass other)
        {
            return other != null &&
                   Value == other.Value &&
                   TypeName == other.TypeName;
        }

        public override int GetHashCode()
        {
            var hashCode = -954234040;
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TypeName);
            return hashCode;
        }
    }
    public class ILEngineUnitTestModel : IEquatable<ILEngineUnitTestModel>
    {
        public int Value;
        public void SetValue(int value)
        {
            this.Value = value;
        }
        public static void SetValueStatic(int value) => InstanceField.SetValue(value);

        public ILEngineUnitTestModel() { }

        public ILEngineUnitTestModel(int value) { this.Value = value; }

        public int GetValue()
        {
            return Value;
        }
        public static int GetValueStatic() => InstanceField.GetValue();

        public static ILEngineUnitTestModel InstanceField = new ILEngineUnitTestModel();
        public static ILEngineUnitTestModel InstanceMethod() => new ILEngineUnitTestModel(1);
        public static ILEngineUnitTestModel InstanceMethodWithArg(int arg) => new ILEngineUnitTestModel(arg);

        public override bool Equals(object obj)
        {
            return Equals(obj as ILEngineUnitTestModel);
        }

        public bool Equals(ILEngineUnitTestModel other)
        {
            return other != null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return -1937169414 + Value.GetHashCode();
        }

        public static bool operator ==(ILEngineUnitTestModel a, ILEngineUnitTestModel b) => a?.Value == b?.Value;
        public static bool operator !=(ILEngineUnitTestModel a, ILEngineUnitTestModel b) => a?.Value != b?.Value;
    }

    public class FieldTest
    {
        public int Value;
        public static int StaticValue;
        public FieldTest() { }
        public FieldTest(int value)
        {
            this.Value = value;
        }
    }
  
    public class ComparisonTest : IEquatable<ComparisonTest>
    {
        public int Value { get; set; }
        public ComparisonTest(int value)
        {
            this.Value = value;
        }
        public static bool operator <(ComparisonTest a, ComparisonTest b)
            => a.Value < b.Value;
        public static bool operator >(ComparisonTest a, ComparisonTest b)
            => a.Value > b.Value;
        public static bool operator <=(ComparisonTest a, ComparisonTest b)
            => a.Value <= b.Value;
        public static bool operator >=(ComparisonTest a, ComparisonTest b)
            => a.Value >= b.Value;
        public static bool operator ==(ComparisonTest a, ComparisonTest b)
            => a.Value == b.Value;
        public static bool operator !=(ComparisonTest a, ComparisonTest b)
            => a.Value != b.Value;

        public override bool Equals(object obj)
        {
            return Equals(obj as ComparisonTest);
        }

        public bool Equals(ComparisonTest other)
        {
            return other != null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return -1937169414 + Value.GetHashCode();
        }
    }

    public class ComparisonTestUn : IEquatable<ComparisonTestUn>
    {
        public uint Value { get; set; }
        public ComparisonTestUn(uint value)
        {
            this.Value = value;
        }
        public static bool operator <(ComparisonTestUn a, ComparisonTestUn b) => a.Value < b.Value;
        public static bool operator >(ComparisonTestUn a, ComparisonTestUn b) => a.Value > b.Value;
        public static bool operator <=(ComparisonTestUn a, ComparisonTestUn b) => a.Value <= b.Value;
        public static bool operator >=(ComparisonTestUn a, ComparisonTestUn b) => a.Value >= b.Value;
        public static bool operator ==(ComparisonTestUn a, ComparisonTestUn b) => a.Value == b.Value;
        public static bool operator !=(ComparisonTestUn a, ComparisonTestUn b) => a.Value != b.Value;

        public override bool Equals(object obj)
        {
            return Equals(obj as ComparisonTestUn);
        }

        public bool Equals(ComparisonTestUn other)
        {
            return other != null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return -1937169414 + Value.GetHashCode();
        }
    }

}
