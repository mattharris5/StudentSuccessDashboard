using EFCachingProvider.Caching;
using Microsoft.ApplicationServer.Caching;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SSD.Data
{
    public class AppFabricCache : ICache
    {
        private static readonly string CacheClientName = CloudConfigurationManager.GetSetting("CacheClientName");
        private static readonly Lazy<DataCacheFactory> _Factory = new Lazy<DataCacheFactory>(() => 
            {
                DataCacheFactoryConfiguration config = new DataCacheFactoryConfiguration(CacheClientName);
                return new DataCacheFactory(config);
            });

        private DataCache _Cache;

        public AppFabricCache(string cacheName)
        {
            _Cache = Factory.GetCache(cacheName);
        }

        public static DataCacheFactory Factory
        {
            get { return _Factory.Value; }
        }

        public bool GetItem(string key, out object value)
        {
            key = GetCacheKey(key);
            value = _Cache.Get(key);

            return value != null;
        }

        public void PutItem(string key, object value, IEnumerable<string> dependentEntitySets, TimeSpan slidingExpiration, DateTime absoluteExpiration)
        {
            key = GetCacheKey(key);
            _Cache.Put(key, value, absoluteExpiration - DateTime.Now, dependentEntitySets.Select(c => new DataCacheTag(c)).ToList());

            foreach (var dep in dependentEntitySets)
            {
                CreateRegionIfNeeded(dep);
                _Cache.Put(key, " ", dep);
            }
        }

        public void InvalidateSets(IEnumerable<string> entitySets)
        {
            foreach (var dep in entitySets)
            {
                foreach (var val in _Cache.GetObjectsInRegion(dep))
                {
                    _Cache.Remove(val.Key);
                }
            }
        }

        public void InvalidateItem(string key)
        {
            key = GetCacheKey(key);

            DataCacheItem item = _Cache.GetCacheItem(key);
            _Cache.Remove(key);

            foreach (var tag in item.Tags)
            {
                _Cache.Remove(key, tag.ToString());
            }
        }

        private static string GetCacheKey(string query)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(query);
            string hashString = Convert.ToBase64String(MD5.Create().ComputeHash(bytes));
            return hashString;
        }

        private void CreateRegionIfNeeded(string regionName)
        {
            try
            {
                _Cache.CreateRegion(regionName);
            }
            catch (DataCacheException de)
            {
                if (de.ErrorCode != DataCacheErrorCode.RegionAlreadyExists)
                {
                    throw;
                }
            }
        }
    }
}