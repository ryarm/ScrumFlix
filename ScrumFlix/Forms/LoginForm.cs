using ScrumFlix.Data;
using ScrumFlix.Models;
using System;
using System.Linq;
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
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter a username and password.");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var user = db.Users
                        .FirstOrDefault(u => u.UserName == username && u.UserPassword == password);

                    if (user == null)
                    {
                        MessageBox.Show("Invalid username or password.");
                        return;
                    }

                    if (user != null)
                    {
                        Session.UserId = user.UserId;
                        Session.UserName = user.UserName;
                        Session.RoleId = user.RoleId;

                        if (user.RoleId == 1 || user.RoleId == 2)
                        {
                            db.AuditLog.Add(new AuditLog
                            {
                                UserId = user.UserId,
                                ActionType = "LOGIN",
                                TableName = "Users",
                                ObjectId = user.UserId,
                                ActionTime = DateTime.Now,
                                Description = $"User '{user.UserName}' logged in",
                                OldValues = null,
                                NewValues = null
                            });

                            db.SaveChanges();
                            MainForm mainForm = new MainForm();
                            mainForm.Show();
                            this.Hide();
                        }
                        else if (user.RoleId == 3)
                        {
                            db.AuditLog.Add(new AuditLog
                            {
                                UserId = user.UserId,
                                ActionType = "LOGIN",
                                TableName = "Users",
                                ObjectId = user.UserId,
                                ActionTime = DateTime.Now,
                                Description = $"User '{user.UserName}' logged in",
                                OldValues = null,
                                NewValues = null
                            });

                            db.SaveChanges();
                            EmployeeMenu EmployeeMenu = new EmployeeMenu();
                            EmployeeMenu.Show();
                            this.Hide();
                        }
                    }
                    else
                    {
                        MessageBox.Show("This user has an invalid role.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login error: " + ex.Message);
            }
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}