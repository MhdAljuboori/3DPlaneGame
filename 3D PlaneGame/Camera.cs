using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace _3D_PlaneGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Camera(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }
        
        
        //Camera matrices
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        // Camera vectors to rotate and Move Camera
        public Vector3 cameraPosition { get; protected set; }
        Vector3 cameraDirection;
        Vector3 cameraUp;

        //to rotate camera
        MouseState prevMouseState;

        // Max yaw/pitch variables
        float totalYaw = MathHelper.PiOver4 / 2;
        float currentYaw = 0;
        float totalPitch = MathHelper.PiOver4 / 2;
        float currentPitch = 0;

        public Vector3 GetCameraDirection
        {
            get { return cameraDirection; }
        }

        //define new view matrix
        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition,cameraPosition + cameraDirection, cameraUp);
        }

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            //view = Matrix.CreateLookAt(pos, target, up);
            // Build camera view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /(float)Game.Window.ClientBounds.Height,
                1, 3000);
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            // Set mouse position and do initial get state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2
                , Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            
            // Yaw rotation
            float yawAngle = (-MathHelper.PiOver4 / 150) *(Mouse.GetState().X - prevMouseState.X);
            if (Math.Abs(currentYaw + yawAngle) < totalYaw)
            {
                cameraDirection = Vector3.Transform(cameraDirection
                    , Matrix.CreateFromAxisAngle(cameraUp, yawAngle));
                currentYaw += yawAngle;
            }

            // Pitch rotation
            float pitchAngle = (MathHelper.PiOver4 / 150) *(Mouse.GetState( ).Y - prevMouseState.Y);
            if (Math.Abs(currentPitch + pitchAngle) < totalPitch)
            {
                cameraDirection = Vector3.Transform(cameraDirection
                    , Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection)
                    , pitchAngle));

                //if we want to change Up vector
                /*currentPitch += pitchAngle;
                cameraUp = Vector3.Transform(cameraUp, Matrix.CreateFromAxisAngle
                    (Vector3.Cross(cameraUp, cameraDirection), pitchAngle));*/
            }

            // Reset prevMouseState
            prevMouseState = Mouse.GetState();


            // Recreate the camera view matrix
            CreateLookAt();

            base.Update(gameTime);
        }
    }
}