﻿using Microsoft.Data.Sqlite;

namespace HnTrends.Caches
{
    using System;
    using Database;
    using Microsoft.Extensions.Caching.Memory;

    internal class StoryCountCache : IStoryCountCache
    {
        private static readonly object Lock = new object();

        private readonly IMemoryCache memoryCache;
        private readonly SqliteConnection connection;

        public StoryCountCache(IMemoryCache memoryCache, ICacheManager cacheManager, SqliteConnection connection)
        {
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));

            cacheManager.Register(nameof(StoryCountCache));
        }

        public int Get()
        {
            lock (Lock)
            {
                if (memoryCache.TryGetValue(nameof(StoryCountCache), out int value))
                {
                    return value;
                }

                value = StoryTable.GetCount(connection);

                memoryCache.Set(nameof(StoryCountCache), value);

                return value;
            }
        }
    }
}
