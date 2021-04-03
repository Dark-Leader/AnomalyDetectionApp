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
            private string arguments = "--generic=socket,in,10,127.0.0.1,5400,tcp,";

            public FlightGear(string fileName, string settings)
            {
                this.fileName = fileName;
                arguments = arguments + settings + " --fdm=null";
                //arguments = arguments + settings;
            }

            public void start()
            {
                //System.Diagnostics.Process.Start(fileName, arguments);

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


            //FlightGear fg = new FlightGear(pathToApp, pathToSettings);
            //fg.start();

            this.csvData = data;
            
            
            
        }

        public void play()
        {
            try
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                int port = 5400;
                TcpClient client = new TcpClient();
                client.Connect(ip, port);
                NetworkStream ns = client.GetStream();


                Console.WriteLine("Connected");

                using (StreamReader rd = new StreamReader(this.csvData))
                {
                    String line;
                    
                    while ((line=rd.ReadLine()) != null)
                    {
                        byte[] buffer = Encoding.ASCII.GetBytes(line + "/r/n");
                        ns.Write(buffer, 0, buffer.Length);
                        Thread.Sleep(100);
                        
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
