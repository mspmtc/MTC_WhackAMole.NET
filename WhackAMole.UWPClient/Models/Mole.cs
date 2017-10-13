using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace WhackAMole.UWPClient.Models
{
    class Mole : INotifyPropertyChanged
    {
        public string Id { get; set; }
        private char _currentChar;
        public char CurrentChar
        {
            get { return _currentChar; }
            private set
            {
                if (_currentChar == value)
                    return;
                _currentChar = value;
                RaisePropertyChanged(nameof(CurrentChar));
            }
        }

        

        public Point Position { get; set; }
        public SolidColorBrush Fill { get; private set; } = new SolidColorBrush( Colors.Cyan);
        public int Speed { get; set; } = 100;
        public double Direction { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Mole(string Id, char currentChar)
        {

        }

        private  void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
