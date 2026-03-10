// This form opens a window that allows the user to CRUD locations from the database

using Microsoft.EntityFrameworkCore;
using ScrumFlix.Data;
using ScrumFlix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace ScrumFlix.Forms
{
    public partial class LocationForm : Form
    {
        public LocationForm()
        {
            InitializeComponent();
        }

        private async Task LoadLocationAsync() // Loads data from database
        {
            using var db = new AppDbContext();
            var locations = await db.Location
                .OrderBy(s => s.LocationId)
                .ToListAsync();
            gridLocations.AutoGenerateColumns = true;
            gridLocations.DataSource = locations;

            gridLocations.Columns[gridLocations.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private async void LocationForm_Load(object sender, EventArgs e) // When form loads it loads the data from the db
        {
            await LoadLocationAsync();
        }

        private async void btnAdd_Click(object sender, EventArgs e) // When the add button is clicked it prompts the user for a location name then adds that location to the db
        {
            string locationName = Interaction.InputBox(
                "Enter the new location name:",
                "Add location"
            );

            if (!string.IsNullOrEmpty(locationName))
            {
                using var db = new AppDbContext();

                var location = new Location
                {
                    LocationName = locationName
                };

                db.Location.Add(location);
                await db.SaveChangesAsync();

                await LoadLocationAsync();
            }
        }
        private Location? SelectedLocation() // When selecting an item in the dataviewgrid this function returns that object
        {
            return gridLocations.CurrentRow?.DataBoundItem as Location;
        }

        private async void btnEdit_Click(object sender, EventArgs e) // When the edit button is clicked it pulls the name from the selected row and updates that locations name in the db
        {
            var selected = SelectedLocation();
            if (selected is null) 
            { 
                MessageBox.Show("Select a screen first please.");
                return;
            }

            string locationName = Interaction.InputBox(
                "Edit location name:",
                "Edit Location",
                selected.LocationName
            );

            if (!string.IsNullOrEmpty(locationName))
            {
                using var db = new AppDbContext();

                var location = await db.Location.FindAsync(selected.LocationId);
                if (location == null)
                    return;

                location.LocationName = locationName;
                
                await db.SaveChangesAsync();

                await LoadLocationAsync();
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e) // When the delete button is clicked it checks to make sure there are no screens attached to the location before removing it from the db
        {
            var selected = SelectedLocation();
            if (selected is null)
            {
                MessageBox.Show("Select a location first please");
                return;
            }
            using var db = new AppDbContext();
            // If location has screens won't delete the location
            bool hasScreens = await db.TheaterScreen.AnyAsync(s => s.LocationId == selected.LocationId);
            if (hasScreens)
            {
                MessageBox.Show("Delete the locations screens before attempting to delete the location.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Delete '{selected.LocationName}'?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            var location = await db.Location.FindAsync(selected.LocationId);
            if (location == null) return;

            db.Location.Remove(location);
            await db.SaveChangesAsync();

            await LoadLocationAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e) // When the refresh button is clicked it refreshes the data in the gridview (in case changes are made to the database while this form is still open on the admin computer)
        {
            await LoadLocationAsync();
        }
    }
}
