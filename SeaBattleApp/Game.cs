
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SeaBattleApp
{
    public class Game
    {
        public delegate void AddShipHandler(object sender, Ship ship);
        public event AddShipHandler? AddingAShipEvent;

        public enum Mode { SinglePlayer, TwoPlayers }
        public const char FIRST_CHAR_RU = 'А';

        public Mode ModeGame { get; init; }
        public string Greeting { get; } = "Добро пожаловать на игру \"Морской бой!\"";
        public BattleField MyField { get; }
        public BattleField OpponentField { get; }


        public Game(Mode mode = Mode.SinglePlayer)
        {
            ModeGame = mode;
            MyField = new BattleField(true, 10, 10);
            OpponentField = new BattleField(false, 10, 10);
            if (mode == Mode.SinglePlayer)
            {
                PlaceOpponentShips();
            }
        }

        public (bool, string) TryAddTheShipOnTheField(Ship ship, Coordinate coord) {
            if (!MyField.TryToPlaceTheShip(ship, coord, out string errorMsg)) {
                return (false, errorMsg);
            };
            AddingAShipEvent?.Invoke(this, ship);
            return (true, "Success");
        }

        public List<Ship> createShips() {
            var shipsOutside = new List<Ship>();
            foreach (var pair in MyField.ExpectedMapLengthByCounter)
            {
                for (int i = 0; i < pair.Value; i++)
                    shipsOutside.Add(new Ship(pair.Key));
            }
            return shipsOutside;
        }

        public bool IsValidRuCoordinate(string coords) {
            coords = coords.ToUpper();
            if (coords == null || coords.Length < 2 || coords.Length > 3) {
                return false;
            }
            int res = int.MaxValue;
            if (coords.Length == 2 && !int.TryParse(coords.Substring(0, 1), out res)) {
                return false;
            }
            else if (coords.Length == 3 && !int.TryParse(coords.Substring(0, 2), out res)) {
                return false;
            }
            else  {
                char coordAsRuChar = coords[coords.Length - 1];
                if (coordAsRuChar == 'Ё' || coordAsRuChar == 'Й') return false;
                return coordAsRuChar >= FIRST_CHAR_RU && 
                        coordAsRuChar <= FIRST_CHAR_RU + MyField.Field.GetLength(1) &&
                        res > 0 && res <= MyField.Field.GetLength(0);
            }
        }

        public void PlaceOpponentShips()
        {
            var size = OpponentField.Rows * OpponentField.Columns;
            var allPositions = OpponentField.AllPositions;

            var shipsOutside = createShips();
            var arrOfLen = new int[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
            var arrOfBool = new bool[] { true, false };
            var random = new Random();
            foreach (int len in arrOfLen)
            {
                var ship = ChooseTheShip(shipsOutside, len);
                ship.IsHorizontalOrientation = arrOfBool[random.Next(0, 2)];
                var randIdx = random.Next(0, size);
                Console.WriteLine($"\nRandom postion: {allPositions[randIdx]}\n");
                while (!OpponentField.TryToPlaceTheShip(ship, Coordinate.ParseRu(allPositions[randIdx]), out string errorMsg))
                {
                    randIdx = random.Next(0, size);
                    ship.IsHorizontalOrientation = arrOfBool[random.Next(0, 2)];
                    // Console.WriteLine($"\nRandom postion: {allPositions[randIdx]}\nOpponent message: {errorMsg} \n");
                    // Console.ReadKey();
                }
                allPositions = RemoveUnvailablePositions(allPositions, allPositions[randIdx], len, ship.IsHorizontalOrientation);
                size = allPositions.Count;
            }
        }

        private List<string> RemoveUnvailablePositions(List<string> allPositions, string pos, int len, bool isHorizontal)
        {
            var positionsForDelete = new HashSet<string>();
            positionsForDelete.Add(pos);
            addAllAroundPositions(positionsForDelete, pos);
            if (isHorizontal)
            {
                for (int i = 1; i < len; i++)
                {
                    var nextHPos = NextHorizontPos(pos, 1);
                    positionsForDelete.Add(nextHPos);
                    addAllAroundPositions(positionsForDelete, nextHPos);
                }
            }
            else
            {
                for (int i = 1; i < len; i++)
                {
                    var nextVPos = NextVerticalPos(pos, 1);
                    positionsForDelete.Add(nextVPos);
                    addAllAroundPositions(positionsForDelete, nextVPos);
                }
            }

            return allPositions.Where(p => positionsForDelete.All(pos => pos != p)).ToList();
        }

        private void addAllAroundPositions(HashSet<string> positionsForDelete, string pos)
        {

            var nextHPos = NextHorizontPos(pos, 1);
            positionsForDelete.Add(nextHPos);
            positionsForDelete.Add(NextVerticalPos(nextHPos, 1));
            positionsForDelete.Add(NextVerticalPos(nextHPos, -1));
            var prevHpos = NextHorizontPos(pos, -1);
            positionsForDelete.Add(prevHpos);
            positionsForDelete.Add(NextVerticalPos(prevHpos, 1));
            positionsForDelete.Add(NextVerticalPos(prevHpos, -1));

            positionsForDelete.Add(NextVerticalPos(pos, 1));
            positionsForDelete.Add(NextVerticalPos(pos, -1));
            
        }

        private string NextHorizontPos(string pos, int delta)
        {
            if (delta == 0) return pos;
            char posChar = (char)(pos[pos.Length - 1] + delta);
            if (posChar == 'Ё' || posChar == 'Й') posChar = (char)(posChar + delta);
            return pos.Substring(0, pos.Length - 1) + posChar;
        }

        private string NextVerticalPos(string pos, int delta)
        {
            if (delta == 0) return pos;
            int posNum = Convert.ToInt32(pos.Substring(0, pos.Length - 1));
            return (posNum + delta).ToString() + pos[pos.Length - 1];
        }

        public Ship ChooseTheShip(List<Ship> ships, int length)
        {
            Ship? ship = ships.Find(sh => sh.Length == length);
            if (ship == null) throw new Exception("The ship not found!");
            ships.Remove(ship);
            return ship;
        }


    }
}