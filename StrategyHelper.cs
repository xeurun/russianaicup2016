using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model
{
    public class StrategyHelper
    {
        const int NEEDKILLBUILDFRIEND = 3;

        enum UnitWeight
        {
            FriendMinion = 1,
            FriendBuilding = 2,
            FriendWizard = 3,
            FriendBase = 3,
            EnemyMinion = 1,
            EnemyBuilding = 2,
            EnemyWizard = 3,
        }

        public static List<LivingUnit> ActiveUnits(World world)
        {
            List<LivingUnit> activeUnits = new List<LivingUnit>();

            activeUnits.AddRange(world.Wizards);
            activeUnits.AddRange(world.Minions);
            activeUnits.AddRange(world.Buildings);

            return activeUnits;
        }

        public static bool CheckBadSituation(Wizard self, World world)
        {
            int enemyCount = 0;
            int friendCount = 0;

            foreach (var unit in ActiveUnits(world))
            {
                double distance = self.GetDistanceTo(unit);
                if (distance < StrategyStorage.visionRange)
                {
                    if (unit.Faction == StrategyStorage.badGuyFaction)
                    {
                        enemyCount += unit is Minion ? (int)UnitWeight.EnemyMinion : unit is Building ? (int)UnitWeight.EnemyBuilding : (int)UnitWeight.EnemyWizard;
                    }
                    else if (unit.Faction == self.Faction)
                    {
                        if(unit.Id == StrategyStorage.mainBase.Id)
                        {
                            friendCount += (int)UnitWeight.FriendBase;
                        }

                        friendCount += unit is Minion ? (int)UnitWeight.FriendMinion : unit is Building ? (int)UnitWeight.FriendBuilding : (int)UnitWeight.FriendWizard;
                    }
                }
            }

            return (enemyCount > friendCount) || (StrategyStorage.target is Building && friendCount <= NEEDKILLBUILDFRIEND);
        }

        public static LivingUnit MainBase(World world, Wizard self)
        {
            foreach (var building in world.Buildings)
            {
                if (building.Type == BuildingType.FactionBase)
                {
                    return building;
                }
            }

            return null;
        }

        public static Bonus NearedBonus(World world, Wizard self)
        {
            Bonus target = null;
            double minDistance = double.MaxValue;
            foreach (var bonus in world.Bonuses)
            {
                double distance = self.GetDistanceTo(bonus);
                if (distance < (self.VisionRange * 3) && distance < minDistance)
                {
                    minDistance = distance;
                    target = bonus;
                }
            }

            return target;
        }

        public static LivingUnit NearedTarget(Game game, World world, Wizard self)
        {
            StrategyStorage.nearedTargets.Clear();
            StrategyStorage.nearedTargets = ActiveUnits(world);

            LivingUnit player = null;
            LivingUnit target = null;
            double playerDistance = double.MaxValue;
            double minDistance = double.MaxValue;
            foreach (var neardTarget in StrategyStorage.nearedTargets)
            {
                double distance = self.GetDistanceTo(neardTarget);

                if (neardTarget.Faction == StrategyStorage.badGuyFaction)
                {
                    if (neardTarget is Wizard && distance < self.CastRange && distance < playerDistance)
                    {
                        playerDistance = distance;
                        player = neardTarget;
                    }

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        target = neardTarget;
                    }
                }
            }

            return player != null ? player : target;
        }

        public static int CountEnemyInCastRange(World world, Wizard self)
        {
            int count = 0;
            
            List<LivingUnit> units = new List<LivingUnit>();
            units.AddRange(world.Minions);
            units.AddRange(world.Wizards);

            foreach (var unit in units)
            {
                double distance = self.GetDistanceTo(unit);
                if (unit.Faction == StrategyStorage.badGuyFaction && distance < self.CastRange)
                {
                    count++;
                }
            }

            return count;
        }
        
        public static LivingUnit CollizedObject(Wizard self, World world)
        {
            StrategyStorage.nearedStaticObject.Clear();
            StrategyStorage.nearedStaticObject = ActiveUnits(world);
            StrategyStorage.nearedStaticObject.AddRange(world.Trees);

            LivingUnit collizedObject = null;
            LivingUnit collizedTree = null;
            double minDistanceToTree = double.MaxValue;
            double minDistanceToObject = double.MaxValue;

            foreach (var neardTarget in StrategyStorage.nearedStaticObject)
            {
                double distance = self.GetDistanceTo(neardTarget) - neardTarget.Radius - self.Radius;
                if (neardTarget.Id != StrategyStorage.myId && distance < self.Radius)
                {
                    if(neardTarget is Tree && distance < minDistanceToTree)
                    {
                        minDistanceToTree = distance;
                        collizedTree = neardTarget;

                    }
                    else if(distance < minDistanceToObject)
                    {
                        minDistanceToObject = distance;
                        collizedObject = neardTarget;
                    }
                }
            }

            return collizedTree != null ? collizedTree : collizedObject;
        }
        
        public static LivingUnit CheckBlockAttack(World world, Wizard self)
        {
            foreach (var unit in ActiveUnits(world))
            {
                if ((unit.Faction == self.Faction || unit.Faction == Faction.Neutral) && unit.Id != StrategyStorage.myId && CheckLineCircleIntersections(unit.X, unit.Y, unit.Radius, self.X, self.Y, StrategyStorage.target.X, StrategyStorage.target.Y))
                {
                      return unit;
                }
            }

            return null;
        }

        public static bool CheckLineCircleIntersections(double cx, double cy, double radius, double px, double py, double pex, double pey)
        {
            double dx, dy, A, B, C, det;

            dx = pex - px;
            dy = pey - py;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (px - cx) + dy * (py - cy));
            C = (px - cx) * (px - cx) + (py - cy) * (py - cy) - radius * radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                //intersection1 = new PointF(float.NaN, float.NaN);
                //intersection2 = new PointF(float.NaN, float.NaN);
                return false;
            }

            return true;

            /*
            if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);
                intersection1 = new PointF(point1.X + t * dx, point1.Y + t * dy);
                intersection2 = new PointF(float.NaN, float.NaN);
                return true;
            }
            else
            {
                // Two solutions.
                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                intersection1 = new PointF(point1.X + t * dx, point1.Y + t * dy);
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                intersection2 = new PointF(point1.X + t * dx, point1.Y + t * dy);
                return true;
            }
            */
        }

        public static void Log(string message)
        {
            if (StrategyStorage.debug)
            {
                Console.WriteLine(message);
            }
        }
    }
}