#region constants
const int TABLE_WIDTH = 125;

const string MAIN_MENU_TEXT = "1 - Training session\n"
            + "2 - Play a match\n"
            + "3 - Statistics\n"
            + "4 - Player menu\n"
            + "0 - Exit\n\n";

const string STATISTICS_MENU_TEXT = "\nPrint out all the players:\n"
            + "\t1 - Default\n"
            + "\t2 - By rating ascending\n"
            + "\t3 - By rating descending\n"
            + "\t4 - Filter by name\n"
            + "\t5 - Filter by rating\n"
            + "\t6 - Filter by position\n"
            + "\t7 - Top 11 players\n"
            + "\t8 - Strikers and the amounts of their scored goals\n"
            + "9 - Print out results of Croatia\n"
            + "10 - Print out results from all teams\n"
            + "11 - Print out results table\n"
            + "0 - Return to main menu\n";

const string PLAYER_CONTROL_MENU_TEXT = "\n1 - Add new player\n"
            + "2 - Delete a player\n"
            + "3 - Edit a player\n"
            + "0 - Return to main menu\n";

const string EDIT_PLAYER_MENU_TEXT = "\n1 - Edit name and surname of a player\n"
            + "2 - Edit player position\n"
            + "3 - Edit player rating\n"
            + "0 - Return to main menu\n";

const int MIN_RATING = 1;
const int MAX_RATING = 100;

string[] POSITIONS = { "GK", "DF", "MF", "FW" };

#endregion

var matchesInLeague = InitializeMatches();
var matchInProgress = 0;

var players = AssemblePlayersTeam();

var mainMenuChoice = 0;

do
{
    mainMenuChoice = GetMenuChoiceFromUser(MAIN_MENU_TEXT);

    switch (mainMenuChoice)
    {
        case 1:
            {
                Console.Clear();
                players = TrainingSessionMenuOption(players);
                break;
            }
        case 2:
            {
                Console.Clear();
                matchInProgress++;
                if (matchInProgress > 6)
                {
                    Console.WriteLine("All matches from the group are already played!");
                    break;
                }
                var isPlayed = PlayMatch(players, matchesInLeague, matchInProgress);
                matchInProgress = (isPlayed == null) ? matchInProgress-- : matchInProgress;
                if (isPlayed == null)
                    matchInProgress = matchInProgress == 0 ? 0 : matchInProgress--;
                else
                {
                    matchesInLeague[matchInProgress] = ((string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored))isPlayed;
                }

                PrintAllTeamsResults(matchesInLeague, matchInProgress);
                break;
            }
        case 3:
            {
                Console.Clear();
                Statistics(players, matchesInLeague, matchInProgress);
                break;
            }
        case 4:
            {
                Console.Clear();
                players = PlayerControl(players);
                break;
            }
        case 0:
            {
                Environment.Exit(0);
                break;
            }
        default:
            {
                Console.Clear();
                Console.WriteLine("There is no action for provided input!\n");
                break;
            }
    }

} while (mainMenuChoice != 0);

Console.ReadKey();

Dictionary<string, (string Position, int Rating)> AssemblePlayersTeam()
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

Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> InitializeMatches()
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

int TrainingSession(int currentRating)
{
    Random rand = new Random();
    var percentage = ((decimal)rand.Next(0, 5)) / 100;
    var newRating = (int)(Math.Round(currentRating + currentRating * percentage, MidpointRounding.AwayFromZero));

    return (currentRating == MAX_RATING || newRating >= MAX_RATING) ? MAX_RATING : newRating;
}

