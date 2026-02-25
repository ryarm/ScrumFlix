namespace ScrumFlix
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnMovies = new Button();
            showtimes = new Button();
            SuspendLayout();
            // 
            // btnMovies
            // 
            btnMovies.Location = new Point(462, 282);
            btnMovies.Name = "btnMovies";
            btnMovies.Size = new Size(98, 30);
            btnMovies.TabIndex = 0;
            btnMovies.Text = "Movies";
            btnMovies.UseVisualStyleBackColor = true;
            btnMovies.Click += btnMovies_Click;
            // 
            // showtimes
            // 
            showtimes.Location = new Point(425, 318);
            showtimes.Name = "showtimes";
            showtimes.Size = new Size(171, 30);
            showtimes.TabIndex = 1;
            showtimes.Text = "Manage Showtimes";
            showtimes.UseVisualStyleBackColor = true;
            showtimes.Click += button1_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1038, 616);
            Controls.Add(showtimes);
            Controls.Add(btnMovies);
            Name = "MainForm";
            Text = "Form1";
            Load += MainForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnMovies;
        private Button showtimes;
    }
}
