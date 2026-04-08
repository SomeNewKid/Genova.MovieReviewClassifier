# Genova.MovieReviewClassifier

Classifies short movie review text into a predicted 1–5 star rating using embedding similarity against precomputed rating centroids.

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

## License

GNU General Public License v3.0 (GPL-3.0)
