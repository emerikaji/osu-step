using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Step.Objects.Drawables
{
    public partial class DrawableRollNote : DrawableHoldNote
    {
        private Container stripeContainer = null!;
        private float lastBuiltHeight;
        private ScrollingDirection lastDirection;

        public DrawableRollNote()
            : this(null)
        {
        }

        public DrawableRollNote(RollNote? hitObject)
            : base(hitObject)
        {
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            MaskingContainer.Add(stripeContainer = new Container
            {
                Name = "Roll stripes",
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Depth = -1,
            });
        }

        protected override void OnApply()
        {
            base.OnApply();
            lastBuiltHeight = -1;
        }

        protected override void Update()
        {
            base.Update();

            if (stripeContainer != null && DrawHeight > 0 && (!Precision.AlmostEquals(DrawHeight, lastBuiltHeight, 1f) || Direction.Value != lastDirection))
                rebuildStripes();
        }

        private void rebuildStripes()
        {
            lastBuiltHeight = DrawHeight;
            lastDirection = Direction.Value;
            stripeContainer.Clear();

            const float stripe_spacing = 24f;
            const float stripe_height = 6f;

            bool scrollingUp = Direction.Value == ScrollingDirection.Up;
            int count = Math.Clamp((int)Math.Ceiling(DrawHeight / stripe_spacing) + 4, 4, 250);

            for (int i = 0; i < count; i++)
            {
                stripeContainer.Add(new Box
                {
                    Anchor = scrollingUp ? Anchor.TopCentre : Anchor.BottomCentre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.X,
                    Width = 1.4f,
                    Height = stripe_height,
                    Y = scrollingUp ? i * stripe_spacing : -i * stripe_spacing,
                    Rotation = -20f,
                    Colour = Color4.White,
                    Alpha = 0.45f,
                });
            }
        }

        public override bool OnPressed(KeyBindingPressEvent<ManiaAction> e)
        {
            if (AllJudged || e.Action != Action.Value)
                return false;

            if (CheckHittable?.Invoke(this, Time.Current) == false)
                return false;

            if (Time.Current > Tail.HitObject.StartTime)
                return false;

            // Subsequent taps while rolling: reset the 250ms roll timer!
            if (Head.AllJudged)
            {
                if (!Head.IsHit || Body.HasHoldBreak)
                    return false;

                ReleaseTime = Time.Current;
                Head.PlaySamples();

                // Flash the stripes & pulse the head smoothly without disturbing DrawableRollNote's layout/height
                stripeContainer?.FadeTo(0.95f).FadeTo(0.45f, 120, Easing.OutQuint);
                Head.ScaleTo(1.15f).ScaleTo(1f, 100, Easing.OutQuint);
                return true;
            }

            // Initial tap: hit the head and start the first 250ms roll timer
            bool hit = Head.UpdateResult();
            if (Head.IsHit)
                ReleaseTime = Time.Current;

            return hit;
        }

        public override void OnReleased(KeyBindingReleaseEvent<ManiaAction> e)
        {
            // Releasing a key does not drop a roll; only failing to re-tap within the grace period drops it.
        }
    }
}
