using System;
using System.Collections.Generic;

namespace Cifra_Substituicao
{
    internal static class ConsoleUtil
    {
        public static void printColoredMessage(string message, ConsoleColor color, bool breakLine = true)
        {
            Console.ForegroundColor = color;
            Console.Write(message + ((breakLine)?"\n":""));
            Console.ResetColor();
        }
    
    
        public static void printKeys(Dictionary<char, char> dict, ConsoleColor color, bool breakLine = true)
        {
            Console.ForegroundColor = color;

            Console.Write("CHAVE => {");
            foreach (var key in dict)
            {
                Console.Write("[" + key.Key + " = " + key.Value + "]");
            }
            Console.WriteLine("}" + ((breakLine)?"\n":""));
            Console.ResetColor();
            
        }

    }
}
