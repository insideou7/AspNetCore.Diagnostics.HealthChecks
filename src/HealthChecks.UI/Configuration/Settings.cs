using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;

namespace HealthChecks.UI.Configuration
{
    public class Settings
    {
        internal List<HealthCheckSetting> HealthChecks { get; set; } = new List<HealthCheckSetting>();
        internal List<WebHookNotification> Webhooks { get; set; } = new List<WebHookNotification>();
        internal int EvaluationTimeInSeconds { get; set; } = 10;
        internal int MinimumSecondsBetweenFailureNotifications { get; set; } = 60 * 10;
        internal string HealthCheckDatabaseConnectionString { get; set; }
        internal Func<IServiceProvider, HttpMessageHandler> ApiEndpointHttpHandler { get; private set; }
        internal Func<IServiceProvider, HttpMessageHandler> WebHooksEndpointHttpHandler { get; private set; }

        public Settings AddHealthCheckEndpoint(string name, string uri)
        {
            HealthChecks.Add(new HealthCheckSetting
            {
                Name = name,
                Uri = uri
            });

            return this;
        }
        
        public Settings AddWebhookNotification(string name, string uri, string payload, string restorePayload = "")
        {
            Webhooks.Add(new WebHookNotification
            {
                Name = name,
                Uri = uri,
                Payload = payload,
                RestoredPayload = restorePayload
            });
            return this;
        }

        public Settings SetEvaluationTimeInSeconds(int seconds)
        {
            EvaluationTimeInSeconds = seconds;
            return this;
        }
        
        public Settings SetMinimumSecondsBetweenFailureNotifications(int seconds)
        {
            MinimumSecondsBetweenFailureNotifications = seconds;
            return this;
        }

        public Settings SetHealthCheckDatabaseConnectionString(string connectionString)
        {
            HealthCheckDatabaseConnectionString = connectionString;
            return this;
        }

        public Settings UseApiEndpointHttpMessageHandler(Func<IServiceProvider, HttpClientHandler> apiEndpointHttpHandler)
        {
            ApiEndpointHttpHandler = apiEndpointHttpHandler;
            return this;
        }
        
        public Settings UseWebhookEndpointHttpMessageHandler(Func<IServiceProvider, HttpClientHandler> webhookEndpointHttpHandler)
        {
            WebHooksEndpointHttpHandler = webhookEndpointHttpHandler;
            return this;
        }
    }

    public class HealthCheckSetting
    {
        public string Name { get; set; }
        public string Uri { get; set; }
    }

    public class WebHookNotification
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public string Payload { get; set; }
        public string RestoredPayload { get; set; }

        public string GetPayload(string name, string failure = "", bool isHealthy = false, string description = null)
        {
            var payload = isHealthy ? RestoredPayload : Payload;
            payload = payload.Replace(Keys.LIVENESS_BOOKMARK, HttpUtility.JavaScriptStringEncode(name))
                .Replace(Keys.FAILURE_BOOKMARK, HttpUtility.JavaScriptStringEncode(failure))
                .Replace(Keys.DESCRIPTIONS_BOOKMARK, HttpUtility.JavaScriptStringEncode(description));

            return payload;
        }
    }
}
