using System;
using System.Collections.Generic;
using System.IO;

namespace PokePanion
{
    
    public class Pokemon
    {
        /// <summary>
        /// Contains the Name, Dex Number, Primary Type, and Secondary Type of a Pokemon.
        /// </summary>
        public string[] Basics;
        /// <summary>
        /// Contains the levels this Pokemon learns moves at, or the TM/HM numbers for teachable moves.
        /// </summary>
        public string[] MoveLevels;
        /// <summary>
        /// Contains the names of all moves this Pokemon can learn.
        /// </summary>
        public string[] Moves;
        /// <summary>
        /// Contains the levels this Pokemon evolves at, or null if not applicable.
        /// </summary>
        public string[] EvoLevels;
        /// <summary>
        /// Contains the names of this Pokemon's evolutions.
        /// </summary>
        public string[] EvoNames;
        /// <summary>
        /// Contains the items used to trigger this Pokemon's evolution, or null if not applicable.
        /// </summary>
        public string[] EvoItems;
        /// <summary>
        /// The current level of this Pokemon.
        /// </summary>
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


                pokeDex[basics[0].ToLower()] = new Pokemon(basics, moveLevels, moves, evoLevels, evoNames, evoItems);
            }

            return pokeDex;
        }
        
        

        /// <summary>
        /// Displays table of moves learned exclusively through leveling for this pokemon
        /// </summary>
        public void DisplayNaturalMoves()
        {
            Console.WriteLine($"{Basics[0]} learns the following moves:");
            Console.WriteLine("Level\tMove");
            for (int i = 0; !MoveLevels[i].StartsWith("TM") && !MoveLevels[i].StartsWith("HM"); i++)
            {
                Console.WriteLine($"{MoveLevels[i]}\t{Moves[i]}");
            }
        }
        
        /// <summary>
        /// Displays table of learnable TMs for this pokemon
        /// </summary>
        public void DisplayTMs()
        {
            Console.WriteLine($"{Basics[0]} learns the following moves:");
            Console.WriteLine("TM\tMove");
            for (int i = 0; i < MoveLevels.Length; i++)
            {
                if (MoveLevels[i].StartsWith("TM") || MoveLevels[i].StartsWith("HM"))
                {
                    Console.WriteLine($"{MoveLevels[i]}\t{Moves[i]}");
                }
            }
        }

        /// <summary>
        /// Displays the next move the instanced pokemon at a given level will learn
        /// </summary>
        public void NextLearnedMove()
        {
            int idx;
            for (idx = 0; Level >= Convert.ToInt32(MoveLevels[idx]); idx++){}

            if (MoveLevels[idx].StartsWith("TM") || MoveLevels[idx].StartsWith("HM"))
            {
                Console.WriteLine($"{Basics[0]} has no more moves to learn by leveling.");
                return;
            }
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
                if (item.Contains("stone"))
                {
                    item = item[0].ToString().ToUpper() + item.Substring(1).Replace("stone", " Stone");
                }
                string evoLevel = EvoLevels[i] == "" ? "N/A" : EvoLevels[i];
                if (EvoNames[i].Length > 7)
                {
                    Console.WriteLine($"{evoLevel}\t{EvoNames[i]}\t{item}");
                }
                else
                {
                    Console.WriteLine($"{evoLevel}\t{EvoNames[i]}\t\t{item}");
                }
            }
        }
    }
}