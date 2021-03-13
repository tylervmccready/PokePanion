using System;
using System.Collections.Generic;
using System.Threading;
// ReSharper disable PossibleNullReferenceException

namespace PokePanion
{
    internal static class Program
    {
        /// <summary>
        /// This program scrapes the web, pulling data on each pokemon and their moves (gen 1 only) from Serebii.
        /// It then stores that data in local text files and allows the user to request information from that data.
        /// </summary>
        private static void Main()
        {
            Dictionary<string, Pokemon> pokeDex;
            Dictionary<string, Move> moveDex;
            if (UserInterface.LocalOrWeb()) // Builds data files from web if user chooses to do so
            {
                (pokeDex, moveDex) = WebScrape.CreateLocalFiles();
            }
            else // Uses pre-existing local files otherwise
            {
                moveDex = Move.CreateMoveDex();
                pokeDex = Pokemon.CreatePokeDex();
            }

            UserPrompt:
            var answer = UserInterface.MoveOrPokemon();
            UserInterface.InformationRouter(answer, pokeDex, moveDex);
            
            // Restart
            Console.WriteLine("Would you like more information?");
            answer = Console.ReadLine();
            if (answer.ToLower().Contains("y")){goto UserPrompt;}
            
            // Application Exit
            Console.WriteLine("Goodbye!");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }
    }
}