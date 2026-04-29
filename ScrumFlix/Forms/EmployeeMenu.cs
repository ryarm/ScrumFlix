using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ScrumFlix.Data;

namespace ScrumFlix.Forms
{
    public partial class EmployeeMenu : Form
    {
        public EmployeeMenu()
        {
            InitializeComponent();
        }

        private void btnTickets_Click(object sender, EventArgs e)
        {
            EmployeeForm ticketForm = new EmployeeForm();
            ticketForm.Show();
            this.Hide();
        }

        private void btnConcessions_Click(object sender, EventArgs e)
        {
            EmployeeConcessionPOSForm concessionForm = new EmployeeConcessionPOSForm();
            concessionForm.Show();
            this.Hide();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Session.UserId = 0;
            Session.UserName = null;
            Session.RoleId = 0;

            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Hide();
        }

        private void EmployeeMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
