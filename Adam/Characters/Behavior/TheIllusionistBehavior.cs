using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThereMustBeAnotherWay.Characters.Enemies;
using ThereMustBeAnotherWay.Graphics;
using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.Misc;
using ThereMustBeAnotherWay.Particles;
using static ThereMustBeAnotherWay.TMBAW_Game;

namespace ThereMustBeAnotherWay.Characters.Behavior
{
    class TheIllusionistBehavior : Behavior
    {
        private bool isInBossFight = false;
        private bool fightStarted = false;
        private bool castingSpell = false;
        private bool isCastingSpellParticles = false;
        private bool isAttacking = false;
        private bool foundEnemySpawners = false;
        private bool isSpawningEnemies = false;
        private bool isLevitatingEntities = false;

        private static Vector2 CameraPositionArena = new Vector2(3474, 3388);

        List<Tile> enemySpawners = new List<Tile>();
        List<Enemy> enemiesBeingSpawned = new List<Enemy>();

        private enum Difficulty { HaveFun, YouAreQuiteTheTrick, StopThat }
        private Difficulty CurrentDifficulty = Difficulty.HaveFun;

        private enum AttackMove { SpawnEnemies, Meteors, ToTheSky, StopTime }
        private AttackMove CurrentAttackMove = AttackMove.SpawnEnemies;

        Timer castingSpellTimer = new Timer(true);
        Timer meteorShowerTimer = new Timer(true);
        Timer meteorSpacingTimer = new Timer(true);
        Timer levitateEntitiesTimer = new Timer(true);
        Timer attackTimer = new Timer(true);

        SoundFx laughSound = new SoundFx("Sounds/Illusionist/evil_laugh");
        SoundFx spawnSound = new SoundFx("Sounds/Illusionist/spawn_enemies");

        public TheIllusionistBehavior()
        {
            if (GameWorld.WorldData.LevelName == "TheIllusionist")
            {
                Console.WriteLine("Boss Fight started with the Illusionist.");
                isInBossFight = true;
            }
        }

        public override void Update(Entity entity)
        {
            if (!foundEnemySpawners)
            {
                enemySpawners = GameWorld.GetTilesWithId(TileType.EnemySpawner);
                foundEnemySpawners = true;
            }

            if (TMBAW_Game.CurrentGameMode == GameMode.Play)
            {
                if (isInBossFight) BossFight();
            }
            base.Update(entity);
        }

        private void BossFight()
        {

            if (!fightStarted)
            {
                //TMBAW_Game.Dialog.Say("Now let's have some fun!", null, null);
                fightStarted = true;
                attackTimer.ResetAndWaitFor(2000);
                attackTimer.SetTimeReached += AttackTimer_SetTimeReached;

                TMBAW_Game.Camera.SetPosition(CameraPositionArena);
                TMBAW_Game.Camera.LockedX = true;
                TMBAW_Game.Camera.LockedY = true;
                TMBAW_Game.Camera.RestricedToGameWorld = false;
                GameWorld.GetPlayer().RestrictedToGameWorld = false;
                GraphicsRenderer.StaticLightsEnabled = false;

                castingSpellTimer.SetTimeReached += OnEnemiesSpawned;
            }

            if (isAttacking)
            {
                attackTimer.Reset();
            }

            if (isLevitatingEntities)
            {
                LevitateEntities();
            }

            if (isSpawningEnemies)
                SpawnEnemyParticles();

            if (castingSpell)
                SpawnSpellParticles();

        }

