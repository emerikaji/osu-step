using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Step.Judgements;

public class TailNoteJudgement : Judgement
{
    public override HitResult MaxResult => HitResult.LargeBonus;
    public override HitResult MinResult => HitResult.IgnoreMiss;
}
