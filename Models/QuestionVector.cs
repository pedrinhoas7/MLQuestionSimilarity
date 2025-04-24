using Microsoft.ML.Data;

namespace MLQuestionSimilarity.Models
{
    public class QuestionVector
    {
        [VectorType]
        public float[] Features { get; set; }
    }
}
