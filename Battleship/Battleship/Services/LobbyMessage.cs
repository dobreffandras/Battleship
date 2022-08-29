using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Services
{
    record class LobbyMessage(MessageType Type, string GameGuid);

    internal enum MessageType
    {
        NewGame, GameDisappeared
    }
}
