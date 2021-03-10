using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;

namespace PokePanion
{
    public class WebScrape
    {
        /// <summary>
        /// Creates a list of all move names by scraping web and writes it to MoveNames.txt
        /// </summary>
        /// <param name="driver">Initialized WebDriver</param>
        /// <param name="allPokemon">Dictionary of names, Pokemon</param>
        public static void CreateMoveNameFile(IWebDriver driver, Dictionary<string, Pokemon> allPokemon)
        {

            // Creates list of all move names
            var moveNameList = new List<string>();
            foreach (var pokemon in allPokemon)
            {
                driver.Navigate().GoToUrl($"https://serebii.net/pokedex/{Convert.ToInt32(pokemon.Value.Basics[1]):000}.shtml");
                for (var i = 3; ; i += 2)
                {
                    var layout = 1;
                    string moveName;
                    try // Creates Move
                    {
                        moveName = driver.FindElement(By.CssSelector(
                                $"#content > main > div > div > table:nth-child(13) > tbody > tr:nth-child({i}) > td:nth-child(2) > a"))
                            .Text;
                    }
                    catch // Catches different table layouts
                    {
                        try
                        {
                            moveName = driver.FindElement(By.CssSelector(
                                    $"#content > main > div > div > table:nth-child(14) > tbody > tr:nth-child({i}) > td:nth-child(2) > a"))
                                .Text;
                            layout = 2;
                        }
                        catch (NoSuchElementException)
                        {
                            break;
                        }
                    }
                    if (!moveNameList.Contains(moveName))
                    {
                        moveNameList.Add(moveName);
                    }
                    
                    // Tentative start for collecting TM/HM moves
                    /*string tmhmName;
                    try // Creates Move
                    {
                        tmhmName = driver.FindElement(By.CssSelector(
                                $"#content > main > div > div > table:nth-child(14) > tbody > tr:nth-child({i}) > td:nth-child(2) > a"))
                            .Text;
                    }
                    catch // Catches different table layouts
                    {
                        try
                        {
                            tmhmName = driver.FindElement(By.CssSelector(
                                    $"#content > main > div > div > table:nth-child(14) > tbody > tr:nth-child({i}) > td:nth-child(2) > a"))
                                .Text;
                        }
                        catch (NoSuchElementException)
                        {
                            break;
                        }
                    }
                    if (!moveNameList.Contains(tmhmName))
                    {
                        moveNameList.Add(tmhmName);
                    }*/
                }
            }

            // Writes list of all move names to a .txt
            using (var writer = new StreamWriter("MoveNames.txt"))
            {
                foreach (var move in moveNameList)
                {
                    writer.WriteLine($"{move}, ");
                }
            }
        }

        /// <summary>
        /// Collects data for a list of moves and writes it to MovesData.txt
        /// </summary>
        /// <param name="driver">Initialized WebDriver</param>
        public static void CreateMovesDataFile(IWebDriver driver)
        {
            // Create objects for all moves, with just the name, from list stored in a .txt
            var moveNames = new List<Move>();
            using (var reader = new StreamReader("MoveNames.txt"))
            {
                while (true)
                {
                    var move = reader.ReadLine()?.Replace(",", "").Trim();
                    if (move is null){break;}
                    moveNames.Add( new Move(move));
                }
            }
            
            // Scrapes move data from Serebii and stores in an array of Moves type
            foreach (var move in moveNames)
            {
                driver.Navigate()
                    .GoToUrl($"https://serebii.net/attackdex-rby/{move.Name.ToLower().Replace(" ", "")}.shtml");
                
                move.Type = driver.FindElement(By.CssSelector(
                        "#content > main > table:nth-child(7) > tbody > tr:nth-child(2) > td:nth-child(2) > a > img"))
                    .GetProperty("src").Substring(36).Replace(".gif", "");
                
                move.Power = Convert.ToInt32(driver.FindElement(By.CssSelector(
                        "#content > main > table:nth-child(7) > tbody > tr:nth-child(4) > td:nth-child(2)")).Text);

                move.Accuracy = Convert.ToDouble(driver.FindElement(By.CssSelector(
                        "#content > main > table:nth-child(7) > tbody > tr:nth-child(4) > td:nth-child(3)")).Text);

                move.Pp = Convert.ToInt32(driver.FindElement(By.CssSelector(
                    "#content > main > table:nth-child(7) > tbody > tr:nth-child(4) > td:nth-child(1)")).Text);

                try
                {
                    move.EffectRate = Convert.ToInt32(driver
                        .FindElement(By.CssSelector(
                            "#content > main > table:nth-child(7) > tbody > tr:nth-child(8) > td.cen")).Text
                        .Replace("%", "").Trim());
                }
                catch
                {
                    move.EffectRate = null;
                }
            }

            // Writes all Move data to MoveData.txt
            using (var writer = new StreamWriter("MoveData.txt"))
            {
                foreach (var move in moveNames)
                {
                    writer.WriteLine($"{move.Name}, {move.Type}, {move.Power}, {move.Accuracy}, {move.Pp}, {move.EffectRate}");
                }
            }
        }

