using Microsoft.Xna.Framework;
using System;
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
        private bool isAttacking = false;
        private bool foundEnemySpawners = false;
        private bool isLevitatingEntities = false;

        private static Vector2 CameraPositionArena = new Vector2(3474, 3388);

        List<Tile> enemySpawners = new List<Tile>();

        private enum Difficulty { HaveFun, YouAreQuiteTheTrick, StopThat }
        private Difficulty CurrentDifficulty = Difficulty.HaveFun;

        private enum AttackMove { SpawnEnemies, Meteors, ToTheSky, StopTime }
        private AttackMove CurrentAttackMove = AttackMove.SpawnEnemies;

        Timer castingSpellTimer = new Timer(true);
        Timer meteorShowerTimer = new Timer(true);
        Timer meteorSpacingTimer = new Timer(true);
        Timer levitateEntities = new Timer(true);
        Timer attackTimer = new Timer(true);

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
            }

            if (isAttacking)
            {
                attackTimer.Reset();
            }

            if (isLevitatingEntities)
            {
                LevitateEntities();
            }

        }

        private void LevitateEntities()
        {
            const float velocity = -20f;
            GameWorld.GetPlayer().SetVelY(velocity);
            GameWorld.ParticleSystem.Add(ParticleType.Tiny, CalcHelper.GetRandXAndY(GameWorld.GetPlayer().GetCollRectangle()), null, Color.Yellow);
            foreach (var entity in GameWorld.Entities)
            {
                if (entity != Entity)
                {
                    entity.SetVelY(velocity);
                    GameWorld.ParticleSystem.Add(ParticleType.Tiny, CalcHelper.GetRandXAndY(entity.GetCollRectangle()), null, Color.Yellow);
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

            foreach (var spawner in enemySpawners)
            {
                var en = new Frog(spawner.DrawRectangle.X, spawner.DrawRectangle.Y);
                GameWorld.Entities.Add(en);
                for (int i = 0; i < 5; i++)
                {
                    GameWorld.ParticleSystem.Add(ParticleType.Round_Common, CalcHelper.GetRandXAndY(en.GetDrawRectangle()), null, Color.White);
                }
            }
            attackTimer.ResetAndWaitFor(10000);
            CurrentAttackMove = AttackMove.ToTheSky;


        }

        private void SpawnMeteors()
        {

        }

        private void ToTheSky()
        {
            CastSpell();
            isLevitatingEntities = true;
            TMBAW_Game.Camera.LockedY = false;
            levitateEntities.ResetAndWaitFor(5000);
            levitateEntities.SetTimeReached += LevitateEntities_SetTimeReached;
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
