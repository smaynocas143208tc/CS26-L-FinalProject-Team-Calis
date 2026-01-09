using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Models
{
    public interface ICirculationRepository
    {
        int CreateTransaction(BorrowTransaction transaction);
        bool ProcessReturn(string copyId, string condition, string targetStatus, decimal fine);
        BorrowTransaction GetActiveLoanByBookId(int bookId);
    }

}