Dictionary<string, (string Position, int Rating)>? FormTeam(Dictionary<string, (string Position, int Rating)> players)
{
    var team = new Dictionary<string, (string Position, int Rating)>();

    //initializing seperate dictionaries for every position ordered by rating descending
    var goalkeepers = players.Where(p => p.Value.Position == POSITIONS[0])
        .OrderByDescending(p => p.Value.Rating)
        .ToDictionary(p => p.Key, p => p.Value);

    var defenders = players.Where(p => p.Value.Position == POSITIONS[1])
        .OrderByDescending(p => p.Value.Rating)
        .ToDictionary(p => p.Key, p => p.Value);

    var midfielders = players.Where(p => p.Value.Position == POSITIONS[2])
        .OrderByDescending(p => p.Value.Rating)
        .ToDictionary(p => p.Key, p => p.Value);

    var strikers = players.Where(p => p.Value.Position == POSITIONS[3])
        .ToDictionary(p => p.Key, p => p.Value);

    //checking if a team can be formed
    if (goalkeepers.Count < 1 || defenders.Count < 4 || midfielders.Count < 3 || strikers.Count < 3)
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

Dictionary<string, (string Position, int Rating)> ChangePlayerRating(Dictionary<string, (string Position, int Rating)> players, string playerName, int percentageNumber, bool shouldIncrease)
{
    foreach (var player in players)
    {
        if (player.Key == playerName)
        {
            var percentage = ((decimal)percentageNumber) / 100;
            var newRating = shouldIncrease ?
                (int)(Math.Round(player.Value.Rating + player.Value.Rating * percentage, MidpointRounding.AwayFromZero))
                : (int)(Math.Round(player.Value.Rating - player.Value.Rating * percentage, MidpointRounding.AwayFromZero));
            if (newRating < MIN_RATING)
                newRating = MIN_RATING;
            else if (newRating > MAX_RATING)
                newRating = MAX_RATING;
            players[player.Key] = (Position: player.Value.Position, Rating: newRating);
        }
    }
    return players;
}

(string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)? PlayMatch(Dictionary<string, (string Position, int Rating)> allPlayers, Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int numberOfMatch)
{
    //calls function that forms the team of players that are playing the current match
    var team = FormTeam(allPlayers);
    if (team == null)
        return null;

    var strikers = allPlayers.Where(p => p.Value.Position == "FW");
    var restOfPlayers = allPlayers.Except(strikers);

    var matchInProgress = matches[numberOfMatch];

    //generate goals for both teams randomly
    var rand = new Random();
    var team1Goals = rand.Next(0, 5);
    var team2Goals = rand.Next(0, 5);
    while (team1Goals == team2Goals)
        team2Goals = rand.Next(0, 5);

    //calculate match winner and the amount of points they get
    var winner = (team1Goals > team2Goals) ? matchInProgress.Team1 : matchInProgress.Team2;
    var winnerPoints = (winner == matchInProgress.Team1) ? team1Goals : team2Goals;

    //generate players that scored the goals for croatia
    var playersThatScored = Array.Empty<string>();
    if (new string[2] { matchInProgress.Team1, matchInProgress.Team2 }.Contains("Croatia"))
    {
        var croatiaGoals = (matchInProgress.Team1 == "Croatia") ? team1Goals : team2Goals;
        playersThatScored = new string[croatiaGoals];

        if (croatiaGoals > 0)
        {
            var rnd = new Random();
            var strikerInd = 0;
            for (var i = 0; i < croatiaGoals; i++)
            {
                strikerInd = rnd.Next(0, croatiaGoals);
                playersThatScored[i] = strikers.ElementAt(strikerInd).Key;
            }
        }

        //change player's ratings accordingly
        foreach (var striker in playersThatScored)
            ChangePlayerRating(allPlayers, striker, 5, true);

        foreach (var player in restOfPlayers)
            ChangePlayerRating(allPlayers, player.Key, 2, (winner == "Croatia") ? true : false);
    }

    return (Team1: matchInProgress.Team1, Team2: matchInProgress.Team2, Team1Goals: team1Goals, Team2Goals: team2Goals, Winner: winner, WinnerPoints: winnerPoints, PlayersThatScored: playersThatScored);
}

Dictionary<string, (string Position, int Rating)> TrainingSessionMenuOption(Dictionary<string,(string Position, int Rating)> players)
{
    PrintLine();
    PrintRow("Player", "Position", "Rating before practice", "Rating after practice");
    PrintLine();
    foreach (var player in players)
    {
        var newRating = TrainingSession(player.Value.Rating);
        PrintRow(player.Key, player.Value.Position, player.Value.Rating.ToString(), newRating.ToString());
        PrintLine();
        players[player.Key] = (Position: player.Value.Position, Rating: newRating);
    }
    Console.WriteLine("\n\n");
    return players;
}

void Statistics(Dictionary<string, (string Position, int Rating)> players, Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int playedMatches)
{
    Console.Clear();

    var statisticsMenuChoice = 0;

    do
    {
        statisticsMenuChoice = GetMenuChoiceFromUser(STATISTICS_MENU_TEXT);

        switch (statisticsMenuChoice)
        {
            case 1:
                {
                    Console.Clear();
                    ListPlayers(players);
                    break;
                }
            case 2:
                {
                    Console.Clear();
                    var playersByRatingAsc = players
                        .OrderBy(p => p.Value.Rating)
                        .ToDictionary(p => p.Key, p => p.Value);
                    ListPlayers(playersByRatingAsc);
                    break;
                }
            case 3:
                {
                    Console.Clear();
                    var playersByRatinsDesc = players
                        .OrderByDescending(p => p.Value.Rating)
                        .ToDictionary(p => p.Key, p => p.Value);
                    ListPlayers(playersByRatinsDesc);
                    break;
                }
            case 4:
                {
                    Console.Clear();
                    FilterPlayerByName(players);
                    break;
                }
            case 5:
                {
                    FilterPlayersByRating(players);
                    break;
                }
            case 6:
                {
                    Console.Clear();
                    FilterPlayersByPosition(players);
                    break;
                }
            case 7:
                {
                    Console.Clear();
                    FilterTop11(players);
                    break;
                }
            case 8:
                {
                    Console.Clear();
                    PrintStrikersAndGoals(players, matches, playedMatches);
                    break;
                }
            case 9:
                {
                    Console.Clear();
                    PrintTeamResult(matches, playedMatches);
                    break;
                }
            case 10:
                {
                    Console.Clear();
                    PrintAllTeamsResults(matches, playedMatches);
                    break;
                }
            case 11:
                {
                    Console.Clear();
                    PrintGroupsResults(matches, playedMatches);
                    break;
                }
            case 0:
                {
                    Console.Clear();
                    break;
                }
            default:
                {
                    Console.Clear();
                    Console.WriteLine("There is no action for provided input!\n");
                    break;
                }
        }

    } while (statisticsMenuChoice != 0);
}

void FilterPlayerByName(Dictionary<string, (string Position, int Rating)> players)
{
    Console.WriteLine("Player name: ");
    var playersName = Console.ReadLine();

    var playersWithName = players.Where(p => p.Key.ToLower().StartsWith(playersName.ToLower().Trim()))
        .ToDictionary(p => p.Key, p => p.Value);

    if (playersWithName.Count() == 0)
    {
        Console.WriteLine($"No matching players by name {playersName} found!\n");
        return;
    }

    Console.WriteLine("\n\n");
    PrintLine();
    PrintRow("Position", "Rating","Name");
    PrintLine();

    foreach (var player in playersWithName)
    {
        PrintRow(player.Value.Position, player.Value.Rating.ToString(),player.Key);
        PrintLine();
    }
}

void FilterPlayersByRating(Dictionary<string, (string Position, int Rating)> players)
{
    var success = false;
    var targetedRating = 0;

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

    ListPlayers(targetedPlayers);
}

void FilterPlayersByPosition(Dictionary<string, (string Position, int Rating)> players)
{
    Console.Write("Players position: ");
    var targetedPosition = Console.ReadLine();

    var targetedPlayers = players
        .Where(p => p.Value.Position.ToLower() == targetedPosition.ToLower().Trim())
        .ToDictionary(p => p.Key, p => p.Value);

    if (targetedPlayers.Count == 0)
    {
        Console.WriteLine("\nThere are no players with targeted position!\n");
        return;
    }

    ListPlayers(targetedPlayers);
}

void FilterTop11(Dictionary<string, (string Position, int Rating)> players)
{
    var sortedPlayers = players
        .OrderByDescending(p => p.Value.Rating)
        .ToDictionary(p => p.Key, p => p.Value);

    var top11Players = new Dictionary<string, (string Position, int Rating)>();

    if (players.Count == 0)
        Console.WriteLine("\nThere are no players on the list!\n");
    else if (players.Count > 11)
    {
        for (var i = 0; i < 11; i++)
            top11Players.Add(sortedPlayers.ElementAt(i).Key,sortedPlayers.ElementAt(i).Value);
    }

    ListPlayers(top11Players);
}

void PrintStrikersAndGoals(Dictionary<string, (string Position, int Rating)> players, Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int playedMatches)
{
    var strikersAndGoals = new Dictionary<string, int>();
    var strikers = players
        .Where(p => p.Value.Position == "FW")
        .ToDictionary(p => p.Key, p => p.Value);

    if (playedMatches == 0)
    {
        Console.WriteLine("\nThere are no matches played yet!\n");
        return;
    }

    if (strikers.Count == 0)
    {
        Console.WriteLine("\nThere are no strikers!\n");
        return;
    }

    foreach (var striker in strikers)
    {
        var numOfGoals = 0;

        foreach (var match in matches.Take(playedMatches))
        {
            if (match.Value.PlayersThatScored.Length != 0)
                numOfGoals += match.Value.PlayersThatScored.Where(p => p == striker.Key).Count();
        }
        strikersAndGoals.Add(striker.Key, numOfGoals);
    }

    foreach (var sg in strikersAndGoals)
        Console.WriteLine($"Striker: {sg.Key} | Goals scored: {sg.Value}\n");
}

void PrintTeamResult(Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int playedMatches)
{
    if (playedMatches < 2)
    {
        Console.WriteLine("\nCroatia hasn't played any matches yet!\n");
        return;
    }
    foreach (var match in matches.Take(playedMatches))
    {
        if (match.Value.Team1 == "Croatia" || match.Value.Team2 == "Croatia")
        {
            Console.Write($"\nTeam1: {match.Value.Team1} | Team2: {match.Value.Team2} | Result: {match.Value.Team1Goals} - {match.Value.Team2Goals} | Winner: {match.Value.Winner} | Winner points: {match.Value.WinnerPoints} | Strikers: ");

            foreach (var player in match.Value.PlayersThatScored)
                Console.Write(player + ", ");
        }
    }
}

void PrintAllTeamsResults(Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int playedMatches)
{
    if (playedMatches == 0)
    {
        Console.WriteLine("\nNo matches played so far!\n");
        return;
    }
    PrintLine();
    PrintRow("Team1","Team2","Result","Winner","Winner Points","Strikers");
    PrintLine();
    foreach (var match in matches)
    {
        var strikers = "";
        foreach (var player in match.Value.PlayersThatScored)
        {
            if (player.Equals(match.Value.PlayersThatScored.Last()))
                strikers += player;
            else
            {
                strikers += player + ", ";
            }
        }
        PrintRow(match.Value.Team1, match.Value.Team2, match.Value.Team1Goals.ToString() + " : " + match.Value.Team2Goals.ToString(), match.Value.Winner, match.Value.WinnerPoints.ToString(),strikers);
        PrintLine();
    }
    Console.WriteLine("\n\n");
}

void PrintGroupsResults(Dictionary<int, (string Team1, string Team2, int Team1Goals, int Team2Goals, string Winner, int WinnerPoints, string[] PlayersThatScored)> matches, int playedMatches)
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
        var currentPoints = groupTable[match.Value.Winner].Points;
        var currentGoalDifference = groupTable[match.Value.Winner].GoalDifference;

        var newGoalDifference = match.Value.Winner == match.Value.Team1
            ? match.Value.Team1Goals - match.Value.Team2Goals
            : match.Value.Team2Goals - match.Value.Team1Goals;

        groupTable[match.Value.Winner] = (Points: currentPoints + match.Value.WinnerPoints, GoalDifference: currentGoalDifference + newGoalDifference);
    }

    groupTable.OrderByDescending(g => g.Value.Points)
        .ToDictionary(g => g.Key, g => g.Value);

    var i = 1;
    Console.WriteLine("Place - Name | Points | Goal difference");

    PrintLine();
    PrintRow("Position", "Name", "Points", "Goal Difference");

    foreach (var group in groupTable)
    {
        PrintRow(i.ToString(), group.Key, group.Value.Points.ToString(), group.Value.GoalDifference.ToString());
        PrintLine();
        i++;
    }

    Console.WriteLine("\n\n");
}

