namespace Battleship.Model
{
    internal class GameModel
    {
        public GameModel(string gameId, Player player, PlayfieldModel playfieldModel)
        {
            GameId = gameId;
            Player = player;
            MyPlayfieldModel = playfieldModel;
            OtherPlayfieldModel = new PlayfieldModel();
            State = new WaitingOpponent();
        }

        public IGameState State { get; set; }

        public string GameId { get; }

        public Player Player { get; }

        public PlayfieldModel MyPlayfieldModel { get; }

        public PlayfieldModel OtherPlayfieldModel { get; }
    }

    internal interface IGameState { }

    internal class WaitingOpponent : IGameState { }

    internal class Playing : IGameState { }

    internal class GameOver : IGameState { }
}
