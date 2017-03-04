using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.Misc.Interfaces;
using Adam.PlayerCharacter;
using Microsoft.Xna.Framework;
using System;

namespace Adam.Interactables
{
    public class CheckPoint : Interactable
    {
        public CheckPoint(Tile tile)
        {
            tile.AnimationStopped = true;
        }

        public override void OnEntityTouch(Tile tile, Entity entity)
        {
            if (entity is Player)
            {
                Player player = (Player)entity;
                player.SetRespawnPoint(tile.DrawRectangle.X, tile.DrawRectangle.Y);
                tile.AnimationStopped = false;
                base.OnEntityTouch(tile, entity);
            }

        }
    }
}
