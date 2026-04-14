using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ScrumFlix.Data;
using ScrumFlix.Models;

namespace ScrumFlix.Forms
{
    public partial class UserEditForm : Form
    {
        public User User { get; private set; } = new User();

        private List<Employee> _employees = new List<Employee>();

        private List<Role> _roles = new List<Role>();

        public UserEditForm()
        {
            InitializeComponent();
            LoadEmployees();
        }

        public UserEditForm(User existing) : this()
        {
            User = new User
            {
                UserId = existing.UserId,
                EmployeeId = existing.EmployeeId,
                UserName = existing.UserName,
                UserPassword = existing.UserPassword,
                RoleId = existing.RoleId
            };

            txtUsername.Text = User.UserName;
            txtPassword.Text = User.UserPassword;

            comboEmployees.SelectedValue = User.EmployeeId;
            comboRoles.SelectedValue = User.RoleId;
        }

        private void LoadEmployees()
        {
            using var db = new AppDbContext();

            _employees = db.Employees
                .OrderBy(e => e.FirstName)
                .ToList();

            _roles = db.Roles
                .OrderBy(e => e.RoleName)
                .ToList();

            comboEmployees.DataSource = _employees;
            comboEmployees.DisplayMember = "FullName";
            comboEmployees.ValueMember = "EmployeeId";

            comboRoles.DataSource = _roles;
            comboRoles.DisplayMember = "RoleName";
            comboRoles.ValueMember = "RoleId";
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username is required.");
                DialogResult = DialogResult.None;
                return;
            }

            if (password.Length > 20)
            {
                MessageBox.Show("Password must be 20 characters or less.");
                DialogResult = DialogResult.None;
                return;
            }

            if (comboEmployees.SelectedValue == null)
            {
                MessageBox.Show("Please select an employee.");
                DialogResult = DialogResult.None;
                return;
            }

            if (comboRoles.SelectedValue == null)
            {
                MessageBox.Show("Please select a role.");
                DialogResult = DialogResult.None;
                return;
            }

            User.UserName = username;
            User.UserPassword = password;
            User.EmployeeId = (int)comboEmployees.SelectedValue;
            User.RoleId = (int)comboRoles.SelectedValue;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}