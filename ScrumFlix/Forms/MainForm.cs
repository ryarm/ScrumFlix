/* FILE OVERVIEW:
MainForm.cs is the main form that opens first when running the scrumflix program.
It opens a screen with 4 buttons (so far) that open other forms when clicked 
on to manage movies, showtimes, screens, and locations.

In the solution explorer you will see folders, the data folder holds the database connection in AppDbContext.cs
the forms folder includes all the forms the project uses,
the models folder holds all the object classes for the movies, showtimes, screens, and locations.
Program.cs is what runs when starting the program, opening the main form */

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
            /*using var db = new AppDbContext();

            if (!db.Movies.Any())
            {
                db.Movies.Add(new Movie
                {
                    Title = "Test Movie",
                    Rating = "PG",
                    RuntimeMinutes = 120
                });

                db.SaveChanges();
            }*/

            // int count = db.Movies.Count();
            // MessageBox.Show($"Movies in database: {count}");
        }
        // Opens MovieForm.cs when clicked
        private void btnMovies_Click(object sender, EventArgs e)
        {
            using var f = new MoviesForm();
            f.ShowDialog();
        }
        // Opens ShowtimesForm.cs when clicked
        private void btnShowtimes_Click(object sender, EventArgs e)
        {
            using var f = new ShowtimesForm();
            f.ShowDialog();
        }
        // Opens TheaterScreenForm.cs when clicked
        private void btnTheaterScreen_Click(object sender, EventArgs e)
        {
            using var f = new TheaterScreenForm();
            f.ShowDialog();
        }
        // Opens LocationForm.cs when clicked
        private void button1_Click(object sender, EventArgs e)
        {
            using var f = new LocationForm();
            f.ShowDialog();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            using var f = new UserManagementForm();
            f.ShowDialog();
        }
    }
}
