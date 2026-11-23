using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Step.Judgements
{
    public class MineNoteJudgement : ManiaJudgement
    {
        public override HitResult MaxResult => HitResult.IgnoreHit;
        public override HitResult MinResult => HitResult.ComboBreak;
    }
}
