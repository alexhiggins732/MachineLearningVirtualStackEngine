namespace ILEngine
{
    /// <summary>
    /// Masks used to generate flags used by the internal <see cref="System.Reflection.Emit.OpCode"/> constructor
    /// as defined in https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/reflection/emit/opcode.cs
    /// </summary>
    ///  Flag layout:
    ///     OperandType (0-4):          000000000000000000000000000XXXXX
    ///     FlowControl (5-8):          00000000000000000000000XXXX00000
    ///     OpCodeType (9-11) :         00000000000000000000XXX000000000
    ///     StackBehaviourPop (12-16):  000000000000000XXXXX000000000000
    ///     StackBehaviourPush (17-21): 0000000000XXXXX00000000000000000
    ///     Size (22-23):               00000000XX0000000000000000000000
    ///     EndsUncondJmpBlkFlag (24):  0000000X000000000000000000000000 (internal only)
    ///     Unused (25-27):             0000XXX0000000000000000000000000 (internal only)  
    ///     StackChangeShift (28-31):   XXXX0000000000000000000000000000
    ///     
    /// <remarks>
    /// </remarks>
    public class OpCodeConstructorFlags
    {
        /// <summary>
        /// Mask for setting <see cref="System.Reflection.Emit.OpCode.OperandType"/> bits 000000000000000000000000000XXXXX
        /// </summary>
        public const int OperandTypeMask = 0x1F;

        /// <summary>
        /// Shift size for the <see cref="System.Reflection.Emit.OpCode.FlowControl"/> bits // 00000000000000000000000XXXX00000
        /// </summary>
        public const int FlowControlShift = 5;
        /// <summary>
        /// Mask for setting <see cref="System.Reflection.Emit.OpCode.FlowControl"/> bits // 00000000000000000000000XXXX00000
        /// </summary>
        public const int FlowControlMask = 0x0F;

        /// <summary>
        /// Shift size for the <see cref="System.Reflection.Emit.OpCode.OpCodeType"/> bits  // 00000000000000000000XXX000000000
        /// </summary>
        public const int OpCodeTypeShift = 9;

        /// <summary>
        /// Mask for setting <see cref="System.Reflection.Emit.OpCode.OpCodeType"/> bits // 00000000000000000000XXX000000000
        /// </summary>

        public const int OpCodeTypeMask = 0x07;

        /// <summary>
        /// Shift size for the <see cref="System.Reflection.Emit.OpCode.StackBehaviourPop"/> bits // 000000000000000XXXXX000000000000
        /// </summary>
        public const int StackBehaviourPopShift = 12;

        /// <summary>
        /// Shift size for the <see cref="System.Reflection.Emit.OpCode.StackBehaviourPush"/> bits // 0000000000XXXXX00000000000000000
        /// </summary>
        public const int StackBehaviourPushShift = 17;

        /// <summary>
        /// Mask for setting the <see cref="System.Reflection.Emit.OpCode.StackBehaviourPop"/> and <see cref="System.Reflection.Emit.OpCode.StackBehaviourPush"/> bits// ‭0000000000XXXXXXXXXX000000000000
        /// </summary>
        public const int StackBehaviourMask = 0x1F;

        /// <summary>
        /// Shift size for the <see cref="System.Reflection.Emit.OpCode.Size"/> bits   // 00000000XX0000000000000000000000
        /// </summary>
        public const int SizeShift = 22;

        /// <summary>
        /// Mask for setting the <see cref="System.Reflection.Emit.OpCode.Size"/> bits // 00000000XX0000000000000000000000
        /// </summary>
        public const int SizeMask = 0x03;

        /// <summary>
        /// Shift size for the private EndsUncondJmpBlkFlag bits   // 0000000X000000000000000000000000
        /// </summary>
        public const int EndsUncondJmpBlk = 24;
        /// <summary>
        /// Mask for setting the private EndsUncondJmpBlkFlag // 0000000X000000000000000000000000
        /// </summary>
        public const int EndsUncondJmpBlkMask = 0x1;   // 0000000X000000000000000000000000

        /// <summary>
        /// Mask for setting the private EndsUncondJmpBlkFlag // 0000000X000000000000000000000000
        /// </summary>
        public const int EndsUncondJmpBlkFlag = 0x01000000;   // 0000000X000000000000000000000000

        /// <summary>
        /// Shift size for setting unused flag bits //0000XXX0000000000000000000000000
        /// </summary>
        public const int UnusedShift = 25;

        /// <summary>
        /// Mask for setting setting unused flag //0000XXX0000000000000000000000000
        /// </summary>
        public const int UnusedMask = 0x07;   


        /// <summary>
        /// Shift size for the private StackChange bits   // 00000000XX0000000000000000000000
        /// </summary>
        public const int StackChangeShift = 28;           // XXXX0000000000000000000000000000

        /// <summary>
        /// Mask for setting setting the private StackChange bits // XXXX0000000000000000000000000000
        /// </summary>
        public const int StackChangeMask = 0x0F;

    }
}
