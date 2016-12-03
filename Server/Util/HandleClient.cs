using Server.Models;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server.Algorithms;

namespace Server.Util
{
    class HandleClient
    {
        TcpClient clientSocket;
        int clientNo;

        public HandleClient(TcpClient inClientSocket, int clientNo) {
            clientSocket = inClientSocket;
            this.clientNo = clientNo;
            new Thread(handle).Start();
        }

        private void handle()
        {
            while (true)
            {
                try
                {

                    byte[] bytes = new Byte[1];

                    NetworkStream stream = clientSocket.GetStream();
                    stream.Read(bytes, 0, bytes.Length);
                    MainWindow.instance.Log("From Client " + clientNo);

                    if (bytes[0] == 0) // connect
                    {
                        stream.Write(ServerMethods.AESPublicKey, 0, ServerMethods.AESPublicKey.Length);
                        stream.Flush();

                        bytes = Encoding.UTF8.GetBytes(ServerMethods.RSAPublicKey);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }


                    if (bytes[0] == 1) // login
                    {
                        bytes = new Byte[256];
                        int lenght = stream.Read(bytes, 0, bytes.Length);
                        String encrypteData = System.Text.Encoding.UTF8.GetString(bytes, 0, lenght);


                        MainWindow.instance.Log("Login Encrypted Data:\t" + encrypteData + "\n");


                        string realData = AES2.Decrypt(encrypteData, Convert.ToBase64String(ServerMethods.AESPublicKey));
                        MainWindow.instance.Log("Login Decrypted Data:\t" + realData + "\n");


                        LoginObject loginObject = LoginObject.newLoginObject(realData);



                        var user = DBContext.getInstace().Clients.SingleOrDefault(item => item.Username == loginObject.username);
                        if (user == null)
                        {
                            bytes = Encoding.UTF8.GetBytes("0");
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();

                            MainWindow.instance.Log("\t" + "No such user: " + loginObject.username);
                        }
                        else
                        {
                            if (!user.Password.Equals(loginObject.password))
                            {
                                bytes = Encoding.UTF8.GetBytes("1");
                                stream.Write(bytes, 0, bytes.Length);
                                stream.Flush();
                                MainWindow.instance.Log("\t" + "Wrong credentials for " + loginObject.username);
                            }
                            else
                            {
                                bytes = Encoding.UTF8.GetBytes("2");
                                stream.Write(bytes, 0, bytes.Length);
                                stream.Flush();

                                bytes = Encoding.UTF8.GetBytes(user.toJsonObject());
                                stream.Write(bytes, 0, bytes.Length);
                                stream.Flush();

                                MainWindow.instance.Log(user.Name + " Logged in");
                            }
                        }

                    }
                    if (bytes[0] == 3) // transfer
                    {
                        bytes = new byte[128];
                        int lenght = stream.Read(bytes, 0, 128);
                        byte[] x = ServerMethods.RSA.Decrypt(bytes, false);
                        String transactionData = Encoding.UTF8.GetString(x);
                        MainWindow.instance.Log("\t" + transactionData);
                        TransactionObject tran = TransactionObject.newLoginObject(transactionData);
                        if (ServerMethods.DoTransaction(tran.senderID, tran.reciverID, tran.amount))
                        {
                            MainWindow.instance.Log("\t" + "Transaction Ok");
                        }
                        else
                        {
                            MainWindow.instance.Log("\t" + "Transaction Error");

                        }
                    }

                    if (bytes[0] == 4)
                    {

                        var allAccountQuery = from t in DBContext.getInstace().Clients select t;
                        string allAccounts = null;
                        foreach (Server.Models.Client clinet in allAccountQuery)
                        {
                            allAccounts += clinet.Username + "\t" + clinet.Balance;
                        }

                        string EncreptedAllAccounts = AES2.Encrypt(allAccounts, Convert.ToBase64String(ServerMethods.AESPublicKey));

                        MainWindow.instance.Log("Send all accounts encypted:");
                        MainWindow.instance.Log(EncreptedAllAccounts);
                        bytes = Encoding.Unicode.GetBytes(EncreptedAllAccounts);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }

                catch (Exception ex)
                {
                    MainWindow.instance.Log(" >> " + ex.ToString());
                }
            }
        }
    }
}
