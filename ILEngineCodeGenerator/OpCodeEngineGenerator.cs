using ILEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILEngine.CodeGenerator
{
    public class OpCodeEngineGenerator
    {
        public static string GenerateFrameEngine(string nameSpace, string className, Action<CsScopedExpression, OpCodeMetaModel> binder = null, Func<string[]> usings = null)
        {

            if (usings == null)
            {
                usings = () => new[] { "System", "System.Collections.Generic", "System.IO", "System.Linq", "System.Reflection", "System.Reflection.Emit", "System.Text" };
            }
            var builder = new CsExpressionBuilder().CreateScope();

            builder.AddUsings(usings());
            builder.AddPreBodyExpression("\r\n");
            builder.AddNamespaceDeclaration(nameSpace);
            builder.AddPreBodyExpression("{\r\n");
            builder.AddPostBodyExpression("}\r\n");

            var classBuilder = builder.CreateChildExpression().CreateScope();
            classBuilder.AddPreBodyExpression($"public class {className} : {nameof(IILEngine)}\r\n");
            classBuilder.AddPreBodyExpression("{\r\n");
            classBuilder.AddPostBodyExpression("}\r\n");

            var classFieldsBuilder = classBuilder.CreateChildExpression().CreateScope();
            classFieldsBuilder.AddPreBodyExpression($"{nameof(ILStackFrameFlowControlTarget)} flowControlTarget;\r\n");
            classFieldsBuilder.AddPreBodyExpression($"{nameof(IILStackFrame)} frame;\r\n");
            classFieldsBuilder.AddPreBodyExpression($"public {nameof(ILStackFrameFlowControlTarget)} FlowControlTarget {{ get => flowControlTarget; set => flowControlTarget = value; }}\r\n");
            classFieldsBuilder.AddPreBodyExpression($"public {nameof(IILStackFrame)} StackFrame {{ get => frame; set => frame = value; }}\r\n");
            classFieldsBuilder.AddPreBodyExpression($"public bool BreakOnDebug {{ get; set; }} = false;\r\n");
            classFieldsBuilder.AddPreBodyExpression($"public bool ThrowOnException {{ get; set; }} = false;\r\n");
            classFieldsBuilder.AddPreBodyExpression($"\r\n");
            var methodBuilder = classBuilder.CreateChildExpression().CreateScope();
            methodBuilder.AddPreBodyExpression($"public void ExecuteFrame({nameof(IILStackFrame)} frame)\r\n");
            methodBuilder.AddPreBodyExpression("{\r\n");
            methodBuilder.AddPostBodyExpression("}\r\n");

            var methodBody = methodBuilder.CreateChildExpression().CreateScope();

            methodBody.AddPreBodyExpression("this.frame = frame;\r\n");
            methodBody.AddPreBodyExpression($"this.flowControlTarget = {nameof(ILStackFrameFlowControlTarget)}.{nameof(ILStackFrameFlowControlTarget.Inc)};\r\n");
            methodBody.AddPreBodyExpression($"frame.{nameof(IILStackFrame.Reset)}();\r\n");
            methodBody.AddPreBodyExpression("goto Inc;\r\n");
            methodBody.AddPreBodyExpression("\r\n");

            methodBody.AddPreBodyExpression("ReadNext:\r\n");
            methodBody.AddPreBodyExpression($"frame.{nameof(IILStackFrame.ReadNext)}();\r\n");
            methodBody.AddPreBodyExpression("\r\n");

            methodBody.AddPreBodyExpression("short opCodeValue = frame.Code.Value;\r\n");
            methodBody.AddPreBodyExpression("ExecuteOpCode(opCodeValue);\r\n");

            methodBody.AddPreBodyExpression("\r\n");
            methodBody.AddPreBodyExpression("switch(flowControlTarget)\r\n");
            methodBody.AddPreBodyExpression("{\r\n");
            methodBody.AddPostBodyExpression("}\r\n");

            var methodSwitch = methodBody.CreateChildExpression();


            var caseReadNext = methodSwitch.CreateScope();
            caseReadNext.AddPreBodyExpression($"case {nameof(ILStackFrameFlowControlTarget)}.{nameof(ILStackFrameFlowControlTarget.ReadNext)}:\r\n");
            caseReadNext.CreateScopedChild().AddPreBodyExpression("goto ReadNext;\r\n");

            methodSwitch.CreateScope().AddPreBodyExpression($"case {nameof(ILStackFrameFlowControlTarget)}.{nameof(ILStackFrameFlowControlTarget.Inc)}:\r\n")
                .CreateScopedChild().AddPreBodyExpression("goto Inc;\r\n");

            methodSwitch.CreateScope().AddPreBodyExpression($"case {nameof(ILStackFrameFlowControlTarget)}.{nameof(ILStackFrameFlowControlTarget.Ret)}:\r\n")
             .CreateScopedChild().AddPreBodyExpression("goto Ret;\r\n");

            methodSwitch.CreateScope().AddPreBodyExpression($"case {nameof(ILStackFrameFlowControlTarget)}.{nameof(ILStackFrameFlowControlTarget.MoveNext)}:\r\n")
            .CreateScopedChild().AddPreBodyExpression("goto MoveNext;\r\n");





            methodBody.AddPostBodyExpression("\r\n");
            methodBody.AddPostBodyExpression("Inc:\r\n");
            methodBody.AddPostBodyExpression($"frame.{nameof(IILStackFrame.Inc)}();\r\n");

            methodBody.AddPostBodyExpression("\r\n");
            methodBody.AddPostBodyExpression("MoveNext:\r\n");
            methodBody.AddPostBodyExpression($"if (frame.{nameof(IILStackFrame.MoveNext)}()) goto ReadNext;\r\n");

            methodBody.AddPostBodyExpression("\r\n");
            methodBody.AddPostBodyExpression("Ret:\r\n");
            methodBody.AddPostBodyExpression($"if (frame.{nameof(IILStackFrame.Exception)} != null && ThrowOnException) throw (frame.{nameof(IILStackFrame.Exception)});\r\n");
            methodBody.AddPostBodyExpression($"frame.{nameof(IILStackFrame.ReturnResult)} = (frame.{nameof(IILStackFrame.Stack)}.Count > 0 && frame.{nameof(IILStackFrame.Exception)} == null) ? frame.{nameof(IILStackFrame.Stack)}.Pop() : null;\r\n");








            var executeMethodBuilder = classBuilder.CreateChildExpression().CreateScope();
            executeMethodBuilder.AddPreBodyExpression($"public void ExecuteOpCode(short opCodeValue)\r\n");
            executeMethodBuilder.AddPreBodyExpression("{\r\n");
            executeMethodBuilder.AddPostBodyExpression("}\r\n");


            var notImplementedBuilder = classBuilder.CreateChildExpression().CreateScope();
            notImplementedBuilder.AddPreBodyExpression($"public void NotImplemented()\r\n");
            notImplementedBuilder.AddPreBodyExpression("{\r\n");
            notImplementedBuilder.AddPostBodyExpression("}\r\n");
            var notImplementedBody = notImplementedBuilder.CreateChildExpression().CreateScope();
            notImplementedBody.AddPreBodyExpression($"frame.{nameof(IILStackFrame.Exception)} = new OpCodeNotImplementedException(frame.{nameof(IILStackFrame.Code)}.Value);\r\n");
            notImplementedBody.AddPostBodyExpression("flowControlTarget = IlStackFrameFlowControlTarget.Ret;\r\n");


            var executeMethodBody = executeMethodBuilder.CreateChildExpression().CreateScope();
            executeMethodBody.AddPreBodyExpression("switch (opCodeValue)\r\n");
            executeMethodBody.AddPreBodyExpression("{\r\n");
            executeMethodBody.AddPostBodyExpression("}\r\n");





            var models = OpCodeMetaModel.OpCodeMetas;
            if (binder == null)
            {
                Action<CsScopedExpression, OpCodeMetaModel> defaultBinder = (scope, model) => BindOpCodeHandler(scope, model);
                binder = defaultBinder;

            }

            foreach (var model in models)
            {
                var methodCase = executeMethodBody.CreateChildExpression().CreateScope();
                methodCase.AddPreBodyExpression($"case unchecked((short)ILOpCodeValues.{model.ClrName}):\r\n");

                var handleOpCodeExpression = methodCase.CreateChildExpression().CreateScope();
                binder(handleOpCodeExpression, model);

            }
            var defaultCase = executeMethodBody.CreateChildExpression().CreateScope();
            defaultCase.AddPreBodyExpression($"default:\r\n");

            var handledefaultCaseExpression = defaultCase.CreateChildExpression().CreateScope();
            handledefaultCaseExpression.AddPreBodyExpression($"NotImplemented();\r\n");
            handledefaultCaseExpression.AddPreBodyExpression($"break;\r\n");
            //handledefaultCaseExpression.AddPreBodyExpression($"goto Ret;\r\n");



            var result = builder.ToString();
            return result;
        }

        private static void BindOpCodeHandler(CsScopedExpression scope, OpCodeMetaModel model)
        {

            scope.AddPreBodyExpression("{\r\n");
            scope.AddPostBodyExpression("}\r\n");
            scope.AddPostBodyExpression("break;\r\n");
            var handlerBody = scope.CreateChildExpression().CreateScope();
            handlerBody.AddPreBodyExpression($"///TODO: Load and bind {model.ClrName} instructions\r\n");

        }

        public static Action<CsScopedExpression, OpCodeMetaModel> StackFrameMethodBinder(Dictionary<short, string> defaultBodies)
        {
            Action<CsScopedExpression, OpCodeMetaModel> result = (scope, model) =>
            {

                scope.AddPreBodyExpression($"{model.ClrName}();\r\n");
                scope.AddPostBodyExpression("break;\r\n");
                var classBuilder = (CsExpressionBuilder)scope.Parent.Parent.Parent.Parent;
                var handlerBuilder = classBuilder.CreateScope();
                handlerBuilder.AddPreBodyExpression($"public void {model.ClrName}()\r\n");
                handlerBuilder.AddPreBodyExpression("{\r\n");
                var handlerBody = handlerBuilder.CreateChildExpression().CreateScope();
                //handlerBody.AddPreBodyExpression($"//TODO: Handle {model.ClrName}();\r\n");
                var flowControlStatement = "";
                switch (model.FlowControl)
                {
                    case "Branch":
                        flowControlStatement = $"{nameof(IILEngine.FlowControlTarget)} = {nameof(ILStackFrameFlowControlTarget)}.{nameof(ILStackFrameFlowControlTarget.ReadNext)};\r\n";
                        break;
                    case "Cond_Branch":
                        handlerBody.AddPreBodyExpression($"bool doJump = false;\r\n");
                        flowControlStatement = $"{nameof(IILEngine.FlowControlTarget)} = (doJump) ? {nameof(ILStackFrameFlowControlTarget)}.{nameof(ILStackFrameFlowControlTarget.ReadNext)} : {nameof(ILStackFrameFlowControlTarget)}.{nameof(ILStackFrameFlowControlTarget.Inc)};\r\n";
                        break;
                    case "Return":
                        flowControlStatement = $"{nameof(IILEngine.FlowControlTarget)} = {nameof(ILStackFrameFlowControlTarget)}.{nameof(ILStackFrameFlowControlTarget.Ret)};\r\n";
                        break;

                }
                if (defaultBodies.ContainsKey((short)model.OpCodeValue))
                {
                    var body = defaultBodies[(short)model.OpCodeValue];
                    var lines = body.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                    foreach (var line in lines)
                    {
                        if (line.IndexOf("break;") > -1 || line.IndexOf("goto") > -1)
                        {
                            handlerBody.AddPreBodyExpression($"//{line}\r\n");
                        }
                        else
                        {
                            handlerBody.AddPreBodyExpression($"{line}\r\n");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(flowControlStatement))
                {
                    handlerBody.AddPreBodyExpression(flowControlStatement);
                }
              
                handlerBuilder.AddPostBodyExpression("}\r\n");

            };
            return result;
        }

    }

}
