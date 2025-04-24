using Microsoft.ML;
using MLQuestionSimilarity.Models;

namespace MLQuestionSimilarity.Services
{
    public class SimilarityService
    {
        private readonly MLContext _mlContext;

        public SimilarityService()
        {
            _mlContext = new MLContext();
        }

        public async Task<SimilarityResult> CalculateCosineSimilarityAsync(Question target, Question compare)
        {
            var questions = new List<Question> { target, compare };

            var data = _mlContext.Data.LoadFromEnumerable(questions);
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(Question.Description));

            var model = pipeline.Fit(data);
            var transformedData = model.Transform(data);

            var vectors = _mlContext.Data.CreateEnumerable<QuestionVector>(transformedData, reuseRowObject: false).ToList();

            var targetVector = vectors[0].Features;
            var compareVector = vectors[1].Features;

            var score = CosineSimilarity(targetVector, compareVector);

            return await Task.FromResult(new SimilarityResult
            {
                Question = compare,
                Score = score
            });
        }


        // Método para calcular a similaridade de cosseno
        private double CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            var dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
            var magnitudeA = Math.Sqrt(vectorA.Sum(x => x * x));
            var magnitudeB = Math.Sqrt(vectorB.Sum(x => x * x));

            if (magnitudeA == 0 || magnitudeB == 0)
                return 0;

            return dotProduct / (magnitudeA * magnitudeB);
        }

    }
}
