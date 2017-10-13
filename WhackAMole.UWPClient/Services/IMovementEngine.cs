using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace WhackAMole.UWPClient.Services
{
    interface IMovementEngine
    {
        double Width { get; set; }
        double Height { get; set; }
        double MoleSize { get; set; }
        double MaxX { get; set; }
        double MaxY { get; set; }
        Tuple<Point,Point> InitializeMole(int speed);
        Tuple<Point,Point> UpdatePosition(Point start, Point vector);
    }
}
