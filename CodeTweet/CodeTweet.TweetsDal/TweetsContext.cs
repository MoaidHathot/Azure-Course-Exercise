using CodeTweet.TweetsDal.Model;
using Microsoft.EntityFrameworkCore;

namespace CodeTweet.TweetsDal
{
    public class TweetsContext : DbContext
    {
        public TweetsContext(DbContextOptions<TweetsContext> options)
            : base(options)
        {
        }

        public DbSet<TweetEntity> Tweets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TweetEntity>()
                .HasIndex(u => u.Timestamp).IsUnique(false);
        }
    }
}