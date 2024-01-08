using ChessApp.Game;
using System.Linq;

namespace ChessApp.Utilities.Database
{
    public class PlayerService
    {
        private readonly PlayerContext context;

        public PlayerService(PlayerContext _context)
        {
            context = _context;
        }

        public void AddPlayer(Player _player)
        {
            context.players.Add(_player);
            context.SaveChanges();
        }

        public Player GetPlayer(string _username, string _password)
        {
            return context.players.Single(player => player.username == _username && player.password == _password);
        }

        public void UpdatePlayer(Player _player)
        {
            context.players.Update(_player);
            context.SaveChanges();
        }
    }
}
