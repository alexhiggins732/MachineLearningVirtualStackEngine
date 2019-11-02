using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IlVmModel.Models
{
    public class VmMethod
    {
        public string MethodName { get; set; }
        public VariableCollection Arguments { get; set; }
        public VariableCollection Locals { get; set; }
        public List<VmInstruction> Instructions { get; set; }
        public VmMethod(string name)
        {

        }
    }
    public class VmInstruction
    {
        public int InstructionIndex { get; private set; }
        public int ByteIndex { get; private set; }
        public short OpCode { get; private set; }
        public List<VmInstructionArgument> Arguments { get; private set; }// should this be argum
    }

    public abstract class VmInstructionArgument
    {
        public abstract int DataByteSize { get; protected set; }
        public abstract int ArgumentIndex { get; protected set; }
    }
    public class VmInstructionArgument<T> : VmInstructionArgument
    {
        public T Value { get; private set; }
        public override int ArgumentIndex { get; protected set; }
        public override int DataByteSize { get; protected set; }

        public VmInstructionArgument(T value, int index)
        {
            this.Value = value;
            this.ArgumentIndex = index;

        }
    }

    //
    // Summary:
    //     Describes the operand type of Microsoft intermediate language (MSIL) instruction.

    public enum OperandTypeBitSize
    {
        //
        // Summary:
        //     The operand is a 32-bit integer branch target.
        InlineBrTarget = 32,
        //
        // Summary:
        //     The operand is a 32-bit metadata token.
        InlineField = 32,
        //
        // Summary:
        //     The operand is a 32-bit integer.
        InlineI = 32,
        //
        // Summary:
        //     The operand is a 64-bit integer.
        InlineI8 = 64,
        //
        // Summary:
        //     The operand is a 32-bit metadata token.
        InlineMethod = 32,
        //
        // Summary:
        //     No operand.
        InlineNone = 0,
        //
        // Summary:
        //     The operand is reserved and should not be used.
        InlinePhi = 0,
        //
        // Summary:
        //     The operand is a 64-bit IEEE floating point number.
        InlineR = 64,
        //
        // Summary:
        //     The operand is a 32-bit metadata signature token.
        InlineSig = 32,
        //
        // Summary:
        //     The operand is a 32-bit metadata string token.
        InlineString = 32,
        //
        // Summary:
        //     The operand is the 32-bit integer argument to a switch instruction.
        InlineSwitch = 32,
        //
        // Summary:
        //     The operand is a FieldRef, MethodRef, or TypeRef token.
        InlineTok = 32,
        //
        // Summary:
        //     The operand is a 32-bit metadata token.
        InlineType = 32,
        //
        // Summary:
        //     The operand is 16-bit integer containing the ordinal of a local variable or an
        //     argument.
        InlineVar = 16,
        //
        // Summary:
        //     The operand is an 8-bit integer branch target.
        ShortInlineBrTarget = 8,
        //
        // Summary:
        //     The operand is an 8-bit integer.
        ShortInlineI = 8,
        //
        // Summary:
        //     The operand is a 32-bit IEEE floating point number.
        ShortInlineR = 32,
        //
        // Summary:
        //     The operand is an 8-bit integer containing the ordinal of a local variable or
        //     an argumenta.
        ShortInlineVar = 8
    }


    public enum OperandTypeByteSize
    {
        //
        // Summary:
        //     The operand is a 32-bit integer branch target.
        InlineBrTarget = 4,
        //
        // Summary:
        //     The operand is a 32-bit metadata token.
        InlineField = 4,
        //
        // Summary:
        //     The operand is a 32-bit integer.
        InlineI = 4,
        //
        // Summary:
        //     The operand is a 64-bit integer.
        InlineI8 = 8,
        //
        // Summary:
        //     The operand is a 32-bit metadata token.
        InlineMethod = 4,
        //
        // Summary:
        //     No operand.
        InlineNone = 0,
        //
        // Summary:
        //     The operand is reserved and should not be used.
        InlinePhi = 0,
        //
        // Summary:
        //     The operand is a 64-bit IEEE floating point number.
        InlineR = 8,
        //
        // Summary:
        //     The operand is a 32-bit metadata signature token.
        InlineSig = 4,
        //
        // Summary:
        //     The operand is a 32-bit metadata string token.
        InlineString = 4,
        //
        // Summary:
        //     The operand is the 32-bit integer argument to a switch instruction.
        InlineSwitch = 4,
        //
        // Summary:
        //     The operand is a FieldRef, MethodRef, or TypeRef token.
        InlineTok = 4,
        //
        // Summary:
        //     The operand is a 32-bit metadata token.
        InlineType = 4,
        //
        // Summary:
        //     The operand is 16-bit integer containing the ordinal of a local variable or an
        //     argument.
        InlineVar = 2,
        //
        // Summary:
        //     The operand is an 8-bit integer branch target.
        ShortInlineBrTarget = 1,
        //
        // Summary:
        //     The operand is an 8-bit integer.
        ShortInlineI = 1,
        //
        // Summary:
        //     The operand is a 32-bit IEEE floating point number.
        ShortInlineR = 4,
        //
        // Summary:
        //     The operand is an 8-bit integer containing the ordinal of a local variable or
        //     an argumenta.
        ShortInlineVar = 1,
    }

    public class VariableCollection //: Dictionary<string, dynamic>
    {
        Dictionary<string, dynamic> variables;
        public VariableCollection()
        {
            variables = new Dictionary<string, dynamic>();
        }
        public void Add(string argumentName, dynamic value)
        {
            this.variables.Add(argumentName, value);
        }
        public void Add(dynamic value)
        {
            this.variables.Add($"arg_{variables.Count}", value);
        }
        public dynamic[] ValuesArray => variables.Values.ToArray();
        public string[] KeysArray => variables.Keys.ToArray();
        public KeyValuePair<string, dynamic>[] Items => variables.ToArray();
        public int Count => variables.Count;
    }
}
