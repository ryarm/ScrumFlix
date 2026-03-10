using ScrumFlix.Data;
using ScrumFlix.Models;

namespace ScrumFlix.Forms
{
    public partial class TheaterScreenEditForm : Form
    {
        private void LoadData()
        {
            using var db = new AppDbContext();

            var Location = db.Location
                .OrderBy(s => s.LocationId)
                .ToList();

            screenLocation.DisplayMember = "LocationName";
            screenLocation.ValueMember = "LocationId";
            screenLocation.DataSource = Location;
        }
        public TheaterScreen TheaterScreen { get; private set; } = new TheaterScreen();
        public TheaterScreenEditForm()
        {
            InitializeComponent();
            LoadData();
        }
        public TheaterScreenEditForm(TheaterScreen existing) : this()
        {
            TheaterScreen = new TheaterScreen
            {
                TheaterScreenId = existing.TheaterScreenId,
                ScreenName = existing.ScreenName,
                LocationId = existing.LocationId,
            };

            screenName.Text = TheaterScreen.ScreenName;
            screenLocation.SelectedValue = TheaterScreen.LocationId;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string name = screenName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Theater Screen name is required.");
                DialogResult = DialogResult.None;
                return;
            }
            TheaterScreen.ScreenName = name;

            TheaterScreen.LocationId = Convert.ToInt32(screenLocation.SelectedValue);
        }
    }
}