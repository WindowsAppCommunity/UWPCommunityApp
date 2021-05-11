using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;

namespace UWPCommunity.Views.Subviews
{
    partial class LlamaBingo
    {
        ParticleSystem confetti = new ConfettiParticleSystem();
        public bool UseSpriteBatch { get; set; }
        public bool ConfettiEnabled { get; set; } = false;

        private void BingoCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            if (args.Reason == CanvasCreateResourcesReason.DpiChanged)
                return;

            if (args.Reason == CanvasCreateResourcesReason.FirstTime)
            {
                bool spriteBatchSupported = CanvasSpriteBatch.IsSupported(sender.Device);

                UseSpriteBatch = spriteBatchSupported;
            }

            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }
        async Task CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            await confetti.CreateResourcesAsync(sender);
        }

        private void BingoCanvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            var elapsedTime = (float)args.Timing.ElapsedTime.TotalSeconds;
            CreateConfetti(elapsedTime);
            confetti.Update(elapsedTime);
        }

        // This function is called when we want to demo the explosion effect. It updates the
        // timeTillExplosion timer, and starts another explosion effect when the timer reaches zero.
        void CreateConfetti(float elapsedTime)
        {
            Vector2 where = BingoCanvas.Size.ToVector2();

            // Create the explosion at some random point on the screen.
            where.X *= Common.RandomBetween(0.25f, 0.75f);
            where.Y *= Common.RandomBetween(0.25f, 0.75f);

            // The overall explosion effect is actually comprised of two particle systems:
            // the fiery bit, and the smoke behind it. Add particles to both of those systems.
            confetti.AddParticles(where);
        }

        private void BingoCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            if (!ConfettiEnabled)
                return;

            confetti.Draw(args.DrawingSession, UseSpriteBatch);
        }
    }

    // ExplosionParticleSystem is a specialization of ParticleSystem which creates a fiery explosion.
    // It should be combined with ExplosionSmokeParticleSystem for best effect.
    public class ConfettiParticleSystem : ParticleSystem
    {
        // Set up the constants that will give this particle system its behavior and properties.
        protected override void InitializeConstants()
        {
            //bitmapFilename = "Assets/ConfettiParticles/blue.png";

            // High initial speed with lots of variance. Make the values closer
            // together to have more consistently circular explosions.
            minInitialSpeed = 30;
            maxInitialSpeed = 300;

            // Doesn't matter what these values are set to, acceleration is tweaked in InitializeParticle.
            minAcceleration = 0;
            maxAcceleration = 0;

            // Explosions should be relatively short lived.
            minLifetime = .5f;
            maxLifetime = 1.0f;

            minScale = .3f;
            maxScale = 1.0f;

            minNumParticles = 20;
            maxNumParticles = 25;

            minRotationSpeed = -(float)Math.PI / 4;
            maxRotationSpeed = (float)Math.PI / 4;

            // Additive blending is very good at creating fiery effects.
            blendState = CanvasBlend.Add;
        }


        public override async Task CreateResourcesAsync(ICanvasResourceCreator resourceCreator)
        {
            await base.CreateResourcesAsync(resourceCreator);

            // This particle system uses additive blending, which has a side effect of leaving 1.0
            // in the framebuffer alpha channel for black texels (which are visually transparent when
            // using additive blending). This doesn't normally affect anything (it is irrelevant what
            // the framebuffer alpha ends up containing, as long as the color channels look the way we
            // want) but thumbnail generation relies on alpha to blend the icon over its background color.
            // To avoid ugly black borders around the thumbnail, we edit the bitmap to have zero alpha,
            // leaving only additive RGB data.

            //if (ThumbnailGenerator.IsDrawingThumbnail)
            //{
            //    var colors = bitmap.GetPixelColors();

            //    for (int i = 0; i < colors.Length; i++)
            //    {
            //        colors[i].A = 0;
            //    }

            //    bitmap.SetPixelColors(colors);
            //}
        }


        protected override void InitializeParticle(Particle particle, Vector2 where)
        {
            base.InitializeParticle(particle, where);

            // The base works fine except for acceleration. Explosions move outwards,
            // then slow down and stop because of air resistance. Let's change acceleration
            // so that when the particle is at max lifetime, the velocity will be zero.
            //
            // We'll use the equation vt = v0 + (a0 * t). If you're not familar with this,
            // it's one of the basic kinematics equations for constant acceleration, and
            // basically says: velocity at time t = initial velocity + acceleration * t.
            //
            // We'll solve the equation for a0, using t = particle.Lifetime and vt = 0.

            particle.Acceleration = -particle.Velocity / particle.Lifetime;
        }
    }

    // ParticleSystem is an abstract class that provides the basic functionality to
    // create a particle effect. Different subclasses will have different effects,
    // such as fire, explosions, and plumes of smoke. To use these subclasses, 
    // simply call AddParticles, and pass in where the particles should be created.
    public abstract class ParticleSystem
    {
        // The texture this particle system will use.
        protected CanvasBitmap[] bitmaps;

        Vector2 bitmapCenter;
        Rect bitmapBounds;

        // The particles currently in use by this system. These are reused,
        // so calling AddParticles will not normally cause any allocations.
        List<Particle> activeParticles = new List<Particle>();

        // Keeps track of particles that are not curently being used by an effect.
        // When new particles are requested, particles are taken from here.
        // When particles are finished they are transferred back to here.
        static Stack<Particle> freeParticles = new Stack<Particle>();


        // This region of values control the "look" of the particle system, and should 
        // be set by derived particle systems in the InitializeConstants method. The
        // values are then used by the virtual function InitializeParticle. Subclasses
        // can override InitializeParticle for further customization.

        #region Constants to be set by subclasses

        // minNumParticles and maxNumParticles control the number of particles that are
        // added when AddParticles is called. The number of particles will be a random
        // number between minNumParticles and maxNumParticles.
        protected int minNumParticles;
        protected int maxNumParticles;

        // This controls the bitmap that the particle system uses.
        protected string bitmapFilename;

        // minInitialSpeed and maxInitialSpeed are used to control the initial velocity
        // of the particles. The particle's initial speed will be a random number between these two.
        // The direction is determined by the function PickRandomDirection, which can be overriden.
        protected float minInitialSpeed;
        protected float maxInitialSpeed;

        // minAcceleration and maxAcceleration are used to control the acceleration of the particles.
        // The particle's acceleration will be a random number between these two. By default, the
        // direction of acceleration is the same as the direction of the initial velocity.
        protected float minAcceleration;
        protected float maxAcceleration;

        // minRotationSpeed and maxRotationSpeed control the particles' angular velocity: the
        // speed at which particles will rotate. Each particle's rotation speed will be a random
        // number between minRotationSpeed and maxRotationSpeed. Use smaller numbers to make
        // particle systems look calm and wispy, and large numbers for more violent effects.
        protected float minRotationSpeed;
        protected float maxRotationSpeed;

        // minLifetime and maxLifetime are used to control the lifetime. Each particle's lifetime
        // will be a random number between these two. Lifetime is used to determine how long a
        // particle "lasts." Also, in the base implementation of Draw, lifetime is also used to
        // calculate alpha and scale values to avoid particles suddenly "popping" into view
        protected float minLifetime;
        protected float maxLifetime;

        // to get some additional variance in the appearance of the particles, we give them all
        // random scales. the scale is a value between minScale and maxScale, and is additionally
        // affected by the particle's lifetime to avoid particles "popping" into view.
        protected float minScale;
        protected float maxScale;

        // Different effects can use different blend states.
        // Fire and explosions work well with additive blending, for example.
        protected CanvasBlend blendState;

        #endregion


        // Constructs a new ParticleSystem.
        protected ParticleSystem()
        {
            InitializeConstants();
        }


        // This abstract function must be overriden by subclasses of ParticleSystem. It is
        // here that they should set all the constants marked in the region "constants to
        // be set by subclasses", which give each ParticleSystem its specific flavor.
        protected abstract void InitializeConstants();


        private readonly Windows.UI.Color[] ColorSelection = new[]
        {
            Windows.UI.Color.FromArgb(255, 244, 67, 54),    // Red
            Windows.UI.Color.FromArgb(255, 33, 150, 243),   // Blue
            Windows.UI.Color.FromArgb(255, 76, 175, 80),    // Green
            Windows.UI.Color.FromArgb(255, 255, 193, 7),    // Yellow

        };
        // Loads the bitmap that will be used to draw this particle system.
        public virtual async Task CreateResourcesAsync(ICanvasResourceCreator resourceCreator)
        {
            if (bitmapFilename != null)
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + bitmapFilename));
                var stream = await file.OpenReadAsync();
                bitmaps = new[] { await CanvasBitmap.LoadAsync(resourceCreator, stream) };
            }
            else
            {
                bitmaps = new CanvasBitmap[ColorSelection.Length];
                for (int r = 0; r < bitmaps.Length; r++)
                {
                    var color = ColorSelection[r];
                    int numPixels = 10 * 20;
                    var colors = new Windows.UI.Color[numPixels];
                    for (int i = 0; i < numPixels; i++)
                    {
                        colors[i] = color;
                    }
                    bitmaps[r] = CanvasBitmap.CreateFromColors(resourceCreator, colors, 10, 20);
                }
            }

            bitmapCenter = bitmaps[0].Size.ToVector2() / 2;
            bitmapBounds = bitmaps[0].Bounds;
        }


        // AddParticles's job is to add an effect somewhere on the screen.
        public void AddParticles(Vector2 where)
        {
            // The number of particles we want for this effect is a random number
            // somewhere between the two constants specified by the subclasses.
            int numParticles =  new Random().Next(minNumParticles, maxNumParticles);

            // Activate that many particles.
            for (int i = 0; i < numParticles; i++)
            {
                // Grab a particle from the freeParticles store, or create a new one if freeParticles is empty.
                Particle particle = (freeParticles.Count > 0) ? freeParticles.Pop() : new Particle();

                // No idea how this happens, but it sometimes does
                // when switching into compact overlay
                if (particle == null) continue;
                InitializeParticle(particle, where);

                activeParticles.Add(particle);
            }
        }


        // Randomizes some properties for a particle, then calls Initialize on it.
        // This can be overriden by subclasses if they  want to modify the way particles
        // are created. For example, SmokePlumeParticleSystem overrides this function
        // make all particles accelerate to the right, simulating wind.
        protected virtual void InitializeParticle(Particle particle, Vector2 where)
        {
            // First, call PickRandomDirection to figure out which way the particle
            // will be moving. Velocity and acceleration values will come from this.
            Vector2 direction = PickRandomDirection();

            var bitmapIndex = new Random().Next() % bitmaps.Length;

            // Pick some random values for our particle.
            float velocity = Common.RandomBetween(minInitialSpeed, maxInitialSpeed);
            float acceleration = Common.RandomBetween(minAcceleration, maxAcceleration);
            float lifetime = Common.RandomBetween(minLifetime, maxLifetime);
            float scale = Common.RandomBetween(minScale, maxScale);
            float rotationSpeed = Common.RandomBetween(minRotationSpeed, maxRotationSpeed);

            // Then initialize the particle with these random values.
            particle.Initialize(where, velocity * direction, acceleration * direction, lifetime, scale, rotationSpeed, bitmapIndex);
        }


        // PickRandomDirection is used by InitializeParticles to decide which direction
        // particles will move. The default implementation is a random vector on a circle.
        protected virtual Vector2 PickRandomDirection()
        {
            float angle = Common.RandomBetween(0, (float)Math.PI * 2);

            return new Vector2((float)Math.Cos(angle),
                               (float)Math.Sin(angle));
        }


        // Updates all of the active particles.
        public void Update(float elapsedTime)
        {
            // Go through the active particles in reverse order, so our loop
            // index stays valid even when we decide to remove some of them.
            for (int i = activeParticles.Count - 1; i >= 0; i--)
            {
                Particle particle = activeParticles[i];

                if (!particle.Update(elapsedTime))
                {
                    // If the particle is no longer active, move it from activeParticles to freeParticles.
                    activeParticles.RemoveAt(i);
                    freeParticles.Push(particle);
                }
            }
        }


        // Draws all of the active particles.
        public void Draw(CanvasDrawingSession drawingSession, bool useSpriteBatch)
        {
            var previousBlend = drawingSession.Blend;

            drawingSession.Blend = blendState;

            if (useSpriteBatch)
            {
                using (var spriteBatch = drawingSession.CreateSpriteBatch())
                {
                    Draw(drawingSession, spriteBatch);
                }
            }
            else
            {
                Draw(drawingSession, null);
            }

            drawingSession.Blend = previousBlend;
        }


        void Draw(CanvasDrawingSession drawingSession, CanvasSpriteBatch spriteBatch)
        {
            // Go through the particles in reverse order, so new ones are drawn underneath
            // older ones. This improves visual appearance of effects like smoke plume
            // where many particles are created at the same position over a period of time.
            for (int i = activeParticles.Count - 1; i >= 0; i--)
            {
                Particle particle = activeParticles[i];

                // Normalized lifetime is a value from 0 to 1 and represents how far a particle
                // is through its life. 0 means it just started, .5 is half way through, and
                // 1.0 means it's just about to be finished. This value will be used to calculate
                // alpha and scale, to avoid  having particles suddenly appear or disappear.
                float normalizedLifetime = particle.TimeSinceStart / particle.Lifetime;

                // We want particles to fade in and fade out, so we'll calculate alpha to be
                // (normalizedLifetime) * (1 - normalizedLifetime). This way, when normalizedLifetime
                // is 0 or 1, alpha is 0. The maximum value is at normalizedLifetime = .5, and is:
                //
                //      (normalizedLifetime) * (1-normalizedLifetime)
                //      (.5)                 * (1-.5)
                //      .25
                //
                // Since we want the maximum alpha to be 1, not .25, we'll scale the entire equation by 4.
                float alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);

                // Make particles grow as they age.
                // They'll start at 75% of their size, and increase to 100% once they're finished.
                float scale = particle.Scale * (.75f + .25f * normalizedLifetime);

                var bitmap = bitmaps[particle.BitmapIndex];

                if (spriteBatch != null)
                {
                    var rand = new Random();
                    float randX = (float)rand.NextDouble() * 0.5f + 0.5f;
                    float randY = (float)rand.NextDouble();
                    float randZ = (float)rand.NextDouble();

                    spriteBatch.Draw(bitmap, particle.Position, new Vector4(1, 1, randZ, alpha), 
                        bitmapCenter, particle.Rotation, new Vector2(scale), CanvasSpriteFlip.None);
                }
                else
                {
                    // Compute a transform matrix for this particle.
                    var transform = Matrix3x2.CreateRotation(particle.Rotation, bitmapCenter) *
                                    Matrix3x2.CreateScale(scale, bitmapCenter) *
                                    Matrix3x2.CreateTranslation(particle.Position - bitmapCenter);

                    // Draw the particle.
                    drawingSession.DrawImage(bitmap, 0, 0, bitmapBounds, alpha,
                        CanvasImageInterpolation.Linear, new Matrix4x4(transform));
                }
            }
        }
    }

    // Particles are the little bits that make up a particle system effect. Each particle
    // system is be comprised of many of these particles. They have basic physical properties,
    // such as position, velocity, acceleration, and rotation. They'll be drawn as sprites,
    // all layered on top of one another, and will be very pretty.
    public class Particle
    {
        // Position, Velocity, and Acceleration represent exactly what their names
        // indicate. They are public fields rather than properties so that users
        // can directly access their .X and .Y properties.
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        public int BitmapIndex;

        // How long this particle will live.
        public float Lifetime;

        // How long it has been since initialize was called.
        public float TimeSinceStart;

        // The scale of this particle.
        public float Scale;

        // Its rotation, in radians.
        public float Rotation;

        // How fast does it rotate?
        public float RotationSpeed;


        // Initialize is called by ParticleSystem to set up the particle, and prepares the particle for use.
        public void Initialize(Vector2 position, Vector2 velocity, Vector2 acceleration, float lifetime, float scale, float rotationSpeed, int bitmapIndex)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.Acceleration = acceleration;
            this.Lifetime = lifetime;
            this.Scale = scale;
            this.RotationSpeed = rotationSpeed;

            // Reset TimeSinceStart - we have to do this because particles will be reused.
            this.TimeSinceStart = 0.0f;

            // Set rotation to some random value between 0 and 360 degrees.
            this.Rotation = Common.RandomBetween(0, (float)Math.PI * 2);

            BitmapIndex = bitmapIndex;
        }


        // Update is called by the ParticleSystem on every frame.
        // This is where the particle's position and that kind of thing get updated.
        // Returns whether the particle is still alive.
        public bool Update(float elapsedTime)
        {
            Velocity += Acceleration * elapsedTime;
            Position += Velocity * elapsedTime;

            Rotation += RotationSpeed * elapsedTime;

            TimeSinceStart += elapsedTime;

            return TimeSinceStart < Lifetime;
        }
    }
}
