using System.Web.Mvc;
using System.Web.Routing;
using Rhino.Mocks;

namespace SSD
{
    public static class MockHtmlHelperFactory
    {
        public static HtmlHelper Create(ViewDataDictionary viewData, RouteCollection routes)
        {
            var mockViewDataContainer = MockRepository.GenerateMock<IViewDataContainer>();
            mockViewDataContainer.Expect(v => v.ViewData).Return(viewData);
            return new HtmlHelper(MockViewContextFactory.Create(viewData), mockViewDataContainer, routes);
        }
    }
}
