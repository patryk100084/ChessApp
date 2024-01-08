using System;
using System.Collections.Generic;
using ChessApp.Utilities;

namespace ChessApp.Game.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(Tuple<int, int> _pos, Colour _colour) : base(_pos, _colour)
        {
            pos = _pos;
            colour = _colour;
        }

        public override List<Tuple<int, int>> GetPotentialMoves(List<Piece> otherPieces)
        {
            List<Tuple<int, int>> fields = new List<Tuple<int, int>>();

            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    fields.AddRange(CheckMovementInDirection(i * 2 - 1, j * 2 - 1, otherPieces, 8, true));

            return fields;
        }
    }
}
