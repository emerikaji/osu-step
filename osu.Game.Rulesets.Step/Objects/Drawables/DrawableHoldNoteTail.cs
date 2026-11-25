// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace osu.Game.Rulesets.Step.Objects.Drawables
{
    /// <summary>
    /// The tail of a <see cref="DrawableHoldNote"/>.
    /// </summary>
    public partial class DrawableHoldNoteTail : DrawableNote
    {
        /// <summary>
        /// The time at which the user starting missing the hold note.
        /// This could be the time at which they missed the head, broke on the body, or missed the tail.
        /// </summary>
        public readonly IBindable<double?> MissingStartTime = new Bindable<double?>();

        protected override StepSkinComponents Component => StepSkinComponents.HoldNoteTail;

        protected internal DrawableHoldNote HoldNote => (DrawableHoldNote)ParentHitObject;

        public DrawableHoldNoteTail()
            : this(null)
        {
        }

        public DrawableHoldNoteTail(TailNote tailNote)
            : base(tailNote)
        {
            Anchor = Anchor.TopCentre;
            Origin = Anchor.TopCentre;
        }

        protected override void OnApply()
        {
            base.OnApply();

            if (ParentHitObject is DrawableHoldNote parentHold)
                MissingStartTime.BindTo(parentHold.MissingStartTime);
        }

        protected override void OnFree()
        {
            base.OnFree();

            if (ParentHitObject is DrawableHoldNote parentHold)
                MissingStartTime.UnbindFrom(parentHold.MissingStartTime);
        }

        public void UpdateResult() => base.UpdateResult(true);

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            // If the hold body broke (grace period expired or head missed), immediately judge as NG
            if (HoldNote.Body.HasHoldBreak)
            {
                ApplyMinResult();
                return;
            }

            // Wait until the tail's timestamp is reached
            if (timeOffset < 0)
                return;

            // Auto-complete with max judgement if the head was hit and the hold wasn't dropped past the grace period
            if (HoldNote.Head.IsHit && !HoldNote.Body.HasHoldBreak)
                ApplyMaxResult();
            else
                ApplyMinResult();
        }

        public override bool OnPressed(KeyBindingPressEvent<ManiaAction> e) => false; // Handled by the hold note

        public override void OnReleased(KeyBindingReleaseEvent<ManiaAction> e)
        {
        }
    }
}
