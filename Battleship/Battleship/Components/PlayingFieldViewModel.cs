using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship.Components
{
    internal class PlayingFieldViewModel : BaseViewModel
    {
        private readonly PlayfieldModel model;

        public PlayingFieldViewModel(PlayfieldModel model)
        {
            this.model = model;
        }

        internal void ShootOn(char x, char y)
        {
            model.ShootOn(x, y);
        }

        public IDictionary<string, string> Shipparts 
            => model.Shipparts.ToDictionary(
                kv => $"{kv.Key.Item1}{kv.Key.Item2}", 
                kv => kv.Value.ToString());
    }
}
