// This form opens a window that allows the user to CRUD movies from the database

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ScrumFlix.Data;
using ScrumFlix.Models;
using ScrumFlix.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;


namespace ScrumFlix.Forms
{
    public partial class MoviesForm : Form
    {
        public MoviesForm()
        {
            InitializeComponent();
        }
        private async void MoviesForm_Load(object sender, EventArgs e) // When form loads it loads the data from the db
        {
            await LoadMoviesAsync();
        }

        private async Task LoadMoviesAsync() // Loads data from database
        {
            using var db = new AppDbContext();
            var movies = await db.Movies.OrderBy(m => m.Title).ToListAsync();

            gridMovies.AutoGenerateColumns = true;
            gridMovies.DataSource = movies;

            gridMovies.Columns[gridMovies.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        private async void btnAdd_Click(object sender, EventArgs e) // When the add button is clicked it opens the MovieEditForm.cs form to allow the user to add a movie to the db
        {
            using var form = new MovieEditForm();

            if (form.ShowDialog() != DialogResult.OK)
                return;

            using var db = new AppDbContext();
            db.Movies.Add(form.Movie);
            await db.SaveChangesAsync();

            await LoadMoviesAsync();
        }
        private Movie? SelectedMovie() // When selecting an item in the dataviewgrid this function returns that object
        {
            return gridMovies.CurrentRow?.DataBoundItem as Movie;
        }

        private async void btnRefresh_Click(object sender, EventArgs e) // When the refresh button is clicked it refreshes the data in the gridview (in case changes are made to the database while this form is still open on the admin computer)
        {
            await LoadMoviesAsync();
        }

        private async void btnEdit_Click(object sender, EventArgs e) // When the edit button is clicked it pulls the info from the selected item and opens a MovieEditForm.cs form to allow the user to edit the movies info
        {
            var selected = SelectedMovie();
            if (selected is null)
            {
                MessageBox.Show("Select a movie first please.");
                return;
            }

            using var form = new MovieEditForm(selected);

            if (form.ShowDialog() != DialogResult.OK)
                return;

            using var db = new AppDbContext();
            var movie = await db.Movies.FindAsync(selected.MovieId);
            if (movie is null) return;

            movie.Title = form.Movie.Title;
            movie.Rating = form.Movie.Rating;
            movie.RuntimeMinutes = form.Movie.RuntimeMinutes;
            movie.Description = form.Movie.Description;

            await db.SaveChangesAsync();
            await LoadMoviesAsync();
        }

        private async void btnDelete_Click(object sender, EventArgs e) // When the delete button is clicked it checks to make sure there are no showtimes attached to the movie before removing it from the db
        {
            var selected = SelectedMovie();
            if (selected is null)
            {
                MessageBox.Show("Select a movie first please.");
                return;
            }

            using var db = new AppDbContext();
            // If showtime exists won't delete movie
            bool hasShowtimes = await db.ShowTimes.AnyAsync(s => s.MovieId == selected.MovieId);
            if (hasShowtimes)
            {
                MessageBox.Show("Delete movie showtimes before attempting to delete the movie.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Delete '{selected.Title}'?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            var movie = await db.Movies.FindAsync(selected.MovieId);
            if (movie is null) return;

            db.Movies.Remove(movie);
            await db.SaveChangesAsync();

            await LoadMoviesAsync();
        }
    }
}
