using Microsoft.EntityFrameworkCore;
using Swap.Object.ChatObjects;
using Swap.Object.GeneralObjects;
using Swap.Object.Items;

namespace Swap.WebApi.Entities
{
    public class SwapContext : DbContext
    {
        public DbSet<Trade> Trades { get; private set; }
        public DbSet<User> Users { get; private set; }
        public DbSet<Chat> Chats { get; private set; }
        public DbSet<InstantMessage> Messages { get; private set; }
        public DbSet<UserToGroup> UserToGroup { get; private set; }
        public DbSet<Item> Items { get; private set; }
        private DbSet<Book> Books { get; set; }
        private DbSet<VideoGame> VideoGames { get; set; }
        private DbSet<Image> Images { get; set; }

        public SwapContext(DbContextOptions<SwapContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupItemTable(modelBuilder);
            SetupChatToMessagesRelationship(modelBuilder);
            SetupUserToGroupToChatRelationship(modelBuilder);
            SetupUserToUserToGroupRelationship(modelBuilder);
            SetupUserToImageRelationship(modelBuilder);
            SetupItemToImageRelationship(modelBuilder);
            SetupItemToUserRelationship(modelBuilder);
            SetupTradeToUserRelationship(modelBuilder);
            SetupTradeToItemRelationship(modelBuilder);
            SetupUserToInstantMessagesRelationship(modelBuilder);
        }

        private void SetupItemTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasBaseType<Item>();
            modelBuilder.Entity<VideoGame>().HasBaseType<Item>();
            modelBuilder.Entity<Item>().ToTable("Items");
            modelBuilder.Entity<Item>().Property(item => item.Description).HasMaxLength(300);
        }

        private void SetupChatToMessagesRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InstantMessage>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId);
        }

        private void SetupUserToGroupToChatRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>()
                .HasMany(c => c.UsersToGroup)
                .WithOne(g => g.Chat)
                .HasForeignKey(c => c.Guid);
        }

        private void SetupUserToUserToGroupRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Groups)
                .WithOne(g => g.User)
                .HasForeignKey(g => g.UserId);
        }

        private void SetupUserToImageRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Images)
                .WithOne(i => i.User)
                .HasForeignKey(i => i.UserId).IsRequired(false);
        }

        private void SetupItemToImageRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasMany(i => i.ImagesOfItem)
                .WithOne(i => i.Item)
                .HasForeignKey(i => i.ItemId).IsRequired(false);
        }

        private void SetupItemToUserRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Owner)
                .WithMany(u => u.ItemsOfUser)
                .HasForeignKey(i => i.IdCustomer);
        }

        private void SetupTradeToUserRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trade>()
                .HasOne(t => t.OfferedTo)
                .WithMany(u => u.Trades)
                .HasForeignKey(t => t.OfferedToId);
        }

        private void SetupTradeToItemRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trade>()
                .HasOne(t => t.OfferedItem)
                .WithMany(i => i.Trades)
                .HasForeignKey(t => t.ItemId);
        }

        private void SetupUserToInstantMessagesRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Messages)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId);
        }
    }
}
