using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlyphTest
{
    class ParsString
    {
        public string HtmlStringParsing(string htmlString)
        {
            htmlString = htmlString.Replace("<", "&lt;");
            htmlString = htmlString.Replace(">", "&gt;");
            htmlString = htmlString.Replace("&lt;m&gt;", "<m> ");
            htmlString = htmlString.Replace("&lt;/m&gt;", " </m>");
            return htmlString;
        }
    }
}
