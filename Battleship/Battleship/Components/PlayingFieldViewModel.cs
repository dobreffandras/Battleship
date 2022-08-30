using System;

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
    }
}
