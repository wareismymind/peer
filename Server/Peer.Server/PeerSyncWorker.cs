using Microsoft.Extensions.Hosting;

namespace Peer.Server
{
    public enum SyncResult
    {
        // Formerly DaemonResult
        Fire,
        Brimstone,
        Success
    }

    public class PeerSyncWorker : BackgroundService
    {
        // Ooh look at me I'm a spooky daemon, souls! bleh!

        public PeerSyncWorker()
        {

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                //Console.WriteLine("OOh so spooky!");
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }
}
