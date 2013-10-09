using System;
using System.Web;
using Rhino.Mocks;
using System.Collections.Specialized;
using System.Security.Principal;

namespace SSD
{
    public static class MockHttpContextFactory
    {
        public static HttpContextBase Create(string targetUrl = null, string httpMethod = "GET", string applicationPath = "/")
        {
            var context = MockRepository.GenerateMock<HttpContextBase>();
            var session = MockRepository.GenerateMock<HttpSessionStateBase>();
            var server = MockRepository.GenerateMock<HttpServerUtilityBase>();

            context.Expect(c => c.Request).Return(CreateRequest(targetUrl, httpMethod, applicationPath));
            context.Expect(c => c.Response).Return(CreateResponse());
            context.Expect(c => c.Session).Return(session);
            context.Expect(c => c.Server).Return(server);
            return context;
        }

        public static HttpRequestBase CreateRequest(string targetUrl = null, string httpMethod = "GET", string applicationPath = "/")
        {
            var request = MockRepository.GenerateMock<HttpRequestBase>();

            //These next two lines are required for the routing to generate valid URLs, apparently: 
            request.Expect(r => r.ApplicationPath).Return(applicationPath);
            request.Expect(r => r.AppRelativeCurrentExecutionFilePath).Return(targetUrl);
            request.Expect(r => r.HttpMethod).Return(httpMethod);
            request.Expect(r => r.Headers).Return(new NameValueCollection());
            return request;
        }

        public static HttpResponseBase CreateResponse()
        {
            var response = MockRepository.GenerateMock<HttpResponseBase>();
            response.Expect(r => r.ApplyAppPathModifier(null)).IgnoreArguments().Do(new Func<string, string>(r => r));
            return response;
        }
    }
}
