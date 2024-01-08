using System;
using System.Collections.Generic;
using System.Linq;
using ChessApp.Game.Pieces;
using ChessApp.Utilities;

namespace ChessApp.Game
{
    public class ChessGame
    {
        private List<Piece> pieces;
        private Colour currentPlayer;
        private GameState gameState;
        private int turnCounter;

        // Constructors and realted methods
        public ChessGame()
        {
            pieces = SetStartingPosition();
            currentPlayer = Colour.White;     
            gameState = SetGameState();
            turnCounter = 1;
        }
        // used to simulate position without making all moves leading to it for unit testing
        public ChessGame(List<Piece> _pieces, Colour _currentPlayer) 
        {
            pieces = _pieces;
            currentPlayer = _currentPlayer;
            gameState = SetGameState();
            turnCounter = 1;
        }
        List<Piece> SetStartingPosition()
        {
            List<Piece> pieces = new List<Piece>()
            {
                new Rook(PositionConverter.StringToPosition("A1"), Colour.White),
                new Rook(PositionConverter.StringToPosition("H1"), Colour.White),
                new Rook(PositionConverter.StringToPosition("A8"), Colour.Black),
                new Rook(PositionConverter.StringToPosition("H8"), Colour.Black),

                new Knight(PositionConverter.StringToPosition("B1"), Colour.White),
                new Knight(PositionConverter.StringToPosition("G1"), Colour.White),
                new Knight(PositionConverter.StringToPosition("B8"), Colour.Black),
                new Knight(PositionConverter.StringToPosition("G8"), Colour.Black),

                new Bishop(PositionConverter.StringToPosition("C1"), Colour.White),
                new Bishop(PositionConverter.StringToPosition("F1"), Colour.White),
                new Bishop(PositionConverter.StringToPosition("C8"), Colour.Black),
                new Bishop(PositionConverter.StringToPosition("F8"), Colour.Black),

                new Queen(PositionConverter.StringToPosition("D1"), Colour.White),
                new Queen(PositionConverter.StringToPosition("D8"), Colour.Black),

                new King(PositionConverter.StringToPosition("E1"), Colour.White),
                new King(PositionConverter.StringToPosition("E8"), Colour.Black)
            };
            for (int i = 0; i < 8; i++)
            {
                pieces.Add(new Pawn(Tuple.Create(i + 1, 2), Colour.White));
                pieces.Add(new Pawn(Tuple.Create(i + 1, 7), Colour.Black));
            }
            return pieces;
            
        }

        // getters
        public int GetTurnCounter()
        {
            return turnCounter;
        }
        public GameState GetGameState()
        {
            return gameState;
        }
        public Colour GetCurrentPlayer()
        {
            return currentPlayer;
        }
        public Colour GetOpponent()
        {
            if (currentPlayer == Colour.White)
                return Colour.Black;
            else
                return Colour.White;
        }
        
        // setters
        GameState SetGameState()
        {
            if (IsItCheck(GetAllPieces()))
            {
                if (!FindAnyLegalMove())
                    return GameState.Checkmate;
                else
                    return GameState.Check;
            }
            else if (!FindAnyLegalMove())
                return GameState.Stalemate;
            else if (!CheckSufficienrMateMaterial())
                return GameState.InsufficientMateMaterial;
            else
                return GameState.Normal;
        }
        void ChangeActivePlayer()
        {
            if (currentPlayer == Colour.White)
                currentPlayer = Colour.Black;
            else
                currentPlayer = Colour.White;
        }

        // castling related methods
        bool CanShortCastle()
        {
            Piece piece;
            King playerKing = GetPlayerKing();
            for (int i = 0; i < 2; i++)
            {
                piece = GetPiece(Tuple.Create(i + 6, playerKing.GetPos().Item2));
                if (piece != null)
                    return false;
            }
            piece = GetPlayerPiece(Tuple.Create(8, playerKing.GetPos().Item2));
            if (piece != null && piece is Rook && !(piece as Rook).HasMoved())
            {
                for (int i = 0; i < 2; i++)
                    if (!TryMovePiece(playerKing, Tuple.Create(i + 6, playerKing.GetPos().Item2)))
                        return false;
                return true;
            }
            return false;
        }
        bool CanLongCastle()
        {
            Piece piece;
            King playerKing = GetPlayerKing();
            for (int i = 0; i < 3; i++)
            {
                piece = GetPiece(Tuple.Create(i + 2, playerKing.GetPos().Item2));
                if (piece != null)
                    return false;
            }
            piece = GetPlayerPiece(Tuple.Create(1, playerKing.GetPos().Item2));
            if (piece != null && piece is Rook && !(piece as Rook).HasMoved())
            {
                for (int i = 0; i < 2; i++)
                    if (!TryMovePiece(playerKing, Tuple.Create(i + 3, playerKing.GetPos().Item2)))
                        return false;
                return true;
            }
            return false;
        }
        void DoShortCastle()
        {
            King king = GetPlayerKing();
            Piece rook = GetPlayerPiece(Tuple.Create(8, king.GetPos().Item2));

            king.MovePiece(Tuple.Create(king.GetPos().Item1 + 2, king.GetPos().Item2));
            rook.MovePiece(Tuple.Create(king.GetPos().Item1 - 1, king.GetPos().Item2));
        }
        void DoLongCastle()
        {
            King king = GetPlayerKing();
            Piece rook = GetPlayerPiece(Tuple.Create(1, king.GetPos().Item2));

            king.MovePiece(Tuple.Create(king.GetPos().Item1 - 2, king.GetPos().Item2));
            rook.MovePiece(Tuple.Create(king.GetPos().Item1 + 1, king.GetPos().Item2));
        }

