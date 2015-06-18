using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Adam;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

using Adam.Enemies;
using System.Threading;
using Adam.Interactables;
using Adam.Obstacles;
using Adam.Network;
using Adam.Characters.Enemies;

namespace Adam
{
    class Map
    {
        //Basic tile grid and the visible tile grid
        public Tile[] tileArray, wallArray;
        int[] visibleTileArray = new int[30 * 50];
        int[] visibleLightArray = new int[60 * 100];
        Light[] lightArray;
        Light playerLight;

        private Level CurrentLevel;

        public Texture2D mapTexture, wall;
        public Player player;
        public Apple apple;
        Background background = new Background();
        BackGroundImage blackCorners = new BackGroundImage();
        PopUp popUp = new PopUp();
        public GameTimer timer;
        int rows;
        int columns;
        int tileSize;
        int enemyTilePos;
        int projectileTilePos;
        int gemTilePos;
        public double fireTimer;
        bool wantClouds;
        public bool isPaused;
        public bool levelComplete;
        bool isRunningOutOfTime, isPlayingFastTheme;
        public static Random randGen = new Random();
        Song levelTheme, fastLevelTheme;

        public List<Cloud> cloudList = new List<Cloud>();
        public List<Gem> gemList = new List<Gem>();
        public List<Chest> chestList = new List<Chest>();
        public List<Enemy> enemyList = new List<Enemy>();
        public List<Particle> effectList = new List<Particle>();
        public List<PlayerWeaponProjectile> projectileList = new List<PlayerWeaponProjectile>();
        public List<Climbables> climbablesList = new List<Climbables>();
        public List<Tech> techList = new List<Tech>();
        public List<AnimatedTile> animatedTileList = new List<AnimatedTile>();
        public List<Door> doorList = new List<Door>();
        public List<Key> keyList = new List<Key>();
        public List<NonPlayableCharacter> noobList = new List<NonPlayableCharacter>();
        public List<Entity> entities = new List<Entity>();
        Weapon weapon = new Weapon();
        ContentManager Content;
        public GameTime gameTime;
        GraphicsDevice GraphicsDevice;
        Vector2 monitorRes;

        SoundEffect hurryUpSound;
        SoundEffectInstance hurryUpInstance;
        double walkingSoundTimer;

        public Map() { }

        public Map(GraphicsDevice GraphicsDevice, Vector2 monitorRes)
        {
            this.GraphicsDevice = GraphicsDevice;
            tileSize = Game1.Tilesize;
            this.monitorRes = monitorRes;

            player = new Player();
        }

        public void Load(ContentManager Content, Vector2 monitorResolution, Player player, Level CurrentLevel)
        {
            this.Content = Content;
            weapon.Load();
            this.player = player;
            popUp.Load(Content);

            this.CurrentLevel = CurrentLevel;
            timer = new GameTimer(180);
            MediaPlayer.Volume = .2f;

            int maxClouds = 5;
            for (int i = 0; i < maxClouds; i++)
            {
                cloudList.Add(new Cloud(Content, monitorResolution, maxClouds, i));
            }

            //Load the texture file to create the map according to the game level.
            switch (CurrentLevel)
            {
                case Level.Level0:
                    break;
                case Level.Level1:
                    mapTexture = Content.Load<Texture2D>("Levels/1-1_main");
                    wall = Content.Load<Texture2D>("Levels/1-1_wall");
                    levelTheme = Content.Load<Song>("Music/Vivacity");
                    fastLevelTheme = ContentHelper.LoadSong("Music/Vivacity x60");
                    timer = new GameTimer(300);
                    wantClouds = true;
                    break;
                case Level.Level2:
                    mapTexture = Content.Load<Texture2D>("Levels/2-1_main");
                    wall = Content.Load<Texture2D>("Levels/2-1_wall");
                    levelTheme = Content.Load<Song>("Music/Desert City");
                    //fastLevelTheme = ContentHelper.LoadSong("Music/Vivacity x60");
                    timer = new GameTimer(300);
                    wantClouds = true;
                    break;
                case Level.Level3:
                    mapTexture = Content.Load<Texture2D>("Levels/debug_main");
                    wall = Content.Load<Texture2D>("Levels/debug_wall");
                    levelTheme = Content.Load<Song>("Music/Heart of Nowhere");
                    wantClouds = true;
                    break;
                case Level.Level4:

                    break;

            }

            if (mapTexture == null || wall == null)
                return;
            columns = mapTexture.Width;
            rows = mapTexture.Height;

            tileArray = new Tile[columns * rows];
            wallArray = new Tile[columns * rows];

            LoadGrid(tileArray, mapTexture);
            LoadGrid(wallArray, wall);
            LoadLights();

            playerLight = new Light();
            playerLight.Load(Content);
            //LoadChunks();

            hurryUpSound = ContentHelper.LoadSound("Sounds/hurryUp");
            hurryUpInstance = hurryUpSound.CreateInstance();

            background.Load(Content, CurrentLevel, monitorResolution, this);
            blackCorners.Texture = ContentHelper.LoadTexture("Backgrounds/blackCorners");
            blackCorners.Rectangle = new Rectangle(0, 0, Game1.DefaultResWidth, Game1.DefaultResHeight);


            if (levelTheme != null)
                MediaPlayer.Play(levelTheme);

        }

