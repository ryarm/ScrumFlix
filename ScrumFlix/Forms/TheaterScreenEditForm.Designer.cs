namespace ScrumFlix.Forms
{
    partial class TheaterScreenEditForm
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
            screenName = new TextBox();
            screenLocation = new ComboBox();
            btnOk = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // screenName
            // 
            screenName.Location = new Point(12, 12);
            screenName.Name = "screenName";
            screenName.Size = new Size(216, 29);
            screenName.TabIndex = 0;
            // 
            // screenLocation
            // 
            screenLocation.FormattingEnabled = true;
            screenLocation.Location = new Point(248, 12);
            screenLocation.Name = "screenLocation";
            screenLocation.Size = new Size(172, 29);
            screenLocation.TabIndex = 1;
            // 
            // btnOk
            // 
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(218, 65);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(98, 30);
            btnOk.TabIndex = 2;
            btnOk.Text = "Ok";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(322, 65);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(98, 30);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // TheaterScreenEditForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(435, 107);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(screenLocation);
            Controls.Add(screenName);
            Name = "TheaterScreenEditForm";
            Text = "TheaterScreenEditForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox screenName;
        private ComboBox screenLocation;
        private Button btnOk;
        private Button btnCancel;
    }
}