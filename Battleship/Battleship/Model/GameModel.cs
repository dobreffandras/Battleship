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

        public void GameOver()
        {
            State = State switch
            {
                Playing s => s.GameOver(),
                _ => throw new InvalidOperationException($"{nameof(GameOver)} method cannot be called in the current state of the game."),
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

            if (MyPlayfieldModel.AllShipsSunk)
            {
                GameOver();
            }
            else if (shootState != ShootState.Hit)
            {
                ChangeTurn();
            }

            return (isShippart, shootState);
        }

        public void ReceiveResponseForShoot(char x, char y, bool isShippart, ShootState shootState)
        {
            OtherPlayfieldModel.SetCell(x, y, isShippart, shootState);
            if (OtherPlayfieldModel.AllShipsSunk)
            {
                GameOver();
            }
            else if (shootState != ShootState.Hit)
            {
                ChangeTurn();
            }
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

        public Player Current { get; set; }

        public Player OnTurn { get; set; }

        public bool IsCurrentPlayerOnTurn { get => Current == OnTurn; }

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

        public IGameState GameOver()
        {
            return new GameOver(Current, OnTurn);
        }
    }

    internal record GameOver(Player CurrentPlayer, Player Winner) : IGameState
    {
        public string Text => CurrentPlayer == Winner ? "You are the winner!" : "You've lost";
    }
}