        public void LoadGrid(Tile[] array, Texture2D data)
        {
            int currentTileNumber = 0;

            //Create basic grid where all block are transparent and not differentiated
            for (int r = 1; r <= rows; r++)
            {
                for (int c = 1; c <= columns; c++)
                {
                    array[currentTileNumber] = new Tile();
                    array[currentTileNumber].TileIndex = currentTileNumber;
                    currentTileNumber++;
                }
            }

            //Check the pixels and differentiate tiles based off their color
            int totalPixelCount = columns * rows;
            Color[] tilePixels = new Color[totalPixelCount];
            data.GetData<Color>(tilePixels);

            for (int i = 0; i < totalPixelCount; i++)
            {
                Tile tile = array[i];
                Color pixel = tilePixels[i];
                Vector3 colorCode = new Vector3(pixel.R, pixel.G, pixel.B);
                int Xcoor = (i % columns) * tileSize;
                int Ycoor = ((i - (i % columns)) / columns) * tileSize;

                tile.rectangle = new Rectangle(Xcoor, Ycoor, tileSize, tileSize);

                if (colorCode == new Vector3(0, 189, 31)) //grass
                {
                    tile.ID = 1;
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(220, 220, 220)) //Stone
                {
                    tile.ID = 2;
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(255, 255, 255)) //marble
                {
                    tile.ID = 3;
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(189, 13, 13)) //hellrock
                {
                    tile.ID = 4;
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(255, 222, 180)) //sand
                {
                    tile.ID = 5;
                    tile.isSolid = true;
                }
                //6 vacant
                //7 shortgrass
                else if (colorCode == new Vector3(78, 78, 78)) //metaltile
                {
                    tile.ID = 8;
                    tile.isSolid = true;
                    animatedTileList.Add(new AnimatedTile(8, tile.rectangle));
                }
                //9 tallgrass
                else if (colorCode == new Vector3(225, 127, 0)) //goldBricks
                {
                    tile.ID = 10;
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(249, 64, 45))//torch
                {
                    tile.ID = 11;
                    animatedTileList.Add(new AnimatedTile(11, tile.rectangle));
                }
                else if (colorCode == new Vector3(191, 129, 9)) //chandelier
                {
                    tile.ID = 12;
                    animatedTileList.Add(new AnimatedTile(12, tile.rectangle));
                }
                //13 see doors
                else if (colorCode == new Vector3(35, 138, 52)) //vines
                {
                    tile.ID = 14;
                    climbablesList.Add(new Climbables(Xcoor, Ycoor));
                }
                else if (colorCode == new Vector3(166, 135, 90)) //ladders
                {
                    tile.ID = 15;
                    climbablesList.Add(new Climbables(Xcoor, Ycoor));
                }
                else if (colorCode == new Vector3(103, 112, 118)) //chains
                {
                    tile.ID = 16;
                    climbablesList.Add(new Climbables(Xcoor, Ycoor));
                }
                //17 daffodyls
                else if (colorCode == new Vector3(239, 239, 239)) //marblecolumns
                {
                    tile.ID = 18;
                }
                else if (colorCode == new Vector3(243, 220, 28)) //chest
                {
                    tile.ID = 19;
                    chestList.Add(new Chest(new Vector2(Xcoor, Ycoor), Content, false));
                }
                else if (colorCode == new Vector3(16, 52, 207)) //tech
                {
                    tile.ID = 20;
                    techList.Add(new Tech(Xcoor, Ycoor, Content));
                }
                else if (colorCode == new Vector3(217, 97, 9)) //scaffolding
                {
                    tile.ID = 21;
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(224, 58, 0)) // Spikes
                {
                    tile.ID = 22;
                    entities.Add(new Spikes(Xcoor, Ycoor));
                }
                //23 24 25
                else if (colorCode == new Vector3(244, 121, 0)) // Lava
                {
                    array[i] = new AnimatedTile(24, tile.rectangle);
                    entities.Add(new Lava(Xcoor, Ycoor));
                }
                else if (colorCode == new Vector3(255, 255, 0)) // Golden Apple
                {
                    tile.ID = 26;
                    apple = new Apple(Xcoor, Ycoor);
                }
                else if (colorCode == new Vector3(255, 244, 147)) // Golden Apple
                {
                    tile.ID = 27;
                    chestList.Add(new Chest(new Vector2(Xcoor, Ycoor), Content, true));
                }


                //CHARACTERS AND OTHERS
                else if (colorCode == new Vector3(0, 255, 0)) //player
                {
                    player.Initialize(Xcoor, Ycoor);
                }
                else if (colorCode == new Vector3(81, 103, 34)) //snake
                {
                    enemyList.Add(new Hellboar(Xcoor, Ycoor));
                   // enemyList.Add(new SnakeEnemy(Xcoor, Ycoor, Content, this));
                }
                else if (colorCode == new Vector3(143, 148, 0)) //potato
                {
                    enemyList.Add(new PotatoEnemy(Xcoor, Ycoor, Content));
                }
                else if (colorCode == new Vector3(15, 74, 225)) //NPC
                {
                    noobList.Add(new NonPlayableCharacter(Xcoor, Ycoor, 1, Content, randGen.Next(), monitorRes));
                }
                else if (colorCode == new Vector3(241, 22, 233)) //falling boulder
                {
                    entities.Add(new FallingBoulder(Xcoor, Ycoor));
                }
                else if (colorCode == new Vector3(116, 143, 220)) //Platform Wide Slow Up
                {
                    bool found = false;
                    foreach (WidePlatformUp wi in entities.OfType<WidePlatformUp>())
                    {
                        found = true;
                        wi.SetStartPoint(Xcoor, Ycoor);
                        break;
                    }
                    if (!found)
                    {
                        WidePlatformUp wi = new WidePlatformUp(player);
                        wi.SetStartPoint(Xcoor, Ycoor);
                        entities.Add(wi);
                    }
                }
                else if (colorCode == new Vector3(116, 143, 221)) //Platform Wide Slow Up END
                {
                    bool found = false;
                    foreach (WidePlatformUp wi in entities.OfType<WidePlatformUp>())
                    {
                        found = true;
                        wi.SetEndPoint(Xcoor, Ycoor);
                        break;
                    }
                    if (!found)
                    {
                        WidePlatformUp wi = new WidePlatformUp(player);
                        wi.SetEndPoint(Xcoor, Ycoor);
                        entities.Add(wi);
                    }
                }


                //WALLS
                else if (colorCode == new Vector3(174, 98, 0)) //goldBricks Wall
                {
                    tile.ID = 100;
                }
                else if (colorCode == new Vector3(174, 174, 174)) //stonewall
                {
                    tile.ID = 101;
                }
                else if (colorCode == new Vector3(131, 68, 0)) //dirtwall
                {
                    tile.ID = 102;
                }
                else if (colorCode == new Vector3(107, 145, 171)) //fences
                {
                    tile.ID = 103;
                }
                else if (colorCode == new Vector3(154, 105, 11)) //marblewall
                {
                    tile.ID = 105;
                }
                else if (colorCode == new Vector3(154, 105, 11)) //sandwall
                {
                    tile.ID = 105;
                }

                else if (colorCode == new Vector3(191, 81, 0)) //door secret 1
                {
                    doorList.Add(new Door(Xcoor, Ycoor, Content, 1, i, monitorRes));
                    tile.ID = 13;
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(191, 81, 1)) //door secret 2
                {
                    tile.ID = 13;
                    doorList.Add(new Door(Xcoor, Ycoor, Content, 2, i, monitorRes));
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(191, 81, 2)) //door secret 3
                {
                    tile.ID = 13;
                    doorList.Add(new Door(Xcoor, Ycoor, Content, 3, i, monitorRes));
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(246, 255, 0)) //key secret 1
                {
                    keyList.Add(new Key(Xcoor, Ycoor, Content, 1));
                }
                else if (colorCode == new Vector3(246, 255, 1)) //key secret 2
                {
                    keyList.Add(new Key(Xcoor, Ycoor, Content, 2));
                }
                else if (colorCode == new Vector3(246, 255, 2)) //key secret 3
                {
                    keyList.Add(new Key(Xcoor, Ycoor, Content, 3));
                }
            }


            foreach (Tile tile in array)
            {
                if (tile.ID == 1 || tile.ID == 2 || tile.ID == 4 || tile.ID == 5 || tile.ID == 10)
                {
                    tile.FindConnectedTextures(array, mapTexture.Width);
                }
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (i - columns - 1 >= 0 && i + columns + 1 < array.Length)
                {
                    Tile t = array[i];
                    if (t.ID == 1 && t.subID == 0 && array[i - columns].isSolid == false)
                    {
                        int prob = randGen.Next(0, 100);
                        if (prob < 25)
                        {
                            Tile a = array[i - columns];
                            a.ID = 9;
                            a.isVoid = true;
                            animatedTileList.Add(new AnimatedTile(9, a.rectangle));
                        }
                    }

                    //shortgrass
                    if (array[i].ID == 1 && array[i - columns].ID == 0 && array[i].subID == 0)
                    {
                        array[i - columns].ID = 7;
                    }
                    //Fences
                    if (array[i].ID == 103 && array[i - columns].ID != 103)
                    {
                        array[i].subID = 1;
                    }
                }
            }



            //now that all IDs have been given, define all textures for the tiles.
            foreach (Tile tile in array)
            {
                tile.DefineTexture();
            }

            foreach (AnimatedTile tile in animatedTileList)
            {
                tile.DefineTexture();
            }

        }

