using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using WhackAMole.UWPClient.Controls;
using WhackAMole.UWPClient.Services;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace WhackAMole.UWPClient.ViewModel
{
    public class MoleViewModel : INotifyPropertyChanged
    {
        private static MoleService _moleService;
        private static AdminService _adminService;
        private static readonly Color ERROR_COLOR = Colors.Red;
        private bool _isDeleted;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        public string MoleName
        {
            get {
                return _name; }
            set
            {
                if (_name == value)
                    return;
                _name = value;
                RaisePropertyChanged(nameof(MoleName));
            }
        }

        private string _phase;
        public string Phase
        {
            get { return _phase; }
            set
            {
                if (_phase == value)
                    return;
                _phase = value;
                RaisePropertyChanged(nameof(Phase));
            }
        }


        private Point _currentPosition = new Point();
        public Point CurrentPosition
        {
            get { return _currentPosition; }
            set
            {
                if (_currentPosition == value)
                    return;
                _currentPosition = value;
                
                RaisePropertyChanged(nameof(CurrentPosition));
            }
        }


        private Point _vector;

        public Point Vector
        {
            get { return _vector; }
            set {
                if (_vector == value)
                    return;
                _vector = value;
                UpdateAlignment();

            }
        }

        private MoleAlignment _alignment;
        public MoleAlignment DisplayAlignment
        {
            get { return _alignment; }
            set
            {
                if (_alignment == value)
                    return;
                _alignment = value;
                RaisePropertyChanged(nameof(DisplayAlignment));
            }
        }

        private Visibility _isVisible;
        public Visibility IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                RaisePropertyChanged(nameof(IsVisible));
            }
        }


        private void UpdateAlignment()
        {
            if (Vector.X <= 0)
                DisplayAlignment = (Vector.Y <= 0) ? MoleAlignment.TopLeft : MoleAlignment.BottomLeft;
            else
                DisplayAlignment = (Vector.Y <= 0) ? MoleAlignment.TopRight: MoleAlignment.BottomRight;


        }


        private SolidColorBrush _color = new SolidColorBrush(Colors.Transparent);
        public SolidColorBrush Color
        {
            get { return _color; }
            set
            {
                if (_color == value)
                    return;
                _color = value;
                RaisePropertyChanged(nameof(Color));
            }
        }

        private string _currentChar = "-";
        private static CoreDispatcher _dispatcher;

        public string CurrentChar
        {
            get { return _currentChar; }
            set
            {
                if (_currentChar == value)
                    return;
                _currentChar = value;
                RaisePropertyChanged(nameof(CurrentChar));
            }
        }

        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (_timestamp == value)
                    return;
                _timestamp = value;
                RaisePropertyChanged(nameof(Timestamp));
            }
        }



        // yuck but gets the job done for now
        static MoleViewModel()
        {
            _dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        }

        public MoleViewModel(Point start, Point vector, MoleService moleService, AdminService adminService)
        {

            CurrentPosition = start;
            Vector = vector;
            _moleService = moleService;
            _adminService = adminService;
        }

        public async Task UpdateStateAsync()
        {
            if (_isDeleted)
                return;
            try
            {


                var mole = await _moleService.GetStateUpdateAsync();
                if (mole == null)
                {
                    SetError();
                    return;
                }
                var timestamp = DateTime.Now;
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Color = GetColorFromHex(mole.Color);
                    CurrentChar = mole.CurrentChar;
                    Timestamp = timestamp;
                });
            }
            catch (Exception)
            {

                SetError();
            }

        }

        public async Task<bool> DeleteAsync()
        {
            IsVisible = Visibility.Collapsed;
     
            if (_isDeleted)
                return true;

            _isDeleted = true;
            var killed = await _adminService.DeletePodAsync(MoleName);
            if (killed)
                return true;
            else
            {
                SetError();
                return false;
            }


        }

        private void SetError()
        {
            Debug.WriteLine("Mole ERROR");
            this.Color = new SolidColorBrush(ERROR_COLOR);
        }

        private async void RaisePropertyChanged(string propName)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal,() => { 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            });
        }

       

        private static SolidColorBrush GetColorFromHex(string hexaColor)
        {
            hexaColor = hexaColor.Replace("#", "");
            return new Windows.UI.Xaml.Media.SolidColorBrush(
            Windows.UI.Color.FromArgb(
            255,
            Convert.ToByte(hexaColor.Substring(0, 2), 16),
            Convert.ToByte(hexaColor.Substring(2, 2), 16),
            Convert.ToByte(hexaColor.Substring(4, 2), 16)
            )
            );
        }
    }
}
