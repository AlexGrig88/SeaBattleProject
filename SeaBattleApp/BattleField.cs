
namespace SeaBattleApp
{

    public class BattleField
    {
        const int ROW_MAX_VALUE = 31;
        const int COLUMN_MAX_VALUE = 31;
        private enum CellState { Unexplored = 0, Empty, BurningShip, DestroyedShip };   // состояние ячейки изменяется путём прибавления нового состояния к начальному 0
        public static int MarkIsAShip { get; } = 5;   //  число, которое служит меткой, что корабля установлен в ячейку и виден
                                                    //  (MarkVisibleShip + BurningShip) ~ 5 + 2 = 7  - число, определяющее, что корабль виден и подбит, аналогично с estroyedShip
        private bool _isItMyField;
        private int _rows;
        private int _columns;

        public List<Ship> ShipsList { get; set; }
        public List<string> AllPositions { get; set; }
        public int[,] Field { get; private set; }
        public int Rows { 
            get => _rows; 
            set {
                if (value > ROW_MAX_VALUE) 
                    throw new Exception($"КОЛЛИЧЕСТВО СТРОК НЕ ДОЛЖНО БЫТЬ БОЛЬШЕ {ROW_MAX_VALUE}");
                _rows = value;
            }
        }
        public int Columns 
        { 
            get => _columns;
            set {
                if (value > COLUMN_MAX_VALUE) 
                    throw new Exception($"КОЛЛИЧЕСТВО СТОЛБЦОВ НЕ ДОЛЖНО БЫТЬ БОЛЬШЕ {COLUMN_MAX_VALUE}");
                _columns = value;
            }
        }

        public Dictionary<int, int> ExpectedMapLengthByCounter { get; }    // счётчик, определяющий колличество кораблей (в зависимости от длины(палубности)), возможных для размещения
        public Dictionary<int, int> CurrentMapLengthByCounter { get; }     // текущий счетчик кораблей на поле

        public BattleField(bool isItMyField, int rows = 10, int columns = 10)
        {
            _isItMyField = isItMyField;
            ExpectedMapLengthByCounter = new Dictionary<int, int>() { {1, 4}, {2, 3}, {3, 2}, {4, 1} };
            CurrentMapLengthByCounter = new Dictionary<int, int>() { {1, 0}, {2, 0}, {3, 0}, {4, 0} };
            Rows = rows;
            Columns = columns;
            Field = new int[Rows, Columns];
            ShipsList = new List<Ship>();
            AllPositions = new List<string>(rows * columns);
            for (int i = 0, ch = 'А'; i < rows; ++i)
            {
                for (int j = 1; j <= columns; j++)
                {
                    if (ch == 'Ё' || ch == 'Й') ++ch;
                    AllPositions.Add($"{j}{(char)ch}");
                }
                Console.WriteLine();
                ch += 1;
            }
        }

        public bool TryToPlaceTheShip(Ship ship, Coordinate beginCoord, out string errorMassage)
        {
            errorMassage = "КОРАБЛЬ РАЗМЕЩЁН";

            var validCoords1 = beginCoord.Row >= 0 || beginCoord.Col >= 0 || beginCoord.Row < _rows || beginCoord.Col < _columns;
            var validCoords2 = ship.IsHorizontalOrientation ?
                               beginCoord.Col + ship.Length <= _columns :
                               beginCoord.Row + ship.Length <= _rows; 
            if (!validCoords1 || !validCoords2) {
                errorMassage = "КООРДИНАТЫ ЛЕЖАТ ЗА ПРЕДЕЛАМИ ПОЛЯ";
                return false;
            }
            
            if (CurrentMapLengthByCounter[ship.Length] >= ExpectedMapLengthByCounter[ship.Length]) {
                errorMassage = $"ЧИСЛО КОРАБЛЕЙ С ДЛИНОЙ {ship.Length} ДОЛЖНО БЫТЬ НЕ БОЛЬШЕ {ExpectedMapLengthByCounter[ship.Length]}";
                return false;
            }

            if (!TryPutInTheField(ship, beginCoord, out string msg)) {
                errorMassage = msg;
                return false;
            }
            CurrentMapLengthByCounter[ship.Length]++;
            ship.BeginCoord = beginCoord;
            ShipsList.Add(ship);
            return true;
        }

        private bool TryPutInTheField(Ship ship, Coordinate begCoord, out string errorMassage)
        {
            errorMassage = "ok";
            for (int i = 0, j = 0; i < ship.Length && j < ship.Length;) {
                if (Field[begCoord.Row + i, begCoord.Col + j] == MarkIsAShip) {
                    errorMassage = "ПРИСУТСТВУЮТ ПЕРЕСЕКАЮЩИЕСЯ ЯЧЕЙКИ";
                    return false;
                }
                if (!ValidAdjacentCells(begCoord.Row + i, begCoord.Col + j, out string errorMsg)) {
                    errorMassage = errorMsg;
                    return false;
                }
                if (ship.IsHorizontalOrientation) ++j;
                else ++i;
            }
            for (int i = 0, j = 0; i < ship.Length && j < ship.Length;) {
                Field[begCoord.Row + i, begCoord.Col + j] = MarkIsAShip;
                if (ship.IsHorizontalOrientation) ++j;
                else ++i;
            }
            return true;
        }

        private bool ValidAdjacentCells(int rowPos, int colPos, out string errorMsg)
        {
            int[][] around = new int[8][] {
                new int[] { 1, 1 }, new int[] {1, 0 }, new int[] {0, 1 }, new int[]{-1, -1 },
                new int[]{-1, 0 }, new int[] {0, -1}, new int[] {1, -1 }, new int[] {-1, 1 }
                };
            errorMsg = "Ok";
            for (int i = 0; i < around.GetLength(0); i++)
            {
                if (rowPos + around[i][0] < 0 || rowPos + around[i][0] >= _rows ||
                    colPos + around[i][1] < 0 || colPos + around[i][1] >= _rows)
                    continue;
                if (Field[rowPos + around[i][0], colPos + around[i][1]] == MarkIsAShip) {
                    errorMsg = "НЕВОЗМОЖНО УСТАНОВИТЬ КОРАБЛЬ КАСАЮЩИЙСЯ СОСЕДНЕГО КОРАБЛЯ";
                    return false;
                }
            }
            return true;
        }
    } 
}