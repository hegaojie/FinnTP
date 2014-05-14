namespace FinnTorget
{
    public interface IConfigManager
    {
        FinnConfig LoadConfiguration();
        void SaveConfiguration(FinnConfig config);
    }
}