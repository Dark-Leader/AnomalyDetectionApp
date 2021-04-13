using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.ComponentModel;
using System.Xml;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Timers;



namespace EX2
{
    public class FlightSimulator : IFlightSimulator
    {

        /// /////////////////////////////////external functions for TimeSeries//////////////////////////////

        /*TS*/

        [DllImport("LinearRegression.dll", CallingConvention = CallingConvention.Cdecl)] //Creating pointer to anomalies TimeSeries
        public static extern IntPtr Create_Regular_TS(String fileName, String[] atts, int size);

        [DllImport("LinearRegression.dll", CallingConvention = CallingConvention.Cdecl)] //creating a pointer for the RowSize
        public static extern IntPtr Extern_getRowSize(IntPtr ts);

        /*Data-Wrapper*/
        [DllImport("LinearRegression.dll", CallingConvention = CallingConvention.Cdecl)] //Creating a float Wrapper for a given string
        public static extern IntPtr CreateWrappedData(IntPtr ts, String s);
        //helper method
        [DllImport("LinearRegression.dll", CallingConvention = CallingConvention.Cdecl)] //given that wrapper, return it's size
        public static extern int Data_Wrapper_size(IntPtr DW);
        //helper method
        [DllImport("LinearRegression.dll", CallingConvention = CallingConvention.Cdecl)] //given that wrapper, get a value based on an index
        public static extern float Data_Wrapper_getter(IntPtr DW, int i);


        /*AnomalyDetector */
        [DllImport("LinearRegression.dll", CallingConvention = CallingConvention.Cdecl)] //Creates pointer to AnomalyDetector
        public static extern IntPtr Create_SimpleAnomalyDetector();
        /*LearnNormal with that Detector
         writes into a txt file called 'Correlated', the correlated features
        API- "feature1" "feature2*/

        [DllImport("LinearRegression.dll", CallingConvention = CallingConvention.Cdecl)] 
        public static extern void LearnNormal(IntPtr AD, IntPtr TS);
        /*Detect using that Detector
        writes into a txt file called 'Anomalies', the anomalies from the flight
        API- "feature1" "feature2" "timestamp"*/

        [DllImport("LinearRegression.dll", CallingConvention = CallingConvention.Cdecl)] 
        public static extern void Detect(IntPtr AD, IntPtr TS);



        ///////////////////////////////////////////real content of class////////////////////////////////////

        private string[] attributesArray;

        //TimeSeries for the regular flight
        private IntPtr TS_regFlight;

        //TimeSeries for the anomaly flight
        private IntPtr TS_anomalyFlight;

        //AnomalyDetector
        private IntPtr AnomalyDetector;

        // Will hold all the data of the regular flight csv. attributes are the keys
        private Dictionary<string, List<float>> regFlightDict;

        // Will hold all the data of the anomaly flight csv. attributes are the keys
        private Dictionary<string, List<float>> anomalyFlightDict;


        // List of attributes as read from XML
        List<string> attributesList;

        IntPtr DW;
        private string FGPath;

        // how many lines are there in the received flight data csv
        private int totalLines;

        private List<KeyValuePair<float, float>> selectedFeature = new List<KeyValuePair<float, float>>();
        private List<KeyValuePair<float, float>> correlatedFeature;
        private List<string> variables = new List<string>();
        private int playbackSpeed = 10;

        private TimeSpan time;

        private System.Timers.Timer playTimer = new System.Timers.Timer();

        // A time period expressed in milliSeconds units used for playingThread.sleep(ticks)
        private int ticks;

        Thread playingThread;

        private bool stop;
        private bool pause;

        private FlightGear fg;

        // socket that is connected to the application
        private Client client;

        // This is a temp feauture untill we have the TS. Holds the data in CSV 
        private List<string> dataByLines = new List<string>();

        // The last line sent to fg
        private int currentLinePlaying;

        public event PropertyChangedEventHandler PropertyChanged;


        private string pathToXML;

        private string csvData;

        public FlightSimulator()
        {

            pathToXML = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            pathToXML = Directory.GetParent(pathToXML).FullName;
            pathToXML = Directory.GetParent(pathToXML).FullName;
            pathToXML += "\\resources\\playback_small.xml";
            parseXML();

            playbackSpeed = 10;
            stop = false;
            pause = false;
            // starting to play the data 10 lines in a second
            ticks = 100;
            playTimer.Interval = 100;
            currentLinePlaying = 0;

            client = new Client();


            //test code for AnomalyDetector

            //paths
            //String Reg_ts_path = "C:\\Users\\USER\\Desktop\\reg_flight.csv"; //with NO features(for beggining of programm)
            //String Ano_ts_path = "C:\\Users\\USER\\Desktop\\anomaly_flight.csv";

            //part A- learn according to reg CSV file
            //IntPtr Regular_TS = Create_Regular_TS(Reg_ts_path, attributes, attributes.Length);// time-series, created by XML
            ////List<float> list = getVectorByName(Regular_TS, "aileron");
            //////test
            ////foreach(var x in list)
            ////{
            ////    Console.WriteLine(x);
            ////}

            //IntPtr AW = CreateWrappedAttributes(Regular_TS);
            //Console.WriteLine(Attributes_Wrapper_size(AW));

            //IntPtr AD = Create_SimpleAnomalyDetector(); //AnomalyDetector
            //LearnNormal(AD, Regular_TS); //test - LearnNormal by the initial CSV file


            //Part B- Detect anomalies based on anomalies CSV file
            //IntPtr Anomalies_TS = Create_Regular_TS(Ano_ts_path, attributes, attributes.Length);
            //Detect(AD, Anomalies_TS);

            playTimer.Elapsed += (s, e) =>
            {
                if (playTimer.Enabled)
                {
                    OnTimedEvent(s, e);
                }
            };
        }


        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public Dictionary<string, List<float>> RegFlightDict {
            get
            {
                return regFlightDict;
            }
        }

