using Battleship.Services;
using System;

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

        public void OpponentConnected()
        {
            State = State switch 
            {
                WaitingOpponent s => s.MoveToPlayingState(currentPlayer: Player.PlayerOne),
                _ => throw new InvalidOperationException(), // TODO Specify error
            };
        }

        public void ConnectionAccepted()
        {
            State = State switch
            {
                WaitingOpponent s => s.MoveToPlayingState(currentPlayer: Player.PlayerTwo),
                _ => throw new InvalidOperationException(), // TODO Specify error
            };
        }

        public void ChangeTurn() 
        {
            State = State switch
            {
                Playing s => s.ChangePlayerOnTurn(),
                _ => throw new InvalidOperationException(), // TODO Specify error
            };
        }

        public void OpponentLeft()
        {
            // TODO 
        }

        public (bool isShippart, ShootState shootState) ReceiveShoot(char x, char y)
        {
            MyPlayfieldModel.ShootOn(x, y);
            var isShippart = MyPlayfieldModel.Shipparts[(x, y)];
            var shootState = MyPlayfieldModel.ShootStates[(x, y)];
            ChangeTurn();
            return (isShippart, shootState);
        }
        
        public void ReceiveResponseForShoot(char x, char y, bool isShippart, ShootState shootState)
        {
            OtherPlayfieldModel.SetCell(x, y, isShippart, shootState);
            ChangeTurn();
        }
    }

    internal interface IGameState
    {
        public string Text { get; }
    }

    internal class WaitingOpponent : IGameState
    {
        public string Text => "Waiting Opponent";

        public IGameState MoveToPlayingState(Player currentPlayer)
        {
            return new Playing(currentPlayer);
        }
    }

    internal class Playing : IGameState
    {
        public Playing(Player currentPlayer, Player onTurn = Player.PlayerOne)
        {
            Current = currentPlayer;
            OnTurn = onTurn;
        }

        Player Current  { get; set; }

        Player OnTurn { get; set; }
        
        public string Text => Current == OnTurn ? "Your turn" : "Opponent's turn";

        public IGameState ChangePlayerOnTurn() 
        { 
            if (OnTurn == Player.PlayerOne) 
            { 
                return new Playing(Current, Player.PlayerTwo);
            }
            else
            {
                return new Playing(Current, Player.PlayerOne);
            }
        }
    }

    internal class GameOver : IGameState
    {
        public string Text => "Game Over";
    }
}
