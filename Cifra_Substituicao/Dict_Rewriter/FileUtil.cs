using System;
using System.IO;

namespace Dict_Rewrite
{
    internal class FileUtil
    {
        public static string[] openDictFile(string fileName)
        {

            string[] words;

            try
            {
                words = File.ReadAllLines(fileName);
            }catch(IOException e)
            {
                return new string[0];
            }

            for(int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].ToLower();
            }
            
            return words;
        }

        public static string[] openTextFile(string fileName)
        {
            string[] words;

            try
            {
                StreamReader sr = new StreamReader(fileName);
                words = sr.ReadToEnd().Split(' ');
            }catch(IOException e)
            {
                words = new string[0];
            }

            return words;
        }

        public static string[] openCsvFile(string fileName)
        {
            string[] words;

            try
            {
                StreamReader sr = new StreamReader(fileName);
                words = sr.ReadToEnd().Split(new char[2] {',', ' '});
            }
            catch (IOException e)
            {
                words = new string[0];
            }

            return words;
        }

        public static void writeToFile(string fileName, string[] content)
        {
            File.WriteAllLines(fileName, content);
        }
    }
}
