using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model
{
    public static class StrategyStateManager
    {
        private enum StrategyStates
        {
            None,
            Prepare,
            Game
        }

        private static Dictionary<StrategyStates, IStrategyState> strategyStates = new Dictionary<StrategyStates, IStrategyState>(5)
        {
            { StrategyStates.None, new StrategyState() },
            { StrategyStates.Prepare, new StrategyStatePrepare() },
            { StrategyStates.Game, new StrategyStateGame() },
        };

        public static IStrategyState GetState(World world, Wizard self)
        {
            /* Основная логика игры */
            StrategyStates strategyState = StrategyStates.Game;

            if (self.Life == 0)
            {
                /* Жизней 0 - мертв */
                strategyState = StrategyStates.None;
            }
            else if (StrategyStorage.prepareTick < StrategyStorage.waitTimer)
            {
                strategyState = StrategyStates.Prepare;
            }
            else if (StrategyStorage.prevLife == 0 && self.Life > 0)
            {
                StrategyHelper.Log(String.Format("RESPAWN, tick {0}", world.TickIndex));
                int enemyCount = StrategyHelper.CountEnemyInCastRange(world, self);

                StrategyStorage.priorityLane = null;
                strategyState = enemyCount == 0 ? StrategyStates.Prepare : StrategyStates.Game;

                StrategyHelper.Log(String.Format("\tEnemyCount: {0}, work strategy: {1}", Enum.GetName(typeof(StrategyStates), strategyState)));
            }

            StrategyHelper.Log(String.Format("strategy {0}, tick: {1}", Enum.GetName(typeof(StrategyStates), strategyState).ToUpper(), world.TickIndex));

            return StrategyStateManager.strategyStates[strategyState];
        }
    }
}