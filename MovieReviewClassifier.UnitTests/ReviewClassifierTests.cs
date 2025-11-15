// This file is part of the Genova project licensed under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using FluentAssertions;

namespace Genova.MovieReviewClassifier.UnitTests;

public sealed class RatingClassifierTests
{
    [Fact]
    public void Constructor_should_instantiate_successfully()
    {
        // act
        using RatingClassifier classifier = new ();

        // assert
        classifier.Should().NotBeNull();
    }

    [Fact]
    public void GetRating_should_return_1_star_for_terrible_review()
    {
        // arrange
        using RatingClassifier classifier = new ();
        const string review = "I walked out halfway because it was painful to watch.";

        // act
        int rating = classifier.GetRating(review);

        // assert
        rating.Should().Be(1, "a strongly negative review should map to the 1-star centroid");
    }

    [Fact]
    public void GetRating_should_return_5_stars_for_excellent_review()
    {
        // arrange
        using RatingClassifier classifier = new ();
        const string review = "A masterpiece, easily one of the best films I've seen this year.";

        // act
        int rating = classifier.GetRating(review);

        // assert
        rating.Should().Be(5, "a strongly positive review should map to the 5-star centroid");
    }
}
