using System;
using System.Collections.Generic;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model
{
    public class StrategyStorage
    {
        /* Делитель для вычисления задержки перед стартом логики */
        public const int TIMEOUTDELUMETER = 60;
        /* Делитель для вычисления задержки перед респауном */
        public const int RESPOWNTIMEOUTDELUMETER = TIMEOUTDELUMETER / 2;
        /* Делитель для вычисления безопасной дистанции */
        public const int PREPAREDEFAULTTICK = 1;
        /* Множитель скорости */
        public const double DEEFAULTMULTIPLER = .25f;
        /* Делитель для вычисления безопасной дистанции */
        public const double DISTANCEDELUMETER = 1.0f;

        /* Отладочные сообщения */
        public static bool debug = true;
        /* Убегаем */
        public static bool needRun = false;
        /* Спавн наверху карты */
        public static bool positionTop = false;
        /* Спавн наверху карты */
        public static long myId = 0;
        /* Жизней в предыдущем тике */
        public static int prevLife = 0;
        /* Половина жизней */
        public static int halfLife = 0;
        /* Время бездействия */
        public static int waitTimer = Int32.MaxValue;
        /* Текущий тик подготовки */
        public static int prepareTick = PREPAREDEFAULTTICK;
        /* Текущий стрейф */
        public static double currentStrafe = 0;
        /* Текущая скорость */
        public static double currentSpeed = 0;
        /* Текущий поворот */
        public static double currentTurn = 0;
        /* Дистанция проверки зоны активности */
        public static double visionRange = 0;
        /* Безопасное расстояние */
        public static double safeDistance = 0;
        /* Координаты дома X */
        public static double spawnPointX = 0;
        /* Координаты дома Y */
        public static double spawnPointY = 0;
        /* Выбранный лайн противника */
        public static LaneType? priorityLane;
        /* Фракция противника */
        public static Faction badGuyFaction;
        /* База */
        public static LivingUnit mainBase;
        /* Текущая цель */
        public static LivingUnit target;
        /* Объект коллизии */
        public static LivingUnit collisionObject;
        /* Ближайшие цели */
        public static List<LivingUnit> nearedTargets = new List<LivingUnit>();
        /* Ближайшие статические объекты */
        public static List<LivingUnit> nearedStaticObject = new List<LivingUnit>();

        public static void Init(Game game, World world, Wizard self)
        {
            myId = self.Id;
            waitTimer = game.TickCount / TIMEOUTDELUMETER;
            safeDistance = self.CastRange / DISTANCEDELUMETER;
            halfLife = self.MaxLife / 2;
            visionRange = self.VisionRange / 2;
            spawnPointX = self.X;
            spawnPointY = self.Y;
            mainBase = StrategyHelper.MainBase(world, self);
            prevLife = self.Life;
            positionTop = self.Y < (world.Height / 2);
            badGuyFaction = self.Faction == Faction.Academy ? Faction.Renegades : Faction.Academy;
        }
        public static void Tick(Game game, World world, Wizard self)
        {
            myId = self.Id;
            prevLife = self.Life;
            target = null;
            currentSpeed = 0;
            currentTurn = 0;
            currentStrafe = 0;
            needRun = false;
            StrategyStorage.collisionObject = null;
            StrategyStorage.target = null;
        }
    }
}