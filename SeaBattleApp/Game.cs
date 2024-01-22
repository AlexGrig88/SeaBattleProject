
namespace SeaBattleApp
{
    public class Game
    {
        public delegate void AddShipHandler(object sender, Ship ship);
        public event AddShipHandler? AddingAShipEvent;

        public enum Mode { Classic, Custom }
        public const char FIRST_CHAR_RU = 'А';

        public Mode ModeGame { get; init; }
        public string Greeting { get; } = "Добро пожаловать на игру \"Морской бой!\"";
        public BattleField MyField { get; }
        public BattleField OpponentField { get; }
        public List<Ship> ShipsOutside { get; private set; }


        public Game(Mode mode = Mode.Classic)
        {
            ModeGame = mode;
            MyField = new BattleField(true, 10, 10);
            OpponentField = new BattleField(false, 10, 10);
            ShipsOutside = new List<Ship>();
        }

        public void Play() {
            
        }

        public (bool, string) TryAddTheShipOnTheField(Ship ship, Coordinate coord) {
            if (!MyField.TryToPlaceTheShip(ship, coord, out string errorMsg)) {
                return (false, errorMsg);
            };
            AddingAShipEvent?.Invoke(this, ship);
            return (true, "Success");
        }

        public void createShips() {
            switch (ModeGame) {
                case Mode.Classic:
                    foreach (var pair in MyField.ExpectedMapLengthByCounter) {
                        for (int i = 0; i < pair.Value; i++)
                            ShipsOutside.Add(new Ship(pair.Key));
                    }
                    break;
                    
                case Mode.Custom:
                    break;

                default:
                    break;

            }
        }

        public Ship ChooseTheShip(int length)
        {
            Ship? ship = ShipsOutside.Find(sh => sh.Length == length);
            if (ship == null) throw new Exception("The ship not found!");
            ShipsOutside.Remove(ship);
            return ship;
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
    }
}