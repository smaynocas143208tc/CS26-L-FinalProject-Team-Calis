using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Models
{
    public class BorrowTransaction
    {
        public int TransactionId { get; set; }
 
        public int MemberId { get; set; }

        public int BookId { get; set; }
 
        public DateTime BorrowDate { get; set; }
 
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}

