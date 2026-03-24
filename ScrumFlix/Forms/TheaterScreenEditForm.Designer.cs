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
            numCapacity = new NumericUpDown();
            labelCapacity = new Label();
            ((System.ComponentModel.ISupportInitialize)numCapacity).BeginInit();
            SuspendLayout();
            // 
            // screenName
            // 
            screenName.Location = new Point(12, 12);
            screenName.Name = "screenName";
            screenName.PlaceholderText = "Screen Name";
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
            // numCapacity
            // 
            numCapacity.Location = new Point(12, 68);
            numCapacity.Name = "numCapacity";
            numCapacity.Size = new Size(156, 29);
            numCapacity.TabIndex = 4;
            // 
            // labelCapacity
            // 
            labelCapacity.AutoSize = true;
            labelCapacity.Location = new Point(12, 44);
            labelCapacity.Name = "labelCapacity";
            labelCapacity.Size = new Size(69, 21);
            labelCapacity.TabIndex = 5;
            labelCapacity.Text = "Capacity";
            // 
            // TheaterScreenEditForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(435, 107);
            Controls.Add(labelCapacity);
            Controls.Add(numCapacity);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(screenLocation);
            Controls.Add(screenName);
            Name = "TheaterScreenEditForm";
            Text = "TheaterScreenEditForm";
            ((System.ComponentModel.ISupportInitialize)numCapacity).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox screenName;
        private ComboBox screenLocation;
        private Button btnOk;
        private Button btnCancel;
        private NumericUpDown numCapacity;
        private Label labelCapacity;
    }
}