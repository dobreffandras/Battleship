using Battleship.Commands;
using Battleship.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Battleship.Components
{
    internal class PlayingFieldViewModel : BaseViewModel
    {
        private readonly PlayfieldModel model;

        public PlayingFieldViewModel(
            PlayfieldModel model,
            PlayingType playingType,
            CommunicationService communicationService)
        {
            this.model = model;
            PlayingType = playingType;
            ShootCommands = model.CellCoordinates
                .ToDictionary<(char, char), string, ICommand>(
                    c => $"{c.Item1}{c.Item2}",
                    c => new ShootCommand(c, communicationService));
        }

        internal void ShootOn(char x, char y)
        {
            model.ShootOn(x, y);
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
