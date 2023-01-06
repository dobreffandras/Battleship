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

    internal interface IGameState
    {
        public string Text { get; }
    }

    internal class WaitingOpponent : IGameState
    {
        public string Text => "Waiting Opponent";
    }

    internal class Playing : IGameState
    {
        public string Text => "Your Turn";
    }

    internal class GameOver : IGameState
    {
        public string Text => "Game Over";
    }
}
