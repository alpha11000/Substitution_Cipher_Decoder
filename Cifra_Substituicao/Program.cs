using System;
using System.Collections.Generic;
using System.IO;
using Dict_Rewrite;

namespace Cifra_Substituicao
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Rewriter rw = new Rewriter(); //Gerenciará os dicionários e os códigos referentes às palavras

            string dir = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string dictDirectory = dir + "/_dict_files/";
            //string testsDirectory = dir + "/_tests/";
            
            string[] dicts = Directory.GetDirectories(dictDirectory); //todos os dicionários presentes na pasta de dicionários
            string currentDictFolder; //pasta do dicionario escolhido pelo usuário
            
            //Escolha do dicionário a ser utilizado
            Console.WriteLine("Qual dicionário você deseja utilizar?");

            for(int i = 0; i < dicts.Length; i++)
            {
                currentDictFolder = dicts[i].Substring(dicts[i].LastIndexOf('/') + 1);

                ConsoleUtil.printColoredMessage((i+1) + "- " + currentDictFolder, ConsoleColor.Yellow);
            }

            int choice; //indice do dicionário escolhido pelo usuário

            while (true)
            {
                Console.Write("Sua escolha: ");
                choice = int.Parse(Console.ReadLine());


                if(choice > dicts.Length || choice < 1)
                {
                    ConsoleUtil.printColoredMessage("Escolha Inválida", ConsoleColor.Red);
                    continue;
                }

                break;
            }

            currentDictFolder = dicts[choice - 1].Substring(dicts[choice - 1].LastIndexOf('/') + 1);

            string dict = dicts[choice - 1];
            string totalWordsFileNAme = dict + "/" + currentDictFolder + "-words.txt";
            string freqDictName = dict + "/_freq/" + currentDictFolder + "-freq.txt";
            string refDictName = dict + "/" + currentDictFolder + "-ref.txt";

            string[] refDict = FileUtil.openDictFile(refDictName);

            if (refDict.Length == 0)
            {
                if(Directory.GetFiles(dict).Length == 1)
                {
                    totalWordsFileNAme = Directory.GetFiles(dict)[0];
                    ConsoleUtil.printColoredMessage($"Importando a partir de: {totalWordsFileNAme}", ConsoleColor.Yellow);
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Não foi encontrado um dicionário de referências.");
                Console.ResetColor();


                Console.WriteLine("Importando arquivos...");

                //string mostFreqDictName = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
                //mostFreqDictName += "/_dict/pt-br/_freq/mostFreqMod.txt";

                string[] mostFreqWords = FileUtil.openDictFile(freqDictName);
                string[] totalWordsFile = FileUtil.openDictFile(totalWordsFileNAme);


                Console.WriteLine("Gerando referencias a partir das palavras mais frequentes...");
                rw.addVariousReferences(mostFreqWords);
                Console.WriteLine("Gerando referencias a partir do dicionário completo...");
                rw.addVariousReferences(totalWordsFile);

                Console.WriteLine("Gerando dicionário de referências e salvando...");
                FileUtil.writeToFile(refDictName, rw.getStructuredString());

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Salvo com sucesso.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Dicionário de referências encontrado. Importando-o...");
                Console.ResetColor();
                
                rw.setReferencesFromString(refDict);
            }


            Console.WriteLine("Digite o caminho para o arquivo que deseja descriptografar. (Ex:  /_testFiles/Caesar/cod6.txt)");
            string fileTextName = dir +  Console.ReadLine();

            string[] fileText = FileUtil.openTextFile(fileTextName);

            ConsoleUtil.printColoredMessage("\nIniciando descriptografia...\n", ConsoleColor.Green);

            Decrypter decrypter = new Decrypter(fileText, rw);

            watch.Reset();
            watch.Start();
            string[] decrypted = decrypter.decrypt();
            Console.WriteLine("Terminado em " + watch.Elapsed + " segundos.\n");
            watch.Stop();

            Console.BackgroundColor = ConsoleColor.Blue;
            ConsoleUtil.printColoredMessage("Texto descriptografado:\n", ConsoleColor.Black);

            foreach(string s in decrypted)
            {
                Console.Write(s + ' ');
            }

            Console.WriteLine();
            Console.ReadLine();
        }

      /* static void Main(string[] args)
         {
             Rewriter rw = new Rewriter();

             string dir = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
             string refDictFileName = dir + "/_dict/pt-br/br_com_acentos_ref_dict.txt";
             string fileTextName = dir + "/TESTE.txt";


             string[] refDict = FileUtil.openDictFile(refDictFileName);
             rw.setReferencesFromString(refDict);

             string[] words = FileUtil.openTextFile(fileTextName);
             Console.WriteLine(words.Length);

             Decrypter decrypter = new Decrypter(words, rw);

             SortedDictionary<int, char[]> testeDict = new SortedDictionary<int, char[]>();
             Dictionary<char, char> keys = new Dictionary<char, char>();

             keys.Add('t', 't');
             keys.Add('s', 's');

             testeDict.Add(0, "teste".ToCharArray());
             testeDict.Add(1, "tartaruga".ToCharArray());
             List<char[]> dict = new List<char[]>();
             List<char[]> similiar = new List<char[]>();

             bool isValid;

             SortedDictionary<int, char[]> sorted =  decrypter.sortByCorrespondences(testeDict, keys, -1, out similiar, out isValid);

             foreach(var s in sorted)
             {
                 Console.WriteLine(s.Key + " --> " + new string(s.Value) + " --> " + isValid);
             }

             Console.WriteLine("***********************");

             keys.Add('a', 'a');
             keys.Add('r', 'r');
             keys.Add('g', 'g');

             sorted = decrypter.sortByCorrespondences(testeDict, keys, -1, out similiar, out isValid);

             foreach (var s in sorted)
             {
                 Console.WriteLine(s.Key + " --> " + new string(s.Value) + " --> " + isValid);
             }


             Console.WriteLine("***********************");

             sorted = decrypter.sortByCorrespondences(testeDict, keys, 0, out similiar, out isValid);

             foreach (var s in sorted)
             {
                 Console.WriteLine(s.Key + " --> " + new string(s.Value) + " --> " + isValid);
             }


             testeDict.Add(2, "tsts".ToCharArray());

             Console.WriteLine("***********************");

             sorted = decrypter.sortByCorrespondences(testeDict, keys, -1, out similiar, out isValid);

             foreach (var s in sorted)
             {
                 Console.WriteLine(s.Key + " --> " + new string(s.Value) + " --> " + isValid);
             }

         }*/
    }
}
