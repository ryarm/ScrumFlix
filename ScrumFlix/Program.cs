using ScrumFlix.Data;
using ScrumFlix.Models;
using System;

namespace ScrumFlix;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
            if (!db.TheaterScreen.Any())
            {
                db.TheaterScreen.AddRange(
                    new TheaterScreen { ScreenName = "Screen 1" },
                    new TheaterScreen { ScreenName = "Screen 2" },
                    new TheaterScreen { ScreenName = "Screen 3" }
                );

                db.SaveChanges();
            }
            if (!db.Movies.Any())
            {
                db.Movies.AddRange(
                    new Movie { Title = "The Shawshank Retraction", Rating = "R", RuntimeMinutes = 142, Description = "A man is sent to prison for a crime he didn't commit, but mostly just hangs out in a library." },
                    new Movie { Title = "The Codfather", Rating = "R", RuntimeMinutes = 175, Description = "An aging patriarch of an organized seafood empire transfers control to his reluctant son." },
                    new Movie { Title = "The Dark Knight Rises (Late)", Rating = "PG-13", RuntimeMinutes = 152, Description = "A billionaire spends millions on a bat costume instead of just going to therapy." },
                    new Movie { Title = "12 Angry Toddlers", Rating = "PG", RuntimeMinutes = 96, Description = "A jury holdout attempts to prevent a naptime miscarriage of justice." },
                    new Movie { Title = "Schindler's Grocery List", Rating = "R", RuntimeMinutes = 195, Description = "In occupied Poland, an industrialist tries to remember if he needed milk or eggs." },
                    new Movie { Title = "Pulp Friction", Rating = "R", RuntimeMinutes = 154, Description = "Two hitmen discuss French burgers while trying to get a rug cleaned." },
                    new Movie { Title = "The Lord of the Rings: The Return of the Receipt", Rating = "PG-13", RuntimeMinutes = 201, Description = "A small man walks a very long way to return a piece of jewelry he didn't want." },
                    new Movie { Title = "The Good, the Bad, and the Clueless", Rating = "PG-13", RuntimeMinutes = 161, Description = "Three men look for gold but mostly just squint at each other in the desert sun." },
                    new Movie { Title = "Fight Club (Don't Talk About It)", Rating = "R", RuntimeMinutes = 139, Description = "An insomniac office worker starts a gym that gets way out of hand." },
                    new Movie { Title = "Forrest Gump: The Speed-Walking Years", Rating = "PG-13", RuntimeMinutes = 142, Description = "A man accidentally participates in every major historical event because he forgot to stop running." },
                    new Movie { Title = "Star Wars: Episode IV - A New Hope (For a Discount)", Rating = "PG", RuntimeMinutes = 121, Description = "A farm boy joins a cult and blows up a government building because his chores were boring." },
                    new Movie { Title = "The Silence of the Lambs (But the Loudness of the Fans)", Rating = "R", RuntimeMinutes = 118, Description = "An FBI trainee interviews a cannibal to find a guy who really needs a moisturizing routine." },
                    new Movie { Title = "Saving Private Ryan's Internet History", Rating = "R", RuntimeMinutes = 169, Description = "A group of soldiers risks everything to ensure a man doesn't have to explain himself to his mother." },
                    new Movie { Title = "Inception: The Nap Within a Nap", Rating = "PG-13", RuntimeMinutes = 148, Description = "A group of people try to explain a complex board game to a sleeping businessman." },
                    new Movie { Title = "Jurassic Park: Safety Inspection Failure", Rating = "PG-13", RuntimeMinutes = 127, Description = "A billionaire recreates extinct monsters but refuses to pay for a decent IT department." },
                    new Movie { Title = "The Matrix: Blue Pill Edition", Rating = "R", RuntimeMinutes = 136, Description = "A man decides the simulation is actually fine because the steak tastes better than reality." },
                    new Movie { Title = "Titanic: Plenty of Room on the Door", Rating = "PG-13", RuntimeMinutes = 194, Description = "A luxury cruise ends abruptly when a drawing enthusiast refuses to share a floating piece of wood." },
                    new Movie { Title = "The Lion King: Hamlet with Fur", Rating = "G", RuntimeMinutes = 88, Description = "A young cat runs away from home because his uncle is a gaslighter, then returns to reclaim his rock." },
                    new Movie { Title = "Top Gun: Volleyball Enthusiasts", Rating = "PG", RuntimeMinutes = 110, Description = "Professional pilots spend 10% of their time flying and 90% of their time looking intense in aviators." },
                    new Movie { Title = "Interstellar: The Dad Who Stayed in the Bookshelf", Rating = "PG-13", RuntimeMinutes = 169, Description = "A man travels through a black hole just to tell his daughter to check the mail more often." }
                );

                db.SaveChanges();
            }
        }

        Application.Run(new MainForm());
    }
}
