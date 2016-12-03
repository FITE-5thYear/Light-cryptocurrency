using Client.Algorithms;
using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;

namespace Client
{
    class ClientMethods
    {

        private static string IP = "127.0.0.1";
        private static int HOST = 13000;
        public static string serverAESPublicKey;
        public static string serverRSAPublicKey;

        public static TcpClient tcpClient;
  
        public static void InitClient()
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

            MainWindow.instance.Log("Connect To Server");
            MainWindow.instance.Log("Server AES Public Key" + serverAESPublicKey);
            MainWindow.instance.Log("Server AES Public Key" + serverRSAPublicKey);
        }

        public static void login(string loginData,MainWindow mainWindow)
        {
            NetworkStream serverStream = tcpClient.GetStream();
            if (tcpClient != null)
            {

                string EncreptedLoginData = AES2.Encrypt(loginData, serverAESPublicKey);

                mainWindow.Log(EncreptedLoginData);

                byte[] bytes = new Byte[1];
                bytes[0] = 1;
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
                    mainWindow.Log("No such user\n");
                }else if(Encoding.UTF8.GetString(bytes) == "1")
                {
                    //wrong password
                    mainWindow.Log("Wrong Password\n");
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

        public static void transfer(string transactionData)
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

                MainWindow.instance.Log(Encoding.UTF8.GetString(EncreptedLoginData));

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
        public static void ViewAllAccounts() {
           
            NetworkStream serverStream = tcpClient.GetStream();
            if (tcpClient != null)
            {

                byte[] bytes = new Byte[1];
                bytes[0] = 4;
                serverStream.Write(bytes, 0, bytes.Length);
                serverStream.Flush();


                bytes = new Byte[256];
                int length = serverStream.Read(bytes, 0, bytes.Length);
                string responseData = System.Text.Encoding.Unicode.GetString(bytes, 0, length);

                MainWindow.instance.Log("Enecrypted Accounts:");
                MainWindow.instance.Log(responseData);
                string derypte = AES2.Decrypt(responseData, serverAESPublicKey);
                MainWindow.instance.Log("Decrypted Accounts:");
                MainWindow.instance.Log(derypte);
            }
            else
            {
                MessageBox.Show("Connect to server First", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }   
}
