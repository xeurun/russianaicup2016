using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model
{
    public class StrategyState : IStrategyState
    {
        /* Генератор */
        public Random random = new Random(DateTime.Now.Millisecond);

        public virtual void Tick(Game game, World world, Wizard self, Move move)
        {
            if (world.TickIndex == 0)
            {
                StrategyStorage.Init(game, world, self);
            }
            else
            {
                StrategyStorage.Tick(game, world, self);
            }

            Bonus bonus = StrategyHelper.NearedBonus(world, self);
            if (bonus != null)
            {
                StrategyStorage.target = null;
                StrategyStorage.currentTurn = self.GetAngleTo(bonus);
            }
            else
            {
                StrategyStorage.target = StrategyHelper.NearedTarget(game, world, self);
            }

            if (self.SpeedX == 0 && self.SpeedY == 0 && (StrategyStorage.target == null || (self.GetDistanceTo(StrategyStorage.target) > self.CastRange)))
            {
                StrategyStorage.collisionObject = StrategyHelper.CollizedObject(self, world);
            }

            this.Run(game, world, self, move);

            if (StrategyStorage.target != null && (StrategyStorage.collisionObject == null || (self.GetDistanceTo(StrategyStorage.target) > self.CastRange)))
            {
                StrategyStorage.currentTurn = self.GetAngleTo(StrategyStorage.target);
            }

            if (StrategyStorage.collisionObject != null && (StrategyStorage.target == null || (self.GetDistanceTo(StrategyStorage.target) > self.CastRange)))
            {
                StrategyStorage.currentTurn = self.GetAngleTo(StrategyStorage.collisionObject);
            }

            if (StrategyStorage.needRun)
            {
                StrategyStorage.currentStrafe = 0;
            }

            move.StrafeSpeed = StrategyStorage.currentStrafe;
            if (move.StrafeSpeed == 0)
            {
                move.Speed = StrategyStorage.currentSpeed;
                move.Turn = StrategyStorage.currentTurn;
            }
        }

        public virtual void Run(Game game, World world, Wizard self, Move move)
        {

        }

        public virtual void InitSpeed(Game game, World world, Wizard self, Move move, double multiple = 1)
        {
            /* 
             * Если нужно отступать врубаем обратную максимальную скорость, иначе если нет цели или дистанция
             * больше безопасной движемся вперед, иначе отступаем
             */
            if (self.Life < StrategyStorage.halfLife || StrategyHelper.CheckBadSituation(self, world))
            {
                StrategyStorage.needRun = true;
                StrategyStorage.currentSpeed = -(game.WizardBackwardSpeed * multiple);
            }
            else if (!(StrategyStorage.target is LivingUnit) || (self.GetDistanceTo(StrategyStorage.target) > (StrategyStorage.safeDistance - StrategyStorage.target.Radius - game.MagicMissileRadius)))
            {
                StrategyStorage.currentSpeed = game.WizardForwardSpeed * multiple;
            }
            else
            {
                StrategyStorage.currentSpeed = -(game.WizardBackwardSpeed * multiple);
            }
        }

        public virtual void CheckCollision(Game game, World world, Wizard self, Move move)
        {
            if (StrategyStorage.collisionObject != null)
            {
                /* Если мы "воткнулись", но не можем разрушить, пытаемся обойти */
                if (StrategyStorage.collisionObject is Tree)
                {
                    if (self.GetDistanceTo(StrategyStorage.collisionObject) < (game.StaffRange - StrategyStorage.collisionObject.Radius))
                    {
                        move.Action = ActionType.Staff;
                    }
                    else
                    {
                        move.Action = ActionType.MagicMissile;
                        move.CastAngle = self.GetAngleTo(StrategyStorage.collisionObject);
                    }

                    StrategyHelper.Log("\tCollise with tree");
                }
                else if (StrategyStorage.collisionObject is Building)
                {
                    double angle = Math.Round(self.GetAngleTo(StrategyStorage.collisionObject));
                    StrategyStorage.currentStrafe = (angle >= 0 ? -1 : 1) * game.WizardStrafeSpeed;
                    StrategyHelper.Log("\tCollise with build");
                }
                else
                {
                    double angle = Math.Round(self.GetAngleTo(StrategyStorage.collisionObject));
                    StrategyStorage.currentStrafe = (angle >= 0 ? -1 : 1) * game.WizardStrafeSpeed;
                    //StrategyStorage.currentTurn = (random.Next(2) > 0 ? 2 : -1) * self.GetAngleTo(StrategyStorage.collisionObject);
                    StrategyHelper.Log("\tCollise with other unit");
                }
            }
        }

        public virtual void CheckNeedAttack(Game game, World world, Wizard self, Move move)
        {
            if (StrategyStorage.target != null)
            {
                double distance = self.GetDistanceTo(StrategyStorage.target);
                if (distance <= self.CastRange)
                {
                    double angle = self.GetAngleTo(StrategyStorage.target);
                    if (Math.Abs(angle) < (game.StaffSector / 2.0D))
                    {
                        if (distance < (game.StaffRange - StrategyStorage.target.Radius))
                        {
                            move.Action = ActionType.Staff;
                            StrategyHelper.Log("\tStaff attack");
                        }
                        else
                        {
                            LivingUnit blockUnit = StrategyHelper.CheckBlockAttack(world, self);
                            if (blockUnit != null)
                            {
                                StrategyStorage.currentStrafe = (Math.Round(self.GetAngleTo(blockUnit)) >= 0 ? -1 : 1) * game.WizardStrafeSpeed;
                            }
                            else
                            {
                                StrategyStorage.currentStrafe = (random.Next(2) > 0 ? 1 : -1) * game.WizardStrafeSpeed;
                            }

                            move.Action = ActionType.MagicMissile;
                            move.CastAngle = angle;
                            move.MinCastDistance = distance - StrategyStorage.target.Radius + game.MagicMissileRadius;
                            StrategyHelper.Log("\tMagic attack");
                        }
                    }
                }
            }
        }
    }
}