using System;

namespace Adam.GameData
{
    [Serializable]
    public class SettingsFile
    {
        public int ResolutionWidth = AdamGame.DefaultResWidth;
        public int ResolutionHeight = AdamGame.DefaultResHeight;
        public bool IsFullscreen = false;
        public float SoundVolume = .5f;
        public float MusicVolume = .5f;
    }
}
