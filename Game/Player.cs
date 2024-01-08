using System;
using System.ComponentModel.DataAnnotations;

namespace ChessApp.Game
{
    public class Player
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public int wins { get; set; }
        [Required]
        public int draws { get; set; }
        [Required]
        public int losses { get; set; }

        public void AddWin()
        {
            wins++;
        }

        public void AddDraw()
        { 
            draws++; 
        }

        public void AddLose() 
        { 
            losses++;
        }
    }
}
