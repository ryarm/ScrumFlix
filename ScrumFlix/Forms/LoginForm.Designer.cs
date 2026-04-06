namespace ScrumFlix.Forms
{
    partial class LoginForm
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
            LoginLabel = new Label();
            txtLogin = new TextBox();
            btnLogin = new Button();
            SuspendLayout();
            // 
            // LoginLabel
            // 
            LoginLabel.AutoSize = true;
            LoginLabel.Location = new Point(209, 55);
            LoginLabel.Name = "LoginLabel";
            LoginLabel.Size = new Size(177, 21);
            LoginLabel.TabIndex = 0;
            LoginLabel.Text = "Enter password to login:";
            // 
            // txtLogin
            // 
            txtLogin.Location = new Point(209, 92);
            txtLogin.Name = "txtLogin";
            txtLogin.Size = new Size(130, 29);
            txtLogin.TabIndex = 1;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(222, 141);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(98, 30);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnLogin);
            Controls.Add(txtLogin);
            Controls.Add(LoginLabel);
            Name = "LoginForm";
            Text = "LoginForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LoginLabel;
        private TextBox txtLogin;
        private Button btnLogin;
    }
}