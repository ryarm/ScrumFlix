namespace ScrumFlix.Forms
{
    partial class EmployeeMenu
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
            btnTickets = new Button();
            btnConcessions = new Button();
            btnLogout = new Button();
            SuspendLayout();
            // 
            // btnTickets
            // 
            btnTickets.Location = new Point(195, 142);
            btnTickets.Name = "btnTickets";
            btnTickets.Size = new Size(120, 30);
            btnTickets.TabIndex = 0;
            btnTickets.Text = "Ticket Sales";
            btnTickets.UseVisualStyleBackColor = true;
            btnTickets.Click += btnTickets_Click;
            // 
            // btnConcessions
            // 
            btnConcessions.Location = new Point(418, 142);
            btnConcessions.Name = "btnConcessions";
            btnConcessions.Size = new Size(147, 30);
            btnConcessions.TabIndex = 1;
            btnConcessions.Text = "Concession Sales";
            btnConcessions.UseVisualStyleBackColor = true;
            btnConcessions.Click += btnConcessions_Click;
            // 
            // btnLogout
            // 
            btnLogout.Location = new Point(319, 225);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(98, 30);
            btnLogout.TabIndex = 2;
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click;
            // 
            // EmployeeMenu
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnLogout);
            Controls.Add(btnConcessions);
            Controls.Add(btnTickets);
            Name = "EmployeeMenu";
            Text = "EmployeeMenu";
            ResumeLayout(false);
        }

        #endregion

        private Button btnTickets;
        private Button btnConcessions;
        private Button btnLogout;
    }
}