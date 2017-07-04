using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.HttpOverrides.Internal;

namespace RestMock
{
    internal sealed class Endpoint
    {
        private static readonly object SyncRoot = new object();
        private static readonly List<Endpoint> FreeEndpoints = new List<Endpoint>();
        private static readonly List<Endpoint> TakenEndpoints = new List<Endpoint>();
        private const int MinPort = 12000;

        private readonly int _port;
        private readonly bool _isCustom;

        private Endpoint(int port) 
            : this("127.0.0.1", port, false)
        { }

        private Endpoint(string address, int port, bool isCustom)
        {
            _port = port;
            _isCustom = isCustom;
            Url = $"http://{address}:{port}";
            Uri = new Uri(Url, UriKind.Absolute);
        }

        public string Url { get; }
        public Uri Uri { get; }

        public static Endpoint CreateEndpoint(string endpoint)
        {
            if (!IPEndPointParser.TryParse(endpoint, out var ep))
            {
                throw new ArgumentException("Malformed endpoint", nameof(endpoint));
            }
            
            return new Endpoint(ep.Address.ToString(), ep.Port, true);
        }

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
                        if (TakenEndpoints.All(_ => _._port != port))
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
            if (endpoint._isCustom)
            {
                return;
            }

            lock (SyncRoot)
            {
                TakenEndpoints.Remove(endpoint);
                FreeEndpoints.Add(endpoint);
            }
        }
    }
}