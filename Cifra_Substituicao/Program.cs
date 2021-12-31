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

            Rewriter rw = new Rewriter();
            string dir = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string refDictFileName = dir + "/_dict/pt-br/br_com_acentos_ref_dict.txt";

            string[] refDict = FileUtil.openDictFile(refDictFileName);

            if(refDict.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red; ;
                Console.WriteLine("Não foi encontrado um dicionário de referências.");
                Console.ResetColor();


                Console.WriteLine("Importando arquivos...");

                string mostFreqDictName = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
                mostFreqDictName += "/_dict/pt-br/_freq/mostFreqMod.txt";

                string[] mostFreqWords = FileUtil.openDictFile(mostFreqDictName);


                string fileName = dir + "/_dict/pt-br/br-com-acentos.txt";
                string[] fil = FileUtil.openDictFile(fileName);


                Console.WriteLine("Gerando referencias a partir das palavras mais frequentes...");
                rw.addVariousReferences(mostFreqWords);
                Console.WriteLine("Gerando referencias a partir do dicionário completo...");
                rw.addVariousReferences(fil);

                Console.WriteLine("Gerando dicionário de referências e salvando...");
                FileUtil.writeToFile(refDictFileName, rw.getStructuredString());

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

            

            string fileTextName = dir +  "/_testFiles/Caesar/cod6.txt";

            string[] words = FileUtil.openTextFile(fileTextName);

            ConsoleUtil.printColoredMessage("\nIniciando descriptografia...\n", ConsoleColor.Green);

            Decrypter decrypter = new Decrypter(words, rw);

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
