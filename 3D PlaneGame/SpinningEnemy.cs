using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3D_PlaneGame
{
    class SpinningEnemy : BasicModel
    {
        Matrix rotation = Matrix.Identity;

        //vector to rotate
        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        Vector3 direction;

        public override Matrix GetWorld()
        {
            return rotation * world;
        }

        public SpinningEnemy(Model m, Vector3 Position,Vector3 Direction
            , float yaw, float pitch, float roll)
            : base(m)
        {
            world = Matrix.CreateTranslation(Position);
            yawAngle = yaw;
            pitchAngle = pitch;
            rollAngle = roll;
            direction = Direction;
        }

        public override void Update()
        {
            // Rotate model
            rotation *= Matrix.CreateFromYawPitchRoll(yawAngle, pitchAngle, rollAngle);

            // Move model
            world *= Matrix.CreateTranslation(direction);
        }
    }
}
