
namespace Assets.Scripts.Data
{
    public interface IDataConfigurationCache
    {
            void UpdateConfiguration(IDataConfiguration configuration);
            void DeleteConfiguration(string id);
            void SaveToDisk();
            void LoadFromDisk();
            IDataConfiguration GetConfigurationDataByID(string id);

    }
}
