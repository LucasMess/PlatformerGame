using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.PlayerCharacter;
using ThereMustBeAnotherWay.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ThereMustBeAnotherWay.Interactables
{
    public abstract class Interactable
    {

        /// <summary>
        /// Returns true if this interactable can be linked to other interactables to activate them.
        /// </summary>
        public bool CanBeLinkedToOtherInteractables { get; protected set; } = false;

        /// <summary>
        /// Returns true if this interactable can be linked by other interactables to be activated by them.
        /// </summary>
        public bool CanBeLinkedByOtherInteractables { get; protected set; } = false;
        Line line = new Line();
        private bool isBeingInteractedWith = false;
        private static bool ButtonWasReleased = false;
        private static bool SelectingAnotherTile = false;
        protected Light Light;

        public delegate void InteractionHandler(Tile tile, Player player);
        public event InteractionHandler OnActivation;

        public Interactable()
        {
        }

        public virtual void Initialize()
        {
            LevelEditor.InteractableConnections.Add(line);
            LightingEngine.AddDynamicLight(Light);
        }

        /// <summary>
        /// Reads the metadata tag and conencts the interactable to others depending on the commands.
        /// </summary>
        /// <param name="tile"></param>
        public void ReadMetaData(Tile tile)
        {
            string[] commands = GetCommands(tile);
            if (commands == null) return;
            switch (commands[0])
            {
                case "activate":
                    int indexOther = int.Parse(commands[1]);
                    Tile other = GameWorld.GetTile(indexOther);
                    other.ConnectToInteractable(this);
                    break;
            }

        }

        /// <summary>
        /// Returns the commands listed under the metadata tag for this tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        protected string[] GetCommands(Tile tile)
        {
            string val;
            if (GameWorld.WorldData.MetaData.TryGetValue(tile.TileIndex, out val))
            {
                return val.Split(':');
            }
            else return null;
        }

        /// <summary>
        /// Called when the tile is updated.
        /// </summary>
        /// <param name="tile"></param>
        public virtual void Update(Tile tile)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Released)
                ButtonWasReleased = true;

            if (isBeingInteractedWith && ButtonWasReleased)
            {
                Rectangle mouse = InputHelper.GetMouseRectGameWorld();
                line.Update(new Vector2(tile.DrawRectangle.Center.X, tile.DrawRectangle.Center.Y),
                    new Vector2(mouse.Center.X, mouse.Center.Y));

                if (InputHelper.IsLeftMousePressed())
                {
                    int index = CalcHelper.GetIndexInGameWorld(mouse.Center.X, mouse.Center.Y);
                    Tile other = GameWorld.GetTile(index);

                    SelectingAnotherTile = false;
                    isBeingInteractedWith = false;
                    ButtonWasReleased = false;
                    LevelEditor.ForceUpdateTile = null;

                    if (other.HasInteractable() && other.Interactable.CanBeLinkedByOtherInteractables)
                    {
                        //TODO: Make a connection between these tiles.
                        // Maybe make the interactable in the other tile be subscribed to this interactable's Action event....
                        other.ConnectToInteractable(this);
                        OnConnectionToInteractable(tile, other);
                    }
                    else
                    {
                        line.IsActive = false;
                    }
                }
            }
        }

        /// <summary>
        /// Called when another interactable is linked to this interactable.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="other"></param>
        protected virtual void OnConnectionToInteractable(Tile source, Tile other)
        {
            line.IsActive = true;
            line.Update(new Vector2(source.DrawRectangle.Center.X, source.DrawRectangle.Center.Y), new Vector2(other.DrawRectangle.Center.X, other.DrawRectangle.Center.Y));
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
        public virtual void OnPlayerAction(Tile tile, Player player)
        {
            OnActivation?.Invoke(tile, player);
        }

        /// <summary>
        /// Invoked when the player clicks on the tile in edit mode.
        /// </summary>
        public virtual void OnPlayerClickInEditMode(Tile tile)
        {
            if (CanBeLinkedToOtherInteractables && !SelectingAnotherTile && ButtonWasReleased)
            {
                isBeingInteractedWith = true;
                SelectingAnotherTile = true;
                ButtonWasReleased = false;
                line.IsActive = true;
                line.Update(Vector2.Zero, Vector2.Zero);
                LevelEditor.ForceUpdateTile = tile;
            }
        }

        /// <summary>
        /// Called when the tile is destroyed.
        /// </summary>
        /// <param name="tile"></param>
        public virtual void OnTileDestroyed(Tile tile)
        {
            GameWorld.WorldData.MetaData.Remove(tile.TileIndex);
            OnActivation = null;
            if (isBeingInteractedWith)
            {
                SelectingAnotherTile = false;
            }
            LightingEngine.RemoveDynamicLight(Light);
        }

        /// <summary>
        /// Used to draw special things that are not part of the tile itself.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tile"></param>
        public virtual void Draw(SpriteBatch spriteBatch, Tile tile)
        {
        }

        /// <summary>
        /// Returns true if interacting with this interactable will activate another interactable.
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedToAnotherInteractable()
        {
            return OnActivation != null;
        }
    }
}
