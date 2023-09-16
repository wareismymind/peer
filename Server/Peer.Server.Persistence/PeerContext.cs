using Microsoft.EntityFrameworkCore;

namespace Peer.Server.Persistence
{
    public class PeerContext : DbContext
    {
        public PeerContext(DbContextOptions<PeerContext> opts)
            : base(opts)
        {
        }

        public DbSet<PullRequestModel> PullRequests { get; set; }
    }

    public class PullRequestModel
    {
        public string Id { get; }
        public string? ExternalId { get; }
        public string? RepoName { get; }
        public string? Owner { get; }
        public string? Provider { get; }
        public string? Author { get; }

        public Uri? Url { get; }
        public string? Title { get; }
        public string? Description { get; }
    }
}
