using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EX2
{
    /// <summary>
    ///logic for JoystickControl.xaml
    /// </summary>
    public partial class JoystickControl : BindableUserControl
    {
        public double AxisX
        {
            get => (double)GetValue(AxisXProperty);
            set => SetValue(AxisXProperty, value);
        }

        public static readonly DependencyProperty AxisXProperty =
            DependencyProperty.Register(nameof(AxisX), typeof(double),
                typeof(JoystickControl), new FrameworkPropertyMetadata(default(double),
                    new PropertyChangedCallback((d, a) =>
                    {
                        ((JoystickControl)d).OnPropertyChanged(nameof(CurrentPosition));
                    })));

        public double AxisY
        {
            get => (double)GetValue(AxisYProperty);
            set=> SetValue(AxisYProperty, value);
        }

        public static readonly DependencyProperty AxisYProperty =
            DependencyProperty.Register(nameof(AxisY), typeof(double),
                typeof(JoystickControl), new FrameworkPropertyMetadata(default(double),
                    new PropertyChangedCallback((d, a) =>
                    {
                        ((JoystickControl)d).OnPropertyChanged(nameof(CurrentPosition));
                    })));

        const double radius = 77;
        public Thickness CurrentPosition
        {
            get
            {
                var test = SetValue(new Vector(Math.Abs(AxisX), Math.Abs(AxisY)), radius);
                return new Thickness(test.Item1 * Math.Sign(AxisX), -test.Item2 * Math.Sign(AxisY), 0, 0);
            }
        }

        private (double, double) SetValue(Vector nonNormalizedValue, double radius)
        {
            //Polar coord system
            double angle = Math.Atan2(nonNormalizedValue.Y, nonNormalizedValue.X);

            if (nonNormalizedValue.Length > 1.0)
            {
                nonNormalizedValue.X = Math.Cos(angle);
                nonNormalizedValue.Y = Math.Sin(angle);
            }

            return UpdateKnobPosition(nonNormalizedValue, radius, 45);
        }

        private (double, double) UpdateKnobPosition(Vector joystickPos, double fJoystickRadius, double fKnobRadius)
        {
            return (joystickPos.X * fJoystickRadius + fJoystickRadius - fKnobRadius, joystickPos.Y * fJoystickRadius + fJoystickRadius - fKnobRadius);
        }

        public JoystickControl()
        {
            InitializeComponent();
        }
    }
}
