using Battleship.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Services
{
    internal record class GameMessage(char X, char Y);

    internal record class GameResponseMessage(char X, char Y, bool IsShippart, ShootState ShootState);
}
