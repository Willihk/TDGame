using MessagePack;

namespace TDGame.Systems.Wave.Messages
{
    [MessagePackObject]
    public struct StoreWaveMessage
    {
        [Key(0)]
        public WaveData Data;
    }
    
    [MessagePackObject]
    public struct GenerateAndStoreWaveMessage
    {
        [Key(0)]
        public int WaveNumber;
    }


    [MessagePackObject]
    public struct StartWaveMessage
    {
    }

    [MessagePackObject]
    public struct ResetWaveMessage { }

    [MessagePackObject]
    public struct LoadWaveMessage
    {
        [Key(0)]
        public int WaveNumber;
    }
}