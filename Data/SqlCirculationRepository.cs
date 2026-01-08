using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                    string insertQuery = @"INSERT INTO BorrowingTransactions (MemberID, BookID, BorrowDate, DueDate) 
                                       VALUES (@mid, @bid, @bdate, @ddate)";

                    SqlCommand cmdInsert = new SqlCommand(insertQuery, conn, sqlTrans);
                    cmdInsert.Parameters.AddWithValue("@mid", transaction.MemberId);
                    cmdInsert.Parameters.AddWithValue("@bid", transaction.BookId);
                    cmdInsert.Parameters.AddWithValue("@bdate", transaction.BorrowDate);
                    cmdInsert.Parameters.AddWithValue("@ddate", transaction.DueDate);
                    cmdInsert.ExecuteNonQuery();

                    string updateQuery = "UPDATE Books SET Status = 'Borrowed' WHERE BookID = @bid";
                    SqlCommand cmdUpdate = new SqlCommand(updateQuery, conn, sqlTrans);
                    cmdUpdate.Parameters.AddWithValue("@bid", transaction.BookId);
                    int rows = cmdUpdate.ExecuteNonQuery();

                    sqlTrans.Commit();
                    return rows;
                }
                catch
                {
                    sqlTrans.Rollback();
                    return 0;
                }
            }

        }













    }
}