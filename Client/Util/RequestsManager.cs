using Client.Algorithms;
using Server.Algorithms;
using Server.Models;
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

            MainWindow.instance.Log("Server AES Public Key", KeyManager.serverAESPublicKey);
            MainWindow.instance.Log("Server AES Public Key", KeyManager.serverRSAPublicKey);
            MainWindow.instance.Log();
        }

        public static bool SignUp(AdvanceStream stream, string signUpData)
        {
            bool signUpResult = false;
            AES aes = AES.getInstance();
            string EncreptedLoginData = aes.Encrypt(signUpData, KeyManager.serverAESPublicKey);

            MainWindow.instance.Log("Sign Up Data", signUpData);
            MainWindow.instance.Log("Encrypted Sign Up Data", EncreptedLoginData);

            stream.Write("5");

            stream.Write(EncreptedLoginData);

            string response = stream.ReadString();
            if (response.Equals("0"))
            {
                //no user
                MainWindow.instance.Log("User name is taken");
                signUpResult = false;
            }
            else if (response.Equals("1"))
            {
                //wrong password
                MainWindow.instance.Log("Password is takten");
                signUpResult = false;
            }
            else
            {
                //ok
                response = stream.ReadString();
                MainWindow.user = Server.Models.Client.newClientObject(response);
                MainWindow.instance.Log(response);
                signUpResult = true;
            }

            MainWindow.instance.Log();
            return signUpResult;
        }

        public static Server.Models.Client Login(AdvanceStream stream, string loginData)
        {
            // To generate private key for RSA if not exist
            RSA rsa = new RSA(LoginObject.newLoginObject(loginData).username);

            KeyManager.generateRSAPublicKey(rsa.rsaSP);
            KeyManager.generateRSAPrivateKey(rsa.rsaSP);


            AES aes = AES.getInstance();
            string EncreptedLoginData = aes.Encrypt(loginData, KeyManager.serverAESPublicKey);

            MainWindow.instance.Log("Login Data", loginData);
            MainWindow.instance.Log("Encrypted Login Data", EncreptedLoginData);

            stream.Write("1");

            stream.Write(EncreptedLoginData);

            string response = stream.ReadString();
            if (response.Equals("0"))
            {
                //no user
                MainWindow.instance.Log("No such user");
                MainWindow.instance.Log();
                return null;
            }
            else if (response.Equals("1"))
            {
                //wrong password
                MainWindow.instance.Log("Wrong Password");
                MainWindow.instance.Log();
                return null;
            }
            else
            {
                //ok
                response = stream.ReadString();
                Server.Models.Client loginClient = Server.Models.Client.newClientObject(response);
                MainWindow.instance.Log(response);
                return loginClient;
            }
        }

        public static void TransferWithRSA(AdvanceStream stream, string transactionData)
        {
            RSA rsa = new RSA(MainWindow.user.Username);
            byte[] EncryptedTransferData = rsa.encrypte(getBytes(transactionData), KeyManager.serverRSAPublicKey);
           
            stream.Write("2");
            stream.Write(EncryptedTransferData);

            MainWindow.instance.Log("Transfer Data", transactionData);
            MainWindow.instance.Log("Encrypted Transfer Data", getString(EncryptedTransferData));
            MainWindow.instance.Log();
        }


        public static void TransferWithPGP(AdvanceStream stream, string transactionData)
        {
            RSA rsa = new RSA(MainWindow.user.Username);
            AES aes = AES.getInstance();
            
            byte[] encryptedSessionKey = rsa.encrypte(getBytes(KeyManager.SessionKey), KeyManager.serverRSAPublicKey);

            stream.Write("4");

            stream.Write(encryptedSessionKey);

            string EncreptedTransferData = aes.Encrypt(transactionData, KeyManager.SessionKey);

            stream.Write(EncreptedTransferData);

            MainWindow.instance.Log("Session Key", Convert.ToBase64String(getBytes(KeyManager.SessionKey)));
            MainWindow.instance.Log("Encrypted Session Key", Convert.ToBase64String(encryptedSessionKey));
            MainWindow.instance.Log("Transfer Data", transactionData);
            MainWindow.instance.Log("Encrypted Transfer Data", EncreptedTransferData);

            stream.Write(KeyManager.RSAPublicKey);

            string message = getString(encryptedSessionKey) + EncreptedTransferData;

            byte[] signture = rsa.signData(getBytes(message), KeyManager.RSAPrivateKey);
        

            stream.Write(signture);

            MainWindow.instance.Log();
        }

        public static void ViewAllAccounts(AdvanceStream stream)
        {
            AES aes = AES.getInstance();
            stream.Write("3");

            string encrptedAccounts = stream.ReadString();
            string deryptedAccounts = aes.Decrypt(encrptedAccounts, KeyManager.serverAESPublicKey);
 
            MainWindow.instance.Log("Encrypted Accounts Data", encrptedAccounts);
            MainWindow.instance.Log("Decrypted Accounts Data", deryptedAccounts);
            MainWindow.instance.Log();
        }



        #region Helper

        private static string getString(byte[] bytes) {
            return Encoding.UTF8.GetString(bytes);
        }

        private static byte[] getBytes(string st)
        {
            return Encoding.UTF8.GetBytes(st);
        }

        #endregion
    }
}
