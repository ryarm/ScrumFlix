namespace ScrumFlix.Forms
{
    partial class MovieEditForm
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
            txtTitle = new TextBox();
            txtRating = new TextBox();
            numRuntime = new NumericUpDown();
            txtDescription = new TextBox();
            btnOk = new Button();
            btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)numRuntime).BeginInit();
            SuspendLayout();
            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(12, 12);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(130, 29);
            txtTitle.TabIndex = 0;
            // 
            // txtRating
            // 
            txtRating.Location = new Point(148, 12);
            txtRating.Name = "txtRating";
            txtRating.Size = new Size(130, 29);
            txtRating.TabIndex = 1;
            // 
            // numRuntime
            // 
            numRuntime.Location = new Point(284, 12);
            numRuntime.Name = "numRuntime";
            numRuntime.Size = new Size(156, 29);
            numRuntime.TabIndex = 2;
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(12, 47);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.ScrollBars = ScrollBars.Vertical;
            txtDescription.Size = new Size(428, 164);
            txtDescription.TabIndex = 3;
            // 
            // btnOk
            // 
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(238, 229);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(98, 30);
            btnOk.TabIndex = 4;
            btnOk.Text = "Ok";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(342, 229);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(98, 30);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // MovieEditForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(450, 271);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(txtDescription);
            Controls.Add(numRuntime);
            Controls.Add(txtRating);
            Controls.Add(txtTitle);
            Name = "MovieEditForm";
            Text = "MovieEditForm";
            ((System.ComponentModel.ISupportInitialize)numRuntime).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtTitle;
        private TextBox txtRating;
        private NumericUpDown numRuntime;
        private TextBox txtDescription;
        private Button btnOk;
        private Button btnCancel;
    }
}