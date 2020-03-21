using Ernestoyaquello.TwoPlayerZeroSumGameEngine.Models;

namespace Ernestoyaquello.TwoPlayerZeroSumGameEngine
{
    public static class Extensions
    {
        public static Player ToOppositePlayer(this Player player)
        {
            return player switch
            {
                Player.First => Player.Second,
                Player.Second => Player.First,
                _ => Player.None,
            };
        }
    }
}
