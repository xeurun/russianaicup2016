using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model
{
    public class StrategyStatePrepare : StrategyState, IStrategyState
    {
        public override void Run(Game game, World world, Wizard self, Move move)
        {
            StrategyStorage.prepareTick++;

            if (StrategyStorage.priorityLane == null)
            {
                switch (random.Next(3))
                {
                    case 0:
                        StrategyStorage.waitTimer = game.TickCount / 15;
                        StrategyStorage.priorityLane = LaneType.Bottom;
                        break;
                    case 1:
                        StrategyStorage.waitTimer = game.TickCount / 30;
                        StrategyStorage.priorityLane = LaneType.Middle;
                        break;
                    case 2:
                    default:
                        StrategyStorage.waitTimer = game.TickCount / 15;
                        StrategyStorage.priorityLane = LaneType.Top;
                        break;
                }

                // TODO: Пока ходим только по миду
                //StrategyStorage.priorityLane = LaneType.Middle;

                StrategyHelper.Log(String.Format("\tChosen line: {0}", StrategyStorage.priorityLane));
            }
            else
            {
                if (StrategyStorage.priorityLane == LaneType.Bottom)
                {
                    StrategyStorage.currentTurn = self.GetAngleTo(StrategyStorage.positionTop ? 0 : world.Width, self.Y);
                }
                else if (StrategyStorage.priorityLane == LaneType.Middle)
                {
                    StrategyStorage.currentTurn = self.GetAngleTo(StrategyStorage.mainBase.Y, StrategyStorage.mainBase.X);
                }
                else if (StrategyStorage.priorityLane == LaneType.Top)
                {
                    StrategyStorage.currentTurn = self.GetAngleTo(self.X, StrategyStorage.positionTop ? world.Height : 0);
                }
            }

            this.InitSpeed(game, world, self, move, 0.7);
            this.CheckCollision(game, world, self, move);
        }
    }
}