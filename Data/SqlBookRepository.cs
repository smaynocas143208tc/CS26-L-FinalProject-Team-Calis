using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                    string query = "INSERT INTO Books (BookID, Title, Author, ISBN, Pages, Status, ResourceType, Published) " +
                                   "VALUES (@id, @title, @author, @isbn, @pages, @status, @type, @published)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", book.BookId);
                    cmd.Parameters.AddWithValue("@title", book.BookTitle);
                    cmd.Parameters.AddWithValue("@author", book.Author);
                    cmd.Parameters.AddWithValue("@isbn", book.ISBN);
                    cmd.Parameters.AddWithValue("@pages", book.NumberOfPages);
                    cmd.Parameters.AddWithValue("@status", book.Status);
                    cmd.Parameters.AddWithValue("@type", book.ResourceType);
                    cmd.Parameters.AddWithValue("@published", book.PublishedDate);

                    conn.Open();
                    cmd.ExecuteNonQuery();
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
                // This query finds the highest ID; if table is empty, it returns 0
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
                // This query checks all three columns for a match
                string query = "SELECT * FROM Books WHERE Title LIKE @search OR Author LIKE @search OR ISBN LIKE @search";

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
                            Status = reader["Status"].ToString(),
                            ResourceType = reader["ResourceType"].ToString()
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
                    string query = @"UPDATE Books SET 
                    Title = @title, Author = @author, ISBN = @isbn, 
                    Pages = @pages, Status = @status, ResourceType = @type, Published = @published 
                 WHERE BookID = @id AND (
                    Title <> @title OR Author <> @author OR ISBN <> @isbn OR 
                    Pages <> @pages OR Status <> @status OR ResourceType <> @type OR Published <> @published
                 )";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", book.BookId);
                    cmd.Parameters.AddWithValue("@title", book.BookTitle);
                    cmd.Parameters.AddWithValue("@author", book.Author);
                    cmd.Parameters.AddWithValue("@isbn", book.ISBN);
                    cmd.Parameters.AddWithValue("@pages", book.NumberOfPages);
                    cmd.Parameters.AddWithValue("@status", book.Status);
                    cmd.Parameters.AddWithValue("@type", book.ResourceType);
                    cmd.Parameters.AddWithValue("@published", book.PublishedDate);

                    conn.Open();
                    return cmd.ExecuteNonQuery(); // Returns the number of rows actually changed
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
                    // SQL query to remove the book based on its ID
                    string query = "DELETE FROM Books WHERE BookID = @id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", bookId);

                    conn.Open();
                    // Returns 1 if deleted, 0 if the ID wasn't found
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                // Handle cases where the book is currently borrowed (Foreign Key conflict)
                throw new Exception("Cannot delete this book. It may be linked to borrowing records.");
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
                    // This query groups books by type and counts them
                    string query = "SELECT ResourceType, COUNT(*) as Total FROM Books GROUP BY ResourceType";
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
            catch (SqlException) { /* Handle or log error */ }

            return counts;
        }
























    }
}
