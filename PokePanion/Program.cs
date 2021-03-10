using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V86.Debugger;
using OpenQA.Selenium.Edge;

namespace PokePanion
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        static void Main()
        {
            Dictionary<string, Pokemon> pokeDex;
            Dictionary<string, Move> moveDex;
            if (UI.LocalOrWeb()) // Builds data files from web if user chooses to do so
            {
                Console.WriteLine("This program supports Chrome and Edge (Version 89.0.774.48), please enter which you would prefer:");
                var browser = Console.ReadLine().ToLower();
                
                Console.WriteLine("Building Moves and Pokemon Data from scratch");
                Console.WriteLine("Please note: This will take several minutes");
                
                IWebDriver driver;
                if (browser.Contains("edge"))
                {
                    driver = new EdgeDriver();
                }
                else
                {
                    driver = new ChromeDriver();
                }
                driver.Url = "https://www.google.com/";
                driver.Manage().Window.Minimize();
            
            
                // Create Dictionary for all pokemon with name and dex number
                var dexes = Pokemon.InitializePokeDex();
                var pokeDexInit = dexes.Item1;
                var dexDict = dexes.Item2;
            
                // Create text file with list of Move Names
                // Warning: Searches web and takes ~3 minutes to complete
                WebScrape.CreateMoveNameFile(driver, pokeDexInit);
            
                // Create MoveData.txt
                // Warning: Searches web and takes ~3 minutes to complete
                WebScrape.CreateMovesDataFile(driver);
            
                // Create local MoveDex
                moveDex = Move.CreateMoveDex();

                Console.WriteLine("Files approximately 50% complete.");
                
                // Create PokemonData.txt
                // Warning: Searches web and takes ~5 minutes to complete
                WebScrape.CreatePokemonDataFile(driver, pokeDexInit, moveDex, dexDict);
                
                // Create local PokeDex
                pokeDex = Pokemon.CreatePokeDex();
                
                Console.WriteLine("File creation complete!");
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
        }
    }
}