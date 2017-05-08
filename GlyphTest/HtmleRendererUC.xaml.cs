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
            if (!string.IsNullOrEmpty(this.HtmlContent))
            {
                SeprateList(this.HtmlContent);
                this.body = String.Join(String.Empty, wordList.ToArray());
            }
        }

        private void BuildHead()
        {
            double width = this.ActualWidth;
            
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
                 mhstr123tag{{
                     background-color:yellow; 
                     font-weight: bold;
                 }}
                 body{{
                        font-family: ""B Nazanin""; 
                        text-align: {align};
                        direction : {dir};
                        padding : 0px;
                        
                        margin-{align}:-1px;
                        margin-top : -3px;
                        float :{align};
                    }}
                .div2 {{
                    white-space: nowrap; 
                    width:{width}px; 
                    
                    text-overflow: ellipsis; 
                }}
	
                  
              </style>

              </head>";

            head= headHtml;
        }


        private string BuildHtml()
        {
            return $@"<html>{head}<body><div class=""div2"">{body}</div></body></html>";
        }

        /// <summary>
        /// Get string and Seprate word by word and return List of Word
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private void SeprateList(string input)
        {

            List<char> tempChar = new List<char>();
            List<string> wordSplitList = new List<string>();
            foreach (var word in input.Split(new string[] { " " }, StringSplitOptions.None))
            {

                if (word == "")
                    wordSplitList.Add(" ");

                else
                    wordSplitList.Add(word);

            }
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
                            if(dir=="ltr" && isPersian)
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
                    for(int i = 0; i<englishChar.Count; i++)
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
                wordList.Add("&nbsp;");
            }
            
        }
        /// <summary>
        /// Html Style and Header
        /// </summary>
        /// <returns></returns>        
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.HtmlContent))
            {
                BuildHead();
                this.HtmlPanel.Text = BuildHtml();
            }
        }
    }
}
