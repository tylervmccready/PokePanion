using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// ReSharper disable PossibleNullReferenceException

namespace PokePanion
{
    public class Move
    {
        /// <summary>
        /// The name of this move.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of this move.
        /// </summary>
        private string Type { get; }

        /// <summary>
        /// The base attack power of this move.
        /// </summary>
        private int Power { get; }

        /// <summary>
        /// The Power Points of this move.
        /// </summary>
        private int Pp { get; }

        /// <summary>
        /// The accuracy rating of this move.
        /// </summary>
        private double Accuracy { get; }

        /// <summary>
        /// The percent chance for this move's effect to trigger, if applicable.
        /// </summary>
        private int? EffectRate { get; }

        /// <summary>
        /// All Pokemon that can learn this move by leveling.
        /// </summary>
        public List<string> LearnedByLevel { get; }

        /// <summary>
        /// All Pokemon that can be taught this move with a TM.
        /// </summary>
        public List<string> LearnedByTm { get; }

        private Move(string name = null, string type = null, int power = default, double accuracy = default, 
            int pp = default, int? effectRate = default, List<string> learnedByLevel = null , List<string> learnedByTm = null)
        {
            Name = name;
            Type = type;
            Power = power;
            Accuracy = accuracy;
            Pp = pp;
            EffectRate = effectRate;
            LearnedByLevel = learnedByLevel;
            LearnedByTm = learnedByTm;
        }
        
        
        /// <summary>
        /// Generates a Dictionary of Move values keyed by their names from an associated text file
        /// </summary>
        /// <returns>Dictionary(string, Moves)</returns>
        public static Dictionary<string, Move> CreateMoveDex()
        {
            var allMoves = new Dictionary<string, Move>();
            using var reader = new StreamReader("MoveData.txt");
            while (true)
            {
                string[] dataGroups;
                try
                {
                    dataGroups = reader.ReadLine().Split("| ");
                }
                catch (NullReferenceException)
                {
                    break;
                }
                var basics = dataGroups[0].Split(", ");

                var byLevels = dataGroups.Length > 1 
                    ? dataGroups[1].Split(", ").ToList() 
                    : null;
                var byTm = dataGroups.Length > 2 
                    ? dataGroups[2].Split(", ").ToList() 
                    : null;

                int? effectRate = basics[5] != "" ? Convert.ToInt32(basics[5]) : null;
                allMoves[basics[0].ToLower()] = new Move(basics[0],
                    basics[1], Convert.ToInt32(basics[2]), Convert.ToDouble(basics[3]), Convert.ToInt32(basics[4]), effectRate, byLevels, byTm);
            }

            return allMoves;
        }
        
        
        /// <summary>
        /// Gives choices for displaying this Move's information.
        /// </summary>
        /// <param name="pokeDex">Initialized PokeDex</param>
        public void MoveInformation(Dictionary<string, Pokemon> pokeDex)
        {
            Console.WriteLine($"Please select the information you would like for {Name}:");
            Console.WriteLine("1\tBasic information (i.e. Power and Accuracy).");
            Console.WriteLine($"2\tAll Pokemon that can learn {Name}.");
            Console.WriteLine($"3\tPokemon that learn {Name} naturally.");
            Console.WriteLine($"4\tPokemon that can be taught {Name}.");
            Console.WriteLine($"5\tIf a specific Pokemon can learn {Name}");
            
            try
            {
                var selection = Convert.ToInt32(Console.ReadLine());
                switch (selection)
                {
                    case 1:
                        WriteMove();
                        break;
                    case 2:
                        DisplayAllLearned();
                        break;
                    case 3:
                        DisplayLevelLearned();
                        break;
                    case 4:
                        DisplayTmLearned();
                        break;
                    case 5:
                        Console.WriteLine("What Pokemon do you want to check?");
                        CanItLearn(pokeDex[Console.ReadLine().ToLower()]);
                        break;
                    // Recursively invokes method if user input is unexpected
                    default:
                        Console.WriteLine("Please make your choice by entering only its corresponding integer.");
                        MoveInformation(pokeDex);
                        break;
                }
            }
            catch (FormatException) // Recursively invokes method if user input is unexpected
            {
                Console.WriteLine("Please make your choice by entering only its corresponding integer.");
                MoveInformation(pokeDex);
            }
        }
        
        
        /// <summary>
        /// Displays all information for all Moves in an enumerable
        /// </summary>
        /// <param name="moves">enumerable of Moves to display</param>
        public static void WriteMoves(IEnumerable<Move> moves)
        {
            foreach (var move in moves)
            {
                move.WriteMove();
            }
        }
        

