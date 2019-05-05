using System;
using System.Net;

namespace BeerO.SpotifyApi
{
    public class ProxyConfig
    {
        public string Host { get; set; }

        public int Port { get; set; } = 80;

        public string Username { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Whether to bypass the proxy server for local addresses.
        /// </summary>
        public bool BypassProxyOnLocal { get; set; }

        public void Set(ProxyConfig proxyConfig)
        {
            this.Host = proxyConfig?.Host;
            this.Port = proxyConfig?.Port ?? 80;
            this.Username = proxyConfig?.Username;
            this.Password = proxyConfig?.Password;
            this.BypassProxyOnLocal = proxyConfig?.BypassProxyOnLocal ?? false;
        }

        /// <summary>
        /// Whether both <see cref="Host"/> and <see cref="Port"/> have valid values.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(this.Host) && this.Port > 0;
        }

        /// <summary>
        /// Create a <see cref="Uri"/> from the host and port number
        /// </summary>
        /// <returns>A URI</returns>
        public Uri GetUri()
        {
            UriBuilder uriBuilder = new UriBuilder(this.Host)
            {
                Port = this.Port
            };
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Creates a <see cref="WebProxy"/> from the proxy details of this object.
        /// </summary>
        /// <returns>A <see cref="WebProxy"/> or <code>null</code> if the proxy details are invalid.</returns>
        public WebProxy CreateWebProxy()
        {
            if (!this.IsValid())
                return null;

            WebProxy proxy = new WebProxy
            {
                Address = this.GetUri(),
                UseDefaultCredentials = true,
                BypassProxyOnLocal = this.BypassProxyOnLocal
            };

            if (string.IsNullOrEmpty(this.Username) || string.IsNullOrEmpty(this.Password)) 
                return proxy;
            
            proxy.UseDefaultCredentials = false;
            proxy.Credentials = new NetworkCredential(this.Username, this.Password);

            return proxy;
        }
    }
}