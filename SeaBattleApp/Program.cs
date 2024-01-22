namespace SeaBattleApp;

class Program
{

    static void Main(string[] args)
    {
        
        Play();

    }

    static void Play()
    {
        var game = new Game();
        Console.WriteLine(game.Greeting);
        Console.WriteLine("Вам даны 10 кораблей:");
        int shV = BattleField.MarkIsAShip;
        Console.WriteLine($"4-ре 1-палубных:   {shV}\t|  {shV}\t|  {shV}\t|  {shV}");
        Console.WriteLine($"3-ри 2-палубных:   {shV}{shV}\t|  {shV}{shV}\t|  {shV}{shV}");
        Console.WriteLine($"2-ва 3-палубных:   {shV}{shV}{shV}\t|  {shV}{shV}{shV}");
        Console.WriteLine($"1-ин 4-палубный:   {shV}{shV}{shV}{shV}");
        game.createShips();
        Console.WriteLine("\nИ 2 поля:\n1-ое для расположения ваших кораблей.\n");
        ShowBattleField(game.MyField);
        Console.WriteLine("\n2-ое для поиска и уничтожения вражеской флотилии.\n");
        ShowBattleField(game.OpponentField);
        System.Console.WriteLine("\nТеперь вы готовы для размещения вашей флотилии.\n");


        /* Добавить событие на добавления корабля в поле (отрисовка моего поля с кораблями) */
        game.AddingAShipEvent += HandleAddingAShip;


        int i = 10;
        while (i > 0) {
            System.Console.WriteLine($"Выберите корабль нужной палубности и разместите его на поле указав координату начала и ориентацию.\nВсего осталось {i} кораблей.");
            System.Console.WriteLine("Скольки палубный вы хотите добавить на поле (до 1 до 4)?: ");
            int len = int.Parse(Console.ReadLine());
            Ship? targetShip = null;
            try {
                targetShip = game.ChooseTheShip(len);
            }
            catch (Exception ex) {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine("Кораблей с такой палубностью не найдено! Попробуйте ещё раз!\n");
                continue;
            }
            i--;
            
            System.Console.WriteLine("Корабль найден и готов к установке на поле."); 
            System.Console.Write("Вводите координату (сначала номер строки, потом букву столбца, без пробелов): ");
            string coords = Console.ReadLine().ToUpper();
            if (!game.IsValidRuCoordinate(coords)) {
                System.Console.WriteLine("Coordinates Not VALID!!!!!!!!!!!!!!!!!!!!!!!!");
                return;
            }
            System.Console.WriteLine("Введите ориентацию корабля (в - вертикальная, г - горизонтальная): ");
            string orientation = Console.ReadLine().ToLower();
            if (orientation != "в" && orientation != "г") {
                System.Console.WriteLine("Coordinates Not VALID!!!!!!!!!!!!!!!!!!!!!!!!");
                return;
            }
            targetShip.IsHorizontalOrientation = orientation == "г" ? true : false;
            (bool, string) result = game.TryAddTheShipOnTheField(targetShip, Coordinate.ParseRu(coords));
            if (!result.Item1) {
                System.Console.WriteLine(result.Item2);
                return;
            }

        }
    }

    static void HandleAddingAShip(object sender, Ship ship) {
        var game = (Game)sender;
        if (game.MyField.MyShips.Count == 0) {
            ShowBattleField(game.MyField);
            return;
        }
        System.Console.WriteLine($"Установлен один {ship.Length}-палубный корабль.\nВаше поле:");
        ShowBattleField(game.MyField);

    }

    static void ShowBattleField(BattleField battleField)
        {
            int[,] field = battleField.Field;
            var charCol = Game.FIRST_CHAR_RU;
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

        static void Test() {
            char ch = 'А';
            for (int i = 0; i < 33; i++)
            {
                System.Console.WriteLine(ch);
                ch = (char)(ch + 1);
            }
            System.Console.WriteLine('И' > 'И');
        }
}
