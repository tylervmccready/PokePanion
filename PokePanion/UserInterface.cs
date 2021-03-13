using System;
using System.Collections.Generic;
// ReSharper disable PossibleNullReferenceException

namespace PokePanion
{
    public static class UserInterface
    {
        /// <summary>
        /// Prompts user to choose between previously created local files and creating new files via web scraping.
        /// </summary>
        /// <returns>User's answer as bool</returns>
        public static bool LocalOrWeb()
        {
            Console.WriteLine($"Welcome to PokePanion!");
            Console.WriteLine("Would you like to build files from scratch?");
            return Console.ReadLine().ToUpper().Contains("Y");
        }
        

        /// <summary>
        /// Prompts user for whether they want to know about a Move or a Pokemon.
        /// </summary>
        /// <returns>User's answer as string</returns>
        public static string MoveOrPokemon()
        {
            Console.WriteLine("Are you looking for information on a Move or a Pokemon?");
            return Console.ReadLine();
        }
        

        /// <summary>
        /// Sends user to either Pokemon or Move Information prompts depending on answer.
        /// </summary>
        /// <param name="answer">User's MoveOrPokemon answer</param>
        /// <param name="pokeDex">Initialized PokeDex</param>
        /// <param name="moveDex">Initialized MoveDex</param>
        public static void InformationRouter(string answer, Dictionary<string, Pokemon> pokeDex, 
            Dictionary<string, Move> moveDex)
        {
            switch (answer.ToLower().Contains("move")) // 
            {
                // Case for Move Information
                case true:
                    Console.WriteLine("What move would you like information on?:");
                    var input = Console.ReadLine();
                    try
                    {
                        var move = moveDex[input.ToLower()];
                        move.MoveInformation(pokeDex);
                    }
                    catch (KeyNotFoundException)
                    {
                        Console.WriteLine($"{input} isn't a recognized move name, would you like to try again?");
                        var choice = Console.ReadLine().ToLower().Contains("y");
                        switch (choice)
                        {
                            case true:
                                InformationRouter(answer, pokeDex, moveDex);
                                break;
                            case false:
                                return;
                        }
                    }
                    break;
                // Case for Pokemon Information
                case false:
                    Console.WriteLine("What pokemon would you like information on?:");
                    var pokemon = Console.ReadLine();
                    try
                    {
                        pokeDex[pokemon.ToLower()].PokemonInformation(moveDex);
                    }
                    catch (KeyNotFoundException)
                    {
                        Console.WriteLine($"{pokemon} isn't a recognized pokemon name, would you like to try again?");
                        var choice = Console.ReadLine().ToLower().Contains("y");
                        switch (choice)
                        {
                            case true:
                                InformationRouter(answer, pokeDex, moveDex);
                                break;
                            case false:
                                return;
                        }
                    }
                    break;
            }
        }
    }
}