        public void LoadLights()
        {
            lightArray = new Light[columns * rows];

            for (int i = 0; i < tileArray.Length; i++)
            {
                lightArray[i] = new Light();
                lightArray[i].pos = i;
            }

            foreach (var li in lightArray)
            {
                li.Load(Content);
            }

            foreach (var li in lightArray)
            {
                li.CalculateLighting(tileArray, wallArray, mapTexture);
            }
        }

        public void GemCollision(Gem gem)
        {
            if (gem.velocity.X == 0 && gem.velocity.Y == 0) { }
            else
            {
                gemTilePos = (int)(gem.topMidBound.Y / tileSize * columns) + (int)(gem.topMidBound.X / tileSize);

                int[] q = new int[9];
                q[0] = gemTilePos - mapTexture.Width - 1;
                q[1] = gemTilePos - mapTexture.Width;
                q[2] = gemTilePos - mapTexture.Width + 1;
                q[3] = gemTilePos - 1;
                q[4] = gemTilePos;
                q[5] = gemTilePos + 1;
                q[6] = gemTilePos + mapTexture.Width - 1;
                q[7] = gemTilePos + mapTexture.Width;
                q[8] = gemTilePos + mapTexture.Width + 1;

                //test = q;

                //check the tiles around the goldOre for collision
                foreach (int quadrant in q)
                {
                    if (quadrant >= 0 && quadrant <= tileArray.Length - 1 && tileArray[quadrant].isSolid == true)
                    {
                        if (gem.yRect.Intersects(tileArray[quadrant].rectangle))
                        {
                            if (gem.rectangle.Y < tileArray[quadrant].rectangle.Y) //hits bot
                            {
                                gem.rectangle.Y = tileArray[quadrant].rectangle.Y - gem.rectangle.Height;
                                gem.velocity.Y = -gem.velocity.Y * .9f;
                                gem.velocity.X = 0;
                            }
                            if (gem.rectangle.Y > tileArray[quadrant].rectangle.Y) //hits top
                            {
                                gem.velocity.Y = -gem.velocity.Y * .9f;
                                gem.velocity.X = 0;
                                gem.rectangle.Y = tileArray[quadrant].rectangle.Y + tileArray[quadrant].rectangle.Height + 1;
                            }
                        }
                        if (gem.xRect.Intersects(tileArray[quadrant].rectangle))
                        {
                            if (gem.rectangle.X < tileArray[quadrant].rectangle.X) //hits right
                            {
                                gem.velocity.X = -gem.velocity.X * .9f;
                                gem.velocity.Y = gem.velocity.Y * .9f;
                                gem.rectangle.X = tileArray[quadrant].rectangle.X - gem.rectangle.Width - 1;
                            }
                            if (gem.rectangle.X > tileArray[quadrant].rectangle.X) //hits left
                            {
                                gem.velocity.X = -gem.velocity.X * .9f;
                                gem.velocity.Y = gem.velocity.Y * .9f;
                                gem.rectangle.X = tileArray[quadrant].rectangle.X + tileArray[quadrant].rectangle.Width + 1;
                            }
                        }
                    }
                }
            }
        }

