namespace ScrumFlix.Forms
{
    partial class EmployeeForm
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
            gridMovieShowtimes = new DataGridView();
            txtSearch = new TextBox();
            btnSearch = new Button();
            btnShowAll = new Button();
            btnSell = new Button();
            txtEmail = new TextBox();
            ((System.ComponentModel.ISupportInitialize)gridMovieShowtimes).BeginInit();
            SuspendLayout();
            // 
            // gridMovieShowtimes
            // 
            gridMovieShowtimes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridMovieShowtimes.Location = new Point(12, 12);
            gridMovieShowtimes.MultiSelect = false;
            gridMovieShowtimes.Name = "gridMovieShowtimes";
            gridMovieShowtimes.ReadOnly = true;
            gridMovieShowtimes.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            gridMovieShowtimes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridMovieShowtimes.Size = new Size(1628, 668);
            gridMovieShowtimes.TabIndex = 0;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(12, 699);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(130, 29);
            txtSearch.TabIndex = 1;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(148, 699);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(98, 30);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // btnShowAll
            // 
            btnShowAll.Location = new Point(252, 699);
            btnShowAll.Name = "btnShowAll";
            btnShowAll.Size = new Size(140, 30);
            btnShowAll.TabIndex = 3;
            btnShowAll.Text = "Show All Movies";
            btnShowAll.UseVisualStyleBackColor = true;
            btnShowAll.Click += btnShowAll_Click;
            // 
            // btnSell
            // 
            btnSell.Location = new Point(1542, 686);
            btnSell.Name = "btnSell";
            btnSell.Size = new Size(98, 30);
            btnSell.TabIndex = 4;
            btnSell.Text = "Sell Ticket";
            btnSell.UseVisualStyleBackColor = true;
            btnSell.Click += btnSell_Click;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(1207, 688);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(329, 29);
            txtEmail.TabIndex = 5;
            // 
            // EmployeeForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1729, 806);
            Controls.Add(txtEmail);
            Controls.Add(btnSell);
            Controls.Add(btnShowAll);
            Controls.Add(btnSearch);
            Controls.Add(txtSearch);
            Controls.Add(gridMovieShowtimes);
            Name = "EmployeeForm";
            Text = "EmployeeForm";
            FormClosed += EmployeeForm_FormClosed;
            Load += EmployeeForm_Load;
            ((System.ComponentModel.ISupportInitialize)gridMovieShowtimes).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView gridMovieShowtimes;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnShowAll;
        private Button btnSell;
        private TextBox txtEmail;
    }
}