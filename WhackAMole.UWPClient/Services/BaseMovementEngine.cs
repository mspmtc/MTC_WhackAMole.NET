using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace WhackAMole.UWPClient.Services
{
    class BaseMovementEngine : IMovementEngine
    {
        private const double ALIGNMENT_BUFFER = 50;
        static Random _rnd;

        private double _width;

        public double Width
        {
            get { return _width; }
            set {
                if (_width == value)
                    return;
                _width = value;
                UpdateConstraints();
            }
        }

    

        private double _height;

        public double Height
        {
            get { return _height; }
            set {
                if (_height == value)
                    return;

                _height = value;
                UpdateConstraints();

            }
        }

        private double _moleSize;

        public double MoleSize
        {
            get { return _moleSize; }
            set {
                if (_moleSize == value)
                    return;
                _moleSize = value;
                UpdateConstraints();
            }
        }

        public double MaxX { get; set; }
        public double MaxY { get; set; }

        static BaseMovementEngine()
        {
            _rnd = new Random(DateTime.Now.Millisecond);    // better than nothing 
        }

        public BaseMovementEngine(double width, double height, double moleSize)
        {
            
            MoleSize = moleSize;
            Width = width;
            Height = height;
        }
        private Point GetStartVector(int speed)
        {
            var direction = _rnd.Next(0, 360);
            var vector = new Point(((Math.Cos(direction * Math.PI / 180) * speed)), (-Math.Sin(direction * Math.PI / 180) * speed));
            return vector;
        }

        private  Point StartPosition()
        {
            return new Point(_rnd.NextDouble() * MaxX, _rnd.NextDouble() * MaxY);
        }

        public Tuple<Point,Point> InitializeMole(int speed)
        {
            var start = StartPosition();
            var vector = GetStartVector(speed);
            return new Tuple<Point, Point>(start, vector);
        }

        public Tuple<Point,Point> UpdatePosition(Point start, Point vector)
        {
            if (MaxX <= 0 || MaxY <= 0)
                throw new InvalidOperationException("maxx and/or maxy cannot be zero or negative");

            var newPoint = new Point();
            var newX = start.X + vector.X;
            var newY = start.Y + vector.Y;

            if (newY > MaxY || newY < 0)
            {
                
                var y = (newY < 0) ? 0 : MaxY;
                newPoint = new Point(start.X, y); ;
                vector.Y = vector.Y * -1;
            }
            else
                newPoint.X = newX;


            if (newX > MaxX || newX < 0)
            {
                var x = (newX < 0) ? 0 : MaxX;
                newPoint = new Point(x, start.Y);
                vector.X = vector.X * -1;
            }
            else
                newPoint.Y = newY;
           
            return new Tuple<Point, Point>(newPoint, vector);
        }

        private void UpdateConstraints()
        {
            MaxX = Width - MoleSize;
            MaxY = Height - MoleSize;
        }
    }
}