Dictionary<string, (string Position, int Rating)> PlayerControl(Dictionary<string, (string Position, int Rating)> players)
{
    Console.Clear();
    var playerControlMenuChoice = 0;

    do
    {
        playerControlMenuChoice = GetMenuChoiceFromUser(PLAYER_CONTROL_MENU_TEXT);

        switch (playerControlMenuChoice)
        {
            case 1:
                {
                    Console.Clear();
                    var playerAdded = AddNewPlayer(players);
                    if (playerAdded != null)
                    {
                        Console.WriteLine("\nPlayer successfully added!\n");
                        players = playerAdded;
                    }
                    break;
                }
            case 2:
                {
                    Console.Clear();
                    var playerDeleted = DeletePlayer(players);
                    if (playerDeleted != null)
                    {
                        Console.WriteLine("\nPlayer successfully removed!\n");
                    }
                    break;
                }
            case 3:
                {
                    Console.Clear();
                    players = EditPlayer(players);
                    break;
                }
            case 0:
                {
                    Console.Clear();
                    break;
                }
            default:
                {
                    Console.Clear();
                    Console.WriteLine("There is no action for provided input!\n");
                    break;
                }
        }
    } while (playerControlMenuChoice != 0);

    return players;
}

Dictionary<string, (string Position, int Rating)>? AddNewPlayer(Dictionary<string, (string Position, int Rating)> allPlayers)
{
    var playerRating = 0;

    //no more than 26 players can be on the list
    if (allPlayers.Count > 26)
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
    else if (playerName.Trim() == "")
    {
        Console.WriteLine("\nPlayer name cannot be empty!\n");
        return null;
    }

    //check if position entered is a valid one
    Console.Write("\nInput player position: ");
    var playerPosition = Console.ReadLine();

    if (!POSITIONS.Contains(playerPosition.ToUpper()))
    {
        Console.WriteLine("\nInvalid player position! Player position must be GK, DF, MF or FW\n");
        return null;
    }
    else if (playerPosition.Trim() == "")
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
    else if (playerRating < MIN_RATING || playerRating > MAX_RATING)
    {
        Console.WriteLine("\nPlayer rating needs to be a value in range between 1 and 100!\n");
        return null;
    }

    //user conformation
    Console.Write($"\nAre you sure you want to add new player: Name: {playerName} | Position: {playerPosition} | Rating: {playerRating} to the list (y/n): ");
    var shouldAdd = Console.ReadLine().ToLower();

    if (shouldAdd == "y")
    {
        players.Add(playerName, (Position: playerPosition, Rating: playerRating));
        return players;
    }

    return null;
}

