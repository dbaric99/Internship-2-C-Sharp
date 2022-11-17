using System.Collections.Generic;

//GK, DF, MF, FW - goalkeeper, defender, midfielder, forward
//Initializing the starting players
var soccerTeam = new Dictionary<string, (string position, int rating)>()
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

Console.WriteLine("This is your choice: " + mainMenu());

Console.ReadKey();




int mainMenu()
{
    bool success = false;
    int choice = 0;

    do
    {
        Console.WriteLine("1 - Odradi trening\n2 - Odigraj utakmicu\n3 - Statistika\n4 - Kontrola igraca\n0 - Izlaz iz aplikacije");
        Console.Write("Unesi svoj izbor: ");
        success = int.TryParse(Console.ReadLine(), out choice);
        Console.WriteLine("\n");
    } while (!success || (choice < 0 || choice > 4));

    return choice;
}