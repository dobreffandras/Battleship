using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship.Components
{
    public class PlayfieldModel
    {
        private readonly Dictionary<(char, char), (bool IsShippart, ShootState ShootState)> cells = new();

        public PlayfieldModel()
        {
            Fillup();
        }

        private void Fillup()
        {
            foreach(var x in new[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' })
            {
                for(var i = 0; i < 10; i++)
                {
                    var y = i.ToString()[0];
                    cells[(x, y)] = (false, ShootState.None);
                }
            }
        }

        public void ToggleShippart(char x, char y)
        {
            var cell = cells[(x, y)];

            cells[(x, y)] = (!cell.IsShippart, cell.ShootState);
        }

        internal void ShootOn(char x, char y)
        {
            var cell = cells[(x, y)];
            var shootState = 
                cell.IsShippart 
                ? ShootState.Hit 
                : ShootState.Miss;

            cells[(x, y)] = (cell.IsShippart, shootState);
        }

        public bool IsPrepared => cells.Count(kv => kv.Value.IsShippart) == 20;

        public IReadOnlyCollection<(char, char)> CellCoordinates => cells.Keys;

        public IDictionary<(char, char), ShootState> ShootStates 
            => cells.ToDictionary(x => x.Key, x => x.Value.ShootState);
        
        public IDictionary<(char, char), bool> Shipparts 
            => cells.ToDictionary(x => x.Key, x => x.Value.IsShippart);
    }

    public enum ShootState
    {
        None,
        Hit,
        Miss
    }
}