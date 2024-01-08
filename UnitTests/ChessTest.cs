using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChessApp.Game;
using ChessApp.Utilities;
using System.Collections.Generic;
using ChessApp.Game.Pieces;
using System;
using System.Linq;

namespace ChessApp.UnitTests
{
    [TestClass]
    public class ChessTest
    {
        [TestMethod]
        public void TestCheckmate()
        {
            // Fool's Mate - Mate by black on 2nd turn
            string[] coords = { "F2", "F3", "E7", "E6", "G2", "G4", "D8", "H4" };
            Piece piece;
            ChessGame chess = new ChessGame();
            for(int i = 0;i<coords.Length/2; i++) 
            {
                piece=chess.GetPlayerPiece(PositionConverter.StringToPosition(coords[i*2]));
                chess.MovePiece(piece, PositionConverter.StringToPosition(coords[i*2+1]));
            }
            Assert.AreEqual(GameState.Checkmate, chess.GetGameState(), "It should be checkmate");
        }

        [TestMethod]
        public void TestCheck() 
        {
            // Johan Upmark vs Robin Johansson (1995) - Check by white on turn 6: Qxd7+
            string[] coords = { "E2", "E3", "A7", "A5", "D1", "H5", "A8", "A6", "H5", "A5", "H7", "H5", "H2", "H4", "A6", "H6", "A5", "C7", "F7", "F6", "C7", "D7" };
            Piece piece;
            ChessGame chess = new ChessGame();
            for (int i = 0; i < coords.Length / 2; i++)
            {
                piece = chess.GetPlayerPiece(PositionConverter.StringToPosition(coords[i * 2]));
                chess.MovePiece(piece, PositionConverter.StringToPosition(coords[i * 2 + 1]));
            }
            Assert.AreEqual(GameState.Check, chess.GetGameState(), "It should be check");
        }

        [TestMethod]
        public void TestStalemate() 
        {
            // Johan Upmark vs Robin Johansson (1995) - Stalemate on white's 10th turn: Qe6 1/2 - 1/2
            string[] coords = { "E2","E3","A7","A5","D1","H5","A8","A6","H5","A5","H7","H5","H2","H4","A6","H6","A5","C7","F7","F6","C7","D7","E8","F7","D7","B7","D8","D3","B7","B8","D3","H7","B8","C8","F7","G6","C8","E6" };
            Piece piece;
            ChessGame chess = new ChessGame();
            for (int i = 0; i < coords.Length / 2; i++)
            {
                piece = chess.GetPlayerPiece(PositionConverter.StringToPosition(coords[i * 2]));
                chess.MovePiece(piece, PositionConverter.StringToPosition(coords[i * 2 + 1]));
            }
            Assert.AreEqual(GameState.Stalemate, chess.GetGameState(), "It should be stalemate");
        }

        [TestMethod]
        public void TestStalemate2()
        {
            List<Piece> pieces = new List<Piece>
            {
                new King(PositionConverter.StringToPosition("F7"), Colour.White),
                new King(PositionConverter.StringToPosition("H7"), Colour.Black),
                new Bishop(PositionConverter.StringToPosition("G7"), Colour.White),
                new Pawn(PositionConverter.StringToPosition("A3"), Colour.White),
                new Pawn(PositionConverter.StringToPosition("A4"), Colour.Black)
            };

            ChessGame chess = new ChessGame(pieces, Colour.Black);

            Assert.AreEqual(GameState.Stalemate, chess.GetGameState(), "It should be draw, you can't checkmate with just king and bishop");
        }

        [TestMethod]
        public void TestDrawByInsufficientMaterial()
        {
            List<Piece> pieces = new List<Piece>
            {
                new King(PositionConverter.StringToPosition("F7"), Colour.Black),
                new King(PositionConverter.StringToPosition("G4"), Colour.White),
                new Bishop(PositionConverter.StringToPosition("C7"), Colour.Black)
            };

            ChessGame chess = new ChessGame(pieces, Colour.White);

            Assert.AreEqual(GameState.InsufficientMateMaterial, chess.GetGameState(), "It should be draw, you can't checkmate with just king and bishop");
        }

