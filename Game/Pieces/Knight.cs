using System;
using System.Collections.Generic;
using ChessApp.Utilities;

namespace ChessApp.Game.Pieces
{
    public class Knight : Piece
    {
        int[] numbers = { -2, -1, 1, 2 };

        public Knight(Tuple<int, int> _pos, Colour _colour) : base(_pos, _colour)
        {
            pos = _pos;
            colour = _colour;
        }

        public override List<Tuple<int, int>> GetPotentialMoves(List<Piece> otherPieces)
        {
            List<Tuple<int, int>> fields = new List<Tuple<int, int>>();

            fields.AddRange(CheckMovementInDirection(2, 1, otherPieces, 1, true));
            fields.AddRange(CheckMovementInDirection(2, -1, otherPieces, 1, true));

            fields.AddRange(CheckMovementInDirection(-2, 1, otherPieces, 1, true));
            fields.AddRange(CheckMovementInDirection(-2, -1, otherPieces, 1, true));

            fields.AddRange(CheckMovementInDirection(1, 2, otherPieces, 1, true));
            fields.AddRange(CheckMovementInDirection(1, -2, otherPieces, 1, true));

            fields.AddRange(CheckMovementInDirection(-1, 2, otherPieces, 1, true));
            fields.AddRange(CheckMovementInDirection(-1, -2, otherPieces, 1, true));

            return fields;
        }
    }
}
