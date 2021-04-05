using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX2
{
    /// <summary>
    /// Provides a mechanism for basic tcp protocol to send data over
    /// </summary>
    interface ITelnetClient
    {
        /// <summary>
        /// Connect to desired server in specified ip and port
        /// </summary>
        /// <param name="ip">The ip of the server.</param>
        /// <param name="port">The port that the server.</param>
        void connect(string ip, int port);

        /// <summary>
        /// Sends the specified data
        /// </summary>
        /// <param name="data">Data to send over</param>
        void write(string data);

        /// <summary>
        /// Terminate connection.
        /// </summary>
        void disconnect();
    }
}
