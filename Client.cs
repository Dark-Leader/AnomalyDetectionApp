using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace EX2
{

    /// <summary>
    /// A program for client socket
    /// </summary>
    class Client
    {
        private int serverPort;
        private TcpClient sender;
        private NetworkStream ns;

        /// <summary>
        /// Initializes a new instance of client which needs to connect to a server in the specified ip and port.
        /// </summary>
        /// <param name="ip">The ip of the server.</param>
        /// <param name="port">The port that the server listens to</param>
        public Client(string ip, int port)
        {
           
            int serverPort = port; // port = 5400

            sender = new TcpClient();
            try
            {
                sender.Connect(IPAddress.Parse(ip), serverPort);
                Console.WriteLine("Connection established");
                ns = sender.GetStream();
            }
            catch (SocketException e)
            {
                // An error occurred when accessing the socket.
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

        }

        public void sendData(string data)
        {
            // TODO send to server/
        }
    }
}
