// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Step.Judgements;

namespace osu.Game.Rulesets.Step.Mods
{
    public class StepModSuddenDeath : ModSuddenDeath
    {
        protected override bool FailCondition(HealthProcessor healthProcessor, JudgementResult result)
        {
            if (result.Judgement is TailNoteJudgement && !result.IsHit)
                return true;

            if (result.Judgement is MineNoteJudgement && !result.IsHit)
                return true;

            return base.FailCondition(healthProcessor, result);
        }
    }
}
