using ILEngine.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace ILEngine
{
    public class ILStringReader
    {

        internal static List<ILInstruction> ReadMethodBody(string ilString)
        {
            var builder = new ILInstructionBuilder();

            var reader = new StringReader(ilString);
            string line = null;
            int idx = -1;
            char addressDelimiter = ':';
            string temp;
            var opCodeArguments = new List<string>();
            var meta = OpCodeMetaModel.OpCodeMetaNativeNameDict;
            var opCodeDict = OpCodeLookup.OpCodesByName;
            ILInstruction instruction;
            //IL_0000: ldarg.0     
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    idx = line.IndexOf(addressDelimiter);
                    if (idx == -1)
                    {
                        throw new ArgumentException($"IL instruction does not begin with an address. {line}");
                    }
                    var ilAddress = line.Substring(0, idx);
                    var ilOffset = Convert.ToInt32(ilAddress.Substring(3), 16);
                    temp = line.Substring(idx + 2);

                    idx = temp.IndexOf(' ');
                    var opCodeName = idx == -1 ? temp : temp.Substring(0, idx);
                    //read arguments.

                    if (!meta.ContainsKey(opCodeName))
                    {
                        throw new ArgumentException($"Invalid OpCode: 'opCodeName'");
                    }
                    var metaModel = meta[opCodeName];
                    var opCode = opCodeDict[metaModel.ClrName];

                    if (metaModel.OperandTypeByteSize == 0)
                    {
                        instruction = ILInstruction.Create(opCode);
                    }
                    else
                    {
                        if (opCodeName.Length >= temp.Length)
                        {
                            throw new ArgumentException($"Instrunction {opCode} missing required argument: {line}");
                        }
                        temp = temp.Substring(opCodeName.Length).Trim();
                        object opCodeArg = null;

                        opCodeArguments.Clear();
                        while (temp.Length > 0 && (idx = temp.IndexOf(' ')) < temp.Length)
                        {
                            opCodeArguments.Add(idx == -1 ? temp : temp.Substring(0, idx));
                            temp = idx == -1 ? "" : temp.Substring(idx+1);
                            //TODO: handle embeded string comment.
                        }

                        opCodeArg = opCodeArguments.Count == 1 ? (object)opCodeArguments[0] : (object)opCodeArguments.ToArray();
                        //TODO: Parse args
                        instruction = ILInstruction.Create(opCode, opCodeArg);
                    }
                   
                    instruction.ByteIndex = ilOffset;
                    if (!instruction.ToString().StartsWith(ilAddress))
                    {
                        throw new ArgumentException($"Error parsing instruction addresses. Expected: {ilAddress}. {instruction}");
                    }
                    builder.Write(instruction);
                    //TODO: Valid ilAddress
                    
                }

            }


            return builder.Instructions;
        }
    }
}