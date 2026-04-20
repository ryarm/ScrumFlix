namespace ScrumFlix.Forms
{
    partial class UserEditForm
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
            label1 = new Label();
            label2 = new Label();
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            btnSubmit = new Button();
            btnCancel = new Button();
            comboEmployees = new ComboBox();
            label3 = new Label();
            comboRoles = new ComboBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(107, 21);
            label1.TabIndex = 0;
            label1.Text = "Set Username";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(195, 9);
            label2.Name = "label2";
            label2.Size = new Size(102, 21);
            label2.TabIndex = 1;
            label2.Text = "Set Password";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(12, 33);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(130, 29);
            txtUsername.TabIndex = 2;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(195, 33);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(130, 29);
            txtPassword.TabIndex = 3;
            // 
            // btnSubmit
            // 
            btnSubmit.DialogResult = DialogResult.OK;
            btnSubmit.Location = new Point(193, 159);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(98, 30);
            btnSubmit.TabIndex = 4;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(297, 159);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(98, 30);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // comboEmployees
            // 
            comboEmployees.FormattingEnabled = true;
            comboEmployees.Location = new Point(12, 97);
            comboEmployees.Name = "comboEmployees";
            comboEmployees.Size = new Size(158, 29);
            comboEmployees.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 73);
            label3.Name = "label3";
            label3.Size = new Size(123, 21);
            label3.TabIndex = 7;
            label3.Text = "Select Employee";
            // 
            // comboRoles
            // 
            comboRoles.FormattingEnabled = true;
            comboRoles.Location = new Point(195, 97);
            comboRoles.Name = "comboRoles";
            comboRoles.Size = new Size(158, 29);
            comboRoles.TabIndex = 8;
            // 
            // UserEditForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(407, 201);
            Controls.Add(comboRoles);
            Controls.Add(label3);
            Controls.Add(comboEmployees);
            Controls.Add(btnCancel);
            Controls.Add(btnSubmit);
            Controls.Add(txtPassword);
            Controls.Add(txtUsername);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "UserEditForm";
            Text = "UserEditForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnSubmit;
        private Button btnCancel;
        private ComboBox comboEmployees;
        private Label label3;
        private ComboBox comboRoles;
    }
}