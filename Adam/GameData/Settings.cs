using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThereMustBeAnotherWay
{
    /// <summary>
    /// This public class stores the settings of the game.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The SamplerState the user wants.
        /// </summary>
        public SamplerState DesiredSamplerState { get; set; }

        /// <summary>
        /// The user's monitor resolution.
        /// </summary>
        public Vector2 DesiredMonitorResolution { get; set; }

        /// <summary>
        /// Whether the user wants lighting or not.
        /// </summary>
        public bool DesiredLight { get; set; }

        /// <summary>
        /// Whether the player wants the game to be fullscreen or not.
        /// </summary>
        public bool IsFullscreen { get; set; }

        /// <summary>
        /// If the settings have been modified, then the file needs to be written again.
        /// </summary>
        public bool HasChanged { get; set; }

        /// <summary>
        /// If the graphical settings have been changed, then it needs to be restarted.
        /// </summary>
        public bool NeedsRestart { get; set; }

        /// <summary>
        /// This is the public class that is saved to the user's computer and is retrieved when the game starts. If no file is found, then a new instance will be created.
        /// </summary>
        public Settings()
        {
            DesiredSamplerState = SamplerState.PointClamp;
            DesiredLight = true;
            IsFullscreen = false;
        }

    }
}
