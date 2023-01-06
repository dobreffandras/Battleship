namespace Battleship.Model
{
    internal class GameModel
    {
        public GameModel(string gameId, PlayfieldModel playfieldModel)
        {
            GameId = gameId;
            PlayfieldModel = playfieldModel;
            State = new WaitingOpponent();
        }

        public IGameState State { get; set; }

        public string GameId { get; }

        public PlayfieldModel PlayfieldModel { get; }
    }

    internal interface IGameState { }

    internal class WaitingOpponent : IGameState { }

    internal class Playing : IGameState { }

    internal class GameOver : IGameState { }
}
