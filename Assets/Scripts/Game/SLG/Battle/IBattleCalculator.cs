using Game.Data;

namespace Game.SLG.Battle
{
    public interface IBattleCalculator
    {
        BattleData Calculate(BattleMsg msg);
    }
}