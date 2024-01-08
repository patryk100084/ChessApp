using System;
using System.Collections.Generic;
using ChessApp.Utilities;

namespace ChessApp.Game.Pieces
{
    public class Rook : Piece
    {
        bool hasMoved = false;
        public Rook(Tuple<int, int> _pos, Colour _colour, bool _hasMoved = false) : base(_pos, _colour)
        {
            pos = _pos;
            colour = _colour;
            hasMoved = _hasMoved;
        }

        public bool HasMoved()
        {
            return hasMoved;
        }

        public override List<Tuple<int, int>> GetPotentialMoves(List<Piece> otherPieces)
        {
            List<Tuple<int, int>> fields = new List<Tuple<int, int>>();

            for (int i = 0; i < 2; i++)
            {
                fields.AddRange(CheckMovementInDirection(0, i * 2 - 1, otherPieces, 8, true));
                fields.AddRange(CheckMovementInDirection(i * 2 - 1, 0, otherPieces, 8, true));
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
