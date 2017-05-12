using System;

namespace ThereMustBeAnotherWay.GameData
{
    [Serializable]
    public class SettingsFile
    {
        public int ResolutionWidth = TMBAW_Game.DefaultResWidth;
        public int ResolutionHeight = TMBAW_Game.DefaultResHeight;
        public bool IsFullscreen = false;
        public float SoundVolume = .5f;
        public float MusicVolume = .5f;
    }
}
