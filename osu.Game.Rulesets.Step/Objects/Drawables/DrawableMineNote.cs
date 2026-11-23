using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Rulesets.Scoring;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Step.Objects.Drawables
{
    public partial class DrawableMineNote : DrawableNote
    {
        private bool isKeyDown;

        [Resolved]
        private ManiaInputManager? maniaInputManager { get; set; }

        public DrawableMineNote()
            : this(null)
        {
        }

        public DrawableMineNote(MineNote? hitObject)
            : base(hitObject)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            // Overlay a clear "X" warning symbol on top of the skinned note piece
            AddInternal(new SpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = "X",
                Font = OsuFont.Default.With(size: 32, weight: FontWeight.Bold),
                Colour = Color4.White,
            });
        }

        public override bool DisplayResult => Result?.Type == HitResult.ComboBreak;

        protected override void OnApply()
        {
            base.OnApply();
            isKeyDown = false;
            Colour = new Color4(220, 40, 40, 255); // Red bomb tint (overrides snap colour)
        }

        protected override void Update()
        {
            base.Update();

            // Keep bomb red even if snap-colour bindables fire
            Colour = new Color4(220, 40, 40, 255);

            if (AllJudged)
                return;

            double timeOffset = Time.Current - HitObject.StartTime;
            bool currentlyHeld = isKeyDown || (maniaInputManager?.KeyBindingContainer.PressedActions.Contains(Action.Value) ?? false);

            if (currentlyHeld && Math.Abs(timeOffset) <= MineNote.MINE_HIT_WINDOW)
            {
                ApplyMinResult();
                return;
            }

            if (timeOffset > MineNote.MINE_HIT_WINDOW)
                ApplyMaxResult();
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            // Handled directly in Update() and OnPressed() using MineNote.MINE_HIT_WINDOW
        }

        public override bool OnPressed(KeyBindingPressEvent<ManiaAction> e)
        {
            if (e.Action != Action.Value)
                return false;

            isKeyDown = true;

            if (!AllJudged && Math.Abs(Time.Current - HitObject.StartTime) <= MineNote.MINE_HIT_WINDOW)
            {
                ApplyMinResult();
                return true;
            }

            return false;
        }

        public override void OnReleased(KeyBindingReleaseEvent<ManiaAction> e)
        {
            if (e.Action == Action.Value)
                isKeyDown = false;
        }
    }
}
