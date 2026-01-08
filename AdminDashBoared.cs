using ReaLTaiizor.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System
{
    public partial class AdminDashBoared : Form
    {
        public AdminDashBoared()
        {
            InitializeComponent();
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);



        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (VerticalMenu.Width == 240)
            {
                VerticalMenu.Width = 70;
            }
            else
            {
                VerticalMenu.Width = 240;
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void AbrirFormEnPanel(object Formhijo)
        {
            //Para masigo og mo open ang mga forms sa sulod sa panel
            if (this.panelContainer.Controls.Count > 0)
                this.panelContainer.Controls.RemoveAt(0);
            Form fh = Formhijo as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            this.panelContainer.Controls.Add(fh);
            this.panelContainer.Tag = fh;
            fh.Show();
        }



        private void AdminDashBoared_Load(object sender, EventArgs e)
        {

            //Para mo show ang system overview na panel sa pag load sa admin dashboard

            // 1. Create the instance of your sub-form
            SystemOverview myChildForm = new SystemOverview();

            // 2. Prepare the form for embedding
            myChildForm.TopLevel = false;
            myChildForm.FormBorderStyle = FormBorderStyle.None;
            myChildForm.Dock = DockStyle.Fill;

            // 3. Add it to the panel and show it
            this.panelContainer.Controls.Add(myChildForm);
            myChildForm.Show();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Para mo show ang system overview na panel sa pag click sa system overview button
            AbrirFormEnPanel(new SystemOverview());
        }

        private void Container_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Para mo show ang search books na panel sa pag click sa search book button
            AbrirFormEnPanel(new SearchBooks());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 1. Ask for confirmation (Good UX)
            DialogResult result = MessageBox.Show("Are you sure you want to log out?",
                                                 "Logout",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {

                LogIn login = new LogIn();
                login.Show();

                this.Close();

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new ManageMembers());

        }

        private void btnManageBook_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new ManageBooks());
        }
    }

}

