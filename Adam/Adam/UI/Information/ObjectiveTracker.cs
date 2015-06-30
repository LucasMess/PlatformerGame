using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI.Information
{
    public class ObjectiveTracker
    {
        public List<Objective> objectives;
        double timer;
        SoundFx completeSound;

        public ObjectiveTracker()
        {
            completeSound = new SoundFx("Sounds/Menu/quest_complete");
            objectives = new List<Objective>();
        }

        public void AddObjective(Objective obj)
        {
            obj.SetPosition(objectives.Count);
            objectives.Add(obj);
        }

        public void Update(GameTime gameTime)
        {
            //timer += gameTime.ElapsedGameTime.TotalSeconds;
            //if (timer > 1)
            //{
            //    Objective obj = new Objective();
            //    obj.Create("Find the map.", ObjectiveType.GoSomewhere, GameWorld.randGen.Next(0, 5));
            //    AddObjective(obj);

            //    CompleteObjective(GameWorld.randGen.Next(0, 5));
            //    timer = 0;
            //}

            foreach (Objective ob in objectives)
                ob.Update(gameTime);

            foreach (Objective ob in objectives)
            {
                if (!ob.isActive)
                {
                    objectives.Remove(ob);
                    break;
                }
            }

            Reorder();
        }

        private void Reorder()
        {
            Objective[] objs = objectives.ToArray();
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i].TransitionIntoNewPosition(i);
            }
            objectives = objs.ToList<Objective>();
        }

        public void CompleteObjective(int ID)
        {
            foreach (Objective ob in objectives)
            {
                if (ob.ID == ID)
                {
                    completeSound.PlayIfStopped();
                    ob.isComplete = true;
                    break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Objective ob in objectives)
            {
                ob.Draw(spriteBatch);
            }
        }





        //public Objective[] Objectives
        //{
        //    get
        //    {
        //        objectives.ToArray();
        //    }
        //    set
        //    {
        //        objectives = new List<Objective>();
        //        Objective[] objs = value;
        //        for (int i = 0; i < objs.Length; i ++)
        //        {
        //            objectives.Add(objs[i]);
        //        }
        //    }
        //}
    }
}
