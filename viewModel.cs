using System.Collections.Generic;
using System;
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
using Microsoft.Win32;
using System.ComponentModel;
using Helpers.MVVM;
using System.Timers;
using System.Globalization;

namespace EX2
{
    public partial class ViewModel : BindableBase
    {

        System.Timers.Timer playTimer = new System.Timers.Timer();
        //responsible for the amount of lines read per second.
        private const int BUFFER_SIZE = 10;
        private const int BASE_FRAMERATE_PER_SECOND = 10;
        private FlightSimulator sim;



        private FrameStack model;
        private FrameStack Model 
        { 
            get
            {
                return model;
            }
            set
            {
                model = value;
                OnPropertyChanged(nameof(Model));
                OnPropertyChanged(nameof(FrameStackProperties));
                OnPropertyChanged(nameof(SliderMaximum));
            }
        }

        public List<string> FrameStackProperties
        {
            get
            {
                return Model?.Properties ?? new List<string>();
            }
        }
        //resposible for the slider's value.
        public int SliderMaximum
        {
            get
            {
                return (model?.Count - 1 - BUFFER_SIZE) ?? 0;
            }
        }

        private object selectedItem;
        public object SelectedProperty
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = (string)value;
                OnPropertyChanged(nameof(SelectedProperty));
                DataPropertyChanged();
            }
        }

        private int currentFrame;
        public int CurrentFrame
        {
            get
            {
                return currentFrame;
            }
            set
            {
                currentFrame = value;
                OnPropertyChanged(nameof(CurrentFrame));
                OnPropertyChanged(nameof(CurrentTime));
                DataPropertyChanged();
            }
        }
        //here  we bind all the features(graphs,clocks etc),
        //and they are being notified when certain change occured.
        private void DataPropertyChanged()
        {
            OnPropertyChanged(nameof(CurrentDataSet));
            OnPropertyChanged(nameof(TopGraphImageSource));
            OnPropertyChanged(nameof(TopGraphImageSource2));
            OnPropertyChanged(nameof(BottomGraphImageSource));
            OnPropertyChanged(nameof(AileronProperty));
            OnPropertyChanged(nameof(ElevatorPropery));
            OnPropertyChanged(nameof(Throttle));
            OnPropertyChanged(nameof(Rudder));

            OnPropertyChanged(nameof(Altimeter));
            OnPropertyChanged(nameof(Airspeed));
            OnPropertyChanged(nameof(FlightDirection));
            OnPropertyChanged(nameof(Pitch));
            OnPropertyChanged(nameof(Yaw));
        }
        //the current 10 lines or whatever value we chose.
        public double[] CurrentDataSet
        {
            get
            {
                try
                {
                    return model[(string)SelectedProperty].Skip(currentFrame + 1).Take(BUFFER_SIZE).ToArray();
                }
                catch
                {
                    return new double[0];
                }
            }
        }

        private double speedMultiplier = 1;
        public double SpeedMultiplier
        {
            get
            {
                return speedMultiplier;
            }
            set
            {
                if(Math.Round(value, 6) > 0)
                {
                    speedMultiplier = value;
                    UpdateTimerInterval();
                    OnPropertyChanged(nameof(SpeedMultiplier));
                    OnPropertyChanged(nameof(CurrentFrameRate));
                    OnPropertyChanged(nameof(CurrentTime));
                }
            }
        }

        public double CurrentFrameRate
        {
            get
            {
                return BASE_FRAMERATE_PER_SECOND * SpeedMultiplier;
            }
        }
        //gives us the time of the current value if the time.
        public TimeSpan CurrentTime
        {
            get
            {
                return TimeSpan.FromMilliseconds(CurrentFrame * playTimer.Interval);
            }
        }

        #region BindedProperties
 //top first graph on the left which shows the information. based on user pick. 
        public DrawingImage TopGraphImageSource
        {
            get
            {
                var ds = CurrentDataSet;
                return new DrawingImage(GetGraphGroup(ds, false, 10, 10, ds.Length > 0 ? (int)Math.Ceiling(Math.Max(Math.Abs(model[(string)SelectedProperty].Min()), Math.Abs(model[(string)SelectedProperty].Max()))) : 1));
            }
        }

        //top second  graph on the left which shows the information. should be based on correlation.
        public DrawingImage TopGraphImageSource2
        {
            get
            {
                var ds = model["airspeed"].Skip(currentFrame + 1).Take(BUFFER_SIZE).ToArray(); 

                return new DrawingImage(GetGraphGroup(ds, false, 10, 10, ds.Length > 0 ? (int)Math.Ceiling(Math.Max(Math.Abs(model["airspeed"].Min()), Math.Abs(model["airspeed"].Max()))) : 1));
            }
        }
        //bottom graph.
        public DrawingImage BottomGraphImageSource
        {
            get
            {
                var ds = CurrentDataSet;
                
                return new DrawingImage(GetGraphGroup(ds, true, 20, 10, ds.Length > 0 ? (int)Math.Ceiling(Math.Max(Math.Abs(model[(string)SelectedProperty].Min()), Math.Abs(model[(string)SelectedProperty].Max()))) : 1));
            }
        }

        public double AileronProperty => model["aileron"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double ElevatorPropery => model["elevator"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();

        public double Throttle => model["throttle"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double ThrottleMax => model["throttle"].Max();
        public double ThrottleMin => model["throttle"].Min();

        public double Rudder => model["rudder"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double RudderMin => model["rudder"].Min();
        public double RudderMax => model["rudder"].Max();

       
        public double Altimeter => model["altimeter"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double AltimeterMin => model["altimeter"].Min();
        public double AltimeterMax => model["altimeter"].Max();

        public double Airspeed => model["airspeed"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double AirspeedMin => model["airspeed"].Min();
        public double AirspeedMax => model["airspeed"].Max();

        public double FlightDirection => model["flight direction"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double FlightDirectionMin => model["flight direction"].Min();
        public double FlightDirectionMax => model["flight direction"].Max();

        public double Pitch => model["pitch"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double PitchMin => model["pitch"].Min();
        public double PitchMax => model["pitch"].Max();

        public double Yaw => model["yaw"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double YawMin => model["yaw"].Min();
        public double YawMax => model["yaw"].Max();

        #endregion

        public RelayCommand StopCommand { get; private set; } 

        public RelayCommand PauseCommand { get; private set; } 

        public RelayCommand ChangeSpeedCommand { get; private set; }

        public ViewModel(FlightSimulator sim)
        {
            //Test version of model. Just for testing
            this.sim = sim;
            Model = new FrameStack(sim);

            ChangeSpeedCommand = new RelayCommand((s) =>
            {
                var deltaSpeedMultiplier = Convert.ToDouble(s, System.Globalization.CultureInfo.InvariantCulture);
                //Change timer speed only on new frame start not to make "double resetting" if it is running now

                if(playTimer.Enabled)
                {
                    ElapsedEventHandler dlg = null;
                    dlg = (s1, e1) =>
                    {
                        SpeedMultiplier += deltaSpeedMultiplier;
                        playTimer.Elapsed -= dlg;
                    };
                    playTimer.Elapsed += dlg;
                }
                else
                {
                    SpeedMultiplier += deltaSpeedMultiplier;
                }

            });

            PauseCommand = new RelayCommand((s) =>
            {
                //not used at the moment.
                if(SelectedProperty == null)
                {
                    MessageBox.Show("Chose the property in the listbox to show");
                    return;
                }

                var newStateIsPlaying = Convert.ToBoolean(s);

                if(newStateIsPlaying)
                {
                    if (CurrentFrame == SliderMaximum)
                    {
                        CurrentFrame = 0;
                    }

                    playTimer.Start();
                }
                else
                {
                    playTimer.Stop();
                }
            });

            StopCommand = new RelayCommand((s) =>
            {
                playTimer.Stop();
                CurrentFrame = 0;
            });

            UpdateTimerInterval();
            playTimer.Elapsed += (s, e) =>
            {
                try
                {
                    //Executing this in main (UI) thread because otherwise it will not push UI to update
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //If timer is still enabled (fixes behavior when elapsed task appears in threadpool earlier when stop-command)
                        if (playTimer.Enabled)
                        {
                            if (CurrentFrame != SliderMaximum)
                            {
                                CurrentFrame++;
                            }
                            else
                            {
                                playTimer.Stop();
                            }
                        }
                    });
                }
                catch
                {

                }
            };
        }

        private void UpdateTimerInterval()
        {
            playTimer.Interval = 1000 / CurrentFrameRate;
        }
        private DrawingGroup GetGraphGroup(double[] Data, bool dots, int width = 10, int height = 10, int maximum = 5)
        {
            DrawingGroup aDrawingGroup = new DrawingGroup();
            int Np = Data.Length - 1;


            for (int DrawingStage = 0; DrawingStage < 10; DrawingStage++)
            {
                GeometryDrawing drw = new GeometryDrawing();
                GeometryGroup gg = new GeometryGroup();


                //Background
                if (DrawingStage == 1)
                {
                    drw.Brush = Brushes.Beige;
                    drw.Pen = new Pen(Brushes.LightGray, 0.01);

                    RectangleGeometry myRectGeometry = new RectangleGeometry();
                    myRectGeometry.Rect = new Rect(0, 0, width, height);
                    gg.Children.Add(myRectGeometry);
                }

                if (DrawingStage == 3)
                {
                    if(!dots)
                    {
                        drw.Brush = Brushes.White;
                        drw.Pen = new Pen(Brushes.Black, 0.05);

                        gg = new GeometryGroup();
                        for (int i = 0; i < Np; i++)
                        {
                            LineGeometry l = new LineGeometry(new Point((double)(i * width) / Np, height / 2d - Data[i] / ((double)maximum * 2 / height)),
                                new Point((double)(i + 1) * width / Np, height / 2 - (Data[i + 1]) / ((double)maximum * 2 / height)));
                            gg.Children.Add(l);
                        }
                    }
                    else
                    {
                        drw.Brush = Brushes.Black;
                        drw.Pen = new Pen(Brushes.Black, 0.05);

                        gg = new GeometryGroup();
                        for (int i = 0; i < Np; i++)
                        {
                            EllipseGeometry el = new EllipseGeometry(new Point((double)(i * width) / Np, height / 2 - Data[i] / ((double)maximum * 2 / height)), 0.01 * Math.Min(width, height), 0.01 * Math.Min(width, height));
                            gg.Children.Add(el);
                        }
                    }

                }

                //Cutting
                if (DrawingStage == 5)
                {
                    drw.Brush = Brushes.Transparent;
                    drw.Pen = new Pen(Brushes.White, 0.2);

                    RectangleGeometry myRectGeometry = new RectangleGeometry();
                    myRectGeometry.Rect = new Rect(-0.1 * width, -0.1 * height, 1.2 * width, 1.2 * height);
                    gg.Children.Add(myRectGeometry);
                }


                //border-מסגרת.
                if (DrawingStage == 6)
                {
                    drw.Brush = Brushes.Transparent;
                    drw.Pen = new Pen(Brushes.LightGray, 0.01);

                    RectangleGeometry myRectGeometry = new RectangleGeometry();
                    myRectGeometry.Rect = new Rect(0 * width, 0 * height, 1 * width, 1 * height);
                    gg.Children.Add(myRectGeometry);
                }


                //labels
                if (DrawingStage == 7)
                {
                    drw.Brush = Brushes.LightGray;
                    drw.Pen = new Pen(Brushes.Gray, 0.003);

                    for (int i = 0; i < 11; i++)
                    {
                        // Create a formatted text string.
#pragma warning disable CS0618 
                        FormattedText formattedText = new FormattedText(
                            ((double)(maximum - i * 0.1 * maximum * 2)).ToString(),
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Verdana"),
                            0.05 * height,
                            Brushes.Black);
#pragma warning restore CS0618 

                        // Set the font weight to Bold for the formatted text.
                        formattedText.SetFontWeight(FontWeights.Bold);

                        // Build a geometry out of the formatted text.
                        Geometry geometry = formattedText.BuildGeometry(new Point(-0.1 * width, i * 0.1 * height - 0.03 * height));
                        gg.Children.Add(geometry);
                    }
                }

                drw.Geometry = gg;
                aDrawingGroup.Children.Add(drw);
            }

            return aDrawingGroup;
        }


        public void set_train_csv(string name)
        {
            sim.RegFlightCSV = name;
        }

        public void set_test_csv(string name)
        {
            sim.AnomalyFlightCSV = name;
        }

        public void set_flight_gear(string name)
        {
            sim.setFGPath(name);
        }

        public void change_speed(int value)
        {
            sim.Playback_speed += value;
        }

        /// <summary>
        /// This button plays the playback
        /// </summary>
        public void play()
        {
            sim.play();
        }

        /// <summary>
        /// This button pauses the playback
        /// </summary>
        public void stop()
        {
            sim.Pause = true;
        }

        /// <summary>
        /// This button stops the playback and next time it will play from the start
        /// </summary>
        public void restart()
        {            
            sim.Stop = true;
        }

    }



    //general format of accepting information to be presented.
    class FrameStack
    {
        private Dictionary<string, double[]> frames;

        public List<string> Properties
        {
            get
            {
                return frames.Keys.ToList();
            }
        }

        public double[] this[string propertyName]
        {
            get
            {
                return frames[propertyName];
            }
        }

        public int Count
        {
            get
            {
                return frames.First().Value.Length;
            }
        }

        public FrameStack(FlightSimulator sim)
        {
            //altimeter,airspeed,flight direction,pitch,yaw,roll
            //TEST DATA INJECTING


            /* 
            Dictionary<string, List<float>> data = new Dictionary<string, List<float>>();
            foreach (string var in sim.Variables)
            {
                data.Add(var, new List<float>(sim.get_col(var))); // NOT IMPLEMENTED YET, need to get timeseries working first.
            }
            */

            #region DataInjewcting
            frames = new Dictionary<string, double[]>()
            
            {
                {"throttle",            new double[]{ 9, 8, 3, 7, 4, 4, 3, 5, 4, 2, 8, 1, 6, 1, 8, 4, 2, 4, 3, 9, 8, 4, 8, 2, 9, 7, 6, 8, 7, 7, 6, 3, 5, 6, 8, 6, 5, 6, 10, 10, 5, 3, 4, 10, 5, 3, 4, 5, 6, 7, 9, 8, 3, 6, 9, 4, 3, 1, 8, 3, 1, 1, 6, 2, 8, 1, 7, 6, 1, 8, 9, 1, 4, 3, 4, 1, 9, 6, 3, 5, 7, 8, 4, 7, 10, 2, 7, 10, 8, 4, 2, 9, 5, 10, 5, 7, 1, 1, 3, 1, 3, 1, 1, 2, 2, 8, 10, 6, 4, 1, 6, 5, 6, 4, 4, 3, 9, 2, 9, 9, 2, 9, 2, 1, 2, 6, 9, 2, 10, 3, 6, 3, 2, 3, 2, 2, 10, 4, 7, 5, 2, 4, 7, 4, 1, 2, 9, 10, 8, 1, 7, 2, 8, 9, 5, 4, 10, 5, 6, 5, 8, 9, 9, 3, 3, 7, 3, 2, 7, 6, 5, 3, 7, 1, 5, 2, 1, 5, 9, 2, 3, 6, 8, 4, 5, 7, 7, 10, 6, 2, 5, 5, 7, 7, 3, 2, 3, 6, 1, 10, 2, 8, 7, 1, 2, 4, 5, 9, 2, 3, 9, 4, 2, 6, 5, 8, 8, 1, 10, 1, 9, 1, 2, 6, 1, 2, 5, 5, 5, 4, 4, 7, 3, 3, 5, 4, 4, 3, 2, 9, 7, 5, 3, 2, 10, 5, 4, 3, 7, 9 } },
                {"angle",               new double[]{ 3, 5, 7, 8, 6, 3, 5, 10, 6, 1, 10, 7, 10, 5, 3, 2, 7, 5, 9, 1, 1, 3, 7, 9, 9, 5, 6, 6, 6, 7, 1, 1, 5, 9, 5, 5, 7, 9, 6, 6, 6, 5, 8, 7, 10, 6, 3, 4, 3, 5, 5, 2, 9, 9, 6, 7, 3, 9, 8, 5, 5, 10, 9, 3, 1, 5, 9, 9, 5, 7, 3, 6, 3, 7, 2, 4, 7, 3, 3, 4, 2, 3, 5, 6, 4, 8, 3, 10, 1, 2, 4, 3, 3, 9, 8, 10, 9, 7, 9, 1, 2, 3, 9, 5, 1, 4, 3, 6, 3, 8, 9, 8, 1, 3, 3, 7, 6, 2, 6, 1, 7, 5, 8, 1, 3, 1, 4, 4, 2, 1, 2, 3, 9, 10, 3, 2, 7, 4, 6, 4, 10, 8, 10, 10, 10, 7, 5, 5, 6, 7, 10, 9, 6, 8, 7, 8, 4, 10, 4, 5, 7, 6, 7, 10, 7, 9, 4, 3, 3, 5, 8, 10, 5, 1, 4, 1, 5, 6, 3, 4, 10, 8, 3, 9, 1, 6, 1, 9, 1, 6, 10, 1, 10, 2, 7, 7, 3, 9, 5, 4, 2, 3, 4, 8, 2, 2, 2, 4, 8, 1, 7, 4, 6, 1, 10, 6, 4, 9, 3, 6, 10, 4, 3, 2, 5, 10, 3, 5, 10, 7, 8, 2, 6, 6, 8, 5, 3, 3, 2, 8, 7, 10, 10, 7, 5, 5, 1, 9, 7, 8 } },
                {"rudder",              new double[]{ 5, 8, 7, 1, 4, 2, 8, 4, 5, 6, 9, 10, 10, 4, 8, 6, 10, 7, 4, 5, 1, 3, 6, 1, 6, 1, 8, 2, 3, 1, 5, 6, 5, 1, 1, 5, 6, 10, 7, 10, 2, 9, 8, 8, 7, 5, 9, 4, 6, 4, 7, 5, 7, 9, 9, 8, 2, 2, 9, 4, 9, 7, 1, 3, 8, 2, 7, 5, 5, 9, 5, 4, 10, 8, 1, 5, 8, 10, 9, 7, 4, 4, 2, 3, 8, 1, 2, 1, 3, 7, 4, 9, 3, 1, 9, 7, 5, 2, 6, 5, 7, 10, 5, 2, 5, 9, 8, 8, 10, 1, 1, 3, 10, 5, 4, 10, 5, 7, 10, 5, 9, 7, 8, 10, 10, 4, 8, 6, 7, 4, 3, 4, 6, 7, 2, 5, 4, 2, 5, 4, 6, 4, 8, 7, 5, 1, 1, 2, 1, 1, 7, 7, 4, 2, 6, 6, 1, 7, 1, 7, 2, 4, 9, 10, 7, 5, 8, 7, 1, 9, 4, 8, 7, 4, 6, 1, 2, 5, 6, 6, 5, 1, 9, 4, 6, 9, 3, 10, 2, 3, 8, 4, 6, 5, 4, 7, 9, 2, 3, 7, 3, 9, 5, 4, 4, 7, 4, 10, 7, 6, 5, 2, 2, 10, 8, 9, 1, 2, 6, 5, 2, 2, 8, 8, 4, 9, 1, 3, 4, 4, 2, 3, 5, 10, 6, 3, 2, 2, 6, 6, 5, 9, 3, 3, 9, 4, 5, 2, 2, 5 } },
                {"aileron",             new double[]{ -0.32, -0.48, 0.71, -0.21, -0.20, -0.62, -0.10, -0.92, 0.79, -0.12, 0.11, 0.59, -0.65, 0.55, 0.96, -0.67, 0.44, 0.74, 0.07, 0.39, -0.59, 0.88, -0.23, 0.02, 0.52, -0.19, 0.66, -0.63, 0.62, -0.27, -0.30, 0.97, 0.10, -0.57, -0.46, -0.40, 0.52, -0.59, -0.63, -0.59, -0.83, 0.90, -0.22, -0.34, 0.90, -0.95, 0.02, 0.59, 0.20, -0.64, -0.75, 0.29, 0.14, 0.96, -0.05, 0.84, 0.02, 0.01, 0.58, -0.89, -0.37, 0.43, 0.59, -0.68, 0.05, 0.64, -0.15, -0.90, -0.25, -0.91, -0.70, -0.83, 0.72, -0.81, -0.79, 0.29, -0.08, 0.96, -0.01, -0.65, -0.83, 0.08, 0.36, -0.86, -0.80, 0.85, -0.48, 0.78, 0.64, -0.99, -0.71, -0.22, 0.78, -0.45, 0.93, -0.93, 0.52, -0.79, -0.77, 0.65, 0.19, -0.61, 0.07, -0.19, 0.23, -0.51, 0.41, 0.21, -0.32, 0.71, 0.85, 0.53, 0.59, -0.40, -0.86, 0.59, 0.71, 0.00, 0.89, 0.62, -0.65, -0.59, 0.61, 0.38, -0.93, 0.53, -0.31, -0.74, -0.61, -0.38, -0.64, 0.50, 0.26, -0.24, 0.88, 0.94, 0.05, -0.45, -0.84, 0.07, -0.51, -0.50, 0.61, -0.48, -0.68, 0.95, 0.23, 0.92, -0.47, 0.44, 0.12, -0.50, 0.16, -0.63, -0.13, 0.88, -0.17, 0.88, 0.34, 0.20, 0.88, -0.77, 0.48, 0.95, 0.26, 0.11, -0.35, 0.84, 0.07, 0.08, -0.97, 0.61, 0.96, -0.96, 0.02, 0.37, -0.45, 0.14, 0.66, 0.28, -0.81, -0.10, -0.57, 0.07, -0.71, -0.43, -0.07, 0.54, 0.33, -0.36, -0.47, 0.82, 0.09, -0.15, 0.04, -0.76, 0.94, 0.47, 0.98, -0.06, 0.28, -0.04, 0.76, 0.86, 0.42, -0.94, 0.81, -0.22, -0.95, -0.65, 0.81, -0.32, 0.33, 0.66, 0.20, -0.03, 0.98, -0.64, 0.26, -0.30, 0.79, -0.37, 0.12, 0.30, -0.65, -0.28, 0.83, 0.44, -0.74, 0.05, -0.14, 0.67, 0.98, -0.78, 0.84, 0.49, 0.66, 0.19, 0.39, -0.99, -0.53, 0.58, -0.42, 0.41, 0.69, -0.00, 0.02, -0.43, 0.15, -0.63 } },
                {"elevator",            new double[]{ -0.93, 0.24, 0.94, -0.12, 0.68, 0.32, 0.44, 0.47, 0.77, 0.46, 0.80, -0.01, 0.10, -0.24, -0.20, 0.01, -0.58, 0.16, 0.46, -0.19, 0.77, 0.06, 0.66, 0.68, 0.76, -0.83, -0.52, 0.40, 0.90, 0.89, 0.36, -0.06, -0.48, -0.95, 0.33, 0.69, 0.01, -0.33, 0.75, 0.82, -0.91, -0.50, 0.02, -0.85, -0.65, 0.03, -0.58, -0.09, -0.57, 0.90, 0.13, 0.85, -0.69, 0.26, 0.00, 0.01, 0.95, -0.14, -0.43, -0.36, -0.99, -0.15, -0.31, -0.98, -0.06, 0.96, 0.74, -0.36, 0.53, 0.31, 0.10, 0.85, -0.30, -0.08, -0.20, -0.60, -0.36, -0.80, -0.44, 0.15, -0.09, 0.68, -0.50, 0.29, 0.42, -0.50, 0.12, -0.07, -0.13, 0.26, 0.18, 0.84, 0.58, -0.16, -0.48, -0.29, -0.18, 0.51, -0.52, 0.31, 0.60, 0.06, -0.02, 0.04, -0.29, 0.90, 0.37, 0.68, 0.59, -0.64, -0.95, 0.81, -0.67, -0.42, 0.04, -0.72, 0.04, -0.80, -0.71, -0.87, 0.60, 0.82, 0.18, -0.19, -0.79, 0.26, -0.34, 0.08, -0.36, -0.52, 0.11, -0.78, 0.18, 0.05, -0.76, 0.30, 0.96, -0.89, -0.11, 0.87, -0.14, 0.97, -0.85, 0.43, -0.55, 0.97, -0.32, -0.25, 0.14, -0.45, 0.56, -0.14, 0.85, 0.66, -0.21, 0.25, -0.56, 0.10, 0.49, 0.22, 0.92, 0.01, -0.21, -0.87, 0.59, 0.28, 0.41, -0.63, -0.28, -0.43, 0.28, -0.70, -0.89, 0.28, 0.17, -0.52, 0.71, 0.58, 0.31, 0.20, 0.35, 0.64, -0.17, -0.20, 0.94, -0.46, -0.66, 0.99, -1.00, 0.89, 1.00, 0.99, -0.19, 0.54, 0.41, -0.62, -0.36, 0.46, -0.20, 0.90, 0.18, -0.80, 0.72, 0.44, -0.95, 0.78, -0.73, -0.48, -0.16, 0.40, -0.98, 0.11, 0.93, -0.37, 0.43, 0.64, -0.19, -0.62, 0.29, -0.12, 0.69, -0.84, -0.20, 0.42, 0.20, 0.80, -0.36, -0.88, -0.87, -0.23, -0.47, -0.53, -0.14, 0.44, -0.01, -0.74, 0.38, 0.69, -0.09, -0.32, -0.54, -0.09, 0.15, 0.90, 0.43, -0.07, 0.84, 0.81, 0.37, -0.73 } },
                {"altimeter",           new double[]{ 7, 6, 10, 2, 8, 2, 1, 7, 3, 9, 8, 9, 7, 4, 5, 7, 7, 9, 2, 5, 7, 1, 4, 8, 4, 4, 8, 1, 6, 8, 8, 10, 5, 8, 9, 5, 8, 9, 1, 10, 7, 1, 9, 2, 3, 4, 7, 1, 8, 6, 5, 2, 2, 6, 7, 1, 6, 8, 9, 5, 7, 1, 2, 2, 10, 1, 1, 7, 2, 5, 10, 8, 9, 3, 8, 8, 10, 10, 2, 9, 2, 5, 9, 7, 5, 8, 1, 10, 7, 8, 2, 2, 4, 7, 6, 9, 1, 1, 1, 9, 6, 4, 5, 9, 4, 10, 10, 9, 7, 6, 7, 1, 8, 10, 4, 3, 2, 5, 6, 4, 6, 2, 2, 8, 7, 1, 4, 4, 5, 7, 10, 5, 10, 10, 8, 7, 10, 8, 5, 4, 7, 4, 1, 6, 10, 3, 4, 5, 7, 6, 10, 6, 3, 3, 2, 8, 4, 9, 9, 5, 9, 7, 9, 6, 4, 10, 7, 7, 2, 7, 2, 8, 1, 3, 7, 1, 1, 3, 2, 5, 2, 3, 5, 7, 4, 6, 10, 6, 4, 9, 9, 2, 3, 7, 3, 8, 5, 8, 3, 8, 9, 5, 8, 2, 8, 4, 9, 9, 7, 6, 10, 10, 4, 6, 6, 10, 4, 4, 3, 6, 4, 1, 7, 2, 5, 7, 5, 2, 1, 5, 9, 2, 10, 6, 2, 4, 1, 8, 3, 5, 2, 2, 6, 10, 1, 1, 2, 9, 5, 4 } },
                {"airspeed",            new double[]{ 6, 6, 4, 5, 8, 1, 5, 5, 3, 4, 3, 4, 1, 4, 7, 9, 5, 3, 6, 2, 1, 9, 10, 10, 1, 6, 7, 7, 2, 3, 3, 5, 8, 10, 3, 7, 10, 4, 7, 5, 9, 8, 9, 10, 6, 5, 5, 2, 4, 9, 10, 3, 9, 2, 8, 1, 3, 10, 9, 2, 3, 4, 7, 3, 10, 9, 7, 8, 10, 5, 1, 1, 2, 9, 4, 6, 5, 5, 5, 8, 6, 3, 2, 9, 4, 4, 4, 2, 7, 7, 7, 9, 1, 7, 3, 1, 4, 5, 10, 6, 3, 10, 8, 6, 10, 5, 10, 6, 10, 3, 3, 1, 8, 8, 10, 9, 1, 7, 9, 4, 8, 3, 7, 9, 9, 3, 9, 5, 6, 6, 10, 7, 5, 7, 1, 4, 1, 4, 7, 1, 2, 9, 5, 1, 5, 5, 9, 8, 1, 10, 1, 3, 2, 6, 1, 5, 1, 4, 5, 9, 2, 2, 5, 4, 2, 10, 8, 5, 2, 6, 3, 6, 8, 3, 4, 8, 10, 8, 3, 8, 9, 4, 8, 8, 3, 7, 6, 7, 5, 3, 8, 7, 2, 4, 3, 5, 9, 10, 8, 6, 1, 1, 1, 5, 2, 7, 8, 6, 8, 1, 5, 3, 2, 9, 3, 1, 8, 4, 2, 6, 8, 3, 5, 8, 3, 6, 1, 4, 1, 6, 6, 3, 5, 10, 8, 10, 6, 1, 3, 4, 10, 7, 4, 2, 5, 10, 4, 7, 4, 3 } },
                {"flight direction",    new double[]{ 9.7, 5.1, 4.8, 7.2, 3.8, 5.6, 9.7, 8.0, 2.0, 2.9, 7.7, 2.2, 4.6, 5.0, 8.5, 7.2, 3.3, 3.8, 3.8, 9.0, 1.9, 8.7, 6.5, 4.2, 6.5, 6.8, 2.3, 2.3, 5.5, 4.8, 5.9, 1.9, 6.2, 4.6, 8.1, 6.1, 2.0, 3.0, 4.8, 3.6, 4.7, 7.2, 3.7, 2.5, 5.6, 4.0, 3.0, 9.8, 5.4, 3.2, 5.4, 9.3, 4.2, 6.4, 5.6, 6.7, 9.3, 4.8, 7.4, 1.4, 8.3, 6.8, 5.5, 4.0, 2.2, 4.8, 5.6, 8.1, 9.5, 9.9, 4.3, 8.9, 6.9, 2.4, 2.0, 2.8, 3.9, 3.6, 7.9, 7.7, 1.4, 1.8, 8.3, 5.5, 3.0, 8.9, 7.9, 6.4, 1.2, 4.6, 8.6, 3.7, 4.9, 4.1, 4.8, 7.7, 1.5, 4.0, 2.8, 4.2, 2.5, 2.1, 4.4, 6.5, 5.2, 1.8, 3.5, 8.1, 8.2, 4.3, 8.4, 7.3, 4.4, 9.8, 2.8, 3.9, 3.2, 1.3, 5.0, 6.8, 1.8, 6.8, 6.5, 7.9, 2.5, 2.6, 2.6, 8.0, 1.8, 7.7, 3.7, 6.1, 7.1, 3.2, 5.8, 3.8, 7.4, 2.3, 9.6, 1.7, 3.4, 5.9, 9.0, 1.3, 6.0, 6.0, 5.3, 3.4, 4.1, 9.7, 3.9, 10.0, 7.6, 2.2, 4.6, 9.6, 2.8, 9.9, 4.4, 9.2, 5.1, 8.4, 2.5, 3.7, 3.9, 2.2, 6.7, 5.5, 7.8, 6.6, 8.7, 1.5, 1.8, 3.8, 6.8, 3.2, 1.9, 4.1, 7.8, 1.7, 5.7, 3.0, 5.8, 2.0, 1.2, 7.0, 5.0, 3.7, 7.5, 9.1, 6.5, 8.7, 1.3, 6.0, 1.2, 6.6, 8.8, 2.1, 9.9, 7.8, 5.9, 4.5, 6.9, 3.3, 9.1, 1.9, 7.4, 4.2, 1.9, 7.4, 7.3, 8.2, 9.5, 9.7, 9.2, 9.8, 1.8, 7.7, 9.6, 9.3, 4.4, 5.6, 9.6, 6.9, 6.5, 9.5, 4.4, 6.2, 2.0, 3.1, 9.4, 7.7, 1.7, 1.9, 7.0, 4.1, 2.3, 4.9, 8.5, 2.0, 2.9, 2.8, 9.7, 8.3, 4.1, 5.6, 6.3, 5.6, 6.2, 2.7 } },
                {"pitch",               new double[]{ 3.5, 4.7, 6.6, 9.0, 6.8, 7.4, 1.1, 5.2, 9.0, 7.9, 8.0, 8.0, 2.8, 5.9, 2.8, 3.3, 4.7, 8.1, 1.0, 3.0, 2.8, 2.1, 1.7, 6.3, 5.5, 6.1, 2.8, 9.0, 3.3, 9.6, 9.5, 3.9, 6.6, 6.5, 2.4, 6.3, 3.1, 5.4, 6.8, 3.8, 6.9, 9.4, 5.0, 8.3, 7.5, 8.6, 2.5, 3.9, 9.2, 8.7, 9.8, 5.3, 5.8, 8.0, 2.0, 2.0, 3.9, 1.6, 8.9, 5.1, 5.1, 9.2, 7.7, 8.9, 7.6, 7.7, 7.6, 8.7, 7.4, 3.8, 6.9, 9.9, 1.7, 4.0, 7.7, 7.0, 4.7, 5.6, 5.8, 3.2, 7.7, 6.4, 2.0, 9.4, 3.4, 4.7, 4.4, 4.2, 6.3, 1.5, 4.9, 6.7, 4.3, 6.2, 8.5, 8.8, 7.3, 7.2, 7.3, 8.7, 9.8, 8.5, 8.0, 6.9, 3.4, 5.8, 7.3, 6.3, 2.8, 2.6, 3.2, 4.1, 4.4, 4.6, 9.2, 9.6, 2.3, 3.4, 3.2, 9.4, 9.9, 4.2, 5.2, 4.0, 2.2, 8.4, 5.1, 4.8, 5.6, 1.4, 5.1, 2.2, 9.2, 5.7, 10.0, 9.0, 2.8, 3.7, 9.9, 2.6, 4.5, 1.1, 2.5, 4.7, 6.6, 1.3, 9.4, 2.6, 3.7, 8.6, 2.2, 9.1, 7.1, 1.9, 6.9, 6.9, 9.0, 5.7, 3.8, 7.8, 1.9, 1.2, 5.8, 9.1, 3.6, 7.6, 5.3, 9.5, 9.6, 8.8, 7.4, 9.3, 3.2, 7.2, 1.8, 1.5, 2.6, 2.5, 9.7, 7.9, 6.2, 5.6, 8.7, 2.9, 4.4, 7.0, 8.7, 6.8, 4.4, 3.3, 2.1, 4.1, 7.0, 8.3, 1.6, 4.5, 6.6, 7.1, 1.6, 1.9, 3.7, 4.6, 7.1, 1.5, 5.9, 6.6, 8.3, 6.7, 9.4, 9.9, 2.4, 8.1, 9.1, 3.2, 6.6, 9.7, 8.7, 8.4, 8.8, 7.2, 2.8, 7.3, 6.8, 6.2, 8.3, 3.4, 5.4, 2.8, 3.1, 1.1, 3.8, 1.4, 4.6, 8.6, 4.2, 1.2, 5.1, 3.2, 5.7, 1.5, 3.1, 2.7, 7.8, 7.4, 8.3, 4.7, 3.2, 1.8, 3.3, 9.5 } },
                {"yaw",                 new double[]{ 4.3, 5.7, 4.8, 9.3, 4.2, 1.2, 9.6, 7.7, 9.4, 1.7, 1.8, 3.3, 9.9, 6.2, 3.0, 8.6, 8.8, 7.2, 8.9, 7.5, 5.3, 1.2, 4.7, 8.5, 9.4, 4.1, 4.0, 5.7, 5.0, 7.4, 5.6, 7.9, 4.7, 2.8, 8.1, 9.1, 9.9, 1.1, 6.1, 6.9, 5.6, 2.8, 7.4, 1.5, 2.3, 6.5, 8.7, 2.6, 1.3, 3.9, 2.5, 8.7, 3.8, 9.9, 5.1, 6.4, 8.7, 3.5, 2.8, 5.6, 8.2, 1.3, 8.5, 1.7, 2.5, 1.5, 4.5, 9.8, 1.9, 9.3, 5.5, 4.1, 3.3, 5.5, 2.2, 2.5, 3.8, 7.0, 1.5, 6.4, 4.3, 2.9, 2.6, 6.2, 2.8, 9.9, 7.1, 9.8, 7.8, 6.1, 6.9, 7.8, 5.9, 4.4, 6.5, 1.3, 7.9, 6.5, 9.9, 9.6, 8.9, 9.5, 7.1, 7.7, 1.5, 6.0, 2.2, 9.5, 3.4, 2.9, 6.8, 3.5, 9.8, 9.8, 6.9, 9.2, 2.0, 3.6, 5.2, 6.6, 8.3, 2.1, 7.0, 3.7, 6.8, 9.0, 2.7, 9.3, 4.5, 6.1, 2.0, 6.3, 7.7, 3.7, 9.7, 5.8, 3.4, 1.7, 6.6, 5.8, 8.3, 9.2, 6.9, 5.5, 3.1, 7.0, 7.9, 3.6, 5.0, 7.5, 6.9, 1.3, 5.9, 9.5, 9.1, 5.4, 7.7, 7.1, 2.7, 4.1, 6.5, 3.9, 8.5, 9.4, 7.4, 1.4, 10.0, 9.0, 2.2, 1.7, 2.3, 1.6, 9.5, 7.3, 1.7, 8.5, 8.2, 1.2, 7.9, 8.8, 2.0, 1.7, 4.5, 7.0, 5.2, 7.2, 8.4, 2.4, 4.9, 6.3, 4.1, 9.1, 1.7, 3.1, 7.7, 1.9, 5.1, 4.7, 4.1, 1.3, 9.4, 3.6, 8.1, 4.4, 5.1, 9.6, 2.1, 3.0, 6.9, 3.7, 9.4, 2.8, 3.1, 2.1, 7.0, 5.7, 9.3, 4.2, 1.7, 8.3, 3.0, 9.5, 10.0, 9.7, 9.7, 8.9, 2.4, 8.6, 8.7, 7.1, 1.1, 3.7, 2.1, 9.1, 9.1, 6.0, 7.4, 8.7, 3.1, 9.5, 1.1, 1.8, 4.6, 2.7, 2.0, 1.6, 2.5, 6.8, 10.0, 6.7 } },
                {"engine-rpm",          new double[]{ 4.3, 5.7, 4.8, 9.3, 4.2, 1.2, 9.6, 7.7, 9.4, 1.7, 1.8, 3.3, 9.9, 6.2, 3.0, 8.6, 8.8, 7.2, 8.9, 7.5, 5.3, 1.2, 4.7, 8.5, 9.4, 4.1, 4.0, 5.7, 5.0, 7.4, 5.6, 7.9, 4.7, 2.8, 8.1, 9.1, 9.9, 1.1, 6.1, 6.9, 5.6, 2.8, 7.4, 1.5, 2.3, 6.5, 8.7, 2.6, 1.3, 3.9, 2.5, 8.7, 3.8, 9.9, 5.1, 6.4, 8.7, 3.5, 2.8, 5.6, 8.2, 1.3, 8.5, 1.7, 2.5, 1.5, 4.5, 9.8, 1.9, 9.3, 5.5, 4.1, 3.3, 5.5, 2.2, 2.5, 3.8, 7.0, 1.5, 6.4, 4.3, 2.9, 2.6, 6.2, 2.8, 9.9, 7.1, 9.8, 7.8, 6.1, 6.9, 7.8, 5.9, 4.4, 6.5, 1.3, 7.9, 6.5, 9.9, 9.6, 8.9, 9.5, 7.1, 7.7, 1.5, 6.0, 2.2, 9.5, 3.4, 2.9, 6.8, 3.5, 9.8, 9.8, 6.9, 9.2, 2.0, 3.6, 5.2, 6.6, 8.3, 2.1, 7.0, 3.7, 6.8, 9.0, 2.7, 9.3, 4.5, 6.1, 2.0, 6.3, 7.7, 3.7, 9.7, 5.8, 3.4, 1.7, 6.6, 5.8, 8.3, 9.2, 6.9, 5.5, 3.1, 7.0, 7.9, 3.6, 5.0, 7.5, 6.9, 1.3, 5.9, 9.5, 9.1, 5.4, 7.7, 7.1, 2.7, 4.1, 6.5, 3.9, 8.5, 9.4, 7.4, 1.4, 10.0, 9.0, 2.2, 1.7, 2.3, 1.6, 9.5, 7.3, 1.7, 8.5, 8.2, 1.2, 7.9, 8.8, 2.0, 1.7, 4.5, 7.0, 5.2, 7.2, 8.4, 2.4, 4.9, 6.3, 4.1, 9.1, 1.7, 3.1, 7.7, 1.9, 5.1, 4.7, 4.1, 1.3, 9.4, 3.6, 8.1, 4.4, 5.1, 9.6, 2.1, 3.0, 6.9, 3.7, 9.4, 2.8, 3.1, 2.1, 7.0, 5.7, 9.3, 4.2, 1.7, 8.3, 3.0, 9.5, 10.0, 9.7, 9.7, 8.9, 2.4, 8.6, 8.7, 7.1, 1.1, 3.7, 2.1, 9.1, 9.1, 6.0, 7.4, 8.7, 3.1, 9.5, 1.1, 1.8, 4.6, 2.7, 2.0, 1.6, 2.5, 6.8, 10.0, 6.7 } },
                {"temp",                new double[]{ 9, 8, 3, 7, 4, 4, 3, 5, 4, 2, 8, 1, 6, 1, 8, 4, 2, 4, 3, 9, 8, 4, 8, 2, 9, 7, 6, 8, 7, 7, 6, 3, 5, 6, 8, 6, 5, 6, 10, 10, 5, 3, 4, 10, 5, 3, 4, 5, 6, 7, 9, 8, 3, 6, 9, 4, 3, 1, 8, 3, 1, 1, 6, 2, 8, 1, 7, 6, 1, 8, 9, 1, 4, 3, 4, 1, 9, 6, 3, 5, 7, 8, 4, 7, 10, 2, 7, 10, 8, 4, 2, 9, 5, 10, 5, 7, 1, 1, 3, 1, 3, 1, 1, 2, 2, 8, 10, 6, 4, 1, 6, 5, 6, 4, 4, 3, 9, 2, 9, 9, 2, 9, 2, 1, 2, 6, 9, 2, 10, 3, 6, 3, 2, 3, 2, 2, 10, 4, 7, 5, 2, 4, 7, 4, 1, 2, 9, 10, 8, 1, 7, 2, 8, 9, 5, 4, 10, 5, 6, 5, 8, 9, 9, 3, 3, 7, 3, 2, 7, 6, 5, 3, 7, 1, 5, 2, 1, 5, 9, 2, 3, 6, 8, 4, 5, 7, 7, 10, 6, 2, 5, 5, 7, 7, 3, 2, 3, 6, 1, 10, 2, 8, 7, 1, 2, 4, 5, 9, 2, 3, 9, 4, 2, 6, 5, 8, 8, 1, 10, 1, 9, 1, 2, 6, 1, 2, 5, 5, 5, 4, 4, 7, 3, 3, 5, 4, 4, 3, 2, 9, 7, 5, 3, 2, 10, 5, 4, 3, 7, 9 } },
                {"clock",               new double[]{ 3, 5, 7, 8, 6, 3, 5, 10, 6, 1, 10, 7, 10, 5, 3, 2, 7, 5, 9, 1, 1, 3, 7, 9, 9, 5, 6, 6, 6, 7, 1, 1, 5, 9, 5, 5, 7, 9, 6, 6, 6, 5, 8, 7, 10, 6, 3, 4, 3, 5, 5, 2, 9, 9, 6, 7, 3, 9, 8, 5, 5, 10, 9, 3, 1, 5, 9, 9, 5, 7, 3, 6, 3, 7, 2, 4, 7, 3, 3, 4, 2, 3, 5, 6, 4, 8, 3, 10, 1, 2, 4, 3, 3, 9, 8, 10, 9, 7, 9, 1, 2, 3, 9, 5, 1, 4, 3, 6, 3, 8, 9, 8, 1, 3, 3, 7, 6, 2, 6, 1, 7, 5, 8, 1, 3, 1, 4, 4, 2, 1, 2, 3, 9, 10, 3, 2, 7, 4, 6, 4, 10, 8, 10, 10, 10, 7, 5, 5, 6, 7, 10, 9, 6, 8, 7, 8, 4, 10, 4, 5, 7, 6, 7, 10, 7, 9, 4, 3, 3, 5, 8, 10, 5, 1, 4, 1, 5, 6, 3, 4, 10, 8, 3, 9, 1, 6, 1, 9, 1, 6, 10, 1, 10, 2, 7, 7, 3, 9, 5, 4, 2, 3, 4, 8, 2, 2, 2, 4, 8, 1, 7, 4, 6, 1, 10, 6, 4, 9, 3, 6, 10, 4, 3, 2, 5, 10, 3, 5, 10, 7, 8, 2, 6, 6, 8, 5, 3, 3, 2, 8, 7, 10, 10, 7, 5, 5, 1, 9, 7, 8 } },
                {"test",                new double[]{ 5, 8, 7, 1, 4, 2, 8, 4, 5, 6, 9, 10, 10, 4, 8, 6, 10, 7, 4, 5, 1, 3, 6, 1, 6, 1, 8, 2, 3, 1, 5, 6, 5, 1, 1, 5, 6, 10, 7, 10, 2, 9, 8, 8, 7, 5, 9, 4, 6, 4, 7, 5, 7, 9, 9, 8, 2, 2, 9, 4, 9, 7, 1, 3, 8, 2, 7, 5, 5, 9, 5, 4, 10, 8, 1, 5, 8, 10, 9, 7, 4, 4, 2, 3, 8, 1, 2, 1, 3, 7, 4, 9, 3, 1, 9, 7, 5, 2, 6, 5, 7, 10, 5, 2, 5, 9, 8, 8, 10, 1, 1, 3, 10, 5, 4, 10, 5, 7, 10, 5, 9, 7, 8, 10, 10, 4, 8, 6, 7, 4, 3, 4, 6, 7, 2, 5, 4, 2, 5, 4, 6, 4, 8, 7, 5, 1, 1, 2, 1, 1, 7, 7, 4, 2, 6, 6, 1, 7, 1, 7, 2, 4, 9, 10, 7, 5, 8, 7, 1, 9, 4, 8, 7, 4, 6, 1, 2, 5, 6, 6, 5, 1, 9, 4, 6, 9, 3, 10, 2, 3, 8, 4, 6, 5, 4, 7, 9, 2, 3, 7, 3, 9, 5, 4, 4, 7, 4, 10, 7, 6, 5, 2, 2, 10, 8, 9, 1, 2, 6, 5, 2, 2, 8, 8, 4, 9, 1, 3, 4, 4, 2, 3, 5, 10, 6, 3, 2, 2, 6, 6, 5, 9, 3, 3, 9, 4, 5, 2, 2, 5 } },
                {"azimoth",             new double[]{ -0.32, -0.48, 0.71, -0.21, -0.20, -0.62, -0.10, -0.92, 0.79, -0.12, 0.11, 0.59, -0.65, 0.55, 0.96, -0.67, 0.44, 0.74, 0.07, 0.39, -0.59, 0.88, -0.23, 0.02, 0.52, -0.19, 0.66, -0.63, 0.62, -0.27, -0.30, 0.97, 0.10, -0.57, -0.46, -0.40, 0.52, -0.59, -0.63, -0.59, -0.83, 0.90, -0.22, -0.34, 0.90, -0.95, 0.02, 0.59, 0.20, -0.64, -0.75, 0.29, 0.14, 0.96, -0.05, 0.84, 0.02, 0.01, 0.58, -0.89, -0.37, 0.43, 0.59, -0.68, 0.05, 0.64, -0.15, -0.90, -0.25, -0.91, -0.70, -0.83, 0.72, -0.81, -0.79, 0.29, -0.08, 0.96, -0.01, -0.65, -0.83, 0.08, 0.36, -0.86, -0.80, 0.85, -0.48, 0.78, 0.64, -0.99, -0.71, -0.22, 0.78, -0.45, 0.93, -0.93, 0.52, -0.79, -0.77, 0.65, 0.19, -0.61, 0.07, -0.19, 0.23, -0.51, 0.41, 0.21, -0.32, 0.71, 0.85, 0.53, 0.59, -0.40, -0.86, 0.59, 0.71, 0.00, 0.89, 0.62, -0.65, -0.59, 0.61, 0.38, -0.93, 0.53, -0.31, -0.74, -0.61, -0.38, -0.64, 0.50, 0.26, -0.24, 0.88, 0.94, 0.05, -0.45, -0.84, 0.07, -0.51, -0.50, 0.61, -0.48, -0.68, 0.95, 0.23, 0.92, -0.47, 0.44, 0.12, -0.50, 0.16, -0.63, -0.13, 0.88, -0.17, 0.88, 0.34, 0.20, 0.88, -0.77, 0.48, 0.95, 0.26, 0.11, -0.35, 0.84, 0.07, 0.08, -0.97, 0.61, 0.96, -0.96, 0.02, 0.37, -0.45, 0.14, 0.66, 0.28, -0.81, -0.10, -0.57, 0.07, -0.71, -0.43, -0.07, 0.54, 0.33, -0.36, -0.47, 0.82, 0.09, -0.15, 0.04, -0.76, 0.94, 0.47, 0.98, -0.06, 0.28, -0.04, 0.76, 0.86, 0.42, -0.94, 0.81, -0.22, -0.95, -0.65, 0.81, -0.32, 0.33, 0.66, 0.20, -0.03, 0.98, -0.64, 0.26, -0.30, 0.79, -0.37, 0.12, 0.30, -0.65, -0.28, 0.83, 0.44, -0.74, 0.05, -0.14, 0.67, 0.98, -0.78, 0.84, 0.49, 0.66, 0.19, 0.39, -0.99, -0.53, 0.58, -0.42, 0.41, 0.69, -0.00, 0.02, -0.43, 0.15, -0.63 } },
                {"lift",                new double[]{ -0.93, 0.24, 0.94, -0.12, 0.68, 0.32, 0.44, 0.47, 0.77, 0.46, 0.80, -0.01, 0.10, -0.24, -0.20, 0.01, -0.58, 0.16, 0.46, -0.19, 0.77, 0.06, 0.66, 0.68, 0.76, -0.83, -0.52, 0.40, 0.90, 0.89, 0.36, -0.06, -0.48, -0.95, 0.33, 0.69, 0.01, -0.33, 0.75, 0.82, -0.91, -0.50, 0.02, -0.85, -0.65, 0.03, -0.58, -0.09, -0.57, 0.90, 0.13, 0.85, -0.69, 0.26, 0.00, 0.01, 0.95, -0.14, -0.43, -0.36, -0.99, -0.15, -0.31, -0.98, -0.06, 0.96, 0.74, -0.36, 0.53, 0.31, 0.10, 0.85, -0.30, -0.08, -0.20, -0.60, -0.36, -0.80, -0.44, 0.15, -0.09, 0.68, -0.50, 0.29, 0.42, -0.50, 0.12, -0.07, -0.13, 0.26, 0.18, 0.84, 0.58, -0.16, -0.48, -0.29, -0.18, 0.51, -0.52, 0.31, 0.60, 0.06, -0.02, 0.04, -0.29, 0.90, 0.37, 0.68, 0.59, -0.64, -0.95, 0.81, -0.67, -0.42, 0.04, -0.72, 0.04, -0.80, -0.71, -0.87, 0.60, 0.82, 0.18, -0.19, -0.79, 0.26, -0.34, 0.08, -0.36, -0.52, 0.11, -0.78, 0.18, 0.05, -0.76, 0.30, 0.96, -0.89, -0.11, 0.87, -0.14, 0.97, -0.85, 0.43, -0.55, 0.97, -0.32, -0.25, 0.14, -0.45, 0.56, -0.14, 0.85, 0.66, -0.21, 0.25, -0.56, 0.10, 0.49, 0.22, 0.92, 0.01, -0.21, -0.87, 0.59, 0.28, 0.41, -0.63, -0.28, -0.43, 0.28, -0.70, -0.89, 0.28, 0.17, -0.52, 0.71, 0.58, 0.31, 0.20, 0.35, 0.64, -0.17, -0.20, 0.94, -0.46, -0.66, 0.99, -1.00, 0.89, 1.00, 0.99, -0.19, 0.54, 0.41, -0.62, -0.36, 0.46, -0.20, 0.90, 0.18, -0.80, 0.72, 0.44, -0.95, 0.78, -0.73, -0.48, -0.16, 0.40, -0.98, 0.11, 0.93, -0.37, 0.43, 0.64, -0.19, -0.62, 0.29, -0.12, 0.69, -0.84, -0.20, 0.42, 0.20, 0.80, -0.36, -0.88, -0.87, -0.23, -0.47, -0.53, -0.14, 0.44, -0.01, -0.74, 0.38, 0.69, -0.09, -0.32, -0.54, -0.09, 0.15, 0.90, 0.43, -0.07, 0.84, 0.81, 0.37, -0.73 } },
                {"temp1",               new double[]{ 7, 6, 10, 2, 8, 2, 1, 7, 3, 9, 8, 9, 7, 4, 5, 7, 7, 9, 2, 5, 7, 1, 4, 8, 4, 4, 8, 1, 6, 8, 8, 10, 5, 8, 9, 5, 8, 9, 1, 10, 7, 1, 9, 2, 3, 4, 7, 1, 8, 6, 5, 2, 2, 6, 7, 1, 6, 8, 9, 5, 7, 1, 2, 2, 10, 1, 1, 7, 2, 5, 10, 8, 9, 3, 8, 8, 10, 10, 2, 9, 2, 5, 9, 7, 5, 8, 1, 10, 7, 8, 2, 2, 4, 7, 6, 9, 1, 1, 1, 9, 6, 4, 5, 9, 4, 10, 10, 9, 7, 6, 7, 1, 8, 10, 4, 3, 2, 5, 6, 4, 6, 2, 2, 8, 7, 1, 4, 4, 5, 7, 10, 5, 10, 10, 8, 7, 10, 8, 5, 4, 7, 4, 1, 6, 10, 3, 4, 5, 7, 6, 10, 6, 3, 3, 2, 8, 4, 9, 9, 5, 9, 7, 9, 6, 4, 10, 7, 7, 2, 7, 2, 8, 1, 3, 7, 1, 1, 3, 2, 5, 2, 3, 5, 7, 4, 6, 10, 6, 4, 9, 9, 2, 3, 7, 3, 8, 5, 8, 3, 8, 9, 5, 8, 2, 8, 4, 9, 9, 7, 6, 10, 10, 4, 6, 6, 10, 4, 4, 3, 6, 4, 1, 7, 2, 5, 7, 5, 2, 1, 5, 9, 2, 10, 6, 2, 4, 1, 8, 3, 5, 2, 2, 6, 10, 1, 1, 2, 9, 5, 4 } },
                {"wind",                new double[]{ 6, 6, 4, 5, 8, 1, 5, 5, 3, 4, 3, 4, 1, 4, 7, 9, 5, 3, 6, 2, 1, 9, 10, 10, 1, 6, 7, 7, 2, 3, 3, 5, 8, 10, 3, 7, 10, 4, 7, 5, 9, 8, 9, 10, 6, 5, 5, 2, 4, 9, 10, 3, 9, 2, 8, 1, 3, 10, 9, 2, 3, 4, 7, 3, 10, 9, 7, 8, 10, 5, 1, 1, 2, 9, 4, 6, 5, 5, 5, 8, 6, 3, 2, 9, 4, 4, 4, 2, 7, 7, 7, 9, 1, 7, 3, 1, 4, 5, 10, 6, 3, 10, 8, 6, 10, 5, 10, 6, 10, 3, 3, 1, 8, 8, 10, 9, 1, 7, 9, 4, 8, 3, 7, 9, 9, 3, 9, 5, 6, 6, 10, 7, 5, 7, 1, 4, 1, 4, 7, 1, 2, 9, 5, 1, 5, 5, 9, 8, 1, 10, 1, 3, 2, 6, 1, 5, 1, 4, 5, 9, 2, 2, 5, 4, 2, 10, 8, 5, 2, 6, 3, 6, 8, 3, 4, 8, 10, 8, 3, 8, 9, 4, 8, 8, 3, 7, 6, 7, 5, 3, 8, 7, 2, 4, 3, 5, 9, 10, 8, 6, 1, 1, 1, 5, 2, 7, 8, 6, 8, 1, 5, 3, 2, 9, 3, 1, 8, 4, 2, 6, 8, 3, 5, 8, 3, 6, 1, 4, 1, 6, 6, 3, 5, 10, 8, 10, 6, 1, 3, 4, 10, 7, 4, 2, 5, 10, 4, 7, 4, 3 } },
                {"coords",              new double[]{ 9.7, 5.1, 4.8, 7.2, 3.8, 5.6, 9.7, 8.0, 2.0, 2.9, 7.7, 2.2, 4.6, 5.0, 8.5, 7.2, 3.3, 3.8, 3.8, 9.0, 1.9, 8.7, 6.5, 4.2, 6.5, 6.8, 2.3, 2.3, 5.5, 4.8, 5.9, 1.9, 6.2, 4.6, 8.1, 6.1, 2.0, 3.0, 4.8, 3.6, 4.7, 7.2, 3.7, 2.5, 5.6, 4.0, 3.0, 9.8, 5.4, 3.2, 5.4, 9.3, 4.2, 6.4, 5.6, 6.7, 9.3, 4.8, 7.4, 1.4, 8.3, 6.8, 5.5, 4.0, 2.2, 4.8, 5.6, 8.1, 9.5, 9.9, 4.3, 8.9, 6.9, 2.4, 2.0, 2.8, 3.9, 3.6, 7.9, 7.7, 1.4, 1.8, 8.3, 5.5, 3.0, 8.9, 7.9, 6.4, 1.2, 4.6, 8.6, 3.7, 4.9, 4.1, 4.8, 7.7, 1.5, 4.0, 2.8, 4.2, 2.5, 2.1, 4.4, 6.5, 5.2, 1.8, 3.5, 8.1, 8.2, 4.3, 8.4, 7.3, 4.4, 9.8, 2.8, 3.9, 3.2, 1.3, 5.0, 6.8, 1.8, 6.8, 6.5, 7.9, 2.5, 2.6, 2.6, 8.0, 1.8, 7.7, 3.7, 6.1, 7.1, 3.2, 5.8, 3.8, 7.4, 2.3, 9.6, 1.7, 3.4, 5.9, 9.0, 1.3, 6.0, 6.0, 5.3, 3.4, 4.1, 9.7, 3.9, 10.0, 7.6, 2.2, 4.6, 9.6, 2.8, 9.9, 4.4, 9.2, 5.1, 8.4, 2.5, 3.7, 3.9, 2.2, 6.7, 5.5, 7.8, 6.6, 8.7, 1.5, 1.8, 3.8, 6.8, 3.2, 1.9, 4.1, 7.8, 1.7, 5.7, 3.0, 5.8, 2.0, 1.2, 7.0, 5.0, 3.7, 7.5, 9.1, 6.5, 8.7, 1.3, 6.0, 1.2, 6.6, 8.8, 2.1, 9.9, 7.8, 5.9, 4.5, 6.9, 3.3, 9.1, 1.9, 7.4, 4.2, 1.9, 7.4, 7.3, 8.2, 9.5, 9.7, 9.2, 9.8, 1.8, 7.7, 9.6, 9.3, 4.4, 5.6, 9.6, 6.9, 6.5, 9.5, 4.4, 6.2, 2.0, 3.1, 9.4, 7.7, 1.7, 1.9, 7.0, 4.1, 2.3, 4.9, 8.5, 2.0, 2.9, 2.8, 9.7, 8.3, 4.1, 5.6, 6.3, 5.6, 6.2, 2.7 } },
                {"hello world",         new double[]{ 3.5, 4.7, 6.6, 9.0, 6.8, 7.4, 1.1, 5.2, 9.0, 7.9, 8.0, 8.0, 2.8, 5.9, 2.8, 3.3, 4.7, 8.1, 1.0, 3.0, 2.8, 2.1, 1.7, 6.3, 5.5, 6.1, 2.8, 9.0, 3.3, 9.6, 9.5, 3.9, 6.6, 6.5, 2.4, 6.3, 3.1, 5.4, 6.8, 3.8, 6.9, 9.4, 5.0, 8.3, 7.5, 8.6, 2.5, 3.9, 9.2, 8.7, 9.8, 5.3, 5.8, 8.0, 2.0, 2.0, 3.9, 1.6, 8.9, 5.1, 5.1, 9.2, 7.7, 8.9, 7.6, 7.7, 7.6, 8.7, 7.4, 3.8, 6.9, 9.9, 1.7, 4.0, 7.7, 7.0, 4.7, 5.6, 5.8, 3.2, 7.7, 6.4, 2.0, 9.4, 3.4, 4.7, 4.4, 4.2, 6.3, 1.5, 4.9, 6.7, 4.3, 6.2, 8.5, 8.8, 7.3, 7.2, 7.3, 8.7, 9.8, 8.5, 8.0, 6.9, 3.4, 5.8, 7.3, 6.3, 2.8, 2.6, 3.2, 4.1, 4.4, 4.6, 9.2, 9.6, 2.3, 3.4, 3.2, 9.4, 9.9, 4.2, 5.2, 4.0, 2.2, 8.4, 5.1, 4.8, 5.6, 1.4, 5.1, 2.2, 9.2, 5.7, 10.0, 9.0, 2.8, 3.7, 9.9, 2.6, 4.5, 1.1, 2.5, 4.7, 6.6, 1.3, 9.4, 2.6, 3.7, 8.6, 2.2, 9.1, 7.1, 1.9, 6.9, 6.9, 9.0, 5.7, 3.8, 7.8, 1.9, 1.2, 5.8, 9.1, 3.6, 7.6, 5.3, 9.5, 9.6, 8.8, 7.4, 9.3, 3.2, 7.2, 1.8, 1.5, 2.6, 2.5, 9.7, 7.9, 6.2, 5.6, 8.7, 2.9, 4.4, 7.0, 8.7, 6.8, 4.4, 3.3, 2.1, 4.1, 7.0, 8.3, 1.6, 4.5, 6.6, 7.1, 1.6, 1.9, 3.7, 4.6, 7.1, 1.5, 5.9, 6.6, 8.3, 6.7, 9.4, 9.9, 2.4, 8.1, 9.1, 3.2, 6.6, 9.7, 8.7, 8.4, 8.8, 7.2, 2.8, 7.3, 6.8, 6.2, 8.3, 3.4, 5.4, 2.8, 3.1, 1.1, 3.8, 1.4, 4.6, 8.6, 4.2, 1.2, 5.1, 3.2, 5.7, 1.5, 3.1, 2.7, 7.8, 7.4, 8.3, 4.7, 3.2, 1.8, 3.3, 9.5 } },
                {"HELP ME",             new double[]{ 4.3, 5.7, 4.8, 9.3, 4.2, 1.2, 9.6, 7.7, 9.4, 1.7, 1.8, 3.3, 9.9, 6.2, 3.0, 8.6, 8.8, 7.2, 8.9, 7.5, 5.3, 1.2, 4.7, 8.5, 9.4, 4.1, 4.0, 5.7, 5.0, 7.4, 5.6, 7.9, 4.7, 2.8, 8.1, 9.1, 9.9, 1.1, 6.1, 6.9, 5.6, 2.8, 7.4, 1.5, 2.3, 6.5, 8.7, 2.6, 1.3, 3.9, 2.5, 8.7, 3.8, 9.9, 5.1, 6.4, 8.7, 3.5, 2.8, 5.6, 8.2, 1.3, 8.5, 1.7, 2.5, 1.5, 4.5, 9.8, 1.9, 9.3, 5.5, 4.1, 3.3, 5.5, 2.2, 2.5, 3.8, 7.0, 1.5, 6.4, 4.3, 2.9, 2.6, 6.2, 2.8, 9.9, 7.1, 9.8, 7.8, 6.1, 6.9, 7.8, 5.9, 4.4, 6.5, 1.3, 7.9, 6.5, 9.9, 9.6, 8.9, 9.5, 7.1, 7.7, 1.5, 6.0, 2.2, 9.5, 3.4, 2.9, 6.8, 3.5, 9.8, 9.8, 6.9, 9.2, 2.0, 3.6, 5.2, 6.6, 8.3, 2.1, 7.0, 3.7, 6.8, 9.0, 2.7, 9.3, 4.5, 6.1, 2.0, 6.3, 7.7, 3.7, 9.7, 5.8, 3.4, 1.7, 6.6, 5.8, 8.3, 9.2, 6.9, 5.5, 3.1, 7.0, 7.9, 3.6, 5.0, 7.5, 6.9, 1.3, 5.9, 9.5, 9.1, 5.4, 7.7, 7.1, 2.7, 4.1, 6.5, 3.9, 8.5, 9.4, 7.4, 1.4, 10.0, 9.0, 2.2, 1.7, 2.3, 1.6, 9.5, 7.3, 1.7, 8.5, 8.2, 1.2, 7.9, 8.8, 2.0, 1.7, 4.5, 7.0, 5.2, 7.2, 8.4, 2.4, 4.9, 6.3, 4.1, 9.1, 1.7, 3.1, 7.7, 1.9, 5.1, 4.7, 4.1, 1.3, 9.4, 3.6, 8.1, 4.4, 5.1, 9.6, 2.1, 3.0, 6.9, 3.7, 9.4, 2.8, 3.1, 2.1, 7.0, 5.7, 9.3, 4.2, 1.7, 8.3, 3.0, 9.5, 10.0, 9.7, 9.7, 8.9, 2.4, 8.6, 8.7, 7.1, 1.1, 3.7, 2.1, 9.1, 9.1, 6.0, 7.4, 8.7, 3.1, 9.5, 1.1, 1.8, 4.6, 2.7, 2.0, 1.6, 2.5, 6.8, 10.0, 6.7 } },
                {"SAD",                 new double[]{ 4.3, 5.7, 4.8, 9.3, 4.2, 1.2, 9.6, 7.7, 9.4, 1.7, 1.8, 3.3, 9.9, 6.2, 3.0, 8.6, 8.8, 7.2, 8.9, 7.5, 5.3, 1.2, 4.7, 8.5, 9.4, 4.1, 4.0, 5.7, 5.0, 7.4, 5.6, 7.9, 4.7, 2.8, 8.1, 9.1, 9.9, 1.1, 6.1, 6.9, 5.6, 2.8, 7.4, 1.5, 2.3, 6.5, 8.7, 2.6, 1.3, 3.9, 2.5, 8.7, 3.8, 9.9, 5.1, 6.4, 8.7, 3.5, 2.8, 5.6, 8.2, 1.3, 8.5, 1.7, 2.5, 1.5, 4.5, 9.8, 1.9, 9.3, 5.5, 4.1, 3.3, 5.5, 2.2, 2.5, 3.8, 7.0, 1.5, 6.4, 4.3, 2.9, 2.6, 6.2, 2.8, 9.9, 7.1, 9.8, 7.8, 6.1, 6.9, 7.8, 5.9, 4.4, 6.5, 1.3, 7.9, 6.5, 9.9, 9.6, 8.9, 9.5, 7.1, 7.7, 1.5, 6.0, 2.2, 9.5, 3.4, 2.9, 6.8, 3.5, 9.8, 9.8, 6.9, 9.2, 2.0, 3.6, 5.2, 6.6, 8.3, 2.1, 7.0, 3.7, 6.8, 9.0, 2.7, 9.3, 4.5, 6.1, 2.0, 6.3, 7.7, 3.7, 9.7, 5.8, 3.4, 1.7, 6.6, 5.8, 8.3, 9.2, 6.9, 5.5, 3.1, 7.0, 7.9, 3.6, 5.0, 7.5, 6.9, 1.3, 5.9, 9.5, 9.1, 5.4, 7.7, 7.1, 2.7, 4.1, 6.5, 3.9, 8.5, 9.4, 7.4, 1.4, 10.0, 9.0, 2.2, 1.7, 2.3, 1.6, 9.5, 7.3, 1.7, 8.5, 8.2, 1.2, 7.9, 8.8, 2.0, 1.7, 4.5, 7.0, 5.2, 7.2, 8.4, 2.4, 4.9, 6.3, 4.1, 9.1, 1.7, 3.1, 7.7, 1.9, 5.1, 4.7, 4.1, 1.3, 9.4, 3.6, 8.1, 4.4, 5.1, 9.6, 2.1, 3.0, 6.9, 3.7, 9.4, 2.8, 3.1, 2.1, 7.0, 5.7, 9.3, 4.2, 1.7, 8.3, 3.0, 9.5, 10.0, 9.7, 9.7, 8.9, 2.4, 8.6, 8.7, 7.1, 1.1, 3.7, 2.1, 9.1, 9.1, 6.0, 7.4, 8.7, 3.1, 9.5, 1.1, 1.8, 4.6, 2.7, 2.0, 1.6, 2.5, 6.8, 10.0, 6.7 } },
            };
            
            #endregion
        }
    }
}