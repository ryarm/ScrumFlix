namespace ScrumFlix.Forms
{
    partial class ManagerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagerForm));
            PicLogo = new PictureBox();
            btnLogout = new Button();
            btnReports = new Button();
            btnPayroll = new Button();
            btnSchedules = new Button();
            lblStaff = new Label();
            btnEmployees = new Button();
            lblTheatres = new Label();
            btnConcessions = new Button();
            btnShowtimes = new Button();
            btnDashboard = new Button();
            lblStockAlert = new Label();
            lblTitle = new Label();
            ((System.ComponentModel.ISupportInitialize)PicLogo).BeginInit();
            SuspendLayout();
            // 
            // PicLogo
            // 
            PicLogo.BackColor = Color.Transparent;
            PicLogo.Image = (Image)resources.GetObject("PicLogo.Image");
            PicLogo.Location = new Point(-28, -18);
            PicLogo.Name = "PicLogo";
            PicLogo.Size = new Size(240, 278);
            PicLogo.SizeMode = PictureBoxSizeMode.Zoom;
            PicLogo.TabIndex = 5;
            PicLogo.TabStop = false;
            // 
            // btnLogout
            // 
            btnLogout.Font = new Font("Microsoft Sans Serif", 17.855999F, FontStyle.Bold);
            btnLogout.Location = new Point(990, 12);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(190, 87);
            btnLogout.TabIndex = 15;
            btnLogout.Text = "Log Out";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click;
            // 
            // btnReports
            // 
            btnReports.Font = new Font("Microsoft Sans Serif", 17.855999F, FontStyle.Bold);
            btnReports.Location = new Point(291, 569);
            btnReports.Name = "btnReports";
            btnReports.Size = new Size(264, 87);
            btnReports.TabIndex = 19;
            btnReports.Text = "Reports";
            btnReports.UseVisualStyleBackColor = true;
            btnReports.Click += btnReports_Click;
            // 
            // btnPayroll
            // 
            btnPayroll.Font = new Font("Microsoft Sans Serif", 17.855999F, FontStyle.Bold);
            btnPayroll.Location = new Point(575, 569);
            btnPayroll.Name = "btnPayroll";
            btnPayroll.Size = new Size(264, 87);
            btnPayroll.TabIndex = 18;
            btnPayroll.Text = "Payroll";
            btnPayroll.UseVisualStyleBackColor = true;
            btnPayroll.Click += btnPayroll_Click;
            // 
            // btnSchedules
            // 
            btnSchedules.Font = new Font("Microsoft Sans Serif", 17.855999F, FontStyle.Bold);
            btnSchedules.Location = new Point(749, 359);
            btnSchedules.Name = "btnSchedules";
            btnSchedules.Size = new Size(183, 126);
            btnSchedules.TabIndex = 23;
            btnSchedules.Text = "Manage Schedules";
            btnSchedules.UseVisualStyleBackColor = true;
            btnSchedules.Click += btnSchedules_Click;
            // 
            // lblStaff
            // 
            lblStaff.AutoSize = true;
            lblStaff.Font = new Font("Segoe UI", 21.8879986F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblStaff.ForeColor = Color.White;
            lblStaff.Location = new Point(792, 170);
            lblStaff.Name = "lblStaff";
            lblStaff.Size = new Size(107, 51);
            lblStaff.TabIndex = 22;
            lblStaff.Text = "Staff";
            // 
            // btnEmployees
            // 
            btnEmployees.Font = new Font("Microsoft Sans Serif", 17.855999F, FontStyle.Bold);
            btnEmployees.Location = new Point(709, 224);
            btnEmployees.Name = "btnEmployees";
            btnEmployees.Size = new Size(260, 129);
            btnEmployees.TabIndex = 21;
            btnEmployees.Text = "Manage Employees";
            btnEmployees.UseVisualStyleBackColor = true;
            btnEmployees.Click += btnEmployees_Click;
            // 
            // lblTheatres
            // 
            lblTheatres.AutoSize = true;
            lblTheatres.Font = new Font("Segoe UI", 21.8879986F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTheatres.ForeColor = Color.White;
            lblTheatres.Location = new Point(279, 170);
            lblTheatres.Name = "lblTheatres";
            lblTheatres.Size = new Size(176, 51);
            lblTheatres.TabIndex = 29;
            lblTheatres.Text = "Theatres";
            // 
            // btnConcessions
            // 
            btnConcessions.Font = new Font("Microsoft Sans Serif", 17.855999F, FontStyle.Bold);
            btnConcessions.Location = new Point(279, 347);
            btnConcessions.Name = "btnConcessions";
            btnConcessions.Size = new Size(196, 105);
            btnConcessions.TabIndex = 28;
            btnConcessions.Text = "Manage Concessions";
            btnConcessions.UseVisualStyleBackColor = true;
            btnConcessions.Click += btnConcessions_Click;
            // 
            // btnShowtimes
            // 
            btnShowtimes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnShowtimes.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnShowtimes.Font = new Font("Microsoft Sans Serif", 17.855999F, FontStyle.Bold);
            btnShowtimes.Location = new Point(279, 236);
            btnShowtimes.Name = "btnShowtimes";
            btnShowtimes.Size = new Size(196, 105);
            btnShowtimes.TabIndex = 25;
            btnShowtimes.Text = "Manage Showtimes";
            btnShowtimes.UseVisualStyleBackColor = true;
            btnShowtimes.Click += btnShowtimes_Click;
            // 
            // btnDashboard
            // 
            btnDashboard.Font = new Font("Microsoft Sans Serif", 17.855999F, FontStyle.Bold);
            btnDashboard.Location = new Point(12, 607);
            btnDashboard.Name = "btnDashboard";
            btnDashboard.Size = new Size(190, 87);
            btnDashboard.TabIndex = 30;
            btnDashboard.Text = "Operations Dashboard";
            btnDashboard.UseVisualStyleBackColor = true;
            btnDashboard.Click += btnDashboard_Click;
            // 
            // lblStockAlert
            // 
            lblStockAlert.AutoSize = true;
            lblStockAlert.BackColor = Color.Red;
            lblStockAlert.Location = new Point(1012, 113);
            lblStockAlert.Name = "lblStockAlert";
            lblStockAlert.Size = new Size(0, 21);
            lblStockAlert.TabIndex = 31;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Franklin Gothic Medium Cond", 47.808F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(429, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(352, 104);
            lblTitle.TabIndex = 32;
            lblTitle.Text = "Welcome!";
            // 
            // ManagerForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Teal;
            ClientSize = new Size(1192, 706);
            Controls.Add(lblTitle);
            Controls.Add(lblStockAlert);
            Controls.Add(btnDashboard);
            Controls.Add(lblTheatres);
            Controls.Add(btnConcessions);
            Controls.Add(btnShowtimes);
            Controls.Add(btnSchedules);
            Controls.Add(lblStaff);
            Controls.Add(btnEmployees);
            Controls.Add(btnReports);
            Controls.Add(btnPayroll);
            Controls.Add(btnLogout);
            Controls.Add(PicLogo);
            Name = "ManagerForm";
            Text = "ManagerForm";
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            ((System.ComponentModel.ISupportInitialize)PicLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox PicLogo;
        private Button btnLogout;
        private Button btnReports;
        private Button btnPayroll;
        private Button btnSchedules;
        private Label lblStaff;
        private Button btnEmployees;
        private Label lblTheatres;
        private Button btnConcessions;
        private Button btnShowtimes;
        private Button btnDashboard;
        private Label lblStockAlert;
        private Label lblTitle;
    }
}