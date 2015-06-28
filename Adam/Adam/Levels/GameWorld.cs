﻿using System;
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
using Adam.UI;
using Adam.UI.Information;
using Adam.Levels;
using Adam.Characters.Non_Playable;
using Adam.Noobs;

namespace Adam
{
    sealed class GameWorld
    {
        private static GameWorld instance;

        public static GameWorld Instance
        {
            get
            {
                if (instance == null)
                    throw new Exception("The instance of Gameworld has not yet been created.");
                else return instance;
            }
        }

        //Basic tile grid and the visible tile grid
        public Tile[] tileArray;
        public Tile[] wallArray;
        int[] visibleTileArray = new int[30 * 50];
        int[] visibleLightArray = new int[60 * 100];
        Light[] lightArray;
        Light playerLight;

        public Level CurrentLevel;
        public Player player;
        public Apple apple;
        Background background = new Background();
        PopUp popUp = new PopUp();
        PlaceNotification placeNotification;
        int enemyTilePos;
        int gemTilePos;

        public int TimesUpdated;
        public int TimesBackgroundUpdated;

        public bool isPaused;
        public bool levelComplete;
        bool hasLoaded;
        double timeStepTimer;
        bool hasUpdated;
        public static Random RandGen;
        public static Texture2D SpriteSheet;
        Game1 game1;
        Camera camera;

        //The goal with all these lists is to have two: entities and particles. The particles will potentially be updated in its own thread to improve
        //performance.
        public List<Cloud> cloudList;
        public List<Gem> gemList; //can be moved to entities
        public List<Chest> chestList; //can be moved to entities
        public List<Climbables> climbablesList; //could be moved to entities
        public List<Key> keyList; //This one is tricky... it could be moved to the WorldData.
        public List<Entity> entities;
        public List<Particle> particles;
        ContentManager Content;
        public GameTime gameTime;
        public WorldData worldData;

        public GameWorld() { }

        public GameWorld(Game1 game1)
        {
            instance = this;

            this.game1 = game1;

            placeNotification = new PlaceNotification();
            RandGen = new Random();
            SpriteSheet = ContentHelper.LoadTexture("Tiles/Spritemaps/spritemap_11");

            Thread thread = new Thread(new ThreadStart(UpdateInBackground));
            thread.IsBackground = true;
            thread.Start();
        }

        public void Load(ContentManager Content, Vector2 monitorResolution, Player player, Level CurrentLevel)
        {
            hasLoaded = false;

            worldData = new WorldData(CurrentLevel);

            cloudList = new List<Cloud>();
            gemList = new List<Gem>();
            chestList = new List<Chest>();
            climbablesList = new List<Climbables>();
            keyList = new List<Key>();
            entities = new List<Entity>();
            particles = new List<Particle>();

            this.Content = Content;
            this.player = player;
            popUp.Load(Content);

            this.CurrentLevel = CurrentLevel;
            MediaPlayer.Volume = .2f;

            int maxClouds = 5;
            for (int i = 0; i < maxClouds; i++)
            {
                cloudList.Add(new Cloud(Content, monitorResolution, maxClouds, i));
            }

            tileArray = new Tile[worldData.mainMap.Width * worldData.mainMap.Height];
            wallArray = new Tile[worldData.mainMap.Width * worldData.mainMap.Height];

            LoadGrid(tileArray, worldData.mainMap);
            LoadGrid(wallArray, worldData.wallMap);

            LoadLights();

            playerLight = new Light();
            playerLight.Load(Content);

            background.Load(CurrentLevel, this);

            if (worldData.song != null)
                MediaPlayer.Play(worldData.song);

            placeNotification.Show(worldData.levelName);

            hasLoaded = true;
        }

