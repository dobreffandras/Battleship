using Battleship.Commands;
using Battleship.Model;
using Battleship.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Battleship.Components
{
    internal class PlayingFieldViewModel : BaseViewModel
    {
        private readonly PlayfieldModel model;
        private readonly CommunicationService communicationService;

        public PlayingFieldViewModel(
            PlayfieldModel model,
            PlayingType playingType,
            CommunicationService communicationService)
        {
            this.model = model;
            PlayingType = playingType;
            this.communicationService = communicationService;
            ShootCommands = model.CellCoordinates
                .ToDictionary<(char, char), string, ICommand>(
                    c => $"{c.Item1}{c.Item2}",
                    c => new ShootCommand(c, communicationService));
        }

        internal void ShootOn(char x, char y)
        {
            // TODO move logic to GameModel ?
            model.ShootOn(x, y);
            NotifyPropertyChanged(nameof(ShootStates));
            var isShippart = model.Shipparts[(x, y)];
            var shootState = model.ShootStates[(x, y)];
            communicationService.Respond((x, y), isShippart, shootState);
        }

        internal void SetCell(char x, char y, bool isShippart, ShootState shootState)
        {
            model.SetCell(x, y, isShippart, shootState);
            NotifyPropertyChanged(nameof(Shipparts));
            NotifyPropertyChanged(nameof(ShootStates));
        }

        public IDictionary<string, string> Shipparts 
            => model.Shipparts.ToDictionary(
                kv => $"{kv.Key.Item1}{kv.Key.Item2}", 
                kv => kv.Value.ToString());
        
        public IDictionary<string, ShootState> ShootStates
            => model.ShootStates.ToDictionary(
                kv => $"{kv.Key.Item1}{kv.Key.Item2}",
                kv => kv.Value);

        public IDictionary<string, ICommand> ShootCommands { get; }

        public PlayingType PlayingType { get; }
    }
}
