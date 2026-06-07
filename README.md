# Genova.MovieReviewClassifier

Classifies short movie review text into a predicted 1–5 star rating using embedding similarity against precomputed rating centroids.

> [!WARNING]
> This is an experimental project and should not be considered production-ready. It exists to explore a small AI, ML, agent, or demo idea within the broader Genova ecosystem.

> [!IMPORTANT]
> A fresh public clone of this repository should not be expected to restore or build without additional Genova infrastructure. Many Genova dependencies are distributed through a private authenticated NuGet feed, and the public source does not include feed credentials or a complete public package graph.

## Installation

```bash
dotnet restore
dotnet build
```

Run the console app:

```bash
dotnet run --project MovieReviewClassifier.Terminal
```

## Usage

Console app:

```bash
dotnet run --project MovieReviewClassifier.Terminal
```

Library:

```csharp
using Genova.MovieReviewClassifier;

using var classifier = new RatingClassifier();
int rating = classifier.GetRating("A tense, clever thriller with a weak ending.");
```

## Features

* Predicts a 1–5 star rating from free-form review text
* Uses an ONNX embedding model and cosine similarity
* Includes a console app for interactive testing
* Includes a training app to generate rating centroid data from sample review files

## Notes

* Targets .NET 8.0
* The classifier depends on embedded centroid data (`rating-centroids.json`)
* The training project uses hard-coded local paths and appears intended for local data generation

## Thanks

* Genova.MiniML
* Microsoft.ML / ONNX Runtime

## Third-Party Notices

This project has direct runtime dependencies on third-party NuGet packages, including `Microsoft.Extensions.*` packages (MIT), `Microsoft.ML*` packages (MIT). See each package's NuGet license metadata for full license and notice terms.

## License

GNU General Public License v3.0. See the `LICENSE` file for details.
