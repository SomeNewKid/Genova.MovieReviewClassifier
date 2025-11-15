// This file is part of the Genova project licensed under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

namespace Genova.MovieReviewClassifier.Terminal;

/// <summary>
/// Console application that lets the user classify a movie review
/// as a 1–5 star rating using a MiniLM-based classifier.
/// </summary>
public static class Program
{
    /// <summary>
    /// Application entry point. Prompts the user to type short movie-review
    /// text, classifies it into 1–5 stars, and prints the result.
    /// </summary>
    /// <param name="args">Command-line arguments (not used).</param>
    public static void Main(string[] args)
    {
        Console.WriteLine("Genova.MovieReviewClassifier.Terminal");
        Console.WriteLine("Type a short movie review to get a 1–5 star rating.");
        Console.WriteLine("Type \"exit\" to quit.");
        Console.WriteLine();

        try
        {
            using RatingClassifier classifier = new ();

            while (true)
            {
                Console.Write("> ");
                string? review = Console.ReadLine();

                if (review == null)
                {
                    continue;
                }

                if (review.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Exiting...");
                    return;
                }

                int rating = classifier.GetRating(review);

                Console.WriteLine($"Rating: {rating} star{(rating == 1 ? "" : "s")}");
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("An error occurred while starting the movie review classifier:");
            Console.Error.WriteLine(ex.Message);
        }
    }
}