        public Dictionary<string, List<float>> AnomalyFlightDict
        {
            get
            {
                return anomalyFlightDict;
            }
        }

        public int Playback_speed
        {
            get
            {
                return this.playbackSpeed;
            }
            set
            {
                if (value != playbackSpeed)
                {
                    if (value < 1)
                    {
                        // The minimal playbackSpeed is 1 row per second
                        this.playbackSpeed = 1;
                    }
                    else if (30 < value )
                    {
                        // The maximal playbackSpeed is 30 row per second
                        this.playbackSpeed = 30;
                    }
                    else
                    {
                        // set it to the given value
                        this.playbackSpeed = value;
                    }

                    // calculate the new ticks
                    this.ticks = 1000 / playbackSpeed;
                    playTimer.Interval = this.ticks;
                    NotifyPropertyChanged("Playback_speed");
                }
            }
        }

        /*
        public int CurrentLinePlaying { get; private set; }
        */

        public List<string> AttributesList
        {
            get
            {
                return this.attributesList;
            }
        }

        /*FvectorToList func.
         creates a list out of a given float-vector wrapper*/
        static public List<float> FvectorToList(IntPtr DW)
        {
            List<float> list = new List<float>();
            int size = Data_Wrapper_size(DW);
            for (int i = 0; i < size; i++)
            {
                list.Add(Data_Wrapper_getter(DW, i));
            }
            return list;
        }


        /// <summary>
        /// getVectorByName func. given a feeature, return it's values 
        /// </summary>
        /// <param name="TS"></pointer to TimeSeries>
        /// <param name="name"><name of a feature>
        /// <returns></returns>
        static public List<float> getVectorByName(IntPtr TS, String name)
        {
            IntPtr DW = CreateWrappedData(TS, name); //create wrapper
            return FvectorToList(DW); //create a vector with it and send it away
        }

        
        public TimeSpan Time
        {
            get
            {
                return time;
            }
            private set
            {
                time = value;
                NotifyPropertyChanged("Time");
            }
        }

        public List<KeyValuePair<float, float>> CorrelatedFeature
        {
            get
            {
                return correlatedFeature;
            }
            set
            {
                if (value != selectedFeature)
                {
                    correlatedFeature = value;
                    //notifyPropertyChanged("CorrelatedFeature");
                }
            }
        }
        /// <summary>
        /// Happens each time the playTimer "ticks" 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {

            Time = TimeSpan.FromMilliseconds(currentLinePlaying * 100);
            Console.WriteLine(this.time);
        }

        /// <summary>
        /// Returns an array of the last 10 values of the specified feauture
        /// </summary>
        /// <param name="feauture">The feature to get data of</param>
        /// <returns>An array of floats in size 10 or less</returns>
        public float[] GetDataOfTheLastSecondByFeature(string feauture)
        {
            if (anomalyFlightDict != null)
            {
                int lastData = currentLinePlaying - 10;
                if (lastData > 0)
                {
                    return anomalyFlightDict[feauture].GetRange(lastData, 10).ToArray();
                }
                return anomalyFlightDict[feauture].GetRange(0, currentLinePlaying).ToArray();
            }
            return new float[0];
        }

        public float GetLastDataOfFeature(string feauture)
        {
            if (anomalyFlightDict != null)
            {
                return anomalyFlightDict[feauture][currentLinePlaying];
            }
            return 0;
        }

        public void StopPlayback()
        {
            stop = true;
            playTimer.Stop();
            currentLinePlaying = 0;
        }

        public void PausePlayback()
        {
            pause = true;
            this.ticks = Timeout.Infinite;
            playTimer.Stop();
        }

        // Holds the path to regular flight CSV file
        private string regFlightCSV;

        public string RegFlightCSV
        {
            get
            {
                return regFlightCSV;
            }
            set
            {
                if (this.regFlightCSV != value)
                {
                    this.regFlightCSV = value;
                    TS_regFlight = Create_Regular_TS(this.regFlightCSV, attributesArray, attributesArray.Length);
                    regFlightDict = new Dictionary<string, List<float>>();
                    foreach (var item in attributesArray)
                    {
                        regFlightDict.Add(item, getVectorByName(TS_regFlight, item));
                    }
                    //readCSV(this.regFlightCSV);
                }
            }
        }

        // Holds the path to regular flight CSV file
        private String anomalyFlightCSV;

