using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HashReader
{
    public class Hasher
    {

        private static SHA1 sha1 = SHA1.Create();
        private static MD5 md5 = MD5.Create();
        private static StringBuilder returnValue = new StringBuilder();
        public static string GetSHA1HashData(string data)
        {
            //create new instance of md5


            //convert the input text to array of bytes
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));

            //create new instance of StringBuilder to save hashed data

            returnValue.Clear();
            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString("X2"));
            }

            // return hexadecimal string
            return returnValue.ToString();
        }
        private string GetMD5HashData(string data)
        {
            //create new instance of md5
            //convert the input text to array of bytes
            byte[] hashData = md5.ComputeHash(Encoding.Default.GetBytes(data));

            //create new instance of StringBuilder to save hashed data

            returnValue.Clear();
            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();

        }
    }
    class Program
    {
        public static string connString = "server=.;initial catalog=sha1db;trusted_connection=true";
        static void Main(string[] args)
        {
            
            var passwordSha1Hash = Hasher.GetSHA1HashData("password");
            var length = passwordSha1Hash.Length;
            var passwordHashBytes = passwordSha1Hash.BytesFromHex();
            var byteLength = passwordHashBytes.Length;

            CompressShaDb();
            var cnt = CountSha1Lines();
            var a = "FF".FromHexString();
            var b = "FFFF".FromHexString();
            var c = "0F".FromHexString();
            var d = "F0".FromHexString();

            var found = FindHash(passwordSha1Hash);
            CompressSha1Counts();
            PeekSha1Counts();
        }
        static string FindHash(string hash)
        {
            var lines = Sha1CountLines();
            foreach (var line in lines)
            {
                if (line.StartsWith(hash))
                    return line;
            }
            return null;
        }

        static void BulkInsert(DataTable dt, string tableName)
        {
            using (var conn = new SqlConnection(connString))
            {
                using (var bulk = new SqlBulkCopy(conn))
                {
                    conn.Open();
                    bulk.DestinationTableName = tableName;
                    bulk.WriteToServer(dt);
                    conn.Close();
                }
            }
        }

        public static void CompressShaDb()
        {
            // insert first bytes into sha1_0

            var sw = Stopwatch.StartNew();
            Console.WriteLine(nameof(CompressShaDb));
            var tableName = $"shaHash";
            var columnNames = Enumerable.Range(0, 20).Select(x => $"b{x}").ToList();
 
            string sql = $"create table [{tableName}] ({string.Join(", ", columnNames.Select(x => $"{x} tinyint"))})";
            Console.WriteLine($"Seeding {tableName}");
            using (var conn = new SqlConnection(connString))
            {
                conn.Execute(sql);
                var dt = new DataTable();
                dt.Columns.Add("val");


                Enumerable.Range(0, 256).ToList().ForEach(i =>
                {
                    var row = dt.NewRow();
                    row["val"] = (byte)i;
                    dt.Rows.Add(row);
                });

                dt.BulkInsert(tableName);


                tableName = $"b{1}";
                Console.WriteLine($"Seeding {tableName}");
                sql = $"create table [{tableName}] (id smallint, val tinyint,previd tinyint)";
                conn.Execute(sql);

                dt = new DataTable();
                dt.Columns.Add("id", typeof(ushort));
                dt.Columns.Add("val", typeof(byte));
                dt.Columns.Add("previd", typeof(byte));
                Enumerable.Range(0, 256).ToList().ForEach(previd =>
                {
                    Enumerable.Range(0, 256).ToList().ForEach(val =>
                    {
                        var row = dt.NewRow();
                        ushort id = (ushort)previd;
                        id <<= 4;
                        id += (byte)val;
                        row[nameof(id)] = id;
                        row[nameof(val)] = (byte)val;
                        row[nameof(previd)] = (byte)previd;

                        dt.Rows.Add(row);
                    });

                });
                dt.BulkInsert(tableName);
                for (var i = 2; i < 20; i++)
                {
                    tableName = $"b{i}";
                    sql = $"create table [{tableName}] (id smallint, val tinyint, previd tinyint)";
                    conn.Execute(sql);
                }
            }




            Console.WriteLine($"  ... completed in {sw.Elapsed}");

            var hash = new byte[20 * 10000];
            var dataTables = Enumerable.Range(0, 20).Skip(2).Select(i =>
              {
                  var dt = new DataTable();
                  dt.TableName = $"b{i}";
                  dt.Columns.Add("id", typeof(ushort));
                  dt.Columns.Add("val", typeof(byte));
                  dt.Columns.Add("previd", typeof(byte));
                  return dt;

              }).ToList();
            using (var fs = File.OpenRead(sha1OrderedByCountBinPath))
            {
                using (var br = new BinaryReader(fs))
                {
                    while (fs.Position < fs.Length)
                    {
                        int read = br.Read(hash, 0, hash.Length);
                        dataTables.ForEach(x => x.Clear());
                        for (var idx = 0; idx < read; idx += 20)
                        {
                            var b0 = (ushort)hash[idx];//first byte
                            var prev = hash[idx + 1];//first byte
                            var prevId = (b0 << 4) + prev;

                            for (var i = idx + 2; i < idx + 20; i++)
                            {
                                var dt = dataTables[i - 2];
                                var row = dt.NewRow();
                                var val = hash[i];
                                row[nameof(prevId)] = prevId;
                                row[nameof(val)] = val;
                                //row[nameof(id)] = prev;

                                dt.Rows.Add(row);
                            }
                            //int prev = hash[idx1 + idx];
                            //int val = hash[idx2 + idx];

                        }

                    }
                }
            }

        }
     
        public static int CountSha1Lines()
        {
            Console.WriteLine($"{nameof(CompressSha1Counts)}");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            int peeked = 0;
            foreach (var line in Sha1CountLines())
            {
                peeked++;
            }
            Console.WriteLine($"{nameof(CountSha1Lines)} - Compressed {peeked} in {sw.Elapsed}");
            return peeked;
        }
        static void PeekSha1Counts()
        {
            int count = 100;
            int peeked = 0;
            foreach (var line in Sha1CountLines())
            {
                var idx = line.IndexOf(":");
                var hash = line.Substring(0, 40);
                var found = line.Substring(41);
                peeked++;
                Console.WriteLine(line);
            }

        }

        static string sha1OrderedByCountPath = @"C:\downloads\pwned-passwords-sha1-ordered-by-count-v5\pwned-passwords-sha1-ordered-by-count-v5.txt";
        static string sha1OrderedByCountBinPath = @"C:\downloads\pwned-passwords-sha1-ordered-by-count-v5\pwned-passwords-sha1-ordered-by-count-v5.bin";
        static IEnumerable<string> Sha1CountLines()
        {
            var enumerable = new FileStreamLineEnumerable(sha1OrderedByCountPath);
            foreach (var line in enumerable)
                yield return line;
        }
        static IEnumerable<byte[]> ReadDistinctSha1BytesPairs(int idx1, int idx2)
        {

            //Dictionary<byte, List<byte>> data = new Dictionary<byte, List<byte>>();
            var masks = Enumerable.Range(0, 256).Select(i => Enumerable.Range(0, 256).Select(y => false).ToArray()).ToArray();
            var hash = new byte[20 * 10000];
            using (var fs = File.OpenRead(sha1OrderedByCountBinPath))
            {
                using (var br = new BinaryReader(fs))
                {
                    while (fs.Position < fs.Length)
                    {
                        int read = br.Read(hash, 0, hash.Length);
                        for (var idx = 0; idx < read; idx += 20)
                        {
                            int prev = hash[idx1 + idx];
                            int val = hash[idx2 + idx];
                            masks[prev][val] = true;
                        }
                        //masks[hash[idx1]][hash[idx2]] = true;
                    }
                }
            }
            List<byte[]> result = new List<byte[]>();
            for (var i = 0; i < 256; i++)
            {
                for (var k = 0; k < 256; k++)
                {
                    if (masks[i][k])
                    {
                        result.Add(new byte[] { (byte)i, (byte)k });
                    }
                }
            }
            return result;
        }

        static IEnumerable<byte[]> ReadDistinctSha1BytesPairsWithBitArray(int idx1, int idx2)
        {
            if (bool.Parse(bool.TrueString)) throw new Exception("POC needs to be tested");
            var enumerable = ReadSha1Bytes();

            var b = new BitArray(65536);
            List<byte[]> result = new List<byte[]>();
            foreach (var hash in enumerable)
            {
                b[(((int)hash[idx1]) << 4) + (hash[idx2])] = true;
            }
            for (var i = 0; i < 65536; i++)
            {
                if (b[i]) result.Add(new byte[] { (byte)(i >> 4), (byte)(i & 256) });
            }
            return result;
        }
        static IEnumerable<byte[]> ReadSha1Bytes()
        {
            var enumerable = new Sha1BinFileEnumerable(sha1OrderedByCountBinPath);
            foreach (var hash in enumerable)
            {
                yield return hash;
            }
        }
        static void CompressSha1Counts()
        {
            Console.WriteLine($"{nameof(CompressSha1Counts)}");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var count = 0;
            using (var fs = File.Create(sha1OrderedByCountPath.Replace(".txt", "bin")))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    foreach (var line in Sha1CountLines())
                    {
                        count++;
                        var idx = line.IndexOf(":");
                        var hash = line.Substring(0, 40);
                        var bytes = hash.BytesFromHex();
                        bw.Write(bytes);
                    }
                }
            }

            Console.WriteLine($"{nameof(CompressSha1Counts)} - Compressed {count} in {sw.Elapsed}");
        }
    }
    public static class StringExtensions
    {
        public static string ToJson(this object value) => value.ToJson(Formatting.None);
        public static string ToJsonIndented(this object value) => value.ToJson(Formatting.Indented);

        public static string ToJson(this object value, Formatting formatting) => JsonConvert.SerializeObject(value, formatting);
        public static byte[] BytesFromHex(this string value)
        {
            return value.FromHexString();
        }
        public static string connString = "server=.;initial catalog=sha1db;trusted_connection=true";
        public static void BulkInsert(this DataTable dt, string tableName)
        {
            using (var conn = new SqlConnection(connString))
            {
                using (var bulk = new SqlBulkCopy(conn))
                {
                    conn.Open();
                    bulk.DestinationTableName = tableName;
                    bulk.WriteToServer(dt);
                    conn.Close();
                }
            }
        }

        public static void BulkInsert(this DataTable dt)
        {
            using (var conn = new SqlConnection(connString))
            {
                using (var bulk = new SqlBulkCopy(conn))
                {
                    conn.Open();
                    bulk.DestinationTableName = dt.TableName;
                    bulk.WriteToServer(dt);
                    conn.Close();
                }
            }
        }
    }
    public class FileStreamLineEnumerable : IEnumerable<string>
    {
        public FileStreamLineEnumerable(string path)
        {
            this.Path = path;
        }

        public string Path { get; }

        public IEnumerator<string> GetEnumerator() => new FileStreamLineEnumerator(this.Path);


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }

    public class Sha1BinFileEnumerable : IEnumerable<byte[]>
    {
        public Sha1BinFileEnumerable(string path)
        {
            this.Path = path;
        }

        public string Path { get; }

        public IEnumerator<byte[]> GetEnumerator() => new Sha1BinFileEnumerator(this.Path);


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }


    internal class Sha1BinFileEnumerator : IEnumerator<byte[]>
    {
        private string path;
        private FileStream fs;
        private BinaryReader br;

        public Sha1BinFileEnumerator(string path)
        {
            this.path = path;
            this.fs = File.OpenRead(path);
            this.br = new BinaryReader(fs);
            this.Current = new byte[20];
        }

        public byte[] Current { get; private set; }

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            if (br != null)
            {
                br.Close();
                br.Dispose();
                br = null;
            }

            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
                fs = null;
            }
        }

        public bool MoveNext()
        {
            var result = fs.Position < fs.Length;
            if (result)
                br.Read(Current, 0, 20);
            return result;
        }

        public void Reset()
        {
            fs.Position = 0;
        }
    }

    internal class FileStreamLineEnumerator : IEnumerator<string>
    {
        private string path;
        private StreamReader sr;

        public FileStreamLineEnumerator(string path)
        {
            this.path = path;
            this.sr = new StreamReader(path);
        }

        public string Current { get; private set; }

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            if (sr != null)
            {
                sr.Close();
                sr.Dispose();
                sr = null;
            }
        }

        public bool MoveNext()
        {
            var result = !sr.EndOfStream;
            if (result)
                Current = sr.ReadLine();
            return result;
        }

        public void Reset()
        {
            sr.BaseStream.Position = 0;
        }
    }
}