        /// <summary>
        /// Creates file PokemonData.txt by scraping web for all pokemon information and writing to file
        /// </summary>
        /// <param name="driver">Initialized WebDriver</param>
        /// <param name="pokeDex">Initialized PokeDex</param>
        /// <param name="moveDex">CompleteMoveDex</param>
        /// <param name="dexDict">Translator from Dex Number to Pokemon Name</param>
        public static void CreatePokemonDataFile(IWebDriver driver, Dictionary<string, Pokemon> pokeDex, 
            Dictionary<string, Move> moveDex, Dictionary<int, string> dexDict)
        {
            try
            {
                // Scrapes pokemon data from Serebii, stores in a dictionary of strings and pokemon,
                // and writes to a text file
                using (var writer = new StreamWriter("PokemonData.txt"))
                {
                    // driver.Manage().Window.Minimize();
                    foreach (var pokemon in pokeDex)
                    {
                        writer.Write($"{pokemon.Key}, {pokemon.Value.Basics[1]}, ");
                        driver.Navigate().GoToUrl($"https://serebii.net/pokedex/{Convert.ToInt32(pokemon.Value.Basics[1]):000}.shtml");

                        // Assigns and writes Pokemon Types
                        FindType(driver, pokemon, writer);
                        writer.Write("| ");

                        // Stores LevelUpMoves with generated dictionary of levels and moves
                        var levelUpMoves = FindLevelUpMoves(driver, moveDex);

                        // Write levelUpMoves to PokemonData
                        foreach (var level in levelUpMoves.moveLevels)
                        {
                            writer.Write($"{level}, ");
                        }
                        writer.Write("| ");
                        
                        foreach (var move in levelUpMoves.Item2)
                        {
                            writer.Write($"{move.Name}, ");
                        }
                        writer.Write("| ");

                        // Initializes variables for collecting further evolution information
                        string evo1Name, evo2Name;
                        int? evo2Level, evo3Level;
                        string evo2Item = null, evo3Item = null;
                        var evoLevels = new List<int?>();
                        var evoNames = new List<string>();
                        var evoItems = new List<string>();
                    
                        // Collects first evolution name and assigns with 1 as level of first evolution to represent
                        // starting form
                        evo1Name = FindEvoName(driver, pokemon, 1, dexDict);
                        evoLevels.Add(1); evoNames.Add(evo1Name); evoItems.Add(null);

                        try // Collects second evolution name
                        {
                            evo2Name = FindEvoName(driver, pokemon, 2, dexDict);
                        }
                        catch (NoSuchElementException) // Assigns values for non - evolutionary pokemon
                        {
                            goto WriteJump;
                        }
                        catch (FormatException) // Catches that stupid anomaly named Eevee
                        {
                            evoLevels.Add(null); evoNames.Add("Jolteon"); evoItems.Add("thunderstone");
                            evoLevels.Add(null); evoNames.Add("Vaporeon"); evoItems.Add("waterstone");
                            evoLevels.Add(null); evoNames.Add("Flareon"); evoItems.Add("firestone");
                            goto WriteJump;
                        }

                        try // Determines if evolutionary line has 2 or 3 pokemon, assigns values for 3 pokemon lines
                        {
                            var evo3Name = FindEvoName(driver, pokemon, 3, dexDict);
                            try // temp stores second evolution level
                            {
                                evo2Level = FindEvoLevel(driver, 2);
                            }
                            catch (NoSuchElementException) // Catches item-based evolutions
                            {
                                evo2Level = null;
                                evo2Item = FindEvoItem(driver, 2);
                            }
                            catch (InvalidCastException) // Catches trade evolutions
                            {
                                evo2Level = null;
                                evo2Item = "Trade";
                            }

                            try // temp stores third evolution level
                            {
                                evo3Level = FindEvoLevel(driver, 3);
                            }
                            catch (NoSuchElementException) // Catches item-based evolutions
                            {
                                evo3Level = null;
                                evo3Item = FindEvoItem(driver, 3);
                            }
                            catch (FormatException) // Catches item-based evolutions
                            {
                                evo3Level = null;
                                evo3Item = "Trade";
                            } 
                            catch (InvalidCastException) // Catches trade evolutions
                            {
                                evo3Level = null;
                                evo3Item = "Trade";
                            }
                            evoLevels.Add(evo2Level); evoNames.Add(evo2Name); evoItems.Add(evo2Item);
                            evoLevels.Add(evo3Level); evoNames.Add(evo3Name); evoItems.Add(evo3Item);
                        }
                        catch (NoSuchElementException) // Assigns values for 2 pokemon lines
                        {
                            try // Collects second evolution level
                            {
                                evo2Level = FindEvoLevel(driver, 2);
                            }
                            catch (NoSuchElementException) // Catches item-based evolutions
                            {
                                evo2Level = null;
                                evo2Item = FindEvoItem(driver, 2);
                            }
                            catch (FormatException) // Catches trade evolutions
                            {
                                evo2Level = null;
                                evo2Item = "Trade";
                            }

                            evoLevels.Add(evo2Level); evoNames.Add(evo2Name); evoItems.Add(evo2Item);
                        }
                        WriteJump:
                        // Write evolution data to PokemonData
                        
                        foreach (var level in evoLevels)
                        {
                            writer.Write($"{level}, ");
                        }
                        writer.Write("| ");
                        foreach (var evo in evoNames)
                        {
                            writer.Write($"{evo}, ");
                        }
                        writer.Write("| ");
                        foreach (var evoItem in evoItems)
                        {
                            writer.Write($"{evoItem}, ");
                        }
                        writer.WriteLine();
                    }
                }
            }
            catch (NoSuchElementException){} // Quits method if it goes past last pokemon
        }

