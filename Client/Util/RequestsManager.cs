using Client.Algorithms;
using Server.Algorithms;
using Server.Util;
using System;
using System.Text;

namespace Client.Util
{
    class RequestsManager
    {
        public static void GetPublicKey(AdvanceStream stream)
        {
            stream.Write("0");

            byte[] inStream = stream.ReadBytes();
            KeyManager.serverAESPublicKey = Convert.ToBase64String(inStream, 0, inStream.Length);


            inStream = stream.ReadBytes();
            KeyManager.serverRSAPublicKey = Encoding.UTF8.GetString(inStream, 0, inStream.Length);

            MainWindow.instance.Log("Server AES Public Key" + KeyManager.serverAESPublicKey);
            MainWindow.instance.Log("Server AES Public Key" + KeyManager.serverRSAPublicKey);
        }
        public static void Login(AdvanceStream stream, string loginData)
        {
            string EncreptedLoginData = AES.Encrypt(loginData, KeyManager.serverAESPublicKey);

            MainWindow.instance.Log(EncreptedLoginData);

            stream.Write("1");

            stream.Write(EncreptedLoginData);

            string response = stream.ReadString();
            if (response.Equals("0"))
            {
                //no user
                MainWindow.instance.Log("No such user\n");
            }
            else if (response.Equals("1"))
            {
                //wrong password
                MainWindow.instance.Log("Wrong Password\n");
            }
            else
            {
                //ok
                response = stream.ReadString();
                MainWindow.user = Server.Models.Client.newClientObject(response);
                MainWindow.instance.Log(response);
            }
            
        }
        public static void Transfer(AdvanceStream stream, string transactionData)
        {
            RSAAlgorithm.RSAServiceProvider.FromXmlString(KeyManager.serverRSAPublicKey);
            byte[] EncreptedLoginData = RSAAlgorithm.RSAServiceProvider.Encrypt(Encoding.UTF8.GetBytes(transactionData), false);

            MainWindow.instance.Log(Encoding.UTF8.GetString(EncreptedLoginData));

            stream.Write("2");

            stream.Write(EncreptedLoginData);

        }
        public static void ViewAllAccounts(AdvanceStream stream)
        {
            stream.Write("3");

            string responseData = stream.ReadString();

            MainWindow.instance.Log("Enecrypted Accounts:");
            MainWindow.instance.Log(responseData);
            string derypte = AES.Decrypt(responseData, KeyManager.serverAESPublicKey);
            MainWindow.instance.Log("Decrypted Accounts:");
            MainWindow.instance.Log(derypte);
        }
    }
}
