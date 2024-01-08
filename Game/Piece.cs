using System;
using System.Collections.Generic;
using ChessApp.Utilities;

namespace ChessApp.Game
{
    public class Piece
    {
        protected Tuple<int, int> pos;
        protected Colour colour;

        public Piece(Tuple<int, int> _pos, Colour _colour)
        {
            pos = _pos;
            colour = _colour;
        }

        public Tuple<int, int> GetPos()
        {
            return pos;
        }

        public void SetPos(Tuple<int, int> _pos)
        {
            pos = _pos;
        }

        public Colour GetColour()
        {
            return colour;
        }
        public virtual List<Tuple<int, int>> GetPotentialMoves(List<Piece> otherPieces)
        {
            return null;
        }

        public List<Tuple<int, int>> CheckMovementInDirection(int xDirection, int yDirection, List<Piece> otherPieces, int limit, bool checkCapture)
        {
            List<Tuple<int, int>> fields = new List<Tuple<int, int>>();
            ;
            for (int i = 1; i <= limit; i++)
            {
                Tuple<int, int> field = Tuple.Create(pos.Item1 + i * xDirection, pos.Item2 + i * yDirection);
                if (field.Item1 > 8 || field.Item2 > 8 || field.Item1 < 1 || field.Item2 < 1)
                    break;
                if (CheckForMovement(field, otherPieces))
                    fields.Add(field);
                else
                {
                    if (checkCapture && CheckForCapture(field, otherPieces))
                        fields.Add(field);
                    break;
                }
            }
            return fields;
        }

        public bool CheckForMovement(Tuple<int, int> pos, List<Piece> otherPieces)
        {
            var piece = otherPieces.Find(piece => piece.pos.Equals(pos));
            if (piece != null)
                return false;
            return true;
        }

        public bool CheckForCapture(Tuple<int, int> pos, List<Piece> otherPieces)
        {
            var piece = otherPieces.Find(piece => piece.pos.Equals(pos));
            if (piece != null && piece.colour != colour)
                return true;
            return false;
        }

        public virtual void MovePiece(Tuple<int, int> newPos)
        {
            SetPos(newPos);
        }
    }
}
