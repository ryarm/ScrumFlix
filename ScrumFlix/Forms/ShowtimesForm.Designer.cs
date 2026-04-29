namespace ScrumFlix.Forms
{
    partial class ShowtimesForm
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
            screenCombo = new ComboBox();
            movieCombo = new ComboBox();
            startTimePicker = new DateTimePicker();
            AddButton = new Button();
            showtimesGrid = new DataGridView();
            deleteButton = new Button();
            txtPricePerTicket = new TextBox();
            chkIsActive = new CheckBox();
            btnUpdatePrice = new Button();
            ((System.ComponentModel.ISupportInitialize)showtimesGrid).BeginInit();
            SuspendLayout();
            // 
            // screenCombo
            // 
            screenCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            screenCombo.FormattingEnabled = true;
            screenCombo.Location = new Point(12, 12);
            screenCombo.Name = "screenCombo";
            screenCombo.Size = new Size(158, 29);
            screenCombo.TabIndex = 0;
            screenCombo.SelectedIndexChanged += screenCombo_SelectedIndexChanged;
            // 
            // movieCombo
            // 
            movieCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            movieCombo.FormattingEnabled = true;
            movieCombo.Location = new Point(176, 12);
            movieCombo.Name = "movieCombo";
            movieCombo.Size = new Size(158, 29);
            movieCombo.TabIndex = 1;
            // 
            // startTimePicker
            // 
            startTimePicker.CustomFormat = "MM/dd/yyyy hh:mm tt";
            startTimePicker.Format = DateTimePickerFormat.Custom;
            startTimePicker.Location = new Point(340, 12);
            startTimePicker.Name = "startTimePicker";
            startTimePicker.Size = new Size(260, 29);
            startTimePicker.TabIndex = 2;
            // 
            // AddButton
            // 
            AddButton.Location = new Point(742, 11);
            AddButton.Name = "AddButton";
            AddButton.Size = new Size(98, 30);
            AddButton.TabIndex = 3;
            AddButton.Text = "Add showtime";
            AddButton.UseVisualStyleBackColor = true;
            AddButton.Click += AddButton_Click;
            // 
            // showtimesGrid
            // 
            showtimesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            showtimesGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            showtimesGrid.Location = new Point(12, 49);
            showtimesGrid.MultiSelect = false;
            showtimesGrid.Name = "showtimesGrid";
            showtimesGrid.ReadOnly = true;
            showtimesGrid.RowHeadersWidth = 53;
            showtimesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            showtimesGrid.Size = new Size(1178, 334);
            showtimesGrid.TabIndex = 4;
            // 
            // deleteButton
            // 
            deleteButton.Location = new Point(846, 12);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(135, 30);
            deleteButton.TabIndex = 5;
            deleteButton.Text = "Delete Selected";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += deleteButton_Click;
            // 
            // txtPricePerTicket
            // 
            txtPricePerTicket.Location = new Point(606, 12);
            txtPricePerTicket.Name = "txtPricePerTicket";
            txtPricePerTicket.PlaceholderText = "Ticket Price";
            txtPricePerTicket.Size = new Size(130, 29);
            txtPricePerTicket.TabIndex = 6;
            // 
            // chkIsActive
            // 
            chkIsActive.AutoSize = true;
            chkIsActive.Location = new Point(987, 17);
            chkIsActive.Name = "chkIsActive";
            chkIsActive.Size = new Size(89, 25);
            chkIsActive.TabIndex = 7;
            chkIsActive.Text = "Is Active";
            chkIsActive.UseVisualStyleBackColor = true;
            // 
            // btnUpdatePrice
            // 
            btnUpdatePrice.Location = new Point(1082, 13);
            btnUpdatePrice.Name = "btnUpdatePrice";
            btnUpdatePrice.Size = new Size(108, 30);
            btnUpdatePrice.TabIndex = 8;
            btnUpdatePrice.Text = "Update Price";
            btnUpdatePrice.UseVisualStyleBackColor = true;
            btnUpdatePrice.Click += btnUpdatePrice_Click;
            // 
            // ShowtimesForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 398);
            Controls.Add(btnUpdatePrice);
            Controls.Add(chkIsActive);
            Controls.Add(txtPricePerTicket);
            Controls.Add(deleteButton);
            Controls.Add(showtimesGrid);
            Controls.Add(AddButton);
            Controls.Add(startTimePicker);
            Controls.Add(movieCombo);
            Controls.Add(screenCombo);
            Name = "ShowtimesForm";
            Text = "ShowtimesForm";
            ((System.ComponentModel.ISupportInitialize)showtimesGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox screenCombo;
        private ComboBox movieCombo;
        private DateTimePicker startTimePicker;
        private Button AddButton;
        private DataGridView showtimesGrid;
        private Button deleteButton;
        private TextBox txtPricePerTicket;
        private CheckBox chkIsActive;
        private Button btnUpdatePrice;
    }
}