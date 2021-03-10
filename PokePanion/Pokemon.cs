using System;
using System.Collections.Generic;
using System.IO;

namespace PokePanion
{
    public class Pokemon
    {
        public string[] Basics, MoveLevels, Moves, EvoLevels, EvoNames, EvoItems;
        public int Level;

        public Pokemon(string[] basics = null, string[] moveLevels = null, string[] moves = null, 
            string[] evoLevels = null, string[] evoNames = null, string[] evoItems = null, int level = 0)
        {
            Basics = basics;
            MoveLevels = moveLevels;
            Moves = moves;
            EvoLevels = evoLevels;
            EvoNames = evoNames;
            EvoItems = evoItems;
            Level = level;
        }


        /// <summary>
        /// Initializes Dictionary(string, Pokemon) with names and Dex numbers from PokemonNames.txt
        /// </summary>
        /// <returns></returns>
        public static (Dictionary<string, Pokemon>, Dictionary<int, string>) InitializePokeDex()
        {
            // Create objects for all pokemon with name and dex number
            var pokeDex = new Dictionary<string, Pokemon>();
            var dexDict = new Dictionary<int, string>();
            using (var reader = new StreamReader("PokemonNames.txt"))
            {
                for (int dexNumber = 1; ; dexNumber++)
                {
                    var name = reader.ReadLine();
                    if (name is null) {break;}
                    pokeDex[name] = new Pokemon(new string[4]{name, dexNumber.ToString(), null, null});
                    dexDict[dexNumber] = name;

                }
            }

            return (pokeDex, dexDict);
        }

        /// <summary>
        /// Finds a pokemon in the PokeDex by searching for Dex Number
        /// </summary>
        /// <param name="dexNum">Dex Number of Pokemon to search</param>
        /// <param name="dexDict">Dex Number to Name Translator</param>
        /// <param name="pokeDex">Generation's PokeDex</param>
        /// <returns>Pokemon object for specified Pokemon</returns>
        public static Pokemon FindByDexNum(int dexNum, Dictionary<int, string> dexDict,
            Dictionary<string, Pokemon> pokeDex)
        {
            return pokeDex[dexDict[dexNum]];
        }

        /// <summary>
        /// Creates a complete PokeDex from data in a local file
        /// </summary>
        /// <returns>Returns completed PokeDex as Dictionary</returns>
        public static Dictionary<string, Pokemon> CreatePokeDex()
        {
            var pokeDex = new Dictionary<string, Pokemon>();
            using var reader = new StreamReader("PokemonData.txt");
            while (true)
            {
                // Ends method when last line is reached
                var dataLine = reader.ReadLine();
                if (dataLine is null){break;}

                var dataGroups = dataLine.Split("| ");
                var basics = dataGroups[0].Split(", ");
                var moveLevels = dataGroups[1].Split(", ");
                var moves = dataGroups[2].Split(", ");
                var evoLevels = dataGroups[3].Split(", ");
                var evoNames = dataGroups[4].Split(", ");
                var evoItems = dataGroups[5].Split(", ");


                pokeDex[basics[0]] = new Pokemon(basics, moveLevels, moves, evoLevels, evoNames, evoItems);
            }

            return pokeDex;
        }
        
        

        /// <summary>
        /// Displays table of learned moves for a given pokemon
        /// </summary>
        /// <param name="pokemon">Pokemon to reference</param>
        public static void DisplayLearnedMoves(Pokemon pokemon)
        {
            try
            {
                Console.WriteLine($"{pokemon.Basics[0]} learns the following moves:");
            }
            catch (IndexOutOfRangeException) // Checks for empty array
            {
                Console.WriteLine($"{pokemon} isn't a recognized pokemon name. Please check your spelling and try again.");
                return;
            }
            Console.WriteLine("Level\tMove");
            for (int i = 0; i < pokemon.MoveLevels.Length; i++)
            {
                Console.WriteLine($"{pokemon.MoveLevels[i]}\t{pokemon.Moves[i]}");
            }
        }

        /// <summary>
        /// Displays the next move the instanced pokemon at a given level will learn
        /// </summary>
        public void NextLearnedMove()
        {
            int idx;
            for (idx = 0; Level >= Convert.ToInt32(MoveLevels[idx]); idx++){}

            Console.WriteLine($"{Basics[0]} will learn {Moves[idx]} at level {MoveLevels[idx]}.");
        }
        
        /// <summary>
        /// Displays table of instanced pokemon's evolutions
        /// </summary>
        public void DisplayEvolutions()
        {
            Console.WriteLine($"{Basics[0]} is part of the following evolution line:");
            Console.WriteLine("Level\tPokemon\t\tEvolution Item");
            for (int i = 0; i < EvoNames.Length - 1; i++)
            {
                string item = EvoItems[i] == "" ? "N/A" : EvoItems[i];
                if (EvoNames[i].Length > 7)
                {
                    Console.WriteLine($"{EvoLevels[i]}\t{EvoNames[i]}\t{item}");
                }
                else
                {
                    Console.WriteLine($"{EvoLevels[i]}\t{EvoNames[i]}\t\t{item}");
                }
            }
        }
    }
}