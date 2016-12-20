using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CertificationAuthorities.Algorithms;

namespace CertificationAuthorities.Util
{


    public delegate void Logger(string message);
    public delegate void Response(AdvanceStream stream);

    public class ServerObject
    {
        private string ip;
        private int host;
        public static RSA rsa = new RSA("Certification Authority");


        private TcpListener tcpListener;
        public Logger logger { get; set; }
        public Response respose { get; set; }
        public ServerObject(string ip, int host)
        {
            this.ip = ip;
            this.host = host;
     
            KeyManager.generateRSAPrivateKey(rsa.rsaSP);
            KeyManager.generateRSAPublicKey(rsa.rsaSP);
            
           


        }
        public void intiServer()
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            tcpListener = new TcpListener(localAddr, host);
        }
        public void stopServer()
        {
            tcpListener.Stop();
        }
        public void startServer()
        {
            tcpListener.Start();
            log("Certification Authority started");
            log("Public Key"+"\n" +KeyManager.RSAPublicKey);
            Thread x = new Thread(() =>
            {
                // waiting for clinets
                try
                {
                    TcpClient socket = default(TcpClient);
                    while (true)
                    {
                        socket = tcpListener.AcceptTcpClient();
                        resposeToClient(new AdvanceStream(socket.GetStream()));
                    }
                }
                catch (Exception e)
                {
                    log("Server failed to start\n" + e.Message);
                }
            });
            x.IsBackground = true;
            x.Start();
        }
        private void resposeToClient(AdvanceStream streamWithClient)
        {
            // waiting to recive smth from spesific client
            Thread x = new Thread(() =>
            {
                while (true)
                {
                    respose.Invoke(streamWithClient);
                }
            });
            x.IsBackground = true;
            x.Start();
        }

        private void log(string message)
        {
            if (logger == null)
            {
                throw new NotImplementedException("Logger is null");
            }
            logger.Invoke(message);
        }
    }
}

