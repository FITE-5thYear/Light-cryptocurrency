using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class client
    {
        private static Int32 port;
        private static IPAddress localAddr;
        private static TcpClient tcpClient = null;
        private static string key = "sec2017work";

        public static void InitClient()
        {
            try
            {
                port = 13000;
                tcpClient = new TcpClient("127.0.0.1", port);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string RequestAcc()
        {
            // 1 request all user account
            try
            {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes("1");
                NetworkStream stream = tcpClient.GetStream();
                stream.Write(data, 0, data.Length);
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                string decrypt = AES.Decrypt(responseData, key);
                return "Encyrpted data \n" + responseData + "\nDecrypted Message\n" + decrypt;

            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}
