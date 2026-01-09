using Library_Management_System.Data;
using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System
{
    public partial class ReturnBooks : Form
    {
        public ReturnBooks()
        {
            InitializeComponent();
            txtReturnStatus.Text = "Status: Stand By";
            txtReturnStatus.ForeColor = Color.Gray;
        }

        private void btnProcessReturn_Click(object sender, EventArgs e)
        {
            string copyId = txtCopyID.Text.Trim();

            if (string.IsNullOrEmpty(copyId))
            {
                MessageBox.Show("Please enter or scan a Copy ID.");
                return;
            }

            if (cmbCondition.SelectedItem == null)
            {
                MessageBox.Show("Please assess and select the book condition.");
                return;
            }

            string selectedCondition = cmbCondition.SelectedItem.ToString();
            string targetStatus = (selectedCondition == "Damaged") ? "Maintenance" : "Available";

            // Declare it ONCE here
            decimal fineToPay = 0;
            decimal.TryParse(txtFineAmount.Text, out fineToPay);

            try
            {
                ICirculationRepository repo = new SqlCirculationRepository();

                // Pass the variables to the repository
                bool success = repo.ProcessReturn(copyId, selectedCondition, targetStatus, fineToPay);

                if (success)
                {
                    // Update the status box
                    txtReturnStatus.Text = "Return Successful";
                    txtReturnStatus.ForeColor = Color.Green;

                    MessageBox.Show($"Book {copyId} returned successfully!");
                    ClearFields();
                }
                else
                {
                    txtReturnStatus.Text = "Status: Transaction Failed";
                    txtReturnStatus.ForeColor = Color.Red;
                    MessageBox.Show("No active borrowing record found.");
                }
            }
            catch (Exception ex)
            {
                txtReturnStatus.Text = "Status: Error Occurred";
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }






        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBookID.Text) || !int.TryParse(txtBookID.Text, out int bookId))
            {
                MessageBox.Show("Please enter a valid numeric Book ID.");
                return;
            }

            try
            {
               
                ICirculationRepository repo = new SqlCirculationRepository();

           
                var loan = repo.GetActiveLoanByBookId(bookId);

                if (loan != null)
                {
       
                    txtCopyID.Text = loan.CopyId;

             
                    dtpDueDate.Value = loan.DueDate;

             
                    DateTime today = DateTime.Now;
                    DateTime dueDate = loan.DueDate;

                    if (today > dueDate)
                    {
            
                        int lateDays = (today - dueDate).Days;

            
                        if (lateDays == 0 && today.Date > dueDate.Date) lateDays = 1;

                        decimal fineAmount = lateDays * 100; 
                        txtFineAmount.Text = fineAmount.ToString("N2");
                        txtFineAmount.Text = $"Book is {lateDays} day(s) overdue!";
                        txtFineAmount.ForeColor = Color.Red;
                    }
                    else
                    {
                        txtFineAmount.Text = "0.00";
                        txtFineAmount.Text = "Book is on time.";
                        txtFineAmount.ForeColor = Color.Green;
                    }
                }
                else
                {
                    MessageBox.Show("No active borrowing record found for this Book ID. The book might already be returned or was never borrowed.");
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching loan details: " + ex.Message);
            }
        }



















        private void ClearFields()
        {
            txtBookID.Clear();
            txtCopyID.Clear();
            txtFineAmount.Clear();
            cmbCondition.SelectedIndex = -1;
            txtReturnStatus.Text = "Status: Stand By";
            txtReturnStatus.ForeColor = Color.Gray;
        }

        private void btnClearFields_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
    }
}