        /// <summary>
        /// Displays all basic information of a Move
        /// </summary>
        private void WriteMove()
        {
            // Display basics, with grammar correction for a/an
            if (Type == "Electric" || Type == "Ice")
            {
                Console.WriteLine($"{Name} is an {Type} move with base power {Power} and accuracy {Accuracy}.");
            }
            else
            {
                Console.WriteLine($"{Name} is a {Type} move with base power {Power} and accuracy {Accuracy}.");
            }
            
            Console.Write($"{Name} can be used {Pp} times");
            // Writes Effect Rate only if applicable
            Console.WriteLine(EffectRate is null 
                ? "." 
                : $" and its effect occurs {EffectRate}% of the time.");
            Console.WriteLine();
        }
        

        /// <summary>
        /// Displays all Pokemon that can learn this move.
        /// </summary>
        private void DisplayAllLearned()
        {
            Console.WriteLine($"These Pokemon can learn {Name}:");
            Console.WriteLine("By leveling up:\t\tBy TM:");
            for (var idx = 0; idx < LearnedByLevel.Count || idx < LearnedByTm.Count; idx++)
            {
                // Checks if pokemon can learn this move naturally
                string byLevel, byTm;
                if (LearnedByLevel[0] == "" || idx >= LearnedByLevel.Count)
                {
                    byLevel = null;
                }
                else
                {
                    byLevel = LearnedByLevel[idx];
                }

                // Checks if pokemon can be taught this move
                if (LearnedByTm[0] == "" || idx >= LearnedByTm.Count)
                {
                    byTm = null;
                }
                else
                {
                    byTm = LearnedByTm[idx];
                }
                
                // Displays pokemon that can learn the move, depending on arrangement
                if (byLevel is null && byTm is not null)
                {
                    Console.WriteLine($"\t\t\t{byTm}");
                }
                else if (byLevel is not null && byTm is null)
                {
                    Console.WriteLine(byLevel);
                }
                else if (byLevel is not null)
                {
                    switch (byLevel.Length)
                    {
                        case > 14:
                            Console.WriteLine($"{byLevel}\t{byTm}");
                            break;
                        case > 7:
                            Console.WriteLine($"{byLevel}\t\t{byTm}");
                            break;
                        default:
                            Console.WriteLine($"{byLevel}\t\t\t{byTm}");
                            break;
                    }
                }
            }
        }
        

        /// <summary>
        /// Displays all Pokemon that learn this move naturally.
        /// </summary>
        private void DisplayLevelLearned()
        {
            // Checks for non-applicability
            if (LearnedByLevel[0] == "")
            {
                Console.WriteLine("This move can only by learned by a Pokemon by using a TM.");
            }
            else
            {
                Console.WriteLine($"{Name} is learned by leveling up the following Pokemon:");
                foreach (var pokemon in LearnedByLevel)
                {
                    Console.WriteLine(pokemon);
                }
            }
        }
        

        /// <summary>
        /// Displays all Pokemon that can be taught this move.
        /// </summary>
        private void DisplayTmLearned()
        {
            // Checks for non-applicability
            if (LearnedByTm[0] == "")
            {
                Console.WriteLine("This move can only by learned by leveling up certain Pokemon.");
            }
            else
            {
                Console.WriteLine($"{Name} can be taught to the following Pokemon:");
                foreach (var pokemon in LearnedByTm)
                {
                    Console.WriteLine(pokemon);
                }
            }
        }
        

        /// <summary>
        /// Determines if a specified Pokemon can learn this Move.
        /// </summary>
        /// <param name="pokemon">Pokemon to check</param>
        private void CanItLearn(Pokemon pokemon)
        {
            // Check for how the pokemon learns the move, then display
            if (LearnedByLevel.Contains(pokemon.Basics[0]))
            {
                Console.WriteLine($"{pokemon.Basics[0]} learns {Name} at level " +
                                  $"{pokemon.MoveLevels[Array.IndexOf(pokemon.Moves, Name)]}");
            }
            else if (LearnedByTm.Contains(pokemon.Basics[0])) // Todo: add handling for possibility of learning both ways
            {
                Console.WriteLine($"{pokemon.Basics[0]} can be taught {Name} with " +
                                  $"{pokemon.MoveLevels[Array.IndexOf(pokemon.Moves, Name)]}");
            }
            else
            {
                Console.WriteLine($"{pokemon.Basics[0]} can not learn {Name}.");
            }
        }
    }
}