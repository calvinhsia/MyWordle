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
using Accessibility;
using MyWordleLib;

namespace MyWordle
{
    public enum GuessState
    {
        Empty, // white
        NotVerifiedYet,
        Wrong, // grey
        RightLetterWrongPosition, // yellow
        RightLetter,    // green
    }
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
        public Random _random = new(0);
        public int TargetWordLength { get; set; } = 5;
        public bool UseLargeDictionary { get; set; } // Large == 1, Small ==2
        public int NumGuessesAllowed { get; set; } = 6;
        private int _NumWordsPossible;
        public HashSet<string> _hashWords;

        public int NumWordsPossible { get { return _NumWordsPossible; } set { _NumWordsPossible = value; RaisePropChanged(); } }

        internal string _TargetWord; // the word the user is trying to guess
        internal GuessTile[,] _ArrGuessTiles;
        internal Dictionary<char, KbdKeyTile> _DictKbdTiles = new();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MyWordleMain()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            InitializeComponent();
            this.DataContext = this;
            this.Height = 600;
            this.Width = 600;
            this.Loaded += (o, e) =>
              {
                  BtnGo.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
              };
        }
        private void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _hashWords = MyWordleLib.MyWordleLib.GetWords(
                    (UseLargeDictionary ? DictionaryLib.DictionaryType.Large : DictionaryLib.DictionaryType.Small),
                    DesiredWordLen: TargetWordLength, ToUpper: true);

                //small:  5 LTR WORDS = 3425, 2329 uniq.   Large 9908, 6406 uniq
                NumWordsPossible = _hashWords.Count;
                _TargetWord = _hashWords.ElementAt(_random.Next(_hashWords.Count));
                UniGrid.Children.Clear();
                UniGrid.Rows = NumGuessesAllowed;
                UniGrid.Columns = TargetWordLength;
                _ArrGuessTiles = new GuessTile[NumGuessesAllowed, TargetWordLength];
                for (int iRow = 0; iRow < UniGrid.Rows; iRow++)
                {
                    for (int iCol = 0; iCol < UniGrid.Columns; iCol++)
                    {
                        var guessTile = new GuessTile(iRow, iCol, this);
                        UniGrid.Children.Add(guessTile);
                        _ArrGuessTiles[iRow, iCol] = guessTile;
                    }
                }
                UCKbdLayout.Content = new KbdLayout(this);
            }
            catch (Exception)
            {
            }
        }
        public (int iRow, int iCol) CurrentRowCol()
        {
            for (int iRow = 0; iRow < NumGuessesAllowed; iRow++)
            {
                for (int iCol = 0; iCol < TargetWordLength; iCol++)
                {
                    if (_ArrGuessTiles[iRow, iCol].IsEmpty())
                    {
                        return (iRow, iCol);
                    }
                }
            }
            return (0, 0);
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            switch (e.Key)
            {
                case Key.A:
                case Key.B:
                case Key.C:
                case Key.D:
                case Key.E:
                case Key.F:
                case Key.G:
                case Key.H:
                case Key.I:
                case Key.J:
                case Key.K:
                case Key.L:
                case Key.M:
                case Key.N:
                case Key.O:
                case Key.P:
                case Key.Q:
                case Key.R:
                case Key.S:
                case Key.T:
                case Key.U:
                case Key.V:
                case Key.W:
                case Key.X:
                case Key.Y:
                case Key.Z:
                    DoKeystroke(e.Key.ToString()[0]); // send 1st char of string as char
                    e.Handled = true;
                    break;
                case Key.Enter:
                    DoKeystroke(MyWordleMain.KbdCharEnter);
                    e.Handled = true;
                    break;
                case Key.Back:
                    DoKeystroke(MyWordleMain.KbdCharBackSpace);
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        internal void DoKeystroke(char c)
        {
            var (iRow, iCol) = CurrentRowCol();
            switch (c)
            {
                case MyWordleMain.KbdCharBackSpace: // send to prior tile
                case MyWordleMain.KbdCharEnter:
                    if (iCol == 0 && iRow == 0 && _ArrGuessTiles[iRow, iCol]._GuessState != GuessState.Empty) // entire grid is full
                    {
                        iCol = TargetWordLength - 1;
                        iRow = NumGuessesAllowed - 1;
                    }
                    else
                    {
                        if (iCol > 0)
                        {
                            iCol--;
                        }
                        else if (iRow > 0)
                        {
                            iRow--;
                            iCol = TargetWordLength - 1;
                        }
                    }
                    break;
            }
            _ArrGuessTiles[iRow, iCol].OnGotChar(c);
        }

    }

    internal class KbdLayout : UserControl
    {
        public KbdLayout(MyWordleMain myWordleMain)
        {
            var spVert = new StackPanel() { Orientation = Orientation.Vertical };
            this.AddChild(spVert);
            var spTopRow = new StackPanel() { Orientation = Orientation.Horizontal };
            var spMidRow = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(10) };
            var spBotRow = new StackPanel() { Orientation = Orientation.Horizontal };
            spVert.Children.Add(spTopRow);
            spVert.Children.Add(spMidRow);
            spVert.Children.Add(spBotRow);
            myWordleMain._DictKbdTiles.Clear();
            foreach (var chr in "QWERTYUIOP")
            {
                spTopRow.Children.Add(new KbdKeyTile(chr, myWordleMain));
            }
            foreach (var chr in "ASDFGHJKL")
            {
                spMidRow.Children.Add(new KbdKeyTile(chr, myWordleMain));
            }
            spBotRow.Children.Add(new KbdKeyTile(MyWordleMain.KbdCharEnter, myWordleMain));
            foreach (var chr in "ZXCVBNM")
            {
                spBotRow.Children.Add(new KbdKeyTile(chr, myWordleMain));
            }
            spBotRow.Children.Add(new KbdKeyTile(MyWordleMain.KbdCharBackSpace, myWordleMain));
        }
    }

    internal class KbdKeyTile : BaseTile
    {
        private readonly char _chr;
        private readonly MyWordleMain _MyWordleMain;

        public KbdKeyTile(char chr, MyWordleMain myWordleMain)
        {
            this._chr = chr;
            this._MyWordleMain = myWordleMain;
            myWordleMain._DictKbdTiles[chr] = this;
            this.Width = 35;
            this.Height = 55;
            switch (chr)
            {
                case MyWordleMain.KbdCharBackSpace:
                    this.Width *= 2;
                    this._txtBlock.Text = "<--";
                    break;
                case MyWordleMain.KbdCharEnter:
                    this.Width *= 2;
                    this._txtBlock.Text = "Enter";
                    break;
                default:
                    this._txtBlock.Text = chr.ToString();
                    break;
            }
            this.SetBackground(Brushes.LightGray);
            _border.MouseDown += (o, e) =>
              {
                  _MyWordleMain.DoKeystroke(_chr);
              };
        }
        public override string ToString() => $"{_chr}";
    }
    internal class GuessTile : BaseTile
    {
        private readonly int _iRow;
        private readonly int _iCol;
        private readonly MyWordleMain _myWordleMain;

        public GuessTile(int iRow, int iCol, MyWordleMain myWordleMain)
        {
            this._iRow = iRow;
            this._iCol = iCol;
            this._myWordleMain = myWordleMain;
            this._GuessState = GuessState.Empty;
            this.SetBackground(Brushes.White);
        }

        internal bool IsEmpty()
        {
            return this._GuessState == GuessState.Empty;
        }

        internal void OnGotChar(char charInput)
        {
            switch (charInput)
            {
                case MyWordleMain.KbdCharBackSpace:// we want to send the backspace and enter to the prior tile (perhaps on prior row)
                    if (this._GuessState == GuessState.NotVerifiedYet)
                    {
                        this._txtBlock.Text = String.Empty;
                        this.SetState(GuessState.Empty);
                    }
                    break;
                case MyWordleMain.KbdCharEnter:
                    if (this._GuessState == GuessState.NotVerifiedYet && this._iCol == _myWordleMain.TargetWordLength - 1)
                    {
                        var curRowWord = string.Empty;
                        for (int i = 0; i < _myWordleMain.TargetWordLength; i++)
                        {
                            curRowWord += _myWordleMain._ArrGuessTiles[_iRow, i]._txtBlock.Text;
                        }
                        if (_myWordleMain._hashWords.Contains(curRowWord))
                        {
                            for (int chIndex = 0; chIndex < _myWordleMain._TargetWord.Length; chIndex++)
                            {
                                var state = GuessState.Wrong;
                                var curch = curRowWord[chIndex];
                                if (_myWordleMain._TargetWord[chIndex] == curch) // exact match of char and position
                                {
                                    state = GuessState.RightLetter;
                                }
                                else if (_myWordleMain._TargetWord.Contains(curch))
                                {
                                    state = GuessState.RightLetterWrongPosition;
                                }
                                _myWordleMain._ArrGuessTiles[_iRow, chIndex].SetState(state);
                                var kbdTile = _myWordleMain._DictKbdTiles[curch];
                                kbdTile.SetState(state);
                            }
                        }
                    }
                    break;
                default:
                    this._txtBlock.Text = charInput.ToString();
                    this.SetState(GuessState.NotVerifiedYet);
                    break;
            }
        }
        public override string ToString() => $"({_iRow},{_iCol} {_txtBlock.Text} {_GuessState})";
    }

    internal class BaseTile : DockPanel
    {
        internal readonly TextBlock _txtBlock = new();
        internal readonly Border _border;
        internal GuessState _GuessState;
        public BaseTile()
        {
            this.Margin = new Thickness(1, 1, 1, 1);
            _border = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(2)
            };
            _border.Child = _txtBlock;
            this.Children.Add(_border);
            _txtBlock.FontSize = 20;
            _txtBlock.HorizontalAlignment = HorizontalAlignment.Center;
            _txtBlock.VerticalAlignment = VerticalAlignment.Center;
        }
        public void SetBackground(Brush brush)
        {
            _border.Background = brush;
        }
        public void SetState(GuessState state)
        {
            this._GuessState = state;
            switch (state)
            {
                case GuessState.Empty:
                case GuessState.NotVerifiedYet:
                    SetBackground(Brushes.White);
                    break;
                case GuessState.Wrong:
                    SetBackground(Brushes.Gray);
                    break;
                case GuessState.RightLetterWrongPosition:
                    SetBackground(Brushes.Yellow);
                    break;
                case GuessState.RightLetter:
                    SetBackground(Brushes.LightGreen);
                    break;

            }
        }
    }
}
