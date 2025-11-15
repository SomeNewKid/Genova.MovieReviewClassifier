// This file is part of the Genova project licensed under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Text.Json;
using Genova.Common.Attributes;
using Genova.Common.Utilities;
using Genova.MiniML;

namespace Genova.MovieReviewClassifier;

/// <summary>
/// Classifies a movie review as a 1–5 star rating by comparing its embedding
/// to precomputed rating centroids stored in rating-centroids.json.
/// </summary>
[CodeQuality(Public = true, Justification = "Intended for use on the Rusty Kane website.")]
public sealed class RatingClassifier : IDisposable
{
    /// <summary>
    /// The name of the JSON file containing the rating centroids.
    /// </summary>
    internal const string CentroidsFileName = "rating-centroids.json";

    private const string DataFolderName = "Data";

    private readonly IEmbeddingModel _model;
    private readonly RatingCentroidIndex _index;

    /// <summary>
    /// Initializes a new instance of the <see cref="RatingClassifier"/> class.
    /// Loads the rating-centroids.json file and prepares the classifier.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the centroid file is missing, corrupt, or incompatible with the model.
    /// </exception>
    public RatingClassifier()
    {
        _model = new OnnxEmbeddingModel();

        string baseDirectory = AppContext.BaseDirectory;
        string dataDirectory = Path.Combine(baseDirectory, DataFolderName);
        string centroidsPath = Path.Combine(dataDirectory, CentroidsFileName);

        Assembly assembly = typeof(RatingClassifier).Assembly;
        using Stream? stream = FileHelper.GetEmbeddedResourceStream(assembly, $"{DataFolderName}.{CentroidsFileName}");

        if (stream == null)
        {
            throw new InvalidOperationException(
                $"Failed to find embedded resource: {CentroidsFileName}");
        }

        RatingCentroidIndex? index =
            JsonSerializer.Deserialize<RatingCentroidIndex>(stream);

        if (index == null)
        {
            throw new InvalidOperationException(
                $"Failed to deserialize centroid data from: {centroidsPath}");
        }

        if (index.EmbeddingSize != _model.EmbeddingSize)
        {
            throw new InvalidOperationException(
                $"Embedding size mismatch. Centroid file = {index.EmbeddingSize}, model = {_model.EmbeddingSize}.");
        }

        if (index.Centroids == null || index.Centroids.Count == 0)
        {
            throw new InvalidOperationException("No centroids found in centroid file.");
        }

        _index = index;
    }

    /// <summary>
    /// Classifies a movie review by returning the star rating (1–5) whose
    /// centroid is closest to the review in embedding space.
    /// </summary>
    /// <param name="review">The user-provided movie review text.</param>
    /// <returns>
    /// An integer rating between 1 and 5.
    /// If the review is empty or something unexpected happens, returns 3 (neutral).
    /// </returns>
    public int GetRating(string review)
    {
        if (string.IsNullOrWhiteSpace(review))
        {
            return 3; // neutral fallback
        }

        try
        {
            TokenizedEmbedding embedding = _model.EmbedWithTokens(review);
            float[] queryVector = (float[])embedding.SentenceVector.Clone();

            NormalizeInPlace(queryVector);

            double bestSim = double.NegativeInfinity;
            int bestRating = 3; // fallback

            foreach (RatingCentroid centroid in _index.Centroids)
            {
                double sim = CosineSimilarity(queryVector, centroid.Vector);

                if (sim > bestSim)
                {
                    bestSim = sim;
                    bestRating = centroid.Rating;
                }
            }

            return bestRating;
        }
        catch
        {
            return 3; // safe fallback
        }
    }

    /// <summary>
    /// Releases resources used by the current <see cref="RatingClassifier"/> instance.
    /// </summary>
    public void Dispose()
    {
        _model.Dispose();
    }

    private static void NormalizeInPlace(float[] vector)
    {
        double sumSquares = 0.0;
        foreach (float f in vector)
        {
            sumSquares += f * f;
        }

        if (sumSquares <= 0.0)
        {
            return;
        }

        double norm = Math.Sqrt(sumSquares);
        for (int i = 0; i < vector.Length; i++)
        {
            vector[i] = (float)(vector[i] / norm);
        }
    }

    private static double CosineSimilarity(float[] a, float[] b)
    {
        double dot = 0.0, normA = 0.0, normB = 0.0;

        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        if (normA == 0.0 || normB == 0.0)
        {
            return 0.0;
        }

        return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}
