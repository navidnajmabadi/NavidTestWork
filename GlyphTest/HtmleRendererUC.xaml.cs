using NBidi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TheArtOfDev.HtmlRenderer.WPF;


namespace GlyphTest
{
    /// <summary>
    /// Interaction logic for HtmleRendererUC.xaml
    /// </summary>
    public partial class HtmleRendererUC : UserControl
    {
        public HtmleRendererUC()
        {
            InitializeComponent();
        }

        List<char> persianChar;
        List<char> englishChar;
        List<string> wordList;
        private int offset = -3;


        public string HtmlContent
        {
            get { return (string)GetValue(ExcelVisiblityProperty); }
            set { SetValue(ExcelVisiblityProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ExcelVisiblity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExcelVisiblityProperty =
            DependencyProperty.Register("HtmlContent", typeof(string), typeof(HtmleRendererUC), new PropertyMetadata(null, Html));
        private static void Html(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (d as HtmleRendererUC);

            control.BuildHead();
            control.BuildBody();
            control.HtmlPanel.Text = control.BuildHtml();
        }
        /// <summary>
        /// put in Html Header
        /// <param name="wordList"></param>
        /// <returns></returns>
        /// </summary>
        /// 
        private string body;
        private string head;
        private void BuildBody()
        {

            int maxLength = 0;
            if (!string.IsNullOrEmpty(this.HtmlContent))
            {

                double width = this.ActualWidth;
                SeprateList(this.HtmlContent);
                string text = String.Join(String.Empty, wordList.ToArray());
                int size = TextSize(text);
                maxLength = (int)width / size - offset;
                if (maxLength < 1 || size <= 0)
                    maxLength = 1;
                this.body = TruncateLongString(text, maxLength, fakeSpace, highlightTagOpen, highlightTagClose);
                //this.body = text;
            }
        }

        private void BuildHead()
        {
            double width = this.ActualWidth;
            int marginSize = 0;
            var align = (FlowDirection == FlowDirection.RightToLeft ? "right" : "left");
            var dir = (FlowDirection == FlowDirection.RightToLeft ? "rtl" : "ltr");
            var headHtml = $@"<head>
               <meta http-equiv=""Content-Type"" content=""text/html""; charset = ""UTF-8"" />
               <style type=""text/css"">
                 @font-face {{
                     font-family: 'B Nazanin';
                     src: url('C:\Users\admin\Desktop\Source\GlyphTest\fonts\BNazanin.eot'); 
                     src: local('BNazanin'), url('C:\Users\admin\Desktop\Sourc\GlyphTest\fonts\BNazanin.ttf') format('truetype'); 
                     font-weight:normal;
                 }}
             </style>
             <style>
                 m{{
                     background-color:yellow; 
                     font-weight: bold;
                 }}
                 body{{
                        font-family: ""B Nazanin""; 
                        text-align: {align};
                        direction : {dir};
                        padding : 5px;
                        margin-{align}: {marginSize}px;
                        margin-top : -2px;
                        float :{align};
                        width: {width}px;
                        word-wrap: break-word;
                    }}
                #text {{ width: {Width-5}px; overflow: hidden; white-space:nowrap; text-overflow: ""...""; }}
              </style>

              </head>";

            head = headHtml;
        }
        private string BuildHtml()
        {

            return $@"<html>{head}<body>{body}</body></html>";
        }
        private List<int> fakeSpace = null;
        private List<int> highlightTagOpen = null;
        private List<int> highlightTagClose = null;
        
        /// <summary>
        /// Get string and Seprate word by word and return List of Word
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private void SeprateList(string input)
        {

            List<string> wordSplitList = new List<string>();
            highlightTagOpen = new List<int>();
            highlightTagClose = new List<int>();
            int invalidChar = 0;
            fakeSpace = new List<int>();
            for (int i = 0; i < input.Length - 1; i++)
            {

                if (input[i] == 32 && input[i + 1] == 32) //find Space After Space
                {
                    fakeSpace.Add(i + 1 - 2);
                    //fakeSpace.Add(i + 1 - invalidChar);
                    //invalidChar += 6;
                }
                if (input[i] == 60 && input[i + 1] == 'm') // find Tag <m> ('<'= 60)
                {
                    highlightTagOpen.Add(i);
                    //highlightTagOpen.Add(i - invalidChar);
                    //invalidChar += 3;
                }
                if (i < input.Length - 2)
                    if (input[i] == 60 && input[i + 1] == '/' && input[i + 2] == 'm') // find Tag </m> ('<'= 60)
                    {
                        highlightTagClose.Add(i-2);
                        //highlightTagClose.Add(i - invalidChar);
                        //invalidChar += 4;
                    }
            }
            foreach (var word in input.Split(new string[] { " " }, StringSplitOptions.None))
            {

                if (word == "")
                    wordSplitList.Add(" ");

                else
                {
                    if(word.Contains("</m>"))
                        wordSplitList.RemoveRange(wordSplitList.Count - 1, 1);
                    wordSplitList.Add(word);
                    if(!word.Contains("<m>"))
                        wordSplitList.Add(" ");
                }
            }
            if (wordSplitList.Count > 1)
                wordSplitList.RemoveRange(wordSplitList.Count - 1, 1);
            reformWords(wordSplitList);
        }

        /// <summary>
        /// Seprate and Reform Persian and English Words : char by char
        /// 
        ///  بتدا چون حرفی از قبل وجود نداشته، چسبندگی حرف قبلی به حرف جاری را غیر چسبنده در نظر می گیریم
        /// U = Non Joining R=rightJoin L=LeftJoin D= Middle Join
        ///  حال به ازای تک تک حروف عملیات زیر را دنبال می کنیم
        ///  حرف جاری را در متغیر زیر می ریزیم
        ///  در صورتی که کد اسکی حرف از 127 بزرگتر باشد یعنی حرف غیر انگلیسی است؛ در غیر این صورت شرط درست است و وارد شرط می شویم
        ///  و چون حرف انگلیسی بوده، وضعیت چسبندگی غیر چسبنده خواهد بود
        ///  پس از آن وضعیت چسبندگی حرف جاری را مشخص می کنیم
        ///  ررسی می کنیم که آیا این حرف، آخرین حرف کلمه است ، در صورتی که بود، وارد شرط زیر می شویم
        /// حرف جاری را به صورت حرف پایانی در نظر می گیریم  
        ///  در صورتی که آخرین حرف کلمه نبودیم 
        ///  حرف بعدی را در متغیر زیر می ریزیم
        ///  var nextCharacter = wordOutPut[i + 1];
        /// از آنجایی که کاراکتر جاری، در اجرای بعدی و به ازای کاراکتر بعدی، کاراکتر قبلی محسوب می شود، حالات چسبندگی کاراکتر قبلی را مساوی با حالت چسبندگی همین کاراکتر در نظر می گیریم
        ///  preCharacterJoiningType = currentCharacterJoiningType;
        /// </summary>
        /// <param name="wordOutPut"></param>
        /// 
        private void reformWords(List<string> wordSplit)
        {
            wordList = new List<string>();
            for (int t = 0; t < wordSplit.Count; t++)
            {
                if (t > 0 && wordSplit[t] == " " && (wordSplit[t - 1] == " " || wordSplit[t - 1] == "&nbsp;"))
                {
                    wordList.Add("&nbsp;");
                    continue;
                }

                Boolean isNumberFlag = true;
                Boolean isPersian = false;
                persianChar = new List<char>();
                englishChar = new List<char>();
                var firstCharacter = wordSplit[t][0];
                var dir = (FlowDirection == FlowDirection.RightToLeft ? "rtl" : "ltr");
                var preCharacterJoiningType = ArabicShapeJoiningType.U;

                for (int j = 0; j < wordSplit[t].Length; j++)
                {
                    if (wordSplit[t][j] < 128)
                    {
                        isPersian = false;
                        break;
                    }
                    isPersian = true;
                }

                for (int i = 0; i < wordSplit[t].Length; i++)
                {

                    var currentCharacter = wordSplit[t][i];

                    if (currentCharacter < 128)
                    {
                        if (persianChar.Count != 0)
                        {
                            if (dir == "ltr" && isPersian)
                                persianChar.Reverse();
                            string persianWord = new string(persianChar.ToArray());
                            wordList.Add(persianWord);
                            persianChar.Clear();
                        }
                        englishChar.Add(currentCharacter);
                        preCharacterJoiningType = ArabicShapeJoiningType.U;
                    }

                    else
                    {
                        if (englishChar.Count != 0)
                        {

                            for (int j = 0; j < englishChar.Count; j++)
                            {
                                if (englishChar[j] <= 48 || englishChar[j] >= 58)
                                {
                                    isNumberFlag = false;
                                    break;
                                }
                            }
                            if (isNumberFlag && dir != "ltr")
                                englishChar.Reverse();

                            string englishWord = new string(englishChar.ToArray());
                            wordList.Add(englishWord);
                            englishChar.Clear();
                        }


                        var currentCharacterJoiningType = UnicodeArabicShapingResolver.GetArabicShapeJoiningType(currentCharacter);

                        if (preCharacterJoiningType == ArabicShapeJoiningType.D || preCharacterJoiningType == ArabicShapeJoiningType.L)
                        {
                            if (i == wordSplit[t].Length - 1)

                                persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Final));
                            else
                            {
                                var nextCharacter = wordSplit[t][i + 1];
                                if ((nextCharacter > 128) && (currentCharacterJoiningType == ArabicShapeJoiningType.D || currentCharacterJoiningType == ArabicShapeJoiningType.L))
                                    persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Medial));
                                else
                                    persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Final));

                            }
                        }
                        else
                        {
                            if (i == wordSplit[t].Length - 1)
                                persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Isolated));
                            else
                            {

                                var nextCharacter = wordSplit[t][i + 1];
                                if ((nextCharacter > 128) && (currentCharacterJoiningType == ArabicShapeJoiningType.D || currentCharacterJoiningType == ArabicShapeJoiningType.L))
                                    persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Initial));
                                else
                                    persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Isolated));

                            }
                        }
                        preCharacterJoiningType = currentCharacterJoiningType;
                    }

                }
                if (englishChar.Count != 0)
                {
                    //englishChar.Reverse();
                    for (int i = 0; i < englishChar.Count; i++)
                    {

                        if ((englishChar[i] <= 48 || englishChar[i] >= 58))
                        {
                            isNumberFlag = false;
                            break;
                        }

                    }
                    if (isNumberFlag && dir != "ltr")
                        englishChar.Reverse();
                    string englishWord = new string(englishChar.ToArray());
                    wordList.Add(englishWord);
                }

                if (persianChar.Count != 0)
                {
                    if (dir == "ltr" && isPersian)
                        persianChar.Reverse();
                    string persianWord = new string(persianChar.ToArray());
                    wordList.Add(persianWord);
                }

            }

        }
        /// <summary>
        /// Html Style and Header
        /// </summary>
        /// <returns></returns>        
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = this.ActualWidth;

            int maxLength = 0;
            if (!string.IsNullOrEmpty(this.HtmlContent))
            {

                BuildHead();
                //calculate siz of each char
                string text = String.Join(String.Empty, wordList.ToArray());
                int size = TextSize(text);
                maxLength = (int)width / size-offset ;

                if (maxLength < 1 || size <= 0)
                    maxLength = 1;
                this.body = TruncateLongString(text, maxLength, fakeSpace, highlightTagOpen, highlightTagClose);

                this.HtmlPanel.Text = BuildHtml();
            }
        }

        private static string TruncateLongString(string str, int charCount, List<int> fakeSpace, List<int> highlightTagOpen, List<int> highlightTagClose)
        {
            var highlightTagOpenCount = highlightTagOpen.Count(x => x < charCount) * 2;
            charCount += highlightTagOpenCount;
            var highlightTagCloseCount = highlightTagClose.Count(x => x < charCount) * 4;
            charCount += highlightTagCloseCount;
            int fakeSpaceCount = fakeSpace.Count(x => x < charCount) * 5;
            charCount += fakeSpaceCount;
            //for(int s=charCount; s>0; )
            //{
            if (str.Length > charCount && charCount > 1)
                if (str.Substring(charCount - 1, 1) == " ")
                {
                    charCount--;
                    //s--;
                }
            //    if (str.Length-6 > s)
            //    {
            //        if (str.Substring(s - 6, 6) == "&nbsp;")
            //        {
            //            charCount-=6;
            //            s-=6;
            //        }

            //        if (str.Substring(s - 6, 6) != "&nbsp;" && str.Substring(s - 1, 1) != " ")
            //            break;
            //    }

            //}
            if (str.Length <= charCount)
                    return str.Substring(0, Math.Min(str.Length, charCount));
                else
                    return str.Substring(0, Math.Min(str.Length, charCount)) + "...";

        }

        private int TextSize(string txt)
        {
            var res = Regex.Replace(txt, @"&nbsp;", " ");
            res = Regex.Replace(res, @"<m>|</m>", "");
            List<double> charSize = new List<double>();
            float widthSizeOfText = System.Windows.Forms.TextRenderer.MeasureText(res, new System.Drawing.Font(this.FontFamily.ToString(), (float)this.FontSize)).Width;
            FormattedText formatted = new FormattedText(res, CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight, new Typeface(this.FontFamily.ToString()), (float)this.FontSize, Brushes.Black);
            
            return (int)widthSizeOfText / res.Length;

        }

    }
}
