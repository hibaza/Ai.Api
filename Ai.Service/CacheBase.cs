using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Collections;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;

namespace Ai.Service
{
    public class CacheBase
    {
        public static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        public static object lockobj = new object();
        
        public static void SetItem(string key, object obj, DateTime cacheTime)
        {
            try
            {
                lock (lockobj)
                {
                    _cache.Set(key, obj, cacheTime);
                }
            }
            catch (Exception ex)
            { }
        }
        public static object GetItem(string key)
        {            
            try
            {
                lock (lockobj)
                {
                    object objs ;
                    _cache.TryGetValue(key, out objs);
                        return objs;
                    
                }
            }
            catch (Exception ex)
            { }
            return null;
        }

    }

}
