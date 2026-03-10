
namespace Assets.Scripts.Data
{
    public interface IDataConfigurationCache
    {
            void UpdateConfiguration(IDataConfiguration configuration);
            void SaveToDisk();
            void LoadFromDisk();
    }
}
