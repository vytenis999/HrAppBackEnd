using MouseTagProject.Interfaces;
using MouseTagProject.Models;

namespace MouseTagProject.Services
{
    public class MyBackgroundService : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;

        public MyBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IEmailSender emailService = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                ICandidate candidate = scope.ServiceProvider.GetRequiredService<ICandidate>();
                IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                while (!stoppingToken.IsCancellationRequested)
                {
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Monday && DateTime.Now.Hour == 12 && DateTime.Now.Minute == 10)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1)); // wait to when minute value will be changed

                        var letter = emailService.GenerateLetter();
                        var users = await userService.GetIdentityUsersAsync();
                        var emails = users.Select(x => x.Email).ToArray();
                        var message = new Message(emails, "Test email", letter);
                        emailService.SendEmail(message);
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1));
                    }
                }
            }
        }
    }
}