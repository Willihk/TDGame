using Mirror;
using TDGame.Systems.Economy.Old;

namespace TDGame.Systems.Economy.Interfaces
{
    public interface IPlayerEconomyManager
    {
        [Server]
        public void AddCurrencyToAllPlayers(int amount);

        [Server]
        public void ReducesCurrencyForPlayer(int playerId, int amount);

        public NetworkedPlayerEconomy GetEconomy(NetworkConnection connection);
    }
}