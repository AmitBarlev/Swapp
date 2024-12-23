using Swap.WebApi.Repositories;
using Swap.WebApi.Repositories.Interfaces;

namespace Swap.WebApi.Repository
{
    public class Database
    {
        public UserRepository UserTable { get; private set; }
        public ItemRepository ItemTable { get; private set; }
        public TradeRepository TradeTable { get; private set; }
        public ChatRepository ChatsTable { get; private set; }
        public UserToGroupRepository UserToGroupTable { get; private set; }
        public InstantMessageRepository InstantMessageTable { get; private set; }
        public ImageRepository ImageTable { get; private set; }

        public Database(IUserRepository userRepository, IItemRepository itemRepository, IInstantMessageRepository instantMessageRepository,
            ITradeRepository tradeRepository, IChatRepository chatRepository, IUserToGroupRepository userToGroupRepository,
            IImageRepository imageRepository)
        {
            UserTable = userRepository as UserRepository;
            ItemTable = itemRepository as ItemRepository;
            TradeTable = tradeRepository as TradeRepository;
            ChatsTable = chatRepository as ChatRepository;
            UserToGroupTable = userToGroupRepository as UserToGroupRepository;
            InstantMessageTable = instantMessageRepository as InstantMessageRepository;
            ImageTable = imageRepository as ImageRepository;
        }
    }
}