        /// <summary>
        /// Scrapes web for type of a given pokemon name and modifies the object to reflect
        /// </summary>
        /// <param name="driver">Initialized WebDriver</param>
        /// <param name="pokemon">Pokemon object to modify</param>
        /// <param name="writer">Initialized StreamWriter</param>
        public static void FindType(IWebDriver driver, KeyValuePair<string, Pokemon> pokemon, StreamWriter writer)
        {
            // Assigns and writes primary type
            pokemon.Value.Basics[2] = driver.FindElement(By.CssSelector(
                    "#content > main > div > div > table:nth-child(5) > tbody > tr:nth-child(2) > td.cen > a:nth-child(1) > img"))
                .GetProperty("src").Substring(36).Replace(".gif", "");
            writer.Write($"{pokemon.Value.Basics[2]}, ");

            try // Assigns Secondary Type
            {
                pokemon.Value.Basics[3] = driver.FindElement(By.CssSelector(
                        "#content > main > div > div > table:nth-child(5) > tbody > tr:nth-child(2) > td.cen > a:nth-child(2) > img"))
                    .GetProperty("src").Substring(36).Replace(".gif", "");
            }
            catch // Catches possibility for single type pokemon
            {
                pokemon.Value.Basics[3] = null;
            }
            writer.Write($"{pokemon.Value.Basics[3]}, "); // Writes secondary type
        }

        /// <summary>
        /// Generates list of levels moves are learned at
        /// </summary>
        /// <param name="driver">Initialized WebDriver</param>
        /// <returns>List(int)</returns>
        public static List<int> FindMoveLevels(IWebDriver driver)
        {
            var moveLevels = new List<int>();
            var elemIdx = 3;
            while (true)
            {
                string level;
                int intLevel;
                try // stores move level value
                {
                    level = driver.FindElement(By.CssSelector(
                            $"#content > main > div > div > table:nth-child(13) > tbody > tr:nth-child({elemIdx}) > td:nth-child(1)"))
                        .Text;
                }
                catch (NoSuchElementException) // Catches errors from different table layout
                {
                    try // stores move level value for different table layout
                    {
                        level = driver.FindElement(By.CssSelector(
                                $"#content > main > div > div > table:nth-child(14) > tbody > tr:nth-child({elemIdx}) > td:nth-child(1)"))
                            .Text;
                    }
                    catch (NoSuchElementException) // Catches end of moves table
                    {
                        break;
                    }
                }

                try // Converts move level to int
                {
                    if (level == "—")
                    {
                        level = "1";
                    }
                    intLevel = Convert.ToInt32(level);
                }
                catch (FormatException) // Catches end of level up moves table
                {
                    break;
                }
                        
                moveLevels.Add(intLevel);
                elemIdx += 2;
            }

            return moveLevels;
        }

