// This file is part of the Genova project licensed under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace Genova.MovieReviewClassifier;

/// <summary>
/// Represents the collection of all rating centroids and the embedding size.
/// </summary>
internal sealed class RatingCentroidIndex
{
    /// <summary>
    /// Gets or sets the dimensionality of the embedding vectors.
    /// </summary>
    [JsonPropertyName("EmbeddingSize")]
    public int EmbeddingSize { get; set; }

    /// <summary>
    /// Gets or sets the list of rating centroids.
    /// </summary>
    [JsonPropertyName("Centroids")]
    public List<RatingCentroid> Centroids { get; set; } = [];
}
