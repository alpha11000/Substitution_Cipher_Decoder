using System;
using System.Collections.Generic;
using System.Linq;
using Dict_Rewrite;

namespace Cifra_Substituicao
{
    internal class Decrypter
    {
        List<string> original;
        Dictionary<string, referenceCode> wordsOriginal = new Dictionary<string, referenceCode>(); //palavra // referencia
        SortedDictionary<int, List<string>> wordsOrder = new SortedDictionary<int, List<string>>(); //quantidade de semelhantes //palavra
        Rewriter rw;

        public Decrypter(string[] words, Rewriter rw)
        {
            original = new List<string>(words);
            this.rw = rw;

            foreach(string word in words)
            {
                referenceCode rC =  rw.findReferenceByCode(Rewriter.toStructWord(word));

                if(this.wordsOriginal.ContainsKey(word))
                {
                    continue;
                }

                this.wordsOriginal.Add(word, rC);

                if(rC == null)
                {
                    continue;
                }

                if (wordsOrder.ContainsKey(rC.getCount()))
                {
                    wordsOrder[rC.getCount()].Add(word);
                    continue;
                }

                wordsOrder.Add(rC.getCount(), new List<string>() {word});

            }
        }

        public void show()
        {
            foreach(var k in wordsOrder)
            {
                foreach(string s in k.Value)
                {
                    Console.WriteLine(k.Key + " -- > " + s + " ===> " + wordsOriginal[s].getRefCode());
                }
            }
        }

        public string[] decrypt()
        {
            List<string> wordsOrderList = new List<string>();

            //procurar alguma forma de diminuir a complexidade...
            foreach (var k in wordsOrder)
            {
                foreach (string sV in k.Value)
                {
                    wordsOrderList.Add(sV);
                }
            }

            //List<char[]> words = desfragmentWords(wordsOrderList); // para v1
            SortedDictionary<int,char[]> words = getSortedDictionaryFromCharVector(desfragmentWords(wordsOrderList)); //para v2

            // List<string> decryptedWords = new List<string>();

            //Dictionary<char, char> keys = internalDecrypt(0, ref words, new Dictionary<char, char>()); // para V1
            Dictionary<char, char> keys = internalDecryptV2(0, words, new Dictionary<char, char>(), new List<char[]>(), new List<int>(), words.Count - 1); // para V2

            List<int> i;
            string[] newText = new string[original.Count];

            for(int x = 0; x < newText.Length; x++)
            {
                newText[x] = null;
            }

            if(keys != null)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                ConsoleUtil.printColoredMessage("\nChave descoberta com sucesso.\n", ConsoleColor.Black);
                ConsoleUtil.printKeys(keys, ConsoleColor.Green);

                ConsoleUtil.printColoredMessage("Aplicando chave ao texto criptografado...\n", ConsoleColor.Blue);

                foreach(char[] word in words.Values) //para v2
                //foreach(char[] word in words) para v1//
                {
                    char[] decrypted = replaceLetters(word, keys, out i);


                    int j = 0;
                    while (true)
                    {
                        int a = original.IndexOf(new string(word), j);
                        //int a = words.Keys.ToList().IndexOf(new string(word), j);
                        
                        if(a == -1)
                        {
                            break;
                        }

                        if(newText[a] != null)
                        {
                            j = a+1;
                            continue;
                        }

                        newText[a] = new string(decrypted);
                    }
                    
                    //Console.WriteLine("indexof " + new string(decrypted) + " is " + a);
                    
                }
            }

            return newText;
        }

