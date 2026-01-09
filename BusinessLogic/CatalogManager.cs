using Library_Management_System.Data;
using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.BusinessLogic
{
    public class CatalogManager
    {
        private readonly IBookRepository _repository;

        // Dependency Injection 
        public CatalogManager(IBookRepository repository)
        {
            _repository = repository;
        }


        public bool AddBook(PhysicalBook book)
        {
            return _repository.SaveBook(book);
        }


        public int GetNextId()
        {
            return _repository.GetNextBookId();
        }


        public List<PhysicalBook> SearchBooks(string searchText)
        {
            return _repository.SearchBooks(searchText);
        }


        public int UpdateBook(PhysicalBook book)
        {
            return _repository.UpdateBook(book);
        }



        public Dictionary<string, int> GetBookCountsByType()
        {
            return _repository.GetBookCountsByType();
        }


        public PhysicalBook GetBookDetails(int id)
        {
            return _repository.GetBookById(id);
        }


        public int DeleteBook(int bookId)
        {

            return _repository.DeleteBook(bookId);
        }





    }
}
