using Adam.PlayerCharacter;
using Microsoft.Xna.Framework;

namespace Adam
{
    public class TriggerZone
    {
        Rectangle _rectangle;
        int _id;
        int _incrementinGameWorldidth;
        int _incrementingHeight;

        public TriggerZone(int x, int y, int id)
        {
            this._id = id;
            _rectangle = new Rectangle(x, y, 1, 1);
        }

        public void IncreaseDimensions(int x, int y)
        {

        }

        public void Update(Player player, GameMode currentLevel)
        {
        }


    }
}
