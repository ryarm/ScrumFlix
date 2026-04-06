using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScrumFlix.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string password = txtLogin.Text;

            if (password == "a123")
            {
                MainForm adminForm = new MainForm();
                adminForm.Show();
                this.Hide();
            }
            else if (password == "e123")
            {
                EmployeeForm employeeForm = new EmployeeForm();
                employeeForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid password");
            }
        }
    }
}
