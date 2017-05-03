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

        

        private void Button_Click()
        {
            List<DataClass> dataList = new List<DataClass>();
                 Random r = new Random();
            var d1 = DateTime.Now;
            PersianParsing obj = new PersianParsing();

            for (int i = 0; i < 1000 ; i++)
            {
                var dataClass = new DataClass();
                dataClass.Text = obj.Reform("ali simple text      space               test");
                dataClass.Text1 = obj.Reform($"<b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven <b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven  <b> فارxyسی </b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven");
                dataClass.Text2 = obj.Reform($"<b>فارxyسی</b> اول دوم سوم fo44uچهارr five    ششم seven هشeight88ت nineده یازدهeleven <b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven  <b> فارxyسی </b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven");
                dataClass.Text3 = obj.Reform($"<b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven     هشeight88ت nineده یازدهeleven <b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven  <b> فارxyسی </b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven");
                dataClass.Text4 = obj.Reform($"<b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven <b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven  <b> فارxyسی </b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven");
                dataClass.Text5 = obj.Reform($"<b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven      هشeight88ت nineده یازدهeleven <b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven  <b> فارxyسی </b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven");
                dataClass.Text6 = obj.Reform($"<b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven <b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven  <b> فارxyسی </b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven");
                dataClass.Text7 = obj.Reform($"<b>فارxyسی</b> اول دوم سوم fo44uچهارr five      ششم seven هشeight88ت nineده یازدهeleven <b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven  <b> فارxyسی </b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven");
                dataClass.Text8 = obj.Reform($"<b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven <b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven  <b> فارxyسی </b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven");
                dataClass.Text9 = obj.Reform($"<b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven <b>فارxyسی</b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven  <b> فارxyسی </b> اول دوم سوم fo44uچهارr five  ششم seven هشeight88ت nineده یازدهeleven");
                dataList.Add(dataClass);
            }
           
            htmlDataGrid.ItemsSource = dataList;
            var d2 = DateTime.Now;
           MessageBox.Show(d2.Subtract(d1).TotalMilliseconds+"");
        }
    }
}
