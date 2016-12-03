
using Server.Models;
using Server.Util;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace Server
{
    class ServerMethods
    {
        public static byte[] AESPublicKey;
        public static string RSAPublicKey;
        public static RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024);


        private static string IP = "127.0.0.1";
        private static int HOST = 13000;

        public static TcpListener tcpListener = null;


        public static void IntiServer()
        {
            IPAddress localAddr = IPAddress.Parse(IP);
            tcpListener = new TcpListener(localAddr, HOST);
            tcpListener.Start();
            AESPublicKey = Algorithms.KeyGenerator.getRandomKeyByteArray();
            RSAPublicKey = RSA.ToXmlString(false);
        }

        public static void BeginListening()
        {
            MainWindow.instance.Log("AES Server Public Key\n" + Convert.ToBase64String(AESPublicKey));
            MainWindow.instance.Log("RSA Server Public Key \n" + RSAPublicKey);
            try
            {
                TcpClient clientSocket = default(TcpClient);
                int counter = 0;
                while (true)
                {
                    counter += 1;
                    clientSocket = tcpListener.AcceptTcpClient();
                    HandleClient client = new HandleClient(clientSocket, counter);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public static Boolean DoTransaction(string sender, string reciver, string amount)
        {
            int senderID = int.Parse(sender);
            int reciverID = int.Parse(reciver);


            DBContext db = DBContext.getInstace();

            int transmitted = Int32.Parse(amount);
            var FromQuery = from t in db.Clients where t.Id == senderID select t;
            var ToQuery = from t in db.Clients where t.Id == reciverID select t;
            int senderBalance = 0;
            int ReciverBalance = 0;
            foreach (Server.Models.Client clinet in FromQuery)
            {
                senderBalance = clinet.Balance;
            }
            foreach (Server.Models.Client clinet in FromQuery)
            {
                ReciverBalance = clinet.Balance;
            }
            if (senderBalance < transmitted)
                return false;
            else
            {
                Server.Models.Client sendeUser = db.Clients.First(e => e.Id.Equals(senderID));
                Server.Models.Client reciverUser = db.Clients.First(e => e.Id.Equals(reciverID));
                reciverUser.Balance += transmitted;
                sendeUser.Balance -= transmitted;
                db.SaveChanges();
                Transaction t = new Transaction();
                t.Amount = transmitted;
                t.ReciverId = reciverUser.Id;
                t.SenderId = sendeUser.Id;
                db.Transactions.Add(t);
                db.SaveChanges();
                return true;
            }



        }

    }
}
