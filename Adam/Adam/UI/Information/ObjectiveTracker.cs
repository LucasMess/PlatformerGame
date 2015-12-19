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
        public List<Objective> Objectives;
        double _timer;
        SoundFx _completeSound;
        SoundFx _newObjective;

        public ObjectiveTracker()
        {
            _completeSound = new SoundFx("Sounds/Menu/quest_complete");
            _newObjective = new SoundFx("Sounds/Menu/quest_new");
            Objectives = new List<Objective>();
        }

        public void AddObjective(Objective obj)
        {
            bool found = false;
            for (int i = 0; i < Objectives.Count; i++)
            {
                if (Objectives[i].Id == obj.Id)
                    found = true;
            }

            if (!found)
            {
                //Add new objective and play the new objective sound.
                _newObjective.Play();
                obj.SetPosition(Objectives.Count);
                Objectives.Add(obj);
            }
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

            foreach (Objective ob in Objectives)
                ob.Update(gameTime);

            foreach (Objective ob in Objectives)
            {
                if (!ob.IsActive)
                {
                    Objectives.Remove(ob);
                    break;
                }
            }

            Reorder();
        }

        private void Reorder()
        {
            Objective[] objs = Objectives.ToArray();
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i].TransitionIntoNewPosition(i);
            }
            Objectives = objs.ToList<Objective>();
        }

        public void CompleteObjective(int id)
        {
            foreach (Objective ob in Objectives)
            {
                if (ob.Id == id)
                {
                    _completeSound.PlayIfStopped();
                    ob.IsComplete = true;
                    break;
                }
            }
        }

        public void Clear()
        {
            Objectives = new List<Objective>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Objective ob in Objectives)
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
