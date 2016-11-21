using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class clientAccount
    {
        private int balance;
        private string userName;
        public static LinkedList<clientAccount> ClientList = new LinkedList<clientAccount>();

        public static void fillList()
        {
            ClientList.AddFirst(new clientAccount("Kate", 500));
            ClientList.AddFirst(new clientAccount("Adam", 200));
            ClientList.AddFirst(new clientAccount("Tom", 400));
            ClientList.AddFirst(new clientAccount("Rose", 700));
            // ClientList.AddFirst(new clientAccount("Charle", 800));

        }

        public clientAccount(string name, int balance)
        {
            if (balance > 0)
                this.balance = balance;
            else
            {
                this.balance = -balance;
            }
            userName = name;
        }

        public string ToString()
        {
            return ("Name: " + userName + "  balance: " + balance.ToString() + " \n");
        }


    }
}