Dictionary<string, (string Position, int Rating)>? DeletePlayer(Dictionary<string, (string Position, int Rating)> players)
{
    Console.Write("\nEnter name of a player you want to delete: ");
    var playerToDelete = Console.ReadLine();

    //user conformation
    if (players.Keys.Contains(playerToDelete))
    {
        Console.Write($"\nAre you sure you want to delete player: {playerToDelete} (y/n): ");
        var shouldDelete = Console.ReadLine().ToLower();

        if (shouldDelete == "y")
        {
            players.Remove(playerToDelete);
            return players;
        }
    }

    Console.WriteLine($"\nThere is no player: ${playerToDelete} on the team!\n");
    return null;
}

Dictionary<string, (string Position, int Rating)> EditPlayer(Dictionary<string, (string Position, int Rating)> players)
{
    Console.Clear();

    var editPlayerMenuChoice = 0;

    do
    {
        editPlayerMenuChoice = GetMenuChoiceFromUser(EDIT_PLAYER_MENU_TEXT);

        Console.Write("\nEnter name of player whose name you want to edit: ");
        var playerToEdit = Console.ReadLine().ToLower().Trim();

        switch (editPlayerMenuChoice)
        {
            case 1:
                {
                    Console.Clear();
                    players = EditPlayerName(players, playerToEdit);
                    break;
                }
            case 2:
                {
                    Console.Clear();
                    players = EditPlayerPosition(players, playerToEdit);
                    break;
                }
            case 3:
                {
                    Console.Clear();
                    players = EditPlayerRating(players, playerToEdit);
                    break;
                }
            case 0:
                {
                    Console.Clear();
                    break;
                }
            default:
                {
                    Console.Clear();
                    Console.WriteLine("There is no action for provided input!\n");
                    break;
                }
        }
    } while (editPlayerMenuChoice != 0);

    return players;
}