        [TestMethod]
        public void TestShortCastle()
        {
            List<Piece> pieces = new List<Piece>()
            {
                new Rook(PositionConverter.StringToPosition("A1"), Colour.White),
                new Rook(PositionConverter.StringToPosition("H1"), Colour.White),
                new Rook(PositionConverter.StringToPosition("A8"), Colour.Black),
                new Rook(PositionConverter.StringToPosition("H8"), Colour.Black),

                new Knight(PositionConverter.StringToPosition("B1"), Colour.White),
                new Knight(PositionConverter.StringToPosition("B8"), Colour.Black),

                new Bishop(PositionConverter.StringToPosition("C1"), Colour.White),         
                new Bishop(PositionConverter.StringToPosition("C8"), Colour.Black),
                    
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
            

            ChessGame chess = new ChessGame(pieces, Colour.White);

            chess.MovePiece(chess.GetPlayerKing(), PositionConverter.StringToPosition("G1"));

            Tuple<bool, bool> kingAndRookPostCastle = Tuple.Create(chess.GetOpponentPiece(PositionConverter.StringToPosition("G1")) is King, chess.GetOpponentPiece(PositionConverter.StringToPosition("F1")) is Rook);
            Assert.AreEqual(Tuple.Create(true,true), kingAndRookPostCastle, "not found either king or rook on desired positions");
        }

        [TestMethod]
        public void TestLongCastle()
        {
            List<Piece> pieces = new List<Piece>()
            {
                new Rook(PositionConverter.StringToPosition("A1"), Colour.White),
                new Rook(PositionConverter.StringToPosition("H1"), Colour.White),
                new Rook(PositionConverter.StringToPosition("A8"), Colour.Black),
                new Rook(PositionConverter.StringToPosition("H8"), Colour.Black),

                new Knight(PositionConverter.StringToPosition("G1"), Colour.White),
                new Knight(PositionConverter.StringToPosition("G8"), Colour.Black),

                new Bishop(PositionConverter.StringToPosition("F1"), Colour.White),
                new Bishop(PositionConverter.StringToPosition("F8"), Colour.Black),

                new King(PositionConverter.StringToPosition("E1"), Colour.White),
                new King(PositionConverter.StringToPosition("E8"), Colour.Black)
            };
            for (int i = 0; i < 8; i++)
            {
                pieces.Add(new Pawn(Tuple.Create(i + 1, 2), Colour.White));
                pieces.Add(new Pawn(Tuple.Create(i + 1, 7), Colour.Black));
            }
            

            ChessGame chess = new ChessGame(pieces, Colour.White);

            chess.MovePiece(chess.GetPlayerKing(), PositionConverter.StringToPosition("C1"));

            Tuple<bool, bool> kingAndRookPostCastle = Tuple.Create(chess.GetOpponentPiece(PositionConverter.StringToPosition("C1")) is King, chess.GetOpponentPiece(PositionConverter.StringToPosition("D1")) is Rook);
            Assert.AreEqual(Tuple.Create(true, true), kingAndRookPostCastle, "not found either king or rook on desired positions");
        }
        
        [TestMethod]
        public void TestEnPassant()
        {
            List<Piece> pieces = new List<Piece>
            {
                new King(PositionConverter.StringToPosition("E1"), Colour.White),
                new King(PositionConverter.StringToPosition("E8"), Colour.Black)
            };
            for (int i = 0; i < 8; i++)
            {
                pieces.Add(new Pawn(Tuple.Create(i + 1, 2), Colour.White));
                pieces.Add(new Pawn(Tuple.Create(i + 1, 7), Colour.Black));
            }
            ChessGame chess = new ChessGame(pieces, Colour.White);
            Piece piece;
            string[] coords = { "D2", "D4", "A7", "A6", "D4", "D5", "C7", "C5", "D5", "C6" };
            for (int i = 0; i < coords.Length / 2; i++)
            {
                piece = chess.GetPlayerPiece(PositionConverter.StringToPosition(coords[i * 2]));
                chess.MovePiece(piece, PositionConverter.StringToPosition(coords[i * 2 + 1]));
            }

            Tuple<int, int> playersPiecesCount = Tuple.Create(chess.GetPlayerPieces().Count(), chess.GetOpponentPieces().Count());

            Assert.AreEqual(Tuple.Create(8, 9), playersPiecesCount, "En passnt was not performed");


        }

    }
}
