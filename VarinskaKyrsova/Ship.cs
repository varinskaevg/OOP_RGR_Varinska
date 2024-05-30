using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarinskaKyrsova
{
    //Клас для моделювання корабля у грі
    internal class Ship
    {
        public int Size { get; set; }
        public Point StartPosition { get; set; }
        public bool Vertical { get; set; }

        // Конструктор для ініціалізації корабля з заданими координатами початкової точки, розміром та орієнтацією
        public Ship(int startX, int startY, int size, bool vertical)
        {
            Size = size;
            StartPosition = new Point(startX, startY);
            Vertical = vertical;
        }
        // Метод для перевірки, чи є задана координата частиною корабля
        internal bool IsCellPartOfShip(int x, int y)
        {
            throw new NotImplementedException();
        }

    }
    // Клас, що представляє точку на ігровому полі
    internal class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsHit { get; set; }
        // Конструктор для ініціалізації точки з заданими координатами X та Y
        public Point(int x, int y)
        {
            X = x;
            Y = y;
            IsHit = false;
        }
    }
}
