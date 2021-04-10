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
    class Client : ITelnetClient
    {
        
        private TcpClient sender;
        private NetworkStream stream;
        public Boolean connectionEstablished { get; private set; }

        public Client()
        {
            sender = new TcpClient();
            connectionEstablished = false;
        }

        public void connect(string ip, int port)
        {
            try
            {
                sender.Connect(IPAddress.Parse(ip), port);
                Console.WriteLine("Connection established");
                this.stream = sender.GetStream();
                connectionEstablished = true;
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


        public void write(string data)
        {
            try
            {
                if (stream.CanWrite)
                {
                    byte[] buffer = Encoding.ASCII.GetBytes(data);
                    stream.Write(buffer, 0, buffer.Length);
                    Console.WriteLine("The data being sent:");
                    Console.WriteLine(data);

                }
                else
                {
                    Console.WriteLine("You cannot write data to this stream.");
                }
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

        public void disconnect()
        {
            stream.Close();
            sender.Close();
        }

    }
}
