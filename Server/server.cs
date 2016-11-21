using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class server
    {
        private static Int32 port;
        private static IPAddress localAddr;
        public static TcpListener tcpListener = null;
        private static string key = "sec2017work";

        public static void IntiServer()
        {
            try
            {
                port = 13000;
                localAddr = IPAddress.Parse("127.0.0.1");
                tcpListener = new TcpListener(localAddr, port);
                tcpListener.Start();

            }
            catch (Exception)
            {

                throw;
            }

        }

        public static void Listening(MainWindow mainWindow)
        {
            try
            {

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                mainWindow.displayStatus("waiting for connection");

                // Enter the listening loop.
                while (true)
                {


                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = tcpListener.AcceptTcpClient();
                    mainWindow.displayStatus("Connected!\n");


                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        mainWindow.displayStatus("Request " + data + "is recived\n");
                        if (data == "1")
                        {
                            string message = prosessReq1();
                            message = AES.Encrypt(message, key);
                            byte[] msg = System.Text.Encoding.UTF8.GetBytes(message);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);

                        }

                    }


                    // Shutdown and end connection
                    client.Close();

                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e);

            }
        }

        public static string prosessReq1()
        {
            string all = "";
            foreach (var Account in clientAccount.ClientList)
            {
                all += Account.ToString();

            }
            return all;
        }

    }
}