        public void EnemyCollision(Enemy enemy, Player player)
        {
            //Remember to give the enemy its own logic for when it hits the player if it can pass through walls
            if (enemy.canPassThroughWalls)
                return;
            if (Math.Abs(enemy.collRectangle.X) - Math.Abs(player.collRectangle.X) < 2000 && enemy.isDead == false)
            {
                if (player.attackBox.Intersects(enemy.damageBox) && player.velocity.Y >= -1f)
                {
                    enemy.Kill();
                    enemy.CreateDeathEffect();
                    player.Jump();
                    goto SkipDamage;
                }
                else if (player.collRectangle.Intersects(enemy.collRectangle))
                {
                    player.TakeDamageAndKnockBack(enemy.GetTouchDamage());
                    enemy.BeMean();
                }

            SkipDamage:
                enemyTilePos = (int)(enemy.topMidBound.Y / tileSize * columns) + (int)(enemy.topMidBound.X / tileSize);

                int[] q = new int[12];
                q[0] = enemyTilePos - mapTexture.Width - 1;
                q[1] = enemyTilePos - mapTexture.Width;
                q[2] = enemyTilePos - mapTexture.Width + 1;
                q[3] = enemyTilePos - 1;
                q[4] = enemyTilePos;
                q[5] = enemyTilePos + 1;
                q[6] = enemyTilePos + mapTexture.Width - 1;
                q[7] = enemyTilePos + mapTexture.Width;
                q[8] = enemyTilePos + mapTexture.Width + 1;
                q[9] = enemyTilePos + mapTexture.Width + mapTexture.Width - 1;
                q[10] = enemyTilePos + mapTexture.Width + mapTexture.Width;
                q[11] = enemyTilePos + mapTexture.Width + mapTexture.Width + 1;

                //test = q;

                //check the tiles around the enemy for collision
                foreach (int quadrant in q)
                {
                    if (quadrant >= 0 && quadrant <= tileArray.Length - 1 && tileArray[quadrant].isSolid == true)
                    {
                        if (enemy.yRect.Intersects(tileArray[quadrant].rectangle))
                        {
                            if (enemy.drawRectangle.Y < tileArray[quadrant].rectangle.Y) //hits bot
                            {
                                enemy.velocity.Y = 0f;
                                enemy.drawRectangle.Y = tileArray[quadrant].rectangle.Y - enemy.drawRectangle.Height;
                                enemy.isFlying = false;
                            }
                            if (enemy.drawRectangle.Y > tileArray[quadrant].rectangle.Y) //hits top
                            {
                                enemy.velocity.Y = 0f;
                                enemy.drawRectangle.Y = tileArray[quadrant].rectangle.Y + tileArray[quadrant].rectangle.Height + 1;
                            }
                        }
                        if (enemy.xRect.Intersects(tileArray[quadrant].rectangle))
                        {
                            if (enemy.drawRectangle.X < tileArray[quadrant].rectangle.X) //hits right
                            {
                                enemy.velocity.X = 0;
                                enemy.drawRectangle.X = tileArray[quadrant].rectangle.X - enemy.drawRectangle.Width - 1;
                                enemy.needsToJump = true;
                            }
                            if (enemy.drawRectangle.X > tileArray[quadrant].rectangle.X) //hits left
                            {
                                enemy.velocity.X = 0;
                                enemy.drawRectangle.X = tileArray[quadrant].rectangle.X + tileArray[quadrant].rectangle.Width + 1;
                                enemy.needsToJump = true;
                            }
                        }
                    }
                }
            }

        }

