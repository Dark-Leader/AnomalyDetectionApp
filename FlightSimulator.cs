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

namespace EX2
{
    public class FlightSimulator : IFlightSimulator
    {

        /// /////////////////////////////////external functions for TimeSeries//////////////////////////////

        /*TS*/
        [DllImport("TS_DLL.dll", CallingConvention = CallingConvention.Cdecl)]//working
        public static extern IntPtr Create_Anomalies_TS(String fileName);

        [DllImport("TS_DLL.dll", CallingConvention = CallingConvention.Cdecl)] //working
        public static extern IntPtr Create_Regular_TS(String fileName, String[] atts, int size);

        [DllImport("TS_DLL.dll", CallingConvention = CallingConvention.Cdecl)] //working
        public static extern IntPtr Extern_getRowSize(IntPtr ts);

        /*Data-Wrapper*/
        [DllImport("TS_DLL.dll", CallingConvention = CallingConvention.Cdecl)] //working
        public static extern IntPtr CreateWrappedData(IntPtr ts, String s);

        [DllImport("TS_DLL.dll", CallingConvention = CallingConvention.Cdecl)] //working
        public static extern int Data_Wrapper_size(IntPtr DW);

        [DllImport("TS_DLL.dll", CallingConvention = CallingConvention.Cdecl)]//working
        public static extern float Data_Wrapper_getter(IntPtr DW, int i);


        /*Attributes-Wrapper*/
        [DllImport("TS_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateWrappedAttributes(IntPtr ts);

        [DllImport("TS_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Attributes_Wrapper_size(IntPtr AW);

        [DllImport("TS_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern String Attributes_Wrapper_getter(IntPtr AW);

        ///////////////////////////////////////////real content of class////////////////////////////////////
       
        private string[] attributes;
        private IntPtr TS; //TimeSeries
        IntPtr DW;
        private string FGPath;

        // how many lines are there in the received flight data csv
        private int totalLines;

        private List<KeyValuePair<float, float>> selectedFeature = new List<KeyValuePair<float, float>>();
        private List<KeyValuePair<float, float>> correlatedFeature;
        private List<string> variables = new List<string>();
        private string time = "00:00:00";
        private int playbackSpeed = 10;


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
            currentLinePlaying = 0;

            client = new Client();

            this.selectedFeature.Add(new KeyValuePair<float, float>(1, 60));
            this.selectedFeature.Add(new KeyValuePair<float, float>(7, 15));
            this.selectedFeature.Add(new KeyValuePair<float, float>(8, 23));
            this.selectedFeature.Add(new KeyValuePair<float, float>(40, 50));
            this.selectedFeature.Add(new KeyValuePair<float, float>(3, 80));
            this.selectedFeature.Add(new KeyValuePair<float, float>(11, 15));
            this.selectedFeature.Add(new KeyValuePair<float, float>(5, 20));
            this.selectedFeature.Add(new KeyValuePair<float, float>(26, 31));
            this.selectedFeature.Add(new KeyValuePair<float, float>(9, 70));
            this.selectedFeature.Add(new KeyValuePair<float, float>(17, 4));
            this.selectedFeature.Add(new KeyValuePair<float, float>(6, 12));
            this.selectedFeature.Add(new KeyValuePair<float, float>(15, 19));
            this.selectedFeature.Add(new KeyValuePair<float, float>(43, 14));
            this.selectedFeature.Add(new KeyValuePair<float, float>(35, 18));
            this.selectedFeature.Add(new KeyValuePair<float, float>(24, 41));
            this.selectedFeature.Add(new KeyValuePair<float, float>(28, 500));
            this.correlatedFeature = new List<KeyValuePair<float, float>>(this.selectedFeature);
            /*test code for creating a TimeSeries*/
            //String Reg_ts_path = "C:\\Users\\USER\\source\\repos\\DllTest\\reg_flight.csv"; //with NO features(for beggining of programm)

            //TS = Create_Regular_TS(Reg_ts_path, attributes, attributes.Length);// time-series, created by XML
            //IntPtr DW = CreateWrappedData(TS, "aileron");
            //FvectorToList(DW);
            

        }

        public string Time
        {
            get
            {
                return this.time;
            }
            set
            {
                if (value != time)
                {
                    this.time = value;
                    notifyPropertyChanged("Time");
                }
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
                    // notifyPropertyChanged("Playback_speed");
                }
            }
        }

        public int CurrentLinePlaying { get; private set; }

        public List<string> Variables
        {
            get
            {
                return this.variables;
            }
            set
            {
                if (value != this.variables)
                {
                    this.variables = value;
                    notifyPropertyChanged("Variables");
                }
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
        /// user moved the time slider.
        /// </summary>
        /// <param name="value"></param>
        public void updateTime(double value)
        {
            return;
        }


        protected void notifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public List<KeyValuePair<float, float>> SelectedFeature
        {
            get
            {
                return selectedFeature;
            }
            set
            {
                if (value != selectedFeature)
                {
                    selectedFeature = value;
                    notifyPropertyChanged("SelectedFeature");
                }
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
                    notifyPropertyChanged("CorrelatedFeature");
                }
            }
        }
        /// <summary>
        /// User pressed restart button.
        /// </summary>
        public void restart()
        {
            Playback_speed = 10;
            CurrentLinePlaying = 0;
            // need to check that the thread is still running and maybe restart the flight gear app.
        }

        /// <summary>
        /// Meaning the playback needs to stop completely and next time restart
        /// </summary>
        public bool Stop
        {
            get
            {
                return stop;
            }
            set
            {
                if (this.stop != value ){
                    this.stop = value;
                }
            }
        }

        /// <summary>
        /// Meaning the playback needs to pause but afterwards play from the same spot
        /// </summary>
        public bool Pause
        {
            get
            {
                return pause;
            }
            set
            {
                if (value)
                {
                    this.pause = value;
                    this.ticks = Timeout.Infinite;
                }
            }
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
                    //TS = Create_Regular_TS(RegFlightCSV, attributes, attributes.Length);
                    readCSV(this.regFlightCSV);
                }
            }
        }

        // Holds the path to regular flight CSV file
        private string anomalyFlightCSV;

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
                this.ticks = 1000 / playbackSpeed;
                this.playingThread.Interrupt();
                return;
            }
            else if (stop)
            {
                Console.WriteLine("in the play method in the if stop = true");
                stop = false;
                currentLinePlaying = 0;
            }

            
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
                        Console.WriteLine(line);
                        currentLinePlaying++;
                        Console.WriteLine("The current line index is:");
                        Console.WriteLine(currentLinePlaying);
                        Console.WriteLine("The playback speed is:");
                        Console.WriteLine(playbackSpeed);
                        Console.WriteLine("The ticks:");
                        Console.WriteLine(ticks);
                        Thread.Sleep(ticks);
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
            List<string> attributes_list = new List<string>();

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
                            if (attributes_list.Contains(att) == true)
                            {
                                att += "1";
                            }
                            attributes_list.Add(att);
                        }
                    }
                    reader.Read();
                }
            }

            this.Variables = attributes_list;
            attributes = attributes_list.ToArray();
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
