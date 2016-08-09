using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Adam.Levels
{
    public class WorldData
    {
        public Song Song;
        private SoundFx _ambience;
        public string LevelName = "";
        public bool HasClouds;
        public bool IsRaining;
        public bool IsSnowing;

        bool _obj0;
        bool _obj1;
        bool _obj2;
        bool _obj3;
        bool _obj4;

        public bool Trigger0;
        public bool Trigger1;
        public bool Trigger2;
        public bool Trigger3;
        public bool Trigger4;
        public bool Trigger5;
        public bool Trigger6;

        bool _privTrig0;

        bool _editMode;
        public bool IsDealinGameWorldithData { get; set; }

        public byte[] TileIDs { get; set; }
        public byte[] WallIDs { get; set; }

        public int LevelWidth { get; set; }
        public int LevelHeight { get; set; }
        public byte BackgroundId { get; set; }
        public byte SoundtrackId { get; set; }

        public Vector2 SpawnPoint { get; set; }

        public string[] MetaData { get; set; }

        public WorldData()
        {
            LevelEditor.Brush.SizeChanged += Brush_SizeChanged;
        }

        private void Brush_SizeChanged()
        {
            _privTrig0 = true;
        }

        public void Update()
        {
            _ambience?.PlayIfStopped();
        }
    }
}