        // queries related to pieces
        public Piece GetPiece(Tuple<int, int> _pos)
        {
            return pieces.Find(piece => piece.GetPos().Equals(_pos));
        }
        public Piece GetPlayerPiece(Tuple<int, int> _pos)
        {
            return pieces.Find(piece => piece.GetPos().Equals(_pos) && piece.GetColour() == GetCurrentPlayer());
        }
        public Piece GetOpponentPiece(Tuple<int, int> _pos)
        {
            return pieces.Find(piece => piece.GetPos().Equals(_pos) && piece.GetColour() == GetOpponent());
        }
        public King GetPlayerKing()
        {
            return pieces.OfType<King>().ToList().Find(piece => piece.GetColour() == GetCurrentPlayer());
        }

        public List<Piece> GetPlayerPieces()
        {
            return pieces.Where(piece => piece.GetColour() == GetCurrentPlayer()).ToList();
        }
        public List<Piece> GetOpponentPieces()
        {
            return pieces.Where(piece => piece.GetColour() == GetOpponent()).ToList();
        }
        public List<Piece> GetOtherPieces(Piece _piece)
        {
            return pieces.Where(piece => !piece.GetPos().Equals(_piece.GetPos())).ToList();
        }
        public List<Piece> GetAllPieces()
        {
            return pieces;
        }

        // en passant related methods
        void SetEnPassantFlag(Pawn _pawn, bool _value)
        {
            _pawn.SetBeEnPassant(_value);
        }
        void ResetEnPassntFlags()
        {
            List<Pawn> pawns = pieces.OfType<Pawn>().ToList();
            foreach (Pawn pawn in pawns)
                SetEnPassantFlag(pawn, false);
        }
        void DoEnPassnt(Piece _pawn, Tuple<int, int> _newPos)
        {
            Piece _pawnToCapture;
            _pawn.MovePiece(_newPos);
            if (_pawn.GetColour() == Colour.White)
                _pawnToCapture = GetOpponentPiece(Tuple.Create(_newPos.Item1, _newPos.Item2 - 1));
            else
                _pawnToCapture = GetOpponentPiece(Tuple.Create(_newPos.Item1, _newPos.Item2 + 1));
            CapturePiece(_pawnToCapture);
        }

        // promotion related methods
        public Pawn GetPawnToPromote()
        {
            return pieces.OfType<Pawn>().ToList().Find(pawn => pawn.GetToPromote() == true);
        }
        public void PromotePawn(Piece _pawn, string _newPiece)
        {
            switch (_newPiece)
            {
                case "Bishop":
                    pieces.Add(new Bishop(_pawn.GetPos(), _pawn.GetColour()));
                    break;
                case "Knight":
                    pieces.Add(new Knight(_pawn.GetPos(), _pawn.GetColour()));
                    break;
                case "Rook":
                    pieces.Add(new Rook(_pawn.GetPos(), _pawn.GetColour(), true));
                    break;
                case "Queen":
                    pieces.Add(new Queen(_pawn.GetPos(), _pawn.GetColour()));
                    break;
            }
            pieces.Remove(_pawn);
            gameState = SetGameState();
        }

