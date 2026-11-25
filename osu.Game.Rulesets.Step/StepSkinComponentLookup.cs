// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Skinning;

namespace osu.Game.Rulesets.Step
{
    public class StepSkinComponentLookup : SkinComponentLookup<StepSkinComponents>
    {
        /// <summary>
        /// Creates a new <see cref="StepSkinComponentLookup"/>.
        /// </summary>
        /// <param name="component">The component.</param>
        public StepSkinComponentLookup(StepSkinComponents component)
            : base(component)
        {
        }
    }

    public enum StepSkinComponents
    {
        ColumnBackground,
        HitTarget,
        KeyArea,
        Note,
        MineNote,
        HoldNoteHead,
        HoldNoteTail,
        HoldNoteBody,
        HitExplosion,
        StageBackground,
        StageForeground,
        BarLine
    }
}
