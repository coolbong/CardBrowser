using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CardBrowser
{

    public class Tl
    {
        public string Tag { get; private set; }
        public int Length { get; private set; }
        public string Value { get; set; }
        public string Desc { get; set; }

        public Tl(string tag, int length)
        {
            this.Tag = tag;
            this.Length = length;
        }
    }

    public class Dol
    {
        public static ICollection<Tl> Parse(byte[] buf)
        {
            int offset = 0;
            ICollection<Tl> list = new List<Tl>();

            if ((buf == null) || (buf.Length == 0))
            {
                return list;
            }

            while (offset < buf.Length)
            {
                int length_of_tag = 1;
                if ((buf[offset] & 0x1f) == 0x1f)  // subsequent byte
                {
                    int idx = offset + 1;
                    do
                    {
                        length_of_tag++;
                    } while ((buf[idx++] & 0x80) == 0x80);
                    
                }

                string tag = Hex.ToHex(buf, offset, length_of_tag);
                offset += length_of_tag;

                int lengthOfLength;
                int length;
                if ((buf[offset] & 0x80) != 0)
                {
                    lengthOfLength = (buf[offset] & 0x7F) + 1;
                    length = Hex.ToInt(buf, offset, (buf[offset] & 0x7F));
                }
                else
                {
                    lengthOfLength = 1;
                    length = buf[offset];
                }
                offset += lengthOfLength;

                list.Add(new Tl(tag, length));
            }

            return list;
        }

    }


    public class Hex
    {
        public static byte[] ToBytes(string hex)
        {
            string trim = Regex.Replace(hex, @"\s", "");

            return Enumerable.Range(0, trim.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(trim.Substring(x, 2), 16))
                     .ToArray();
        }

        public static byte[] ToBytes(int val)
        {
            return ToBytes(ToHex(val));
        }

        public static string ToAscii(byte[] arr)
        {
            return Encoding.ASCII.GetString(arr);
        }

        public static string ToHex(int val)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0:X2}", val);
            if ((sb.Length & 1) == 1)
            {
                sb.Insert(0, "0");
            }
            return sb.ToString();
        }

        public static string ToHex(byte[] arr)
        {
            var sb = new StringBuilder(arr.Length * 2);
            foreach (byte b in arr)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }

        public static string ToHex(byte[] arr, int offset, int length)
        {
            var sb = new StringBuilder(length * 2);
            for (int i = offset; i < (offset + length); i++)
            {
                sb.AppendFormat("{0:X2}", arr[i]);
            }
            return sb.ToString();
        }

        public static int ToInt(byte[] arr, int offset, int length)
        {
            var result = 0;
            for (var i = 0; i < length; i++)
            {
                result = (result << 8) | arr[offset + i];
            }
            return result;
        }

        public static int ToInt(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }
    }
}
