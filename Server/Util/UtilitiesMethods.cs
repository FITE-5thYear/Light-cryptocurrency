using Newtonsoft.Json;
using Server.Algorithms;
using Server.Models;
using Server.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;

namespace Server
{
    class ServerMethods
    {
        public static byte[] AESPublicKey;
        public static string RSAPublicKey;
        public static RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024);


        private static string IP    = "127.0.0.1";
        private static int HOST     = 13000;

        public static TcpListener tcpListener = null;
     

        public static void IntiServer()
        {
            IPAddress localAddr = IPAddress.Parse(IP);
            tcpListener = new TcpListener(localAddr,HOST);
            tcpListener.Start();
            AESPublicKey = Algorithms.KeyGenerator.getRandomKeyByteArray();
            RSAPublicKey = RSA.ToXmlString(false);
        }

        public static void BeginListening(ClientWindows mainWindow)
        {
            mainWindow.Log("AES Server Public Key is \"" + Convert.ToBase64String(AESPublicKey) + "\"\n\n");
            mainWindow.Log("RSA Server Public Key is \"" + RSAPublicKey + "\"\n\n");
            mainWindow.Log("\n\n\n");
            mainWindow.Log("Waiting For Connections...\n");
            try
            {
                TcpClient clientSocket = default(TcpClient);
                int counter = 0;
                while (true)
                {
                    counter += 1;
                    clientSocket = tcpListener.AcceptTcpClient();
                    mainWindow.Log("Client " + counter + " Started.\n");
                    handleClinet client = new handleClinet();
                    client.startClient(clientSocket, counter, mainWindow);
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

            CryptocurrencyContext db = new CryptocurrencyContext();
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



    public class handleClinet
    {
        ClientWindows mainWindow;
        TcpClient clientSocket;
        int clientNo;

        public void startClient(TcpClient inClientSocket, int clientNo, ClientWindows mainWindow)
        {
            this.mainWindow = mainWindow;
            this.clientSocket = inClientSocket;
            this.clientNo = clientNo;
            new Thread(response).Start();
        }

        private void response(){     
            while (true)
            {
                try
                {

                    byte[] bytes = new Byte[1];


                    NetworkStream stream = clientSocket.GetStream();
                    stream.Read(bytes, 0, bytes.Length);
                    mainWindow.Log("From Client " + clientNo + ":\n");

                    if (bytes[0] == 0) // connect
                    {
                        stream.Write(ServerMethods.AESPublicKey, 0, ServerMethods.AESPublicKey.Length);
                        stream.Flush();
                        
                        bytes = Encoding.UTF8.GetBytes(ServerMethods.RSAPublicKey);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                        mainWindow.Log("\tClient " + clientNo + " Connected.\n");
                    }


                    if (bytes[0] == 1) // login
                    {
                        bytes = new Byte[1];
                        stream.Read(bytes, 0, bytes.Length);
                        bytes = new Byte[bytes[0]];
                        stream.Read(bytes, 0, bytes.Length);
                        String encrypteData = System.Text.Encoding.UTF8.GetString(bytes);
                        mainWindow.Log("\t" + encrypteData + "\n");


                        string realData = AES2.Decrypt(encrypteData, Convert.ToBase64String(ServerMethods.AESPublicKey));
                        mainWindow.Log("\t" + realData +"\n");


                        LoginObject loginObject = LoginObject.newLoginObject(realData);

                        CryptocurrencyContext db = new CryptocurrencyContext();

                        var user = db.Clients.SingleOrDefault(item => item.Username == loginObject.username);
                        if(user == null)
                        {
                            bytes = Encoding.UTF8.GetBytes("0");
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();

                            mainWindow.Log("\t" + "No such user: " + loginObject.username + "\n");
                        }                            
                        else
                        {
                            if (!user.Password.Equals(loginObject.password))
                            {
                                bytes = Encoding.UTF8.GetBytes("1");
                                stream.Write(bytes, 0, bytes.Length);
                                stream.Flush();
                                mainWindow.Log("\t" + "Wrong credentials for " + loginObject.username + "\n");
                            }else
                            {
                                bytes = Encoding.UTF8.GetBytes("2");
                                stream.Write(bytes, 0, bytes.Length);
                                stream.Flush();

                                bytes = Encoding.UTF8.GetBytes(user.toJsonObject());
                                stream.Write(bytes, 0, bytes.Length);
                                stream.Flush();

                                mainWindow.Log("\tClient " + clientNo + " Connected.\n");
                                mainWindow.Log("\t" + user.Balance.ToString() + "\n");
                            }
                        }

                    }
                    if (bytes[0] == 3) // transfer
                    {
                        bytes = new byte[128];
                        int lenght = stream.Read(bytes, 0, 128);
                        byte [] x = ServerMethods.RSA.Decrypt(bytes, false);
                        String transactionData = Encoding.UTF8.GetString(x);
                        mainWindow.Log("\t" + transactionData + "\n");
                        TransactionObject tran = TransactionObject.newLoginObject(transactionData);
                        if (ServerMethods.DoTransaction(tran.senderID, tran.reciverID, tran.amount))
                        {
                            mainWindow.Log("\t" + "Transaction Ok" + "\n");
                        }
                        else
                        {
                            mainWindow.Log("\t" + "Transaction Error" + "\n");

                        }
                    }

                    if (bytes[0] == 4)
                    {

                        CryptocurrencyContext db = new CryptocurrencyContext();
                        var allAccountQuery = from t in db.Clients select t ;
                        string allAccounts = null;
                        foreach (Server.Models.Client clinet in allAccountQuery) {
                            allAccounts += clinet.Username + "\t" + clinet.Balance + "\n"; 
                        }

                        string EncreptedAllAccounts = AES2.Encrypt(allAccounts, Convert.ToBase64String(ServerMethods.AESPublicKey));

                        mainWindow.Log("Send all accounts encypted:\n");
                        mainWindow.Log(EncreptedAllAccounts + "\n\n\n");
                        bytes = Encoding.Unicode.GetBytes(EncreptedAllAccounts);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }

                catch (Exception ex){
                    mainWindow.Log(" >> " + ex.ToString());
                }
            }
        }
    }
}
