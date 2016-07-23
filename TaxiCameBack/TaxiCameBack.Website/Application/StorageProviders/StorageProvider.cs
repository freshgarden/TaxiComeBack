using System;

namespace TaxiCameBack.Website.Application.StorageProviders
{
    public static class StorageProvider
    {
        private static readonly Lazy<IStorageProvider> CurrentStorageProvider = new Lazy<IStorageProvider>(() =>
        {

            try
            {
                return new DiskStorageProvider();
            }
            catch (Exception)
            {
                return new DiskStorageProvider();
            }
        });

        public static IStorageProvider Current => CurrentStorageProvider.Value;
    }
}