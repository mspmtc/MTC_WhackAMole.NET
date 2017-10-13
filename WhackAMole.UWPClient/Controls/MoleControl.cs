using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WhackAMole.UWPClient.ViewModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace WhackAMole.UWPClient.Controls
{


    public enum MoleAlignment { TopLeft, BottomLeft, TopRight, BottomRight}

    public class MoleTappedArgs : EventArgs
    {
        public string MoleName { get; set; }
        public MoleViewModel Mole { get; set; }
    }


    [TemplatePart(Name = "PART_Mole", Type = typeof(Ellipse))]
    [TemplatePart(Name = "PART_Value", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_MoleName", Type = typeof(Run))]
    [TemplatePart(Name = "PART_Timestamp", Type = typeof(Run))]
    [TemplatePart(Name = "PART_DisplayTransform", Type = typeof(CompositeTransform))]
    [TemplatePart(Name = "PART_MoleTransform", Type = typeof(TranslateTransform))]
    [TemplatePart(Name = "PART_ValueTransform", Type = typeof(TranslateTransform))]
    //[TemplatePart(Name = "PART_Line", Type = typeof(Line))]
    [TemplatePart(Name = "PART_DisplayBorder", Type = typeof(Border))]
    public sealed class MoleControl : Control
    {
        const int MOLE_SPEED = 5;
        double _maxTransformX;
        double _maxTransformY;
        private static Random _rnd;
        private Point _vector;
        private bool _isDestroyed;

        Ellipse _mole;
        TextBlock _displayValue;
        private Run _moleName;
        private Run _timeStamp;
        //private Line _line;
        private CompositeTransform _displayTransform;
        private TranslateTransform _moleTransform;
        private TranslateTransform _valueTransform;
        private Border _displayBorder;

        private bool _isReady;

        static MoleControl()
        {
            _rnd = new Random(DateTime.Now.Millisecond);
            
        }

        public event EventHandler<MoleTappedArgs> MoleTapped;



        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Point.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Point), typeof(MoleControl), new PropertyMetadata(null,OnPositionChanged));

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as MoleControl;
            var position = (Point)e.NewValue;
            me.SetPosition(position);
        }

        private void SetPosition(Point position)
        {
            SetPosition(position.X, position.Y);
        }

        private void SetPosition(double x, double y)
        {
            _moleTransform.X = x;
            _moleTransform.Y = y;
        }

        public int Speed
        {
            get { return (int)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Speed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof(int), typeof(MoleControl), new PropertyMetadata(MOLE_SPEED, OnSpeedChanged));

        private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as MoleControl;
        }

        public MoleAlignment DisplayAlignment
        {
            get { return (MoleAlignment)GetValue(DisplayAlignmentProperty); }
            set { SetValue(DisplayAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Alignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayAlignmentProperty =
            DependencyProperty.Register("DisplayAlignment", typeof(MoleAlignment), typeof(MoleControl), new PropertyMetadata(MoleAlignment.TopRight, OnDisplayAlignmentChanged));

        private static void OnDisplayAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = d as MoleControl;
            var alignment = (MoleAlignment)e.NewValue;
            me.LayoutDisplayPanel();
        }

           public string DisplayValue
        {
            get { return (string)GetValue(DisplayValueProperty); }
            set { SetValue(DisplayValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayValueProperty =
            DependencyProperty.Register("DisplayValue", typeof(string), typeof(MoleControl), new PropertyMetadata("", new PropertyChangedCallback(OnDisplayValueChanged)));

        private static void OnDisplayValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            var me = d as MoleControl;
            if (!me._isReady)
                return;

            //me._displayValue.Text = e.NewValue as string;
        }



        public string MoleName
        {
            get { return (string)GetValue(MoleNameProperty); }
            set { SetValue(MoleNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MoleName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoleNameProperty =
            DependencyProperty.Register("MoleName", typeof(string), typeof(MoleControl), new PropertyMetadata(""));
    

        public MoleControl()
        {
            this.DefaultStyleKey = typeof(MoleControl);
        }

        public void Init(double maxX, double maxY)
        {
            _maxTransformX = maxX;
            _maxTransformY = maxY;

            var direction = _rnd.Next(0, 360);
            _vector = new Point(((Math.Cos(direction * Math.PI / 180) * Speed)), (-Math.Sin(direction * Math.PI / 180) * Speed));
            
           
        }

        public void UpdatePosition()
        {

            var newX = Position.X + _vector.X;
            var newY = Position.Y + _vector.Y;

            if (newY > _maxTransformY || newY < 0)
            {
                var y = (newY < 0) ? 0 : _maxTransformY;
                Position = new Point(Position.X, y); ;
                _vector.Y = _vector.Y * -1;
                
            }
            else
            {
                Position = new Point(Position.X, newY); 
                if (_maxTransformY - newY <= _displayBorder.ActualHeight + 50 && (DisplayAlignment == MoleAlignment.BottomLeft || DisplayAlignment == MoleAlignment.BottomRight))
                    DisplayAlignment = (DisplayAlignment == MoleAlignment.BottomLeft) ? MoleAlignment.TopLeft : MoleAlignment.TopRight;

                if (newY < _displayBorder.ActualHeight + 50 && (DisplayAlignment == MoleAlignment.TopRight || DisplayAlignment == MoleAlignment.TopLeft))
                    DisplayAlignment = (DisplayAlignment == MoleAlignment.TopRight) ? MoleAlignment.BottomRight : MoleAlignment.BottomLeft;
            }

            if (newX > _maxTransformX || newX < 0)
            {
                var x = (newX < 0) ? 0 : _maxTransformX;
                Position = new Point(x, Position.Y); 
                _vector.X = _vector.X * -1;
            }
            else
            {
                Position = new Point(newX, Position.Y);
                if (_maxTransformX - newX <= _displayBorder.ActualWidth + 50 && (DisplayAlignment == MoleAlignment.BottomRight || DisplayAlignment == MoleAlignment.TopRight))
                    DisplayAlignment = (DisplayAlignment == MoleAlignment.BottomRight) ? MoleAlignment.BottomLeft : MoleAlignment.BottomRight;

                if (newX < _displayBorder.ActualWidth + 50 && (DisplayAlignment == MoleAlignment.TopLeft || DisplayAlignment == MoleAlignment.BottomLeft))
                    DisplayAlignment = (DisplayAlignment == MoleAlignment.TopLeft) ? MoleAlignment.TopRight : MoleAlignment.BottomRight;
            }

           
        


        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _displayBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //_displayValue.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return base.MeasureOverride(availableSize);

        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _displayBorder.Arrange(new Rect(new Point(), _displayBorder.DesiredSize));
            //_displayValue.Arrange(new Rect(new Point(), _displayValue.DesiredSize));
            LayoutDisplayPanel();

            _valueTransform.X = _mole.ActualWidth / 2 - _displayValue.ActualWidth / 2;
            _valueTransform.Y = _mole.ActualHeight / 2 - _displayValue.ActualHeight / 2;
            //SetPosition(_rnd.NextDouble() * _maxTransformX, _rnd.NextDouble() * _maxTransformY);
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnApplyTemplate()
        {
            _mole = GetTemplateChild("PART_Mole") as Ellipse;
            _displayValue = GetTemplateChild("PART_Value") as TextBlock;
            _moleName = GetTemplateChild("PART_MoleName") as Run;
            _timeStamp = GetTemplateChild("PART_Timestamp") as Run;
            //_line = GetTemplateChild("PART_Line") as Line;
            _displayTransform = GetTemplateChild("PART_DisplayTransform") as CompositeTransform;
            _moleTransform = GetTemplateChild("PART_MoleTransform") as TranslateTransform;
            _valueTransform = GetTemplateChild("PART_ValueTransform") as TranslateTransform;
            _displayBorder = GetTemplateChild("PART_DisplayBorder") as Border;

            VisualStateManager.GoToState(this, "Normal", true);
            _mole.Tapped += _mole_Tapped;
            _isReady = true;
            base.OnApplyTemplate();
        }

        private void _mole_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;
            MoleTapped?.Invoke(this, new MoleTappedArgs() { MoleName = MoleName, Mole = this.DataContext as MoleViewModel });
        }

        private void LayoutDisplayPanel()
        {

            if (_displayTransform == null || _displayBorder == null || _mole == null)
                return;
            

            int angle = 0;
            double offSetX = _displayBorder.ActualWidth - _mole.ActualWidth;

            var align = DisplayAlignment;
            switch (align)
            {

                case MoleAlignment.BottomRight:
                    offSetX = 0;
                    angle = 315;
                    break;
                case MoleAlignment.BottomLeft:
                    _displayTransform.CenterX = 1;

                    angle = 225;
                    break;
                case MoleAlignment.TopRight:
                    offSetX = 0; ;
                    angle = 45;
                    break;
                case MoleAlignment.TopLeft:
                    _displayTransform.CenterX = 1;
                    angle = 135;
                    break;
                default:
                    break;
            }
            double radius = _mole.ActualWidth  + 10;
            var targetPoint = new Point(((Math.Cos(angle * Math.PI / 180) * radius)) , ( -Math.Sin(angle*Math.PI / 180) * radius));
            _displayTransform.TranslateX = targetPoint.X - offSetX;
            _displayTransform.TranslateY = targetPoint.Y;
        }

       


    }
}
