using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyWordleLib;

namespace MyWordle
{
    /// <summary>
    /// Interaction logic for MyWordleMain.xaml
    /// </summary>
    public partial class MyWordleMain : Window, INotifyPropertyChanged
    {
        public const char KbdCharEnter = '\n';
        public const char KbdCharBackSpace = '\\'; // can be any char to represent backspace
        public event PropertyChangedEventHandler? PropertyChanged;
        void RaisePropChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public int TargetWordLength { get; set; } = 5;
        public bool UseLargeDictionary { get; set; } // Large == 1, Small ==2
        public int NumGuessesAllowed { get; set; } = 6;
        private int _NumWordsPossible;
        public int NumWordsPossible { get { return _NumWordsPossible; } set { _NumWordsPossible = value; RaisePropChanged(); } }
        public MyWordleMain()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Height = 600;
            this.Width = 600;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hashWords = MyWordleLib.MyWordleLib.GetWords(
                    (UseLargeDictionary ? DictionaryLib.DictionaryType.Large : DictionaryLib.DictionaryType.Small),
                    DesiredWordLen: TargetWordLength);
                //small:  5 LTR WORDS = 3425, 2329 uniq.   Large 9908, 6406 uniq
                NumWordsPossible = hashWords.Count;
                UniGrid.Children.Clear();
                UniGrid.Rows = NumGuessesAllowed;
                UniGrid.Columns = TargetWordLength;
                for (int iRow = 0; iRow < UniGrid.Rows; iRow++)
                {
                    for (int iCol = 0; iCol < UniGrid.Columns; iCol++)
                    {
                        UniGrid.Children.Add(new GuessTile(iRow, iCol));
                    }
                }
                UCKbdLayout.Content = new KbdLayout();
            }
            catch (Exception)
            {
            }
        }
    }

    internal class KbdLayout : UserControl
    {
        public KbdLayout()
        {
            var spVert = new StackPanel() { Orientation = Orientation.Vertical };
            this.AddChild(spVert);
            var spTopRow = new StackPanel() { Orientation = Orientation.Horizontal };
            var spMidRow = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(10) };
            var spBotRow = new StackPanel() { Orientation = Orientation.Horizontal };
            spVert.Children.Add(spTopRow);
            spVert.Children.Add(spMidRow);
            spVert.Children.Add(spBotRow);
            foreach (var chr in "qwertyuiop")
            {
                spTopRow.Children.Add(new KbdKey(chr));
            }
            foreach (var chr in "asdfghjkl")
            {
                spMidRow.Children.Add(new KbdKey(chr));
            }
            spBotRow.Children.Add(new KbdKey(MyWordleMain.KbdCharEnter));
            foreach (var chr in "zxcvbnm")
            {
                spBotRow.Children.Add(new KbdKey(chr));
            }
            spBotRow.Children.Add(new KbdKey(MyWordleMain.KbdCharBackSpace));
        }
    }

    internal class KbdKey : BaseTile
    {
        private readonly char chr;

        public KbdKey(char chr)
        {
            this.chr = chr;
            this.Width = 35;
            this.Height = 55;
            switch (chr)
            {
                case MyWordleMain.KbdCharBackSpace:
                    this.Width *= 2;
                    this.txtBlock.Text = "<--";
                    break;
                case MyWordleMain.KbdCharEnter:
                    this.Width *= 2;
                    this.txtBlock.Text = "Enter";
                    break;
                default:
                    this.txtBlock.Text = chr.ToString();
                    break;
            }
            this.SetBackground(Brushes.LightGray);
        }
    }
    internal class GuessTile : BaseTile
    {
        private readonly int iRow;
        private readonly int iCol;
        public GuessTile(int iRow, int iCol)
        {
            this.iRow = iRow;
            this.iCol = iCol;
            this.SetBackground(Brushes.White);
        }
    }

    internal class BaseTile : DockPanel
    {
        internal readonly TextBlock txtBlock = new();
        internal readonly Border brd;
        public BaseTile()
        {
            this.Margin = new Thickness(1, 1, 1, 1);
            //            Background = Brushes.LightBlue;
            brd = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(2)
            };
            brd.Child = txtBlock;
            //            brd.Background = Brushes.LightGreen;
            //            txtBlock.Background = Brushes.LightGreen;
            this.Children.Add(brd);
            txtBlock.FontSize = 20;
            txtBlock.HorizontalAlignment = HorizontalAlignment.Center;
            txtBlock.VerticalAlignment = VerticalAlignment.Center;
        }
        public void SetBackground(Brush brush)
        {
            brd.Background = brush;
        }
    }
}
