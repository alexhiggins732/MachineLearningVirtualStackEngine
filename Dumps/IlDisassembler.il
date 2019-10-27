
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.7.3081.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ILDisassembler
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 0E 49 4C 44 69 73 61 73 73 65 6D 62 6C 65   // ...ILDisassemble
                                                                                              72 00 00 )                                        // r..
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 0E 49 4C 44 69 73 61 73 73 65 6D 62 6C 65   // ...ILDisassemble
                                                                                                72 00 00 )                                        // r..
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 12 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. 
                                                                                                  20 32 30 31 39 00 00 )                            //  2019..
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 66 36 38 65 34 66 64 31 2D 66 32 37 62   // ..$f68e4fd1-f27b
                                                                                                  2D 34 36 62 65 2D 61 36 34 33 2D 36 61 33 37 61   // -46be-a643-6a37a
                                                                                                  62 66 38 63 62 62 64 00 00 )                      // bf8cbbd..
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 1C 2E 4E 45 54 46 72 61 6D 65 77 6F 72 6B   // ....NETFramework
                                                                                                        2C 56 65 72 73 69 6F 6E 3D 76 34 2E 37 2E 32 01   // ,Version=v4.7.2.
                                                                                                        00 54 0E 14 46 72 61 6D 65 77 6F 72 6B 44 69 73   // .T..FrameworkDis
                                                                                                        70 6C 61 79 4E 61 6D 65 14 2E 4E 45 54 20 46 72   // playName..NET Fr
                                                                                                        61 6D 65 77 6F 72 6B 20 34 2E 37 2E 32 )          // amework 4.7.2
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module ILDisassembler.exe
// MVID: {0136C505-B9AB-4CF6-BB8E-273796973DDB}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00020003    //  ILONLY 32BITPREFERRED
// Image base: 0x0C370000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ILDisassembler.Program
       extends [mscorlib]System.Object
{
  .method private hidebysig static void  Main(string[] args) cil managed
  {
    .entrypoint
    // Code size       114 (0x72)
    .maxstack  2
    .locals init ([0] int32 i,
             [1] bool V_1,
             [2] int32 k,
             [3] bool V_3,
             [4] int32 V_4,
             [5] int32 V_5,
             [6] bool V_6,
             [7] bool V_7)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0012

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  call       void ILDisassembler.Program::AddIt(int32)
    IL_000c:  nop
    IL_000d:  nop
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.1
    IL_0010:  add
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.s   10
    IL_0015:  clt
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  brtrue.s   IL_0005

    IL_001b:  ldc.i4.0
    IL_001c:  stloc.2
    IL_001d:  br.s       IL_002c

    IL_001f:  nop
    IL_0020:  ldloc.2
    IL_0021:  call       void ILDisassembler.Program::AddIt(int32)
    IL_0026:  nop
    IL_0027:  nop
    IL_0028:  ldloc.2
    IL_0029:  ldc.i4.1
    IL_002a:  add
    IL_002b:  stloc.2
    IL_002c:  ldloc.2
    IL_002d:  ldc.i4.s   10
    IL_002f:  clt
    IL_0031:  stloc.3
    IL_0032:  ldloc.3
    IL_0033:  brtrue.s   IL_001f

    IL_0035:  ldc.i4.0
    IL_0036:  stloc.s    V_4
    IL_0038:  br.s       IL_0065

    IL_003a:  nop
    IL_003b:  ldc.i4.0
    IL_003c:  stloc.s    V_5
    IL_003e:  br.s       IL_0052

    IL_0040:  nop
    IL_0041:  ldloc.s    V_4
    IL_0043:  ldloc.s    V_5
    IL_0045:  call       void ILDisassembler.Program::AddIt(int32,
                                                            int32)
    IL_004a:  nop
    IL_004b:  nop
    IL_004c:  ldloc.s    V_5
    IL_004e:  ldc.i4.1
    IL_004f:  add
    IL_0050:  stloc.s    V_5
    IL_0052:  ldloc.s    V_5
    IL_0054:  ldc.i4.s   10
    IL_0056:  clt
    IL_0058:  stloc.s    V_6
    IL_005a:  ldloc.s    V_6
    IL_005c:  brtrue.s   IL_0040

    IL_005e:  nop
    IL_005f:  ldloc.s    V_4
    IL_0061:  ldc.i4.1
    IL_0062:  add
    IL_0063:  stloc.s    V_4
    IL_0065:  ldloc.s    V_4
    IL_0067:  ldc.i4.s   10
    IL_0069:  clt
    IL_006b:  stloc.s    V_7
    IL_006d:  ldloc.s    V_7
    IL_006f:  brtrue.s   IL_003a

    IL_0071:  ret
  } // end of method Program::Main

  .method private hidebysig static void  AddIt(int32 i,
                                               int32 k) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  2
    .locals init ([0] int32 j)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  add
    IL_0004:  stloc.0
    IL_0005:  ret
  } // end of method Program::AddIt

  .method private hidebysig static void  AddIt(int32 i) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  2
    .locals init ([0] int32 k)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  add
    IL_0004:  stloc.0
    IL_0005:  ret
  } // end of method Program::AddIt

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method Program::.ctor

} // end of class ILDisassembler.Program


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\Users\alexander.higgins\source\repos\ILDisassembler\Dumps\IlDisassembler.res
