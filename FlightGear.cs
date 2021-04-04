using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EX2
{
    class FlightGear
    {
        // path to the application file
        private string fileName;


        // command line arguments to pass to the application
        private string arguments = "--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small --fdm=null";

        public FlightGear(string fileName, string settings)
        {
            this.fileName = fileName;

            // ADD path to settings and add it to data/protocol in fg and to args
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
    }
}
