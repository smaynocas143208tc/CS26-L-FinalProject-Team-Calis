using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Data
{
    public class SqlMemberRepository : IMemberRepository
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;



        // Add a new member to the database
        public void AddMember(Member m, string hashedPass)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "INSERT INTO Members (UserName, PasswordHash, UserRole, Name, Address, Email, Phone) VALUES (@uname, @pass, @urole, @name, @address, @email, @phone)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uname", m.UserName);
                cmd.Parameters.AddWithValue("@urole", m.UserRole);
                cmd.Parameters.AddWithValue("@pass", hashedPass);
                cmd.Parameters.AddWithValue("@name", m.Name);
                cmd.Parameters.AddWithValue("@address", m.Address);
                cmd.Parameters.AddWithValue("@email", m.Email);
                cmd.Parameters.AddWithValue("@phone", m.Phone);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }




        // Role-Based Login Verification
         public Member verifyLogin(string username, string plainPassword)
         {
             using (SqlConnection conn = new SqlConnection(connString))
             {
                 string query = "SELECT MemberID, UserName, UserRole, PasswordHash " +
                                "FROM Members " +
                                "WHERE UserName = @user COLLATE Latin1_General_BIN";

                 SqlCommand cmd = new SqlCommand(query, conn);
                 cmd.Parameters.AddWithValue("@user", username);

                 conn.Open();
                 using (SqlDataReader reader = cmd.ExecuteReader())
                 {
                     if (reader.Read())
                     {
                         string storedHash = reader["PasswordHash"].ToString();
                         if (BCrypt.Net.BCrypt.Verify(plainPassword, storedHash))
                         {
                             return new Member
                             {
                                 MemberID = (int)reader["MemberID"],
                                 UserName = reader["UserName"].ToString(),
                                 UserRole = reader["UserRole"].ToString()
                             };
                         }
                     }
                 }
             }
             return null;
         } 




        // +GetRegisteredMembers()
        public DataTable GetRegisteredMembers()
        {
            DataTable dt = new DataTable();
            string query = "SELECT MemberID, Name, Username, Address, Email, Phone, UserRole  FROM Members";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    try
                    {
                        adapter.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Database error: " + ex.Message);
                    }
                }
            }
            return dt;
        }




        // +GetNextMemberID()
        public string GetNextMemberID()
        {

            string query = "SELECT IDENT_CURRENT('Members') + 1";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : "1";
                }
                catch
                {
                    return "1";
                }
            }
        }





        // +GetMemberByID()
        public Member GetMemberByID(int id)
        {

            string query = "SELECT * FROM Members WHERE MemberID = @ID";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", id); 

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
             
                        return new Member
                        {
                            MemberID = Convert.ToInt32(reader["MemberID"]),
                            UserName = reader["UserName"].ToString(),
                            Name = reader["Name"].ToString(),
                            Address = reader["Address"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(), 
                            UserRole = reader["UserRole"].ToString()
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Search Error: " + ex.Message);
                }
            }
            return null; 
        }




        // +UpdateMember()
        public void UpdateMember(Member m, string newHashedPassword)
        {
          
            string query = @"UPDATE Members 
                     SET UserName = @User, 
                         Name = @Name, 
                         Address = @Addr, 
                         Email = @Email, 
                         Phone = @Phone, 
                         UserRole = @Role,
                         PasswordHash = @Pass 
                     WHERE MemberID = @ID";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@User", m.UserName);
                cmd.Parameters.AddWithValue("@Name", m.Name);
                cmd.Parameters.AddWithValue("@Addr", m.Address);
                cmd.Parameters.AddWithValue("@Email", m.Email);
                cmd.Parameters.AddWithValue("@Phone", m.Phone);
                cmd.Parameters.AddWithValue("@Role", m.UserRole);
                cmd.Parameters.AddWithValue("@Pass", newHashedPassword);
                cmd.Parameters.AddWithValue("@ID", m.MemberID);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Update Error: " + ex.Message);
                }
            }
        }




        // +RemoveMember()
        public void RemoveMember(int id)
        {
            string query = "DELETE FROM Members WHERE MemberID = @ID";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", id);

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("Member not found or already deleted.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Delete Error: " + ex.Message);
                }
            }
        }



        // +GetMemberByUsername()
        public Member GetMemberByUsername(string username)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
             
                string query = "SELECT * FROM Members WHERE UserName = @user  COLLATE Latin1_General_BIN ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user", username);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        
                        return new Member
                        {
                            MemberID = Convert.ToInt32(reader["MemberID"]),
                            UserName = reader["UserName"].ToString(),
                            Name = reader["Name"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            UserRole = reader["UserRole"].ToString()
                        };
                    }
                }
            }
            return null; 
        }



        // +GetAllMembers()
        public DataTable GetAllMembers()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT MemberID, Name, Username, Address, Email, Phone, UserRole FROM Members";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }































































    }
}
