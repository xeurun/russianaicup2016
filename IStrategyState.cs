using Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWizards2016.DevKit.CSharpCgdk {
    public interface IStrategyState
    {
        void Tick(Game game, World world, Wizard self, Move move);
        void Run(Game game, World world, Wizard self, Move move);
    }
}