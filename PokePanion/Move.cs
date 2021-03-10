using System;
using System.Collections.Generic;
using System.IO;

namespace PokePanion
{
    public class Move
    {
        public string Name, Type;
        public int Power, Pp;
        public double Accuracy;
        public int? EffectRate;

        /// <summary>
        /// Parametric Move Constructor
        /// </summary>
        /// <param name="name">Name of Move</param>
        /// <param name="type">Type of Move</param>
        /// <param name="power">Base Power of Move</param>
        /// <param name="accuracy">Accuracy Rating of Move</param>
        /// <param name="pp">Power Points of Move</param>
        /// <param name="effectRate">Percent Chance of Effect</param>
        public Move(string name = null, string type = null, int power = default, double accuracy = default, int pp = default, int? effectRate = default)
        {
            Name = name;
            Type = type;
            Power = power;
            Accuracy = accuracy;
            Pp = pp;
            EffectRate = effectRate;
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
                    string[] moveTxt;
                    try
                    {
                        moveTxt = reader.ReadLine().Split(", ");
                    }
                    catch 
                    {
                        break;
                    }
                    allMoves[moveTxt[0].ToLower()] = new Move(moveTxt[0], moveTxt[1], Convert.ToInt32(moveTxt[2]),
                        Convert.ToDouble(moveTxt[3]), Convert.ToInt32(moveTxt[4]));
                    
                    if (moveTxt[5] != "")
                    {
                        allMoves[moveTxt[0].ToLower()].EffectRate = Convert.ToInt32(moveTxt[5]);
                    }
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
        public void WriteMove()
        {
            Console.WriteLine($"{Name} is a {Type} move with base power {Power} and accuracy {Accuracy}.");
            Console.Write($"{Name} can be used {Pp} times");
            if (EffectRate is null)
            {
                Console.WriteLine(".");
            }
            else
            {
                Console.WriteLine($"and its effect occurs {EffectRate}% of the time.");
            }
            Console.WriteLine();
        }
    }
}