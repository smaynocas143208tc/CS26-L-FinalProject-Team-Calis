using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Data
{
    public interface IBookRepository
    {
        bool SaveBook(PhysicalBook book);
        int GetNextBookId();
        List<PhysicalBook> SearchBooks(string searchText);
        int UpdateBook(PhysicalBook book);
        int DeleteBook(int bookId);
        Dictionary<string, int> GetBookCountsByType();
        PhysicalBook GetBookById(int id);

    }
}
