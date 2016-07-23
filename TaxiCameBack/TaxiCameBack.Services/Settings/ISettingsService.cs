namespace TaxiCameBack.Services.Settings
{
    public interface ISettingsService
    {
        Core.DomainModel.Settings.Settings GetSettings();
        CrudResult Add(Core.DomainModel.Settings.Settings settings);
        CrudResult Update(Core.DomainModel.Settings.Settings oldSettings, Core.DomainModel.Settings.Settings newSettings);
        Core.DomainModel.Settings.Settings Get(int id);
    }
}