        /// <summary>
        /// Collects level up move data and creates dictionary
        /// </summary>
        /// <param name="driver">Initialized WebDriver</param>
        /// <param name="movesDex">Dictionary of Moves</param>
        /// <returns>Dictionary of levels and moves learned</returns>
        public static (List<int> moveLevels, List<Move>) FindLevelUpMoves(IWebDriver driver, Dictionary<string, Move> movesDex)
        {
            var moveLevels = FindMoveLevels(driver);
            var levelUpMoves = (moveLevels, new List<Move>());
            var elemIdx = 3;
            for (; levelUpMoves.Item2.Count < moveLevels.Count; elemIdx += 2) 
                // Stops program from passing end of level up moves table
            {
                string moveName;
                try // stores move name
                {
                    moveName = driver.FindElement(By.CssSelector(
                            $"#content > main > div > div > table:nth-child(13) > tbody > tr:nth-child({elemIdx}) > td:nth-child(2)"))
                        .Text;
                }
                catch (NoSuchElementException)
                {
                    try // stores move name for different table layout
                    {
                        moveName = driver.FindElement(By.CssSelector(
                                $"#content > main > div > div > table:nth-child(14) > tbody > tr:nth-child({elemIdx}) > td:nth-child(2)"))
                            .Text;
                        if (moveName == "") // Accounts for Kakuna's weird layout
                        {
                            break;
                        }
                    }
                    catch (NoSuchElementException) // Catches end of moves table
                    {       
                        break;
                    }
                }
                levelUpMoves.Item2.Add(movesDex[moveName]);
            }

            return levelUpMoves;
        }

        /// <summary>
        /// Collects the name of the specified stage of evolution 
        /// </summary>
        /// <param name="driver">Initialized WebDriver</param>
        /// <param name="pokemon">KeyValuePair being used</param>
        /// <param name="evoNum">Number of Stage of Evolution</param>
        /// <param name="dexDict">Translator from Dex Number to Pokemon Name</param>
        /// <returns>Name of Evolution</returns>
        public static string FindEvoName(IWebDriver driver, KeyValuePair<string, Pokemon> pokemon, int evoNum, 
            Dictionary<int, string> dexDict)
        {
            var evoDexNum = driver.FindElement(By.CssSelector(
                    $"#content > main > div > div > table:nth-child(8) > tbody > tr:nth-child(2) > td > table > tbody > tr > td:nth-child({2 * evoNum - 1}) > a"))
                .GetProperty("href").Substring(28).Replace(".shtml", "");
            return dexDict[Convert.ToInt32(evoDexNum)];
        }

        /// <summary>
        /// Finds the level the specified stage of evolution occurs at
        /// </summary>
        /// <param name="driver">Initialized WebDriver</param>
        /// <param name="evoNum">Stage of evolution</param>
        /// <returns>Level as int</returns>
        public static int FindEvoLevel(IWebDriver driver, int evoNum)
        {
            return Convert.ToInt32(driver.FindElement(By.CssSelector(
                    $"#content > main > div > div > table:nth-child(8) > tbody > tr:nth-child(2) > td > table > tbody > tr > td:nth-child({evoNum * 2 - 2}) > img"))
                .GetProperty("src").Substring(40).Replace(".png", ""));
        }
        
        /// <summary>
        /// Finds the item that causes the specified stage of evolution
        /// </summary>
        /// <param name="driver">Initialized WebDriver</param>
        /// <param name="evoNum">Stage of Evolution</param>
        /// <returns>Item as string</returns>
        public static string FindEvoItem(IWebDriver driver, int evoNum)
        {
            return driver.FindElement(By.CssSelector(
                    $"#content > main > div > div > table:nth-child(8) > tbody > tr:nth-child(2) > td > table > tbody > tr > td:nth-child({evoNum * 2 - 2}) > a > img"))
                .GetProperty("src").Substring(39).Replace(".png", "");
        }
    }
}