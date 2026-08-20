using System.Text.Json;

namespace HamperMarket.Data
{
    /// <summary>
    /// A tiny, dependency-free "database": each store keeps its records in memory
    /// and persists them to a JSON file under App_Data. No SQL Server, no SQLite,
    /// no external packages - just the file system.
    /// </summary>
    public class JsonStore<T>
    {
        private readonly string _filePath;
        private readonly object _lock = new();
        private List<T> _items = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public JsonStore(string fileName, IWebHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, fileName);
            Load();
        }

        private void Load()
        {
            lock (_lock)
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _items = JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new List<T>();
                        return;
                    }
                }
                _items = new List<T>();
            }
        }

        private void Save()
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(_items, JsonOptions);
                File.WriteAllText(_filePath, json);
            }
        }

        public List<T> GetAll()
        {
            lock (_lock)
            {
                return new List<T>(_items);
            }
        }

        public void Add(T item)
        {
            lock (_lock)
            {
                _items.Add(item);
                Save();
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            lock (_lock)
            {
                _items.AddRange(items);
                Save();
            }
        }

        public void Update(Func<T, bool> predicate, Action<T> updateAction)
        {
            lock (_lock)
            {
                var item = _items.FirstOrDefault(predicate);
                if (item != null)
                {
                    updateAction(item);
                    Save();
                }
            }
        }

        public void Remove(Func<T, bool> predicate)
        {
            lock (_lock)
            {
                _items.RemoveAll(x => predicate(x));
                Save();
            }
        }

        public bool IsEmpty()
        {
            lock (_lock)
            {
                return _items.Count == 0;
            }
        }
    }
}
