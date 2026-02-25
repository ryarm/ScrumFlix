using ScrumFlix.Models;

namespace ScrumFlix.Forms;

public partial class MovieEditForm : Form
{
    public Movie Movie { get; private set; } = new Movie();

    public MovieEditForm()
    {
        InitializeComponent();

        numRuntime.Minimum = 1;
        numRuntime.Maximum = 600;
        numRuntime.Value = 90;
    }
    public MovieEditForm(Movie existing) : this()
    {
        Movie = new Movie
        {
            MovieId = existing.MovieId,
            Title = existing.Title,
            Rating = existing.Rating,
            RuntimeMinutes = existing.RuntimeMinutes,
            Description = existing.Description
        };

        txtTitle.Text = Movie.Title;
        txtRating.Text = Movie.Rating ?? "";
        numRuntime.Value = Movie.RuntimeMinutes > 0 ? Movie.RuntimeMinutes : 90;
        txtDescription.Text = Movie.Description ?? "";
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        string title = txtTitle.Text.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            MessageBox.Show("Title is required.");
            DialogResult = DialogResult.None;
            return;
        }

        Movie.Title = title;

        string rating = txtRating.Text.Trim();
        if (rating.Length > 20)
        {
            MessageBox.Show("Rating must be 20 characters or less.");
            DialogResult = DialogResult.None;
            return;
        }
        Movie.Rating = string.IsNullOrWhiteSpace(rating) ? null : rating;

        Movie.RuntimeMinutes = (int)numRuntime.Value;

        string desc = txtDescription.Text.Trim();
        Movie.Description = string.IsNullOrWhiteSpace(desc) ? null : desc;
    }
}