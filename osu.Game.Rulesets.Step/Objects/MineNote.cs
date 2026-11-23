using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Step.Judgements;

namespace osu.Game.Rulesets.Step.Objects
{
    public class MineNote : Note
    {
        public const double MINE_HIT_WINDOW = 75;

        public override Judgement CreateJudgement() => new MineNoteJudgement();

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}
