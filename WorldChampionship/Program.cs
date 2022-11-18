using System.Collections;
using System.Collections.Generic;

//GK, DF, MF, FW - goalkeeper, defender, midfielder, forward

var players = assemblePlayersTeam();

int mainMenuChoice = 0;

do
{
    mainMenuChoice = mainMenu();

    switch (mainMenuChoice)
    {
        case 1:
            {
                printDividingLine();
                foreach (var player in players)
                {
                    int newRating = trainingSession(player.Value.rating);
                    Console.WriteLine($"Player: {player.Key} | Position: {player.Value.position} | Rating before: {player.Value.rating} | Rating after practice: {newRating}\n");
                    players[player.Key] = (position: player.Value.position, rating: newRating);
                }
                printDividingLine();
                break;
            }
        case 2:
            {
                var team = formTeam(players);
                playMatch(team);
                break;
            }
        case 3:
            {
                Console.WriteLine("Choice is 3");
                break;
            }
        case 4:
            {
                Console.WriteLine("Choice is 4");
                break;
            }
        case 0:
            {
                Environment.Exit(0);
                break;
            }
        default:
            {
                Console.WriteLine("There is no action for provided input!\n");
                break;
            }
    }

} while (mainMenuChoice != 0);

Console.ReadKey();

/* 
 * Used for initialization and return of Dictionary with starting default players 
*/
Dictionary<string,(string position, int rating)> assemblePlayersTeam()
{
    return new Dictionary<string, (string position, int rating)>()
    {
        {"Luka Modrić", (position: "MF", 88)},
        {"Marcelo Brozović", (position: "FW", 86)},
        {"Mateo Kovačić", (position: "DF", 84)},
        {"Ivan Perišić", (position: "MF", 84)},
        {"Andrej Kramarić", (position: "MF", 82)},
        {"Ivan Rakitić", (position: "MF", 82)},
        {"Joško Gvardiol", (position: "DF",81)},
        {"Marko Livaja", (position: "FW", 77)},
        {"Marko Pjaca", (position: "FW", 75)},
        {"Borna Barišić", (position: "DF", 73)},
        {"Josip Vuković", (position: "MF", 69)},
        {"Antonio Marin", (position: "GK", 68)},
        {"Damjan Đoković", (position: "DF", 70)},
        {"Domagoj Antolić", (position: "FW", 72)},
        {"Josip Šutalo", (position: "DF", 75)},
        {"Ante Budimir", (position: "GK", 76)},
        {"Mario Pašalić", (position: "MF", 81)},
        {"Lovro Majer", (position: "FW", 80)},
        {"Ante Rebić", (position: "DF", 80)},
        {"Franko Andrijašević", (position: "MF", 72)},
        {"Fran Tudor", (position: "MF", 72)},
        {"Dante Stipica", (position: "MF", 71)},
        {"Ivan Santini", (position: "FW", 69)},
        {"Dario Melnjak", (position: "GK", 70)},
        {"Mato Miloš", (position: "FW", 69)}
    };
}

/* 
 * Used for display of the main menu and repeating it until the user inserts a number of a valid action
 * Returns user input, takes no parameters
*/
int mainMenu()
{
    bool success = false;
    int choice = 0;

    do
    {
        Console.WriteLine("1 - Training session\n2 - Play a match\n3 - Statistics\n4 - Player menu\n0 - Exit");
        Console.Write("Input action: ");
        success = int.TryParse(Console.ReadLine(), out choice);
        Console.WriteLine("\n");
    } while (!success);

    return choice;
}

/*
 * Used for increasing players rating by 0-5% randomly
 * Returns players new rating as int, takes current rating as parameter
*/
int trainingSession(int currentRating)
{
    Random rand = new Random();
    decimal percentage = ((decimal)rand.Next(0, 5))/100;
    int newRating = (int)(Math.Round(currentRating + currentRating * percentage, MidpointRounding.AwayFromZero));

    //if the rating it already 100 (maximum) or new rating is over 100, return 100, if not return the new calculated rating
    return (currentRating==100 || newRating >= 100)? 100 : newRating;
}

/*
 * Forms a team for a match containing of 1 GK, 4 DF, 3 MF and 3 FW or strikers which are generated randomly,
 * while other players are determined by their rating
 * Returns Dictionary of players that will participate in the game and takes Dictionary of all players as a parameter
*/
Dictionary<string, (string position, int rating)> formTeam(Dictionary<string, (string position, int rating)> players)
{
    var team = new Dictionary<string, (string position, int rating)>();

    //initializing seperate dictionaries for every position ordered by rating descending
    var goalkeepers = players.Where(p => p.Value.position == "GK")
        .OrderByDescending(p => p.Value.rating)
        .ToDictionary(p => p.Key, p => p.Value);

    var defenders = players.Where(p => p.Value.position == "DF")
        .OrderByDescending(p => p.Value.rating)
        .ToDictionary(p => p.Key, p => p.Value);

    var midfielders = players.Where(p => p.Value.position == "MF")
        .OrderByDescending(p => p.Value.rating)
        .ToDictionary(p => p.Key, p => p.Value);

    var strikers = players.Where(p => p.Value.position == "FW")
        .ToDictionary(p => p.Key, p => p.Value);

    //adding all the players to the team
    team.Add(goalkeepers.ElementAt(0).Key, goalkeepers.ElementAt(0).Value);
    listPlayers(team);

    team.Add(defenders.ElementAt(0).Key, defenders.ElementAt(0).Value);
    team.Add(defenders.ElementAt(1).Key, defenders.ElementAt(1).Value);
    team.Add(defenders.ElementAt(2).Key, defenders.ElementAt(2).Value);
    team.Add(defenders.ElementAt(3).Key, defenders.ElementAt(3).Value);

    team.Add(midfielders.ElementAt(0).Key, midfielders.ElementAt(0).Value);
    team.Add(midfielders.ElementAt(1).Key, midfielders.ElementAt(1).Value);
    team.Add(midfielders.ElementAt(2).Key, midfielders.ElementAt(2).Value);

    Random rand = new Random();
    int indexToAdd = 0;
    for (int i = 0; i < 3; i++)
    {
        indexToAdd = rand.Next(0, strikers.Count);
        team.Add(strikers.ElementAt(indexToAdd).Key, strikers.ElementAt(indexToAdd).Value);
    }

    return team;
}

/*
 * Randomly generate match result and opponent, raise striker rating by 5% and other players by 2% if the team won
 * Print out the result and save it
 * Max number of matches that can be played are 6
*/
void playMatch(Dictionary<string, (string position, int rating)> matchPlayers)
{

}

/*
 * Prints out all players from the Dictionary
*/
void listPlayers(Dictionary<string, (string position, int rating)> listOfPlayers)
{
    printDividingLine();
    foreach (var player in listOfPlayers)
    {
        Console.WriteLine($"Player: {player.Key} | Position: {player.Value.position} | Rating: {player.Value.rating}\n");
    }
    printDividingLine();
}

/*
 * Prints a dividing line
*/
void printDividingLine()
{
    Console.WriteLine("\n\n<<----------------------------------->>\n\n");
}