using ILEngine.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ILEngine.CodeGenerator
{
    public class OpCodeBodyParser
    {
        public static Dictionary<short, string> ParseBodyFromIlEngineWithDiagnostics()
        {
            var engineSourceFilePath = Path.GetFullPath("../../../IlEngine/Implementations/IlEngineWithDiagnostics.cs");

            return ParseBodyFromFile(engineSourceFilePath);
        }
        public static Dictionary<short, string> ParseBodyFromFile(string path)
        {
            Dictionary<short, string> result = new Dictionary<short, string>();

            var fullText = File.ReadAllText(path);
            string delim = "switch (opCodeValue)";
            var idx = fullText.IndexOf(delim);
            var src = fullText.Substring(idx);
            delim = "Inc:";
            idx = src.IndexOf(delim);
            src = src.Substring(0, idx);
            var models = OpCodeMetaModel.OpCodeMetas;
            int endIdx = -1;

            foreach (var model in models)
            {
                delim = $"ILOpCodeValues.{model.ClrName}:";
                idx = src.IndexOf(delim);
                if (idx == -1)
                {
                    delim = $"ILOpCodeValues.{model.ClrName}):";
                    idx = src.IndexOf(delim);
                }
                if (idx == -1)
                {
                    result.Add((short)model.OpCodeValue, $"// Missing: {model.ClrName}\r\nthrow new {nameof(OpCodeNotImplementedException)}({model.OpCodeValue});");
                }
                else
                {
                    var tmp = src.Substring(idx).TrimStart();
                    endIdx = tmp.IndexOf("                case ");
                    if (endIdx == -1) endIdx = tmp.IndexOf("default:");
                    var def = tmp.Substring(0, endIdx).Trim();

                    var defBody = def.Substring(delim.Length).Trim().Trim("break;".ToCharArray()).Trim();

                    if (string.IsNullOrEmpty(defBody))
                    {
                        if (model.OpCodeValue > 0)
                        {
                            if (def == delim)
                            {
                                idx = idx + delim.Length;
                                tmp = src.Substring(idx).TrimStart();
                                var fallThrough = tmp.Substring(0, tmp.IndexOf(":"));
                                var fallThroughMethod = fallThrough.Substring(fallThrough.IndexOf(".") + 1);
                                if (models.Any(x => x.ClrName == fallThroughMethod))
                                {
                                    result.Add((short)model.OpCodeValue, $"//redirect {model.ClrName} => {fallThroughMethod}()\r\n{fallThroughMethod}();");
                                }
                                else
                                {
                                    result.Add((short)model.OpCodeValue, $"//Missing: {model.ClrName}\r\nthrow new {nameof(OpCodeNotImplementedException)}({model.OpCodeValue});");
                                }

                            }
                            else
                            {
                                result.Add((short)model.OpCodeValue, $"//Missing: {model.ClrName}\r\nthrow new {nameof(OpCodeNotImplementedException)}({model.OpCodeValue});");
                            }
                        }
                        else
                        {
                            result.Add((short)model.OpCodeValue, $"//handle {model.ClrName}\r\n{defBody}");
                        }
                    }
                    else
                    {
                        result.Add((short)model.OpCodeValue, $"//handle {model.ClrName}\r\n{defBody}");
                    }

                }
            }
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            return result;
        }
    }


}
