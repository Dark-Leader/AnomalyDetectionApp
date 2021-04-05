using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EX2
{
    /// <summary>
    /// This class responsible for all the flight gear part
    /// </summary>
    class FlightGear
    {
        // path to the application file
        private string fileName;

        // socket that is connected to the application
        private Client client;

        // command line arguments to pass to the application
        private string arguments = "--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small --fdm=null";

        public FlightGear(string fileName, string settings)
        {
            this.fileName = fileName;
            client = new Client();

            // TODO add path to settings and add it to data/protocol in fg and to args
            //arguments = arguments + settings + " --fdm=null";

        }

        /// <summary>
        /// opens the flight gear application
        /// </summary>
        public void start()
        { 
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName);

            startInfo.WorkingDirectory = "D:\\Applications\\FlightGear 2020.3.6\\bin";

            startInfo.Arguments = arguments;

            Process.Start(startInfo);
        }

        public void sendData(string data)
        {
            if (!client.connectionEstablished)
            {
                client.connect("127.0.0.1", 5400);
            }
            client.write(data);
        }
    }
}
