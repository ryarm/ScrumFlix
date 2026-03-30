using Microsoft.EntityFrameworkCore;
using ScrumFlix.Data;
using ScrumFlix.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ScrumFlix.Forms
{
    public partial class EmployeeForm : Form
    {
        public EmployeeForm()
        {
            InitializeComponent();
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.ToLower();

            using (var context = new AppDbContext())
            {
                var filtered = context.Showtime
                    .Where(s => s.Movie!.Title.ToLower().Contains(search))
                    .Select(s => new
                    {
                        Movie = s.Movie!.Title,
                        ShowTime = s.StartTime,
                        Location = s.TheaterScreen!.Location,
                        Screen = s.TheaterScreen!.ScreenName,
                        Capacity = s.Capacity
                    })
                    .ToList();

                gridMovieShowtimes.DataSource = filtered;
            }
        }

        private void EmployeeForm_Load(object sender, EventArgs e)
        {
            using (var context = new AppDbContext())
            {
                var showtimes = context.Showtime
                    .Include(s => s.Movie)
                    .Include(s => s.TheaterScreen)
                        .ThenInclude(ts => ts.Location)
                    .Select(s => new
                    {
                        Movie = s.Movie!.Title,
                        ShowTime = s.StartTime,
                        Location = s.TheaterScreen!.GetLocation,
                        Screen = s.TheaterScreen!.ScreenName,
                        Capacity = s.Capacity
                    })
                    .ToList();

                gridMovieShowtimes.DataSource = showtimes;
                gridMovieShowtimes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                gridMovieShowtimes.Columns["Movie"].FillWeight = 30;
                gridMovieShowtimes.Columns["ShowTime"].FillWeight = 20;
                gridMovieShowtimes.Columns["Location"].FillWeight = 20;
                gridMovieShowtimes.Columns["Screen"].FillWeight = 15;
                gridMovieShowtimes.Columns["Capacity"].FillWeight = 15;
            }
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            ShowAll();
        }

        private void btnSell_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Enter a valid email.");
                return;
            }

            if (gridMovieShowtimes.CurrentRow == null)
                return;

            int ticketsToSell = 1;

            var movieTitle = gridMovieShowtimes.CurrentRow.Cells["Movie"].Value.ToString();
            var startTime = (DateTime)gridMovieShowtimes.CurrentRow.Cells["ShowTime"].Value;
            var location = gridMovieShowtimes.CurrentRow.Cells["Location"].Value.ToString();
            var screen = gridMovieShowtimes.CurrentRow.Cells["Screen"].Value.ToString();

            using (var context = new AppDbContext())
            {
                var showtime = context.Showtime
                    .Include(s => s.Movie)
                    .Include(s => s.TheaterScreen)
                    .FirstOrDefault(s =>
                        s.Movie!.Title == movieTitle &&
                        s.StartTime == startTime);

                if (showtime == null)
                    return;

                if (showtime.SellTicket(ticketsToSell))
                {
                    context.SaveChanges();

                    string code = new Random().Next(100000, 999999).ToString();

                    var ticket = new Ticket
                    {
                        TicketCode = int.Parse(code),
                        ShowtimeId = showtime.ShowtimeId
                    };

                    context.Ticket.Add(ticket);
                    context.SaveChanges();

                    try
                    {
                        SendEmail(email, code, movieTitle, startTime, location, screen);
                        MessageBox.Show("Ticket sold and email sent!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ticket sold but email failed: " + ex.Message);
                    }

                    EmployeeForm_Load(null, null);
                }
                else
                {
                    MessageBox.Show("No tickets available :(");
                }
            }
        }

        private void ShowAll()
        {
            using (var context = new AppDbContext())
            {
                var showtimes = context.Showtime
                    .Select(s => new
                    {
                        Movie = s.Movie!.Title,
                        ShowTime = s.StartTime,
                        Location = s.TheaterScreen!.Location,
                        Screen = s.TheaterScreen!.ScreenName,
                        Capacity = s.Capacity
                    })
                    .ToList();

                gridMovieShowtimes.DataSource = showtimes;
            }
        }

        private void SendEmail(string toEmail, string code, string movie, DateTime showTime, string location, string screen)
        {
            var fromAddress = new MailAddress("scrumflix@gmail.com");
            var toAddress = new MailAddress(toEmail);

            const string fromPassword = "tltiuneyjoqpkbmh";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "ScrumFlix Theaters Ticket Code",
                Body = $"Your ticket code is: {code}" +
                $"\n\nMovie: {movie}\r\nShowtime: {showTime}\r\nLocation: {location}\r\nScreen: {screen}"
            };

            smtp.Send(message);
        }

        private void EmployeeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text.ToLower();

            using (var context = new AppDbContext())
            {
                var filtered = context.Showtime
                    .Where(s => s.Movie!.Title.ToLower().Contains(search))
                    .Select(s => new
                    {
                        Movie = s.Movie!.Title,
                        ShowTime = s.StartTime,
                        Location = s.TheaterScreen!.Location,
                        Screen = s.TheaterScreen!.ScreenName,
                        Capacity = s.Capacity
                    })
                    .ToList();

                gridMovieShowtimes.DataSource = filtered;
            }
        }
    }
}
