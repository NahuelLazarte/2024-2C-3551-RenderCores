using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Content;
using TGC.MonoGame.TP.Collisions;
using System.Collections.Specialized;
using System.Threading;
using BepuPhysics.CollisionDetection.SweepTasks;
using System.Net.Http.Headers;
using BepuPhysics.Collidables;

namespace TGC.MonoGame.TP.Objects{
    class Sphere {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";

        public Effect Effect { get; set; }
        public int index { get; set; }

        public Model SphereModel{get; set;}
        public Matrix SphereWorld{get; set;}
        public BoundingCylinder SphereCollider;
        public BoundingBox[] Colliders { get; set; }
        
        public Matrix SphereScale { get; set; }
        public Matrix SphereRotation { get; set; }
        public Vector3 SpherePosition { get; set; }
        public Vector3 SphereVelocity { get; set; }
        public Vector3 SphereAcceleration { get; set; }
        public Vector3 SphereFrontDirection { get; set; }

        //Camara
        public FollowCamera SphereCamera { get; set; }

        // Valores que afectan el movimiento de la esfera
        private const float SphereRotatingVelocity = 0.06f;
        private const float SphereSideSpeed = 30f;
        private const float SphereJumpSpeed = 30f;
        private const float EPSILON = 0.00001f;
        private const float Gravity = 30f;

        private bool OnGround { get; set; }
        
        private static bool Compare(float a, float b){
            return MathF.Abs(a - b) < float.Epsilon;
        }

        public Sphere(Vector3 initialPosition){
            //Inicializacion de la esfera
            SpherePosition = initialPosition;
            SphereScale = Matrix.CreateScale(0.01f);
            
            SphereCollider = new BoundingCylinder(SpherePosition, 4f, 4f);
            /*SphereRotation = Matrix.Identity;
            SphereFrontDirection = Vector3.Backward;*/

            //Atributos para el movimiento de la esfera
            SphereAcceleration = Vector3.Down * Gravity;
            SphereVelocity = Vector3.Zero;
        }

        public void LoadContent(ContentManager Content){
            SphereModel = Content.Load<Model>(ContentFolder3D + "Spheres/sphere");

            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");
            
            foreach (var mesh in SphereModel.Meshes){
                foreach (var meshPart in mesh.MeshParts){
                    meshPart.Effect = Effect;
                }
            }
            
            var extents = BoundingVolumesExtensions.CreateAABBFrom(SphereModel);
            var height = extents.Max.Y - extents.Min.Y;
            SpherePosition += height * 0.5f * Vector3.Up * SphereScale.M22;
            SphereCollider.Center = SpherePosition;
            SphereWorld = SphereScale * Matrix.CreateTranslation(SpherePosition);
        }

        public void Update(GameTime gameTime){

            var deltaTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            var keyboardState = Keyboard.GetState();

            Vector3 friccion = -SphereVelocity * 0.05f;

            if (keyboardState.IsKeyDown(Keys.W))
                SphereAcceleration += Vector3.Forward;
            if (keyboardState.IsKeyDown(Keys.S))
                SphereAcceleration += Vector3.Backward;
            if (keyboardState.IsKeyDown(Keys.A))
                SphereAcceleration += Vector3.Left;
            if (keyboardState.IsKeyDown(Keys.D))
                SphereAcceleration += Vector3.Right;
            if (keyboardState.IsKeyDown(Keys.Space) && OnGround) {
                SphereVelocity += Vector3.Up * SphereJumpSpeed;
            }
            SphereAcceleration += new Vector3(friccion.X, 0f, friccion.Z);
            if(SphereAcceleration.X == 0f){
                SphereVelocity += new Vector3(friccion.X, 0f, friccion.Z);
            }
            if(SphereAcceleration.Z == 0f){
                SphereVelocity += new Vector3(friccion.X, 0f, friccion.Z);
            }

            SphereVelocity += SphereAcceleration * deltaTime;
            var scaledVelocity = SphereVelocity * deltaTime;

            SolveVerticalMovement(scaledVelocity); 

            //SphereVelocity = new Vector3(SphereVelocity.X, 0f, SphereVelocity.Z);

            SolveHorizontalMovementSliding(scaledVelocity);

            SpherePosition = SphereCollider.Center;
            //SphereVelocity = new Vector3(0f, SphereVelocity.Y, 0f);
            SphereWorld = SphereScale * Matrix.CreateTranslation(SpherePosition);
            
            if(SpherePosition.Y < -10f){
                respawn();
            }
        }

        private void respawn(/*checkpoint checkpoint*/){
            SphereCollider.Center = new(0f, 30f, 0f); //Aca deberia cambiarse si pasa un checkpoint
            SpherePosition = SphereCollider.Center;
            SphereAcceleration = Vector3.Down * Gravity;
            SphereVelocity = Vector3.Zero;
        }

