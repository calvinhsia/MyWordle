namespace MyWordleLib
{
    public class MyWordleLib
    {
        public static HashSet<string> GetWords(DictionaryLib.DictionaryType dictType, int DesiredWordLen, bool UniqueLtrs = true, Random random = null)
        {
            var dic = new DictionaryLib.DictionaryLib(dictType, random);
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