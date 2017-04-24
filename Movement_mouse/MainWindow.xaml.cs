using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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


namespace Movement_mouse
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Node> nodes = new ObservableCollection<Node>();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Line line = new Line();
            //Ellipse currentDot = new Ellipse();
            Point p1 = e.GetPosition(this);
            //currentDot.Height = 7;
            //currentDot.Width = 7;
            //currentDot.Fill = new SolidColorBrush(Colors.Black);
            //currentDot.Margin = new Thickness((p1.X) - 3, (p1.Y) - 3, -1, -1);
            //MyCanvas.Children.Add(currentDot);
            line = new Line();
            line.Stroke = Brushes.Red;
            this.DataContext = this;
            line.X1 = 50;
            line.X2 = 12; 
            line.Y1 = 50;
            line.Y2 = 50;

            line.StrokeThickness = 2;
            MyCanvas.Children.Add(line);

        }
    }
    public class Node : INotifyPropertyChanged
    {
        private string postion;
        public string PostionName
        {
            get { return this.postion; }
            set
            {
                if (this.postion != value)
                {
                    this.postion = value;
                    this.NotifyPropertyChanged("Postion");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}





