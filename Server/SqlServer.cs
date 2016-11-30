using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Server
{
    static class SqlServer
    {



        static DataClasses1DataContext db = new DataClasses1DataContext();

        //  private const string connectionStrig = "Server= hania\\sqlexpress; Database= Cryptocurrency; Integrated Security=True;";
        // public static SqlConnection sqlConnection = new SqlConnection(connectionStrig);

        //public static bool startconnection()
        //{
        //  try
        //{
        //  sqlConnection.Open();
        // return true;
        //}
        //catch (Exception e)
        //{
        //    return false;
        //  }
        //}

        public static void InsertUser(string userName, string password, int balance)
        {
            User s = new User();
            s.Users_Name = userName;
            s.User_Password = password;
            s.User_balance = balance;
            db.Users.InsertOnSubmit(s);
            db.SubmitChanges();

        }

        public static void Authentication(string userName, string password)
        {
            
        }



    }

}

