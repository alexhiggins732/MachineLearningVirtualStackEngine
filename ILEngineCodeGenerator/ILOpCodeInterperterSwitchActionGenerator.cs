using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.CodeGenerator
{
    public class ILOpcodeInterperterSwitchActionGenerator : ILOpcodeActionGeneratorBase
    {
        private StringBuilder sb;

        public static void GenerateOpCodeJmpTable()
        {

           
            var valueType = typeof(ILOpCodeValues);

           


            //var opCodeDict = Enum.GetNames(valueType).ToDictionary(x => x, x=> (int)Enum.Parse(valueType, x));
            //var opCodesByValue = opCodeDict.ToDictionary(x => x.Value, x => x.Key);

            //var values = opCodeDict.ToList();

            //var lines = values.Select(kvp => $"case {kvp.Value}: goto {kvp.Key}; // {kvp.Value.ToString("x2")}").ToList();

            var sbSwitch = new StringBuilder();
            sbSwitch.AppendLine("\t\tswitch (switch (op))");
            sbSwitch.AppendLine("\t\t{");
            var sbJump = new StringBuilder();
            
            for( var i=0; i< 256;i++)
            {
                var opCodeLookup = Enum.GetName(valueType, i);
                if (!string.IsNullOrEmpty(opCodeLookup))
                {
                    var opCode = (ILOpCodeValues)i;
                    var line = $"case {i}: goto {opCode}; // 0x{i.ToString("x2")}";
          
                    sbSwitch.AppendLine($"\t\t\t{line}");
                    sbJump.AppendLine($"{opCode}: goto Read;");
                }
                else
                {
                    sbSwitch.AppendLine($"\t\t\tcase {i}: goto NotSupported_S; //0x{i.ToString("X2")}");
                } 
            }
            sbSwitch.AppendLine("\t\t}");
            sbSwitch.AppendLine();
            var allCode = sbSwitch.ToString() + sbJump.ToString();

        }
        public ILOpcodeInterperterSwitchActionGenerator()
        {
            sb = new System.Text.StringBuilder();


        }

        private void NextInstruction()
        {
            sb.AppendLine("\t\t\tgoto FLOWCONTROL;");
        }
        private void ExecuteInstruction()
        {
            sb.AppendLine("\t\t\tgoto JMPTABLE;");
        }

        public void HandleOpcode(int opcode) => opcodeHandlers[opcode]();

        public string GeneratorSwitchStatement()
        {
            sb = new System.Text.StringBuilder();
            GeneratorFlowControlStatement();
            var fc = sb.ToString();
            sb = new StringBuilder();
            sb.AppendLine("\tvar opcode = 0;");
            sb.AppendLine("\tvar pos = -1;");

            sb.AppendLine("\tgoto FLOWCONTROL;");
            sb.AppendLine();

            
            sb.AppendLine("\tJMPTABLE:");
            sb.AppendLine();

            sb.AppendLine("\topcode = instructions[pos]");
            sb.AppendLine("\tHandleOpcode(opcode);");
            sb.AppendLine("\t//TODO: Find efficient way to eliminate this switch");
            sb.AppendLine("\t// 1) Set flow contol field in the handler");
            sb.AppendLine("\t// 2) Dictionary? but already not liking the dictionary lookup and method call for every instruction (for simplicity it works)");
            sb.AppendLine("\t// 3) Alternately, go in reverse - remove the handle call  by inlining the instructions and flow control directly in the MSIL jump table");
            sb.AppendLine();

            sb.AppendLine("\tswitch(opcode)");
            sb.AppendLine("\t{");
            GenerateAll();
            sb.AppendLine("\t\tdefault: throw new NotImplementedException();");
            sb.AppendLine("\t}");
            sb.AppendLine();
            sb.AppendLine(fc);
            sb.AppendLine();
            return sb.ToString();
        }

        public string GeneratorFlowControlStatement()
        {
            sb = new StringBuilder();

            sb.AppendLine("\ttargetpos = pos+1");
            sb.AppendLine("\t//branchcondition = true; refactoring has eliminated this need");
            sb.AppendLine("\tfor simplicity this works. Could be better to do jump to conditional/branch directly from those methods ");
            sb.AppendLine("\tswitch (flowcontrol)");
            sb.AppendLine("\t{");
            var groups = new[] { 1, 7, 3, 0, 4 };
            for (var i = 0; i < groups.Length; i++)
            {
                switch (groups[i])
                {
                    case 5: //Next = 5,
                    case 2: //Call = 2
                    case 8: //Throw =8
                    case 1: //Break = 1

                        sb.AppendLine("\t\tcase 5: //Next = 5");
                        sb.AppendLine("\t\tcase 2: //Call = 2 (if we are here the breakpoint already executed)");
                        sb.AppendLine("\t\tcase 8: ///Throw = 8 (if we are here exception has already been thrown");
                        sb.AppendLine("\t\tcase 1: //Break = 1 (if we are here the call already executed)");
                        sb.AppendLine("\t\t\tgoto SETPOS;");
                        NextInstruction();
                        break;
                    case 7: //Return = 7,
                        sb.AppendLine("\t\tcase 7: //Return = 7");
                        sb.AppendLine("\t\t\treturn; //targetpos = [jmpaddress] verify not need for tail call");
                        break;
                    case 3: //Cond_Branch = 3,
                        sb.AppendLine("\t\tcase 3: //Cond_Branch = 3");
                        sb.AppendLine("\t\t\tif(stack.Pop()) targetpos = [jmpaddress];");
                        sb.AppendLine("\t\t\tgoto SETPOS;");
                        break;
                    case 0: // Branch = 0
                        sb.AppendLine("\t\tcase 0: //Branch = 0");
                        sb.AppendLine("\t\t\ttargetpos = [jmpaddress]");
                        sb.AppendLine("\t\t\tgoto SETPOS;");
                        break;

                    case 4: //Meta = 4,
                    case 6: //Phi = 6,
                    default:
                        sb.AppendLine("//todo is it safe to just swallow these? If so we only need to set a custom jump flowcontrol for 0,3 and 7");
                        sb.AppendLine("\t\tcase 4: //Meta = 4");
                        sb.AppendLine("\t\tcase 6: //PHI = 6");
                        sb.AppendLine("\t\tcase default");
                        sb.AppendLine("\t\t\tthrow new NotImplementedException();");
                        break;

                }
            }
            sb.AppendLine("\t}");
            sb.AppendLine("SETPOS:");
            sb.AppendLine("\tpos = targetpos;");
            sb.AppendLine("\tflowcontrol = 5; //reset flow control");
            sb.AppendLine("\tgoto JMPTABLE;");

            return sb.ToString();

        }
        private void CaseStatement(OpCodeValueAttribute info)
        {
            sb.AppendLine($"\t\tcase {info.OpcodeValue}:\r\n\t\t\t//{info.OpCode}");
            sb.AppendLine($"\t\t\tflowcontrol = {(int)info.OpCode.FlowControl}");
        }

        public override void GenerateAdd()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateAdd_Ovf()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateAdd_Ovf_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateAnd()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateArglist()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBeq()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBeq_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBge()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBge_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBge_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBge_Un_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBgt()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBgt_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBgt_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBgt_Un_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBle()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBle_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBle_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBle_Un_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBlt()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBlt_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBlt_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBlt_Un_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBne_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBne_Un_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBox()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBr()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBreak()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBrfalse()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBrfalse_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBrtrue()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBrtrue_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateBr_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateCall()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateCalli()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateCallvirt()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateCastclass()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateCeq()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateCgt()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateCgt_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateCkfinite()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateClt()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateClt_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConstrained_()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_I()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_I1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_I2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_I4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_I8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_I()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_I1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_I1_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_I2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_I2_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_I4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_I4_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_I8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_I8_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_I_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_U()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_U1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_U1_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_U2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_U2_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_U4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_U4_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_U8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_U8_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_Ovf_U_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_R4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_R8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_R_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_U()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_U1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_U2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_U4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateConv_U8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateCpblk()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateCpobj()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateDiv()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateDiv_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateDup()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateEndfilter()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateEndfinally()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateExec_MSIL_I()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateExec_MSIL_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateInitblk()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateInitobj()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateIsinst()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateJmp()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdarg()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdarga()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdarga_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdarg_0()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdarg_1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdarg_2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdarg_3()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdarg_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I4_0()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I4_1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I4_2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I4_3()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I4_4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I4_5()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I4_6()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I4_7()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I4_8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I4_M1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I4_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_I8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_R4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdc_R8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelem()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelema()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelem_I()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelem_I1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelem_I2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelem_I4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelem_I8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelem_R4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelem_R8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelem_Ref()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelem_U1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelem_U2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdelem_U4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdfld()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdflda()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdftn()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdind_I()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdind_I1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdind_I2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdind_I4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdind_I8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdind_R4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdind_R8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdind_Ref()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdind_U1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdind_U2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdind_U4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdlen()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdloc()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdloca()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdloca_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdloc_0()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdloc_1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdloc_2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdloc_3()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdloc_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdnull()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdobj()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdsfld()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdsflda()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdstr()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdtoken()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLdvirtftn()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLeave()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLeave_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateLocalloc()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateMkrefany()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateMul()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateMul_Ovf()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateMul_Ovf_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateNeg()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateNewarr()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateNewobj()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateNop()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();

        }

        public override void GenerateNot()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateOr()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GeneratePop()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GeneratePrefix1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GeneratePrefix2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GeneratePrefix3()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GeneratePrefix4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GeneratePrefix5()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GeneratePrefix6()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GeneratePrefix7()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GeneratePrefixref()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateReadonly_()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateRefanytype()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateRefanyval()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateRem()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateRem_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateRet()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateRethrow()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateShl()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateShr()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateShr_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateSizeof()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStarg()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStarg_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStelem()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStelem_I()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStelem_I1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStelem_I2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStelem_I4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStelem_I8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStelem_R4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStelem_R8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStelem_Ref()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStfld()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStind_I()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStind_I1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStind_I2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStind_I4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStind_I8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStind_R4()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStind_R8()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStind_Ref()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStloc()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStloc_0()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStloc_1()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStloc_2()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStloc_3()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStloc_S()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStobj()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateStsfld()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateSub()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateSub_Ovf()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateSub_Ovf_Un()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateSwitch()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateTail_()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateThrow()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateUnaligned_()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateUnbox()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateUnbox_Any()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateVolatile_()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }

        public override void GenerateXor()
        {
            CaseStatement(GetOpcodeInfo());
            NextInstruction();
        }
    }
}
