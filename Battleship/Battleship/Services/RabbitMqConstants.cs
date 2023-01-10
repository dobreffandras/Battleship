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
    }
}
