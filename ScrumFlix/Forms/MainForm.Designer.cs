namespace ScrumFlix
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btnMovies = new Button();
            btnShowtimes = new Button();
            btnTheaterScreen = new Button();
            btnLocation = new Button();
            PicLogo = new PictureBox();
            btnUsers = new Button();
            btnConcessions = new Button();
            lblTitle = new Label();
            btnEmployees = new Button();
            lblStockAlert = new Label();
            ((System.ComponentModel.ISupportInitialize)PicLogo).BeginInit();
            SuspendLayout();
            // 
            // btnMovies
            // 
            btnMovies.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnMovies.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnMovies.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            btnMovies.Location = new Point(164, 264);
            btnMovies.Name = "btnMovies";
            btnMovies.Size = new Size(429, 105);
            btnMovies.TabIndex = 0;
            btnMovies.Text = "Manage Movies";
            btnMovies.UseVisualStyleBackColor = true;
            btnMovies.Click += btnMovies_Click;
            // 
            // btnShowtimes
            // 
            btnShowtimes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnShowtimes.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnShowtimes.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            btnShowtimes.Location = new Point(679, 264);
            btnShowtimes.Name = "btnShowtimes";
            btnShowtimes.Size = new Size(429, 105);
            btnShowtimes.TabIndex = 1;
            btnShowtimes.Text = "Manage Showtimes";
            btnShowtimes.UseVisualStyleBackColor = true;
            btnShowtimes.Click += btnShowtimes_Click;
            // 
            // btnTheaterScreen
            // 
            btnTheaterScreen.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnTheaterScreen.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnTheaterScreen.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            btnTheaterScreen.Location = new Point(164, 470);
            btnTheaterScreen.Name = "btnTheaterScreen";
            btnTheaterScreen.Size = new Size(429, 105);
            btnTheaterScreen.TabIndex = 2;
            btnTheaterScreen.Text = "Manage Screens";
            btnTheaterScreen.UseVisualStyleBackColor = true;
            btnTheaterScreen.Click += btnTheaterScreen_Click;
            // 
            // btnLocation
            // 
            btnLocation.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnLocation.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnLocation.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            btnLocation.Location = new Point(679, 470);
            btnLocation.Name = "btnLocation";
            btnLocation.Size = new Size(429, 105);
            btnLocation.TabIndex = 3;
            btnLocation.Text = "Manage Locations";
            btnLocation.UseVisualStyleBackColor = true;
            btnLocation.Click += button1_Click;
            // 
            // PicLogo
            // 
            PicLogo.BackColor = Color.Transparent;
            PicLogo.Image = (Image)resources.GetObject("PicLogo.Image");
            PicLogo.Location = new Point(-15, -29);
            PicLogo.Name = "PicLogo";
            PicLogo.Size = new Size(240, 278);
            PicLogo.SizeMode = PictureBoxSizeMode.Zoom;
            PicLogo.TabIndex = 4;
            PicLogo.TabStop = false;
            // 
            // btnUsers
            // 
            btnUsers.Location = new Point(272, 642);
            btnUsers.Name = "btnUsers";
            btnUsers.Size = new Size(193, 30);
            btnUsers.TabIndex = 5;
            btnUsers.Text = "Manage User Accounts";
            btnUsers.UseVisualStyleBackColor = true;
            btnUsers.Click += btnUsers_Click;
            // 
            // btnConcessions
            // 
            btnConcessions.Location = new Point(714, 648);
            btnConcessions.Name = "btnConcessions";
            btnConcessions.Size = new Size(189, 30);
            btnConcessions.TabIndex = 6;
            btnConcessions.Text = "Manage Concessions";
            btnConcessions.UseVisualStyleBackColor = true;
            btnConcessions.Click += btnConcessions_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Franklin Gothic Medium Cond", 47.808F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(467, 40);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(352, 104);
            lblTitle.TabIndex = 7;
            lblTitle.Text = "Welcome!";
            // 
            // btnEmployees
            // 
            btnEmployees.Location = new Point(935, 648);
            btnEmployees.Name = "btnEmployees";
            btnEmployees.Size = new Size(173, 30);
            btnEmployees.TabIndex = 8;
            btnEmployees.Text = "Manage Employees";
            btnEmployees.UseVisualStyleBackColor = true;
            btnEmployees.Click += btnEmployees_Click;
            // 
            // lblStockAlert
            // 
            lblStockAlert.AutoSize = true;
            lblStockAlert.BackColor = Color.Red;
            lblStockAlert.Location = new Point(1147, 26);
            lblStockAlert.Name = "lblStockAlert";
            lblStockAlert.Size = new Size(0, 21);
            lblStockAlert.TabIndex = 9;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Teal;
            ClientSize = new Size(1294, 732);
            Controls.Add(lblStockAlert);
            Controls.Add(btnEmployees);
            Controls.Add(lblTitle);
            Controls.Add(btnConcessions);
            Controls.Add(btnUsers);
            Controls.Add(PicLogo);
            Controls.Add(btnLocation);
            Controls.Add(btnTheaterScreen);
            Controls.Add(btnShowtimes);
            Controls.Add(btnMovies);
            Name = "MainForm";
            Text = "ScrumFlix";
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)PicLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnMovies;
        private Button btnShowtimes;
        private Button btnTheaterScreen;
        private Button btnLocation;
        private PictureBox PicLogo;
        private Button btnUsers;
        private Button btnConcessions;
        private Label lblTitle;
        private Button btnEmployees;
        private Label lblStockAlert;
    }
}
