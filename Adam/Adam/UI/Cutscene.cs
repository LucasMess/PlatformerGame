using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam
{
    public class Cutscene
    {
        Video _video;
        VideoPlayer _videoPlayer;
        Texture2D _videoTexture;
        public bool PlayIntro, WasPlayed;

        public Cutscene()
        {
            _videoPlayer = new VideoPlayer();
            PlayIntro = true;
        }

        public void Load(ContentManager content)
        {
           _video = content.Load<Video>("Cutscenes/cutscene_narration_sammy_sfx_st_xna");
        }

        public void Update(GameState currentGameState)
        {
            if (PlayIntro == true)
            {
                _videoPlayer.Play(_video);
                PlayIntro = false;
            }
            if (_videoPlayer.State == MediaState.Stopped)
            {
                Stop();
            }
            else _videoTexture = _videoPlayer.GetTexture();
        }

        public void Stop()
        {
            _videoPlayer.Stop();
            _videoTexture = null;
            WasPlayed = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_videoTexture != null)
            {
                spriteBatch.Draw(_videoTexture, new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight), Color.White);
            }
        }


    }
}
