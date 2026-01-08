using Library_Management_System.BusinessLogic;
using Library_Management_System.Data;
using Library_Management_System.Models;
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
    public partial class LogIn : Form
    {
        public LogIn()
        {
            InitializeComponent();
        }

        [DllImport("user32.DLL",EntryPoint ="ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {
            Register signUp = new Register();

            this.Hide();

            signUp.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            MemberManager manager = new MemberManager(new SqlMemberRepository());
            Member loggedInUser = manager.VerifyLogin(txtUsername.Text, txtPassword.Text);

            if (loggedInUser != null)
            {

                if (loggedInUser.UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    // Admins go to the Full Management Dashboard
                    AdminDashBoared adminDash = new AdminDashBoared();
                    adminDash.Show();
                }
                else if (loggedInUser.UserRole.Equals("Staff", StringComparison.OrdinalIgnoreCase))
                {
                    // Staff go to a restricted Overview Form
                    StaffDashBoared staffDash = new StaffDashBoared();
                    staffDash.Show();
                }

                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid Credentials.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }




                /* AdminDashBoared admin = new AdminDashBoared();

                this.Hide();

                admin.Show();*/
            }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
    }
  }
