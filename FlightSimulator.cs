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

namespace EX2
{
    public class FlightSimulator : IFlightSimulator
    {
        private string[] attributes;

        private string FGPath;
        private List<KeyValuePair<float, float>> selectedFeature = new List<KeyValuePair<float, float>>();
        private List<KeyValuePair<float, float>> correlatedFeature;
        private List<string> variables = new List<string>();
        private string time;
        private string playbackSpeed;
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


            string pathToApp = "D:\\Applications\\FlightGear 2020.3.6\\bin\\fgfs.exe";
            string pathToSettings = "D:\\Learn It\\2nd year\advanced programming2\\project\\playback_small.xml";
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
            /*
            this.variables.Add("speed");
            this.variables.Add("height");
            this.variables.Add("throttle");
            this.variables.Add("width");
            this.variables.Add("HAHAHAHAHHAHA");
            this.variables.Add("speed");
            this.variables.Add("height");
            this.variables.Add("throttle");
            this.variables.Add("width");
            this.variables.Add("HAHAHAHAHHAHA");
            this.variables.Add("speed");
            this.variables.Add("height");
            this.variables.Add("throttle");
            this.variables.Add("width");
            this.variables.Add("HAHAHAHAHHAHA");
            */

            //FlightGear fg = new FlightGear(pathToApp, "D:\\MyData\\Documents\\GitHub\\ex1\resources\\playback_small.xml");
            //fg.start();             
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
                    notifyPropertyChanged("time");
                }
            }
        }

        public string Playback_speed
        {
            get
            {
                return this.playbackSpeed;
            }
            set
            {
                if (value != playbackSpeed)
                {
                    this.playbackSpeed = value;
                    notifyPropertyChanged("playbackSpeed");
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
                    notifyPropertyChanged("variables");
                }
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
                    notifyPropertyChanged("selectedFeature");
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
                    notifyPropertyChanged("correlatedFeature");
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


    }
}
