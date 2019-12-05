using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HashReader
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public abstract class MD4 : HashAlgorithm
    {
        static MD4()
        {
            CryptoConfig.AddAlgorithm(typeof(MD4CryptoServiceProvider), "System.Security.Cryptography.MD4");
        }

        protected MD4()
        {
            HashSizeValue = 128;
        }

        new static public MD4 Create()
        {
            return Create("System.Security.Cryptography.MD4");
        }

        new static public MD4 Create(string algName)
        {
            return (MD4)CryptoConfig.CreateFromName(algName);
        }
    }

    [System.Runtime.InteropServices.ComVisible(true)]
    public sealed class MD4CryptoServiceProvider : MD4
    {
        internal static class Utils
        {
            internal static Type UtilsType = Type.GetType("System.Security.Cryptography.Utils");

            public static T InvokeInternalMethodOfType<T>(object o, object pType, string methodName, params object[] args)
            {
                var internalType = (pType is string internalTypeName) ? Type.GetType(internalTypeName) : (Type)pType;
                var internalMethods = internalType.GetMethods(BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | (o == null ? BindingFlags.Static : 0));
                var internalMethod = internalMethods.Where(m => m.Name == methodName && m.GetParameters().Length == args.Length).Single();
                return (T)internalMethod?.Invoke(o, args);
            }

            public static T GetInternalPropertyValueOfInternalType<T>(object o, object pType, string propertyName)
            {
                var internalType = (pType is string internalTypeName) ? Type.GetType(internalTypeName) : (Type)pType;
                var internalProperty = internalType.GetProperty(propertyName, BindingFlags.NonPublic | (o == null ? BindingFlags.Static : 0));
                return (T)internalProperty.GetValue(o);
            }

            internal static SafeHandle CreateHash(int algid)
            {
                return InvokeInternalMethodOfType<SafeHandle>(null, UtilsType, "CreateHash", GetInternalPropertyValueOfInternalType<object>(null, UtilsType, "StaticProvHandle"), algid);
            }

            internal static void HashData(SafeHandle h, byte[] data, int ibStart, int cbSize)
            {
                InvokeInternalMethodOfType<object>(null, UtilsType, "HashData", h, data, ibStart, cbSize);
            }

            internal static byte[] EndHash(SafeHandle h)
            {
                return InvokeInternalMethodOfType<byte[]>(null, UtilsType, "EndHash", h);
            }
        }

        internal const int ALG_CLASS_HASH = (4 << 13);
        internal const int ALG_TYPE_ANY = (0);
        internal const int ALG_SID_MD4 = 2;
        internal const int CALG_MD4 = (ALG_CLASS_HASH | ALG_TYPE_ANY | ALG_SID_MD4);

        [System.Security.SecurityCritical]
        private SafeHandle _safeHashHandle = null;

        [System.Security.SecuritySafeCritical]
        public MD4CryptoServiceProvider()
        {
            if (CryptoConfig.AllowOnlyFipsAlgorithms)
                throw new InvalidOperationException("Cryptography_NonCompliantFIPSAlgorithm");
            Contract.EndContractBlock();
            // cheat with Reflection
            _safeHashHandle = Utils.CreateHash(CALG_MD4);
        }

        protected override void Dispose(bool disposing)
        {
            if (_safeHashHandle != null && !_safeHashHandle.IsClosed)
                _safeHashHandle.Dispose();
            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            if (_safeHashHandle != null && !_safeHashHandle.IsClosed)
                _safeHashHandle.Dispose();

            _safeHashHandle = Utils.CreateHash(CALG_MD4);
        }

        protected override void HashCore(byte[] rgb, int ibStart, int cbSize)
        {
            Utils.HashData(_safeHashHandle, rgb, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            return Utils.EndHash(_safeHashHandle);
        }
    }

    static class Ext
    {
        public static HashAlgorithm MD4Singleton;
        public static HashAlgorithm Sha1Singleton;
        static Ext()
        {
            MD4Singleton = HashReader.MD4.Create();
            Sha1Singleton = SHA1.Create();
        }

        public static byte[] MD4(this string s)
        {
            return MD4Singleton.ComputeHash(System.Text.Encoding.Unicode.GetBytes(s));
        }
        public static byte[] Sha1(this string s)
        {
            return Sha1Singleton.ComputeHash(System.Text.Encoding.Unicode.GetBytes(s));
        }

        public static string AsHexString(this byte[] bytes)
        {
            return String.Join("", bytes.Select(h => h.ToString("X2")));
        }


        public static byte[] FromHexStringWArray(this string hex)
        {
            byte[] result = new byte[hex.Length >> 1];
            int hi, low;
            int i = result.Length - 1;
            char[] alpha = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            for (var k = hex.Length - 1; k > 0; k -= 2)
            {
                low = Array.IndexOf(alpha, hex[k]);
                hi = (Array.IndexOf(alpha, hex[k - 1]) << 4);
                result[i--] = (byte)(hi + low);
                //result[i++] = (byte)((Array.IndexOf(alpha, hex[k + 1]) << 2) + Array.IndexOf(alpha, hex[k]));
            }
            return result;
        }

        public static byte[] FromHexString(this string hex)
        {
            byte[] result = new byte[hex.Length >> 1];
            int hi, low;
            int i = result.Length - 1;
            for (var k = 0; k < hex.Length; k += 2)
            {
                low = (int)hex[k + 1];
                hi = (int)hex[k];
                low = low - (low < 58 ? 48 : 55);
                hi = hi - (hi < 58 ? 48 : 55);
                result[i--] = (byte)((hi << 4) + low);
            }
            return result;
        }
    }
}