        private Dictionary<char,char> internalDecryptV2(int atualElement, SortedDictionary<int, char[]> words, Dictionary<char, char> key, List<char[]> similarWords, List<int> alteredPositions, int maxElement)
        {

            //if((words.Count <= 0) && atualElement != 0)
            if(atualElement > maxElement)
            {
                ConsoleUtil.printColoredMessage("Last Element", ConsoleColor.Green, true);
                return key;
            }

            char[] alteredWord = new char[0];

            List<char[]> similarWordsAtual;
            List<int> alteredPositionsAtual;


            if (similarWords.Count <= 0)
            {
                referenceCode rC = wordsOriginal[new string (words.Values.ElementAt(atualElement))];
                alteredPositionsAtual = new List<int>();
                similarWordsAtual = rC.getElementsNEW();
            }
            else
            {
                similarWordsAtual = similarWords;
                alteredPositionsAtual = alteredPositions;
            }

            foreach (char[] word in similarWordsAtual)
            {
                Dictionary<char, char> newDict = new Dictionary<char, char>(key);

                ConsoleUtil.printColoredMessage("tentando: " + new string(word), ConsoleColor.Magenta);

                alteredWord = new char[word.Length];
                word.CopyTo(alteredWord, 0);
                int i = 0;
                bool valido = true;

                foreach (char c in alteredWord)
                {
                    if (newDict.ContainsValue(c) && !alteredPositionsAtual.Contains(i))
                    {
                        ConsoleUtil.printColoredMessage("Letra invalida: " + c, ConsoleColor.Red);
                        valido = false;
                        break;
                    }
                    i++;
                }

                if (valido == false)
                {
                    continue;
                }


                i = 0;

                foreach (char letter in words.Values.ElementAt(atualElement))
                {
                    if (!newDict.ContainsKey(letter))
                    {
                        //caso a letra ñ já tenha sido modificada
                        try { 
                           newDict.Add(letter, alteredWord[i]);
                        }catch(Exception e)
                        {
                            ConsoleUtil.printColoredMessage("\t\tERRO FATAL EM 193 -> " + new string(alteredWord) + " pos -> " + i +
                                "\n \t\toriginal word: " + new string(words.Values.ElementAt(atualElement))+
                                " similiar: " + new string(word), ConsoleColor.Red);
                        }
                    }
                    i++;
                }

                bool isValid = false;

                List<char[]> nextSimilarWords = new List<char[]>();
                List<int> nextAlteredPositions = new List<int>();

                SortedDictionary<int, char[]> ordened = sortByCorrespondences(words, newDict, atualElement, 
                    out nextSimilarWords, out nextAlteredPositions, out isValid);

                if (!isValid)
                {
                    continue;
                }

                Dictionary<char, char> nextDict = internalDecryptV2(atualElement + 1, ordened, newDict, nextSimilarWords, nextAlteredPositions, maxElement);

                if(nextDict != null)
                {
                    ConsoleUtil.printColoredMessage("Sucesso em: " + new string(word), ConsoleColor.Green);
                    return nextDict;
                }
                else
                {
                    ConsoleUtil.printColoredMessage("Falha em: " + new string(word), ConsoleColor.Red);
                    continue;
                }

            }
            return null;
        }


        public static SortedDictionary<int, char[]> getSortedDictionaryFromCharVector(List<char[]> words)
        {
            SortedDictionary<int, char[]> sorted = new SortedDictionary<int, char[]>();
            int i = 0;

            foreach(char[] word in words)
            {
                sorted.Add(i, word);
                i++;
            }

            return sorted;
        }

        public SortedDictionary<int, char[]> sortByCorrespondences(SortedDictionary<int, char[]> encWords, Dictionary<char, char> atualKey, int atualElement,out List<char[]> nextSimiliars, out List<int> nextAlteredPositions, out bool isValid){

            SortedDictionary<int, List<char[]>> wordsSorted = new SortedDictionary<int, List<char[]>>();
            wordsSorted.Add(-1, new List<char[]>()); //onde serão guardados os elementos passados
            
            int i = 0;
            nextSimiliars = new List<char[]>();
            nextAlteredPositions = new List<int>();

            foreach(char[] word in encWords.Values)
            {
                //caso haja erro de modificar o vetor de char anterior
                char[] atualWord = new char[word.Length];
                word.CopyTo(atualWord, 0);

                if (i <= atualElement)
                {
                    wordsSorted[-1].Add(word);
                    i++;
                    continue;
                }

                string wordStr = new string(word);

                List<int> modifiedPositions = new List<int>();
                char[] replacedWord = replaceLetters(atualWord, atualKey, out modifiedPositions);

                referenceCode rC = wordsOriginal[wordStr];
                List<char[]> correspondences;

                
                if (modifiedPositions.Count <= 0)
                {
                    correspondences = rC.getElementsNEW();
                }
                else
                {
                    correspondences = rC.getSimiliarWordsNEW(replacedWord, modifiedPositions);
                }



                if(correspondences.Count <= 0)
                {
                    //ConsoleUtil.printColoredMessage(wordStr+ " " + i, ConsoleColor.Red);
                    isValid = false;
                    //return wordsSorted;
                    return new SortedDictionary<int, char[]>();
                }

                if (wordsSorted.ContainsKey(correspondences.Count))
                {
                    wordsSorted[correspondences.Count].Add(word);
                }
                else
                {
                    wordsSorted.Add(correspondences.Count, new List<char[]> { word });
                }

                if(wordsSorted.Keys.ElementAt(1) == correspondences.Count)
                {
                    if(wordsSorted[correspondences.Count].Count == 1)
                    {
                        nextSimiliars = correspondences;
                        nextAlteredPositions = modifiedPositions;
                    }
                }


                //A similiar só pode ser passada depois que todos os elementos já estão atribuidos
                /*if (wordsSorted.Count >= atualElement+3 && wordsSorted.Keys.ElementAt(atualElement+2) == correspondences.Count)
                {

                    if (wordsSorted[correspondences.Count].Count == 1)
                    {
                        nextSimiliars = correspondences;
                        nextAlteredPositions = modifiedPositions;
                    }

                }*/

                i++;

            }

            isValid = true;

            //Mudar essa parte para algo com melhor desempenho
            SortedDictionary<int, char[]> wordsSortedDict = new SortedDictionary<int, char[]>();
            i = 0;

            foreach(List<char[]> words in wordsSorted.Values)
            {
                foreach(char[] word in words)
                {
                    wordsSortedDict.Add(i++, word);
                }
            }

            return wordsSortedDict;

        }

