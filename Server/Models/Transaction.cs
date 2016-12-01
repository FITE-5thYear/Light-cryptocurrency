using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    class Transaction
    {
        public int Id { get; set; }
        public int Amount { get; set; }

        public int SenderId { get; set; }
        public int ReciverId { get; set; }

        public virtual Client Sender { get; set; }
        public virtual Client Reciver{ get; set; }
    }
}
