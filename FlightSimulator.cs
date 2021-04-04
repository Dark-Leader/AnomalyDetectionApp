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





namespace EX2
{
    class FlightSimulator
    {
        private string csvData;
        /*
        class FlightGear
        {
            // path to the application file
            private string fileName;


            // command line arguments to pass to the application
            private string arguments = "--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small --fdm=null";

            /*
            public FlightGear(string fileName, string settings)
            {
                this.fileName = fileName;

                // ADD path to settings and add it to data/protocol in fg and to args
                //arguments = arguments + settings + " --fdm=null";
                
            }

            public void start()
            {
                // opens the application

                ProcessStartInfo startInfo = new ProcessStartInfo(fileName);

                startInfo.WorkingDirectory = "D:\\Applications\\FlightGear 2020.3.6\\bin";

                startInfo.Arguments = arguments;

                Process.Start(startInfo);

            }
        }
            */


        public FlightSimulator(string data)
        {
            string pathToApp = "D:\\Applications\\FlightGear 2020.3.6\\bin\\fgfs.exe";
            string pathToSettings = "D:\\Learn It\\2nd year\advanced programming2\\project\\playback_small.xml";

            this.csvData = data;

            // FlightGear fg = new FlightGear(pathToApp, pathToSettings);
            // fg.start();             
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
    }
}
