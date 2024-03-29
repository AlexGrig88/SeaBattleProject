﻿namespace SeaBattleApp;

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
        var shipsOutside = game.createShips();
        Console.WriteLine("\nИ 2 поля:\n1-ое для расположения ваших кораблей.\n");
        ShowBattleField(game.MyField);
        Console.WriteLine("\n2-ое для поиска и уничтожения вражеской флотилии.\n");
        ShowBattleField(game.OpponentField);
        System.Console.WriteLine("\nТеперь вы готовы для размещения вашей флотилии.\n");


        // Добавить событие на добавления корабля в поле(отрисовка моего поля с кораблями)
        game.AddShipEvent += HandleAddingAShip;



        while (shipsOutside.Count > 0)
        {
            break;
            Console.WriteLine($"Выберите корабль нужной палубности и разместите его на поле указав координату начала и ориентацию.\nВсего осталось {shipsOutside.Count} кораблей.");
            Console.WriteLine("Скольки палубный вы хотите добавить на поле (до 1 до 4)?: ");
            int len;
            while (!(int.TryParse(Console.ReadLine(), out len) && len > 0 && len < 5))
            {
                Console.WriteLine("Стольки палубных кораблей не существует.\nВведите целое число в диапазоне [1, 4]: ");
            }

            Ship? targetShip = null;
            try
            {
                // targetShip = game.ChooseTheShip(len);
                targetShip = game.ChooseTheShip(shipsOutside, len);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine("Кораблей с такой палубностью не найдено! Попробуйте ещё раз!\n");
                Console.ForegroundColor = ConsoleColor.Gray;
                continue;
            }

            Console.WriteLine("Корабль найден и готов к установке на поле.");


        // label for goto
        TryCoordinates:      
            Console.Write("Вводите координату (сначала номер строки, потом букву столбца, без пробелов): ");
            string coords = ReadValidCoords(game);

            System.Console.WriteLine("Введите ориентацию корабля (в - вертикальная, г - горизонтальная): ");
            string orientation = Console.ReadLine()!.ToLower();
            while (orientation != "в" && orientation != "г")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Такая ориентация не поддерживается!\nTry again!\n");
                Console.ForegroundColor = ConsoleColor.Gray;
                orientation = Console.ReadLine()!.ToLower();
            }
            targetShip.IsHorizontalOrientation = orientation == "г" ? true : false;
            (bool, string) result = game.TryAddTheShipOnTheField(targetShip, Coordinate.ParseRu(coords));
            if (!result.Item1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Item2 + " Try again!\n");
                Console.ForegroundColor = ConsoleColor.Gray;
                goto TryCoordinates;
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Done!!!!!!!!!!!!!!!!!!!!!!!!!!!\n");
        Console.ForegroundColor = ConsoleColor.Gray;

        Console.WriteLine("Тперерь можете стрелять по вражеским кораблям!\nНаведите вашу пушку и пли (введите координату)!!!!\n");
        string targetCoords = ReadValidCoords(game);
        System.Console.WriteLine($"targetCoord = {targetCoords}");
        bool shipIsDestroyed = false;
        while (game.TryShootAtTheTarget(Coordinate.ParseRu(targetCoords), true, ref shipIsDestroyed)) {
            if (!shipIsDestroyed) {
                System.Console.WriteLine("Вы молодец, вы подбили в корабль! Стреляйте ещё раз (введите координату)!!!");
                targetCoords = ReadValidCoords(game);
            }
            else {
                System.Console.WriteLine("УРА!!!!!!!!!!!!\nКорабль уничтожен!!!\nСтреляйте ещё раз (введите координату)!!!");
                if (game.CurrentField.ShipsCounter == 0) {
                    System.Console.WriteLine("О НЕТ!!! ВЫ ЖЕ ПОБЕДИЛИ!!!! КРАСАВЧИК!!!");
                    return;
                }
                targetCoords = ReadValidCoords(game);
            }
        }
        

    }

    static string ReadValidCoords(Game game) {
        string coords = Console.ReadLine()?.ToUpper() ?? "123DDD";
        while (!game.IsValidRuCoordinate(coords))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Координата не корректная!\nПопробуйте ещё раз!\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            coords = Console.ReadLine()?.ToUpper() ?? "123DDD";
        }
        return coords;
    }

    static void HandleAddingAShip(object sender, Ship? ship) {
        var game = (Game)sender;
        // if (game.MyField.ShipsList.Count == 0) {
        //     ShowBattleField(game.MyField);
        //     return;
        // }
        if (ship != null)
            Console.WriteLine($"Установлен один {ship.Length}-палубный корабль.\nВаше поле:");
        ShowBattleField(game.CurrentField);

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
                //char shipImg = field[i, j] == BattleField.MarkIsAShip ? 'S' : '.';
                char shipImg = '.';
                if (field[i, j] == BattleField.MarkIsAShip) {
                    shipImg = 'S';
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (field[i, j] == (int)BattleField.CellState.Unexplored) {
                    shipImg = '*';
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else if (field[i, j] == (int)BattleField.CellState.Empty) {
                    shipImg = '-';
                    Console.ForegroundColor = ConsoleColor.White;
                } 
                else if (field[i, j] == (int)BattleField.CellState.BurningShip) {
                    shipImg = 'B';
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (field[i, j] == (int)BattleField.CellState.DestroyedShip) {
                    shipImg = 'X';
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                Console.Write(shipImg + " ");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.WriteLine();
        }
        System.Console.WriteLine();
    }

}
