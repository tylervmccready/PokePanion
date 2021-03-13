# PokePanion

## Getting Started
Thanks for checking out PokePanion! I built this application as I was playing Pokemon Red and kept wanting to look up some basic information on what levels Pokemon evolve and learn moves. I decided to create this app to answer questions that I had been asking, as opposed to just searching for the answers myself. I thought this would be a good way to learn how to use tools in C# centered around scraping the web, as well as how to handle errors and write readable code when dealing with larger projects. 

All the files necessary to run PokePanion are included in this repository. You can either load it into an IDE, or just use the executable to run the program on Windows. I've also included both EdgeDriver and ChromeDriver executables to allow the program to surf the web, as long as they are left in the file paths they are currently in. These can be omitted as well, as there is previously collected data stored in text files that the program can also use. You must always retain PokemonNames.txt, as well as either the webdrivers or the local files, for the program to have data to use in outputting requested information.

## Running PokePanion
When PokePanion is first started, it will ask the user if they would like to build the data files from scratch, or use already created versions. PokePanion comes pre-packaged with the necessary data files, but I welcome you to run it through its web-scraping paces again. The process will usually take about 10 minutes (internet-dependent) to complete, and PokePanion will give you quick update messages as the data is collected.

[Insert Data Collection Updates Picture Here]

Once it has completed the file creation process, or immediately if you skip file creation, PokePanion will create two dictionaries called the PokeDex and MoveDex. These are the data structures the rest of the program will reference when the user requests information on any Move or Pokemon. Once these references are created, PokePanion will ask the user  what information they are looking for, narrowing down the search with successive prompts. Let's say you wanted to know what Pokemon are in Machop's evolutionary line, and how and when they evolve. This is what PokePanion would show you:

[Insert User Prompts and Final Returns Picture Here]

Once PokePanion has displayed the requested information, it will the ask the user if they have any more questions. PokePanion will repeat this loop until the user is satisfied and selects to exit the program. 

## Behind the Scenes

### File Creation
WebScraping.cs contains most of the code that enables PokePanion to scrape the web. PokePanion uses Selenium packages for C# to extract data from CSS elements of Serebii.net. There are several loops, some nested, to enable collecting information for each pokemon and move, including data from multiple tables on each page. There is a significant amount of error-handling required to keep this portion of the program running, as there's no easy way to tell how long these tables will be as they change on each Pokemon or Move's page, and the layout of the webpage isn't always 100% consistent. I will picture the primary part of the WebScraping class below. However, for more details and expanded comments, please feel free to view the source code.

![WebScrape.cs Snapshot](WebScraping%20Snapshot.png?raw=true "WebScrape.cs Snapshot")

### Pokemon and Move Classes
Pokemon.cs and Move.cs contain classes defining what attributes a Pokemon and a Move will have, as well as several methods related to creating the Dexes and displaying Pokemon and Move information. I will picture the primary part of the two classes below. However, for more details and expanded comments, please feel free to view the source code.

![Pokemon.cs Snapshot](Pokemon%20Snapshot.png?raw=true "Pokemon.cs Snapshot")

![Move.cs Snapshot](Move%20Snapshot.png?raw=true "Move.cs Snapshot")

### User Interaction
There's also a class to handle User Interface. This is where methods containing prompts for the user and logic for where to route the user based on their inputs. I will picture the primary part of the User Interface class below. However, for more details and expanded comments, please feel free to view the source code.

![UserInterface.cs Snapshot](UserInterfacec%20Snapshot.png?raw=true "UserInterface.cs Snapshot")

### Wrapping Up
Thanks for dropping by to check out PokePanion! I hope you can get some use out of the program or maybe draw some inspiration from it. If you have questions or just want to connect, please reach out either here or on LinkedIn!
