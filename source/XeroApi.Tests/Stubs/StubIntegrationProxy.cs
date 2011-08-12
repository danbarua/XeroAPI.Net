﻿using System;
using System.Text;
using XeroApi.Integration;
using XeroApi.Linq;

namespace XeroApi.Tests.Stubs
{
    internal class StubIntegrationProxy : IIntegrationProxy
    {
        public string FindElements(ApiQueryDescription apiQueryDescription)
        {
            LastQueryDescription = apiQueryDescription;
            return GenerateSampleResponseXml(apiQueryDescription.ElementName);
        }

        public byte[] FindOne(string endpointName, string itemId, string acceptMimeType)
        {
            return Encoding.UTF8.GetBytes(GenerateSampleResponseXml(endpointName));
        }

        public string GetElement(string endpointName, string itemId)
        {
            return GenerateSampleResponseXml(endpointName);
        }

        public string UpdateOrCreateElements(string endpointName, string body)
        {
            return GenerateSampleResponseXml(endpointName);
        }

        public string CreateElements(string endpointName, string body)
        {
            return GenerateSampleResponseXml(endpointName);
        }

        public ApiQueryDescription LastQueryDescription
        {
            get;
            private set;
        }

        private static string GenerateSampleResponseXml(string elementName)
        {
            return "<Response><Id>" + Guid.NewGuid() + "</Id><Status>OK</Status><ProviderName>NullIntegrationProxy</ProviderName><DateTimeUTC>" + DateTime.UtcNow.ToString("s") + "</DateTimeUTC><" + elementName + "s" + " /></Response>";
        }
    }

}
