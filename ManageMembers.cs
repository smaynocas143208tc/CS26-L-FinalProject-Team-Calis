using Library_Management_System.BusinessLogic;
using Library_Management_System.Data;
using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System
{
    public partial class ManageMembers : Form
    {

        private MemberManager memberManager = new MemberManager(new SqlMemberRepository());

        public ManageMembers()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Member m = new Member
                {
                    UserName = txtUsername.Text,
                    UserRole = cmbUserrole.Text,
                    Name = txtName.Text,
                    Address = txtAddress.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text

                };

                memberManager.RegisterMember(m, txtPass.Text);
                MessageBox.Show("Registration Successful!");
                
                LoadMemberData();
                RefreshID();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void ManageMembers_Load(object sender, EventArgs e)
        {
            try
            {
                // Ask the manager for the table
                DataTable dt = memberManager.GetRegisteredMembers();

                // Show it in the grid
                dgvMember.DataSource = dt;

                // Optional: Make it look nice
                dgvMember.Columns["Name"].HeaderText = "MemberName";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            LoadMemberData();
            RefreshID();

        }



        private void button5_Click(object sender, EventArgs e)
        {
        
            ClearFields();
        
        }





         private void btnMemberSearch_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtSearchID.Text, out int id))
            {
                // Call the manager method
                Member m = memberManager.GetMemberByID(id);

                if (m != null)
                {
                    // Display the found data in your "Member Details" textboxes
                    txtMemberID.Text = m.MemberID.ToString();
                    txtUsername.Text = m.UserName;
                    txtName.Text = m.Name;
                    txtAddress.Text = m.Address;
                    txtEmail.Text = m.Email;
                    txtPhone.Text = m.Phone;
                    cmbUserrole.Text = m.UserRole;
                }
                else
                {
                    MessageBox.Show("No member found with ID: " + id);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid numeric ID.");
            }
        }




        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Basic Validation
                if (string.IsNullOrEmpty(txtPass.Text))
                {
                    MessageBox.Show("Please enter a password (new or old) to confirm changes.");
                    return;
                }

                // 2. Create the Member object
                Member updatedMember = new Member
                {
                    MemberID = int.Parse(txtMemberID.Text),
                    UserName = txtUsername.Text,
                    Name = txtName.Text,
                    Address = txtAddress.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text,
                    UserRole = cmbUserrole.Text
                };

                // 3. Hash the text from the password box
                // Use the same hashing method you used during Registration
                string hashedPass = BCrypt.Net.BCrypt.HashPassword(txtPass.Text);

                // 4. Send to Manager
                memberManager.UpdateMember(updatedMember, hashedPass);

                MessageBox.Show("Member and Password updated successfully!");
                LoadMemberData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void btnRemove_Click(object sender, EventArgs e)
        {
            // 1. Ensure a member is selected (MemberID textbox shouldn't be empty)
            if (string.IsNullOrEmpty(txtMemberID.Text))
            {
                MessageBox.Show("Please search for a member first.");
                return;
            }

            // 2. Ask for Confirmation
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to permanently delete this member?",
                "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int id = int.Parse(txtMemberID.Text);

                    // 3. Call the manager to delete
                    memberManager.RemoveMember(id);

                    MessageBox.Show("Member removed successfully.");

                    // 4. Clean up the UI
                    LoadMemberData(); // Refresh the grid
                    ClearFields();    // Empty the textboxes
                    RefreshID();  // Update the ID for the next registration
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }




















        private void LoadMemberData()
        {
            try
            {
                // 1. Get the data from the manager class as a DataTable
                DataTable dt = memberManager.GetRegisteredMembers();

                // 2. Point your DataGridView to that data
                dgvMember.DataSource = dt;

                // 3. Make the columns look professional
                if (dgvMember.Columns["UserName"] != null)
                    dgvMember.Columns["UserName"].HeaderText = "User Name";

                if (dgvMember.Columns["MemberID"] != null)
                    dgvMember.Columns["MemberID"].HeaderText = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data: " + ex.Message);
            }
        }


        private void RefreshID()
        {
            txtMemberID.Text = memberManager.GetNextMemberID();
        }

        private void ClearFields()
        {
            txtMemberID.Clear();
            txtUsername.Clear();
            cmbUserrole.SelectedIndex = -1;
            txtPass.Clear();
            txtName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtName.Focus();
        }



    }
}
