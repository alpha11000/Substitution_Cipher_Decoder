using System.Linq;
using System.Collections.Generic;
using System;

namespace Dict_Rewrite
{
    internal class referenceCode
    {

        private string refCode;
        private HashSet<string> wordList = new HashSet<string>();

        public referenceCode(string code)
        {
            refCode = code;
        }

        public int getSize()
        {
            return refCode.Length;
        }

        public List<string> getElements()
        {
            List<string> words = wordList.ToList();

            return words; 
        }


        public List<char[]> getElementsNEW()
        {
            List<char[]> words = new List<char[]>();

            foreach(string s in wordList)
            {
                words.Add(s.ToArray());
            }

            return words;
        }

        public string getRefCode()
        {
            return refCode; ;
        }

        public void addElement(string element)
        {
            try
            {
                wordList.Add(element);
            }
            catch (Exception e) { }//caso tente inserir palavras repetidas, apenas ignorar
            
        }

        public int getCount()
        {
            return wordList.Count;
        }

        //REMOVER 
        public string[] getSimiliarWords(string word, int[] positionsToCompare)
        {
            List<string> similiarWords = new List<string>();
            char[] wordToCompare = word.ToCharArray();
            char[] atualWord;

            foreach(string w in wordList)
            {
                bool isSimiliar = true;
                atualWord = w.ToCharArray();

                foreach(int i in positionsToCompare)
                {
                    if(atualWord[i].CompareTo(wordToCompare[i]) != 0)
                    {
                        isSimiliar = false;
                    }
                }

                if (isSimiliar)
                {
                    similiarWords.Add(w);
                }
            }

            return similiarWords.ToArray();
        }

        //RENAME
        public List<char[]> getSimiliarWordsNEW(char[] word, List<int> positionsToCompare)
        {
            List<char[]> similiarWords = new List<char[]>();
            char[] atualWord;

            foreach (string w in wordList)
            {
                bool isSimiliar = true;
                atualWord = w.ToCharArray();

                foreach (int i in positionsToCompare)
                {
                    if (atualWord[i].CompareTo(word[i]) != 0)
                    {
                        isSimiliar = false;
                    }
                }

                if (isSimiliar)
                {
                    similiarWords.Add(w.ToCharArray());
                }
            }

            return similiarWords;
        }
    }
}
