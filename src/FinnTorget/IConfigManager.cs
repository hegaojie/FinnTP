namespace FinnTorget
{
    public interface IConfigManager
    {
        FinnConfig LoadSettings();
        void SaveSettings(FinnConfig config);
    }
}