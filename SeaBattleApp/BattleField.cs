namespace SeaBattleApp
{
    struct Coordinate
    {
        int row;
        int col;
        public Coordinate(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }

    class BattleField
    {
        const int ROW_MAX_VALUE = 31;
        const int COLUMN_MAX_VALUE = 31;
        private enum CellState { Unexplored = 0, Empty, BurningShip, DestroyedShip };
        private bool _isItMyField;
        private int _rows;
        private int _columns;

        private List<Coordinate> _coordsWithShips;
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

        public BattleField(bool isItMyField, int rows = 10, int columns = 10)
        {
            Rows = rows;
            Columns = columns;
            _isItMyField = isItMyField;
            if (isItMyField) _coordsWithShips = new List<Coordinate>();

            Field = new int[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Field[i, j] = (int)CellState.Unexplored;
                }
            }
        }

        public bool TryToPlaceTheShip(Ship ship, Coordinate beginCoord)
        {
            return false;
        }

    }

    class Ship
    {
        private int _length;  // колличество палуб
        private Coordinate _beginCoord;

        public bool IsHorizontalOrientation { get; set; }
        public bool IsDestroyed { get; set; }
        public int Length
        {
            get => _length;
            set => _length = value;
        }
        public Coordinate BeginCoord
        {
            get => _beginCoord;
            set => _beginCoord = value;
        }

        public Ship(int length, bool isHorizontalOrientation = true)
        {
            Length = length;
            IsHorizontalOrientation = isHorizontalOrientation;
            IsDestroyed = false;
        }
    }
}