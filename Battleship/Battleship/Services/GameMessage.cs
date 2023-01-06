using Battleship.Model;

namespace Battleship.Services
{
    internal record class ShootMessage(char X, char Y);

    internal record class ShootResponseMessage(char X, char Y, bool IsShippart, ShootState ShootState);
}
