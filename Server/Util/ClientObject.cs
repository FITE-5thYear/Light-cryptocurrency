using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Server.Util
{
    public delegate void onConnect(AdvanceStream stream);
    public class ClientObject
    {
        private string ip;
        private int host;
        private TcpClient tcpClient;
        public AdvanceStream stream { get; set; }
        public bool isServerReady { get; set; }

        public Logger logger { get; set; }

        public ClientObject()
        {
            isServerReady = false;
        }

        public void initClient(string ip, int host)
        {
            this.ip = ip;
            this.host = host;
        }

        public void connect(onConnect onConnect)
        {
            try
            {
                tcpClient = new TcpClient(ip, host);
                stream = new AdvanceStream(tcpClient.GetStream());
                onConnect.Invoke(stream);
            }
            catch (Exception e)
            {
                logger.Invoke("Error while connect to server " + e.Message);
            }
        }

        public void connectUntilSuss(onConnect onConnect)
        {
            Thread x = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        tcpClient = new TcpClient(ip, host);
                        stream = new AdvanceStream(tcpClient.GetStream());
                        isServerReady = true;
                       
                        if (tcpClient != null)
                        {
                            onConnect.Invoke(stream);
                            break;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }

            });
            x.IsBackground = true;
            x.Start();
        }

        public void close()
        {
            if (tcpClient != null)
                tcpClient.Close();
        }
    }
}

