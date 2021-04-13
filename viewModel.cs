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
    public partial class ViewModel : INotifyPropertyChanged
    {

        private FlightSimulator sim;

        // Constructor
        public ViewModel(FlightSimulator sim)
        {
            //Test version of model. Just for testing
            this.sim = sim;
            
            this.sim.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                OnPropertyChanged("VM_" + e.PropertyName);
            };

            //Model = new FrameStack(sim);

            /*
            ChangeSpeedCommand = new RelayCommand((s) =>
            {
                var deltaSpeedMultiplier = Convert.ToDouble(s, System.Globalization.CultureInfo.InvariantCulture);
                //Change timer speed only on new frame start not to make "double resetting" if it is running now

                if (playTimer.Enabled)
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
                if (SelectedProperty == null)
                {
                    MessageBox.Show("Chose the property in the listbox to show");
                    return;
                }

                var newStateIsPlaying = Convert.ToBoolean(s);

                if (newStateIsPlaying)
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
            */
            //UpdateTimerInterval();
            
            /*
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
            */
        }


        //INotifyPropertyChanged 
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        System.Timers.Timer playTimer = new System.Timers.Timer();
        //responsible for the amount of lines read per second.
        private const int BUFFER_SIZE = 10;
        private const int BASE_FRAMERATE_PER_SECOND = 10;
        
        
        public List<string> VM_AttributesList
        {
            get
            {
                return sim.AttributesList;
                //return Model?.Properties ?? new List<string>();
            }
        }

        public int VM_SliderMax
        {
            get
            {
                //return (model?.Count - 1 - BUFFER_SIZE) ?? 0;
                return sim.SliderMax;
            }
            set 
            {
                this.sim.SliderMax = value;
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
            OnPropertyChanged(nameof(Roll)); // Lola - TODO ADD
        }

        //the current 10 lines or whatever value we chose.
        public float[] CurrentDataSet
        {
            get
            {
                try
                {
                    //return model[(string)SelectedProperty].Skip(currentFrame + 1).Take(BUFFER_SIZE).ToArray();
                    return sim.GetDataOfTheLastSecondByFeature((string)SelectedProperty);
                }
                catch
                {
                    return new float[0];
                }
            }
        }

        public double VM_Playback_speed
        {
            get 
            {
                return Convert.ToDouble(this.sim.Playback_speed) / 10; 
            }
        }

        public TimeSpan VM_Time
        {
            get
            {
                Console.WriteLine("in the VM_TIME in VM updated;");
                DataPropertyChanged();
                return this.sim.Time;

            }
        }

        #region BindedProperties
 //top first graph on the left which shows the information. based on user pick. 
        public DrawingImage TopGraphImageSource
        {
            get
            {
                var ds = CurrentDataSet;
                return new DrawingImage(GetGraphGroup(ds, false, 10, 10, ds.Length > 0 ? (int)Math.Ceiling(Math.Max(Math.Abs(CurrentDataSet.Min()), Math.Abs(CurrentDataSet.Max()))) : 1));
            }
        }

        //top second  graph on the left which shows the information. should be based on correlation.
        public DrawingImage TopGraphImageSource2
        {
            get
            {
                var ds = CurrentDataSet;

                return new DrawingImage(GetGraphGroup(ds, false, 10, 10, ds.Length > 0 ? (int)Math.Ceiling(Math.Max(Math.Abs(CurrentDataSet.Min()), Math.Abs(CurrentDataSet.Max()))) : 1));
            }
        }
        //bottom graph.
        public DrawingImage BottomGraphImageSource
        {
            get
            {
                var ds = CurrentDataSet;

                return new DrawingImage(GetGraphGroup(ds, true, 20, 10, ds.Length > 0 ? (int)Math.Ceiling(Math.Max(Math.Abs(CurrentDataSet.Min()), Math.Abs(CurrentDataSet.Max()))) : 1));
            }
        }

        //public double AileronProperty => model["aileron"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double AileronProperty => sim.GetLastDataOfFeature("aileron");
        //public double ElevatorPropery => model["elevator"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double ElevatorPropery => sim.GetLastDataOfFeature("elevator");
        //public double Throttle => model["throttle"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double Throttle => sim.GetLastDataOfFeature("throttle");

        //public double Rudder => model["rudder"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double Rudder => sim.GetLastDataOfFeature("rudder");
        public double RudderMin = 0;
        public double RudderMax = 1;


        //public double Altimeter => model["altimeter_indicated-altitude-ft"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double Altimeter => sim.GetLastDataOfFeature("altimeter_indicated-altitude-ft");
        public double AltimeterMin = -20;
        public double AltimeterMax = 700;

        //public double Airspeed => model["airspeed-indicator_indicated-speed-kt"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double Airspeed => sim.GetLastDataOfFeature("airspeed-indicator_indicated-speed-kt");
        public double AirspeedMin = -1;
        public double AirspeedMax => 100;

        //public double FlightDirection => model["indicated-heading-deg"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double FlightDirection => sim.GetLastDataOfFeature("indicated-heading-deg");
        public double FlightDirectionMin => 0;
        public double FlightDirectionMax => 370;

        //public double Pitch => model["pitch-deg"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double Pitch => sim.GetLastDataOfFeature("pitch-deg");
        public double PitchMin => -10;
        public double PitchMax => 20;

        //public double Yaw => model["side-slip-deg"].Skip(currentFrame + 1).Take(BUFFER_SIZE).First();
        public double Yaw => sim.GetLastDataOfFeature("side-slip-deg");
        public double YawMin => -30;
        public double YawMax => 90;

        public double Roll => sim.GetLastDataOfFeature("roll-deg");
        public double RollMin => -40;
        public double RollMax => 20;

        public int VM_CurrentLinePlaying
        {
            get
            {
                return sim.CurrentLinePlaying;
            }
            set
            {
                this.sim.CurrentLinePlaying = value;
            }
        }

        #endregion

        public RelayCommand StopCommand { get; private set; } 

        public RelayCommand PauseCommand { get; private set; } 

        public RelayCommand ChangeSpeedCommand { get; private set; }

        
        /*
        private void UpdateTimerInterval()
        {
            playTimer.Interval = 1000 / CurrentFrameRate;
        }*/

        private DrawingGroup GetGraphGroup(float[] Data, bool dots, int width = 10, int height = 10, int maximum = 5)
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
            VM_SliderMax = this.sim.RegFlightDict["elevator"].Count;
            Console.WriteLine(VM_SliderMax);
            Console.WriteLine(VM_CurrentLinePlaying);

        }

        public void set_test_csv(string name)
        {
             sim.AnomalyFlightCSV = name; //PRODUCES RUNTIME ERROR !!!
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
            sim.PausePlayback();
        }

        /// <summary>
        /// This button stops the playback and next time it will play from the start
        /// </summary>
        public void restart()
        {
            sim.StopPlayback();
        }

    }
}