        private void SpawnSpellParticles()
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 offset;
                if (Entity.IsFacingRight)
                {
                    offset = new Vector2(40, 3);
                }
                else
                {
                    offset = new Vector2(20, 3);
                }
                GameWorld.ParticleSystem.Add(ParticleType.Tiny, new Vector2(Entity.GetDrawRectangle().X, Entity.GetDrawRectangle().Y) + offset , CalcHelper.GetRandXAndY(new Rectangle(-10,-10,20,20))/10, Color.Yellow);
            }
        }

        private void SpawnEnemyParticles()
        {
            foreach (var en in enemiesBeingSpawned)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameWorld.ParticleSystem.Add(ParticleType.Round_Common, CalcHelper.GetRandXAndY(en.GetDrawRectangle()), CalcHelper.GetRandXAndY(new Rectangle(-1, -1, 2, 2)), Color.Yellow);
                }

                en.SetVelX(0);
                en.SetVelY(0);
            }
        }

        private void LevitateEntities()
        {
            const float maxVel = 20f;
            float velocity = maxVel/2 * -(float)Math.Cos(2 * Math.PI * levitateEntitiesTimer.TimeElapsedInMilliSeconds / 10000) + maxVel / 2 + 1;
            GameWorld.GetPlayer().SetVelY(-velocity);
            GameWorld.ParticleSystem.Add(ParticleType.Tiny, CalcHelper.GetRandXAndY(GameWorld.GetPlayer().GetCollRectangle()), Vector2.Zero, new Color(196, 148, 255));
            foreach (var entity in GameWorld.Entities)
            {
                if (entity != Entity)
                {
                    entity.SetVelY(-velocity);
                    GameWorld.ParticleSystem.Add(ParticleType.Tiny, CalcHelper.GetRandXAndY(entity.GetCollRectangle()), Vector2.Zero, new Color(196, 148, 255));
                }
            }
        }
        private void AttackTimer_SetTimeReached()
        {
            Attack();
        }

        private void Attack()
        {

            switch (CurrentAttackMove)
            {
                case AttackMove.SpawnEnemies:
                    SpawnEnemies();
                    break;
                case AttackMove.Meteors:
                    SpawnMeteors();
                    break;
                case AttackMove.ToTheSky:
                    ToTheSky();
                    break;
            }
        }

        /// <summary>
        /// Does the animation for spell casting.
        /// </summary>
        private void CastSpell()
        {
            if (!castingSpell)
            {
                castingSpell = true;
                Entity.AddAnimationToQueue("castSpell");
                castingSpellTimer.ResetAndWaitFor(3000);
                castingSpellTimer.SetTimeReached += CastingSpellTimer_SetTimeReached;
                spawnSound.Play();
                laughSound.Play();
            }
        }
        private void CastingSpellTimer_SetTimeReached()
        {
            castingSpell = false;
            Entity.RemoveAnimationFromQueue("castSpell");
            castingSpellTimer.SetTimeReached -= CastingSpellTimer_SetTimeReached;
        }

        private void SpawnEnemies()
        {
            CastSpell();
            enemiesBeingSpawned.Clear();
            foreach (var spawner in enemySpawners)
            {
                var en = new Frog(spawner.DrawRectangle.X, spawner.DrawRectangle.Y);
                GameWorld.Entities.Add(en);
                enemiesBeingSpawned.Add(en);
            }
            attackTimer.ResetAndWaitFor(10000);
            CurrentAttackMove = AttackMove.ToTheSky;
            isSpawningEnemies = true;

        }

        private void OnEnemiesSpawned()
        {
            isSpawningEnemies = false;
        }

        private void SpawnMeteors()
        {

        }

        private void ToTheSky()
        {
            CastSpell();
            isLevitatingEntities = true;
            TMBAW_Game.Camera.LockedY = false;
            levitateEntitiesTimer.ResetAndWaitFor(5000);
            levitateEntitiesTimer.SetTimeReached += LevitateEntities_SetTimeReached;
            attackTimer.ResetAndWaitFor(Int32.MaxValue);
            GameWorld.GetPlayer().CollidedWithTerrain += OnPlayerFallFromLevitation;
        }

        private void LevitateEntities_SetTimeReached()
        {
            isLevitatingEntities = false;
        }

        private void OnPlayerFallFromLevitation(Entity entity, Tile tile)
        {
            attackTimer.ResetAndWaitFor(5000);
            GameWorld.GetPlayer().CollidedWithTerrain -= OnPlayerFallFromLevitation;

        }
    }
}
