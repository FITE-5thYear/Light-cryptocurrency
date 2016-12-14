using Server.Algorithms;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server.Util
{
    public class ResposesManager
    {
        public static void ProcessRequst(string requestType, AdvanceStream stream) {
            switch (requestType) {
                case "0":
                    sendKeys(stream);
                    break;
                case "1":
                    login(stream);
                    break;
                case "2":
                    transferWithRSA(stream);
                    break;
                case "3":
                    viewAllAccount(stream);
                    break;
                case "4":
                    transferWithPGP(stream);
                    break;
                case "5":
                    signUp(stream);
                    break;
            }
        }

        private static void signUp(AdvanceStream stream)
        {
            AES aes = AES.getInstance();
            string encrypteData = stream.ReadString();
            string realData = aes.Decrypt(encrypteData, KeysManager.AESkey);

            MainWindow.instance.Log("Encrypted SignUp Data", encrypteData);
            MainWindow.instance.Log("Decrypted SignUp Data", realData);

            SignUpObject signUpObject = SignUpObject.newLoginObject(realData);

            var user = DBContext.getInstace().Clients.SingleOrDefault(item => item.Username == signUpObject.username);
            if (user != null)
            {
                if (user.Username.Equals(signUpObject.username))
                {
                    stream.Write("0");
                    MainWindow.instance.Log("Error username already taken ", signUpObject.username);
                } else
                {
                    stream.Write("1");
                    MainWindow.instance.Log("Error password already taken");
                }

            }
            else
            {
                DBContext.getInstace().Clients.Add(new Models.Client(signUpObject.name, signUpObject.username, signUpObject.password));
                DBContext.getInstace().SaveChanges();
                stream.Write("2");
                user = DBContext.getInstace().Clients.SingleOrDefault(item => item.Username == signUpObject.username);
                stream.Write(user.toJsonObject());
                MainWindow.instance.Log("Sign Up Successfully", user.Name);  
            }

            MainWindow.instance.Log();
        }

        private static void sendKeys(AdvanceStream stream)
        {
            stream.Write(KeysManager.AESkey);

            stream.Write(KeysManager.RSAPublicKey);
        }

        private static void login(AdvanceStream stream)
        {
            AES aes = AES.getInstance();
            string encrypteData = stream.ReadString();
            string realData = aes.Decrypt(encrypteData, KeysManager.AESkey);

            MainWindow.instance.Log("Encrypted Login Data", encrypteData);
            MainWindow.instance.Log("Decrypted Login Data", realData);

            LoginObject loginObject = LoginObject.newLoginObject(realData);

            var user = DBContext.getInstace().Clients.SingleOrDefault(item => item.Username == loginObject.username);
            if (user == null)
            {
                stream.Write("0");
                MainWindow.instance.Log("Error no such user",loginObject.username);
            }
            else
            {
                if (!user.Password.Equals(loginObject.password))
                {
                    stream.Write("1");
                    MainWindow.instance.Log("Error Wrong credentials",loginObject.username);
                }
                else
                {
                    stream.Write("2");

                    stream.Write(user.toJsonObject());

                    MainWindow.instance.Log("Logged in",user.Name);
                }
            }

            MainWindow.instance.Log();
        }

        private static void transferWithRSA(AdvanceStream stream)
        {
            RSA rsa = new RSA ("Server");
            byte[] encrypytedTransferBytes  = stream.ReadBytes();
            byte[] decryptedTransferBytes   = rsa.decrypt(encrypytedTransferBytes, KeysManager.RSAPrivateKey);
            MainWindow.instance.Log("Encrypted Transfer Data", getString(encrypytedTransferBytes));
            MainWindow.instance.Log("Decrypted Transfer Data", getString(decryptedTransferBytes));

            TransactionObject tran = TransactionObject.newLoginObject(getString(decryptedTransferBytes));
            if (DBContext.DoTransaction(tran.senderID, tran.reciverID, tran.amount))
            {
                MainWindow.instance.Log("Transaction Ok");
            }
            else
            {
                MainWindow.instance.Log("Transaction Error");

            }

            MainWindow.instance.Log();
        }

        private static void transferWithPGP(AdvanceStream stream)
        {
            AES aes = AES.getInstance();
            RSA rsa = new RSA ("Server");
            byte[] encrypytedSessionKey = stream.ReadBytes();
            byte[] sessionKeyBytes = rsa.decrypt(encrypytedSessionKey, KeysManager.RSAPrivateKey);
            string encrypteTransferData = stream.ReadString();
            string decrptedTransferData = aes.Decrypt(encrypteTransferData, getString(sessionKeyBytes));


            MainWindow.instance.Log("Encrypted Session Key", Convert.ToBase64String(encrypytedSessionKey));
            MainWindow.instance.Log("Decrypted Session Key", Convert.ToBase64String(sessionKeyBytes));
            MainWindow.instance.Log("Encrypted Transfer Data", encrypteTransferData);
            MainWindow.instance.Log("Decrypted Transfer Data", decrptedTransferData);


            string clientPublicKey = stream.ReadString();
            MainWindow.instance.Log("Client Public RSA Key", clientPublicKey);


            byte[] signture = stream.ReadBytes();

            string recivedMessage = getString(encrypytedSessionKey) + encrypteTransferData;

            if (rsa.verifyData(getBytes(recivedMessage),clientPublicKey, signture))
            {
                TransactionObject tran = TransactionObject.newLoginObject(decrptedTransferData);

                if (DBContext.DoTransaction(tran.senderID, tran.reciverID, tran.amount))
                {
                    MainWindow.instance.Log("Transfer money done");
                }
                else
                {
                    MainWindow.instance.Log("Can't transfer money");
                }
            }
            else
            {
                MainWindow.instance.Log("Can't transfer money invalide signture");
            }
            MainWindow.instance.Log();
        }

        private static void viewAllAccount(AdvanceStream stream)
        {
            AES aes = AES.getInstance();

            var allAccountQuery = from t in DBContext.getInstace().Clients select t;
            string allAccounts = null;
            foreach (Server.Models.Client clinet in allAccountQuery)
            {
                allAccounts += clinet.Username + "\t" + clinet.Balance + "\n";
            }

            string EncreptedAllAccounts = aes.Encrypt(allAccounts, KeysManager.AESkey);
            stream.Write(EncreptedAllAccounts);

            MainWindow.instance.Log("Accounts Data", allAccounts);
            MainWindow.instance.Log("Encrypted Accounts Data", EncreptedAllAccounts);
            MainWindow.instance.Log();
        }


        private static string getString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        private static byte[] getBytes(string st)
        {
            return Encoding.UTF8.GetBytes(st);
        }
    }
}

