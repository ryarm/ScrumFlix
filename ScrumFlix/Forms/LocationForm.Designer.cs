namespace ScrumFlix.Forms
{
    partial class LocationForm
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
            gridLocations = new DataGridView();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            btnRefresh = new Button();
            txtName = new TextBox();
            txtAddress = new TextBox();
            chkIsActive = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)gridLocations).BeginInit();
            SuspendLayout();
            // 
            // gridLocations
            // 
            gridLocations.AllowUserToAddRows = false;
            gridLocations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridLocations.Location = new Point(12, 12);
            gridLocations.Name = "gridLocations";
            gridLocations.ReadOnly = true;
            gridLocations.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            gridLocations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridLocations.Size = new Size(675, 261);
            gridLocations.TabIndex = 0;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(12, 324);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(98, 30);
            btnAdd.TabIndex = 1;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(116, 324);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(98, 30);
            btnUpdate.TabIndex = 2;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(220, 324);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(98, 30);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(324, 324);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(98, 30);
            btnRefresh.TabIndex = 4;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // txtName
            // 
            txtName.Location = new Point(12, 279);
            txtName.Name = "txtName";
            txtName.PlaceholderText = "Location Name";
            txtName.Size = new Size(174, 29);
            txtName.TabIndex = 5;
            // 
            // txtAddress
            // 
            txtAddress.Location = new Point(192, 279);
            txtAddress.Name = "txtAddress";
            txtAddress.PlaceholderText = "Location Address";
            txtAddress.Size = new Size(292, 29);
            txtAddress.TabIndex = 6;
            // 
            // chkIsActive
            // 
            chkIsActive.AutoSize = true;
            chkIsActive.Location = new Point(490, 281);
            chkIsActive.Name = "chkIsActive";
            chkIsActive.Size = new Size(89, 25);
            chkIsActive.TabIndex = 7;
            chkIsActive.Text = "Is Active";
            chkIsActive.UseVisualStyleBackColor = true;
            // 
            // LocationForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(699, 366);
            Controls.Add(chkIsActive);
            Controls.Add(txtAddress);
            Controls.Add(txtName);
            Controls.Add(btnRefresh);
            Controls.Add(btnDelete);
            Controls.Add(btnUpdate);
            Controls.Add(btnAdd);
            Controls.Add(gridLocations);
            Name = "LocationForm";
            Text = "LocationForm";
            Load += LocationForm_Load;
            ((System.ComponentModel.ISupportInitialize)gridLocations).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView gridLocations;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnRefresh;
        private TextBox txtName;
        private TextBox txtAddress;
        private CheckBox chkIsActive;
    }
}