using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ILDasmLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            string asmPath = string.Empty;
            if (args.Length == 0)
            {
                asmPath = GetIlArgs();
            }
            else
            {
                asmPath = args[0];
                if (IsRelativePath(asmPath))
                {
                    var root = GetProjectFolder(Assembly.GetExecutingAssembly());
                    var target = Path.GetFullPath(Path.Combine(root, asmPath));
                    asmPath = target;

                    //C:\Source\Repos\MachineLearningVirtualStackEngine\ILDasmTarget\bin\Debug
                }
            }
            var psi = new ProcessStartInfo();
            //var ildasmDir = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.2 Tools\";
            var ildasmDir = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools";
            var ildasmExeFileName = "ildasm.exe";
            var ildasmFullPath = Path.Combine(ildasmDir, ildasmExeFileName);
            if (!File.Exists(ildasmFullPath))
            {
                throw new Exception($"Could not find disassembler at path '{ildasmFullPath}'");
            }
            psi.FileName = ildasmFullPath; // [options] [PEfilename] [options]"
            psi.Arguments = asmPath;

            var p = Process.Start(psi);
            p.WaitForExit();
        }

        private static string GetProjectFolder(Assembly assembly)
        {
            var location = assembly.Location;
            var fi = new FileInfo(location);
            var di = fi.Directory;
            var projectFolderName = nameof(ILDasmLauncher);
            while (di.Parent != null && string.Equals(di.Name, projectFolderName, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                di = di.Parent;
            }
            return di.FullName;
        }

        private static bool IsRelativePath(string asmPath)
        {
            if (!asmPath.Contains(":") && !asmPath.StartsWith("/") && !asmPath.StartsWith(@"\"))
                return true;
            return false;
        }

        private static string GetIlArgs()
        {
            var argList = new List<string>();
            var exeFullPath = Assembly.GetExecutingAssembly().Location;
            //var root = @"C:\Source\Repos\MachineLearningVirtualStackEngine\IlDasmLauncher\bin\Debug\";
            //var exeName = "ILDasmLauncher.exe";
            //var exeFullPath = Path.Combine(root, exeName);
            argList.Add(exeFullPath);

            return string.Join(" ", argList);
        }
    }
}
