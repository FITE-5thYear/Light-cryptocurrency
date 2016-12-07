using Server.Algorithms;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    transfer(stream);
                    break;
                case "3":
                    viewAllAccount(stream);
                    break;
            }
        }

        private static void sendKeys(AdvanceStream stream)
        {
            stream.Write(KeysManager.AESPublicKey);

            stream.Write(KeysManager.RSAPublicKey);
        }

        private static void login(AdvanceStream stream)
        {
            String encrypteData = stream.ReadString();

            MainWindow.instance.Log("Login Encrypted Data:\t" + encrypteData + "\n");


            string realData = AES.Decrypt(encrypteData, Convert.ToBase64String(KeysManager.AESPublicKey));
            MainWindow.instance.Log("Login Decrypted Data:\t" + realData + "\n");


            LoginObject loginObject = LoginObject.newLoginObject(realData);



            var user = DBContext.getInstace().Clients.SingleOrDefault(item => item.Username == loginObject.username);
            if (user == null)
            {
                stream.Write("0");

                MainWindow.instance.Log("\t" + "No such user: " + loginObject.username);
            }
            else
            {
                if (!user.Password.Equals(loginObject.password))
                {
                    stream.Write("1");
                    MainWindow.instance.Log("\t" + "Wrong credentials for " + loginObject.username);
                }
                else
                {
                    stream.Write("2");

                    stream.Write(user.toJsonObject());

                    MainWindow.instance.Log(user.Name + " Logged in");
                }
            }
        }

        private static void transfer(AdvanceStream stream)
        {
            byte[] x = RSAAlgorithm.RSAServiceProvider.Decrypt(stream.ReadBytes(), false);
            String transactionData = Encoding.UTF8.GetString(x);
            MainWindow.instance.Log("\t" + transactionData);
            TransactionObject tran = TransactionObject.newLoginObject(transactionData);
            if (DBContext.DoTransaction(tran.senderID, tran.reciverID, tran.amount))
            {
                MainWindow.instance.Log("\t" + "Transaction Ok");
            }
            else
            {
                MainWindow.instance.Log("\t" + "Transaction Error");

            }
        }

        private static void viewAllAccount(AdvanceStream stream)
        {
            var allAccountQuery = from t in DBContext.getInstace().Clients select t;
            string allAccounts = null;
            foreach (Server.Models.Client clinet in allAccountQuery)
            {
                allAccounts += clinet.Username + "\t" + clinet.Balance + "\n";
            }

            string EncreptedAllAccounts = AES.Encrypt(allAccounts, Convert.ToBase64String(KeysManager.AESPublicKey));

            MainWindow.instance.Log("Send all accounts encypted:");
            MainWindow.instance.Log(EncreptedAllAccounts);

            stream.Write(EncreptedAllAccounts);
        }
    }
}

