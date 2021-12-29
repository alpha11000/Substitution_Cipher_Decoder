using System;
using System.Collections.Generic;
using System.Linq;
using Dict_Rewrite;

namespace Cifra_Substituicao
{
    internal class Decrypter
    {
        List<string> original;
        Dictionary<string, referenceCode> words = new Dictionary<string, referenceCode>(); //palavra // referencia
        SortedDictionary<int, List<string>> wordsOrder = new SortedDictionary<int, List<string>>(); //quantidade de semelhantes //palavra
        Rewriter rw;

        public Decrypter(string[] words, Rewriter rw)
        {
            original = new List<string>(words);
            this.rw = rw;

            foreach(string word in words)
            {
                referenceCode rC =  rw.findReferenceByCode(Rewriter.toStructWord(word));

                //Console.WriteLine("+" + word);

                if(this.words.ContainsKey(word))
                {
                    continue;
                }

                this.words.Add(word, rC);

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
                    Console.WriteLine(k.Key + " -- > " + s + " ===> " + words[s].getRefCode());
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

            List<char[]> charWords = desfragmentWords(wordsOrderList);
           
           // List<string> decryptedWords = new List<string>();
           
            Dictionary<char, char> keys = internalDecrypt(0, ref charWords, new Dictionary<char, char>());


            List<int> i;
            string[] newText = new string[original.Count];

            for(int x = 0; x < newText.Length; x++)
            {
                newText[x] = null;
            }

            Console.WriteLine("Size of new Text = " + newText.Length);

            if(keys != null)
            {
                foreach(char[] word in charWords)
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

                        Console.WriteLine(new string(word) + " at index " + a);

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

        //se retornar null, passa para o proximo valor da palavra
        private Dictionary<char, char> internalDecrypt(int atualElement, ref List<char[]> charWords, Dictionary<char, char> lastDict)
        {

            if(atualElement >= charWords.Count)
            {
                Console.WriteLine("Last Element");
                return lastDict;
            }

            string atualWord = new string(charWords[atualElement]);
            

            referenceCode rC = words[atualWord];
            char[] wordChar = charWords[atualElement].ToArray();

            List<int> replacedPositions; //mudar para hash set? 
            //Dictionary<char, char> newDict = new Dictionary<char, char>(lastDict);

            //newDict.Concat(lastDict);

            char[] alteredWord = replaceLetters(wordChar, lastDict, out replacedPositions);

            Console.Write("atualWord -> " + atualWord + " dec -> "); //********************************8
            Console.WriteLine(alteredWord);/////**********************************************

            string[] wordsFinded = (replacedPositions.Count() > 0) ?
                rC.getSimiliarWords(new string(alteredWord), replacedPositions.ToArray()) :
                rC.getElements().ToArray();

            /*if (replacedPositions.Count() > 0 && wordsFinded.Count() <= 0 && replacedPositions.Count() < wordChar.Count())
            {
                Console.WriteLine("Retornou nulo");
                return null;
            }*/

            //verificar se todas as letras foram substituidas e tratar disto

            Console.WriteLine("\nCorrespondências: ");
            foreach (string word in wordsFinded)
            {
                Console.WriteLine(word);
            }

            foreach(string word in wordsFinded)
            {
                Dictionary<char, char> newDictAtual = new Dictionary<char, char>(lastDict);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("triyng: " + word);
                Console.ResetColor();
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
                //o erro está por aqui
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Falha em -> " + word);
                    Console.ResetColor();
                    continue;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("sucesso em -> " + word);
                    Console.ResetColor();
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

            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach(var k in toReplace)
            {
                Console.Write("[" + k.Key + " -> " + k.Value + "]");
            }
            Console.WriteLine();
            Console.ResetColor();

            char[] replaced = new char[original.Length];
            original.CopyTo(replaced, 0);
            List<int> changed = new List<int>();
            int i = 0;

            foreach(char letter in original)
            {
                if (toReplace.ContainsKey(letter))
                {
                    //Console.WriteLine(replaced[i] + " ====> " +  toReplace[letter]);

                    replaced[i] = toReplace[letter];
                    changed.Add(i);

                }
                else
                {
                    replaced[i] = letter;
                }

                i++;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("{");
            foreach(int n in changed)
            {
                Console.Write(n + ", ");
            }
            Console.WriteLine("}");
            Console.ResetColor();

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
