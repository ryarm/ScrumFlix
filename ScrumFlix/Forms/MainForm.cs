using ScrumFlix.Data;
using ScrumFlix.Forms;
using ScrumFlix.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ScrumFlix
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            using var db = new AppDbContext();

            if (!db.Movies.Any())
            {
                db.Movies.Add(new Movie
                {
                    Title = "Test Movie",
                    Rating = "PG",
                    RuntimeMinutes = 120
                });

                db.SaveChanges();
            }

            // int count = db.Movies.Count();
            // MessageBox.Show($"Movies in database: {count}");
        }
        private void btnMovies_Click(object sender, EventArgs e)
        {
            using var f = new MoviesForm();
            f.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using var f = new ShowtimesForm();
            f.ShowDialog();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using var f = new TheaterScreenForm();
            f.ShowDialog();
        }
    }
}
