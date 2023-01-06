namespace Battleship.Services
{
    record class LobbyMessage(MessageType Type, string GameGuid);

    internal enum MessageType
    {
        NewGame, GameDisappeared
    }
}
