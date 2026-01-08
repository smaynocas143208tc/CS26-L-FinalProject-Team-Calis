using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;



namespace Library_Management_System.Models
{
    public class MemberManager
    {
        private string connString = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;

        // +RegisterMember()
        public void registerMember(Member member, string plainPassword)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "INSERT INTO Members (UserName, PasswordHash, UserRole, Name, Address, Email, Phone) VALUES (@uname, @pass, @urole, @name, @address, @email, @phone)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uname", member.UserName);
                cmd.Parameters.AddWithValue("@urole", member.UserRole);
                cmd.Parameters.AddWithValue("@pass", hashedPassword);
                cmd.Parameters.AddWithValue("@name", member.Name);
                cmd.Parameters.AddWithValue("@address", member.Address);
                cmd.Parameters.AddWithValue("@email", member.Email);
                cmd.Parameters.AddWithValue("@phone", member.Phone);

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
                cmd.Parameters.AddWithValue("@ID", id); // Use parameters to prevent SQL injection

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Create a member object and fill it with data from the database
                        return new Member
                        {
                            MemberID = Convert.ToInt32(reader["MemberID"]),
                            UserName = reader["UserName"].ToString(),
                            Name = reader["Name"].ToString(),
                            Address = reader["Address"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(), // Stored as string to avoid the Int32 error
                            UserRole = reader["UserRole"].ToString()
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Search Error: " + ex.Message);
                }
            }
            return null; // Return null if no member is found
        }



        // +UpdateMember()
        public void UpdateMember(Member m, string newHashedPassword)
        {
            // We add Password = @Pass to the SET clause
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
















    }

}
