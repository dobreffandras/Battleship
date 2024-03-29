﻿using Battleship.Model;
using Battleship.Services;

namespace Battleship.Commands
{
    internal class ShootCommand : BaseCommand
    {
        private readonly GameMetadata gameMeta;
        private readonly (char x, char y) coords;
        private readonly CommunicationService communicationService;

        public ShootCommand(
            GameMetadata gameMeta,
            (char x, char y) coords, 
            CommunicationService communicationService)
        {
            this.gameMeta = gameMeta;
            this.coords = coords;
            this.communicationService = communicationService;
        }

        public override bool CanExecute(object? parameter) => true;

        public override void Execute(object? parameter)
        {
            communicationService.Shoot(gameMeta, coords);
        }
    }
}