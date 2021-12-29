using System;
using System.IO;
using Dict_Rewrite;

namespace Cifra_Substituicao
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Rewriter rw = new Rewriter();
            string dir = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string refDictFileName = dir + "/_dict/pt-br/br_com_acentos_ref_dict.txt";

            string[] refDict = FileUtil.openDictFile(refDictFileName);

            if(refDict.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red; ;
                Console.WriteLine("Não foi encontrado um dicionário de referencias.");
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

                Console.WriteLine("Gerando dicionário de frequencia e salvando...");
                FileUtil.writeToFile(refDictFileName, rw.getStructuredString());

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Salvo com sucesso.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Dicionário de frequência encontrado. Importando-o...");
                Console.ResetColor();
                
                rw.setReferencesFromString(refDict);
            }

            

            string fileTextName = dir +  "/_testFiles/Caesar/cod6.txt";

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
