using Client.Algorithms;
using Server.Algorithms;
using Server.Models;
using Server.Util;
using System;
using System.Text;
using System.Windows;

namespace Client.Util
{
    class RequestsManager
    {
        public static void GetPublicKey(AdvanceStream stream)
        {
            stream.Write("0");
            byte[] inStream;
            inStream = stream.ReadBytes();
            KeyManager.serverRSAPublicKey = Encoding.UTF8.GetString(inStream, 0, inStream.Length);

            MainWindow.instance.Log("Server AES Public Key", KeyManager.serverAESPublicKey);
            MainWindow.instance.Log("Server AES Public Key", KeyManager.serverRSAPublicKey);
            MainWindow.instance.Log();
        }

        public static bool SignUp(AdvanceStream stream, string signUpData)
        {
            bool result = false;
            bool signUpResult = false;
            SignUpObject signUp = SignUpObject.newLoginObject(signUpData);
            MainWindow.clientForCertificate.connectUntilSuss((e) =>
            {
                RequestsManager.connectToCA(e);
            });
            RSA rsa = new RSA(signUp.name);
            KeyManager.generateRSAPublicKey(rsa.rsaSP);
            KeyManager.generateRSAPrivateKey(rsa.rsaSP);
            Models.DigitalCertificate dc=new Models.DigitalCertificate();
            result =getCertificate(MainWindow.clientForCertificate.stream, signUp.name, KeyManager.RSAPublicKey, out dc);

            stream.Write("5");
            if (result)

            {
                stream.Write(dc.toJsonObject());
                string checkResult=stream.ReadString();
                if (checkResult == "1")
                {
                    MainWindow.instance.Log("Certificate has been checked\nreciving Server Public Key");
                    byte[] signUpByte = Encoding.UTF8.GetBytes(signUpData);
                    byte[] EncreptedLoginDataByte = rsa.encrypte(signUpByte, KeyManager.serverRSAPublicKey);

                    MainWindow.instance.Log("Sign Up Data", signUpData);
                    MainWindow.instance.Log("Encrypted Sign Up Data",Encoding.UTF8.GetString( EncreptedLoginDataByte));

                    stream.Write(EncreptedLoginDataByte);
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
                        string[] words = response.Split('\t');
                        MainWindow.user = Server.Models.Client.newClientObject(words[0]);
                        MainWindow.instance.Log(words[0]);
                        byte[] inStream = stream.ReadBytes();
                        byte[] decrypKey = rsa.decrypt(inStream, KeyManager.RSAPrivateKey);
                        MainWindow.instance.Log("Encrypted AES Key", Convert.ToBase64String(inStream, 0, inStream.Length));
                        KeyManager.serverAESPublicKey = Convert.ToBase64String(decrypKey, 0, decrypKey.Length);
                        MainWindow.instance.Log("AES Key",KeyManager.serverAESPublicKey);
                        signUpResult = true;
                    }
                }
                else
                {
                    MessageBox.Show("Not a vailed certificate");
                }
                MainWindow.instance.Log();
            }
            else
            {
                MessageBox.Show("Can't get a certificate");
            }
            return signUpResult;
        }

        public static Server.Models.Client Login(AdvanceStream stream, string loginData)
        {
            // To generate private key for RSA if not exist
            RSA rsa = new RSA(LoginObject.newLoginObject(loginData).username);

            KeyManager.generateRSAPublicKey(rsa.rsaSP);
            KeyManager.generateRSAPrivateKey(rsa.rsaSP);


            AES aes = AES.getInstance();
            byte[] msg = Encoding.UTF8.GetBytes(loginData);
            byte[] EncreptedLoginData = rsa.encrypte(msg, KeyManager.serverRSAPublicKey);

            MainWindow.instance.Log("Login Data", loginData);
            MainWindow.instance.Log("Encrypted Login Data",Encoding.UTF8.GetString( EncreptedLoginData));

            stream.Write("1");
            
            stream.Write(KeyManager.RSAPublicKey+'\t'+ Encoding.UTF8.GetString( EncreptedLoginData));
          

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
                byte[] inStream = stream.ReadBytes();
                byte[] decrypKey = rsa.decrypt(inStream, KeyManager.RSAPrivateKey);
                MainWindow.instance.Log("Encrypted AES Key", Convert.ToBase64String(inStream, 0, inStream.Length));
                KeyManager.serverAESPublicKey = Convert.ToBase64String(decrypKey, 0, decrypKey.Length);
                MainWindow.instance.Log("AES Key", KeyManager.serverAESPublicKey);


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

        public static void connectToCA(AdvanceStream stream)
        {
            stream.Write("100");
        }
        public static bool getCertificate(AdvanceStream stream,string userName,string publicKey,out Models.DigitalCertificate dc)
        {
            stream.Write("0");
            stream.Write(userName+'\t');
            stream.Write(publicKey);
            string result = stream.ReadString();
            if (result == "1")
            {
                string certificateString = stream.ReadString();
                Models.DigitalCertificate certificate = Models.DigitalCertificate.newClientObject(certificateString);
                dc = certificate;
                MainWindow.instance.Log(certificate.ToString());
                MessageBox.Show(certificate.ToString());
                return true;
            }
            else
            {
                dc = null;
                return false;
            }

            

            
        }



        #region Helper

        private static string getString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        private static byte[] getBytes(string st)
        {
            return Encoding.UTF8.GetBytes(st);
        }

        #endregion
    }
}
