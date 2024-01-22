using System.Diagnostics.CodeAnalysis;

namespace SeaBattleApp
{
    public struct Coordinate
    {
        public int Row { get; }
        public int Col { get; }
        public Coordinate(int row, int col)
        {
            Row = row; 
            Col = col;
        }
        public static Coordinate ParseRu(string coords) {
            coords = coords.ToUpper();
            int res = int.MaxValue;
            if (coords.Length == 2 && !int.TryParse(coords.Substring(0, 1), out res)) {
                throw new Exception("Error Parsing coordinate!");
            }
            else if (coords.Length == 3 && !int.TryParse(coords.Substring(0, 2), out res)) {
                throw new Exception("Error Parsing coordinate!");
            }
            else  {
                if (coords[^1] > 'И') return new Coordinate(res - 1, coords[^1] - 'А' - 1);
                else return new Coordinate(res - 1, coords[^1] - 'А');
            }
        } 
    }

    public class Ship
    {
        const int MAX_LENGTH = 4;
        private int _length;  // колличество палуб
        private Coordinate _beginCoord;

        public bool IsHorizontalOrientation { get; set; }
        public bool IsDestroyed { get; set; }
        public int CounterLostParts {get; set; }
        public int Length
        {
            get => _length;
            set {
                if (value < 1) throw new Exception("ДЛИНА КОРАБЛЯ ДОЛЖНА БЫТЬ БОЛЬШЕ 0");
                if (value > MAX_LENGTH) throw new Exception($"ДЛИНА КОРАБЛЯ НЕ ДОЛЖНА БЫТЬ БОЛЬШЕ {MAX_LENGTH}");
                _length = value;
            }
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
            CounterLostParts = length;
        }

        public override bool Equals(object? obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || ! this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else {
                Ship other = (Ship) obj;
                return (Length == other.Length) && (BeginCoord.Row == other.BeginCoord.Row) && (BeginCoord.Col == other.BeginCoord.Col);
            }
        }

        public override int GetHashCode() => HashCode.Combine(this.Length, this.BeginCoord.Col, this.BeginCoord.Row);
    }
}