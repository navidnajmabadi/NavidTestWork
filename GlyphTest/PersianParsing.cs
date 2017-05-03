using NBidi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlyphTest
{
    public class PersianParsing
    {
        int count = 0;
        int index = 0;
        string Word;
        string nextWord = null;
        List<char> persianChar ;
        List<char> englishChar ;
        List<string> wordList ;
       
        List<string> Temp = new List<string>();
        





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
            persianChar = new List<char>();
            englishChar = new List<char>();
            var firstCharacter = wordOutPut[0];
   

            var preCharacterJoiningType = ArabicShapeJoiningType.U;

            for (int i = 0; i < wordOutPut.Length; i++)
            {

                var currentCharacter = wordOutPut[i];

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
                    preCharacterJoiningType = ArabicShapeJoiningType.U;
                }

                else
                {
                    if (englishChar.Count != 0)
                    {
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
            //return wordList;

        }

        /// <summary>
        /// Change tag => <b> something </b> to  
        /// <p dir = \"ltr\"><span  style="background-color:yellow;color:red;font-weight:bold;"> something </span></p>  
        /// اگر ورودی ما دارای تگ بود، چون باید همان ورودی به خروجی منتقل شود
        /// از آنجایی که در اچ تی ام ال، تگ های بزرگتر و کوچکتر دارای معنی خاص هستند، باید با رشته کد 
        ///&lt;
        /// جایگزین شود
        ///  در عین حال ، ما یک تگ خاص را برای هایلایت در نظر گرفته ایم که نباید به عنوان متن در نظر گرفته شود
        /// در اینجا تگ مد نظر ما تگ B است
        /// این تگ را دوباره به حالت اول باز میگردانیم
        /// </summary>
        /// <param name="wordList"></param>
        /// <returns></returns>
        public string ConvertTextToHtmlTag(List<string> wordList)
        {
            string output;
            output = String.Join(String.Empty, wordList.ToArray());
            output = output.Replace("<", "&lt;");
            output = output.Replace("&lt;b>", "<span style=\"background-color:yellow;color:red;font-weight:bold;\">");
            output = "<p dir=\"ltr\">" + output.Replace("&lt;/b>", "</span>") + "</p>";
            return output;
        }
        /// <summary>
        /// Get string and Seprate word by word and return List of Word
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
         public List<string> SeprateList(string input)
         {
            List<string> wordSplitList = new List<string>();
            foreach (var word in input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
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
            var wordSplitList = SeprateList(input);
            List<string>  OutPutWord = new List<string>();
            for (int j = 0; j < wordSplitList.Count; j++)
            {
                Word = wordSplitList[j];
                if (j < wordSplitList.Count - 1)
                {

                    if (wordSplitList[j + 1] == null)
                        nextWord = " ";
                    else
                        nextWord = wordSplitList[j + 1];
                }
                else
                    nextWord = " ";


                if (Word[0] < 128)
                {

                    count = 0;
                    OutPutWord.Add(wordSplitList[j]);

                }

                else
                {

                    if (count == 0 && (nextWord[0] > 128) && j != (wordSplitList.Count - 1))
                    {
                        index = j;
                    }
                    else if (count == 0)
                    {
                        OutPutWord.Add(wordSplitList[j]);
                    }
                    count++;

                }

                if (nextWord[0] < 128 && count > 1)
                {
                    for (int k = index; k < (count + index); k++)
                        Temp.Add(wordSplitList[k]);
                    Temp.Reverse();
                    for (int s = 0; s < Temp.Count; s++)
                    {
                        OutPutWord.Add(Temp[s]);
                    }

                    Temp.Clear();
                }
            }
            return OutPutWord;
        }



        public string Reform(string text)
        {
            wordList = new List<string>();
            var OutPutWord = PersianSortWordList(text);
            var separateWord = SeprateList(text);
            if (text.Length == 0)
                return null;
            string sentence = text;
            for (int t = 0; t < separateWord.Count; t++)
                    reformWords(OutPutWord[t]);
            return ConvertTextToHtmlTag(wordList);
        }
    }
}