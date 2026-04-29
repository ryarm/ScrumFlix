using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ScrumFlix.Data;
using ScrumFlix.Models;
using ScrumFlix.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace ScrumFlix.Forms
{
    public partial class UserManagementForm : Form
    {
        public UserManagementForm()
        {
            InitializeComponent();
        }

        private void gridUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new UserEditForm();

            if (form.ShowDialog() != DialogResult.OK)
                return;

            using var db = new AppDbContext();
            db.Users.Add(form.User);
            await db.SaveChangesAsync();

            db.AuditLog.Add(new AuditLog
            {
                UserId = Session.UserId,
                ActionType = "ADD_USER",
                TableName = "Users",
                ObjectId = form.User.UserId,
                ActionTime = DateTime.Now,
                Description = $"Added user '{form.User.UserName}'",
                OldValues = null,
                NewValues = $"UserName={form.User.UserName}, EmployeeId={form.User.EmployeeId}, RoleId={form.User.RoleId}"
            });

            await db.SaveChangesAsync();

            await LoadUsersAsync();
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            var selected = SelectedUser();
            if (selected is null)
            {
                MessageBox.Show("Select a user first please.");
                return;
            }

            using var form = new UserEditForm(selected);

            if (form.ShowDialog() != DialogResult.OK)
                return;

            using var db = new AppDbContext();
            var user = await db.Users.FindAsync(selected.UserId);
            if (user is null) return;

            var oldUserName = user.UserName;
            var oldPassword = user.UserPassword;
            var oldEmployeeId = user.EmployeeId;
            var oldRoleId = user.RoleId;

            user.UserName = form.User.UserName;
            user.UserPassword = form.User.UserPassword;
            user.EmployeeId = form.User.EmployeeId;
            user.RoleId = form.User.RoleId;

            db.AuditLog.Add(new AuditLog
            {
                UserId = Session.UserId,
                ActionType = "UPDATE_USER",
                TableName = "Users",
                ObjectId = user.UserId,
                ActionTime = DateTime.Now,
                Description = $"Updated user '{oldUserName}'",
                OldValues = $"UserName={oldUserName}, Password={oldPassword}, EmployeeId={oldEmployeeId}, RoleId={oldRoleId}",
                NewValues = $"UserName={user.UserName}, Password=hidden, EmployeeId={user.EmployeeId}, RoleId={user.RoleId}"
            });

            await db.SaveChangesAsync();
            await LoadUsersAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadUsersAsync();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var selected = SelectedUser();
            if (selected is null)
            {
                MessageBox.Show("Select a user first please.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Delete user '{selected.UserName}'?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            using var db = new AppDbContext();
            var user = await db.Users.FindAsync(selected.UserId);
            if (user is null) return;

            db.AuditLog.Add(new AuditLog
            {
                UserId = Session.UserId,
                ActionType = "DELETE_USER",
                TableName = "Users",
                ObjectId = user.UserId,
                ActionTime = DateTime.Now,
                Description = $"Deleted user '{user.UserName}'",
                OldValues = $"UserName={user.UserName}, EmployeeId={user.EmployeeId}, RoleId={user.RoleId}",
                NewValues = null
            });

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            await LoadUsersAsync();
        }

        private async void UserManagement_Load(object sender, EventArgs e)
        {
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            using var db = new AppDbContext();
            var users = await db.Users.OrderBy(m => m.UserName).ToListAsync();

            gridUsers.AutoGenerateColumns = true;
            gridUsers.DataSource = users;

            gridUsers.Columns[gridUsers.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private User? SelectedUser() // When selecting an item in the dataviewgrid this function returns that object
        {
            return gridUsers.CurrentRow?.DataBoundItem as User;
        }
    }
}
