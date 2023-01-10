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

        public PlayingFieldViewModel(
            GameMetadata gameMeta,
            PlayfieldModel model,
            PlayingType playingType,
            CommunicationService communicationService)
        {
            this.model = model;
            PlayingType = playingType;
            ShootCommands = model.CellCoordinates
                .ToDictionary<(char, char), string, ICommand>(
                    c => $"{c.Item1}{c.Item2}",
                    c => new ShootCommand(gameMeta, c, communicationService));
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

        internal void NotifyAllPropertiesChanged()
        {
            NotifyPropertyChanged(string.Empty); // Notify for all
        }
    }
}
