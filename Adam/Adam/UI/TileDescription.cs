using Adam.Misc.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.UI
{
    class TileDescription
    {
        string description = "";
        string name = "";

        Vector2 textPosition;
        Vector2 namePosition;
        SpriteFont nameFont;
        SpriteFont textFont;

        Texture2D image;
        Texture2D border;
        Rectangle imageRect;

        byte ID;
        byte lastID;

        public TileDescription()
        {
            nameFont = ContentHelper.LoadFont("Fonts/objectiveHead");
            textFont = ContentHelper.LoadFont("Fonts/objectiveText");

            imageRect = new Rectangle((int)(230 / Main.WidthRatio), (int)(90 / Main.HeightRatio), (int)(500 / Main.WidthRatio), (int)(300 / Main.HeightRatio));
            namePosition = new Vector2((int)(210 / Main.WidthRatio), (int)(410 / Main.HeightRatio));
            textPosition = new Vector2((int)(210 / Main.WidthRatio), (int)(440 / Main.HeightRatio));

            border = ContentHelper.LoadTexture("Example Tile Images/border");
        }

        public void Update()
        {
            ID = GameWorld.Instance.levelEditor.selectedID;

            if (lastID != ID)
            {

                try
                {
                    image = ContentHelper.LoadTexture("Example Tile Images/" + ID);
                }
                catch (ContentLoadException)
                {
                    image = ContentHelper.LoadTexture("Example Tile Images/missing");
                }

                Tile.Names.TryGetValue(ID, out name);
                Descriptions.TryGetValue(ID, out description);

                description = FontHelper.WrapText(textFont, description, (int)(540 / Main.WidthRatio));

                lastID = ID;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.Instance.levelEditor.onInventory)
            {
                if (image != null)
                {
                    spriteBatch.Draw(image, imageRect, Color.White);
                    spriteBatch.Draw(border, imageRect, Color.White);
                }

                FontHelper.DrawWithOutline(spriteBatch, nameFont, name, namePosition, 2, Color.Yellow, Color.Black);
                spriteBatch.DrawString(textFont, description, textPosition, Color.White);
            }
        }

        static Dictionary<byte, string> Descriptions = new Dictionary<byte, string>()
        {
            {1,"Uprooted (illegally) from God's own garden, these tiles shall provide a luxurious green accent to your build. Maintenance required: things grow on it." },
            {2,"Much harder than dirt, God thought stone wasn't pretty enough for his paradise in the sky. That's why there is so much." },
            {3,"Great for tap dancing, as it really amplifies the sound and provides great acoustics for echoes." },
            {4,"Satan can't stand the cold, so each tile has been prewarmed to 330 degree Fahrenheit.  Make sure to wear your rubber boots!" },
            {5,"During his ritual Monday Cleanings, God sweeps underneath his furniture to remove all the dust.  This dust clumps together and falls to the Earth below, forming vast wastes of nothingness, and becomes sand.  " },
            {6,"This dust from God's cleaning is slightly oranger, because he had some Cheetos the night before." },
            {7,"Short Grass" },
            {8,"Metal" },
            {9,"Tall Grass" },
            {10,"Only the elite could afford walls made of pure gold.  And by pure we mean gold-plated clay brick." },
            {11,"Let there be light! At least that is what the stoner at the thrift shop told me when he gave me this." },
            {12,"Need to lighten the atmosphere?  Nail this into a ceiling tile in one of your elegant dining halls, and give your (unwanted) guests a way not to stub their toes on the wall." },
            {13,"Door" },
            {14,"Woven and twisted by the Nymphs of the Nine Pines, these vines will be able to support the heaviest of weight." },
            {15,"Carved by the Carpenters of the Willow Forest, these ladders are sturdy enough for the, erm, 'largest' of men." },
            {16,"Linked together by the Blacksmiths of the Iron Forges, chains like these won't rust for a time one day longer than normal chains." },
            {17,"Flower" },
            {18,"Architecture is a difficult thing, and if you want your ceiling to remain a ceiling and not add to the floor, you need something to support it.  Pillars will help keep your marble structures looking classy, and help your ceiling obey the laws of physics." },
            {19,"What goodies could be in there? A mighty sword? A royal crown? No... just shiny rocks." },
            {20,"Tech" },
            {21,"Scaffolding" },
            {22,"Spikes" },
            {23,"The essence of life, the purest resource of them all, and you want to swim in it.  Knock yourself out." },
            {24,"Imagine a pot of boiling water.  Then imagine it was a slush of crystals, liquid, and bubbles instead of water.  And it was called 'lava'." },
            {25,"Tainted by Satan himself, this water is not safe enough to drink.  And it stings a bit if you swim in it.  Like being stung by a bee, just over your entire body every second." },
            {26,"Golden Apple" },
            {27,"This is much prettier than the wooden chests. THIS must have the mighty sword, or royal crown, or… nope just some more shiny rocks and a jetpack." },
            {28,"Health Apple" },
            {29,"Who knew white rocks could be so elegant? Perfect for keeping the rain out, as no one wants a soggy meal or wet floor." },
            {30,"Marble Ceiling Support" },
            {31,"Grown from the acorns that grew on the branches of the Tree of Knowledge, these trees will one day grow slightly bigger than they are now." },
            {32,"Small Rock" },
            {33,"After God removed the mountains from around the Tree of Knowledge, he didn't do a good job of removing the debris.  Boulders like this are from the center of Mount Una'waki, making them 0.01% more valuable." },
            {34,"Medium Rock" },
            {35,"Pebbles" },
            {36,"It reads: 'Caution: Private property.  Violators will be smited, survivors will be fed to the dinosaurs.'" },
            {37,"Checkpoint" },
            {38,"For when your majesty just came out of college and could only afford a cheap castle." },
            {39,"Don't worry, this ice is thick enough it won't break.  However, you can still slip and fall on your butt." },
            {40,"Coated in cloud-shavings, the grass here does not require maintenance.  God is a bit inconsiderate when it comes to cleaning up his mess." },
            {41, "Void tile" },

            {100,"Gold Brick Wall" },
            {101,"Stone Wall" },
            {102,"Dirt Wall" },
            {103,"To keep out those pesky neighbor kids and their pet dinosaur, Gerald." },
            {104,"Marble Wall" },
            {105,"Sand Wall" },
            {106,"Hellstone Wall" },
            {107,"Stone brick Wall" },
            {108,"" },

            {200,"Player" },
            {201,"Snake" },
            {202,"Frog" },
            {203,"God" },
            {204,"Lost" },
            {205,"Hellboar" },
            {206,"Falling Boulder (Desert)" },
            {207,"Bat" },
            {208, "Duck" },
            {209,"Flying Wheel" },
        };
    }
}
