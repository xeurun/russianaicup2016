using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model
{
    public class StrategyStateGame : StrategyState, IStrategyState
    {
        public override void Run(Game game, World world, Wizard self, Move move)
        {
            this.InitSpeed(game, world, self, move);
            this.CheckCollision(game, world, self, move);
            this.CheckNeedAttack(game, world, self, move);
        }
    }
}