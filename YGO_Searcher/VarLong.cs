using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGO_Searcher
{
    internal static class VarLong
    {
        public static byte[] GetBytes(ulong value)
        {
            using (var ms = new MemoryStream())
            {
                while (value != 0)
                {
                    var b = value & 0x7f;
                    value >>= 7;
                    if (value != 0)
                        b |= 0x80;
                    ms.WriteByte((byte)b);
                }
                return (ms.ToArray());
            }
        }

        public static ulong ReadNext(byte[] bytes, out int length)
        {
            length = 0;
            ulong result = 0;
            foreach (var b in bytes)
            {
                var value = (ulong)b & 0x7f;
                result |= value << length * 7;
                if ((b & 0x80) != 0x80)
                    break;
                length++;
            }
            length++;
            return (result);
        }
    }
}
