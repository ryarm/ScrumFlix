using ScrumFlix.Data;
using ScrumFlix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace ScrumFlix.Forms
{
    public partial class ShowtimesForm : Form
    {
        public ShowtimesForm()
        {
            InitializeComponent();
            LoadData();
            LoadShowtimes();
        }
        private void LoadData()
        {
            using var db = new AppDbContext();

            var screens = db.Screens
                .OrderBy(s => s.TheaterScreenId)
                .ToList();

            screenCombo.DisplayMember = "ScreenName";
            screenCombo.ValueMember = "TheaterScreenId";
            screenCombo.DataSource = screens;

            var movies = db.Movies
                .OrderBy(m => m.Title)
                .ToList();

            movieCombo.DisplayMember = "Title";
            movieCombo.ValueMember = "MovieId";
            movieCombo.DataSource = movies;
        }

        private void screenCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadShowtimes();
        }
        private void LoadShowtimes()
        {
            if (screenCombo.SelectedValue is null)
                return;

            int screenId = (int)screenCombo.SelectedValue;

            using var db = new AppDbContext();

            var rows = db.ShowTimes
                .Include(s => s.Movie)
                .Where(s => s.TheaterScreenId == screenId)
                .Select(s => new
                {
                    s.ShowtimeId,
                    MovieTitle = s.Movie!.Title,
                    StartTime = s.StartTime,
                    EndTime = s.StartTime.AddMinutes(s.Movie!.RuntimeMinutes)
                })
                .OrderBy(r => r.StartTime)
                .ToList();

            showtimesGrid.DataSource = rows;

            if (showtimesGrid.Columns["ShowtimeId"] != null)
                showtimesGrid.Columns["ShowtimeId"].Visible = false;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (screenCombo.SelectedValue is null || movieCombo.SelectedValue is null)
            {
                MessageBox.Show("Please select a screen and a movie");
                return;
            }

            int screenId = (int)screenCombo.SelectedValue;
            int movieId = (int)movieCombo.SelectedValue;
            DateTime startTime = startTimePicker.Value;

            using var db = new AppDbContext();

            var movie = db.Movies.First(m => m.MovieId == movieId);
            int runtime = movie.RuntimeMinutes;

            DateTime newStart = startTime;
            DateTime newEnd = startTime.AddMinutes(runtime);

            var existing = db.ShowTimes
                .Include(s => s.Movie)
                .Where(s => s.TheaterScreenId == screenId)
                .ToList();
            bool overlaps = existing.Any(s =>
            {
                DateTime existingStart = s.StartTime;
                DateTime existingEnd = s.StartTime.AddMinutes(s.Movie!.RuntimeMinutes);

                return newStart < existingEnd && existingStart < newEnd;
            });

            if (overlaps)
            {
                MessageBox.Show("That showtime overlaps an existing showtime in this screen, please choose another showtime.");
                return;
            }

            var showtime = new Showtime
            {
                TheaterScreenId = screenId,
                MovieId = movieId,
                StartTime = startTime
            };

            db.ShowTimes.Add(showtime);
            db.SaveChanges();

            LoadShowtimes();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (showtimesGrid.CurrentRow == null)
            {
                MessageBox.Show("Choose a showtime to delete.");
                return;
            }

            var cellValue = showtimesGrid.CurrentRow.Cells["ShowtimeId"].Value;
            if (cellValue == null)
            {
                MessageBox.Show("Could not determine the selected showtime.");
                return;
            }

            int showtimeId = (int)cellValue;

            var confirm = MessageBox.Show(
                "Delete the selected showtime?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            using var db = new AppDbContext();

            var showtime = db.ShowTimes.FirstOrDefault(s => s.ShowtimeId == showtimeId);
            if (showtime == null)
            {
                MessageBox.Show("That showtime no longer exists.");
                LoadShowtimes();
                return;
            }

            db.ShowTimes.Remove(showtime);
            db.SaveChanges();

            LoadShowtimes();
        }
    }
}
