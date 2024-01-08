using System;

namespace ChessApp.Utilities
{
    public class PositionConverter
    {
        public static Tuple<int, int> StringToPosition(string _name)
        {
            int x = _name[0] - 64;
            int y = _name[1] - '0';

            Tuple<int, int> pos = Tuple.Create(x, y);

            return pos;
        }
        public static string PositionToString(Tuple<int, int> _pos)
        {
            return (char)(_pos.Item1 + 64) + _pos.Item2.ToString();
        }
    }
}
