// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Step.Configuration;
using osu.Game.Rulesets.Step.Skinning;
using osu.Game.Rulesets.Step.Skinning.Default;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Skinning;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Step.Objects.Drawables
{
    /// <summary>
    /// Visualises a <see cref="Note"/> hit object.
    /// </summary>
    public partial class DrawableNote : DrawableManiaHitObject<Note>, IKeyBindingHandler<ManiaAction>
    {
        [Resolved]
        private OsuColour colours { get; set; }

        [Resolved(canBeNull: true)]
        private IBeatmap beatmap { get; set; }

        protected virtual StepSkinComponents Component => StepSkinComponents.Note;

        protected Drawable HeadPiece;

        public DrawableNote()
            : this(null)
        {
        }

        public DrawableNote(Note hitObject)
            : base(hitObject)
        {
            AutoSizeAxes = Axes.Y;
        }

        protected virtual Drawable CreateHeadPiece() => new SkinnableDrawable(new StepSkinComponentLookup(Component), _ => new DefaultNotePiece())
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y
        };

        [BackgroundDependencyLoader(true)]
        private void load(ManiaRulesetConfigManager rulesetConfig)
        {
            AddInternal(HeadPiece = CreateHeadPiece());
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            StartTimeBindable.BindValueChanged(_ => updateSnapColour(), true);
        }

        protected override void OnApply()
        {
            base.OnApply();
            updateSnapColour();
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
            base.OnDirectionChanged(e);

            HeadPiece.Anchor = HeadPiece.Origin = e.NewValue == ScrollingDirection.Up ? Anchor.TopCentre : Anchor.BottomCentre;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyMinResult();

                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);

            if (result == HitResult.None)
                return;

            result = GetCappedResult(result);
            ApplyResult(result);
        }

        /// <summary>
        /// Some objects in mania may want to limit the max result.
        /// </summary>
        protected virtual HitResult GetCappedResult(HitResult result) => result;

        public virtual bool OnPressed(KeyBindingPressEvent<ManiaAction> e)
        {
            if (e.Action != Action.Value)
                return false;

            if (CheckHittable?.Invoke(this, Time.Current) == false)
                return false;

            return UpdateResult(true);
        }

        public virtual void OnReleased(KeyBindingReleaseEvent<ManiaAction> e)
        {
        }

        private void updateSnapColour()
        {
            if (this is DrawableHoldNoteTail || this is DrawableMineNote)
                return;

            Colour = Color4.White;

            if (beatmap == null || HitObject == null)
                return;

            int snapDivisor = beatmap.ControlPointInfo.GetClosestBeatDivisor(HitObject.StartTime);

            AccentColour.Value = CurrentSkin.GetConfig<BeatSnapColourLookup, Color4>(new BeatSnapColourLookup(snapDivisor))?.Value
                                 ?? getStepManiaSnapColour(snapDivisor);
        }

        private static Color4 getStepManiaSnapColour(int divisor) => divisor switch
        {
            1 => Color4.Red,
            2 => Color4.Blue,
            3 => Color4.Purple,
            4 => Color4.Yellow,
            6 => Color4.Pink,
            8 => Color4.Orange,
            12 or 16 => Color4.Cyan,
            _ => Color4.Green
        };
    }
}