        //se retornar null, passa para o proximo valor da palavra
        private Dictionary<char, char> internalDecrypt(int atualElement, ref List<char[]> charWords, Dictionary<char, char> lastDict)
        {

            if(atualElement >= charWords.Count)
            {
                Console.WriteLine("Last Element");
                return lastDict;
            }

            string atualWord = new string(charWords[atualElement]);
            

            referenceCode rC = wordsOriginal[atualWord];
            char[] wordChar = charWords[atualElement].ToArray();

            List<int> replacedPositions; //mudar para hash set? 
            //Dictionary<char, char> newDict = new Dictionary<char, char>(lastDict);

            //newDict.Concat(lastDict);

            char[] alteredWord = replaceLetters(wordChar, lastDict, out replacedPositions);

            //Console.Write("atualWord -> " + atualWord + " dec -> "); //********************************8
            //Console.WriteLine(alteredWord);/////**********************************************

            string[] wordsFinded = (replacedPositions.Count() > 0) ?
                rC.getSimiliarWords(new string(alteredWord), replacedPositions.ToArray()) :
                rC.getElements().ToArray();

            if (replacedPositions.Count() > 0 && wordsFinded.Count() <= 0 && replacedPositions.Count() < wordChar.Count())
            {
                Console.WriteLine("Retornou nulo");
                return null;
            }

            //verificar se todas as letras foram substituidas e tratar disto

           /* Console.WriteLine("\nCorrespondências: ");
            foreach (string word in wordsFinded)
            {
                Console.WriteLine(word);
            }*/

            foreach(string word in wordsFinded)
            {
                Dictionary<char, char> newDictAtual = new Dictionary<char, char>(lastDict);

                ConsoleUtil.printColoredMessage("Trying: " + word, ConsoleColor.Magenta);
                alteredWord = word.ToCharArray();

                int i = 0;
                bool valido = true;


                Console.ForegroundColor = ConsoleColor.Red;
                foreach(char c in alteredWord)
                {
                    if (newDictAtual.ContainsValue(c) && !replacedPositions.Contains(i))
                    {
                        Console.WriteLine("letra invalida: " + c);
                        valido = false;
                    }
                    i++;
                }
                Console.ResetColor();

                if(valido == false)
                {
                    continue;
                }


                i = 0;
                foreach(char letter in atualWord)
                {
                    if (!newDictAtual.ContainsKey(letter))
                    {
                        //caso a letra ñ já tenha sido modificada
                        newDictAtual.Add(letter, alteredWord[i]);
                    }
                    i++;
                }

                Dictionary<char, char> nextDict = internalDecrypt(atualElement + 1, ref charWords, newDictAtual);

                if(nextDict == null)
                {
                    ConsoleUtil.printColoredMessage("Falha em: " + word, ConsoleColor.Red);
                    continue;
                }
                else
                {
                    ConsoleUtil.printColoredMessage("Sucesso em: " + word, ConsoleColor.Green);
                    return nextDict;
                }
            }

            
            return null;
        }

        public static char[] replaceLetters(char[] original, Dictionary<char, char> toReplace,out List<int> positionsReplaced)
        {
            if(toReplace.Count <= 0 || toReplace == null)
            {
                positionsReplaced = new List<int>();
                return new char[0];
            }

            /*Console.ForegroundColor = ConsoleColor.Yellow;
            foreach(var k in toReplace)
            {
                Console.Write("[" + k.Key + " -> " + k.Value + "]");
            }
            Console.WriteLine();
            Console.ResetColor();*/

            char[] replaced = new char[original.Length];
            original.CopyTo(replaced, 0);
            List<int> changed = new List<int>();
            int i = 0;

            foreach(char letter in original)
            {
                if (toReplace.ContainsKey(letter))
                {
                   // Console.WriteLine(replaced[i] + " ====> " +  toReplace[letter]);

                    replaced[i] = toReplace[letter];
                    changed.Add(i);

                }
                else
                {
                    replaced[i] = letter;
                }

                i++;
            }

            /*Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("{");
            foreach(int n in changed)
            {
                Console.Write(n + ", ");
            }
            Console.WriteLine("}");
            Console.ResetColor();*/

            positionsReplaced = changed;

            return replaced;
        }

        public List<char[]> desfragmentWords(List<string> words)
        {
            List<char[]> charWords = new List<char[]>();

            foreach(string word in words)
            {
                char[] charWord = word.ToCharArray();
                charWords.Add(charWord);
            }

            return charWords;

        }

        
    }
}
