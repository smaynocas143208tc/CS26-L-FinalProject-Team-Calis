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
    public partial class ManageBooks : Form
    {

        private CatalogManager _catalog = new CatalogManager(new SqlBookRepository());

        public ManageBooks()
        {
            InitializeComponent();
        }

        // Add Book
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtISBN.Text) || string.IsNullOrWhiteSpace(cmbType.Text))
            {
                MessageBox.Show("Title, Type and ISBN are required!");
                return;
            }

            var book = new PhysicalBook
            {
                BookId = int.Parse(txtBookID.Text),
                BookTitle = txtTitle.Text,
                ResourceType = cmbType.SelectedItem?.ToString(),
                ISBN = txtISBN.Text,
                PublishedDate = string.IsNullOrWhiteSpace(txtPublished.Text) ? "Unknown" : txtPublished.Text,
                Author = string.IsNullOrWhiteSpace(txtAuthor.Text) ? "Unknown" : txtAuthor.Text,
                NumberOfPages = string.IsNullOrWhiteSpace(txtPages.Text) ? 0 : int.Parse(txtPages.Text),
            };

            bool isSuccess = _catalog.AddBook(book);

            if (isSuccess)
            {
 
                MessageBox.Show("Added Successfully!");
                ClearFields();
                RefreshNextId();
                RefreshStatistics();
            }
        }






        // Clear Fields
        private void button5_Click(object sender, EventArgs e)
        {
        
            ClearFields();
        
        }






        // Search Books
        private void btnMemberSearch_Click(object sender, EventArgs e)
        {
            string searchInput = txtSearchBook.Text;

            if (string.IsNullOrWhiteSpace(searchInput))
            {
                dgvSearchResults.Visible = false;
                btnCloseSearch.Visible = false;
                MessageBox.Show("Please enter a Title, Author, or ISBN to search.");
                return;
            }

   
            var results = _catalog.SearchBooks(searchInput);

            if (results.Count > 0)
            {
                if (results.Count > 1)
                {
                    dgvSearchResults.DataSource = results;

   
                    if (dgvSearchResults.Columns["IsDeleted"] != null)
                    {
                        dgvSearchResults.Columns["IsDeleted"].Visible = false;
                    }

                    dgvSearchResults.Visible = true;
                    btnCloseSearch.Visible = true;
                    dgvSearchResults.BringToFront();
                    btnCloseSearch.BringToFront();
                }
                else 
                {
                    dgvSearchResults.Visible = false;
                    btnCloseSearch.Visible = false;
                    DisplayBookDetails(results[0]); 
                }
            }
            else
            {
                dgvSearchResults.Visible = false;
                btnCloseSearch.Visible = false;
                MessageBox.Show("No books found matching your search.");
            }
        }








        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // 1. Validation: Ensure ID and Title are present
            if (string.IsNullOrWhiteSpace(txtBookID.Text) || string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtISBN.Text))
            {
                MessageBox.Show("BookID, Title and ISBN is required.");
                return;
            }

            // 2. Map data from TextBoxes to the Object
            var updatedBook = new PhysicalBook
            {
                BookId = int.Parse(txtBookID.Text),
                BookTitle = txtTitle.Text,
                Author = txtAuthor.Text,
                ISBN = txtISBN.Text,
                NumberOfPages = int.Parse(txtPages.Text),
                Status = cmbStatus.SelectedItem?.ToString() ?? "Available",
                ResourceType = cmbType.SelectedItem?.ToString(),
                PublishedDate = txtPublished.Text
            };

            // 3. Execute and Notify
            try
            {
                int rowsAffected = _catalog.UpdateBook(updatedBook);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Book details updated successfully!");
                    ClearFields(); // Reset the form after success
                    RefreshNextId();
                    RefreshStatistics();
                }
                else
                {
                    // Triggered if the ID exists but the user didn't change any text in the boxes
                    MessageBox.Show("Nothing changed.");
                }
            }
            catch (Exception ex)
            {
                // Catches duplicate ISBN errors or database connection issues
                MessageBox.Show("Error updating book: " + ex.Message);
            }
        }




        private void btnRemove_Click(object sender, EventArgs e)
        {
            // 1. Validation: Ensure an ID is present
            if (string.IsNullOrWhiteSpace(txtBookID.Text))
            {
                MessageBox.Show("Please select or enter a Book ID to remove.");
                return;
            }

            if (!int.TryParse(txtBookID.Text, out int idToRemove))
            {
                MessageBox.Show("Invalid Book ID format.");
                return;
            }

            DialogResult confirm = MessageBox.Show($"Are you sure you want to remove Book ID {idToRemove}?\n\nNote: This will hide the book from the catalog but preserve its borrowing history.",
                "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    // 3. Execute Soft Delete
                    // We changed this to call DeleteBook to match the IBookRepository interface
                    int rowsAffected = _catalog.DeleteBook(idToRemove);

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Book removed successfully!");
                        ClearFields();
                        RefreshNextId();
                        RefreshStatistics(); 
                    }
                    else
                    {
                        MessageBox.Show("Book ID not found or already removed.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


























        private void ClearFields()
        {
            txtBookID.Clear();
            txtPages.Clear();
            cmbType.SelectedIndex = -1;
            txtISBN.Clear();
            txtTitle.Clear();
            cmbStatus.SelectedIndex = -1;
            txtAuthor.Clear();
            txtPublished.Clear();
            txtTitle.Focus();
            RefreshNextId();
        }


        private void ManageBooks_Load(object sender, EventArgs e)
        {

            RefreshStatistics();
            RefreshNextId();

        }


        private void RefreshNextId()
        {
            int nextId = _catalog.GetNextId();
            txtBookID.Text = nextId.ToString();
        }


        private void RefreshStatistics()
        {
            var stats = _catalog.GetBookCountsByType();
            lblPhysicalBooks.Text = (stats.ContainsKey("Physical") ? stats["Physical"] : 0).ToString();
            lblEbook.Text = (stats.ContainsKey("EBook") ? stats["EBook"] : 0).ToString();
            lblThesesBook.Text = (stats.ContainsKey("Thesis Book") ? stats["Thesis Book"] : 0).ToString();
        }


        private void txtPublished_TextChanged(object sender, EventArgs e)
        {

        }


        private void txtPublished_Click(object sender, EventArgs e)
        {
            monthCalendar1.Visible = true;
            int centerX = (this.ClientSize.Width - monthCalendar1.Width) / 2;
            int centerY = (this.ClientSize.Height - monthCalendar1.Height) / 2;
            monthCalendar1.Location = new Point(txtPublished.Left, txtPublished.Bottom);
            monthCalendar1.BringToFront();
        }


        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtPublished.Text = e.Start.ToString("MMM d yyyy");

            monthCalendar1.Visible = false;

        }


        private void DisplayBookDetails(PhysicalBook book)
        {
            if (book == null) return;

            txtBookID.Text = book.BookId.ToString();
            txtTitle.Text = book.BookTitle;
            txtAuthor.Text = book.Author;
            txtISBN.Text = book.ISBN;
            txtPages.Text = book.NumberOfPages.ToString();
            txtPublished.Text = book.PublishedDate;
            cmbStatus.SelectedItem = book.Status;
            cmbType.SelectedItem = book.ResourceType;

            if (book.Status == "Borrowed")
            {
                txtTitle.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                txtTitle.ForeColor = System.Drawing.Color.Black;
            }
        }


        private void dgvSearchResults_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedBook = (PhysicalBook)dgvSearchResults.Rows[e.RowIndex].DataBoundItem;

                DisplayBookDetails(selectedBook);
                dgvSearchResults.Visible = false;
                btnCloseSearch.Visible = false;
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            dgvSearchResults.Visible = false;
            btnCloseSearch.Visible = false;
        }
    }
}
