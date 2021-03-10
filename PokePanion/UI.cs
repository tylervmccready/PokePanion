using System;
using System.Collections.Generic;

namespace PokePanion
{
    public class UI
    {
        public static bool LocalOrWeb()
        {
            Console.WriteLine($"Welcome to PokePanion!");
            Console.WriteLine("Would you like to build files from scratch?");
            return Console.ReadLine().ToUpper().Contains("Y");
        }

        public static string MoveOrPokemon()
        {
            Console.WriteLine("Are you looking for information on a Move or a Pokemon?");
            return Console.ReadLine();
        }

        public static void WhosThatPokemon(string answer, Dictionary<string, Pokemon> pokeDex, Dictionary<string, Move> moveDex)
        {
            switch (answer.ToLower().Contains("move")) // 
            {
                case true:
                    Console.WriteLine("What move would you like information on?:");
                    var move = Console.ReadLine();
                    MoveInformation(moveDex[move]);
                    break;
                case false:
                    Console.WriteLine("What pokemon would you like information on?:");
                    var pokemon = Console.ReadLine();
                    PokemonInformation(pokeDex[pokemon]);
                    break;
            }

            return;
        }

        /// <summary>
        /// Gives user choices for displaying Pokemon information
        /// </summary>
        /// <param name="pokemon">Pokemon they would like information on</param>
        public static void PokemonInformation(Pokemon pokemon)
        {
            Console.WriteLine("Please select the information you would like for this pokemon:");
            Console.WriteLine("1\tAll moves learned by a specific pokemon via leveling");
            Console.WriteLine("2\tNext move a pokemon will learn by leveling");
            Console.WriteLine("3\tDisplay evolution line for a pokemon");
            
            try
            {
                var selection = Convert.ToInt32(Console.ReadLine());
                switch (selection)
                {
                    case 1:
                        Pokemon.DisplayLearnedMoves(pokemon);
                        break;
                    case 2:
                        Console.WriteLine("What level is your pokemon?");
                        pokemon.Level = Convert.ToInt32(Console.ReadLine());
                        pokemon.NextLearnedMove();
                        break;
                    case 3:
                        pokemon.DisplayEvolutions();
                        break;
                }
            }
            catch (FormatException) // Recursively retries method if user input is unexpected
            {
                Console.WriteLine("Please make your choice by entering only its corresponding integer.");
                PokemonInformation(pokemon);
            }
        }

        public static void MoveInformation(Move move)
        {
            Console.WriteLine("Please select the information you would like for this Move:");
            Console.WriteLine("1\tAll available");
            Console.WriteLine("2\tSecondChoice");
            Console.WriteLine("3\tThirdChoice");
            
            try
            {
                var selection = Convert.ToInt32(Console.ReadLine());
                switch (selection)
                {
                    case 1:
                        
                        break;
                    case 2:

                        break;
                    case 3:

                        break;
                }
            }
            catch (FormatException) // Recursively retries method if user input is unexpected
            {
                Console.WriteLine("Please make your choice by entering only its corresponding integer.");
                MoveInformation(move);
            }
        }
    }
}