        // making turn related methods
        public List<Tuple<int, int>> GetPieceLegalMoves(Piece _piece)
        {
            List<Tuple<int, int>> fields = new List<Tuple<int, int>>();
            List<Tuple<int, int>> possibleFields = _piece.GetPotentialMoves(GetOtherPieces(_piece));
            if (_piece is Pawn)
            {
                List<Pawn> opponentPawnsOnSides = GetOpponentPieces().OfType<Pawn>().ToList().FindAll(piece => Math.Abs(piece.GetPos().Item1 - _piece.GetPos().Item1) == 1 && piece.GetPos().Item2 == _piece.GetPos().Item2);
                foreach (Pawn pawn in opponentPawnsOnSides)
                    if (pawn.GetBeEnPassant())
                    {
                        if (pawn.GetColour() == Colour.White)
                            fields.Add(Tuple.Create(pawn.GetPos().Item1, pawn.GetPos().Item2 - 1));
                        else
                            fields.Add(Tuple.Create(pawn.GetPos().Item1, pawn.GetPos().Item2 + 1));
                    }
            }
            if (_piece is King)
            {
                if (CanShortCastle())
                    fields.Add(Tuple.Create(_piece.GetPos().Item1 + 2, _piece.GetPos().Item2));
                if (CanLongCastle())
                    fields.Add(Tuple.Create(_piece.GetPos().Item1 - 2, _piece.GetPos().Item2));
            }
            foreach (Tuple<int, int> field in possibleFields)
            {
                if (TryMovePiece(_piece, field))
                    fields.Add(field);
            }
            return fields;
        }
        bool TryMovePiece(Piece _piece, Tuple<int, int> _newPos)
        {
            Tuple<int, int> orgPos = _piece.GetPos();
            List<Piece> piecesAfterMove = pieces.Where(piece => !piece.GetPos().Equals(_newPos)).ToList();
            _piece.SetPos(_newPos);
            bool kingUnderCheck = IsItCheck(piecesAfterMove);
            _piece.SetPos(orgPos);
            return !kingUnderCheck;
        }
        public void MovePiece(Piece _piece, Tuple<int, int> _newPos)
        {
            List<Tuple<int, int>> legalMoves = GetPieceLegalMoves(_piece);
            if (legalMoves.Contains(_newPos))
            {
                ResetEnPassntFlags();
                Piece opponentPiece = GetOpponentPiece(_newPos);
                if (_piece is Pawn)
                {
                    if (Math.Abs(_piece.GetPos().Item2 - _newPos.Item2) == 2)
                    {
                        _piece.MovePiece(_newPos);
                        SetEnPassantFlag(_piece as Pawn, true);
                    }
                    else if (Math.Abs(_piece.GetPos().Item2 - _newPos.Item2) == 1 && Math.Abs(_piece.GetPos().Item1 - _newPos.Item1) == 1 && opponentPiece == null)
                        DoEnPassnt(_piece as Pawn, _newPos);
                    else
                    {
                        _piece.MovePiece(_newPos);
                    }
                }
                else if (_piece is King)
                {
                    if (_piece.GetPos().Item1 - _newPos.Item1 == -2)
                        DoShortCastle();
                    else if (_piece.GetPos().Item1 - _newPos.Item1 == 2)
                        DoLongCastle();
                    else
                        _piece.MovePiece(_newPos);
                }
                else
                    _piece.MovePiece(_newPos);
                if (opponentPiece != null)
                    CapturePiece(opponentPiece);
                if (currentPlayer == Colour.Black)
                    turnCounter++;
                ChangeActivePlayer();
                gameState = SetGameState();
            }
        }
        void CapturePiece(Piece _piece)
        {
            if (_piece != null)
                pieces.Remove(_piece);
        }

        // check, checkmate and stalemate related methods
        bool IsItCheck(List<Piece> _pieces)
        {
            List<Piece> opponentPieces = _pieces.Where(piece => piece.GetColour() != currentPlayer).ToList();
            King king = GetPlayerKing();
            foreach (Piece piece in opponentPieces)
            {
                List<Tuple<int, int>> attackedFields = piece.GetPotentialMoves(_pieces);
                if (attackedFields.Find(field => field.Equals(king.GetPos())) != null)
                    return true;
            }              
            return false;
        }
        bool FindAnyLegalMove()
        {
            foreach (Piece playerPiece in GetPlayerPieces())
            {
                List<Tuple<int, int>> possibleMoves = GetPieceLegalMoves(playerPiece);
                if (possibleMoves.Any())
                    return true;
            }
            return false;
        }
        bool CheckSufficienrMateMaterial()
        {
            List<Piece> playerPieces = GetPlayerPieces();
            List<Piece> opponentPieces = GetOpponentPieces();
            if (playerPieces.Count == 1 && opponentPieces.Count == 1)
                return false;
            else if (playerPieces.Count == 2 && opponentPieces.Count == 1)
            {
                if (playerPieces.OfType<Knight>().ToList().Count == 1 || playerPieces.OfType<Bishop>().ToList().Count == 1)
                    return false;
            }
            else if (playerPieces.Count == 1 && opponentPieces.Count == 2)
            {
                if (opponentPieces.OfType<Knight>().ToList().Count == 1 || opponentPieces.OfType<Bishop>().ToList().Count == 1)
                    return false;
            }
            return true;
        }
    }
}
