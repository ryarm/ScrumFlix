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
            btnShowtimes = new Button();
            btnTheaterScreen = new Button();
            btnLocation = new Button();
            SuspendLayout();
            // 
            // btnMovies
            // 
            btnMovies.Location = new Point(115, 25);
            btnMovies.Name = "btnMovies";
            btnMovies.Size = new Size(171, 30);
            btnMovies.TabIndex = 0;
            btnMovies.Text = "Manage Movies";
            btnMovies.UseVisualStyleBackColor = true;
            btnMovies.Click += btnMovies_Click;
            // 
            // btnShowtimes
            // 
            btnShowtimes.Location = new Point(115, 61);
            btnShowtimes.Name = "btnShowtimes";
            btnShowtimes.Size = new Size(171, 30);
            btnShowtimes.TabIndex = 1;
            btnShowtimes.Text = "Manage Showtimes";
            btnShowtimes.UseVisualStyleBackColor = true;
            btnShowtimes.Click += btnShowtimes_Click;
            // 
            // btnTheaterScreen
            // 
            btnTheaterScreen.Location = new Point(115, 97);
            btnTheaterScreen.Name = "btnTheaterScreen";
            btnTheaterScreen.Size = new Size(171, 30);
            btnTheaterScreen.TabIndex = 2;
            btnTheaterScreen.Text = "Manage Screens";
            btnTheaterScreen.UseVisualStyleBackColor = true;
            btnTheaterScreen.Click += btnTheaterScreen_Click;
            // 
            // btnLocation
            // 
            btnLocation.Location = new Point(115, 133);
            btnLocation.Name = "btnLocation";
            btnLocation.Size = new Size(171, 30);
            btnLocation.TabIndex = 3;
            btnLocation.Text = "Manage Locations";
            btnLocation.UseVisualStyleBackColor = true;
            btnLocation.Click += button1_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(405, 211);
            Controls.Add(btnLocation);
            Controls.Add(btnTheaterScreen);
            Controls.Add(btnShowtimes);
            Controls.Add(btnMovies);
            Name = "MainForm";
            Text = "Form1";
            Load += MainForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnMovies;
        private Button btnShowtimes;
        private Button btnTheaterScreen;
        private Button btnLocation;
    }
}
