using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ScrumFlix.Models;
using ScrumFlix.Data;
using System.Linq;

namespace ScrumFlix.Forms
{
    public partial class PayPeriodForm : Form
    {
        public PayPeriodForm()
        {
            InitializeComponent();
        }

        private void PayPeriodForm_Load(object sender, EventArgs e)
        {
            dateStart.Format = DateTimePickerFormat.Short;
            dateEnd.Format = DateTimePickerFormat.Short;

            LoadPayPeriods();
        }

        private void LoadPayPeriods()
        {
            using var db = new AppDbContext();

            var payPeriods = db.PayPeriods
                .OrderBy(p => p.StartDate)
                .Select(p => new
                {
                    p.PayPeriodId,
                    p.StartDate,
                    p.EndDate
                })
                .ToList();

            gridPayPeriods.DataSource = payPeriods;

            if (gridPayPeriods.Columns.Count > 0)
            {
                gridPayPeriods.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }

            if (gridPayPeriods.Columns["PayPeriodId"] != null)
            {
                gridPayPeriods.Columns["PayPeriodId"].Visible = false;
            }
        }

        private int? GetSelectedPayPeriodId()
        {
            if (gridPayPeriods.CurrentRow == null)
                return null;

            var value = gridPayPeriods.CurrentRow.Cells["PayPeriodId"].Value;

            if (value == null)
                return null;

            if (int.TryParse(value.ToString(), out int payPeriodId))
                return payPeriodId;

            return null;
        }

        private bool DatesAreValid(DateTime start, DateTime end)
        {
            if (end < start)
            {
                MessageBox.Show("End date must be after or equal to start date.");
                return false;
            }

            return true;
        }

        private void gridPayPeriods_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var payPeriodId = GetSelectedPayPeriodId();

            if (payPeriodId == null)
                return;

            using var db = new AppDbContext();

            var payPeriod = db.PayPeriods.FirstOrDefault(p => p.PayPeriodId == payPeriodId.Value);

            if (payPeriod == null)
                return;

            dateStart.Value = payPeriod.StartDate;
            dateEnd.Value = payPeriod.EndDate;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DateTime start = dateStart.Value.Date;
            DateTime end = dateEnd.Value.Date;

            if (!DatesAreValid(start, end))
                return;

            using var db = new AppDbContext();

            var payPeriod = new PayPeriod
            {
                StartDate = start,
                EndDate = end
            };

            db.PayPeriods.Add(payPeriod);
            db.SaveChanges();

            db.AuditLog.Add(new AuditLog
            {
                UserId = Session.UserId,
                ActionType = "ADD_PAY_PERIOD",
                TableName = "PayPeriods",
                ObjectId = payPeriod.PayPeriodId,
                ActionTime = DateTime.Now,
                Description = $"Added pay period {start:d} - {end:d}",
                OldValues = null,
                NewValues = $"StartDate={payPeriod.StartDate}, EndDate={payPeriod.EndDate}"
            });

            db.SaveChanges();

            LoadPayPeriods();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var payPeriodId = GetSelectedPayPeriodId();

            if (payPeriodId == null)
            {
                MessageBox.Show("Select a pay period to update.");
                return;
            }

            DateTime start = dateStart.Value.Date;
            DateTime end = dateEnd.Value.Date;

            if (!DatesAreValid(start, end))
                return;

            using var db = new AppDbContext();

            var payPeriod = db.PayPeriods.FirstOrDefault(p => p.PayPeriodId == payPeriodId.Value);

            if (payPeriod == null)
            {
                MessageBox.Show("Pay period not found.");
                return;
            }

            var oldStart = payPeriod.StartDate;
            var oldEnd = payPeriod.EndDate;

            payPeriod.StartDate = start;
            payPeriod.EndDate = end;

            db.AuditLog.Add(new AuditLog
            {
                UserId = Session.UserId,
                ActionType = "UPDATE_PAY_PERIOD",
                TableName = "PayPeriods",
                ObjectId = payPeriod.PayPeriodId,
                ActionTime = DateTime.Now,
                Description = $"Updated pay period {oldStart:d} - {oldEnd:d}",
                OldValues = $"StartDate={oldStart}, EndDate={oldEnd}",
                NewValues = $"StartDate={payPeriod.StartDate}, EndDate={payPeriod.EndDate}"
            });

            db.SaveChanges();

            LoadPayPeriods();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var payPeriodId = GetSelectedPayPeriodId();

            if (payPeriodId == null)
            {
                MessageBox.Show("Select a pay period to delete.");
                return;
            }

            var confirm = MessageBox.Show(
                "Delete this pay period?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            using var db = new AppDbContext();

            var payPeriod = db.PayPeriods.FirstOrDefault(p => p.PayPeriodId == payPeriodId.Value);

            if (payPeriod == null)
            {
                MessageBox.Show("Pay period not found.");
                return;
            }

            bool hasTimesheets = db.Timesheets.Any(t => t.PayPeriodId == payPeriod.PayPeriodId);
            bool hasPayrolls = db.Payrolls.Any(p => p.PayPeriodId == payPeriod.PayPeriodId);

            if (hasTimesheets || hasPayrolls)
            {
                MessageBox.Show("This pay period is linked to timesheets or payroll records and cannot be deleted.");
                return;
            }

            db.AuditLog.Add(new AuditLog
            {
                UserId = Session.UserId,
                ActionType = "DELETE_PAY_PERIOD",
                TableName = "PayPeriods",
                ObjectId = payPeriod.PayPeriodId,
                ActionTime = DateTime.Now,
                Description = $"Deleted pay period {payPeriod.StartDate:d} - {payPeriod.EndDate:d}",
                OldValues = $"StartDate={payPeriod.StartDate}, EndDate={payPeriod.EndDate}",
                NewValues = null
            });

            db.PayPeriods.Remove(payPeriod);
            db.SaveChanges();

            LoadPayPeriods();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPayPeriods();
        }
    }
}