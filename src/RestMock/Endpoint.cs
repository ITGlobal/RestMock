using System;
using System.Collections.Generic;
using System.Linq;

namespace RestMock
{
    internal sealed class Endpoint
    {
        private static readonly object SyncRoot = new object();
        private static readonly List<Endpoint> FreeEndpoints = new List<Endpoint>();
        private static readonly List<Endpoint> TakenEndpoints = new List<Endpoint>();
        private const int MinPort = 12000;

        private readonly int _port;

        private Endpoint(int port)
        {
            _port = port;
            Url = $"http://127.0.0.1:{port}";
            Uri = new Uri(Url, UriKind.Absolute);
        }

        
        public string Url { get; }
        public Uri Uri { get; }

        public static Endpoint GetEndpoint()
        {
            lock (SyncRoot)
            {
                Endpoint endpoint;

                if (FreeEndpoints.Count > 0)
                {
                    endpoint = FreeEndpoints[FreeEndpoints.Count - 1];
                    FreeEndpoints.Remove(endpoint);
                }
                else
                {
                    var port = MinPort;

                    while (true)
                    {
                        if (Enumerable.All(TakenEndpoints, _ => _._port != port))
                        {
                            break;
                        }

                        port++;
                    }

                    endpoint = new Endpoint(port);
                }

                TakenEndpoints.Add(endpoint);
                return endpoint;
            }
        }

        public static void ReleaseEndpoint(Endpoint endpoint)
        {
            lock (SyncRoot)
            {
                TakenEndpoints.Remove(endpoint);
                FreeEndpoints.Add(endpoint);
            }
        }
    }
}