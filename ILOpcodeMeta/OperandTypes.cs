using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILOpcodeMeta
{
    public class OperandTypes
    {
        public static OperandType InlineBrTarget = new OperandType(1, "InlineBrTarget", 0, "The operand is a 32-bit integer branch target.", 4, 32, false, "System.Int32");
        public static OperandType InlineField = new OperandType(2, "InlineField", 1, "The operand is a 32-bit metadata token.", 4, 32, false, "System.Int32");
        public static OperandType InlineI = new OperandType(3, "InlineI", 2, "The operand is a 32-bit integer.", 4, 32, false, "System.Int32");
        public static OperandType InlineI8 = new OperandType(4, "InlineI8", 3, "The operand is a 64-bit integer.", 8, 64, false, "System.Int64");
        public static OperandType InlineMethod = new OperandType(5, "InlineMethod", 4, "The operand is a 32-bit metadata token.", 4, 32, false, "System.Int32");
        public static OperandType InlineNone = new OperandType(6, "InlineNone", 5, "No operand.", 0, 0, false, "System.Object");
        public static OperandType InlinePhi = new OperandType(7, "InlinePhi", 6, "The operand is reserved and should not be used.", 0, 0, false, "System.Object");
        public static OperandType InlineR = new OperandType(8, "InlineR", 7, "The operand is a 64-bit IEEE floating point number.", 8, 64, true, "System.Double");
        public static OperandType InlineSig = new OperandType(9, "InlineSig", 9, "The operand is a 32-bit metadata signature token.", 4, 32, false, "System.Int32");
        public static OperandType InlineString = new OperandType(10, "InlineString", 10, "The operand is a 32-bit metadata string token.", 4, 32, false, "System.Int32");
        public static OperandType InlineSwitch = new OperandType(11, "InlineSwitch", 11, "The operand is the 32-bit integer argument to a switch instruction.", 4, 32, false, "System.Int32");
        public static OperandType InlineTok = new OperandType(12, "InlineTok", 12, "The operand is a FieldRef, MethodRef, or TypeRef token.", 4, 32, false, "System.Int32");
        public static OperandType InlineType = new OperandType(13, "InlineType", 13, "The operand is a 32-bit metadata token.", 4, 32, false, "System.Int32");
        public static OperandType InlineVar = new OperandType(14, "InlineVar", 14, "The operand is 16-bit integer containing the ordinal of a local variable or an argument.", 2, 16, false, "System.Int16");
        public static OperandType ShortInlineBrTarget = new OperandType(15, "ShortInlineBrTarget", 15, "The operand is an 8-bit integer branch target.", 1, 8, false, "System.Byte");
        public static OperandType ShortInlineI = new OperandType(16, "ShortInlineI", 16, "The operand is an 8-bit integer.", 1, 8, false, "System.Byte");
        public static OperandType ShortInlineR = new OperandType(17, "ShortInlineR", 17, "The operand is a 32-bit IEEE floating point number.", 4, 32, true, "System.Single");
        public static OperandType ShortInlineVar = new OperandType(18, "ShortInlineVar", 18, "The operand is an 8-bit integer containing the ordinal of a local variable or an argumenta.", 1, 8, false, "System.Byte");

        public static OperandType[] AllOperandTypes => new[]
        {
            InlineBrTarget,
            InlineField,
            InlineI,
            InlineI8,
            InlineMethod,
            InlineNone,
            InlinePhi,
            InlineR,
            InlineSig,
            InlineString,
            InlineSwitch,
            InlineTok,
            InlineType,
            InlineVar,
            ShortInlineBrTarget,
            ShortInlineI,
            ShortInlineR,
            ShortInlineVar,
        };
    }
}
