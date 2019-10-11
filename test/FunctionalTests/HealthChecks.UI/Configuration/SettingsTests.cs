using System.IO;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using FunctionalTests.Base;
using HealthChecks.UI;
using HealthChecks.UI.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;

namespace FunctionalTests.HealthChecks.UI.Configuration
{
    public class Settings_should
    {
        private WebHookNotification CreateNotification()
        {
            return new WebHookNotification()
            {
                Name = "ExampleName",
                Uri = "http://example.org",
                Payload = @"{
                    'group': '[[LIVENESS]]',
	                'failure': '[[FAILURE]]',
	                'description': '[[DESCRIPTIONS]]'
                    }",
                RestoredPayload = @"{
                    'group': '[[LIVENESS]]',
	                'failure': '[[FAILURE]]',
	                'description': '[[DESCRIPTIONS]]'
                    }"
            };
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void webhooknotification_should_json_escape_payload(bool isHealthy)
        {
            var notification = CreateNotification();
            string name = "na'me";
            string failure = "fail'ure";
            string description = "descr'iption";

            var payload = notification.GetPayload(name, failure, isHealthy, description);
            var decodedPayload = JsonConvert.DeserializeObject<WebHookPayload>(payload);

            decodedPayload.group.Should().Be(name);
            decodedPayload.failure.Should().Be(failure);
            decodedPayload.description.Should().Be(description);
        }

        public class WebHookPayload
        {
            public string group { get; set; }
            public string failure { get; set; }
            public string description { get; set; }
        }
    }
}
