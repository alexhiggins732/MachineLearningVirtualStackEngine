using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ILEngineTests
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MyPerson
    {
        public string first;
        public string last;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MyPerson2
    {
        public IntPtr person;
        public int age;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MyPerson3
    {
        public MyPerson person;
        public int age;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ptr
    {
        public IntPtr value;
    }
}
