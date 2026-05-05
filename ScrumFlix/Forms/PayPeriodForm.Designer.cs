namespace ScrumFlix.Forms
{
    partial class PayPeriodForm
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
            gridPayPeriods = new DataGridView();
            dateStart = new DateTimePicker();
            dateEnd = new DateTimePicker();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            btnRefresh = new Button();
            ((System.ComponentModel.ISupportInitialize)gridPayPeriods).BeginInit();
            SuspendLayout();
            // 
            // gridPayPeriods
            // 
            gridPayPeriods.AllowUserToAddRows = false;
            gridPayPeriods.AllowUserToDeleteRows = false;
            gridPayPeriods.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridPayPeriods.Location = new Point(12, 12);
            gridPayPeriods.MultiSelect = false;
            gridPayPeriods.Name = "gridPayPeriods";
            gridPayPeriods.ReadOnly = true;
            gridPayPeriods.RowHeadersWidth = 53;
            gridPayPeriods.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridPayPeriods.Size = new Size(776, 362);
            gridPayPeriods.TabIndex = 0;
            gridPayPeriods.CellClick += gridPayPeriods_CellClick;
            // 
            // dateStart
            // 
            dateStart.Location = new Point(12, 380);
            dateStart.Name = "dateStart";
            dateStart.Size = new Size(260, 29);
            dateStart.TabIndex = 1;
            // 
            // dateEnd
            // 
            dateEnd.Location = new Point(278, 380);
            dateEnd.Name = "dateEnd";
            dateEnd.Size = new Size(260, 29);
            dateEnd.TabIndex = 2;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(12, 415);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(98, 30);
            btnAdd.TabIndex = 3;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(116, 415);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(98, 30);
            btnUpdate.TabIndex = 4;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(220, 415);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(98, 30);
            btnDelete.TabIndex = 5;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(324, 415);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(98, 30);
            btnRefresh.TabIndex = 6;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // PayPeriodForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnRefresh);
            Controls.Add(btnDelete);
            Controls.Add(btnUpdate);
            Controls.Add(btnAdd);
            Controls.Add(dateEnd);
            Controls.Add(dateStart);
            Controls.Add(gridPayPeriods);
            Name = "PayPeriodForm";
            Text = "PayPeriodForm";
            Load += PayPeriodForm_Load;
            ((System.ComponentModel.ISupportInitialize)gridPayPeriods).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView gridPayPeriods;
        private DateTimePicker dateStart;
        private DateTimePicker dateEnd;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnRefresh;
    }
}