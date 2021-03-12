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
                    var input = Console.ReadLine();
                    try
                    {
                        var move = moveDex[input.ToLower()];
                        MoveInformation(move);
                    }
                    catch (KeyNotFoundException)
                    {
                        Console.WriteLine($"{input} isn't a recognized move name, would you like to try again?");
                        var choice = Console.ReadLine().ToLower().Contains("y");
                        switch (choice)
                        {
                            case true:
                                WhosThatPokemon(answer, pokeDex, moveDex);
                                break;
                            case false:
                                return;
                        }
                    }
                    break;
                case false:
                    Console.WriteLine("What pokemon would you like information on?:");
                    var pokemon = Console.ReadLine();
                    try
                    {
                        PokemonInformation(pokeDex[pokemon.ToLower()]);
                    }
                    catch (KeyNotFoundException)
                    {
                        Console.WriteLine($"{pokemon} isn't a recognized pokemon name, would you like to try again?");
                        var choice = Console.ReadLine().ToLower().Contains("y");
                        switch (choice)
                        {
                            case true:
                                WhosThatPokemon(answer, pokeDex, moveDex);
                                break;
                            case false:
                                return;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Gives choices for displaying Pokemon information
        /// </summary>
        /// <param name="pokemon">Pokemon you would like information on</param>
        public static void PokemonInformation(Pokemon pokemon)
        {
            Console.WriteLine("Please select the information you would like for this pokemon:");
            Console.WriteLine($"1\tAll moves learned by {pokemon.Basics[0]} via leveling");
            Console.WriteLine($"2\tNext move {pokemon.Basics[0]} will learn by leveling");
            Console.WriteLine($"3\tAll TMs {pokemon.Basics[0]} can learn");
            Console.WriteLine($"4\tDisplay evolution line for {pokemon.Basics[0]}");
            
            try
            {
                var selection = Convert.ToInt32(Console.ReadLine());
                switch (selection)
                {
                    case 1:
                        pokemon.DisplayNaturalMoves();
                        break;
                    case 2:
                        Console.WriteLine("What level is your pokemon?");
                        pokemon.Level = Convert.ToInt32(Console.ReadLine());
                        pokemon.NextLearnedMove();
                        break;
                    case 3:
                        pokemon.DisplayTMs();
                        break;
                    case 4:
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
            Console.WriteLine($"Please select the information you would like for {move.Name}:");
            Console.WriteLine("1\tAll available");
            Console.WriteLine("2\tSecondChoice");
            Console.WriteLine("3\tThirdChoice");
            
            try
            {
                var selection = Convert.ToInt32(Console.ReadLine());
                switch (selection)
                {
                    case 1:
                        move.WriteMove();
                        break;
                    case 2:
                        // Todo: Add options for displaying Learned By
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