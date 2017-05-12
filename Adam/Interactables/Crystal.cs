﻿using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;

namespace ThereMustBeAnotherWay.Interactables
{
    public class Crystal
    {
        Rectangle _collRectangle;
        bool _broken;
        SoundFx _breakSound;
        Tile _sourceTile;
        byte _gemId;

        public Crystal(Tile sourceTile, byte gemId)
        {
            this._gemId = gemId;
            this._sourceTile = sourceTile;

            sourceTile.OnTileDestroyed += SourceTile_OnTileDestroyed;
            sourceTile.OnTileUpdate += Update;

            _collRectangle = sourceTile.DrawRectangle;

            int rand = TMBAW_Game.Random.Next(1, 9);
            _breakSound = new SoundFx("Sounds/Crystal/Glass_0" + rand, GameWorld.Player);
        }

        private void SourceTile_OnTileDestroyed(Tile t)
        {
            _sourceTile.OnTileUpdate -= Update;
            _sourceTile.OnTileUpdate -= SourceTile_OnTileDestroyed;
        }

        public void Update(Tile t)
        {
            Player player = GameWorld.Player;

            if (player.GetCollRectangle().Intersects(_collRectangle) && !_broken)
            {
                _breakSound.Play();
                _broken = true;
                Gem.GenerateIdentical(_gemId, _sourceTile, TMBAW_Game.Random.Next(4, 8));
                _sourceTile.ResetToDefault();
            }
        }
    }
}
