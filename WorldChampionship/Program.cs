using System.Collections;
using System.Collections.Generic;
using System.Reflection;

//GK, DF, MF, FW - goalkeeper, defender, midfielder, forward

//BUG: when entering 0 sometimes the app doesn't exit on the first input

var matchesInLeague = new Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)>()
{
    {1, (Team1: "Belgium", Team2: "Canada", Team1Goals: 0, Team2Goals: 0, Winner: "", WinnerPoints: 0, PlayersThatScored: Array.Empty<string>())},
    {2, (Team1: "Morocco", Team2: "Croatia", Team1Goals: 0, Team2Goals: 0, Winner: "", WinnerPoints: 0, PlayersThatScored: Array.Empty<string>())},
    {3, (Team1: "Belgium", Team2: "Morocco", Team1Goals: 0, Team2Goals: 0, Winner: "", WinnerPoints: 0, PlayersThatScored: Array.Empty<string>())},
    {4, (Team1: "Croatia", Team2: "Canada", Team1Goals: 0, Team2Goals: 0, Winner: "", WinnerPoints: 0, PlayersThatScored: Array.Empty<string>())},
    {5, (Team1: "Croatia", Team2: "Belgium", Team1Goals: 0, Team2Goals: 0, Winner: "", WinnerPoints: 0, PlayersThatScored: Array.Empty<string>())},
    {6, (Team1: "Canada", Team2: "Morocco", Team1Goals: 0, Team2Goals: 0, Winner: "", WinnerPoints: 0, PlayersThatScored: Array.Empty<string>())}
};

