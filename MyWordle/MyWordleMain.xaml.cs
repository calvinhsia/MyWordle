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
            var dicSmall = new DictionaryLib.DictionaryLib(DictionaryLib.DictionaryType.Small); // 5 LTR WORDS = 3425, 2329 uniq
            var hashSmall = GetWords(dicSmall, DesiredWordLen: 5);

            var dicLarge = new DictionaryLib.DictionaryLib(DictionaryLib.DictionaryType.Large); // 9908, 6406 uniq
            var hashLarge = GetWords(dicLarge, DesiredWordLen: 5);
            var x = hashLarge.ElementAt(2);

        }

        private HashSet<string> GetWords(DictionaryLib.DictionaryLib dic, int DesiredWordLen, bool UniqueLtrs = true)
        {
            var hashResult = new HashSet<string>();
            var wrd = dic.SeekWord("a");
            while (!string.IsNullOrEmpty(wrd))
            {
                if (wrd.Length == DesiredWordLen)
                {

                    var uniqueCnt = 0;
                    if (UniqueLtrs)
                    {
                        uniqueCnt = wrd.GroupBy(l => l).Count();
                    }
                    else
                    {
                        uniqueCnt = DesiredWordLen;
                    }
                    if (uniqueCnt == DesiredWordLen)
                    {
                        hashResult.Add(wrd);
                    }
                }
                wrd = dic.GetNextWord();
            }
            return hashResult;
        }
    }
}
