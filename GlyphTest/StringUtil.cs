using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlyphTest
{
    public static class StringUtil
    {
        public static List<string> Tokenize(this string value)
        {
            var res = new List<string>();

            var str = value ?? string.Empty;
            str += " ";
            string lastStr = string.Empty;

            for (int i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                if ((!char.IsLetterOrDigit(ch) || string.IsNullOrWhiteSpace(ch.ToString()))/* && (ch != 'ß') && (ch != 'µ') && (ch != 'ˆ')*/)
                {
                    if (i != 0)
                    {
                        res.Add(lastStr.Trim());
                        lastStr = string.Empty;
                        lastStr += ch;
                    }
                }
                else
                {
                    lastStr += ch;
                }
            }

            return res;
        }
    }
}
