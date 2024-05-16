using MouseTagProject.Interfaces;

namespace MouseTagProject.Services
{
    /*public class MyHostedServise : IHostedService
    {
        private readonly IEmailService _emailService;
        private readonly ICandidate _candidate;

        public MyHostedServise(IEmailService emailService, ICandidate candidate)
        {
            _emailService = emailService;
            _candidate = candidate;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    //await Task.Delay(TimeSpan.FromHours(24)); //Production
                    //await Task.Delay(TimeSpan.FromSeconds(5)); //Testing
                    var candidates = _candidate.GetCandidatesReminder().Where(c => c.WillBeContacted > DateTime.Now && c.WillBeContacted < DateTime.Now.AddDays(2)).ToList();
                    if (candidates.Count() != 0)
                    {
                        var letter = _emailService.GenerateLetter(candidates);
                        _emailService.SendEmail(letter);
                        Console.WriteLine("Išsiūsta!");
                    }
                }
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }*/
}

