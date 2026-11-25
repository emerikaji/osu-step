// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Configuration;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Step.Judgements;

namespace osu.Game.Rulesets.Step.Mods
{
    public class StepModPerfect : ModPerfect
    {
        [SettingSource("Require perfect hits")]
        public BindableBool RequirePerfectHits { get; } = new BindableBool();

        protected override bool FailCondition(HealthProcessor healthProcessor, JudgementResult result)
        {
            if (result.Judgement is TailNoteJudgement && !result.IsHit)
                return true;

            if (result.Judgement is MineNoteJudgement && !result.IsHit)
                return true;

            if (!isRelevantResult(result.Judgement.MinResult) && !isRelevantResult(result.Judgement.MaxResult) && !isRelevantResult(result.Type))
                return false;

            // Mania allows imperfect "Great" hits without failing.
            if (result.Judgement.MaxResult == HitResult.Perfect && !RequirePerfectHits.Value)
                return result.Type < HitResult.Great;

            return result.Type != result.Judgement.MaxResult;
        }

        private bool isRelevantResult(HitResult result) => result.AffectsAccuracy() || result.AffectsCombo();
    }
}
