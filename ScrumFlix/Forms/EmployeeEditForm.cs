using ScrumFlix.Data;
using ScrumFlix.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ScrumFlix.Forms
{
    public partial class EmployeeEditForm : Form
    {
        public Employee EmployeeRecord { get; private set; }

        public EmployeeEditForm()
        {
            InitializeComponent();
            EmployeeRecord = new Employee();
            LoadLocations();
        }

        public EmployeeEditForm(Employee employee)
        {
            InitializeComponent();

            EmployeeRecord = employee;
            LoadLocations();

            txtFirst.Text = employee.FirstName;
            txtMiddle.Text = employee.MiddleName;
            txtLast.Text = employee.LastName;
            dateDOB.Value = employee.DOB;
            txtPhone.Text = employee.Phone;
            txtEmail.Text = employee.Email;
            txtAddress.Text = employee.Address;
            txtPayRate.Text = employee.PayRate.ToString("F2");
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string first = txtFirst.Text.Trim();
            string middle = txtMiddle.Text.Trim();
            string last = txtLast.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();
            string address = txtAddress.Text.Trim();
            DateTime dob = dateDOB.Value.Date;

            if (string.IsNullOrWhiteSpace(first))
            {
                MessageBox.Show("First name is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(last))
            {
                MessageBox.Show("Last name is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Phone number is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Email is required.");
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Enter a valid email.");
                return;
            }

            using var context = new AppDbContext();

            bool emailExists = context.Employees.Any(e2 =>
                e2.Email == email &&
                e2.EmployeeId != EmployeeRecord.EmployeeId);

            if (emailExists)
            {
                MessageBox.Show("That email is already in use.");
                return;
            }
            if (!decimal.TryParse(txtPayRate.Text.Trim(), out decimal payRate) || payRate < 0)
            {
                MessageBox.Show("Enter a valid pay rate.");
                return;
            }

            EmployeeRecord.FirstName = first;
            EmployeeRecord.MiddleName = string.IsNullOrWhiteSpace(middle) ? null : middle;
            EmployeeRecord.LastName = last;
            EmployeeRecord.DOB = dob;
            EmployeeRecord.Phone = phone;
            EmployeeRecord.Email = email;
            EmployeeRecord.Address = string.IsNullOrWhiteSpace(address) ? null : address;
            EmployeeRecord.PayRate = payRate;
            EmployeeRecord.LocationId = Convert.ToInt32(comboLocation.SelectedValue);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void LoadLocations()
        {
            using var db = new AppDbContext();

            var locations = db.Location
                .OrderBy(l => l.LocationName)
                .ToList();

            comboLocation.DisplayMember = "LocationName";
            comboLocation.ValueMember = "LocationId";
            comboLocation.DataSource = locations;
        }
    }
}