        public void NoobCollision(NonPlayableCharacter noob)
        {
            int noobTilePos = (int)(noob.topMidBound.Y / tileSize * columns) + (int)(noob.topMidBound.X / tileSize);

            int[] q = new int[12];
            q[0] = noobTilePos - mapTexture.Width - 1;
            q[1] = noobTilePos - mapTexture.Width;
            q[2] = noobTilePos - mapTexture.Width + 1;
            q[3] = noobTilePos - 1;
            q[4] = noobTilePos;
            q[5] = noobTilePos + 1;
            q[6] = noobTilePos + mapTexture.Width - 1;
            q[7] = noobTilePos + mapTexture.Width;
            q[8] = noobTilePos + mapTexture.Width + 1;
            q[9] = noobTilePos + mapTexture.Width + mapTexture.Width - 1;
            q[10] = noobTilePos + mapTexture.Width + mapTexture.Width;
            q[11] = noobTilePos + mapTexture.Width + mapTexture.Width + 1;

            //test = q;

            //check the tiles around the noob for collision
            foreach (int quadrant in q)
            {
                if (quadrant >= 0 && quadrant <= tileArray.Length - 1 && tileArray[quadrant].isSolid == true)
                {
                    if (noob.yRect.Intersects(tileArray[quadrant].rectangle))
                    {
                        if (noob.collRectangle.Y < tileArray[quadrant].rectangle.Y) //hits bot
                        {
                            noob.velocity.Y = 0f;
                            noob.collRectangle.Y = tileArray[quadrant].rectangle.Y - noob.collRectangle.Height;
                            noob.isFlying = false;
                        }
                        if (noob.collRectangle.Y > tileArray[quadrant].rectangle.Y) //hits top
                        {
                            noob.velocity.Y = 0f;
                            noob.collRectangle.Y = tileArray[quadrant].rectangle.Y + tileArray[quadrant].rectangle.Height + 1;
                        }
                    }
                    if (noob.xRect.Intersects(tileArray[quadrant].rectangle))
                    {
                        if (noob.collRectangle.X < tileArray[quadrant].rectangle.X) //hits right
                        {
                            noob.velocity.X = 0;
                            noob.collRectangle.X = tileArray[quadrant].rectangle.X - noob.collRectangle.Width - 1;
                            noob.jumpStartTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                        }
                        if (noob.collRectangle.X > tileArray[quadrant].rectangle.X) //hits left
                        {
                            noob.velocity.X = 0;
                            noob.collRectangle.X = tileArray[quadrant].rectangle.X + tileArray[quadrant].rectangle.Width + 1;
                            noob.jumpStartTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                        }
                    }
                }
            }
            if (noob.jumpStartTimer > 200)
            {
                noob.needsToJump = true;
                noob.jumpStartTimer = 0;
            }
        }

