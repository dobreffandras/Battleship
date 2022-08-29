using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    internal class LobbyViewModel
    {
        public LobbyViewModel()
        {
            OpenGames = new List<string> { Guid.NewGuid().ToString() };
        }

        public IEnumerable<string> OpenGames { get; set; }
    }
}
