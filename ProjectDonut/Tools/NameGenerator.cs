using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Tools
{
    public class NameGenerator
    {
        //private readonly List<string> _syllables = new List<string>()
        //{
        //    "kun", "kai", "koo", "kii", "kee", "kay", "koy", "kuy", "kuy", "koy", "kay",
        //};
        private static List<string> _syllables = new List<string>
        {
            "ar", "bel", "cor", "dan", "el", "fen", "gor", "hal", "il", "jor",
            "kas", "lor", "mor", "nar", "or", "pan", "qua", "ren", "sar", "tan",
            "ul", "val", "wen", "xor", "yn", "zar", "ber", "dor", "fen", "gar",
            "hir", "jal", "kon", "lir", "mal", "nor", "par", "que", "ras", "sol",
            "tur", "ven", "war", "xil", "yer", "zan", "ara", "bri", "can", "dor",
            "eth", "fir", "gan", "hil", "jor", "kar", "len", "mar", "nal", "orn",
            "pra", "quin", "ram", "ser", "tal", "ur", "vor", "win", "xar", "yar",
            "zel", "bron", "cer", "del", "enth", "fal", "gon", "hur", "jen", "kir",
            "lar", "mor", "nal", "ol", "pon", "quir", "rol", "sen", "tor", "urn",
            "vul", "wyn", "xer", "yar", "zen", "bre", "cra", "dur", "el", "far",
            "gel", "hor", "jan", "kir", "lir", "mon", "nor", "op", "par", "rim",
            "sel", "tar", "uth", "vor", "wen", "xar", "yor", "zar", "bal", "dar",
            "fin", "gol", "her", "jin", "kul", "len", "mer", "nar", "ol", "pel",
            "quar", "rin", "sar", "tol", "ul", "ven", "wyn", "xer", "yon", "zor"
        };



        public static string GenerateRandomName(int syllableCount)
        {
            var name = "";

            for (int i = 0; i < syllableCount; i++)
            {
                var syllable = GetRandomSyllable();
                name += syllable;
            }

            return char.ToUpper(name[0]) + name.Substring(1); ;
        }

        private static string GetRandomSyllable()
        {
            var random = new Random();
            return _syllables[random.Next(0, _syllables.Count)];
        }
    }
}
