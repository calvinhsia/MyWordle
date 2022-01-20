using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using MyWordleLib;

namespace MyWordle
{
    /// <summary>
    /// Interaction logic for MyWordleMain.xaml
    /// </summary>
    public partial class MyWordleMain : Window
    {
        public MyWordleMain()
        {
            InitializeComponent();
            var hashSmall = MyWordleLib.MyWordleLib.GetWords(DictionaryLib.DictionaryType.Small, DesiredWordLen: 5);// 5 LTR WORDS = 3425, 2329 uniq

            var hashLarge = MyWordleLib.MyWordleLib.GetWords(DictionaryLib.DictionaryType.Large, DesiredWordLen: 5);// 9908, 6406 uniq
            var x = hashLarge.ElementAt(2);
        }

    }
}