        public string AnomalyFlightCSV
        {
            get
            {
                return anomalyFlightCSV;
            }
            set
            {
                if (this.anomalyFlightCSV != value)
                {
                    this.anomalyFlightCSV = value;

                    TS_anomalyFlight = Create_Regular_TS(this.anomalyFlightCSV, attributesArray, attributesArray.Length);
                    anomalyFlightDict = new Dictionary<string, List<float>>();
                    foreach (var item in attributesArray)
                    {
                        anomalyFlightDict.Add(item, getVectorByName(TS_anomalyFlight, item));
                    }
                }
            }
        }

        /*
        public void setCSVFile(string name)
        {
            this.csvData = name;
            readCSV(this.csvData);
        }
        */
        public void setFGPath(string name)
        {
            this.FGPath = name;
            this.fg = new FlightGear(name, pathToXML);
            this.fg.openApplication();
            
        }

        public void play()
        {
            Console.WriteLine("in the play method in fs");
            if (!pause && !stop)
            {
                // meaning it's the 1st time we play data to fg
                // so we need to establish connection
                //this.client.connect("127.0.0.1", 5400);
                Console.WriteLine("Connected");
            }
            else if (pause)
            {
                // Meaning the playingThread is sleeping infinite time and we need to resume it
                pause = false;
                stop = false;
                this.ticks = 1000 / playbackSpeed;
                this.playingThread.Interrupt();
                playTimer.Start();
                return;
            }
            else if (stop)
            {
                Console.WriteLine("in the play method in the if stop = true");
                stop = false;
                pause = false;
            }

            playTimer.Start();
            //  we now initialize a new thread 
            this.playingThread = new Thread(new ThreadStart(this.playback));
            this.playingThread.Start();
        }

        /// <summary>
        /// This is the function that runs in a different thread and sends data to fg
        /// </summary>
        private void playback()
        {
            try
            {
                String line;
                while (!this.stop)
                {
                    try
                    {
                        /* This is working just want to test if the speed changes correctly
                        line = getLine(this.currentLinePlaying);
                        line += "\r\n";
                        this.client.write(line);
                        Console.WriteLine(line);
                        currentLinePlaying++;
                        Console.WriteLine("The line index current is:");
                        Console.WriteLine(currentLinePlaying);

                        Thread.Sleep(ticks);
                        */
                        line = getLine(this.currentLinePlaying);
                        line += "\r\n";
                                                
                        Thread.Sleep(ticks);

                        currentLinePlaying++;
                        if (currentLinePlaying == dataByLines.Count)
                        {
                            StopPlayback();
                        }
                    }
                    catch (ThreadInterruptedException e)
                    {
                        // meaning the thread noe needs to continue after it's been paused
                        continue;
                    }
                }
                Console.WriteLine("The thread has stopped!");
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        /// <summary>
        /// Temp function untill we have ts, gets the specific line 
        /// </summary>
        /// <param name="lineNumber">the line number to get</param>
        /// <returns></returns>
        public String getLine(int lineNumber)
        {
            if (lineNumber < this.dataByLines.Count)
            {
                return this.dataByLines[lineNumber];
            }
            return "error in getLine";
        }

        /// <summary>
        /// Returns the number of samples received in the CSV aka - number of lines
        /// </summary>
        /// <returns>Number of samples in the CSV</returns>
        public int NumberOfFlighSamples()
        {
            return dataByLines.Count;
        }

        /// <summary>
        /// Temp function untill we have ts, gets the specific line 
        /// </summary>
        /// <param name="csvPath"></param>
        private void readCSV(string path)
        {
            Console.WriteLine("readsthe CSV and saves lines");
            using (StreamReader rd = new StreamReader(path))
            {
                String line;

                while ((line = rd.ReadLine()) != null)
                {
                    this.dataByLines.Add(line);
                }
            }
        }

        /// <summary>
        /// This methods gets the feautures stated in the XML file 
        /// </summary>
        public void parseXML()
        {
            attributesList = new List<string>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            XmlReader reader = null;

            /*
               string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
               filePath = Directory.GetParent(filePath).FullName;
               filePath = Directory.GetParent(filePath).FullName;
               reader = XmlReader.Create(filePath + "\\resources\\playback_small.xml", settings);
               */


            // TODO read from XML the speed and save it as a property. 
            string att;
            reader = XmlReader.Create(pathToXML, settings);
            if (reader.ReadToFollowing("output"))
            {
                reader.Read();
                while (reader.Name != "output")
                {
                    if (reader.IsStartElement())
                    {

                        if (reader.Name == "chunk")
                        {
                            reader.Read();
                            att = reader.ReadString();
                            if (attributesList.Contains(att) == true)
                            {
                                att += "1";
                            }
                            attributesList.Add(att);
                        }
                    }
                    reader.Read();
                }
            }

            
            attributesArray = attributesList.ToArray();
            if (reader != null)
                reader.Close();

        }
        /// <summary>
        /// User selected new variable to focus on.
        /// </summary>
        /// <param name="name"></param>
        public void variableSelected(string name)
        {
            Console.WriteLine(name);
            return;
        }
    }
}
