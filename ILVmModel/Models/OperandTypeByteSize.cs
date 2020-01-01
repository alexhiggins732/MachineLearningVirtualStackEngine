namespace ILVmModel.Models
{
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
}
