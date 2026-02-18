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
        private async void MoviesForm_Load(object sender, EventArgs e)
        {
            await LoadMoviesAsync();
        }

        private async Task LoadMoviesAsync()
        {
            using var db = new AppDbContext();
            var movies = await db.Movies.OrderBy(m => m.Title).ToListAsync();

            gridMovies.AutoGenerateColumns = true;
            gridMovies.DataSource = movies;
        }
        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string title = Interaction.InputBox("Movie title:", "Add Movie");
            title = title.Trim();

            if (string.IsNullOrWhiteSpace(title))
                return;

            using var db = new AppDbContext();
            db.Movies.Add(new Movie { Title = title });
            await db.SaveChangesAsync();

            await LoadMoviesAsync();
        }
        private Movie? SelectedMovie()
        {
            return gridMovies.CurrentRow?.DataBoundItem as Movie;
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadMoviesAsync();
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            var selected = SelectedMovie();
            if (selected is null)
            {
                MessageBox.Show("Select a movie first.");
                return;
            }

            string newTitle = Interaction.InputBox("Edit title:", "Edit Movie", selected.Title);
            newTitle = newTitle.Trim();

            if (string.IsNullOrWhiteSpace(newTitle))
                return;

            using var db = new AppDbContext();
            var movie = await db.Movies.FindAsync(selected.MovieId);
            if (movie is null) return;

            movie.Title = newTitle;
            await db.SaveChangesAsync();

            await LoadMoviesAsync();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var selected = SelectedMovie();
            if (selected is null)
            {
                MessageBox.Show("Select a movie first.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Delete '{selected.Title}'?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            using var db = new AppDbContext();
            var movie = await db.Movies.FindAsync(selected.MovieId);
            if (movie is null) return;

            db.Movies.Remove(movie);
            await db.SaveChangesAsync();

            await LoadMoviesAsync();
        }
    }
}
