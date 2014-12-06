using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using MovingPicture;

namespace XepHinhTroll
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static string ImagePath = "mario1";

        ContentManager contentManager;
        GameTimer timer;
        GameBoard board;
        Texture2D bigSample;
        Texture2D sample;
        int cellWidth;
        int cellHeight;
        Position movingPosition;
        SoundEffect soundHyperspaceActivation;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            board = new GameBoard(3, 4);
            board.RandomizeFragments(10000);
            initialWidthAndHeightOfCell();
            bigSample = contentManager.Load<Texture2D>(ImagePath);
            sample = crop(
                bigSample,
                new Microsoft.Xna.Framework.Rectangle(
                    0,
                    0,
                    SharedGraphicsDeviceManager.DefaultBackBufferWidth,
                    SharedGraphicsDeviceManager.DefaultBackBufferHeight - cellHeight
                )
            );
            soundHyperspaceActivation =
                contentManager.Load<SoundEffect>("shovelsn");

            base.Initialize();
        }

        private void initialWidthAndHeightOfCell()
        {
            int deviceWidth = SharedGraphicsDeviceManager.DefaultBackBufferWidth;
            int deviceHeight = SharedGraphicsDeviceManager.DefaultBackBufferHeight;

            int reallyWidth = deviceWidth;
            int reallyHeight = deviceHeight - (deviceHeight / (board.RowNumber + 1)); // Ignore a small space to show target picture.

            cellWidth = reallyWidth / board.ColumnNumber;
            cellHeight = reallyHeight / board.RowNumber;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            // Process touch events
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if (tl.State == TouchLocationState.Pressed)
                {
                    int x = (int)tl.Position.X;
                    int y = (int)tl.Position.Y;

                    movingPosition = convertToIndex(x, y);
                }

                if ((tl.State == TouchLocationState.Moved))
                {
                    int x = (int)tl.Position.X;
                    int y = (int)tl.Position.Y;

                    Position position = convertToIndex(x, y);
                    if (board.BlankX == position.X && board.BlankY == position.Y)
                    {
                        if (board.MoveAt(movingPosition.X, movingPosition.Y))
                        {
                            movingPosition.X = position.X;
                            movingPosition.Y = position.Y;
                            soundHyperspaceActivation.Play();
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        private Position convertToIndex(int x, int y)
        {
            Position position = new Position();
            position.X = (int)x / cellWidth;
            position.Y = (int)y / cellHeight;

            return position;
        }

        private Texture2D crop(Texture2D image, Microsoft.Xna.Framework.Rectangle source)
        {
            GraphicsDevice graphics = SharedGraphicsDeviceManager.Current.GraphicsDevice;
            RenderTarget2D ret = new RenderTarget2D(graphics, source.Width, source.Height);

            graphics.SetRenderTarget(ret); // draw to image
            graphics.Clear(new Color(0, 0, 0, 0));

            spriteBatch.Begin();
            spriteBatch.Draw(image, Vector2.Zero, source, Color.White);
            spriteBatch.End();

            graphics.SetRenderTarget(null); // set back to main window

            return (Texture2D)ret;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            int indentBetweenCells = 3;
            drawGameView(spriteBatch, sample, cellWidth, cellHeight, indentBetweenCells);

            int sampleIndent = 10;
            drawSample(spriteBatch, sample, cellWidth, cellHeight, sampleIndent, indentBetweenCells);

            base.Draw(gameTime);
        }

        private void drawSample(SpriteBatch spriteBatch, Texture2D sample, int cellWidth, int cellHeight, int sampleIndent, int indentBetweenCells)
        {
            int deviceWidth = SharedGraphicsDeviceManager.DefaultBackBufferWidth;
            int sampleXPosition = (int)(cellWidth + sampleIndent + (float)((deviceWidth - cellWidth) / 2) - (float)(cellWidth / 2));
            int sampleYPosition = sampleIndent + (board.RowNumber * cellHeight);

            Texture2D dummyTexture = new Texture2D(SharedGraphicsDeviceManager.Current.GraphicsDevice, 1, 1);
            Microsoft.Xna.Framework.Rectangle dummyRectangle =
                new Microsoft.Xna.Framework.Rectangle(
                    cellWidth + indentBetweenCells, sampleYPosition - sampleIndent + indentBetweenCells,
                    deviceWidth - cellWidth - indentBetweenCells, cellHeight - indentBetweenCells);
            dummyTexture.SetData(new Color[] { Color.White });

            spriteBatch.Begin();
            spriteBatch.Draw(
                dummyTexture,
                dummyRectangle,
                Color.White);
            spriteBatch.Draw(
                        sample,
                        new Microsoft.Xna.Framework.Rectangle(
                            sampleXPosition,
                            sampleYPosition,
                            cellWidth - (sampleIndent * 2),
                            cellHeight - (sampleIndent * 2)),
                        Color.White
                    );

            spriteBatch.End();
        }

        private void drawGameView(SpriteBatch spriteBatch, Texture2D sample, int cellWidth, int cellHeight, int indentBetweenCells)
        {
            spriteBatch.Begin();
            for (int rowIndex = 0; rowIndex < board.RowNumber; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < board.ColumnNumber; columnIndex++)
                {
                    drawCell(rowIndex, columnIndex, indentBetweenCells);
                }
            }
            drawCell(board.RowNumber, 0, indentBetweenCells);

            spriteBatch.End();
        }

        private void drawCell(int rowIndex, int columnIndex, int indentBetweenCells)
        {
            if (board.PictureMatrix[rowIndex, columnIndex] == 0)
            {
                return;
            }

            int number = board.TotalElementNumberExceptBlankCell - board.PictureMatrix[rowIndex, columnIndex];
            int x = number % board.ColumnNumber;
            int y = number / board.ColumnNumber;

            spriteBatch.Draw(
                bigSample,
                new Vector2(
                    indentBetweenCells + (columnIndex * cellWidth),
                    indentBetweenCells + (rowIndex * cellHeight)),
                new Microsoft.Xna.Framework.Rectangle(
                    indentBetweenCells + (x * cellWidth),
                    indentBetweenCells + (y * cellHeight),
                    cellWidth - (indentBetweenCells * 2),
                    cellHeight - (indentBetweenCells * 2)),
                Color.White,
                0.0f,
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                SpriteEffects.None,
                0.0f
            );
        }
    }
}
