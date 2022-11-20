using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

var matchesInLeague = initializeMatches();
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
                var isPlayed = playMatch(players, matchesInLeague, matchInProgress);
                matchInProgress = (isPlayed == null) ? matchInProgress-- : matchInProgress;
                if (isPlayed == null)
                    matchInProgress = matchInProgress == 0 ? 0 : matchInProgress--;
                else
                    matchesInLeague[matchInProgress] = ((string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored))isPlayed;
                break;
            }
        case 3:
            {
                statistics(players, matchesInLeague, matchInProgress);
                break;
            }
        case 4:
            {
                players = playerControl(players);
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
        {"Franko Andrijašević", (Position: "MF", Rating: 72)}
    };
}

/* 
 * Returns dictionary of matches from the league
*/
Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> initializeMatches()
{
    return new Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)>()
    {
        {1, (Team1: "Belgium", Team2: "Canada", Team1Goals: 0, Team2Goals: 0, Winner: "", WinnerPoints: 0, PlayersThatScored: Array.Empty<string>())},
        {2, (Team1: "Morocco", Team2: "Croatia", Team1Goals: 0, Team2Goals: 0, Winner: "", WinnerPoints: 0, PlayersThatScored: Array.Empty<string>())},
        {3, (Team1: "Belgium", Team2: "Morocco", Team1Goals: 0, Team2Goals: 0, Winner: "", WinnerPoints: 0, PlayersThatScored: Array.Empty<string>())},
        {4, (Team1: "Croatia", Team2: "Canada", Team1Goals: 0, Team2Goals: 0, Winner: "", WinnerPoints: 0, PlayersThatScored: Array.Empty<string>())},
        {5, (Team1: "Croatia", Team2: "Belgium", Team1Goals: 0, Team2Goals: 0, Winner: "", WinnerPoints: 0, PlayersThatScored: Array.Empty<string>())},
        {6, (Team1: "Canada", Team2: "Morocco", Team1Goals: 0, Team2Goals: 0, Winner: "", WinnerPoints: 0, PlayersThatScored: Array.Empty<string>())}
    };
}

/* 
 * Used for display of the main menu and return of user choice
*/
int mainMenu()
{
    bool success = false;
    int choice = 0;

    do
    {
        Console.WriteLine("1 - Training session\n"
            + "2 - Play a match\n"
            + "3 - Statistics\n"
            + "4 - Player menu\n"
            + "0 - Exit");

        Console.Write("Input action: ");

        success = int.TryParse(Console.ReadLine(), out choice);
        Console.WriteLine("\n");
    } while (!success);

    return choice;
}

/*
 * Used for increasing players rating by 0-5% randomly
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
 * Returns Dictionary of players that will participate in the game
*/
Dictionary<string, (string Position, int Rating)>? formTeam(Dictionary<string, (string Position, int Rating)> players)
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

    //checking if a team can be formed
    if(goalkeepers.Count < 1 || defenders.Count < 4 || midfielders.Count < 3 || strikers.Count < 3)
    {
        Console.WriteLine("\nThere aren't enough players to form a team!\n");
        return null;
    }

    //adding all the players to the team
    foreach (var goalkeeper in goalkeepers.Take(1))
        team.Add(goalkeeper.Key, goalkeeper.Value);

    foreach (var defender in defenders.Take(4))
        team.Add(defender.Key, defender.Value);

    foreach (var midfielder in midfielders.Take(3))
        team.Add(midfielder.Key, midfielder.Value);

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
 * Randomly generate match result and opponent, raise striker rating by 5% and other players by 2% if the team won, and decrease by 2% if they lost
 * Print out the result and save it
 * Max number of matches that can be played are 6
*/
(string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)? playMatch(Dictionary<string, (string Position, int Rating)> allPlayers, Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int numberOfMatch)
{
    //calls function that forms the team of players that are playing the current match
    var team = formTeam(allPlayers);
    if (team == null)
        return null;

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
        Console.WriteLine("\nPrint out all the players:\n"
            + "\t1 - Default\n"
            + "\t2 - By rating ascending\n"
            + "\t3 - By rating descending\n"
            + "\t4 - Filter by name and surname\n"
            + "\t5 - Filter by rating\n"
            + "\t6 - Filter by position\n"
            + "\t7 - Top 11 players\n"
            + "\t8 - Strikers and the amounts of their scored goals\n"
            + "9 - Print out results of Croatia\n"
            + "10 - Print out results from all teams\n"
            + "11 - Print out results table\n"
            + "0 - Return to main menu\n");

        Console.Write("Input action: ");
        success = int.TryParse(Console.ReadLine(), out statisticsChoice);
        Console.WriteLine("\n");
    } while (!success);

    return statisticsChoice;
}

