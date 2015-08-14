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
        Video video;
        VideoPlayer videoPlayer;
        Texture2D videoTexture;
        public bool playIntro, wasPlayed;

        public Cutscene()
        {
            videoPlayer = new VideoPlayer();
            playIntro = true;
        }

        public void Load(ContentManager Content)
        {
           video = Content.Load<Video>("Cutscenes/cutscene_narration_sammy_sfx_st_xna");
        }

        public void Update(GameState currentGameState)
        {
            if (playIntro == true)
            {
                videoPlayer.Play(video);
                playIntro = false;
            }
            if (videoPlayer.State == MediaState.Stopped)
            {
                Stop();
            }
            else videoTexture = videoPlayer.GetTexture();
        }

        public void Stop()
        {
            videoPlayer.Stop();
            videoTexture = null;
            wasPlayed = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (videoTexture != null)
            {
                spriteBatch.Draw(videoTexture, new Rectangle(0, 0, Main.DefaultResWidth, Main.DefaultResHeight), Color.White);
            }
        }


    }
}
