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
    public class SqlCirculationRepository : ICirculationRepository
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;



        // Create a new borrowing transaction
        public int CreateTransaction(BorrowTransaction transaction)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlTransaction sqlTrans = conn.BeginTransaction();

                try
                {
                    string insertQuery = @"INSERT INTO BorrowingTransactions (MemberID, CopyID, BookID, BorrowDate, DueDate) 
                                   VALUES (@mid, @cid, @bid, @bdate, @ddate)";

                    SqlCommand cmdInsert = new SqlCommand(insertQuery, conn, sqlTrans);

                    cmdInsert.Parameters.AddWithValue("@mid", transaction.MemberId);
                    cmdInsert.Parameters.AddWithValue("@cid", transaction.CopyId ?? (object)DBNull.Value);
                    cmdInsert.Parameters.AddWithValue("@bid", transaction.BookId);
                    cmdInsert.Parameters.AddWithValue("@bdate", transaction.BorrowDate);
                    cmdInsert.Parameters.AddWithValue("@ddate", transaction.DueDate);

                    cmdInsert.ExecuteNonQuery();
                    string updateQuery = "UPDATE BookCopies SET Status = 'Borrowed' WHERE BookID = @bid";
                    SqlCommand cmdUpdate = new SqlCommand(updateQuery, conn, sqlTrans);
                    cmdUpdate.Parameters.AddWithValue("@bid", transaction.BookId);
                    int rows = cmdUpdate.ExecuteNonQuery();

                    sqlTrans.Commit();
                    return rows;
                }
                catch (Exception ex)
                {
                    sqlTrans.Rollback();
                    MessageBox.Show("Database Error: " + ex.Message);
                    return 0; 
                }
            }
        }




        // Update a borrowing transaction when a book is returned
        public bool ReturnBook(string copyId, string newCondition, out decimal fine)
        {
            fine = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString))
            {
    
                string selectQuery = "SELECT DueDate FROM BorrowingTransactions WHERE CopyID = @copyId AND ReturnDate IS NULL";
                SqlCommand selectCmd = new SqlCommand(selectQuery, conn);
                selectCmd.Parameters.AddWithValue("@copyId", copyId);

                conn.Open();
                object result = selectCmd.ExecuteScalar();

                if (result != null)
                {
                    DateTime dueDate = (DateTime)result;

   
                    if (DateTime.Now > dueDate)
                    {
                        fine += (DateTime.Now - dueDate).Days * 5.00m;
                    }
                    if (newCondition.Contains("Damaged")) fine += 20.00m;
                    if (newCondition.Contains("Lost")) fine += 50.00m;

     
                    string updateQuery = @"
                UPDATE BookCopies 
                SET Status = 'Available', 
                    Condition = @newCondition 
                WHERE CopyID = @copyId;

                UPDATE BorrowingTransactions 
                SET ReturnDate = GETDATE() 
                WHERE CopyID = @copyId AND ReturnDate IS NULL;";

                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@copyId", copyId);
                    updateCmd.Parameters.AddWithValue("@newCondition", newCondition);
                    updateCmd.ExecuteNonQuery();

                    return true;
                }
            }
            return false;
        }







        public bool ProcessReturn(string copyId, string newCondition, string targetStatus, decimal fine)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    string updateTxQuery = @"UPDATE BorrowingTransactions 
                        SET ReturnDate = GETDATE(), 
                            ConditionOnReturn = @cond,
                            FineAmount = @fine 
                        WHERE CopyID = @cid AND ReturnDate IS NULL";

                    SqlCommand cmdTx = new SqlCommand(updateTxQuery, conn, transaction);
                    cmdTx.Parameters.AddWithValue("@condition", newCondition);
                    cmdTx.Parameters.AddWithValue("@cid", copyId);
                    cmdTx.Parameters.AddWithValue("@fine", fine);
                    cmdTx.ExecuteNonQuery();
                    string updateBookQuery = @"UPDATE BookCopies 
                                      SET Status = 'Available', 
                                          Condition = @condition 
                                      WHERE CopyID = @cid";

                    SqlCommand cmdBook = new SqlCommand(updateBookQuery, conn, transaction);
                    cmdBook.Parameters.AddWithValue("@condition", newCondition);
                    cmdBook.Parameters.AddWithValue("@cid", copyId);
                    cmdBook.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }



        public BorrowTransaction GetActiveLoanByBookId(int bookId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {

                string query = @"SELECT t.CopyID, t.DueDate, c.Condition 
                 FROM BorrowingTransactions t
                 JOIN BookCopies c ON t.CopyID = c.CopyID
                 WHERE t.BookID = @bid AND t.ReturnDate IS NULL";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@bid", bookId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BorrowTransaction
                        {
                            CopyId = reader["CopyID"].ToString(),
                            DueDate = (DateTime)reader["DueDate"]
                            
                        };
                    }
                }
            }
            return null;
        }






















    }
}