namespace Battleship.Services
{
    internal static class RabbitMqConstants
    {
        internal static class ExchangeNames
        {
            internal const string OPEN_GAMES = "open_games";
        }

        internal static class UtilityMessages 
        {
            internal const string OPPONENT_CONNECTED = "opponentConnected";
            internal const string CONNECTION_ACCEPTED = "connectionAccepted";
            internal const string OPPONENT_LEFT = "opponentLeft";
        }

        internal class GameInfo 
        {
            private readonly string gameId;
            private readonly string player;
            private readonly string otherPlayer;

            public GameInfo(string gameId, string player, string otherPlayer)
            {
                this.gameId = gameId;
                this.player = player;
                this.otherPlayer = otherPlayer;
            }

            public string ReceivingQueue => $"game-{gameId}-receive-{player}";

            public string ResponseQueue => $"game-{gameId}-response-{player}";

            public string UtilityQueue => $"game-{gameId}-utility-{player}";

            public string Exchange => $"game-{gameId}";

            public string ReceivingRoutingKeyIn => $"{otherPlayer}.receive";
            
            public string ResponseRoutingKeyIn => $"{otherPlayer}.response";
            
            public string UtilityRountingKeyIn => $"{otherPlayer}.utility";

            public string ReceivingRoutingKeyOut => $"{player}.receive";
            
            public string ResponseRoutingKeyOut => $"{player}.response";
            
            public string UtilityRountingKeyOut => $"{player}.utility";
        }
    }
}
