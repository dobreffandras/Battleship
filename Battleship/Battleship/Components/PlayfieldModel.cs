using System;
using System.Collections.Generic;

namespace Battleship.Components
{
    public class PlayfieldModel : Dictionary<(char, char), bool>
    {
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
                    this[(x, y)] = false;
                }
            }
        }
    }
}