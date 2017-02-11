using Adam.Levels;
using Adam.PlayerCharacter;
using Adam.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Adam.Interactables
{
    public abstract class Interactable
    {

        public bool CanBeLinkedToOtherInteractables { get; protected set; } = false;
        Line line;
        private bool isBeingInteractedWith = false;
        private static bool buttonWasReleased = false;
        private static bool selectingAnotherTile = false;

        public delegate void InteractionHandler(Tile tile);
        public event InteractionHandler OnActivation;

        public void ReadMetaData(Tile tile)
        {
            string val;
            if (GameWorld.WorldData.MetaData.TryGetValue(tile.TileIndex, out val))
            {
                string[] commands = val.Split(':');
                switch (commands[0])
                {
                    case "activate":
                        int indexOther = int.Parse(commands[1]);
                        Tile other = GameWorld.GetTile(indexOther);
                        other.ConnectToInteractable(this);
                        break;
                }
            }
        }


        /// <summary>
        /// Called when the tile is updated.
        /// </summary>
        /// <param name="tile"></param>
        public virtual void Update(Tile tile)
        {

            if (Mouse.GetState().LeftButton == ButtonState.Released)
                buttonWasReleased = true;

            if (isBeingInteractedWith && buttonWasReleased)
            {
                Rectangle mouse = InputHelper.GetMouseRectGameWorld();
                line = new Line(new Vector2(tile.DrawRectangle.Center.X, tile.DrawRectangle.Center.Y),
                    new Vector2(mouse.Center.X, mouse.Center.Y));

                if (InputHelper.IsLeftMousePressed())
                {
                    selectingAnotherTile = false;
                    isBeingInteractedWith = false;
                    buttonWasReleased = false;

                    int index = CalcHelper.GetIndexInGameWorld(mouse.Center.X, mouse.Center.Y);
                    if (GameWorld.GetTile(index).IsInteractable())
                    {
                        //TODO: Make a connection between these tiles.
                        // Maybe make the interactable in the other tile be subscribed to this interactable's Action event....
                        GameWorld.GetTile(index).ConnectToInteractable(this);
                        OnConnectionToInteractable(tile, GameWorld.GetTile(index));
                    }
                    else
                    {
                        line = null;
                    }                }
            }
        }

        protected virtual void OnConnectionToInteractable(Tile source, Tile other)
        {
            LevelEditor.SaveLevel();
        }

        /// <summary>
        /// Defines what should happen when the player touches the object. Should not call on connected interactables.
        /// </summary>
        /// <param name="tile"></param>
        public virtual void OnEntityTouch(Tile tile, Entity entity)
        {

        }

        /// <summary>
        /// Defines what should happen when the player interacts with the object. It is also what happens when the object is remotely activated, i.e. via a player detector.
        /// </summary>
        public virtual void OnPlayerAction(Tile tile)
        {
            OnActivation?.Invoke(tile);
        }

        /// <summary>
        /// Invoked when the player clicks on the tile in edit mode.
        /// </summary>
        public virtual void OnPlayerClickInEditMode(Tile tile)
        {
            if (CanBeLinkedToOtherInteractables && !selectingAnotherTile && buttonWasReleased)
            {
                isBeingInteractedWith = true;
                selectingAnotherTile = true;
                buttonWasReleased = false;
            }
        }

        /// <summary>
        /// Called when the tile is destroyed.
        /// </summary>
        /// <param name="tile"></param>
        public virtual void OnTileDestroyed(Tile tile)
        {
            GameWorld.WorldData.MetaData.Remove(tile.TileIndex);
        }

        /// <summary>
        /// Used to draw special things that are not part of the tile itself.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tile"></param>
        public virtual void Draw(SpriteBatch spriteBatch, Tile tile)
        {
            line?.Draw(spriteBatch);
        }
    }
}
