using NBidi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GlyphTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Button_Click();
        }
        public class DataClass
        {
            public string Text { get; set; }
            public string Text1 { get; set; }
            public string Text2 { get; set; }
            public string Text3 { get; set; }
            public string Text4 { get; set; }
            public string Text5 { get; set; }
            public string Text6 { get; set; }
            public string Text7 { get; set; }
            public string Text8 { get; set; }
            public string Text9 { get; set; }
        }

        private string GetOrdered(string input)
        {
            if (input.Length == 0)
                return "";

            string sentence = input;
            var glyphChars = new List<char>();

            Stack<char> asciiCharacterStack = new Stack<char>();

            //NBidi.UnicodeGeneralCategory.

            foreach (var word in sentence.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
            {
                //NBidi.BidiCharacterType.

                var preCharacterJoiningType = ArabicShapeJoiningType.U;

                for (int i = 0; i < word.Length; i++)
                {
                    var currentCharacter = word[i];
                    if (currentCharacter < 128)
                    {
                       // asciiCharacterStack.Push(currentCharacter);
                        preCharacterJoiningType = ArabicShapeJoiningType.U;
                    }
                    else
                    {
                        //while (asciiCharacterStack.Count > 0)
                        //{
                        //    glyphChars.Add(asciiCharacterStack.Pop());
                        //}
                        var currentCharacterJoiningType = UnicodeArabicShapingResolver.GetArabicShapeJoiningType(currentCharacter);

                        if (preCharacterJoiningType == ArabicShapeJoiningType.D || preCharacterJoiningType == ArabicShapeJoiningType.L)
                        {
                            if (i == word.Length - 1)
                            {
                                glyphChars.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Final));
                            }
                            else
                            {
                                var nextCharacter = word[i + 1];
                                if ((nextCharacter > 128) && (currentCharacterJoiningType == ArabicShapeJoiningType.D || currentCharacterJoiningType == ArabicShapeJoiningType.L))
                                    glyphChars.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Medial));
                                else
                                    glyphChars.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Final));
                            }
                        }
                        else
                        {
                            if (i == word.Length - 1)
                            {
                                glyphChars.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Isolated));
                            }
                            else
                            {
                                if (currentCharacterJoiningType == ArabicShapeJoiningType.D || currentCharacterJoiningType == ArabicShapeJoiningType.L)
                                    glyphChars.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Initial));
                                else
                                    glyphChars.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Initial));
                            }
                        }

                        preCharacterJoiningType = currentCharacterJoiningType;
                    }
                }

                //if (asciiCharacterStack.Count > 0)
                //    asciiCharacterStack.Push(' ');

                glyphChars.Add(' ');
            }

            //while (asciiCharacterStack.Count > 0)
            //{
            //    glyphChars.Add(asciiCharacterStack.Pop());
            //}

            //Stack<string> stackedWords = new Stack<string>();
            //var thisLineWords = "";
            //var lastSpaceIndex = 0;
            //var lineWidth = HtmlPanel.ActualWidth;
            //var fontSize = 11;

            //HtmlPanel.FontFamily = new FontFamily("tahoma");

            //var currentLineWidth = 0d;
            //var tempWord = "";

            //GlyphTypeface glyphTypeface = new GlyphTypeface(new Uri($@"C:\Windows\Fonts\segoeui.ttf"));

            //if (glyphTypeface != null)
            //{
            //    int i = 0;

            //    while (i < glyphChars.Count && glyphChars[i] == ' ')
            //        i++;

            //    lastSpaceIndex = i - 1;

            //    while (i < glyphChars.Count)
            //    {
            //        for (; i < glyphChars.Count; i++)
            //        {
            //            ushort glyph;
            //            if (!glyphTypeface.CharacterToGlyphMap.TryGetValue(glyphChars[i], out glyph))
            //                break;

            //            currentLineWidth += 96d / 72d * fontSize * glyphTypeface.AdvanceWidths[glyph];

            //            if (glyphChars[i] == ' ')
            //            {
            //                if (!string.IsNullOrEmpty(tempWord))
            //                    stackedWords.Push(tempWord);

            //                tempWord = "";
            //                lastSpaceIndex = i;
            //            }
            //            else
            //            {
            //                if (currentLineWidth > lineWidth)
            //                {
            //                    i = lastSpaceIndex + 1;
            //                    tempWord = "";
            //                    break;
            //                }
            //                else
            //                {
            //                    tempWord += glyphChars[i];
            //                }
            //            }
            //        }

            //        if (stackedWords.Count() > 0)
            //        {
            //            while (stackedWords.Count() > 0)
            //            {
            //                foreach (var ch in stackedWords.Pop().Reverse())
            //                {
            //                    thisLineWords += ch;
            //                }
            //                thisLineWords += " ";
            //            }
            //        }
            //        if (lastSpaceIndex + tempWord.Length == glyphChars.Count - 1)
            //            foreach (var ch in tempWord.Reverse())
            //            {
            //                thisLineWords += ch;
            //            }

            //        currentLineWidth = 0;
            //    }
            //}

            //var finalOutput = "";

            //foreach (var item in thisLineWords)
            //{
            //    finalOutput += item;
            //}

            var finalOutput = "";

            //foreach (var ch in glyphChars)
            //{
            //    finalOutput += ch.ToString();
            //}

            finalOutput = new string(glyphChars.ToArray());

            return finalOutput;
        }

        private void Button_Click()
        {
            List<DataClass> dataList = new List<DataClass>();
                 Random r = new Random();
            var d1 = DateTime.Now;


            for (int i = 0; i < 1000; i++)
            {
                var dataClass = new DataClass();

                //dataClass.Text3 = Reform($"در این قسمت یک. متن طولانی قرار داده شده است a + b = x");
                //dataClass.Text = $"<p>بابا آمد به خانه English 123 B و رفت</p>";
                //dataClass.Text = $"<p dir=\"rtl\">بابا به خانه home Water رفت و آمد می کند</p>";

                dataClass.Text = Reform($"<b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjاا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا");
                dataClass.Text1 = Reform($"<b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا");
                dataClass.Text2 = Reform($"<b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا");
                dataClass.Text3 = Reform($"<b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا");
                dataClass.Text4 = Reform($"<b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا");
                dataClass.Text5 = Reform($"<b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا");
                dataClass.Text6 = Reform($"<b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا");
                dataClass.Text7 = Reform($"<b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا");
                dataClass.Text8 = Reform($"<b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا");
                dataClass.Text9 = Reform($"<b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا <b>فارxyسی</b>متن کاملا tلtt eee  تکست rrr جدjjا");

                //dataClass.Text1 = Reform($"نوید naببjmabadi نجم آبffادی najm abadi <b> نو </b>");
                //dataClass.Text2 = Reform($"نوید naببjmabadi نجم آبffادی najm abadi <b> نو </b>");
                //dataClass.Text3 = Reform($"this tex hfdksjhkdsh ابتنیابنست  یباسین I<b> تالیبتسلتبلسی   تبلسیتلسیبت </b> تانیباسنیتبا11");
                //dataClass.Text4 = Reform($"نوید najmabadi نجم آبادی najm abadi </b> نو <b>");
                //dataClass.Text5 = Reform($"نوید naببjmabadi نجم آبffادی najm abadi </b> نو <b>");
                //dataClass.Text6 = Reform($"this tex hfdksjhkdsh ابتنیابنستیباسین I<b> تالیبتسلتبلس  یتبلسیتلسیبت </b> تانیباسنیتبا11");
                //dataClass.Text7 = Reform($"this tex hfdksjhkdsh ابتنیابن  ستیباسین I<b> تالیبتسلتبلسیتبل    سیتلسیبت </b> تانیباسنیتبا11");
                //dataClass.Text8 = Reform($"this tex hfdksjhkdsh ابتنیابن  ستیباسین I<b> تالیبتسلتبل   سیتبلسیتلسیبت </b> تانیباسنیتبا11");
                //dataClass.Text9 = Reform($"this tex hfdksjhkdsh ابتنیابنستیباسین I<b> تالیبتسلتبلسیتبلسیتلسیبت </b> تانیباسنیتبا11");

                dataList.Add(dataClass);
            }

            

            htmlDataGrid.ItemsSource = dataList;


            var d2 = DateTime.Now;

           MessageBox.Show(d2.Subtract(d1).TotalMilliseconds+"");

        }






        // با استفاده از متد زیر، عملیات فارسی سازی متن را انجام می دهیم
        private string Reform(string input)
        {
            // اگر متن شامل یک رشته خالی بود، خروجی ما نال خواهد بود
            if (input.Length == 0)
                return null;

            int i = 0;

            string sentence = input;
            // یک لیست از کاراکترها را در زیر تعریف می کنیم؛ این لیست باید حروف مد نظر ما را در خود جای دهد
            // نتیجه نهایی از این آرایه حاصل می شود
            var glyphChars = new List<char>();
            List<char> persianChar =null;
            List<char> englishChar = null;
            var wordList = new List<string>();
            var wordListRead = new List<string>();
            // استک زیر برای حروف انگلیسی استفاده می شود؛ به این ترتیب که حروف انگلیسی را در پشته میریزیم تا زمانی که یک حرف فارسی پیدا شود؛ به محض پیدا شدن حرف فارسی، تمامی حروف موجود در پشته را 
            Stack<char> asciiCharacterStack = new Stack<char>();

            // به ازای تک تک کلمات، فرآیند زیر را تکرار می کنیم
            foreach (var word in sentence.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
            {
                wordListRead.Add(word);
                var firstCharacter = word[0];
                persianChar = new List<char>();
                englishChar = new List<char>();
                // ابتدا چون حرفی از قبل وجود نداشته، چسبندگی حرف قبلی به حرف جاری را غیر چسبنده در نظر می گیریم
                // U = Non Joining
                var preCharacterJoiningType = ArabicShapeJoiningType.U;
                //  حال به ازای تک تک حروف عملیات زیر را دنبال می کنیم
                for (i = 0; i < word.Length; i++)
                {
                    // حرف جاری را در متغیر زیر می ریزیم
                    var currentCharacter = word[i];

                    // در صورتی که کد اسکی حرف از 127 بزرگتر باشد یعنی حرف غیر انگلیسی است؛ در غیر این صورت شرط درست است و وارد شرط می شویم
                    if (currentCharacter < 128)
                    {
                        if (persianChar.Count != 0)
                        {
                            persianChar.Reverse();
                            string persianWord = new string(persianChar.ToArray());
                            wordList.Add(persianWord);
                            persianChar.Clear();
                        }
                        englishChar.Add(currentCharacter);
                        // و چون حرف انگلیسی بوده، وضعیت چسبندگی غیر چسبنده خواهد بود
                        preCharacterJoiningType = ArabicShapeJoiningType.U;
                    }
                    // در صورتی که حرف غیر انگلیسی باشد، وارد دستورات زیر می شویم
                    else
                    {
                        if (englishChar.Count != 0)
                        {
                            //englishChar.Reverse();
                            string englishWord = new string(englishChar.ToArray());
                            wordList.Add(englishWord);
                            englishChar.Clear();
                        }

                        // پس از آن وضعیت چسبندگی حرف جاری را مشخص می کنیم
                        var currentCharacterJoiningType = UnicodeArabicShapingResolver.GetArabicShapeJoiningType(currentCharacter);

                        // در صورتی که حرف قبلی به حرف بعدی می چسبید، شرط پایین درست بوده و وارد شرط می شویم
                        if (preCharacterJoiningType == ArabicShapeJoiningType.D || preCharacterJoiningType == ArabicShapeJoiningType.L)
                        {
                            // بررسی می کنیم که آیا این حرف، آخرین حرف کلمه است ، در صورتی که بود، وارد شرط زیر می شویم
                            if (i == word.Length - 1)
                                // حرف جاری را به صورت حرف پایانی در نظر می گیریم
                                persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Final));
                            // در صورتی که آخرین حرف کلمه نبودیم
                            else
                            {
                                // حرف بعدی را در متغیر زیر می ریزیم
                                var nextCharacter = word[i + 1];
                                // در صورتی که حرف بعدی حرف غیر انگلیسی بود و همچنین حرف جاری به بعدی می چسبید، کاراکتر جاری را به صورت حرف وسط در نظر می گیریم
                                if ((nextCharacter > 128) && (currentCharacterJoiningType == ArabicShapeJoiningType.D || currentCharacterJoiningType == ArabicShapeJoiningType.L))
                                    persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Medial));
                                // در غیر این صورت، کاراکتر جاری را به صورت حالت پایانی در نظر می گیریم
                                else
                                    persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Final));

                            }
                        }
                        // قسمت پایین مربوط به زمانی است که حرف قبلی به حرف بعدی نمی چسبید
                        else
                        {
                            // اگر حرف آخر بودیم
                            if (i == word.Length - 1)
                                // کاراکتر را به صورت غیرچسبان آخر در نظر می گیریم
                                persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Isolated));

                            // اگر حرف آخر نبودیم
                            else
                            {
                                // ابتدا کاراکتر بعدی را در متغیر زیر می ریزیم
                                var nextCharacter = word[i + 1];
                                // در صورتی که حرف بعدی حرف غیر انگلیسی بود و همچنین حرف جاری به بعدی می چسبید، کاراکتر جاری را به صورت حرف آغازین در نظر می گیریم
                                if ((nextCharacter > 128) && (currentCharacterJoiningType == ArabicShapeJoiningType.D || currentCharacterJoiningType == ArabicShapeJoiningType.L))
                                    persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Initial));

                                // در غیر این صورت، کاراکتر جاری را به صورت غیرچسبان آخر در نظر می گیریم
                                else
                                    persianChar.Add(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(currentCharacter, LetterForm.Isolated));

                            }
                        }

                        // از آنجایی که کاراکتر جاری، در اجرای بعدی و به ازای کاراکتر بعدی، کاراکتر قبلی محسوب می شود، حالات چسبندگی کاراکتر قبلی را مساوی با حالت چسبندگی همین کاراکتر در نظر می گیریم
                        preCharacterJoiningType = currentCharacterJoiningType;
                    }

                }
            
                if (englishChar.Count != 0)
                {
                   // englishChar.Reverse();
                    string englishWord = new string(englishChar.ToArray());
                    wordList.Add(englishWord);
                }

                if (persianChar.Count != 0)
                {
                    persianChar.Reverse();
                    string persianWord = new string(persianChar.ToArray());
                    wordList.Add(persianWord);
                }
                wordList.Add(" ");
            }





            string Word;
            string nextWord =null;
            int count = 0;
            int index = 0;
            List<string> Temp = new List<string>();
            List<string> OutPutWord = new List<string>();
            for (int j = 0; j < wordList.Count; j++)
            {
                if (wordList[j] == " ")
                {
                    OutPutWord.Add(" ");
                    continue;
                }
                Word = wordList[j];
                if (j != wordList.Count - 1)
                {
                    nextWord = wordList[j + 1];
                   
                }
                   
                
                if (Word[0] < 128 )
                {
                    count = 0;
                    OutPutWord.Add(wordList[j]);
                    
                }

                else
                {
                    if (count == 0 && !(nextWord[0] < 128 && nextWord[0]!=32) && j!= (wordList.Count - 1))
                    {
                        index = j; 
                    }
                    else if(count==0)
                    {
                        OutPutWord.Add(wordList[j]);    
                    }
                    count++;

                }
                if (count > 1)
                {
                    for (int k = index; k < (count+index) ; k++)
                        Temp.Add(wordList[k]);
                    Temp.Reverse();
                    for (int s = 0; s<Temp.Count ; s++)
                    {
                        OutPutWord.Add(Temp[s]);
                    }
                        
                    Temp.Clear();
                }
            }

            input  = String.Join(String.Empty, OutPutWord.ToArray());

            // اگر ورودی ما دارای تگ بود، چون باید همان ورودی به خروجی منتقل شود
            // از آنجایی که در اچ تی ام ال، تگ های بزرگتر و کوچکتر دارای معنی خاص هستند، باید با رشته کد 
            //&lt;
            // جایگزین شود

            input = input.Replace("<", "&lt;");
            // در عین حال ، ما یک تگ خاص را برای هایلایت در نظر گرفته ایم که نباید به عنوان متن در نظر گرفته شود
            // در اینجا تگ مد نظر ما تگ B است
            // این تگ را دوباره به حالت اول باز میگردانیم
            input = input.Replace("&lt;b>", "<span style=\"background-color:yellow;color:red;font-weight:bold; \">");
            // یک تگ پی به عنوان تگ کلی در نظر می گیریم و خروجی را در آن می ریزیم
            input = "<p dir=\"ltr\">" +  input.Replace("&lt;/b>", "</span>") + "</p>";

            return input;
        }

    }
}
