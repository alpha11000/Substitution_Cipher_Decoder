using System;
using System.IO;
using Dict_Rewrite;

namespace Cifra_Substituicao
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string csvName = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            csvName += "/_dict/pt-br/_freq/mostFreqMod.txt";

            string[] mostFreqWords = FileUtil.openDictFile(csvName);


            string fileName = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            fileName += "/_dict/pt-br/br-com-acentos.txt";
            string[] fil = FileUtil.openDictFile(fileName);

            Rewriter rw = new Rewriter();
            Console.WriteLine("Inserindo dicionário de maior frequencia...");
            rw.addVariousReferences(mostFreqWords);
            Console.WriteLine("Inserindo dicionário com todas as palavras...");
            rw.addVariousReferences(fil);

            string fileTextName = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            fileTextName += "/_testFiles/Caesar/co0.txt";

            string[] words = FileUtil.openTextFile(fileTextName);
            

            Decrypter decrypter = new Decrypter(words, rw);

            string[] decrypted = decrypter.decrypt();

            Console.WriteLine("Palavras descriptografadas:");

            foreach(string s in decrypted)
            {
                Console.Write(s + ' ');
            }

            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
