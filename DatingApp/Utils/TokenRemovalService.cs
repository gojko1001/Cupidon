using DatingApp.Repository.Interfaces;

namespace DatingApp.Utils
{
    public class TokenRemovalService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer timer;

        public TokenRemovalService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var nextRunTime = DateTime.Today.AddDays(1);
            var firstInterval = nextRunTime.Subtract(DateTime.UtcNow);

            Action action = () =>
            {
                var t1 = Task.Delay(firstInterval);
                t1.Wait();
                RemoveExpiredTokens(null);

                timer = new Timer(RemoveExpiredTokens, null, TimeSpan.Zero, TimeSpan.FromHours(12));
            };

            Task.Run(action);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void RemoveExpiredTokens(object state)
        {
            using var scope = _scopeFactory.CreateScope();

            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            uow.RefreshTokenRepository.RemoveExpiredTokensAsync();
            uow.Complete();
        } 
    }
}
