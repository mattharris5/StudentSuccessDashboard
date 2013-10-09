using SSD.ActionFilters;
using SSD.Business;
using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Web.Mvc;

namespace SSD.Controllers
{
    [AuthenticateAndAuthorize(Roles = "Data Admin, Site Coordinator, Provider")]
    [RequireHttps]
    public class PublicController : CustomFieldController<PublicFieldModel>
    {
        public PublicController(ICustomFieldManager logicManager)
            : base(logicManager)
        { }

        [HttpPost, ActionName("UploadWizard")]
        public override ViewResult UploadWizardConfirmed(UploadWizardFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var page2Model = LogicManager.GenerateMapFieldsViewModel(model, typeof(PublicField), (EducationSecurityPrincipal)User);
                return View("UploadWizard2", page2Model);
            }
            return View(model);
        }

        protected override IClientDataTable<CustomField> CreateClientDataTable()
        {
            return new PublicFieldClientDataTable(Request);
        }
    }
}