﻿using System;
using System.Net.Sockets;
using System.Text;

namespace Server.Util
{
    public class AdvanceStream
    {
        private NetworkStream stream;
        public AdvanceStream(NetworkStream networkStream) {
            this.stream = networkStream;
        }
        public string ReadString()
        {
            byte[] bytes = new byte[1024];
            int lenght = stream.Read(bytes, 0, bytes.Length);
            return System.Text.Encoding.UTF8.GetString(bytes, 0, lenght);
        }

        public byte[] ReadBytes()
        {
            byte[] bytes = new byte[1024];
            int lenght = stream.Read(bytes, 0, bytes.Length);
            byte[] realBytes = new byte[lenght];
            Array.Copy(bytes, realBytes, lenght);
            return realBytes;
        }

        public void Write(byte[] bytesToWrite) {
            stream.Write(bytesToWrite, 0, bytesToWrite.Length);
            stream.Flush();
            System.Threading.Thread.Sleep(1000);
        }

        public void Write(string stringToWrites)
        {
            Byte[] bytes = Encoding.UTF8.GetBytes(stringToWrites);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            System.Threading.Thread.Sleep(1000);
        }

    }
}
