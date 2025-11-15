// This file is part of the Genova project licensed under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using System.Text.Json.Serialization;
using Genova.MiniML;

namespace Genova.MovieReviewClassifier.Training;

/// <summary>
/// Training console application that loads 1–5 star movie reviews,
/// embeds each review using a MiniLM-based ONNX model,
/// computes a centroid vector for each star rating,
/// and writes the result to a JSON file for classification.
/// </summary>
public static class Program
{
    private const string SolutionDirectory =
        @"C:\Git\Genova.MovieReviewClassifier\";
    private const string InputDirectory =
       SolutionDirectory + @"MovieReviewClassifier.Training\Input";
    private const string OutputFilePath =
        SolutionDirectory + @"MovieReviewClassifier\Data\" + RatingClassifier.CentroidsFileName;

    private static readonly string[] RatingFiles =
    {
        "1-star.txt",
        "2-stars.txt",
        "3-stars.txt",
        "4-stars.txt",
        "5-stars.txt"
    };

    /// <summary>
    /// Application entry point. Loads each star-rating file, embeds the reviews,
    /// computes rating centroids, and writes the results to a JSON file.
    /// </summary>
    /// <param name="args">Command-line arguments (not used).</param>
    /// <returns>0 on success; non-zero otherwise.</returns>
    public static int Main(string[] args)
    {
        try
        {
            Console.WriteLine("Genova.MovieReviewClassifier.Training");
            Console.WriteLine("Building movie review rating centroids...");
            Console.WriteLine();

            using IEmbeddingModel model = new OnnxEmbeddingModel();
            int embeddingSize = model.EmbeddingSize;

            List<RatingCentroid> centroids = new List<RatingCentroid>();

            for (int rating = 1; rating <= 5; rating++)
            {
                string fileName = RatingFiles[rating - 1];
                string path = Path.Combine(InputDirectory, fileName);

                Console.WriteLine($"Loading reviews for rating {rating}: {path}");

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException(
                        $"Cannot find review file for rating {rating}. Expected at: {path}");
                }

                string[] lines = File.ReadAllLines(path);
                List<float[]> vectors = [];

                foreach (string raw in lines)
                {
                    string review = raw.Trim();
                    if (review.Length == 0)
                    {
                        continue;
                    }

                    try
                    {
                        TokenizedEmbedding emb = model.EmbedWithTokens(review);
                        float[] vec = (float[])emb.SentenceVector.Clone();

                        if (vec.Length != embeddingSize)
                        {
                            throw new InvalidOperationException(
                                $"Embedding size mismatch. Expected {embeddingSize}, got {vec.Length}.");
                        }

                        NormalizeInPlace(vec);
                        vectors.Add(vec);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  [Warning] Failed to embed review: \"{review}\". Error: {ex.Message}");
                    }
                }

                if (vectors.Count == 0)
                {
                    throw new InvalidOperationException($"No valid embeddings created for rating {rating}.");
                }

                float[] centroid = ComputeCentroid(vectors, embeddingSize);

                centroids.Add(new RatingCentroid
                {
                    Rating = rating,
                    Vector = centroid
                });

                Console.WriteLine($"  Added centroid for rating {rating} (from {vectors.Count} reviews).");
                Console.WriteLine();
            }

            // Create output directory if needed
            string? outputDir = Path.GetDirectoryName(OutputFilePath);
            if (!string.IsNullOrEmpty(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            RatingCentroidIndex index = new RatingCentroidIndex
            {
                EmbeddingSize = embeddingSize,
                Centroids = centroids
            };

            JsonSerializerOptions opts = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            string json = JsonSerializer.Serialize(index, opts);
            File.WriteAllText(OutputFilePath, json);

            Console.WriteLine($"Centroids saved to: {OutputFilePath}");
            Console.WriteLine("Done.");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error while building rating centroids:");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static float[] ComputeCentroid(IReadOnlyList<float[]> vectors, int embeddingSize)
    {
        float[] centroid = new float[embeddingSize];

        foreach (float[] vec in vectors)
        {
            for (int i = 0; i < embeddingSize; i++)
            {
                centroid[i] += vec[i];
            }
        }

        for (int i = 0; i < embeddingSize; i++)
        {
            centroid[i] /= vectors.Count;
        }

        NormalizeInPlace(centroid);

        return centroid;
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
}
