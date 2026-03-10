using ScrumFlix.Data;
using ScrumFlix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ScrumFlix.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace ScrumFlix.Forms
{
    public partial class TheaterScreenForm : Form
    {
        public TheaterScreenForm()
        {
            InitializeComponent();
        }
        private async void TheaterScreenForm_Load(object sender, EventArgs e)
        {
            await LoadTheaterScreenAsync();
        }

        private async Task LoadTheaterScreenAsync()
        {
            using var db = new AppDbContext();
            var screens = await db.TheaterScreen.OrderBy(m => m.ScreenName).ToListAsync();

            gridScreens.AutoGenerateColumns = true;
            gridScreens.DataSource = screens;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            using var form = new TheaterScreenEditForm();

            if (form.ShowDialog() != DialogResult.OK)
                return;
            using var db = new AppDbContext();
            db.TheaterScreen.Add(form.TheaterScreen);

            await db.SaveChangesAsync();

            await LoadTheaterScreenAsync();
        }
        private TheaterScreen? SelectedScreen()
        {
            return gridScreens.CurrentRow?.DataBoundItem as TheaterScreen;
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            var selected = SelectedScreen();
            if (selected is null)
            {
                MessageBox.Show("Select a screen first please.");
                return;
            }

            using var form = new TheaterScreenEditForm(selected);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            using var db = new AppDbContext();
            var screen = await db.TheaterScreen.FindAsync(selected.TheaterScreenId);
            if (screen is null) return;

            screen.ScreenName = form.TheaterScreen.ScreenName;
            screen.LocationId = form.TheaterScreen.LocationId;

            await db.SaveChangesAsync();
            await LoadTheaterScreenAsync();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var selected = SelectedScreen();
            if (selected is null)
            {
                MessageBox.Show("Select a screen first please.");
                return;
            }

            using var db = new AppDbContext();
            // If showtime exists won't delete screen
            bool hasShowtimes = await db.ShowTimes.AnyAsync(s => s.TheaterScreenId == selected.TheaterScreenId);
            if (hasShowtimes)
            {
                MessageBox.Show("Delete screen's showtimes before attempting to delete the screen.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Delete '{selected.ScreenName}'?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            var screen = await db.TheaterScreen.FindAsync(selected.TheaterScreenId);
            if (screen is null) return;

            db.TheaterScreen.Remove(screen);
            await db.SaveChangesAsync();

            await LoadTheaterScreenAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadTheaterScreenAsync();
        }
    }
}
