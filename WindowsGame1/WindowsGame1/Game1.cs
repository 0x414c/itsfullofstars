using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1 {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private readonly RenderTarget2D[] myRenderTarget2D = new RenderTarget2D[2];
        private Texture2D bufferTexture;
        private Texture2D mySceneBackground;
        //private Texture2D vignetteTexture;
        private Effect myEffect;
        private SpriteFont mySpriteFont;
        private ParticleEngine particleEngine;

        private string text = "";
        //private string systemUptime = "";
        private float timer;

        private const int renderPassesCount = 3;
        private const int screenWidth = 1280;
        private const int screenHeight = 1024;
        private const bool isFullscreen = true;


        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = isFullscreen;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            Debug.Assert(renderPassesCount >= 0);

            for (int i = 0; i < 2; i++) {
               myRenderTarget2D[i] = new RenderTarget2D(
               GraphicsDevice,
               GraphicsDevice.PresentationParameters.BackBufferWidth,
               GraphicsDevice.PresentationParameters.BackBufferHeight,
               false,
               GraphicsDevice.PresentationParameters.BackBufferFormat,
               DepthFormat.Depth24);
            }
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            mySceneBackground = Content.Load<Texture2D>("Gray");
            //vignetteTexture = Content.Load<Texture2D>("vignette");

            myEffect = Content.Load<Effect>("Effect1");
            Debug.Assert(renderPassesCount < myEffect.CurrentTechnique.Passes.Count);
            myEffect.Parameters["WIDTH"].SetValue(graphics.PreferredBackBufferWidth);
            myEffect.Parameters["HEIGHT"].SetValue(graphics.PreferredBackBufferHeight);

            mySpriteFont = Content.Load<SpriteFont>("SpriteFont1");

            var textures = new List<Texture2D> {
                Content.Load<Texture2D>("star"),
                Content.Load<Texture2D>("diamond"),
                Content.Load<Texture2D>("circle")
            };
            particleEngine = new ParticleEngine(textures, new Vector2(400, 240));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
            myRenderTarget2D[0].Dispose();
            myRenderTarget2D[1].Dispose();
        }

        //public TimeSpan UpTime {
        //    get {
        //        using (var uptime = new PerformanceCounter("System", "System Up Time")) {
        //            uptime.NextValue();       //Call this an extra time before reading its value
        //            return TimeSpan.FromSeconds(uptime.NextValue());
        //        }
        //    }
        //}

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) ||
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
            // TODO: Add your update logic here
            text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            //systemUptime = UpTime.ToString();
            timer += 0.01f;
            particleEngine.EmitterLocation = new Vector2(
                graphics.PreferredBackBufferWidth / 2.0f,
                graphics.PreferredBackBufferHeight / 2.0f);
            //particleEngine.EmitterLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            particleEngine.Update();
            base.Update(gameTime);
        }

        //private void DrawModel(Model model) {
        //    Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        //    Matrix view = Matrix.CreateLookAt(new Vector3(10, 10, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        //    Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 480f, 0.1f, 100f);

        //    foreach (ModelMesh mesh in model.Meshes) {
        //        foreach (BasicEffect effect in mesh.Effects) {
        //            effect.EnableDefaultLighting();
        //            effect.World = world;
        //            effect.View = view;
        //            effect.Projection = projection;
        //        }
        //        mesh.Draw();
        //    }
        //}

        private void Drawer(SpriteBatch sprite) {
            particleEngine.Draw(spriteBatch);
            sprite.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            sprite.DrawString(mySpriteFont, String.Format("{0,10:F2}", timer), new Vector2(
                graphics.PreferredBackBufferWidth - 250,
                graphics.PreferredBackBufferHeight - 100),
                Color.Gray);
            sprite.DrawString(mySpriteFont, text, new Vector2(
                72,
                graphics.PreferredBackBufferHeight - 100),
                Color.LightGray);
            //sprite.Draw(vignetteTexture, new Rectangle(0, 0,
            //        graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
            //        Color.White);
            sprite.End();
        }

        /// <summary>
        /// Draws the entire scene in the given render target.
        /// </summary>
        /// <returns>A texture2D with the scene drawn in it.</returns>
        private Texture2D DrawToTexture(RenderTarget2D renderTarget, Texture2D picture, Effect effect, int pass) {
            // Set the render target
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };

            // Draw the scene
            GraphicsDevice.Clear(Color.Black);

            using (var sprite = new SpriteBatch(GraphicsDevice)) {
                sprite.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                effect.CurrentTechnique.Passes[pass].Apply();
                sprite.Draw(picture, new Rectangle(0, 0,
                    graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                    Color.White); //Draw bg or rendertarget if multipass.
                sprite.End(); //
                if (pass == 0) {
                    Drawer(sprite);
                }
            }

            // Reset the render target
            GraphicsDevice.SetRenderTarget(null);
            // Return the texture in the render target
            return renderTarget;
        }

        /// <summary>
        /// Applies the given shader passes sequentially.
        /// Draws with 2 swapping render targets.
        /// </summary>
        /// <returns>A texture2D with the final result.</returns>
        private Texture2D MultipassRenderer2D(RenderTarget2D[] renderTarget, int sPass, int ePass, Texture2D initialTexture2D) {
            int i = sPass;
            //Draw background for 1 time.
            DrawToTexture(myRenderTarget2D[0], initialTexture2D, myEffect, sPass);
            //Apply different shader passes by swapping 2 targets
            while (i <= ePass) {
                if (i % 2 == 0) DrawToTexture(myRenderTarget2D[1], myRenderTarget2D[0], myEffect, i);
                else DrawToTexture(myRenderTarget2D[0], myRenderTarget2D[1], myEffect, i);
                i++;
            }
            //Return last processed target.
            return renderTarget[i % 2];
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            // TODO: Add your drawing code here
            bufferTexture = MultipassRenderer2D(myRenderTarget2D, 0, renderPassesCount, mySceneBackground);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.Default,
                RasterizerState.CullNone);
            spriteBatch.Draw(bufferTexture,
                new Rectangle(0, 0,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight),
                Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}