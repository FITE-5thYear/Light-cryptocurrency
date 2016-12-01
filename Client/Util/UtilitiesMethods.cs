using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;

namespace Client
{
    class UtilitiesMethods
    {

        private static string IP = "127.0.0.1";
        private static int HOST = 13000;
        public static string serverAESPublicKey;
        public static string serverRSAPublicKey;

        public static TcpClient tcpClient;
  
        public static void InitClient(MainWindow main)
        {
            tcpClient = new TcpClient(IP, HOST);
            NetworkStream serverStream = tcpClient.GetStream();
            byte [] sendBytes = new Byte[1];
            sendBytes[0] = 0;
            serverStream.Write(sendBytes, 0, sendBytes.Length);
            serverStream.Flush();

            byte[] inStream = new byte[256];
            int lenght = serverStream.Read(inStream, 0, inStream.Length);

            serverAESPublicKey = Convert.ToBase64String(inStream, 0, lenght);


            inStream = new byte[256];
            lenght = serverStream.Read(inStream, 0, inStream.Length);

            serverRSAPublicKey = Encoding.UTF8.GetString(inStream,0, lenght);

            main.Log("Connect To Server\n");
            main.Log("Server AES Public Key is \"" + serverAESPublicKey + "\"\n\n");
            main.Log("Server AES Public Key is \"" + serverRSAPublicKey + "\"\n\n");
            main.Log("\n\n\n");
        }

        public static void login(string loginData,MainWindow mainWindow)
        {
            NetworkStream serverStream = tcpClient.GetStream();
            if (tcpClient != null)
            {

                string EncreptedLoginData = AES1.Encrypt(loginData, serverAESPublicKey);

                mainWindow.Log(EncreptedLoginData);

                byte[] bytes = new Byte[1];
                bytes[0] = 1;
                serverStream.Write(bytes, 0, bytes.Length);
                serverStream.Flush();

                bytes = new Byte[1];
                bytes[0] = (Byte)EncreptedLoginData.Length;
                serverStream.Write(bytes, 0, bytes.Length);
                serverStream.Flush();


                bytes = Encoding.UTF8.GetBytes(EncreptedLoginData);
                serverStream.Write(bytes, 0, bytes.Length);
                serverStream.Flush();

                bytes = new byte[1];
                int length = serverStream.Read(bytes, 0, bytes.Length);                
                if(Encoding.UTF8.GetString(bytes) == "0")
                {
                    //no user
                    mainWindow.Log("No such user");
                }else if(Encoding.UTF8.GetString(bytes) == "1")
                {
                    //wrong password
                    mainWindow.Log("Wrong Password");
                }
                else
                {
                    //ok
                    bytes = new byte[256];
                    length = serverStream.Read(bytes, 0, bytes.Length);
                    string response = Encoding.UTF8.GetString(bytes, 0, length);

                    MainWindow.user = Client.Models.Client.newClientObject(response);
                    mainWindow.Log(response);                    
                }

            }
            else
            {
                MessageBox.Show("Connect to server First", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public static void transfer(string transactionData,MainWindow mainWindow)
        {
            if (MainWindow.user == null) { 
                MessageBox.Show("You need to login first");
                return;
            }

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024);
           
            NetworkStream serverStream = tcpClient.GetStream();
            if (tcpClient != null)
            {
                RSA.FromXmlString(serverRSAPublicKey);
                byte[] EncreptedLoginData = RSA.Encrypt(Encoding.UTF8.GetBytes(transactionData), false);

                mainWindow.Log(Encoding.UTF8.GetString(EncreptedLoginData));

                byte[] bytes = new Byte[1];
                bytes[0] = 3;
                serverStream.Write(bytes, 0, bytes.Length);
                serverStream.Flush();

                serverStream.Write(EncreptedLoginData, 0, EncreptedLoginData.Length);
                serverStream.Flush();
            }
            else
            {
                MessageBox.Show("Connect to server First", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }   
}
