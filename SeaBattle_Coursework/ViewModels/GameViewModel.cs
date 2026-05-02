using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattle_Coursework.ViewModels
{
    public class GameViewModel
    {
        public BoardViewModel Player1Board { get; set; }
        public BoardViewModel Player2Board { get; set; }

        public GameViewModel()
        {
            Player1Board = new BoardViewModel();
            Player2Board = new BoardViewModel();
        }
    }
}
