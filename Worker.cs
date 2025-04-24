using MLQuestionSimilarity.Services;

namespace MLQuestionSimilarity
{
    public class Worker : BackgroundService
    {
        private readonly MessagingService _messagingService;
        private readonly SimilarityService _similarityService;

        public Worker()
        {
            _messagingService = new MessagingService();
            _similarityService = new SimilarityService();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _messagingService.StartConsuming(async (input) =>
            {
                Console.WriteLine("Target: " + input.TargetQuestions.Description);

                foreach (var q in input.AllQuestions)
                {
                    var similarity = await _similarityService.CalculateCosineSimilarityAsync(input.TargetQuestions, q);
                    Console.WriteLine($"Similaridade com \"{q.Description}\": {similarity.Score:F2}");
                }
            }, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _messagingService.StopAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
