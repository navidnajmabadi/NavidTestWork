using System;
using System.Collections.Generic;
using System.Windows;


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

        

        private void Button_Click()
        {
            List<DataClass> dataList = new List<DataClass>();
                 Random r = new Random();
            ParsString obj = new ParsString();
            var d1 = DateTime.Now;
            

            for (int i = 0; i < 10 ; i++)
            {
                var dataClass = new DataClass();
                //dataClass.Text = obj.HtmlStringParsing("one <mhstr123tag>t</mhstr123tag>wo <mhstr123tag>t</mhstr123tag>ree four five six seven eigh<mhstr123tag>t</mhstr123tag> nine <mhstr123tag>t</mhstr123tag>en one <mhstr123tag>t</mhstr123tag>wo tree four five six seven eight nine ten one two tree four five six seven eight nine ten one two tree four five six seven eight nine ten one two tree four five six seven eight nine ten");//obj.HtmlStringParsing( " <b>text</b>");
                dataClass.Text = obj.HtmlStringParsing("english <mhstr123tag>text</mhstr123tag> sample <mhstr123tag>text</mhstr123tag> high character text");
                dataList.Add(dataClass);
            }
           
            htmlDataGrid.ItemsSource = dataList;
            var d2 = DateTime.Now;
            //MessageBox.Show(d2.Subtract(d1).TotalMilliseconds + "");
        }

        
    }
}
