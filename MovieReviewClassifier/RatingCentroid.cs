// This file is part of the Genova project licensed under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace Genova.MovieReviewClassifier;

/// <summary>
/// Represents a centroid for a particular star rating.
/// </summary>
internal sealed class RatingCentroid
{
    /// <summary>
    /// Gets or sets the movie rating (1 to 5 stars).
    /// </summary>
    [JsonPropertyName("Rating")]
    public int Rating { get; set; }

    /// <summary>
    /// Gets or sets the centroid embedding vector for this rating.
    /// </summary>
    [JsonPropertyName("Vector")]
    public float[] Vector { get; set; } = [];
}
