using System;
using System.Collections.Generic;

public class Program
{
    static Spelare spelaren;

    public static void Main(string[] args)
    {
        Console.Title = "Fantastiskt Äventyr";
        Console.ForegroundColor = ConsoleColor.Green;

        char[,] spelplan = new char[10, 10];

        spelaren = new Spelare(0, 0, '@');
        PlaceraEntitet(spelplan, spelaren);

        Varelse monster1 = new Varelse(3, 5, 'M', ConsoleColor.Red);
        monster1.Livskraft = 100;
        monster1.LäggTillFörmåga(new Förmåga("Sparka", s => { s.Livskraft -= 25; }));
        PlaceraEntitet(spelplan, monster1);

        Varelse monster2 = new Varelse(7, 2, 'M', ConsoleColor.Red);
        monster2.Livskraft = 100;
        monster2.LäggTillFörmåga(new Förmåga("Sparka", s => { s.Livskraft -= 25; }));
        PlaceraEntitet(spelplan, monster2);

         Varelse monster3 = new Varelse(5, 4, 'M', ConsoleColor.Red);
        monster3.Livskraft = 100;
        monster3.LäggTillFörmåga(new Förmåga("Sparka", s => { s.Livskraft -= 25; }));
        PlaceraEntitet(spelplan, monster3);


        Hälsopaket hälsopaket1 = new Hälsopaket(5, 8, '+', ConsoleColor.Blue);
        PlaceraEntitet(spelplan, hälsopaket1);

          Hälsopaket hälsopaket2 = new Hälsopaket(7, 3, '+', ConsoleColor.Blue);
        PlaceraEntitet(spelplan, hälsopaket2);

          Hälsopaket hälsopaket3 = new Hälsopaket(2, 8, '+', ConsoleColor.Blue);
        PlaceraEntitet(spelplan, hälsopaket3);

        while (true)
        {
            RitaSpelplan(spelplan);

            ConsoleKeyInfo keyInfo = Console.ReadKey();
            HanteraSpelarensDrag(spelplan, keyInfo);

            foreach (Entitet entitet in new List<Entitet> { monster1, monster2,monster3, hälsopaket1,hälsopaket2, hälsopaket3 })
            {
                if (spelaren.X == entitet.X && spelaren.Y == entitet.Y)
                {
                    if (entitet is Varelse)
                    {
                        MötVarelse((Varelse)entitet);
                    }
                    else if (entitet is Hälsopaket)
                    {
                        MötHälsopaket((Hälsopaket)entitet);
                    }

                    RitaSpelplan(spelplan);
                }
            }

            if (spelaren.Livskraft <= 0)
            {
                Console.WriteLine("Du förlorade! Spelet är över.");
                break;
            }

            Console.Clear();
        }
    }

    public static void RitaSpelplan(char[,] spelplan)
    {
        Console.Clear();

        int rows = spelplan.GetLength(0);
        int cols = spelplan.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.ForegroundColor = ConsoleColor.Gray;

                if (spelplan[i, j] == '\0')
                {
                    Console.Write(". ");
                }
                else if (spelplan[i, j] == spelaren.Symbol)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(@"\S/");
                }
                else if (spelplan[i, j] == 'M')
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write('M' + " ");
                }
                else if (spelplan[i, j] == '+')
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write('+' + " ");
                }
            }
            Console.WriteLine();
        }

        Console.WriteLine($"Din hälsa: {spelaren.Livskraft}%");
    }

    public static void PlaceraEntitet(char[,] spelplan, Entitet entitet)
    {
        spelplan[entitet.Y, entitet.X] = entitet.Symbol;
    }

    public static void HanteraSpelarensDrag(char[,] spelplan, ConsoleKeyInfo keyInfo)
    {
        spelplan[spelaren.Y, spelaren.X] = '\0';

        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                if (spelaren.Y > 0)
                    spelaren.Y--;
                break;
            case ConsoleKey.DownArrow:
                if (spelaren.Y < spelplan.GetLength(0) - 1)
                    spelaren.Y++;
                break;
            case ConsoleKey.LeftArrow:
                if (spelaren.X > 0)
                    spelaren.X--;
                break;
            case ConsoleKey.RightArrow:
                if (spelaren.X < spelplan.GetLength(1) - 1)
                    spelaren.X++;
                break;
        }

        PlaceraEntitet(spelplan, spelaren);
    }

    public static void MötVarelse(Varelse varelse)
    {
        Console.WriteLine($"Du möter en varelse med symbol {varelse.Symbol}");
        Console.WriteLine($"Varelsen har {varelse.Livskraft}% hälsa.");

        Random random = new Random();
        int index = random.Next(varelse.Förmågor.Count);
        Förmåga förmåga = varelse.Förmågor[index];

        Console.WriteLine($"Varelsen använder förmågan: {förmåga.Namn}");
        förmåga.Använd(spelaren);
    }

    public static void MötHälsopaket(Hälsopaket hälsopaket)
    {
        Console.WriteLine($"Du hittade ett hälsopaket med symbol {hälsopaket.Symbol}");
        Console.WriteLine($"Tryck 'H' för att använda hälsopaketet.");

        ConsoleKeyInfo keyInfo = Console.ReadKey();
        if (keyInfo.KeyChar == 'H' || keyInfo.KeyChar == 'h')
        {
            hälsopaket.Använd(spelaren);
        }
    }
}

public class Entitet
{
    public int X { get; set; }
    public int Y { get; set; }
    public char Symbol { get; set; }

    public Entitet(int x, int y, char symbol)
    {
        X = x;
        Y = y;
        Symbol = symbol;
    }
}

public class Spelare : Entitet
{
    public int Livskraft { get; set; }

    public Spelare(int x, int y, char symbol) : base(x, y, symbol)
    {
        Livskraft = 100;
    }
}

public class Varelse : Entitet
{
    public int Livskraft { get; set; }
    public List<Förmåga> Förmågor { get; set; }
    public ConsoleColor Färg { get; set; }

    public Varelse(int x, int y, char symbol, ConsoleColor färg) : base(x, y, symbol)
    {
        Livskraft = 100;
        Förmågor = new List<Förmåga>();
        Färg = färg;
    }

    public void LäggTillFörmåga(Förmåga förmåga)
    {
        Förmågor.Add(förmåga);
    }
}

public class Förmåga
{
    public string Namn { get; set; }
    public Action<Spelare> Använd { get; set; }

    public Förmåga(string namn, Action<Spelare> använd)
    {
        Namn = namn;
        Använd = använd;
    }
}

public class Hälsopaket : Entitet
{
    public Hälsopaket(int x, int y, char symbol, ConsoleColor färg) : base(x, y, symbol)
    {
        Färg = färg;
    }

    public ConsoleColor Färg { get; set; }

    public void Använd(Spelare spelare)
    {
        spelare.Livskraft += 25;
        if (spelare.Livskraft > 100) spelare.Livskraft = 100;
    }
}