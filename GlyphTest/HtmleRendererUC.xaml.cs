using NBidi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            List<string> Temp = new List<string>();
            
            public string HtmlContent
            {
                get { return (string)GetValue(ExcelVisiblityProperty); }
                set { SetValue(ExcelVisiblityProperty, value); }
            }
            // Using a DependencyProperty as the backing store for ExcelVisiblity.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty ExcelVisiblityProperty =
                DependencyProperty.Register("HtmlContent", typeof(string), typeof(HtmleRendererUC), new PropertyMetadata(null, asdasd));
              private static void asdasd(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var control = (d as HtmleRendererUC);
                //control.Text = control.Reform(control.HtmlContent ?? "");
                control.HtmlPanel.Text = control.Reform(control.HtmlContent ?? "");
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
            public void reformWords(string wordOutPut)
            {
                Boolean isNumberFlag = true;
                persianChar = new List<char>();
                englishChar = new List<char>();
                var firstCharacter = wordOutPut[0];
                var dir = (FlowDirection == FlowDirection.RightToLeft ? "rtl" : "ltr");

                var preCharacterJoiningType = ArabicShapeJoiningType.U;

                for (int i = 0; i < wordOutPut.Length; i++)
                {

                    var currentCharacter = wordOutPut[i];

                    if (currentCharacter < 128)
                    {
                        if (persianChar.Count != 0)
                        {
                            if(dir=="ltr")
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
                        if (isNumberFlag)
                            englishChar.Reverse();

                        string englishWord = new string(englishChar.ToArray());
                            wordList.Add(englishWord);
                            englishChar.Clear();
                        }


                        var currentCharacterJoiningType = UnicodeArabicShapingResolver.GetArabicShapeJoiningType(currentCharacter);

                        if (preCharacterJoiningType == ArabicShapeJoiningType.D || preCharacterJoiningType == ArabicShapeJoiningType.L)
                        {
                            if (i == wordOutPut.Length - 1)

                                persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Final));
                            else
                            {
                                var nextCharacter = wordOutPut[i + 1];
                                if ((nextCharacter > 128) && (currentCharacterJoiningType == ArabicShapeJoiningType.D || currentCharacterJoiningType == ArabicShapeJoiningType.L))
                                    persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Medial));
                                else
                                    persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Final));

                            }
                        }
                        else
                        {
                            if (i == wordOutPut.Length - 1)
                                persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Isolated));
                            else
                            {

                                var nextCharacter = wordOutPut[i + 1];
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
                    for(int i = 0; i<englishChar.Count; i++)
                    {
                        
                        if ((englishChar[i] <= 48 || englishChar[i] >= 58))
                        {
                            isNumberFlag = false;
                            break;
                        }
                            
                }
                if (isNumberFlag)
                    englishChar.Reverse();
                    string englishWord = new string(englishChar.ToArray());
                    wordList.Add(englishWord);
                }

                if (persianChar.Count != 0)
                {
                if (dir == "ltr")
                    persianChar.Reverse();
                string persianWord = new string(persianChar.ToArray());
                    wordList.Add(persianWord);
                }

                wordList.Add("&nbsp;");
         

            //return wordList;

        }
            /// <summary>
            /// put in Html Header
            /// </summary>
            /// <param name="wordList"></param>
            /// <returns></returns>
            public string ConvertTextToHtmlTag(List<string> wordList)
            {
                string output;
                //wordList.Reverse();
                output = String.Join(String.Empty, wordList.ToArray());
            //return $@"<html dir=""{dir}"">{HtmlHead()}<body><div class=""Myclass"">{output}</div></body></html>";
                string Out = $@"<html>{HtmlHead()}<body>{output}</body></html>";
                return Out;

            }
            /// <summary>
            /// Get string and Seprate word by word and return List of Word
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public List<string> SeprateList(string input)
            {

                List<char> tempChar = new List<char>() ;
                string[] tagWordLast;
                List<string> wordSplitList = new List<string>();
                foreach (var word in input.Split(new string[] { " " }, StringSplitOptions.None))
                {
                    
                    if (word == "")
                        wordSplitList.Add(" ");
                     
                    else
                        wordSplitList.Add(word);

                }
                return wordSplitList;
            }
            public List<string> PersianSortWordList(string input)
            {
                int count = 0;
                Boolean isPersain = false;
                var wordSplitList = SeprateList(input);
                List<string> OutPutWord = new List<string>();
                if (wordSplitList.Count == 1)
                {
                    OutPutWord.Add(wordSplitList[0]);
                    return OutPutWord;
                }

                for (int j = 0; j < wordSplitList.Count; j++)
                {
                    //Word = wordSplitList[j];
                    //if (j < wordSplitList.Count - 1)
                    //{

                    //    if (wordSplitList[j + 1] == null)
                    //        nextWord = " ";
                    //    else
                    //        nextWord = wordSplitList[j + 1];
                    //}
                    //else
                    //    nextWord = "$"; // end of the text


                    //if (Word[0] < 128 && !(isPersain && Word[0]==32))
                    //{
                    //    isPersain = false;
                    //    count = 0;
                    OutPutWord.Add(wordSplitList[j]);

                    //    }

                    //    else
                    //    {
                    //        isPersain = true;
                    //        if (count == 0 && (nextWord[0] > 128 || (isPersain && nextWord[0] == 32) ) && j != (wordSplitList.Count - 1))
                    //        {
                    //            index = j;
                    //        }
                    //        else if (count == 0)
                    //        {
                    //            OutPutWord.Add(wordSplitList[j]);
                    //        }
                    //        count++;

                    //    }

                    //    if ((nextWord[0] < 128 && !(isPersain && nextWord[0] == 32)) && count > 1 )
                    //    {
                    //        for (int k = index; k < (count + index); k++)
                    //            Temp.Add(wordSplitList[k]);
                    //        Temp.Reverse();
                    //        for (int s = 0; s < Temp.Count; s++)
                    //        {
                    //            OutPutWord.Add(Temp[s]);
                    //        }

                    //        Temp.Clear();
                    //    }
                }
                return OutPutWord;
            }
            public string Reform(string text)
            {
                wordList = new List<string>();
                var OutPutWord = PersianSortWordList(text);
                // var separateWord = SeprateList(text);
                if (text.Length == 0)
                    return null;
                string sentence = text;
                for (int t = 0; t < OutPutWord.Count; t++)
                    reformWords(OutPutWord[t]);

                return ConvertTextToHtmlTag(wordList);
            }
            /// <summary>
            /// Html Style and Header
            /// </summary>
            /// <returns></returns>
            private string HtmlHead()
            {

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
                 b{{
                     background-color:yellow;   
margin-top : -30px;
                 }}
                 body{{
                        font-family: ""B Nazanin""; 
                        text-align: {align};
                        direction : {dir};
                        padding : 0px;
                        
                        margin-right:-3px;
                        margin-top : -3px;
                        float :{align};
                 }}   
                  
              </style>
              </head>";

                return headHtml;
            }
        
    }
}
