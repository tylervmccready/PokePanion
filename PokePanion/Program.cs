using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;

namespace PokePanion
{
    class Program
    {
        /// <summary>
        /// This program scrapes the web, pulling data on each pokemon and their moves (gen 1 only) from Serebii.
        /// It then stores that data in local text files and allows the user to request information from that data.
        /// </summary>
        static void Main()
        {
            var writer = new StreamWriter("Test.txt");
            writer.Write("Test");
            writer.Close();
            var rewriter = File.ReadAllText("Test.txt");
            rewriter = rewriter.Remove(rewriter.Length - 2) + "| ";
            File.WriteAllText("Test.txt", rewriter);
            writer = File.AppendText("Test.txt");
            writer.Write("st");
            writer.Close();
            Console.ReadLine();
            
            Dictionary<string, Pokemon> pokeDex;
            Dictionary<string, Move> moveDex;
            if (UI.LocalOrWeb()) // Builds data files from web if user chooses to do so
            {
                Console.WriteLine("This program supports both Chrome and Edge, but is significantly faster with Chrome.");
                Console.WriteLine("Please enter which you would prefer:");
                var browser = Console.ReadLine().ToLower();
                
                Console.WriteLine("Building Moves and Pokemon Data from scratch");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Please note: This will take several minutes");
                Console.ForegroundColor = Console.BackgroundColor;

                IWebDriver driver;
                if (browser.Contains("edge"))
                {
                    driver = new EdgeDriver(EdgeDriverService.CreateChromiumService());
                }
                else
                {
                    var options = new ChromeOptions();
                    options.AddArgument("log-level=3");
                    driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options);
                }
                driver.Url = "https://www.google.com/";
                driver.Manage().Window.Minimize();
            
            
                // Create Dictionary for all pokemon with name and dex number
                var dexes = Pokemon.InitializePokeDex();
                var pokeDexInit = dexes.Item1;
                var dexDict = dexes.Item2;
            
                // Create text file with list of Move Names
                WebScrape.CreateMoveNameFile(driver);
                Console.ResetColor();
                Console.WriteLine("Move Names file created.");
            
                // Create MoveData.txt
                WebScrape.CreateMovesDataFile(driver);
            
                // Create local MoveDex
                moveDex = Move.CreateMoveDex();
                
                Console.WriteLine("Moves data file created.");
                
                // Create PokemonData.txt
                WebScrape.CreatePokemonDataFile(driver, pokeDexInit, moveDex, dexDict);
                
                // Create local PokeDex
                pokeDex = Pokemon.CreatePokeDex();

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("File creation complete!");
                Console.ResetColor();
            }
            else // Create local MoveDex regardless
            {
                // Create local MoveDex
                moveDex = Move.CreateMoveDex();

                // Create local PokeDex
                pokeDex = Pokemon.CreatePokeDex();
            }

            Restart:
            var answer = UI.MoveOrPokemon();
            UI.WhosThatPokemon(answer, pokeDex, moveDex);
            
            Console.WriteLine("Would you like more information?");
            answer = Console.ReadLine();
            if (answer.ToLower().Contains("y")){goto Restart;}
            Console.WriteLine("Goodbye!");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }
    }
}