        private void SolveVerticalMovement(Vector3 scaledVelocity){
            // If the Robot has vertical velocity
            if (scaledVelocity.Y == 0f)
                return;

            // Start by moving the Cylinder
            SphereCollider.Center += Vector3.Up * scaledVelocity.Y;
            // Set the OnGround flag on false, update it later if we find a collision
            OnGround = false;

            // Collision detection
            var collided = false;
            var foundIndex = -1;
            for (var index = 0; index < Colliders.Length; index++){
                if(!SphereCollider.Intersects(Colliders[index]).Equals(BoxCylinderIntersection.Intersecting))
                    continue;
                
                // If we collided with something, set our velocity in Y to zero to reset acceleration
                SphereVelocity = new Vector3(SphereVelocity.X, 0f, SphereVelocity.Z);

                // Set our index and collision flag to true
                // The index is to tell which collider the Robot intersects with
                collided = true;
                foundIndex = index;
                break;
            }


            // We correct based on differences in Y until we don't collide anymore
            // Not usual to iterate here more than once, but could happen
            while (collided){
                var collider = Colliders[foundIndex];
                var colliderY = BoundingVolumesExtensions.GetCenter(collider).Y;
                var sphereY = SphereCollider.Center.Y;
                var extents = BoundingVolumesExtensions.GetExtents(collider);

                float penetration;
                // If we are on top of the collider, push up
                // Also, set the OnGround flag to true
                if (sphereY > colliderY){
                    penetration = colliderY + extents.Y - sphereY + SphereCollider.HalfHeight;
                    OnGround = true;
                } else {
                    penetration = -sphereY -SphereCollider.HalfHeight + colliderY - extents.Y;
                }
                // Move our Cylinder so we are not colliding anymore
                SphereCollider.Center += Vector3.Up * penetration;
                collided = false;

                // Check for collisions again
                for (var index = 0; index < Colliders.Length; index++)
                {
                    if (!SphereCollider.Intersects(Colliders[index]).Equals(BoxCylinderIntersection.Intersecting))
                        continue;

                    // Iterate until we don't collide with anything anymore
                    collided = true;
                    foundIndex = index;
                    break;
                }
            }
            
        }

        private void SolveHorizontalMovementSliding(Vector3 scaledVelocity)
        {
            // Has horizontal movement?
            if (Vector3.Dot(scaledVelocity, new Vector3(1f, 0f, 1f)) == 0f)
                return;
            
            // Start by moving the Cylinder horizontally
            SphereCollider.Center += new Vector3(scaledVelocity.X, 0f, scaledVelocity.Z);

            // Check intersection for every collider
            for (var index = 0; index < Colliders.Length; index++)
            {
                if (!SphereCollider.Intersects(Colliders[index]).Equals(BoxCylinderIntersection.Intersecting))
                    continue;

                // Get the intersected collider and its center
                var collider = Colliders[index];
                var colliderCenter = BoundingVolumesExtensions.GetCenter(collider);

                // The Robot collided with this thing
                // Is it a step? Can the Robot climb it?
                bool stepClimbed = SolveStepCollision(collider, index);

                // If the Robot collided with a step and climbed it, stop here
                // Else go on
                if (stepClimbed)
                    return;

                // Get the cylinder center at the same Y-level as the box
                var sameLevelCenter = SphereCollider.Center;
                sameLevelCenter.Y = colliderCenter.Y;

                // Find the closest horizontal point from the box
                var closestPoint = BoundingVolumesExtensions.ClosestPoint(collider, sameLevelCenter);

                // Calculate our normal vector from the "Same Level Center" of the cylinder to the closest point
                // This happens in a 2D fashion as we are on the same Y-Plane
                var normalVector = sameLevelCenter - closestPoint;
                var normalVectorLength = normalVector.Length();

                // Our penetration is the difference between the radius of the Cylinder and the Normal Vector
                // For precission problems, we push the cylinder with a small increment to prevent re-colliding into the geometry
                var penetration = SphereCollider.Radius - normalVector.Length() + EPSILON;

                // Push the center out of the box
                // Normalize our Normal Vector using its length first
                SphereCollider.Center += (normalVector / normalVectorLength * penetration);
            }
            
        }

        private bool SolveStepCollision(BoundingBox collider, int colliderIndex)
        {
            // Get the collider properties to check if it's a step
            // Also, to calculate penetration
            var extents = BoundingVolumesExtensions.GetExtents(collider);
            var colliderCenter = BoundingVolumesExtensions.GetCenter(collider);

            // Is this collider a step?
            // If not, exit
            if (extents.Y >= 6f)
                return false;

            // Is the base of the cylinder close to the step top?
            // If not, exit
            var distanceToTop = MathF.Abs((SphereCollider.Center.Y - SphereCollider.HalfHeight) - (colliderCenter.Y + extents.Y));
            if (distanceToTop >= 12f)
                return false;

            // We want to climb the step
            // It is climbable if we can reposition our cylinder in a way that
            // it doesn't collide with anything else
            var pastPosition = SphereCollider.Center;
            SphereCollider.Center += Vector3.Up * distanceToTop;
            for (int index = 0; index < Colliders.Length; index++)
                if (index != colliderIndex && SphereCollider.Intersects(Colliders[index]).Equals(BoxCylinderIntersection.Intersecting))
                {
                    // We found a case in which the cylinder
                    // intersects with other colliders, so the climb is not possible
                    SphereCollider.Center = pastPosition;
                    return false;
                }

            // If we got here the climb was possible
            // (And the Robot position was already updated)
            return true;
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection){
            Effect.Parameters["View"].SetValue(view); //Cambio View por Eso
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Yellow.ToVector3());
            foreach (var mesh in SphereModel.Meshes)
            {
                Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * SphereWorld);
                mesh.Draw();
            }
        }
    }
}