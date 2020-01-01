namespace ILVmModel.Models
{
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
}
