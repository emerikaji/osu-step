using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Step.Skinning.Default;
using osu.Game.Skinning;
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

        public override bool DisplayResult => Result?.Type == HitResult.ComboBreak;

        protected override StepSkinComponents Component => StepSkinComponents.MineNote;

        protected override Drawable CreateHeadPiece() => new SkinnableDrawable(new StepSkinComponentLookup(Component), _ => new DefaultMineNotePiece())
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y
        };

        protected override void OnApply()
        {
            base.OnApply();
            isKeyDown = false;
            Colour = Color4.White;
            AccentColour.Value = Color4.DarkRed;
        }

        protected override void Update()
        {
            base.Update();

            if (AllJudged)
                return;

            bool currentlyHeld = isKeyDown || (maniaInputManager?.KeyBindingContainer.PressedActions.Contains(Action.Value) ?? false);
            if (currentlyHeld)
                UpdateResult(true);
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (!userTriggered)
            {
                if (timeOffset > MineNote.MINE_HIT_WINDOW)
                    ApplyMaxResult();

                return;
            }

            if (Math.Abs(timeOffset) <= MineNote.MINE_HIT_WINDOW)
            {
                ApplyMinResult();
            }
        }

        public override bool OnPressed(KeyBindingPressEvent<ManiaAction> e)
        {
            if (e.Action != Action.Value)
                return false;

            isKeyDown = true;

            return UpdateResult(true);
        }

        public override void OnReleased(KeyBindingReleaseEvent<ManiaAction> e)
        {
            if (e.Action == Action.Value)
                isKeyDown = false;
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            if (state == ArmedState.Miss)
                base.UpdateHitStateTransforms(ArmedState.Hit);
            if (state == ArmedState.Hit)
                base.UpdateHitStateTransforms(ArmedState.Miss);
        }
    }
}
