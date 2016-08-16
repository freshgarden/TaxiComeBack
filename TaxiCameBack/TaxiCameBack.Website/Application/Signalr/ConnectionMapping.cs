using System;
using System.Collections.Generic;
using System.Linq;

namespace TaxiCameBack.Website.Application.Signalr
{
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, string> _connections =
            new Dictionary<T, string>();

        public int Count => _connections.Count;

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                if (!string.IsNullOrEmpty(connectionId))
                {
                    _connections.Add(key, connectionId);
                }
            }
        }

        public string GetConnections(T key)
        {
            lock (_connections)
            {
                string connections;
                if (_connections.TryGetValue(key, out connections))
                {
                    return connections;
                }
            }

            return string.Empty;
        }

        public void Remove(T key)
        {
            lock (_connections)
            {
                _connections.Remove(key);
            }
        }

        public void RemoveByValue(string value)
        {
            lock (_connections)
            {
                var item = _connections.FirstOrDefault(kvp => kvp.Value == value);
                if (!Equals(item.Key, default(T)))
                    _connections.Remove(item.Key);
            }
        }

        public IList<string> ToList()
        {
            IList<string> result = new List<string>();
            lock (_connections)
            {
                foreach (var connection in _connections)
                {
                    result.Add(connection.Value);
                }
            }
            return result;
        }
    }
}