using System;
using System.Collections.Generic;
using ChessApp.Utilities;

namespace ChessApp.Game.Pieces
{
    public class King : Piece
    {
        bool hasMoved = false;
        public King(Tuple<int, int> _pos, Colour _colour) : base(_pos, _colour)
        {
            pos = _pos;
            colour = _colour;
        }

        public override List<Tuple<int, int>> GetPotentialMoves(List<Piece> otherPieces)
        {
            List<Tuple<int, int>> fields = new List<Tuple<int, int>>();

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (i - 1 == 0 && j - 1 == 0)
                        continue;
                    fields.AddRange(CheckMovementInDirection(i - 1, j - 1, otherPieces, 1, true));
                }

            return fields;
        }

        public override void MovePiece(Tuple<int, int> newPos)
        {
            SetPos(newPos);
            if (!hasMoved)
                hasMoved = true;
        }
    }
}
