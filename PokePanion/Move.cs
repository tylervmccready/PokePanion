using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PokePanion
{
    public class Move
    {
        /// <summary>
        /// The name of this move.
        /// </summary>
        public string Name; 
        /// <summary>
        /// The type of this move.
        /// </summary>
        public string Type;
        /// <summary>
        /// The base attack power of this move.
        /// </summary>
        public int Power; 
        /// <summary>
        /// The Power Points of this move.
        /// </summary>
        public int Pp;
        /// <summary>
        /// The accuracy rating of this move.
        /// </summary>
        public double Accuracy;
        /// <summary>
        /// The percent chance for this move's effect to trigger, if applicable.
        /// </summary>
        public int? EffectRate;
        /// <summary>
        /// All Pokemon that can learn this move by leveling.
        /// </summary>
        public List<string> LearnedByLevel;
        /// <summary>
        /// All Pokemon that can be taught this move with a TM.
        /// </summary>
        public List<string> LearnedByTm;
        
        public Move(string name = null, string type = null, int power = default, double accuracy = default, 
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
        public static Dictionary<string, Move> CreateMoveDex() // Need to identify exceptions for catches
        {
            var allMoves = new Dictionary<string, Move>();
            using (var reader = new StreamReader("MoveData.txt"))
            {
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
                    List<string> byLevels = dataGroups[1].Split(", ").ToList(), byTm = dataGroups[2].Split(", ").ToList();

                    int? effectRate = basics[5] != "" ? Convert.ToInt32(basics[5]) : null;
                    allMoves[basics[0].ToLower()] = new Move(basics[0],
                        basics[1], Convert.ToInt32(basics[2]), Convert.ToDouble(basics[3]), Convert.ToInt32(basics[4]), effectRate, byLevels, byTm);
                }
            }
            return allMoves;
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
        /// Displays all information of a Move
        /// </summary>
        public void WriteMove() // Todo: Update for Learned by
        {
            if (Type == "Electric" || Type == "Ice")
            {
                Console.WriteLine($"{Name} is an {Type} move with base power {Power} and accuracy {Accuracy}.");
            }
            else
            {
                Console.WriteLine($"{Name} is an {Type} move with base power {Power} and accuracy {Accuracy}.");
            }
            Console.Write($"{Name} can be used {Pp} times");
            if (EffectRate is null)
            {
                Console.WriteLine(".");
            }
            else
            {
                Console.WriteLine($" and its effect occurs {EffectRate}% of the time.");
            }
            Console.WriteLine();
        }
    }
}