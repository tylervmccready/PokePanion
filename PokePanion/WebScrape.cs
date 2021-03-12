using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;

namespace PokePanion
{
    public class WebScrape
    {
        /// <summary>
        /// Collects all move names by scraping web and writes them to MoveNames.txt
        /// </summary>
        /// <param name="driver">Initialized WebDriver</param>
        public static void CreateMoveNameFile(IWebDriver driver)
        {
            using var writer = new StreamWriter("MoveNames.txt");
            driver.Navigate().GoToUrl("https://www.serebii.net/attackdex-rby/");
            for (int moveInc = 2, listInc = 1; listInc < 4; moveInc++)
            {
                try
                {
                    var move = driver.FindElement(By.CssSelector(
                            $"#content > main > div:nth-child(3) > table > tbody > tr > td:nth-child({listInc}) > form > div > select > option:nth-child({moveInc})"))
                        .Text;
                    if (move == "Sand Attack")
                    {
                        move = "Sand-Attack";
                    }
                    writer.WriteLine(move);
                }
                catch (NoSuchElementException)
                {
                    moveInc = 1;
                    listInc++;
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
            var writer = new StreamWriter("MoveData.txt");
            using var reader = new StreamReader("MoveNames.txt");
            var i = 0;
            while (true)
            {
                var move = reader.ReadLine()?.Replace(",", "").Trim();
                if (move is null){break;}
                // moveNames.Add( new Move(move));
                driver.Navigate()
                    .GoToUrl($"https://serebii.net/attackdex-rby/{move.ToLower().Replace(" ", "")}.shtml");
                
                var type = driver.FindElement(By.CssSelector(
                        "#content > main > table:nth-child(7) > tbody > tr:nth-child(2) > td:nth-child(2) > a > img"))
                    .GetProperty("src").Substring(36).Replace(".gif", "");
                
                var power = Convert.ToInt32(driver.FindElement(By.CssSelector(
                    "#content > main > table:nth-child(7) > tbody > tr:nth-child(4) > td:nth-child(2)")).Text);

                var accuracy = Convert.ToDouble(driver.FindElement(By.CssSelector(
                    "#content > main > table:nth-child(7) > tbody > tr:nth-child(4) > td:nth-child(3)")).Text);

                var pp = Convert.ToInt32(driver.FindElement(By.CssSelector(
                    "#content > main > table:nth-child(7) > tbody > tr:nth-child(4) > td:nth-child(1)")).Text);

                int? effectRate;
                try
                {
                    effectRate = Convert.ToInt32(driver
                        .FindElement(By.CssSelector(
                            "#content > main > table:nth-child(7) > tbody > tr:nth-child(8) > td.cen")).Text
                        .Replace("%", "").Trim());
                }
                catch
                {
                    effectRate = null;
                }

                var byLevels = new List<string>();
                for (int pokeIdx = 3; ; pokeIdx++)
                {
                    try
                    {
                        byLevels.Add(driver
                            .FindElement(By.CssSelector(
                                $"#content > main > table:nth-child(9) > tbody > tr:nth-child({pokeIdx}) > td:nth-child(3) > a"))
                            .Text);
                    }
                    catch (NoSuchElementException)
                    {
                        break;
                    }
                }

                int layout;
                var byTm = new List<string>();
                try
                {
                    driver.FindElement(By.CssSelector(
                        "#content > main > table:nth-child(9) > tbody > tr:nth-child(3) > td:nth-child(3) > a"));
                    layout = 1;
                }
                catch (NoSuchElementException)
                {
                    layout = 2;
                }
                for (int pokeIdx = 3; layout < 3; pokeIdx++)
                {
                    switch (layout)
                    {
                        case 1:
                            try
                            {
                                byTm.Add(driver.FindElement(By.CssSelector(
                                        $"#content > main > table:nth-child(10) > tbody > tr:nth-child({pokeIdx}) > td:nth-child(3) > a"))
                                    .Text);
                            }
                            catch (NoSuchElementException)
                            {
                                layout = 3;
                            }
                            break;
                        case 2:
                            try
                            {
                                byTm.Add(driver.FindElement(By.CssSelector(
                                        $"#content > main > table:nth-child(12) > tbody > tr:nth-child({pokeIdx}) > td:nth-child(3) > a"))
                                    .Text);
                            }
                            catch (NoSuchElementException)
                            {
                                layout = 3;
                            }
                            break;
                    }
                }
                writer.Write($"{move}, {type}, {power}, {accuracy}, {pp}, {effectRate}| ");
                foreach (var pokemon in byLevels)
                {
                    writer.Write($"{pokemon}, ");
                }

                writer = RemoveLastTwo(writer, "MoveData.txt");
                writer.Write("| ");
                
                foreach (var pokemon in byTm)
                {
                    writer.Write($"{pokemon}, ");
                }

                writer = RemoveLastTwo(writer, "MoveData.txt");
                writer.WriteLine();
                i++;
                if (i % 50 == 0)
                {
                    Console.WriteLine($"Data for {i} moves has been collected!");
                }
            }
            writer.Close();
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
                using (var writer = new StreamWriter("PokemonData.txt"))
                {
                    int i = 0;
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
                        i++;
                        if (i % 50 == 0)
                        {
                            Console.WriteLine($"Data for {i} Pokemon has been collected!");
                        }
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
        public static List<string> FindMoveLevels(IWebDriver driver)
        {
            var moveLevels = new List<string>();
            int layout, tableIdx, elemIdx;
            string level = null;
            try
            {
                driver.FindElement(By.CssSelector(
                    "#content > main > div > div > table:nth-child(13) > tbody > tr:nth-child(3) > td:nth-child(2) > a"));
                layout = 1;
                tableIdx = 13;
            }
            catch (NoSuchElementException)
            {
                layout = 2;
                tableIdx = 14;
            }

            for (elemIdx = 3; ; elemIdx += 2)
            {
                switch (layout)
                {
                    case 1:
                        try
                        {
                            level = driver.FindElement(By.CssSelector(
                                    $"#content > main > div > div > table:nth-child({tableIdx}) > tbody > tr:nth-child({elemIdx}) > td:nth-child(1)"))
                                .Text;
                        }
                        catch (NoSuchElementException)
                        {
                            if (tableIdx == 14){layout = 3;}
                            tableIdx++;
                            elemIdx = 1;
                        }
                        break;
                    case 2:
                        try
                        {
                            level = driver.FindElement(By.CssSelector(
                                    $"#content > main > div > div > table:nth-child({tableIdx}) > tbody > tr:nth-child({elemIdx}) > td:nth-child(1)"))
                                .Text;
                        }
                        catch (NoSuchElementException)
                        {
                            if (tableIdx == 18){layout = 3;}
                            tableIdx += 4;
                            elemIdx = 1;
                        }
                        break;
                }
                
                if (layout == 3 || level == ""){break;}
                if (elemIdx == 1){continue;}

                if (level.Length > 5)
                {
                    level = level.Substring(0, 4);
                }
                
                moveLevels.Add(level.ToUpper());
            }
            return moveLevels;
        }

        /// <summary>
        /// Collects level up move data and creates dictionary
        /// </summary>
        /// <param name="driver">Initialized WebDriver</param>
        /// <param name="movesDex">Dictionary of Moves</param>
        /// <returns>Dictionary of levels and moves learned</returns>
        public static (List<string> moveLevels, List<Move>) FindLevelUpMoves(IWebDriver driver, Dictionary<string, Move> movesDex)
        {
            var moveLevels = FindMoveLevels(driver);
            var levelUpMoves = (moveLevels, new List<Move>());
            int layout, tableIdx, elemIdx;
            string moveName = null;
            try
            {
                driver.FindElement(By.CssSelector(
                    "#content > main > div > div > table:nth-child(13) > tbody > tr:nth-child(3) > td:nth-child(2) > a"));
                layout = 1;
                tableIdx = 13;
            }
            catch (NoSuchElementException)
            {
                layout = 2;
                tableIdx = 14;
            }

            for (elemIdx = 3; ; elemIdx += 2)
            {
                switch (layout)
                {
                    case 1:
                        try
                        {
                            moveName = driver.FindElement(By.CssSelector(
                                    $"#content > main > div > div > table:nth-child({tableIdx}) > tbody > tr:nth-child({elemIdx}) > td:nth-child(2)"))
                                .Text;
                        }
                        catch (NoSuchElementException)
                        {
                            if (tableIdx == 14){layout = 3;}
                            tableIdx++;
                            elemIdx = 1;
                        }
                        break;
                    case 2:
                        try
                        {
                            moveName = driver.FindElement(By.CssSelector(
                                    $"#content > main > div > div > table:nth-child({tableIdx}) > tbody > tr:nth-child({elemIdx}) > td:nth-child(2)"))
                                .Text;
                        }
                        catch (NoSuchElementException)
                        {
                            if (tableIdx == 18){layout = 3;}
                            tableIdx += 4;
                            elemIdx = 1;
                        }
                        break;
                }
                if (layout == 3 || moveName == ""){break;}
                if (elemIdx == 1){continue;}
                levelUpMoves.Item2.Add(movesDex[moveName.ToLower()]);
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

        /// <summary>
        /// Deletes the last two characters from a file and continues StreamWriter on same line.
        /// </summary>
        /// <param name="writer">Current StreamWriter</param>
        /// <param name="filename">File to work with</param>
        /// <returns>StreamWriter</returns>
        public static StreamWriter RemoveLastTwo(StreamWriter writer, string filename)
        {
            writer.Close();
            var rewriter = File.ReadAllText(filename);
            rewriter = rewriter.Remove(rewriter.Length - 2);
            File.WriteAllText(filename, rewriter);
            return File.AppendText(filename);
        }
    }
}