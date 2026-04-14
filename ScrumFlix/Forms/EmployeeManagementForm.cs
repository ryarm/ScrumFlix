using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ScrumFlix.Data;
using ScrumFlix.Models;
using System;
using System.Linq;

namespace ScrumFlix.Forms
{
    public partial class EmployeeManagementForm : Form
    {
        public EmployeeManagementForm()
        {
            InitializeComponent();
        }

        private void EmployeeManagementForm_Load(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            using var context = new AppDbContext();

            var employees = context.Employees
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.MiddleName,
                    e.LastName,
                    e.DOB,
                    e.Phone,
                    e.Email,
                    e.Address
                })
                .ToList();

            gridEmployees.DataSource = employees;

            if (gridEmployees.Columns.Count > 0)
            {
                gridEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private int? GetSelectedEmployeeId()
        {
            if (gridEmployees.CurrentRow == null)
                return null;

            var value = gridEmployees.CurrentRow.Cells["EmployeeId"].Value;

            if (value == null)
                return null;

            if (int.TryParse(value.ToString(), out int employeeId))
                return employeeId;

            return null;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new EmployeeEditForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                using var context = new AppDbContext();

                context.Employees.Add(form.EmployeeRecord);
                context.SaveChanges();

                LoadEmployees();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var employeeId = GetSelectedEmployeeId();

            if (employeeId == null)
            {
                MessageBox.Show("Select an employee to edit.");
                return;
            }

            using var context = new AppDbContext();

            var employee = context.Employees.FirstOrDefault(e => e.EmployeeId == employeeId.Value);

            if (employee == null)
            {
                MessageBox.Show("Employee not found.");
                return;
            }

            using var form = new EmployeeEditForm(employee);

            if (form.ShowDialog() == DialogResult.OK)
            {
                employee.FirstName = form.EmployeeRecord.FirstName;
                employee.MiddleName = form.EmployeeRecord.MiddleName;
                employee.LastName = form.EmployeeRecord.LastName;
                employee.DOB = form.EmployeeRecord.DOB;
                employee.Phone = form.EmployeeRecord.Phone;
                employee.Email = form.EmployeeRecord.Email;
                employee.Address = form.EmployeeRecord.Address;

                context.SaveChanges();
                LoadEmployees();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var employeeId = GetSelectedEmployeeId();

            if (employeeId == null)
            {
                MessageBox.Show("Select an employee to delete.");
                return;
            }

            var confirm = MessageBox.Show(
                "Are you sure you want to delete this employee?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            using var context = new AppDbContext();

            var employee = context.Employees.FirstOrDefault(e => e.EmployeeId == employeeId.Value);

            if (employee == null)
            {
                MessageBox.Show("Employee not found.");
                return;
            }

            try
            {
                context.Employees.Remove(employee);
                context.SaveChanges();
                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete employee. They may be linked to another record.\n\n" + ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEmployees();
        }
    }
}