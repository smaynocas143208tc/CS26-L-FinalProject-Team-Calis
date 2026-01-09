using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.BusinessLogic
{
    internal class CirculationManager
    {
        private readonly ICirculationRepository _repository;

        public CirculationManager(ICirculationRepository repository)
        {
            _repository = repository;
        }

       
        public bool BorrowBook(int memberId, int bookId, string copyId, DateTime dueDate)
        {
            var transaction = new BorrowTransaction
            {
                MemberId = memberId,
                BookId = bookId,
                CopyId = copyId, 
                BorrowDate = DateTime.Now,
                DueDate = dueDate

            };

      
            return _repository.CreateTransaction(transaction) > 0;
        }
    }
}

