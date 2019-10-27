using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlDasmLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            var psi = new ProcessStartInfo();
            var ildasmDir = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.2 Tools\";
            var ildasmExeFileName = "ildasm.exe";
            var ildasmFullPath = Path.Combine(ildasmDir, ildasmExeFileName);
            psi.FileName = ildasmFullPath; // [options] [PEfilename] [options]"
            psi.Arguments = GetIlArgs();
            var p = Process.Start(psi);
            p.WaitForExit();
        }

        private static string GetIlArgs()
        {
            var argList = new List<string>();
            var root = @"C:\Users\alexander.higgins\source\repos\ILDisassembler\ILDisassembler\bin\Debug\";
            var exeName = "ILDisassembler.exe";
            var exeFullPath = Path.Combine(root, exeName);
            argList.Add(exeFullPath);

            return string.Join(" ", argList);
        }
    }
}
