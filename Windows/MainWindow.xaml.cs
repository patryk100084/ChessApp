using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using ChessApp.Game.Pieces;
using ChessApp.Game;
using ChessApp.Utilities;
using ChessApp.Utilities.Database;
using System.IO;
using System.Threading;

namespace ChessApp.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChessGame game;
        Piece selectedPiece;
        Player whitePlayer, blackPlayer;
        int whiteTime, blackTime;
        List<Button> buttons = new List<Button>();
        List<Tuple<int, int>> fields;
        PlayerService service = new PlayerService(new PlayerContext());
        bool isGameInProgress = false;
        bool pauseTimers = false;
        string imageDirectory = Directory.GetCurrentDirectory() + "\\Images\\";
        Thread timerThread;

        public MainWindow()
        {
            InitializeComponent();
            CreateChessboard();
            DrawChessboard();
            PrintRowsLabels(rows);
            PrintRowsLabels(rows2);
            PrintColumnsLabels(columns);
            PrintColumnsLabels(columns2);
        }
        void WindowClosing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result;
            if(isGameInProgress && game.GetTurnCounter() > 1)
            {
                pauseTimers = true;
                result = MessageBox.Show("Closing window with game in progress is equivalent to resigning. Are you sure?", "Game in progress", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                    HandleResign(game);
                else
                {
                    e.Cancel = true;
                    pauseTimers = false;
                }
            }
        }

        void CreateChessboard()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    Button b = new Button();
                    b.Name = (char)(j + 65) + "" + (8-i);
                    b.Click += ChessboardField;
                    buttons.Add(b);
                    board.Children.Add(b);
                }
        }
        void PrintRowsLabels(UniformGrid _grid)
        {
            for (int i = 0; i < 8; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.Text = (8 - i).ToString();
                _grid.Children.Add(textBlock);
            }
        }
        void PrintColumnsLabels(UniformGrid _grid)
        {
            for (int i = 0; i < 8; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.Text = Char.ToString((char)(i+65));
                _grid.Children.Add(textBlock);
            }
        }
        void DrawChessboard()
        {
            bool darkerColor = true;
            for (int i = 0; i < 8; i++)
            {
                darkerColor = !darkerColor;
                for (int j = 0; j < 8; j++)
                {
                    Button b = buttons.ElementAt(i*8+j);
                    if (darkerColor)
                        b.Background = new SolidColorBrush(Color.FromRgb(163, 112, 67));
                    else
                        b.Background = new SolidColorBrush(Color.FromRgb(248, 224, 176));
                    darkerColor = !darkerColor;
                    b.Content = null;
                }
            }
        }
        void DrawPieces()
        {
            foreach (var piece in game.GetAllPieces())
            {
                Button b = GetButton(piece.GetPos());
                if (b != null)
                {
                    string text = piece.GetType().Name;
                    if (piece.GetColour() == Colour.White)
                        text += "W.png";

                    else
                        text += "B.png";
                    BitmapImage bitmap = new BitmapImage(new Uri(imageDirectory + text));
                    Image img = new Image();
                    img.Source = bitmap;
                    img.Stretch = Stretch.Fill;
                    b.Content = img;
                }
            }
        }
        void SetStatusBarText(ChessGame _game)
        {
            if (_game != null)
            {
                string currentPlayer = _game.GetCurrentPlayer().ToString();
                GameState state = _game.GetGameState();
                string stateName = "";
                if (state != GameState.Normal)
                {
                    stateName = state.ToString() + " - ";
                }
                stateText.Text = "Turn " + _game.GetTurnCounter() + " - " + stateName + currentPlayer + " turn";
            }
        }
        void PrintPlayerInfo(TextBlock _textBlock, Player _player)
        {
            string info = "";
            if (_player != null)
                info = _player.username + "\nW:" + _player.wins + " L:" + _player.losses + " D:" + _player.draws;
            _textBlock.Text = info;
        }
        void PrintPlayersInfos()
        {
            PrintPlayerInfo(blackPlayerInfo, blackPlayer);
            PrintPlayerInfo(whitePlayerInfo, whitePlayer);
        }
        void UpdateGUI()
        {
            DrawChessboard();
            DrawPieces();
            SetStatusBarText(game);
        }
        

        void CreateNewGame()
        {
            NewGameWindow modal = new NewGameWindow();
            modal.Owner = this;
            modal.ShowDialog();
            if(modal.gameTime > 0)
            {
                whiteTime = blackTime = modal.gameTime;
                game = new ChessGame();
                isGameInProgress = true;
                UpdateGUI();
                PrintPlayersInfos();
                UpdateTimers();
                timerThread = new Thread(() => HandleTimers());
                timerThread.Start();
            }
            
        }
        Player CreateGuestAccount()
        {
            Random random = new Random();
            string username = "Guest#" + random.Next(0x8FFFFF);
            return new Player{username = username, password=""};
        }
        void NewGame(object sender, RoutedEventArgs e)
        {
            blackPlayer ??= CreateGuestAccount();
            whitePlayer ??= CreateGuestAccount();
            if(isGameInProgress && game.GetTurnCounter() > 1) 
            {
                pauseTimers = true;
                MessageBoxResult result = MessageBox.Show("Creating new game while one is in progress is equivalent to resigning. Are you sure?", "Game in progress", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                pauseTimers = false;
                if (result == MessageBoxResult.Yes) 
                {
                    HandleResign(game);
                    CreateNewGame();
                }
            }
            else
                CreateNewGame();
        }
        void RegisterUser(object sender, RoutedEventArgs e)
        {
            RegisterWindow modal = new RegisterWindow();
            modal.Owner = this;
            pauseTimers = true;
            modal.ShowDialog();
            pauseTimers = false;
        }
        Player HandleLogin(Player _otherPlayer)
        {
            LoginWindow modal = new LoginWindow();
            modal.Owner = this;
            modal.ShowDialog();
            Player toLogIn = modal.player;
            if (toLogIn != null && _otherPlayer != null && toLogIn.username == _otherPlayer.username)
                MessageBox.Show("You can't login as both white and black player", "Already loggen in", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                return modal.player;
            return null;
        }
        void LoginAsBlack(object sender, RoutedEventArgs e)
        {
            if (!isGameInProgress)
            {
                blackPlayer = HandleLogin(whitePlayer);
                if (blackPlayer != null)
                {
                    MessageBox.Show("Logged in successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    PrintPlayerInfo(blackPlayerInfo, blackPlayer);
                }
            }
            else
            {
                pauseTimers = true;
                MessageBox.Show("You can't login during game, wait for game to end", "Game in progress", MessageBoxButton.OK, MessageBoxImage.Error);
                pauseTimers = false;
            }
        }
        void LoginAsWhite(object sender, RoutedEventArgs e)
        {
            if (!isGameInProgress)
            {
                whitePlayer = HandleLogin(blackPlayer);
                if (whitePlayer != null)
                {
                    MessageBox.Show("Logged in successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    PrintPlayerInfo(whitePlayerInfo, whitePlayer);
                }
            }
            else
            {
                pauseTimers = true;
                MessageBox.Show("You can't login during game, wait for game to end", "Game in progress", MessageBoxButton.OK, MessageBoxImage.Error);
                pauseTimers = false;
            }
        }

        Button GetButton(Tuple<int,int> _pos)
        {
            return buttons.Find(button => button.Name == PositionConverter.PositionToString(_pos));
        }

        void SelectPiece(string _name)
        {
            fields = new List<Tuple<int, int>>();
            selectedPiece = game.GetPlayerPiece(PositionConverter.StringToPosition(_name));
            if (selectedPiece != null)
            {
                fields = game.GetPieceLegalMoves(selectedPiece);
                foreach (Tuple<int, int> field in fields)
                {  
                    Button b = GetButton(field);
                    if (b != null)
                        b.Background = new SolidColorBrush(Color.FromRgb(255, 130, 130));
                }
            }
            if(!fields.Any())
                CancelSelection();          
        }
        void CancelSelection()
        {
            selectedPiece = null;
            fields = null;
        }
        void ChessboardField(object sender, RoutedEventArgs e)
        {
            if(isGameInProgress)
            {
                string name = (sender as Button).Name;
                if (selectedPiece == null)
                    SelectPiece(name);
                else // piece already selected
                {
                    if (PositionConverter.StringToPosition(name).Equals(selectedPiece.GetPos())) // clicked selected piece again - cancel selection
                        CancelSelection();
                    else
                    {
                        Tuple<int, int> newPos = PositionConverter.StringToPosition(name);
                        if (fields.Contains(newPos))
                        {
                            game.MovePiece(selectedPiece, newPos);
                            Pawn toPromote = game.GetPawnToPromote();
                            if(toPromote != null)
                                HandlePromotion(toPromote);
                            CancelSelection();
                            if (game.GetGameState() != GameState.Normal && game.GetGameState() != GameState.Check)
                                HandleEndOfGame(game);
                        }
                        else
                            return;
                    }
                    UpdateGUI();
                }
            }
        }  

        void WinLose(Player _winner, Player _loser, string _message, string _title)
        {
            if(isGameInProgress)
            {
                _winner.AddWin();
                if (!String.IsNullOrEmpty(_winner.password))
                    service.UpdatePlayer(_winner);
                _loser.AddLose();
                if (!String.IsNullOrEmpty(_loser.password))
                    service.UpdatePlayer(_loser);
                isGameInProgress = false;
                MessageBox.Show(_message, _title, MessageBoxButton.OK, MessageBoxImage.Information);
                Dispatcher.Invoke(() => { PrintPlayersInfos(); });
            }
        }
        void Draw(string _message, string _title)
        {
            if(isGameInProgress)
            { 
                blackPlayer.AddDraw();
                if (!String.IsNullOrEmpty(blackPlayer.password))
                    service.UpdatePlayer(blackPlayer);
                whitePlayer.AddDraw();
                if (!String.IsNullOrEmpty(whitePlayer.password))
                    service.UpdatePlayer(whitePlayer);
                isGameInProgress = false;
                MessageBox.Show(_message, _title, MessageBoxButton.OK, MessageBoxImage.Information);
                Dispatcher.Invoke(() => { PrintPlayersInfos(); });
            }
        }
     
        void HandleEndOfGame(ChessGame _game)
        {
            pauseTimers = true;
            GameState state = _game.GetGameState();
            if (state == GameState.Checkmate)
            {
                Colour losingPlayerColour = _game.GetCurrentPlayer();
                if (losingPlayerColour == Colour.White)
                    WinLose(blackPlayer, whitePlayer, "Checkmate - white player lost", "Checkmate");
                else
                    WinLose(whitePlayer, blackPlayer, "Checkmate - black player lost", "Checkmate");
            }
            else if(state == GameState.Stalemate || state == GameState.InsufficientMateMaterial) 
            {
                if(state == GameState.Stalemate)
                    Draw("Draw - stalemate", "Stalemate");
                else
                    Draw("Draw - both players do not have sufficent material to mate another", "Insufficent game material");
            }
        }
        void HandlePromotion(Pawn _pawn)
        {
            PromotionWindow modal = new PromotionWindow();
            modal.Owner = this;
            pauseTimers = true;
            modal.ShowDialog();
            string newPiece = modal.pieceName;
            game.PromotePawn(_pawn, newPiece);
            pauseTimers = false;
        }
        void HandleResign(ChessGame _game)
        {
            if (_game != null)
            {
                Colour currentPlayer = _game.GetCurrentPlayer();
                if (currentPlayer == Colour.White)
                    WinLose(blackPlayer, whitePlayer, "black player has resigned", "Player Resigned");
                else
                    WinLose(whitePlayer, blackPlayer, "white player has resigned", "Player Resigned");
            }
        }
        void OfferDraw(object sender, RoutedEventArgs e)
        {
            if(isGameInProgress && game != null)
            {
                MessageBoxResult result;
                pauseTimers = true;
                result = MessageBox.Show(game.GetCurrentPlayer().ToString() + " player offers a draw do you ( " + game.GetOpponent().ToString() + " player ) accept?", "Draw offer", MessageBoxButton.YesNo, MessageBoxImage.Information);
                pauseTimers = false;
                if (result == MessageBoxResult.Yes)
                    Draw("Draw - players agreed to draw", "Draw accepted");
            }
        }
        void Resign(object sender, RoutedEventArgs e)
        {
            if (isGameInProgress)
            {
                MessageBoxResult result;
                pauseTimers = true;
                result = MessageBox.Show("Are you sure? If you resign you will lose this game", "Resign", MessageBoxButton.YesNo, MessageBoxImage.Information);
                pauseTimers = false;
                if(result == MessageBoxResult.Yes)
                    HandleResign(game);
            }
        }

        void HandleTimers()
        {
            try
            {
                while (isGameInProgress)
                {
                    Thread.Sleep(100);
                    if(!pauseTimers)
                    {
                        if (game.GetCurrentPlayer() == Colour.White)
                            whiteTime--;
                        else if (game.GetCurrentPlayer() == Colour.Black)
                            blackTime--;
                        Dispatcher.Invoke(() => { UpdateTimers(); });
                    }
                    if (whiteTime == 0 || blackTime == 0)
                    {
                        if (whiteTime == 0)
                            WinLose(blackPlayer, whitePlayer, "white player run out of time - black player wins", "Out of time"); 
                        else
                            WinLose(blackPlayer, whitePlayer, "black player run out of time - white player wins", "Out of time");
                        return;
                    }
                }
            }
            catch { }
        }
        void UpdateTimer(TextBlock _timer, int _time)
        {
            string timeFormat = (_time / 600).ToString("00") + ":" + ( (_time/10) % 60).ToString("00") + "." + (_time%10).ToString("0");
            _timer.Text = timeFormat;
        }
        void UpdateTimers()
        {
            UpdateTimer(whiteTimer, whiteTime);
            UpdateTimer(blackTimer, blackTime);
        }
    }
}
