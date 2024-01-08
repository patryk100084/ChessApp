using System;
using System.Collections.Generic;
using ChessApp.Utilities;

namespace ChessApp.Game.Pieces
{
    public class Pawn : Piece
    {
        int promotionRow;
        bool hasMoved;
        bool canBeEnPassant;
        bool toPromote;
        public Pawn(Tuple<int, int> _pos, Colour _colour) : base(_pos, _colour)
        {
            pos = _pos;
            colour = _colour;
            hasMoved = false;
            canBeEnPassant = false;
            toPromote = false;
            if (_colour == Colour.White)
                promotionRow = 8;
            else
                promotionRow = 1;
        }

        public int GetPromotionRow()
        {
            return promotionRow;
        }

        public bool GetBeEnPassant()
        {
            return canBeEnPassant;
        }

        public bool GetToPromote()
        {
            return toPromote;
        }

        public void SetBeEnPassant(bool _canEnPassant)
        {
            canBeEnPassant = _canEnPassant;
        }

        public List<Tuple<int, int>> GetCaptureFields()
        {
            List<Tuple<int, int>> fields = new List<Tuple<int, int>>();
            if (colour == Colour.White)
            {
                fields.Add(Tuple.Create(pos.Item1 + 1, pos.Item2 + 1));
                fields.Add(Tuple.Create(pos.Item1 - 1, pos.Item2 + 1));
            }
            else
            {
                fields.Add(Tuple.Create(pos.Item1 + 1, pos.Item2 - 1));
                fields.Add(Tuple.Create(pos.Item1 - 1, pos.Item2 - 1));
            }
            return fields;
        }

        public override List<Tuple<int, int>> GetPotentialMoves(List<Piece> otherPieces)
        {
            List<Tuple<int, int>> fields = new List<Tuple<int, int>>();

            if (colour == Colour.White)
            {
                if (!hasMoved)
                    fields.AddRange(CheckMovementInDirection(0, 1, otherPieces, 2, false));
                else
                    fields.AddRange(CheckMovementInDirection(0, 1, otherPieces, 1, false));
                foreach (var field in GetCaptureFields())
                    if (CheckForCapture(field, otherPieces))
                        fields.Add(field);
            }
            else if (colour == Colour.Black)
            {
                if (!hasMoved)
                    fields.AddRange(CheckMovementInDirection(0, -1, otherPieces, 2, false));
                else
                    fields.AddRange(CheckMovementInDirection(0, -1, otherPieces, 1, false));
                foreach (var field in GetCaptureFields())
                    if (CheckForCapture(field, otherPieces))
                        fields.Add(field);
            }
            return fields;
        }

        public override void MovePiece(Tuple<int, int> newPos)
        {
            SetPos(newPos);
            if (!hasMoved)
                hasMoved = true;
            if (canBeEnPassant)
                canBeEnPassant = false;
            if (newPos.Item2 == promotionRow)
                toPromote = true;

        }
    }
}
