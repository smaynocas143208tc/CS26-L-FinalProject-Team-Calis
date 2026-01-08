using Library_Management_System.BusinessLogic;
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
    public partial class BorrowBook : Form
    {

        private readonly CatalogManager _catalogManager;
        private readonly MemberManager _memberManager;

        public BorrowBook()
        {
            InitializeComponent();

            _memberManager = new MemberManager(new SqlMemberRepository());
            _catalogManager = new CatalogManager(new SqlBookRepository());
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }



        private void btnBorrow_Click(object sender, EventArgs e)
        {
            // 1. Basic Validation
            if (string.IsNullOrWhiteSpace(txtMemberID.Text) || string.IsNullOrWhiteSpace(txtBookID.Text))
            {
                MessageBox.Show("Please enter both Member ID and Book ID.");
                return;
            }

            try
            {
                // 2. Setup the Architecture (Dependency Injection)    
                ICirculationRepository repo = new SqlCirculationRepository();

                // Inject the repository into the Manager
                CirculationManager circulationManager = new CirculationManager(repo);

                // 3. Collect Data from UI
                int memberId = int.Parse(txtMemberID.Text);
                int bookId = int.Parse(txtBookID.Text);
                DateTime dueDate = dtpDueDate.Value;

                // 4. Execute the Borrowing Logic
                bool isSuccess = circulationManager.BorrowBook(memberId, bookId, dueDate);

                if (isSuccess)
                {
                    MessageBox.Show("Book borrowed successfully!");
                    ClearBorrowFields(); 
                }
                else
                {
                    MessageBox.Show("Failed to borrow book. It might already be borrowed.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }




        private void btnCheck_Click(object sender, EventArgs e)
        {
     
            // 1. Validation: Ensure both IDs are entered
            if (string.IsNullOrWhiteSpace(txtMemberID.Text) || string.IsNullOrWhiteSpace(txtBookID.Text))
            {
                MessageBox.Show("Please enter both Member ID and Book ID to verify.");
                return;
            }

            try
            {
                int memberId = int.Parse(txtMemberID.Text);
                int bookId = int.Parse(txtBookID.Text);

                // 2. Fetch Member Info
                var member = _memberManager.VerifyMember(memberId);
                bool memberValid = false;

                if (member != null)
                {
                    txtMemberName.Text = member.Name;
                    txtMemberName.ForeColor = Color.Green;
                    memberValid = true;
                }
                else
                {
                    txtMemberName.Text = "Member Not Found";
                    txtMemberName.ForeColor = Color.Red;
                }

                // 3. Fetch Book Info
                var book = _catalogManager.GetBookDetails(bookId);
                bool bookValid = false;

                if (book != null)
                {
                    txtBookTitle.Text = book.BookTitle;
                    txtBookType.Text = book.ResourceType;
                    txtStatus.Text = book.Status;

                    // Business Rule: Only allow 'Available' status
                    if (book.Status == "Available")
                    {
                        txtStatus.ForeColor = Color.Green;
                        bookValid = true;
                    }
                    else
                    {
                        txtStatus.ForeColor = Color.Red;
                        MessageBox.Show($"Book is currently {book.Status}. It cannot be borrowed.");
                    }
                }
                else
                {
                    lblBookTitle.Text = "Book Not Found";
                    txtStatus.Text = "-";
                    txtBookType.Text = "-";
                }

                // 4. Enable Borrow Button only if both are valid
                btnBorrow.Enabled = (memberValid && bookValid);
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter numeric IDs only.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }




        private void btnClearFields_Click(object sender, EventArgs e)
        {
            ClearBorrowFields();
        }
























        
        
        
        
        
        
        
        
        
        private void ClearBorrowFields()
        {
            txtMemberID.Clear();
            txtBookID.Clear();
            txtMemberName.Text = "";
            txtBookTitle.Text = "";
            txtBookType.Clear();
            txtStatus.Clear();
            dtpDueDate.Value = DateTime.Now;
        }


    }
    
}
