﻿using Server.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Util
{

    public delegate void onConnect(AdvanceStream stream);
    public class ClientObject
    {
        private string ip;
        private int host;
        private TcpClient tcpClient;
        public AdvanceStream stream { get; set; }

        public Logger logger { get; set; }

        public ClientObject() { }

        public void initClient(string ip,int host) {
            this.ip = ip;
            this.host = host;
        }

        public void connect(onConnect onConnect) {
            try
            {
                tcpClient = new TcpClient(ip, host);
                stream = new AdvanceStream(tcpClient.GetStream());
                onConnect.Invoke(stream);
            }
            catch (Exception e) {
                logger.Invoke("Error while connect to server " + e.Message);
            }
        }

        public void connectUntilSuss(onConnect onConnect)
        {
            new Thread(() => {
                while (true)
                {
                    try
                    {
                        tcpClient = new TcpClient(ip, host);
                        stream = new AdvanceStream(tcpClient.GetStream());
                        onConnect.Invoke(stream);
                        if (tcpClient != null)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                       
                    }
                }

            }).Start();
        }

        public void close()
        {
            tcpClient.Close();
        }
    }
}