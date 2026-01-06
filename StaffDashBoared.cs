using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Library_Management_System
{
    public partial class StaffDashBoared : Form
    {
        public StaffDashBoared()
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

        private void AdminDashBoared_Load(object sender, EventArgs e)
        {

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
                // 2. Clear any session data (If you created a UserSession class)
                // UserSession.Clear(); 

                // 3. Show the Login Form
                // Assuming your login form is named 'LoginForm'
                LogIn login = new LogIn();
                login.Show();

                // 4. Close the current dashboard
                this.Close();


            }
        }
    }

}
