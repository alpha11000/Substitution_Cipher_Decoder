using System;
using System.Collections.Generic;

namespace Dict_Rewrite
{
    internal class Rewriter{

        //private SortedList<string, referenceCode> references = new SortedList<string, referenceCode>();
        private Dictionary<string, referenceCode> references = new Dictionary<string, referenceCode>();
        private int wordsCount = 0;

        public void setReferences(string[] _dict)
        {
            string lastValue = "";

            foreach(string word in _dict)
            {
                if (word.Contains("#"))
                {
                    string value = word.Substring(1);

                    referenceCode refCode = new referenceCode(value);
                    references.Add(value, refCode);
                    lastValue = value;
                    continue;
                }

                references[lastValue].addElement(word);
            }
        }

        public void addVariousReferences(string[] words)
        {
            int i = 0;
            int s = words.GetLength(0);
            int percent = 0;
            int lastPercent = -1;

            Console.WriteLine();

            foreach (string word in words)
            {
                addToReference(word);
                i++;

                percent = calculatePercentage(i, s);


                if (percent != lastPercent)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.WriteLine(percent.ToString() + " %");
                    lastPercent = percent;
                }
            }

        }

        public string[] getStructuredString()
        {
            List<string> structuredString = new List<string>();

            Console.WriteLine("gerando a partir da referencia: ");

            foreach(var k in references)
            {
                Console.SetCursorPosition(32, Console.CursorTop - 1);
                Console.WriteLine(k.Key + "\t\t");

                structuredString.Add("#" + k.Key);
                structuredString.AddRange(k.Value.getElements());

            }


            Console.WriteLine("Pronto");

            return structuredString.ToArray();
        }

        public void addToReference(string word)
        {

            string wordCode = toStructWord(word);

            referenceCode rC = findReferenceByCode(wordCode);

            if (rC != null)
            {
                rC.addElement(word);
                return;
            }

            var newReference = new referenceCode(wordCode);
            newReference.addElement(word);

            references.Add(wordCode, newReference);
            wordsCount++;

        }

        public static string toStructWord(string word)
        {
            Dictionary<char, char> lettersCode = new Dictionary<char, char>();

            char[] lettersArray = word.ToCharArray();
            string lettersCodeString = "";

            char a = 'a';

            for(int i = 0; i < lettersArray.Length; i++)
            {
                if (lettersCode.ContainsKey(lettersArray[i]))
                {
                    lettersCodeString += lettersCode[lettersArray[i]];
                    continue;
                }

                lettersCode.Add(lettersArray[i], a);
                lettersCodeString += a++;
            }

            return lettersCodeString;
        }

        public List<string> getSimiliarStructWords(string word)
        {
            referenceCode refCode = findReferenceByCode(toStructWord(word));
            List<string> words = new List<string>();

            if(refCode != null)
            {
                words = refCode.getElements();

            }
            
            return words;
        }

        public referenceCode findReferenceByCode(string code)
        {

            referenceCode refC;

            try
            {
                refC = references[code];
            }
            catch (Exception e)
            {
                refC = null;
            }

            return refC;
        }

        public static int calculatePercentage(int atual, int total)
        {
            int percent = (100 * atual) / total;
            return percent;
        }

    }
}