        private void LoadGrid(Tile[] array, Texture2D data)
        {
            int currentTileNumber = 0;

            //Create basic grid where all block are transparent and not differentiated
            for (int r = 1; r <= worldData.mainMap.Height; r++)
            {
                for (int c = 1; c <= worldData.mainMap.Width; c++)
                {
                    array[currentTileNumber] = new Tile();
                    array[currentTileNumber].TileIndex = currentTileNumber;
                    currentTileNumber++;
                }
            }

            //Check the pixels and differentiate tiles based off their color
            int totalPixelCount = worldData.mainMap.Width * worldData.mainMap.Height;
            Color[] tilePixels = new Color[totalPixelCount];
            data.GetData<Color>(tilePixels);

            for (int i = 0; i < totalPixelCount; i++)
            {
                Tile tile = array[i];
                Color pixel = tilePixels[i];
                Vector3 colorCode = new Vector3(pixel.R, pixel.G, pixel.B);
                int Xcoor = (i % worldData.mainMap.Width) * Game1.Tilesize;
                int Ycoor = ((i - (i % worldData.mainMap.Width)) / worldData.mainMap.Width) * Game1.Tilesize;

                tile.rectangle = new Rectangle(Xcoor, Ycoor, Game1.Tilesize, Game1.Tilesize);

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
                    tileArray[i] = new AnimatedTile(8, tile.rectangle);
                    tileArray[i].isSolid = true;
                }
                //9 tallgrass
                else if (colorCode == new Vector3(225, 127, 0)) //goldBricks
                {
                    tile.ID = 10;
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(249, 64, 45))//torch
                {
                    tileArray[i] = new AnimatedTile(11, tile.rectangle);
                }
                else if (colorCode == new Vector3(191, 129, 9)) //chandelier
                {
                    tileArray[i] = new AnimatedTile(12, tile.rectangle);
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
                else if (colorCode == new Vector3(239, 239, 239)) //marbleworldData.mainMap.Width
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
                    //REMOVED
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
                    tileArray[i] = new AnimatedTile(24, tile.rectangle);
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
                else if (colorCode == new Vector3(255, 255, 250)) // Marble ceiling
                {
                    tile.ID = 29;
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(255, 255, 241)) // Marble ceiling support
                {
                    tile.ID = 30;
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(0, 138, 50)) //Tree
                {
                    tileArray[i] = new AnimatedTile(31, tile.rectangle);
                }
                else if (colorCode == new Vector3(127,169,186)) //Small Rock
                {
                    tile.ID = 32;
                }
                else if (colorCode == new Vector3(127, 169, 187)) //Big Rock
                {
                    tileArray[i] = new AnimatedTile(33, tile.rectangle);
                }
                else if (colorCode == new Vector3(127, 169, 188)) //Medium Rock
                {
                    tileArray[i] = new AnimatedTile(34, tile.rectangle);
                }
                //35 Small pebbles - automatic
                else if (colorCode == new Vector3(213, 172, 31)) //Sign 1
                {
                    tile.ID = 36;
                    entities.Add(new Sign(Xcoor, Ycoor, 1));
                }
                else if (colorCode == new Vector3(213, 172, 32)) //Sign 2
                {
                    tile.ID = 36;
                    entities.Add(new Sign(Xcoor, Ycoor, 2));
                }
                else if (colorCode == new Vector3(213, 172, 33)) //Sign 3
                {
                    tile.ID = 36;
                    entities.Add(new Sign(Xcoor, Ycoor, 3));
                }
                else if (colorCode == new Vector3(213, 172, 34)) //Sign 4
                {
                    tile.ID = 36;
                    entities.Add(new Sign(Xcoor, Ycoor, 4));
                }


      //CHARACTERS AND OTHERS
                else if (colorCode == new Vector3(0, 255, 0)) //player
                {
                    player.Initialize(Xcoor, Ycoor);
                }
                else if (colorCode == new Vector3(81, 103, 34)) //snake
                {
                    entities.Add(new SnakeEnemy(Xcoor, Ycoor, Content, this));
                }
                else if (colorCode == new Vector3(143, 148, 0)) //potato
                {
                    entities.Add(new PotatoEnemy(Xcoor, Ycoor, Content));
                }
                else if (colorCode == new Vector3(177, 0, 203)) //God
                {
                    entities.Add(new God(Xcoor, Ycoor));
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
                else if (colorCode == new Vector3(225, 225, 225)) //marblewall
                {
                    tile.ID = 104;
                }
                else if (colorCode == new Vector3(154, 105, 11)) //sandwall
                {
                    tile.ID = 105;
                }

                else if (colorCode == new Vector3(191, 81, 0)) //door secret 1
                {
                    entities.Add(new Door(Xcoor, Ycoor, Content, 1, i));
                    tile.ID = 13;
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(191, 81, 1)) //door secret 2
                {
                    tile.ID = 13;
                    entities.Add(new Door(Xcoor, Ycoor, Content, 2, i));
                    tile.isSolid = true;
                }
                else if (colorCode == new Vector3(191, 81, 2)) //door secret 3
                {
                    tile.ID = 13;
                    entities.Add(new Door(Xcoor, Ycoor, Content, 3, i));
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

            //Connected Textures
            foreach (Tile tile in array)
            {
                if (tile.ID == 1 || tile.ID == 2 || tile.ID == 4 || tile.ID == 5 || tile.ID == 10)
                {
                    tile.FindConnectedTextures(array, worldData.mainMap.Width);
                }
            }

            //Now that all IDs have been given, define all textures for the tiles.
            foreach (Tile tile in array)
            {
                tile.DefineTexture();
                tile.AddRandomlyGeneratedDecoration(array, worldData.mainMap.Width);
                tile.DefineTexture();
            }

            //Check lava to see if it is on top for lava effects.
            foreach (Lava lava in entities.OfType<Lava>())
            {
                lava.CheckOnTop(array, this);
            }

        }

        public void LoadLights()
        {
            lightArray = new Light[worldData.mainMap.Width * worldData.mainMap.Height];

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
                li.CalculateLighting(tileArray, wallArray, worldData.mainMap);
            }
        }

        public void GemCollision(Gem gem)
        {
            if (gem.velocity.X == 0 && gem.velocity.Y == 0) { }
            else
            {
                gemTilePos = (int)(gem.topMidBound.Y / Game1.Tilesize * worldData.mainMap.Width) + (int)(gem.topMidBound.X / Game1.Tilesize);

                int[] q = new int[9];
                q[0] = gemTilePos - worldData.mainMap.Width - 1;
                q[1] = gemTilePos - worldData.mainMap.Width;
                q[2] = gemTilePos - worldData.mainMap.Width + 1;
                q[3] = gemTilePos - 1;
                q[4] = gemTilePos;
                q[5] = gemTilePos + 1;
                q[6] = gemTilePos + worldData.mainMap.Width - 1;
                q[7] = gemTilePos + worldData.mainMap.Width;
                q[8] = gemTilePos + worldData.mainMap.Width + 1;

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
                enemyTilePos = (int)(enemy.topMidBound.Y / Game1.Tilesize * worldData.mainMap.Width) + (int)(enemy.topMidBound.X / Game1.Tilesize);

                int[] q = new int[12];
                q[0] = enemyTilePos - worldData.mainMap.Width - 1;
                q[1] = enemyTilePos - worldData.mainMap.Width;
                q[2] = enemyTilePos - worldData.mainMap.Width + 1;
                q[3] = enemyTilePos - 1;
                q[4] = enemyTilePos;
                q[5] = enemyTilePos + 1;
                q[6] = enemyTilePos + worldData.mainMap.Width - 1;
                q[7] = enemyTilePos + worldData.mainMap.Width;
                q[8] = enemyTilePos + worldData.mainMap.Width + 1;
                q[9] = enemyTilePos + worldData.mainMap.Width + worldData.mainMap.Width - 1;
                q[10] = enemyTilePos + worldData.mainMap.Width + worldData.mainMap.Width;
                q[11] = enemyTilePos + worldData.mainMap.Width + worldData.mainMap.Width + 1;

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

        public void Update(GameTime gameTime, Level CurrentLevel, Camera camera)
        {
            TimesUpdated++;
            if (player.hasChronoshifted)
                return;

            this.Content = Game1.Content;
            this.gameTime = gameTime;
            this.camera = camera;

            popUp.Update(gameTime, player);
            background.Update(camera);
            placeNotification.Update(gameTime);
            worldData.Update(gameTime);

            if (apple != null)
                apple.Update(player, gameTime, this, game1);



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
                        max = RandGen.Next(6, 10);
                    }
                    else max = RandGen.Next(3, 6);
                    for (int i = 0; i <= max; i++)
                    {
                        gemList.Add(new Gem(chest, RandGen.Next(0, 100), Content));
                    }
                    max = RandGen.Next(10, 20);
                    for (int i = 0; i <= max; i++)
                    {
                        particles.Add(new Particle(chest, RandGen.Next(0, 100)));
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

            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                if (entity is Enemy)
                {
                    Enemy enemy = (Enemy)entity;
                    enemy.Update(player, gameTime);
                    EnemyCollision(enemy, player);
                }
                if (entity is Obstacle)
                {
                    Obstacle obstacle = (Obstacle)entity;
                    obstacle.Update(gameTime, player, this);
                }
                if (entity is PowerUp)
                {
                    PowerUp power = (PowerUp)entity;
                    power.Update(gameTime, player, this);
                }
                if (entity is Projectile)
                {
                    Projectile proj = (Projectile)entity;
                    proj.Update(player, gameTime);
                }
                if (entity is NonPlayableCharacter)
                {
                    NonPlayableCharacter npc = (NonPlayableCharacter)entity;
                    npc.Update(gameTime, player);
                }
                if (entity is Door)
                {
                    Door door = (Door)entity;
                    door.Update(gameTime, player, tileArray);
                }
                if (entity is Sign)
                {
                    Sign sign = (Sign)entity;
                    sign.Update();
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
            while (true)
            {
                if (hasLoaded && gameTime != null)
                {
                    if (TimesBackgroundUpdated < TimesUpdated)
                    {
                        TimesBackgroundUpdated++;

                        for (int i = 0; i < particles.Count; i++)
                        {
                            particles[i].Update(gameTime);
                        }


                        for (int i = entities.Count - 1; i >= 0; i--)
                        {
                            Entity entity = entities[i];
                            if (entity.toDelete)
                                entities.Remove(entity);
                        }

                        for (int i = particles.Count - 1; i >= 0; i--)
                        {
                            Particle p = particles[i];
                            if (p.ToDelete())
                                particles.Remove(p);
                        }

                        if (camera != null)
                            UpdateVisibleIndexes();
                    }
                }
            }
        }

        private void UpdateVisibleIndexes()
        {
            if (player.isPlayerDead == false)
            {
                //defines which tiles are in range
                int initial = camera.tileIndex - 17 * worldData.mainMap.Width - 25;
                int maxHoriz = 50;
                int maxVert = 30;
                int i = 0;

                for (int v = 0; v < maxVert; v++)
                {
                    for (int h = 0; h < maxHoriz; h++)
                    {
                        visibleTileArray[i] = initial + worldData.mainMap.Width * v + h;
                        i++;
                    }
                }
                initial = camera.tileIndex - 17 * 2 * worldData.mainMap.Width - 25 * 2;
                maxHoriz = 100;
                maxVert = 60;
                i = 0;
                for (int v = 0; v < maxVert; v++)
                {
                    for (int h = 0; h < maxHoriz; h++)
                    {
                        visibleLightArray[i] = initial + worldData.mainMap.Width * v + h;
                        i++;
                    }
                }
            }
        }

        public void DrawLights(SpriteBatch spriteBatch)
        {

            //if (wallArray[player.TileIndex].ID != 0)
            //    playerLight.Draw(spriteBatch);

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
            foreach (Particle ef in particles)
                ef.DrawLights(spriteBatch);
            foreach (Gem ge in gemList)
                ge.DrawLights(spriteBatch);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (apple != null)
                apple.Draw(spriteBatch);
            foreach (Chest chest in chestList)
            {
                chest.Draw(spriteBatch);
            }
            foreach (Gem gem in gemList)
            {
                gem.Draw(spriteBatch);
            }
            foreach (int tileNumber in visibleTileArray)
            {
                if (tileNumber >= 0 && tileNumber < tileArray.Length)
                {
                    if (tileArray[tileNumber].texture != null)
                        tileArray[tileNumber].Draw(spriteBatch);
                }
            }
            foreach (Key key in keyList)
            {
                key.Draw(spriteBatch);
            }
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(spriteBatch);
            }
            for (int i = 0; i < entities.Count; i++)
                entities[i].Draw(spriteBatch);
        }

        public void DrawClouds(SpriteBatch spriteBatch)
        {
            foreach (Cloud c in cloudList)
            {
                if (worldData.wantClouds == true)
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

        public void DrawUI(SpriteBatch spriteBatch)
        {
            placeNotification.Draw(spriteBatch);
        }

        public void ResetWorld()
        {
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                Entity entity = entities[i];
                if (entity is Enemy)
                {
                    Enemy enemy = (Enemy)entity;
                    enemy.health = enemy.maxHealth;
                    enemy.isDead = false;
                }
                if (entity is Food)
                {
                    entities.Remove(entity);
                }
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
            isPaused = m.isPaused;
            levelComplete = m.levelComplete;

            gameTime = m.gameTime;
        }
    }
}
