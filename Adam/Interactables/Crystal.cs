using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;
using System.Linq;

namespace ThereMustBeAnotherWay.Interactables
{
    public class Crystal : Interactable
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

            _collRectangle = sourceTile.DrawRectangle;

            int rand = TMBAW_Game.Random.Next(1, 9);
            _breakSound = new SoundFx("Sounds/Crystal/Glass_0" + rand, GameWorld.GetPlayers()[0]);
            Light = new Light(new Vector2(_collRectangle.X + 16, _collRectangle.Y + 16), Light.MaxLightLevel, Gem.GetGemColor(gemId), false);
            Initialize();
        }

        public override void Update(Tile tile)
        {
            foreach (var proj in GameWorld.EnemyProjectiles.Concat(GameWorld.PlayerProjectiles))
            {
                if (proj.GetCollRectangle().Intersects(_collRectangle))
                {
                    OnPlayerAction(tile, GameWorld.GetPlayers()[0]);
                }
            }

            base.Update(tile);
        }

        public override void OnPlayerAction(Tile tile, Player player)
        {
            if (!_broken)
            {
                _breakSound.Play();
                _broken = true;
                Gem.GenerateIdentical(_gemId, _sourceTile, TMBAW_Game.Random.Next(4, 8));
                tile.IsHidden = true;
                LightingEngine.RemoveDynamicLight(Light);
            }

            base.OnPlayerAction(tile, player);

        }
    }
}