        public void PlayWalkingSounds()
        {
            //walkingSoundTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            //if (walkingSoundTimer > 500)
            //{
            //    if (player.playerTileIndex + map.Width + map.Width > 0 && player.playerTileIndex + map.Width + map.Width < tileArray.Length)
            //    {
            //        if (tileArray[player.playerTileIndex + map.Width + map.Width].ID == 1 && (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.A)))
            //        {
            //            grassSound.Play();
            //            walkingSoundTimer = 0;
            //        }
            //        if (tileArray[player.playerTileIndex + map.Width + map.Width].ID == 2 && (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.A)))
            //        {
            //            stoneSound.Play();
            //            walkingSoundTimer = 0;
            //        }
            //    }
            //}
        }

        public void Update(GameTime gameTime, Level CurrentLevel, Camera camera)
        {
            if (player.hasChronoshifted)
                return;

            this.Content = Game1.Content;
            this.gameTime = gameTime;
            this.player = player;
            popUp.Update(gameTime, player);
            background.Update(camera);
            UpdateInBackground();
            if (apple != null)
                apple.Update(player, gameTime, this);
            PlayWalkingSounds();

            if (player.isPlayerDead == false)
            {
                //defines which tiles are in range
                int initial = camera.tileIndex - 17 * mapTexture.Width - 25;
                int maxHoriz = 50;
                int maxVert = 30;
                int i = 0;

                for (int v = 0; v < maxVert; v++)
                {
                    for (int h = 0; h < maxHoriz; h++)
                    {
                        visibleTileArray[i] = initial + mapTexture.Width * v + h;
                        i++;
                    }
                }
                initial = camera.tileIndex - 17 * 2 * mapTexture.Width - 25 * 2;
                maxHoriz = 100;
                maxVert = 60;
                i = 0;
                for (int v = 0; v < maxVert; v++)
                {
                    for (int h = 0; h < maxHoriz; h++)
                    {
                        visibleLightArray[i] = initial + mapTexture.Width * v + h;
                        i++;
                    }
                }
            }

            foreach (Cloud c in cloudList)
            {
                c.CheckOutOfRange();
                c.Update(gameTime);
            }

            foreach (Chest chest in chestList)
            {
                chest.Animate(gameTime);
                if (chest.CheckOpened(gameTime, player) == true)
                {
                    int max;
                    if (chest.IsGolden)
                    {
                        entities.Add(new JetpackPowerUp(chest.rectangle.X, chest.rectangle.Y));
                        entities.Add(new CDPlayer(new Vector2(chest.rectangle.X, chest.rectangle.Y)));
                        max = randGen.Next(6, 10);
                    }
                    else max = randGen.Next(3, 6);
                    for (int i = 0; i <= max; i++)
                    {
                        gemList.Add(new Gem(chest, randGen.Next(0, 100), Content));
                    }
                    max = randGen.Next(10, 20);
                    for (int i = 0; i <= max; i++)
                    {
                        effectList.Add(new Particle(chest, randGen.Next(0, 100)));
                    }
                }


            }

            foreach (Gem gem in gemList)
            {
                gem.Update(gameTime);
                GemCollision(gem);
                if (gem.WasPickedUp(player))
                {
                    gem.AddScore(player);
                    gem.SoundAdd();
                    gemList.Remove(gem);
                    break;
                }
            }

            foreach (Enemy enemy in enemyList)
            {
                enemy.Update(player, gameTime, entities, this);
                EnemyCollision(enemy, player);
            }

            foreach (Obstacle ob in entities.OfType<Obstacle>())
            {
                ob.Update(gameTime, player, this);
            }

            foreach (PowerUp pow in entities.OfType<PowerUp>())
            {
                pow.Update(gameTime, player, this);
                if (pow.toDelete)
                {
                    entities.Remove(pow);
                    break;
                }
            }

            foreach (var vine in climbablesList)
            {
                if (vine.IsOnPlayer(player))
                {
                    player.isOnVines = true;
                    break;
                }
                else player.isOnVines = false;
            }

            foreach (var tech in techList)
            {
                tech.Update(gameTime, player, popUp);
            }

            foreach (var tech in techList)
            {
                if (tech.ToDelete)
                {
                    techList.Remove(tech);
                    break;
                }
            }

            playerLight.Update(player);

            foreach (int tileNumber in visibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < lightArray.Length)
                {
                    lightArray[tileNumber].Shake();
                }
            }