Dictionary<string, (string Position, int Rating)> EditPlayerName(Dictionary<string, (string Position, int Rating)> players, string playerToEdit)
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
    {
        Console.WriteLine($"\nPlayer of name {playerToEdit} not found!");
    }
        
    return players;
}

Dictionary<string, (string Position, int Rating)> EditPlayerPosition(Dictionary<string, (string Position, int Rating)> players, string playerToEdit)
{
    if (players.Keys.Contains(playerToEdit))
    {
        Console.Write("\nEnter new player position: ");
        var newPosition = Console.ReadLine().ToUpper();

        if (!POSITIONS.Contains(newPosition))
            Console.WriteLine("\nPlayer position not valid! Player position must be GK, DF, MF or FW\n");
        else
        {
            players[playerToEdit] = (Position: newPosition, Rating: players[playerToEdit].Rating);
        }
    }
    else
    {
        Console.WriteLine($"\nPlayer of name {playerToEdit} not found!");
    }
   
    return players;
}

Dictionary<string, (string Position, int Rating)> EditPlayerRating(Dictionary<string, (string Position, int Rating)> players, string playerToEdit)
{
    if (players.Keys.Contains(playerToEdit))
    {
        var success = false;
        var newRating = 0;
        Console.Write("\nEnter new player rating: ");
        success = int.TryParse(Console.ReadLine(), out newRating);

        if (!success)
        {
            Console.WriteLine("\nRating has to be a number!\n");
        }
        else if (newRating < MIN_RATING || newRating > MAX_RATING)
        {
            Console.WriteLine("\nRating has to be between 1 and 100!\n");
        }
        else
        {
            players[playerToEdit] = (Position: players[playerToEdit].Position, Rating: newRating);
        }
    }
    else
    {
        Console.WriteLine($"\nPlayer of name {playerToEdit} not found!");
    }
        
    return players;
}

