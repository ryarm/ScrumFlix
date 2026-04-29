using System;
using System.Linq;
using System.Windows.Forms;
using ScrumFlix.Data;
using ScrumFlix.Models;
using Microsoft.EntityFrameworkCore;

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
                .Include(e => e.Location)
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
                    e.Address,
                    e.PayRate,
                    Location = e.Location != null ? e.Location.LocationName : ""
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

                context.AuditLog.Add(new AuditLog
                {
                    UserId = Session.UserId,
                    ActionType = "ADD_EMPLOYEE",
                    TableName = "Employees",
                    ObjectId = form.EmployeeRecord.EmployeeId,
                    ActionTime = DateTime.Now,
                    Description = $"Added employee '{form.EmployeeRecord.FullName}'",
                    OldValues = null,
                    NewValues = $"FirstName={form.EmployeeRecord.FirstName}, MiddleName={form.EmployeeRecord.MiddleName}, LastName={form.EmployeeRecord.LastName}, DOB={form.EmployeeRecord.DOB}, Phone={form.EmployeeRecord.Phone}, Email={form.EmployeeRecord.Email}, Address={form.EmployeeRecord.Address}, PayRate={form.EmployeeRecord.PayRate}, LocationId={form.EmployeeRecord.LocationId}"
                });

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
                var oldFirstName = employee.FirstName;
                var oldMiddleName = employee.MiddleName;
                var oldLastName = employee.LastName;
                var oldDOB = employee.DOB;
                var oldPhone = employee.Phone;
                var oldEmail = employee.Email;
                var oldAddress = employee.Address;
                var oldPayRate = employee.PayRate;
                var oldLocationId = employee.LocationId;

                employee.FirstName = form.EmployeeRecord.FirstName;
                employee.MiddleName = form.EmployeeRecord.MiddleName;
                employee.LastName = form.EmployeeRecord.LastName;
                employee.DOB = form.EmployeeRecord.DOB;
                employee.Phone = form.EmployeeRecord.Phone;
                employee.Email = form.EmployeeRecord.Email;
                employee.Address = form.EmployeeRecord.Address;
                employee.PayRate = form.EmployeeRecord.PayRate;
                employee.LocationId = form.EmployeeRecord.LocationId;

                context.AuditLog.Add(new AuditLog
                {
                    UserId = Session.UserId,
                    ActionType = "UPDATE_EMPLOYEE",
                    TableName = "Employees",
                    ObjectId = employee.EmployeeId,
                    ActionTime = DateTime.Now,
                    Description = $"Updated employee '{oldFirstName} {oldLastName}'",
                    OldValues = $"FirstName={oldFirstName}, MiddleName={oldMiddleName}, LastName={oldLastName}, DOB={oldDOB}, Phone={oldPhone}, Email={oldEmail}, Address={oldAddress}, PayRate={oldPayRate}, LocationId={oldLocationId}",
                    NewValues = $"FirstName={employee.FirstName}, MiddleName={employee.MiddleName}, LastName={employee.LastName}, DOB={employee.DOB}, Phone={employee.Phone}, Email={employee.Email}, Address={employee.Address}, PayRate={employee.PayRate}, LocationId={employee.LocationId}"
                });

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
                context.AuditLog.Add(new AuditLog
                {
                    UserId = Session.UserId,
                    ActionType = "DELETE_EMPLOYEE",
                    TableName = "Employees",
                    ObjectId = employee.EmployeeId,
                    ActionTime = DateTime.Now,
                    Description = $"Deleted employee '{employee.FullName}'",
                    OldValues = $"FirstName={employee.FirstName}, MiddleName={employee.MiddleName}, LastName={employee.LastName}, DOB={employee.DOB}, Phone={employee.Phone}, Email={employee.Email}, Address={employee.Address}, PayRate={employee.PayRate}, LocationId={employee.LocationId}",
                    NewValues = null
                });

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