using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Solitaire.Models;

namespace Solitaire.Services
{
    public class GameManager
    {
        private readonly Canvas _canvas;
        private readonly GameState _gameState;

        public GameManager(Canvas canvas)
        {
            _canvas = canvas;
            _gameState = new GameState();
        }

        public void StartNewGame()
        {
            // Логика новой игры
        }

        public void SaveGame()
        {
            // Логика сохранения игры
        }

        public void LoadGame()
        {
            // Логика загрузки игры
        }

        public void RestartGame()
        {
            // Логика перезапуска
        }
    }
}