/*
 * Contains logic for the statistics menu
*/
void statistics(Dictionary<string, (string Position, int Rating)> players, Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int playedMatches)
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
                    filterTop11(players);
                    break;
                }
            case 8:
                {
                    printStrikersAndGoals(players, matches, playedMatches);
                    break;
                }
            case 9:
                {
                    printTeamResult(matches, playedMatches);
                    break;
                }
            case 10:
                {
                    printAllTeamsResults(matches, playedMatches);
                    break;
                }
            case 11:
                {
                    printGroupsResults(matches, playedMatches);
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

    Console.Write("Player rating: ");
    success = int.TryParse(Console.ReadLine(), out targetedRating);

    if (!success)
    {
        Console.WriteLine("\nPlayer rating must be a number!\n");
        return;
    }

    var targetedPlayers = players
        .Where(p => p.Value.Rating == targetedRating)
        .ToDictionary(p => p.Key, p => p.Value);

    if (targetedPlayers.Count == 0)
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
 * Filters top 11 players
*/
void filterTop11(Dictionary<string, (string Position, int Rating)> players)
{
    var top11 = players
        .OrderByDescending(p => p.Value.Rating)
        .ToDictionary(p => p.Key, p => p.Value);

    if (players.Count == 0)
        Console.WriteLine("\nThere are no players on the list!\n");
    else if (players.Count > 11)
        top11 = (Dictionary<string, (string Position, int Rating)>)top11.Take(11);

    listPlayers(players);
}

/*
 * Traverses Dictionaries for players and counts strikers goals and prints out the data
*/
void printStrikersAndGoals(Dictionary<string, (string Position, int Rating)> players, Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int playedMatches)
{
    var strikersAndGoals = new Dictionary<string, int>();
    var strikers = players
        .Where(p => p.Value.Position == "FW")
        .ToDictionary(p => p.Key, p => p.Value);

    if(playedMatches == 0)
    {
        Console.WriteLine("\nThere are no matches played yet!\n");
        return;
    }

    if(strikers.Count == 0)
    {
        Console.WriteLine("\nThere are no strikers!\n");
        return;
    }

    foreach (var striker in strikers)
    {
        var numOfGoals = 0;

        foreach(var match in matches.Take(playedMatches))
        {
            if(match.Value.PlayersThatScored.Length != 0)
                numOfGoals += match.Value.PlayersThatScored.Where(p => p == striker.Key).Count();
        }
        strikersAndGoals.Add(striker.Key, numOfGoals);
    }

    foreach (var sg in strikersAndGoals)
        Console.WriteLine($"Striker: {sg.Key} | Goals scored: {sg.Value}\n");
}

/*
 * Displays results of matches Croatia has played in
*/
void printTeamResult(Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int playedMatches)
{
    if(playedMatches < 2)
    {
        Console.WriteLine("\nCroatia hasn't played any matches yet!\n");
        return;
    }
    printDividingLine();
    foreach (var match in matches.Take(playedMatches))
    {
        if(match.Value.Team1 == "Croatia" || match.Value.Team2 == "Croatia")
        {
            Console.Write($"\nTeam1: {match.Value.Team1} | Team2: {match.Value.Team2} | Result: {match.Value.Team1Goals} - {match.Value.Team2Goals} | Winner: {match.Value.Winner} | Winner points: {match.Value.WinnerPoints} | Strikers: ");

            foreach (var player in match.Value.PlayersThatScored)
                Console.Write(player + ", ");
        }
    }
    printDividingLine();
}

/*
 * Displays results of all matches played so far
*/
void printAllTeamsResults(Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int playedMatches)
{
    if(playedMatches == 0)
    {
        Console.WriteLine("\nNo matches played so far!\n");
        return;
    }
    printDividingLine();
    foreach (var match in matches)
    {
        Console.Write($"\nTeam1: {match.Value.Team1} | Team2: {match.Value.Team2} | Result: {match.Value.Team1Goals} - {match.Value.Team2Goals} | Winner: {match.Value.Winner} | Winner points: {match.Value.WinnerPoints} | Strikers: ");

        foreach (var player in match.Value.PlayersThatScored)
            Console.Write(player + ", ");
    }
    printDividingLine();
}

/*
 * Display current ratings of all teams played so far
*/
void printGroupsResults(Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int playedMatches)
{
    var groupTable = new Dictionary<string, (int Points, int GoalDifference)>()
    {
        {"Belgium", (Points: 0, GoalDifference: 0)},
        {"Canada", (Points: 0, GoalDifference: 0)},
        {"Croatia", (Points: 0, GoalDifference: 0)},
        {"Morocco", (Points: 0, GoalDifference: 0)},
    };

    foreach (var match in matches.Take(playedMatches))
    {
        int currentPoints = groupTable[match.Value.Winner].Points;
        int currentGoalDifference = groupTable[match.Value.Winner].GoalDifference;

        int newGoalDifference = match.Value.Winner == match.Value.Team1
            ? match.Value.Team1Goals - match.Value.Team2Goals
            : match.Value.Team2Goals - match.Value.Team1Goals;

        groupTable[match.Value.Winner] = (Points: currentPoints + match.Value.WinnerPoints, GoalDifference: currentGoalDifference + newGoalDifference);
    }

    groupTable.OrderByDescending(g => g.Value.Points)
        .ToDictionary(g => g.Key, g => g.Value);

    printDividingLine();
    int i = 1;
    Console.WriteLine("Place - Name | Points | Goal difference");
    foreach (var group in groupTable)
    {
        Console.WriteLine($"{i} - {group.Key} | Points: {group.Value.Points} | Goal difference: {group.Value.GoalDifference}");
        i++;
    }
    printDividingLine();
}

/*
 * Displays player control menu and returns user choice
*/
int playerControlMenu()
{
    bool success = false;
    int playerControlChoice = 0;

    do
    {
        Console.WriteLine("\n1 - Add new player\n"
            + "2 - Delete a player\n"
            + "3 - Edit a player\n");

        Console.Write("Input action: ");
        success = int.TryParse(Console.ReadLine(), out playerControlChoice);
        Console.WriteLine("\n");
    } while (!success);

    return playerControlChoice;
}

/*
 * Logic for player CRUD operations
*/
Dictionary<string, (string Position, int Rating)> playerControl(Dictionary<string, (string Position, int Rating)> players)
{
    Console.Clear();
    int playerControlMenuChoice = 0;

    do
    {
        playerControlMenuChoice = playerControlMenu();

        switch (playerControlMenuChoice)
        {
            case 1:
                {
                    var playerAdded = addNewPlayer(players);
                    if (playerAdded != null)
                    {
                        Console.WriteLine("\nPlayer successfully added!\n");
                        players = playerAdded;
                    }
                    break;
                }
            case 2:
                {
                    var playerDeleted = deletePlayer(players);
                    if(playerDeleted != null)
                    {
                        Console.WriteLine("\nPlayer successfully removed!\n");
                    }
                    break;
                }
            case 3:
                {
                    players = editPlayer(players);
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
    } while (playerControlMenuChoice != 0);

    return players;
}

/*
 * Create new player and add it to the list
*/
Dictionary<string, (string Position, int Rating)>? addNewPlayer(Dictionary<string, (string Position, int Rating)> allPlayers)
{
    int playerRating;

    //no more than 26 players can be on the list
    if(allPlayers.Count > 26)
    {
        Console.WriteLine("\nThere is maximum amount of players already!\n");
        return null;
    }

    //check if the new name is unique
    Console.Write("\nInput name of the new player: ");
    var playerName = Console.ReadLine();

    if (allPlayers.ContainsKey(playerName))
    {
        Console.WriteLine("\nPlayer with same name already exists!\n");
        return null;
    }
    else if(playerName.Trim() == "")
    {
        Console.WriteLine("\nPlayer name cannot be empty!\n");
        return null;
    }

    //check if position entered is a valid one
    Console.Write("\nInput player position: ");
    var playerPosition = Console.ReadLine();

    if(!new string[4] {"GK", "DF", "MF", "FW" }.Contains(playerPosition.ToUpper()))
    {
        Console.WriteLine("\nInvalid player position!\n");
        return null;
    }
    else if(playerPosition.Trim() == "")
    {
        Console.WriteLine("\nPlayer position cannot be empty!\n");
        return null;
    }

    //check if rating is a number between 1 and 100
    Console.Write("\nInput player rating: ");
    var success = int.TryParse(Console.ReadLine(), out playerRating);

    if (!success)
    {
        Console.WriteLine("\nPlayer rating needs to be a number!\n");
        return null;
    }
    else if(playerRating < 1 || playerRating > 100)
    {
        Console.WriteLine("\nPlayer rating needs to be a value in range between 1 and 100!\n");
        return null;
    }

    //user conformation
    Console.Write($"\nAre you sure you want to add new player: Name: {playerName} | Position: {playerPosition} | Rating: {playerRating} to the list (y/n): ");
    var shouldAdd = Console.ReadLine().ToLower();

    if(shouldAdd == "y")
    {
        players.Add(playerName, (Position: playerPosition, Rating: playerRating));
        return players;
    }

    return null;
}

/*
 * Delete a player from the list
*/
Dictionary<string, (string Position, int Rating)>? deletePlayer(Dictionary<string, (string Position, int Rating)> players)
{
    Console.Write("\nEnter name of a player you want to delete: ");
    var playerToDelete = Console.ReadLine();

    //user conformation
    if (players.Keys.Contains(playerToDelete))
    {
        Console.Write($"\nAre you sure you want to delete player: {playerToDelete} (y/n): ");
        var shouldDelete = Console.ReadLine().ToLower();

        if(shouldDelete == "y")
        {
            players.Remove(playerToDelete);
            return players;
        }
    }

    Console.WriteLine($"\nThere is no player: ${playerToDelete} on the team!\n");
    return null;
}

/*
 * Displays edit menu and returns user choice
*/
int editPlayerMenu()
{
    bool success = false;
    int editPlayerMenuChoice = 0;

    printDividingLine();

    do
    {
        Console.WriteLine("\n1 - Edit name and surname of a player\n"
            + "2 - Edit player position\n"
            + "3 - Edit player rating\n"
            + "0 - Return to main menu\n");

        Console.Write("Input action: ");
        success = int.TryParse(Console.ReadLine(), out editPlayerMenuChoice);
        Console.WriteLine("\n");
    } while (!success);

    return editPlayerMenuChoice;
}

/*
 * Logic for editing players
*/
Dictionary<string, (string Position, int Rating)> editPlayer(Dictionary<string, (string Position, int Rating)> players)
{
    Console.Clear();

    int editPlayerMenuChoice = 0;

    do
    {
        editPlayerMenuChoice = editPlayerMenu();

        Console.Write("\nEnter name of player whose name you want to edit: ");
        var playerToEdit = Console.ReadLine();

        switch (editPlayerMenuChoice)
        {
            case 1:
                {
                    players = editPlayerName(players, playerToEdit);
                    break;
                }
            case 2:
                {
                    players = editPlayerPosition(players, playerToEdit);
                    break;
                }
            case 3:
                {
                    players = editPlayerRating(players, playerToEdit);
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
    } while (editPlayerMenuChoice != 0);

    return players;
}

/*
 * Edit name of selected player
*/
Dictionary<string, (string Position, int Rating)> editPlayerName(Dictionary<string, (string Position, int Rating)> players, string playerToEdit)
{
    if (players.Keys.Contains(playerToEdit))
    {
        Console.Write("\nEnter new player name: ");
        var newName = Console.ReadLine();

        if (players.ContainsKey(newName))
            Console.WriteLine($"\nPlayer by name {newName} already exists!\n");
        else
        {
            players.Add(playerToEdit, players[playerToEdit]);
            players.Remove(playerToEdit);
            Console.WriteLine("\nPlayer name edit successful!\n");
        }
    }
    else
        Console.WriteLine($"\nPlayer of name {playerToEdit} not found!");
    return players;
}

/*
 * Edit position of selected player
*/
Dictionary<string, (string Position, int Rating)> editPlayerPosition(Dictionary<string, (string Position, int Rating)> players, string playerToEdit)
{
    if (players.Keys.Contains(playerToEdit))
    {
        Console.Write("\nEnter new player position: ");
        var newPosition = Console.ReadLine().ToUpper();

        if(!new string[4] {"GK", "DF", "MF", "FW" }.Contains(newPosition))
            Console.WriteLine("\nPlayer position not valid!\n");
        else
            players[playerToEdit] = (Position: newPosition, Rating: players[playerToEdit].Rating);
    }
    else
        Console.WriteLine($"\nPlayer of name {playerToEdit} not found!");
    return players;
}

/*
 * Edit rating of selected player
*/
Dictionary<string, (string Position, int Rating)> editPlayerRating(Dictionary<string, (string Position, int Rating)> players, string playerToEdit)
{
    if (players.Keys.Contains(playerToEdit))
    {
        bool success = false;
        int newRating = 0;
        Console.Write("\nEnter new player rating: ");
        success = int.TryParse(Console.ReadLine(), out newRating);

        if (!success)
        {
            Console.WriteLine("\nRating has to be a number!\n");
        }
        else if(newRating < 1 || newRating > 100)
        {
            Console.WriteLine("\nRating has to be between 1 and 100!\n");
        }
        else
        {
            players[playerToEdit] = (Position: players[playerToEdit].Position, Rating: newRating);
        }
    }
    else
        Console.WriteLine($"\nPlayer of name {playerToEdit} not found!");
    return players;
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
 * Prints a dividing line, used for printing matches or players
*/
void printDividingLine()
{
    Console.WriteLine("\n\n<<----------------------------------->>\n\n");
}