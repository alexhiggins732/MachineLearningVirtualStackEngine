using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ILEngine.Tests
{
    [TestClass()]
    public class IlInstructionReaderTests
    {
        [TestMethod()]
        public void ConvertIConvertibleByteCodeTest()
        {

            Func<List<string>> IConvertibleByteCodeTests = () =>
                {
                    var failedTests = new List<string>();
                    var except = new[] { TypeCode.DateTime, TypeCode.DBNull, TypeCode.Empty, TypeCode.Object/*,TypeCode.Boolean, TypeCode.Char*/ };
                    var exceptTypes = new[] { typeof(DateTime), typeof(DBNull), typeof(object), typeof(string)/*, typeof(bool), typeof(char)*/ };
                    var typeCodeValues = Enum.GetValues(typeof(TypeCode)).Cast<TypeCode>().Except(except).ToList();
                    var convertType = typeof(Convert);
                    var minMaxValueFlags = BindingFlags.Public | BindingFlags.Static;


                    var converterMethodGroups = convertType.GetMethods().Where(x => x.Name.StartsWith("To")).ToLookup(x => x.Name);
                    foreach (var converterMethodGroup in converterMethodGroups)
                    {
                        var converterMethodName = converterMethodGroup.Key;
                        var converterMethods = converterMethodGroup.ToList();
                        var convertToType = converterMethods.First().ReturnType;
                        if (exceptTypes.Contains(convertToType)|| convertToType==typeof(int))
                            continue;
                        ParameterInfo[] parameters;
                        var convertToThis = converterMethods
                        .FirstOrDefault(x =>
                            (parameters = x.GetParameters()).Length == 1 && parameters[0].ParameterType == x.ReturnType);

                        var convertFromInt = converterMethods
                            .FirstOrDefault(x => (parameters = x.GetParameters()).Length == 1 && parameters[0].ParameterType == typeof(int));

                        var defaultValue = convertFromInt.Invoke(null, new object[] { 0 });
                        if (defaultValue is null) failedTests.Add($"{convertToType.Name}.defaultValue is null");
                        var mx = Single.MaxValue;
                        TypeCode targetTypeCode = Convert.GetTypeCode(defaultValue);
                        switch (targetTypeCode) // switch isn't implemented yet :/
                        {
                            case TypeCode.Char:
                                {
                                    var maxValue = (object)(char)0;
                                    var minValue = (object)(char)ushort.MaxValue;
                                    var convertedMax = convertToThis.Invoke(null, new object[] { maxValue });
                                    if (convertedMax is null) failedTests.Add($"{convertToType.Name}.convertedMin is null");
                                    if (((IComparable)maxValue).CompareTo(convertedMax) != 0) failedTests.Add($"{convertToType.Name}.maxValue({maxValue}) != (convertedMax){convertedMax}");


                                    var convertedMin = convertToThis.Invoke(null, new[] { minValue });
                                    if (convertedMin is null) failedTests.Add($"{convertToType.Name}.convertedMin is null");
                                    if (((IComparable)minValue).CompareTo(convertedMin) != 0) failedTests.Add($"{convertToType.Name}.minValue({minValue}) != (convertedMin){convertedMin}");

                                }
                                break;
                            case TypeCode.Boolean:
                                {


                                    var maxValue = (object)true;
                                    var convertedMax = convertToThis.Invoke(null, new object[] { maxValue });
                                    if (convertedMax is null) failedTests.Add($"{convertToType.Name}.convertedMin is null");
                                    if (((IComparable)maxValue).CompareTo(convertedMax) != 0) failedTests.Add($"{convertToType.Name}.maxValue({maxValue}) != (convertedMax){convertedMax}");

                                    var minValue = (object)false;
                                    var convertedMin = convertToThis.Invoke(null, new[] { minValue });
                                    if (convertedMin is null) failedTests.Add($"{convertToType.Name}.convertedMin is null");
                                    if (((IComparable)minValue).CompareTo(convertedMin) != 0) failedTests.Add($"{convertToType.Name}.minValue({minValue}) != (convertedMin){convertedMin}");
                                }
                                break;
                            default:
                                {
                                    var maxValueField = convertToType.GetField("MaxValue", minMaxValueFlags);
                                    if (maxValueField is null && convertToType != typeof(bool)) failedTests.Add($"{convertToType.Name}.maxValueField is null");

                                    var maxValue = maxValueField.GetValue(null);
                                    if (maxValue is null) failedTests.Add($"{convertToType.Name}.maxValue is null");


                                    var convertedMax = convertToThis.Invoke(null, new[] { maxValue });
                                    if (convertedMax is null) failedTests.Add($"{convertToType.Name}.convertedMax is null");

                                    if (((IComparable)maxValue).CompareTo(convertedMax) != 0) failedTests.Add($"{convertToType.Name}.maxValue({maxValue}) != (convertedMax){convertedMax}");

                                    var minValueField = convertToType.GetField("MinValue", minMaxValueFlags);
                                    if (minValueField is null) failedTests.Add($"{convertToType.Name}.minValueField is null");

                                    var minValue = minValueField.GetValue(null);
                                    if (minValue is null) failedTests.Add($"{convertToType.Name}.minValue is null");
                                    var convertedMin = convertToThis.Invoke(null, new[] { minValue });
                                    if (convertedMin is null) failedTests.Add($"{convertToType.Name}.convertedMin is null");
                                    if (((IComparable)minValue).CompareTo(convertedMin) != 0) failedTests.Add($"{convertToType.Name}.minValue({minValue}) != (convertedMin){convertedMin}");
                                }
                                break;

                        }

                        foreach (var converterMethod in converterMethods)
                        {
                            parameters = converterMethod.GetParameters();
                            if (parameters.Length == 1 && !exceptTypes.Contains(parameters[0].ParameterType))
                            {
                                var paramDefault = Convert.ChangeType(0, parameters[0].ParameterType);
                                TypeCode paramTypeCode = Convert.GetTypeCode(paramDefault);
                                if (targetTypeCode == TypeCode.Boolean && paramTypeCode == TypeCode.Char)
                                    continue;
                                if (targetTypeCode == TypeCode.Char && (paramTypeCode == TypeCode.Boolean || paramTypeCode == TypeCode.Single || paramTypeCode == TypeCode.Double || paramTypeCode == TypeCode.Decimal))
                                    continue;
                                if (paramTypeCode == TypeCode.Char && (targetTypeCode == TypeCode.Boolean || targetTypeCode == TypeCode.Single || targetTypeCode == TypeCode.Double || targetTypeCode == TypeCode.Decimal))
                                    continue;
                                //var inline = Convert.ToBoolean('0');

                                var convertFromDefault = converterMethod.Invoke(null, new object[] { paramDefault });
                                if (convertFromDefault is null) failedTests.Add($"{converterMethod.Name}({defaultValue}) method is null");
                                var convertToBoolInt = Convert.ToInt32(convertFromDefault);
                                if (convertToBoolInt != 0) failedTests.Add($"{converterMethod.Name}({convertFromDefault}{{0}}) !=0");
                            }
                        }

                    }
                    return failedTests;
                };

            //make sure the test is succesful in c#
            List<string> expected = IConvertibleByteCodeTests();
            var engine = new ILEngine.IlInstructionEngine();
            var engineResult = engine.ExecuteTyped<List<string>>(IConvertibleByteCodeTests.Method, IConvertibleByteCodeTests.Target);
        }
    }
}