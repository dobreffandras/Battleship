namespace Battleship.Model
{
    internal class GameMetadata
    {
        public GameMetadata(string gameId, Player player, Player otherPlayer)
        {
            GameId = gameId;
            Player = player;
            OtherPlayer = otherPlayer;
        }

        public string GameId { get; }

        public Player Player { get; }

        public Player OtherPlayer { get; }

        public string ReceivingQueue => $"game-{GameId}-receive-{Player}";

        public string ResponseQueue => $"game-{GameId}-response-{Player}";

        public string UtilityQueue => $"game-{GameId}-utility-{Player}";

        public string Exchange => $"game-{GameId}";

        public string ReceivingRoutingKeyIn => $"{OtherPlayer}.receive";

        public string ResponseRoutingKeyIn => $"{OtherPlayer}.response";

        public string UtilityRountingKeyIn => $"{OtherPlayer}.utility";

        public string ReceivingRoutingKeyOut => $"{Player}.receive";

        public string ResponseRoutingKeyOut => $"{Player}.response";

        public string UtilityRountingKeyOut => $"{Player}.utility";
    }
}