void ListPlayers(Dictionary<string, (string Position, int Rating)> listOfPlayers)
{
    PrintLine();
    PrintRow("Player", "Position", "Rating");
    PrintLine();

    foreach (var player in listOfPlayers)
    {
        PrintRow(player.Key, player.Value.Position, player.Value.Rating.ToString());
        PrintLine();
    }
}

int GetMenuChoiceFromUser(string menuText)
{
    var success = false;
    var choice = 0;
    do
    {
        Console.WriteLine(menuText);

        Console.Write("Input action: ");

        success = int.TryParse(Console.ReadLine(), out choice);

        if (!success)
            Console.WriteLine("You must enter a number!");

        Console.WriteLine("\n");
    } while (!success);

    return choice;
}

//<<<---------- METHODS FOR PRINTING TABLES ---------->>>
void PrintLine(int width = 0)
{
    Console.WriteLine(new string('-', width != 0 ? width : TABLE_WIDTH));
}

void PrintRow(params string[] columns)
{
    var width = (TABLE_WIDTH - columns.Length) / columns.Length;
    var row = "|";

    foreach (var column in columns)
    {
        row += AlignCentre(column, width) + "|";
    }

    Console.WriteLine(row);
}

string AlignCentre(string text, int width)
{
    if (string.IsNullOrEmpty(text))
    {
        return new string(' ', width);
    }
    else
    {
        return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
    }
}