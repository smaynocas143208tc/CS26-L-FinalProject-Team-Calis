using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace Library_Management_System.Data
{
    public class SqlBookRepository : IBookRepository
    {
        private string connString = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;



        // Save a new book to the database
        public bool SaveBook(PhysicalBook book)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string query = "INSERT INTO Books (BookID, Title, Author, ISBN, Pages, ResourceType, Published) " +
                                   "VALUES (@id, @title, @author, @isbn, @pages, @type, @published)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", book.BookId);
                    cmd.Parameters.AddWithValue("@title", book.BookTitle);
                    cmd.Parameters.AddWithValue("@author", book.Author);
                    cmd.Parameters.AddWithValue("@isbn", book.ISBN);
                    cmd.Parameters.AddWithValue("@pages", book.NumberOfPages);
                    cmd.Parameters.AddWithValue("@type", book.ResourceType);
                    cmd.Parameters.AddWithValue("@published", book.PublishedDate);

                    cmd.ExecuteNonQuery();

                    string copyQuery = "INSERT INTO BookCopies (CopyID, BookID, Status, Condition) " +
                                       "VALUES (@cid, @bid, 'Available', 'Good')";

                    SqlCommand copyCmd = new SqlCommand(copyQuery, conn);
                    copyCmd.Parameters.AddWithValue("@cid", book.ISBN + "-A");
                    copyCmd.Parameters.AddWithValue("@bid", book.BookId);

                    copyCmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Error: Either the Book ID or the ISBN is already used by another book!", "Duplicate Error");
                }
                else
                {
                    MessageBox.Show("Database error: " + ex.Message);
                }
                return false;
            }
        }




        // Get the next available BookID
        public int GetNextBookId()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT ISNULL(MAX(BookID), 0) + 1 FROM Books";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }



        //Search for books by Title, Author, or ISBN
        public List<PhysicalBook> SearchBooks(string searchText)
        {
            List<PhysicalBook> bookList = new List<PhysicalBook>();
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string query = @"SELECT b.BookID, b.Title, b.Author, b.ISBN, b.Published, b.Pages, b.ResourceType, c.Status 
                         FROM Books b 
                         INNER JOIN BookCopies c ON b.BookID = c.BookID 
                         WHERE b.IsDeleted = 0 
                         AND (b.Title LIKE @search OR b.Author LIKE @search OR b.ISBN LIKE @search)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@search", "%" + searchText + "%"); 

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bookList.Add(new PhysicalBook
                        {
                            BookId = (int)reader["BookID"],
                            BookTitle = reader["Title"].ToString(),
                            Author = reader["Author"].ToString(),
                            ISBN = reader["ISBN"].ToString(), 
                            PublishedDate = reader["Published"].ToString(),
                            NumberOfPages = (int)reader["Pages"],
                            ResourceType = reader["ResourceType"].ToString(),
                            Status = reader["Status"].ToString() 
                        });
                    }
                }
            }
            return bookList;
        }

          



        // Update existing book details
        public int UpdateBook(PhysicalBook book)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    int totalRowsAffected = 0;

                    string bookQuery = @"UPDATE Books SET 
                                Title = @title, Author = @author, ISBN = @isbn, 
                                Pages = @pages, ResourceType = @type, Published = @published
                                WHERE BookID = @id";

                    SqlCommand bookCmd = new SqlCommand(bookQuery, conn);
                    bookCmd.Parameters.AddWithValue("@id", book.BookId);
                    bookCmd.Parameters.AddWithValue("@title", book.BookTitle);
                    bookCmd.Parameters.AddWithValue("@author", book.Author);
                    bookCmd.Parameters.AddWithValue("@isbn", book.ISBN);
                    bookCmd.Parameters.AddWithValue("@pages", book.NumberOfPages);
                    bookCmd.Parameters.AddWithValue("@type", book.ResourceType);
                    bookCmd.Parameters.AddWithValue("@published", book.PublishedDate);

                    totalRowsAffected += bookCmd.ExecuteNonQuery();


                    string copyQuery = @"UPDATE BookCopies SET Status = @status WHERE BookID = @id";

                    SqlCommand copyCmd = new SqlCommand(copyQuery, conn);
                    copyCmd.Parameters.AddWithValue("@id", book.BookId);
                    copyCmd.Parameters.AddWithValue("@status", (object)book.Status ?? "Available");

                    totalRowsAffected += copyCmd.ExecuteNonQuery();

                    return totalRowsAffected;
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    throw new Exception("Duplicate ISBN found.");
                }
                throw;
            }
        }




        // Delete a book by its ID
        public int DeleteBook(int bookId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "UPDATE Books SET IsDeleted = 1 WHERE BookID = @id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", bookId);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                return 0;
            }
        }



        // Get counts of books by their ResourceType
        public Dictionary<string, int> GetBookCountsByType()
        {
            var counts = new Dictionary<string, int>
    {
        { "Physical", 0 },
        { "EBook", 0 },
        { "Thesis Book", 0 }
    };

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"SELECT ResourceType, COUNT(*) as Total 
                             FROM Books 
                             WHERE IsDeleted = 0 
                             GROUP BY ResourceType";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string type = reader["ResourceType"].ToString();
                            int total = Convert.ToInt32(reader["Total"]);

                            if (counts.ContainsKey(type)) counts[type] = total;
                        }
                    }
                }
            }
            catch (SqlException) {  }

            return counts;
        }



        // Get a book by its ID
        public PhysicalBook GetBookById(int bookId)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString))
            {
                string query = @"SELECT b.BookID, b.Title, b.ResourceType, c.Status, c.CopyID 
                         FROM Books b 
                         LEFT JOIN BookCopies c ON b.BookID = c.BookID 
                         WHERE b.BookID = @id AND b.IsDeleted = 0";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", bookId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new PhysicalBook
                        {
                            BookId = (int)reader["BookID"],
                            BookTitle = reader["Title"].ToString(),
                            ResourceType = reader["ResourceType"].ToString(),
                            CopyID = reader["CopyID"].ToString(),
                            Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : "No Copy Registered"
                        };
                    }
                }
            }
            return null;
        }




















    }
}