int matchInProgress = 0;

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
                    int newRating = trainingSession(player.Value.Rating);
                    Console.WriteLine($"Player: {player.Key} | Position: {player.Value.Position} | Rating before: {player.Value.Rating} | Rating after practice: {newRating}\n");
                    players[player.Key] = (Position: player.Value.Position, Rating: newRating);
                }
                printDividingLine();
                break;
            }
        case 2:
            {
                matchInProgress++;
                if(matchInProgress > 6)
                {
                    Console.WriteLine("All matches from the group are already played!");
                    break;
                }
                playMatch(players, matchesInLeague, matchInProgress);
                break;
            }
        case 3:
            {
                statistics(players);
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
Dictionary<string,(string Position, int Rating)> assemblePlayersTeam()
{
    return new Dictionary<string, (string Position, int Rating)>()
    {
        {"Luka Modrić", (Position: "MF", Rating: 88)},
        {"Marcelo Brozović", (Position: "FW", Rating: 86)},
        {"Mateo Kovačić", (Position: "DF", Rating: 84)},
        {"Ivan Perišić", (Position: "MF", Rating: 84)},
        {"Andrej Kramarić", (Position: "MF", Rating: 82)},
        {"Ivan Rakitić", (Position: "MF", Rating: 82)},
        {"Joško Gvardiol", (Position: "DF",Rating: 81)},
        {"Marko Livaja", (Position: "FW", Rating: 77)},
        {"Marko Pjaca", (Position: "FW", Rating: 75)},
        {"Borna Barišić", (Position: "DF", Rating: 73)},
        {"Josip Vuković", (Position: "MF", Rating: 69)},
        {"Antonio Marin", (Position: "GK", Rating: 68)},
        {"Damjan Đoković", (Position: "DF", Rating: 70)},
        {"Domagoj Antolić", (Position: "FW", Rating: 72)},
        {"Josip Šutalo", (Position: "DF", Rating: 75)},
        {"Ante Budimir", (Position: "GK", Rating: 76)},
        {"Mario Pašalić", (Position: "MF", Rating: 81)},
        {"Lovro Majer", (Position: "FW", Rating: 80)},
        {"Ante Rebić", (Position: "DF", Rating: 80)},
        {"Franko Andrijašević", (Position: "MF", Rating: 72)},
        {"Fran Tudor", (Position: "MF", Rating: 72)},
        {"Dante Stipica", (Position: "MF", Rating: 71)},
        {"Ivan Santini", (Position: "FW", Rating: 69)},
        {"Dario Melnjak", (Position: "GK", Rating: 70)},
        {"Mato Miloš", (Position: "FW", Rating: 69)}
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
Dictionary<string, (string Position, int Rating)> formTeam(Dictionary<string, (string Position, int Rating)> players)
{
    var team = new Dictionary<string, (string Position, int Rating)>();

    //initializing seperate dictionaries for every position ordered by rating descending
    var goalkeepers = players.Where(p => p.Value.Position == "GK")
        .OrderByDescending(p => p.Value.Rating)
        .ToDictionary(p => p.Key, p => p.Value);

    var defenders = players.Where(p => p.Value.Position == "DF")
        .OrderByDescending(p => p.Value.Rating)
        .ToDictionary(p => p.Key, p => p.Value);

    var midfielders = players.Where(p => p.Value.Position == "MF")
        .OrderByDescending(p => p.Value.Rating)
        .ToDictionary(p => p.Key, p => p.Value);

    var strikers = players.Where(p => p.Value.Position == "FW")
        .ToDictionary(p => p.Key, p => p.Value);

    //TODO: handle if there are not enough players to form a team

    //adding all the players to the team
    team.Add(goalkeepers.ElementAt(0).Key, goalkeepers.ElementAt(0).Value);

    team.Add(defenders.ElementAt(0).Key, defenders.ElementAt(0).Value);
    team.Add(defenders.ElementAt(1).Key, defenders.ElementAt(1).Value);
    team.Add(defenders.ElementAt(2).Key, defenders.ElementAt(2).Value);
    team.Add(defenders.ElementAt(3).Key, defenders.ElementAt(3).Value);

    team.Add(midfielders.ElementAt(0).Key, midfielders.ElementAt(0).Value);
    team.Add(midfielders.ElementAt(1).Key, midfielders.ElementAt(1).Value);
    team.Add(midfielders.ElementAt(2).Key, midfielders.ElementAt(2).Value);

    var rand = new Random();
    var randomIndices = Enumerable.Range(0, strikers.Count).OrderBy(x => rand.Next()).Take(3).ToList();

    
    foreach (var ind in randomIndices)
        team.Add(strikers.ElementAt(ind).Key, strikers.ElementAt(ind).Value);

    return team;
}

/*
 * Increases a players rating in the players Dictionary
 * Returns a new updated dictionary and takes players name and percentage by which a rating should increase
*/
Dictionary<string, (string Position, int Rating)> changePlayerRating(Dictionary<string, (string Position, int Rating)> players, string playerName, int percentageNumber, bool shouldIncrease)
{
    //TODO: could use one method to training and increasing the rating after the game
    foreach (var player in players)
    {
        if (player.Key == playerName)
        {
            decimal percentage = ((decimal)percentageNumber) / 100;
            int newRating = shouldIncrease ?
                (int)(Math.Round(player.Value.Rating + player.Value.Rating * percentage, MidpointRounding.AwayFromZero))
                : (int)(Math.Round(player.Value.Rating - player.Value.Rating * percentage, MidpointRounding.AwayFromZero));
            if (newRating < 1)
                newRating = 1;
            else if (newRating > 100)
                newRating = 100;
            players[player.Key] = (Position: player.Value.Position, Rating: newRating);
        }
    }
    return players;
}

/*
 * Randomly generate match result and opponent, raise striker rating by 5% and other players by 2% if the team won
 * Print out the result and save it
 * Max number of matches that can be played are 6
*/
(string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored) playMatch(Dictionary<string, (string Position, int Rating)> allPlayers, Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int numberOfMatch)
{
    //calls function that forms the team of players that are playing the current match
    var team = formTeam(allPlayers);

    //TODO: check if team has all the players if it doesn't return null

    var strikers = allPlayers.Where(p => p.Value.Position == "FW");
    var restOfPlayers = allPlayers.Except(strikers);

    var matchInProgress = matches[numberOfMatch];

    //generate goals for both teams randomly
    var rand = new Random();
    int team1Goals = rand.Next(0, 5);
    int team2Goals = rand.Next(0, 5);
    while (team1Goals == team2Goals)
        team2Goals = rand.Next(0, 5);

    //calculate match winner and the amount of points they get
    string winner = (team1Goals > team2Goals) ? matchInProgress.Team1 : matchInProgress.Team2;
    int winnerPoints = (winner == matchInProgress.Team1) ? team1Goals : team2Goals;

    //generate players that scored the goals for croatia
    string[] playersThatScored = Array.Empty<string>();
    if (new string[2] {matchInProgress.Team1, matchInProgress.Team2}.Contains("Croatia"))
    {
        var croatiaGoals = (matchInProgress.Team1 == "Croatia") ? team1Goals : team2Goals;
        playersThatScored = new string[croatiaGoals];

        if(croatiaGoals > 0)
        {
            var rnd = new Random();
            int strikerInd = 0;
            for (int i = 0; i < croatiaGoals; i++)
            {
                strikerInd = rnd.Next(0, croatiaGoals);
                playersThatScored[i] = strikers.ElementAt(strikerInd).Key;
            }
        }

        //change player's ratings accordingly
        foreach (var striker in playersThatScored)
            changePlayerRating(allPlayers, striker, 5, true);

        foreach (var player in restOfPlayers)
            changePlayerRating(allPlayers, player.Key, 2, (winner == "Croatia") ? true : false);
    }

    return (Team1: matchInProgress.Team1, Team2: matchInProgress.Team2, Team1Goals: team1Goals, Team2Goals: team2Goals, Winner: winner, WinnerPoints: winnerPoints, PlayersThatScored: playersThatScored);
}

/*
 * Displays statistics menu and return users choice
*/
int statisticsMenu()
{
    bool success = false;
    int statisticsChoice = 0;

    do
    {
        Console.WriteLine("Print out all the players:\n\t1 - Default\n\t2 - By rating ascending\n\t3 - By rating descending\n\t4 - Filter by name and surname\n\t5 - Filter by rating\n\t6 - Filter by position\n\t7 - Top 11 players (4:3:3 positioning)\n\t8 - Strikers and the amounts of their scored goals\n9 - Print out results of Croatia\n10 - Print out results from all teams\n11 - Print out results table\n0 - Return to main menu\n");
        Console.Write("Input action: ");
        success = int.TryParse(Console.ReadLine(), out statisticsChoice);
        Console.WriteLine("\n");
    } while (!success);

    return statisticsChoice;
}

void statistics(Dictionary<string, (string Position, int Rating)> players)
{
    Console.Clear();

    int statisticsMenuChoice = 0;

    do
    {
        statisticsMenuChoice = statisticsMenu();

        switch (statisticsMenuChoice)
        {
            case 1:
                {
                    listPlayers(players);
                    break;
                }
            case 2:
                {
                    var playersByRatingAsc = players
                        .OrderBy(p => p.Value.Rating)
                        .ToDictionary(p => p.Key, p => p.Value);
                    listPlayers(playersByRatingAsc);
                    break;
                }
            case 3:
                {
                    var playersByRatinsDesc = players
                        .OrderByDescending(p => p.Value.Rating)
                        .ToDictionary(p => p.Key, p => p.Value);
                    listPlayers(playersByRatinsDesc);
                    break;
                }
            case 4:
                {
                    filterPlayerByName(players);
                    break;
                }
            case 5:
                {
                    filterPlayersByRating(players);
                    break;
                }
            case 6:
                {
                    filterPlayersByPosition(players);
                    break;
                }
            case 7:
                {
                    break;
                }
            case 8:
                {
                    break;
                }
            case 9:
                {
                    break;
                }
            case 10:
                {
                    break;
                }
            case 11:
                {
                    break;
                }
            case 0:
                {
                    Console.Clear();
                    break;
                }
            default:
                {
                    Console.WriteLine("There is no action for provided input!\n");
                    break;
                }
        }

    } while (statisticsMenuChoice != 0);
}

/*
 * Traverses the Dictionary and tries to match the players with user inputted filter,
 * if a match is found it prints out the position and rating of the matching player
*/
void filterPlayerByName(Dictionary<string, (string Position, int Rating)> players)
{
    Console.WriteLine("Player name: ");
    var playersName = Console.ReadLine();

    foreach (var player in players)
    {
        if(player.Key.ToLower() == playersName.ToLower().Trim())
        {
            Console.WriteLine($"Position: {player.Value.Position} | Rating: {player.Value.Rating}\n");
            return;
        }
    }
    Console.WriteLine("No matching players found!\n");
}

/*
 * Filters the Dictionary and prints out players with matching rating from user input
*/
void filterPlayersByRating(Dictionary<string, (string Position, int Rating)> players)
{
    bool success = false;
    int targetedRating = 0;

    do
    {
        Console.Write("Player rating: ");
        success = int.TryParse(Console.ReadLine(), out targetedRating);
    } while (!success);

    var targetedPlayers = players
        .Where(p => p.Value.Rating == targetedRating)
        .ToDictionary(p => p.Key, p => p.Value);

    if(targetedPlayers.Count == 0)
    {
        Console.WriteLine("\nThere are no players with targeted rating!\n");
        return;
    }

    listPlayers(targetedPlayers);
}

/*
 * Filters the Dictionary and prints out players with matching position from user input
*/
void filterPlayersByPosition(Dictionary<string, (string Position, int Rating)> players)
{
    Console.Write("Players position: ");
    var targetedPosition = Console.ReadLine();

    var targetedPlayers = players
        .Where(p => p.Value.Position.ToLower() == targetedPosition.ToLower().Trim())
        .ToDictionary(p => p.Key, p => p.Value);

    if(targetedPlayers.Count == 0)
    {
        Console.WriteLine("\nThere are no players with targeted position!\n");
        return;
    }

    listPlayers(targetedPlayers);
}

/*
 * Prints out all players from the Dictionary
*/
void listPlayers(Dictionary<string, (string Position, int Rating)> listOfPlayers)
{
    printDividingLine();
    foreach (var player in listOfPlayers)
    {
        Console.WriteLine($"Player: {player.Key} | Position: {player.Value.Position} | Rating: {player.Value.Rating}\n");
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