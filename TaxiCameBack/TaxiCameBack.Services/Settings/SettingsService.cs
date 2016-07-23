using System;
using System.Linq;
using TaxiCameBack.Core;
using TaxiCameBack.Services.Logging;

namespace TaxiCameBack.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly IRepository<Core.DomainModel.Settings.Settings> _settingsRepository;
        private readonly ILoggingService _loggingService;

        public SettingsService(IRepository<Core.DomainModel.Settings.Settings> settingsRepository, ILoggingService loggingService)
        {
            _settingsRepository = settingsRepository;
            _loggingService = loggingService;
        }

        public Core.DomainModel.Settings.Settings GetSettings()
        {
            return _settingsRepository.GetAll().FirstOrDefault();
        }

        public CrudResult Add(Core.DomainModel.Settings.Settings settings)
        {
            var result = new CrudResult();
            _settingsRepository.Insert(settings);
            try
            {
                _settingsRepository.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                _settingsRepository.UnitOfWork.Rollback();
                result.AddError(exception.Message);
                _loggingService.Error(exception);
            }
            return result;
        }

        public CrudResult Update(Core.DomainModel.Settings.Settings oldSettings, Core.DomainModel.Settings.Settings newSettings)
        {
            var result = new CrudResult();
            _settingsRepository.Merge(oldSettings, newSettings);
            try
            {
                _settingsRepository.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                _settingsRepository.UnitOfWork.Rollback();
                result.AddError(exception.Message);
                _loggingService.Error(exception);
            }
            return result;
        }

        public Core.DomainModel.Settings.Settings Get(int id)
        {
            return _settingsRepository.GetById(id);
        }
    }
}
