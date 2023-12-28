namespace SeaBattleApp;

class Program
{
    private const char FIRST_CHAR_RU = 'А';

    static void Main(string[] args)
    {
        Console.WriteLine();
        ShowBattleField(new BattleField(true).Field);
    }

    static void ShowBattleField(int[,] field)
    {
        var charCol = FIRST_CHAR_RU;
        Console.Write("    ");
        for (int i = 0; i < field.GetLength(1); i++)
        {
            var ch = charCol == 'Й' || charCol == 'Ё' ? ++charCol : charCol;
            Console.Write($"{charCol++} ");
        }
        Console.WriteLine("\n   " + new string('-', field.GetLength(1) * 2));

        for (int i = 0; i < field.GetLength(0); ++i) {
            var indent = (i + 1) < 10 ? " " : ""; 
            Console.Write($"{i + 1}{indent}| ");
            for (int j = 0; j < field.GetLength(1); ++j)
            {
                Console.Write(field[i, j] + " ");
            }
            Console.WriteLine();
        }
        System.Console.WriteLine();
    }
}
