using System.Web.Mvc;
using System.Web.Routing;
using Rhino.Mocks;

namespace SSD
{
    public static class MockViewContextFactory
    {
        public static ViewContext Create(ViewDataDictionary viewData)
        {
            var mock = MockRepository.GenerateMock<ViewContext>();
            mock.Expect(v => v.HttpContext).Return(MockHttpContextFactory.Create());
            mock.Expect(v => v.Controller).Return(MockRepository.GenerateMock<ControllerBase>());
            mock.Expect(v => v.View).Return(MockRepository.GenerateMock<IView>());
            mock.Expect(v => v.ViewData).Return(viewData);
            mock.Expect(v => v.TempData).Return(new TempDataDictionary());
            mock.Expect(v => v.RouteData).Return(new RouteData());
            return mock;
        }
    }
}