            foreach (Key key in keyList)
            {
                key.Update(player);
                if (key.toDelete)
                {
                    keyList.Remove(key);
                    break;
                }
            }

            foreach (Door door in doorList)
            {
                door.Update(gameTime, player, tileArray);
            }

            foreach (var noob in noobList)
            {
                noob.Update(gameTime, player);
                NoobCollision(noob);
            }

            foreach (int tileNumber in visibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < tileArray.Length)
                {
                    tileArray[tileNumber].Update(gameTime);
                }
            }
        }

        public void UpdateInBackground()
        {
            foreach (var tile in animatedTileList)
                tile.Update(gameTime);

            foreach (Particle effect in effectList)
                effect.Update(gameTime);

            for (int i = effectList.Count; i == 0; i--)
            {
                if (effectList.Count == 0)
                    break;
                if (effectList[i].ToDelete())
                {
                    effectList.Remove(effectList[i]);
                }
            }
        }

        public void DrawLights(SpriteBatch spriteBatch)
        {

            playerLight.Draw(spriteBatch);

            foreach (int tileNumber in visibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < lightArray.Length && lightArray[tileNumber].intensity <= 2)
                {
                    lightArray[tileNumber].Draw(spriteBatch);
                }
            }
            foreach (int tileNumber in visibleLightArray)
            {
                if (tileNumber >= 0 && tileNumber < lightArray.Length && lightArray[tileNumber].intensity > 2)
                {
                    lightArray[tileNumber].Draw(spriteBatch);
                }
            }

            if (player.weapon != null)
                player.weapon.DrawLights(spriteBatch);
            //foreach (Projectile pr in projectileList)
            //    pr.DrawLights(spriteBatch);
            foreach (Particle ef in effectList)
                ef.DrawLights(spriteBatch);
            foreach (Gem ge in gemList)
                ge.DrawLights(spriteBatch);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (apple != null)
                apple.Draw(spriteBatch);

            foreach (var tech in techList)
            {
                tech.Draw(spriteBatch);
            }
            foreach (Chest chest in chestList)
            {
                chest.Draw(spriteBatch);
            }
            foreach (Gem gem in gemList)
            {
                gem.Draw(spriteBatch);
            }
            foreach (var tile in animatedTileList)
            {
                tile.Draw(spriteBatch);
            }
            foreach (int tileNumber in visibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < tileArray.Length)
                {
                    if (tileArray[tileNumber].texture != null)
                        tileArray[tileNumber].Draw(spriteBatch);
                }
            }
            foreach (Enemy enemy in enemyList)
            {
                enemy.Draw(spriteBatch);
            }
            foreach (Key key in keyList)
            {
                key.Draw(spriteBatch);
            }

            foreach (Door door in doorList)
            {
                door.Draw(spriteBatch);
            }
            foreach (Particle effect in effectList)
            {
                effect.Draw(spriteBatch);
            }
            foreach (Projectile proj in projectileList)
            {
                proj.Draw(spriteBatch);
            }
            foreach (NonPlayableCharacter noob in noobList)
            {
                noob.Draw(spriteBatch);
            }

            foreach (Entity en in entities)
                en.Draw(spriteBatch);




        }

        public void DrawClouds(SpriteBatch spriteBatch)
        {
            foreach (Cloud c in cloudList)
            {
                if (wantClouds == true)
                    c.Draw(spriteBatch);
            }
        }

        public void DrawInBack(SpriteBatch spriteBatch)
        {

            foreach (int tileNumber in visibleTileArray)
            {
                if (tileNumber > 0 && tileNumber < tileArray.Length)
                {
                    if (wallArray[tileNumber].texture != null)
                        wallArray[tileNumber].Draw(spriteBatch);
                }
            }
        }

        public void DrawBackground(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
        }

        public void DrawAfterEffects(SpriteBatch spriteBatch)
        {
            //black corners of the screen
            spriteBatch.Draw(blackCorners.Texture, blackCorners.Rectangle, Color.White);
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {


            popUp.Draw(spriteBatch);

            foreach (var noob in noobList)
            {
                noob.DrawUI(spriteBatch);
            }
            foreach (var door in doorList)
            {
                door.DrawUI(spriteBatch);
            }

        }

        public void WarnRunningOutOfTime()
        {
            if (!isRunningOutOfTime)
            {
                isRunningOutOfTime = true;
                MediaPlayer.Stop();
                hurryUpInstance.Play();
            }
            else
            {
                if (hurryUpInstance.State == SoundState.Stopped && !isPlayingFastTheme)
                {
                    if (fastLevelTheme != null)
                        MediaPlayer.Play(fastLevelTheme);
                    isPlayingFastTheme = true;
                }
            }
        }

        public void WarnOutOfTime()
        {

        }

        public void RespawnEnemies()
        {
            foreach (var en in enemyList)
            {
                en.health = 1;
                en.isDead = false;
            }
        }

        public MapDataPacket GetMapDataPacket()
        {
            MapDataPacket m = new MapDataPacket(this);
            return m;
        }

        public void UpdateFromDataPacket(MapDataPacket m)
        {
            apple = m.apple;
            timer = m.timer;
            isPaused = m.isPaused;
            levelComplete = m.levelComplete;


            cloudList = m.cloudList;
            gemList = m.gemList;
            chestList = m.chestList;
            enemyList = m.enemyList;
            effectList = m.effectList;
            projectileList = m.projectileList;
            climbablesList = m.climbablesList;
            techList = m.techList;
            doorList = m.doorList;
            keyList = m.keyList;
            noobList = m.noobList;
            entities = m.entities;

            gameTime = m.gameTime;
        }
    }
}
