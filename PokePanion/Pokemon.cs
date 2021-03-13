using System;
using System.Collections.Generic;
using System.IO;
// ReSharper disable PossibleNullReferenceException

namespace PokePanion
{
    
    public class Pokemon
    {
        /// <summary>
        /// Contains the Name, Dex Number, Primary Type, and Secondary Type of a Pokemon.
        /// </summary>
        public string[] Basics { get; }

        /// <summary>
        /// Contains the levels this Pokemon learns moves at, or the TM/HM numbers for teachable moves.
        /// </summary>
        public string[] MoveLevels { get; }

        /// <summary>
        /// Contains the names of all moves this Pokemon can learn.
        /// </summary>
        public string[] Moves { get; }

        /// <summary>
        /// Contains the levels this Pokemon evolves at, or null if not applicable.
        /// </summary>
        private string[] EvoLevels { get; }

        /// <summary>
        /// Contains the names of this Pokemon's evolutions.
        /// </summary>
        private string[] EvoNames { get; }

        /// <summary>
        /// Contains the items used to trigger this Pokemon's evolution, or null if not applicable.
        /// </summary>
        private string[] EvoItems { get; }

        /// <summary>
        /// The current level of this Pokemon.
        /// </summary>
        private int Level { get; set; }

        private Pokemon(string[] basics = null, string[] moveLevels = null, string[] moves = null, 
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
            var pokeDex = new Dictionary<string, Pokemon>();
            var dexDict = new Dictionary<int, string>();
            using (var reader = new StreamReader("PokemonNames.txt"))
            {
                for (var dexNumber = 1; ; dexNumber++)
                {
                    var name = reader.ReadLine();
                    if (name is null || name == "") {break;}
                    pokeDex[name] = new Pokemon(new[]{name, dexNumber.ToString(), null, null});
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
                var dataLine = reader.ReadLine();
                // Ends method when last line is reached
                if (dataLine is null){break;}

                // Splits based on group and individual separators
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
        /// Gives choices for displaying this Pokemon's information
        /// </summary>
        public void PokemonInformation(Dictionary<string, Move> moveDex)
        {
            Console.WriteLine("Please select the information you would like for this pokemon:");
            Console.WriteLine($"1\tAll moves learned by {Basics[0]} via leveling");
            Console.WriteLine($"2\tNext move {Basics[0]} will learn by leveling");
            Console.WriteLine($"3\tAll TMs {Basics[0]} can learn");
            Console.WriteLine($"4\tDisplay evolution line for {Basics[0]}");
            Console.WriteLine($"5\tCan {Basics[0]} learn a specific move?");
            
            try
            {
                var selection = Convert.ToInt32(Console.ReadLine());
                switch (selection)
                {
                    case 1:
                        DisplayNaturalMoves();
                        break;
                    case 2:
                        Console.WriteLine("What level is your pokemon?");
                        Level = Convert.ToInt32(Console.ReadLine());
                        NextLearnedMove();
                        break;
                    case 3:
                        DisplayTMs();
                        break;
                    case 4:
                        DisplayEvolutions();
                        break;
                    case 5:
                        Console.WriteLine("What Move do you want to check?");
                        CanItLearn(moveDex[Console.ReadLine().ToLower()]);
                        break;
                    // Recursively invokes method if user input is unexpected
                    default:
                        Console.WriteLine("Please make your choice by entering only its corresponding integer.");
                        PokemonInformation(moveDex);
                        break;
                }
            }
            catch (FormatException) // Recursively invokes method if user input is unexpected
            {
                Console.WriteLine("Please make your choice by entering only its corresponding integer.");
                PokemonInformation(moveDex);
            }
        }
        

        /// <summary>
        /// Displays table of moves learned exclusively through leveling for this pokemon
        /// </summary>
        private void DisplayNaturalMoves()
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
        private void DisplayTMs()
        {
            Console.WriteLine($"{Basics[0]} learns the following moves:");
            Console.WriteLine("TM\tMove");
            for (int i = 0; i < MoveLevels.Length; i++)
            {
                // Ensure only TM/HM moves are displayed
                if (MoveLevels[i].StartsWith("TM") || MoveLevels[i].StartsWith("HM"))
                {
                    Console.WriteLine($"{MoveLevels[i]}\t{Moves[i]}");
                }
            }
        }
        

        /// <summary>
        /// Displays the next move the instanced pokemon at a given level will learn
        /// </summary>
        private void NextLearnedMove()
        {
            int idx;
            for (idx = 0; Level >= Convert.ToInt32(MoveLevels[idx]); idx++){}

            // Checks if Pokemon has any moves left to learn
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
        private void DisplayEvolutions()
        {
            Console.WriteLine($"{Basics[0]} is part of the following evolution line:");
            Console.WriteLine("Level\tPokemon\t\tEvolution Item");
            for (var i = 0; i < EvoNames.Length - 1; i++)
            {
                // Display N/A when items aren't applicable
                var item = EvoItems[i] == "" ? "N/A" : EvoItems[i];
                // Makes evolution stone names look pretty
                if (item.Contains("stone"))
                {
                    item = item[0].ToString().ToUpper() + item.Substring(1).Replace("stone", " Stone");
                }
                // Display N/A when levels aren't applicable
                var evoLevel = EvoLevels[i] == "" ? "N/A" : EvoLevels[i];
                // Variable formatting based on name length
                Console.WriteLine(EvoNames[i].Length > 7
                    ? $"{evoLevel}\t{EvoNames[i]}\t{item}"
                    : $"{evoLevel}\t{EvoNames[i]}\t\t{item}");
            }
        }
        
        
        /// <summary>
        /// Determines if this Pokemon can learn the specified Move.
        /// </summary>
        /// <param name="move">Move to check</param>
        private void CanItLearn(Move move)
        {
            // Check for how the pokemon learns the move, then display
            if (move.LearnedByLevel.Contains(Basics[0]))
            {
                Console.WriteLine($"{Basics[0]} learns {move.Name} at level " +
                                  $"{MoveLevels[Array.IndexOf(Moves, move.Name)]}");
            }
            else if (move.LearnedByTm.Contains(Basics[0]))
            {
                Console.WriteLine($"{Basics[0]} can be taught {move.Name} with " +
                                  $"{MoveLevels[Array.IndexOf(Moves, move.Name)]}");
            }
            else
            {
                Console.WriteLine($"{Basics[0]} can not learn {move.Name}.");
            }
        }
        
    }
}