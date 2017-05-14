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

        }

        

        private void Button_Click()
        {
            List<DataClass> dataList = new List<DataClass>();
                 Random r = new Random();
            ParsString obj = new ParsString();
            var d1 = DateTime.Now;
            

            for (int i = 0; i < 1000 ; i++)
            {
                var dataClass = new DataClass();
                //dataClass.Text = obj.HtmlStringParsing(" navid najmabadi is test lorem ipus hjhj dhkjeh jkrhe");
                dataClass.Text = obj.HtmlStringParsing(" تنمبتسینم بتسینمتب تمنبیتسمن خعح  کنبکمیسنبکسی هخحهقصندمبئس تهخستبنمس");
                //dataClass.Text = obj.HtmlStringParsing("english text sample text high character text");
                //dataClass.Text = obj.HtmlStringParsing("ENGLISH TEXT SAMPLE TEXT HIGH CHARACTER TEXT");
                dataList.Add(dataClass);
            }
           
            htmlDataGrid.ItemsSource = dataList;
            var d2 = DateTime.Now;
           // MessageBox.Show(d2.Subtract(d1).TotalMilliseconds + "");
        }

        
    }
}
