using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LemmaSharp.Classes;

namespace Lemmalizer {
    public static class Lemmalizer {
        private const string DataFilepathEn = "lemmalib/full7z-mlteast-en.lem";
        private const string DataFilepathRu = "lemmalib/full7z-mlteast-ru.lem";
        private static readonly Lemmatizer s_lemmatizerEn
            = new(File.OpenRead(GetPathToLemmatizer(DataFilepathEn)));
        private static readonly Lemmatizer s_lemmatizerRu
            = new(File.OpenRead(GetPathToLemmatizer(DataFilepathRu)));

        private static string GetPathToLemmatizer(string DataFilepath) {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Join(path, DataFilepath);
        }

        public static async Task<Dictionary<string, uint>> LemmalizeToDict(string text) {
            Dictionary<string, uint> lemmas = new();
            if (text == "")
                return lemmas;
            var words = text.Split().Where(x => !string.IsNullOrWhiteSpace(x));

            foreach (var word in words) {
                var lemma = "";
                if (!Regex.IsMatch(word, @"\P{IsCyrillic}"))
                    lemma = s_lemmatizerRu.Lemmatize(word.ToLower());

                if (!Regex.IsMatch(word, @"\P{IsBasicLatin}"))
                    lemma = s_lemmatizerEn.Lemmatize(word.ToLower());

                if (lemma == "")
                    lemma = word;

                if (lemmas.ContainsKey(lemma))
                    lemmas[lemma]++;
                else
                    lemmas.Add(lemma, 1);
            }

            return lemmas;
        }
    }
}
