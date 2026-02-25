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
            ((System.ComponentModel.ISupportInitialize)showtimesGrid).BeginInit();
            SuspendLayout();
            // 
            // screenCombo
            // 
            screenCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            screenCombo.FormattingEnabled = true;
            screenCombo.Location = new Point(267, 127);
            screenCombo.Name = "screenCombo";
            screenCombo.Size = new Size(158, 29);
            screenCombo.TabIndex = 0;
            screenCombo.SelectedIndexChanged += screenCombo_SelectedIndexChanged;
            // 
            // movieCombo
            // 
            movieCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            movieCombo.FormattingEnabled = true;
            movieCombo.Location = new Point(492, 127);
            movieCombo.Name = "movieCombo";
            movieCombo.Size = new Size(158, 29);
            movieCombo.TabIndex = 1;
            // 
            // startTimePicker
            // 
            startTimePicker.CustomFormat = "MM/dd/yyyy hh:mm tt";
            startTimePicker.Format = DateTimePickerFormat.Custom;
            startTimePicker.Location = new Point(390, 216);
            startTimePicker.Name = "startTimePicker";
            startTimePicker.Size = new Size(260, 29);
            startTimePicker.TabIndex = 2;
            // 
            // AddButton
            // 
            AddButton.Location = new Point(429, 320);
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
            showtimesGrid.Location = new Point(61, 388);
            showtimesGrid.MultiSelect = false;
            showtimesGrid.Name = "showtimesGrid";
            showtimesGrid.ReadOnly = true;
            showtimesGrid.RowHeadersWidth = 53;
            showtimesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            showtimesGrid.Size = new Size(878, 195);
            showtimesGrid.TabIndex = 4;
            // 
            // deleteButton
            // 
            deleteButton.Location = new Point(552, 320);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(135, 30);
            deleteButton.TabIndex = 5;
            deleteButton.Text = "Delete Selected";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += deleteButton_Click;
            // 
            // ShowtimesForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1043, 647);
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
        }

        #endregion

        private ComboBox screenCombo;
        private ComboBox movieCombo;
        private DateTimePicker startTimePicker;
        private Button AddButton;
        private DataGridView showtimesGrid;
        private Button deleteButton;
    }
}