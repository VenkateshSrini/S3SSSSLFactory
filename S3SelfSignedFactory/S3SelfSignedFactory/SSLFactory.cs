using System;
using System.Net;
using System.Net.Http;
using Amazon.Runtime;
namespace S3SelfSignedFactory
{
    public  class SSLFactory : HttpClientFactory
    {
        public override HttpClient CreateHttpClient(IClientConfig clientConfig)
        {
            var httpMessageHandler = CreateClientHandler();
            if (clientConfig.MaxConnectionsPerServer.HasValue)
                httpMessageHandler.MaxConnectionsPerServer = clientConfig.MaxConnectionsPerServer.Value;
            httpMessageHandler.AllowAutoRedirect = clientConfig.AllowAutoRedirect;

            // Disable automatic decompression when Content-Encoding header is present
            httpMessageHandler.AutomaticDecompression = DecompressionMethods.None;

            var proxy = clientConfig.GetWebProxy();
            if (proxy != null)
            {
                httpMessageHandler.Proxy = proxy;
            }

            if (httpMessageHandler.Proxy != null && clientConfig.ProxyCredentials != null)
            {
                httpMessageHandler.Proxy.Credentials = clientConfig.ProxyCredentials;
            }
            var httpClient = new HttpClient(httpMessageHandler);

            if (clientConfig.Timeout.HasValue)
            {
                // Timeout value is set to ClientConfig.MaxTimeout for S3 and Glacier.
                // Use default value (100 seconds) for other services.
                httpClient.Timeout = clientConfig.Timeout.Value;
            }
            return httpClient;
        }
        protected virtual HttpClientHandler CreateClientHandler()=>
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };
            

        
    }
}
