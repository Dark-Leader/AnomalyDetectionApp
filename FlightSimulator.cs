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

namespace EX2
{
    public class FlightSimulator : IFlightSimulator
    {
        private string[] attributes;

        private string FGPath;

        // how many lines are there in the received flight data csv
        private int totalLines;

        private List<KeyValuePair<float, float>> selectedFeature = new List<KeyValuePair<float, float>>();
        private List<KeyValuePair<float, float>> correlatedFeature;
        private List<string> variables = new List<string>();
        private string time = "00:00:00";
        private string playbackSpeed;
        private float linesPerSecond = 10;
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

            playbackSpeed = "1.0";
           
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

        public string Playback_speed
        {
            get
            {
                return this.playbackSpeed;
            }
            private set
            {
                if (value != playbackSpeed)
                {
                    this.playbackSpeed = value;
                    notifyPropertyChanged("Playback_speed");
                }
            }
        }

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
        /// <summary>
        /// user moved the time slider.
        /// </summary>
        /// <param name="value"></param>
        public void updateTime(double value)
        {
            return;
        }


        public void bottom_control_clicked(string name)
        {
            switch (name)
            {
                case "Start": // user clicked play button.
                    break;
                case "Stop": // user clicked pause button.
                    break;
                case "Inc": // user clicked increment playback speed button.
                    increaseSpeed();
                    break;
                case "Dec": // user clicked decrease playback speed button.
                    decreaseSpeed();
                    break;
                case "Max": // user clicked change playback speed to maximum button.
                    maxSpeed();
                    break;
                case "Min": // user clicked change playback speed to minimum button.
                    minSpeed();
                    break;
                case "Restart": // user clicked the restart playback of video button.
                    break;

                default:
                    return;
            } 
        }

        /// <summary>
        /// deacrease playback speed by 2 rows per frame.
        /// </summary>
        private void decreaseSpeed()
        {
            double current = double.Parse(Playback_speed, CultureInfo.InvariantCulture);
            if (current > 0.2)
            {
                double newSpeed = current - 0.2;
                Playback_speed = newSpeed.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// set playback speed to 20 rows per frame.
        /// </summary>
        private void maxSpeed()
        {
            Playback_speed = "2.0";
        }

        /// <summary>
        /// set playback speed to 2 rows per frame.
        /// </summary>
        private void minSpeed()
        {
            Playback_speed = "0.20";
        }

        /// <summary>
        /// increase playback speed by 2 rows per frame.
        /// </summary>
        private void increaseSpeed()
        {
            double current = double.Parse(Playback_speed, CultureInfo.InvariantCulture);
            if (current < 2)
            {
                double newSpeed = current + 0.2;
                Playback_speed = newSpeed.ToString(CultureInfo.InvariantCulture);
            }
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

        public void setCSVFile(string name)
        {
            this.csvData = name;
        }

        public void setFGPath(string name)
        {
            this.FGPath = name;
            FlightGear fg = new FlightGear(name, pathToXML);
        }

        public void play()
        {
            try
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                int port = 5400;
                TcpClient client = new TcpClient();

                client.Connect(ip, port);
                Console.WriteLine("Connection established");
                NetworkStream ns = client.GetStream();


                Console.WriteLine("Connected");

                using (StreamReader rd = new StreamReader(this.csvData))
                {
                    String line;

                    while ((line = rd.ReadLine()) != null)
                    {
                        line += "\r\n";
                        Console.WriteLine(line);
                        if (ns.CanWrite)
                        {
                            byte[] buffer = Encoding.ASCII.GetBytes(line);
                            ns.Write(buffer, 0, buffer.Length);
                            Thread.Sleep(100);
                        }
                        else
                        {
                            Console.WriteLine("You cannot write data to this stream.");
                            /*
                            tcpClient.Close();

                            // Closing the tcpClient instance does not close the network stream.
                            netStream.Close();
                            return;
                            */
                        }
                    }
                }
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
                            attributes_list.Add(reader.ReadString());
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
