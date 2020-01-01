using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Reflection.Emit;

namespace ILEngine.Tests
{
    [TestClass()]
    public class OpCodeMetaModelTests
    {
        [TestMethod()]
        public void UpdateModelFromDbTest()
        {
            //Assert.Fail();
            var testFileName = "testModelPath.json";
            var testFilePath = Path.GetFullPath(testFileName);
            OpCodeMetaModel.UpdateModelFromDb(new[] { testFilePath });
            Assert.IsTrue(File.Exists(testFilePath));
            var json = File.ReadAllText(testFilePath);
            List<OpCodeMetaModel> OpCodeMetas = JsonConvert.DeserializeObject<List<OpCodeMetaModel>>(json);
            Assert.IsTrue(OpCodeMetas.Count == OpCodeMetaModel.OpCodeMetas.Count);
            File.Delete(testFilePath);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            var metaModel = OpCodeMetaModel.OpCodeMetaDict[OpCodes.Nop.Value];
            var actual = metaModel.ToString();
            var expected = "Nop: Next InlineNone (0 bits) Primitive Push: Push0 Pop: Pop0";
            Assert.IsTrue(actual == expected);
        }
    }
}