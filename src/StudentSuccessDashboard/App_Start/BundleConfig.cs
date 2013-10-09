using System.Web;
using System.Web.Optimization;

namespace SSD
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterStyleBundles(bundles);
            RegisterScriptBundles(bundles);
#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }

        private static void RegisterStyleBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/bundle").Include(
                        "~/Content/Main.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap/bundle").Include(
                        "~/Content/bootstrap/bootstrap-responsive.css",
                        "~/Content/bootstrap/bootstrap.css",
                        "~/Content/bootstrap/bsmselect.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/bundle").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css",
                        "~/Content/themes/base/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/FontAwesome/css/bundle").Include(
                        "~/Content/FontAwesome/css/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/DataTables/css/bundle").Include(
                        "~/Content/DataTables-1.9.4/media/css/jquery.dataTables.css",
                        "~/Content/DataTables-1.9.4/media/css/DT_bootstrap.css"));
        }

        private static void RegisterScriptBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/head/bundle").Include(
                        "~/Scripts/modernizr-2.6.2.js",
                        "~/Scripts/prefixfree.min.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Framework/bundle").Include(
                        "~/Scripts/jQuery/jquery-1.10.1.js",
                        "~/Scripts/jQuery/jquery-ui-1.10.3.js",
                        "~/Scripts/jQuery/jquery-migrate-1.1.1.js",
                        "~/Scripts/jQuery/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jQuery/jquery.validate.js",
                        "~/Scripts/jQuery/jquery.validate.unobtrusive.js",
                        "~/Scripts/jQuery/jquery.multiOpenAccordion.js",
                        "~/Scripts/jQuery/jquery.bsmselect.js",
                        "~/Scripts/jQuery/jquery.suggestion-text.js",
                        "~/Scripts/DataTables-1.9.4/media/js/jquery.dataTables.js",
                        "~/Scripts/DataTables-1.9.4/media/js/DT_bootstrap.js",
                        "~/Scripts/DataTables-1.9.4/media/js/ssd-datatables.js",
                        "~/Scripts/DataTables-1.9.4/extras/TableTools/media/js/ZeroClipboard.js",
                        "~/Scripts/DataTables-1.9.4-Overrides/TableTools.js",
                        "~/Scripts/Bootstrap/bootstrap.js",
                        "~/Scripts/Bootstrap/bootstrapx-clickover.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Views/bundle").Include(
                        "~/Scripts/Views/StringExtensions.js",
                        "~/Scripts/Views/Selector.js",
                        "~/Scripts/Views/DataTable.js",
                        "~/Scripts/Views/Filter.js",
                        "~/Scripts/Views/ssd.js",
                        "~/Scripts/Views/ManagePage.js",
                        "~/Scripts/Views/Account/Login.js",
                        "~/Scripts/Views/Account/UserProfile.js",
                        "~/Scripts/Views/Administrator/DistrictDataTable.js",
                        "~/Scripts/Views/Administrator/ManageDistricts.js",
                        "~/Scripts/Views/Partner/PartnerFilter.js",
                        "~/Scripts/Views/Partner/ProviderDataTable.js",
                        "~/Scripts/Views/Partner/ProgramDataTable.js",
                        "~/Scripts/Views/Partner/Manage.js",
                        "~/Scripts/Views/Service/ScheduleOfferingDataTable.js",
                        "~/Scripts/Views/Service/ScheduleOfferingFilter.js",
                        "~/Scripts/Views/Service/ScheduleOffering.js",
                        "~/Scripts/Views/ServiceOffering/ServiceOfferingFilter.js",
                        "~/Scripts/Views/ServiceOffering/ServiceOfferingDataTable.js",
                        "~/Scripts/Views/ServiceOffering/Manage.js",
                        "~/Scripts/Views/ServiceOffering/FileUploadComplete.js",
                        "~/Scripts/Views/ServiceType/ServiceTypeDataTable.js",
                        "~/Scripts/Views/ServiceType/ServiceTypeFilter.js",
                        "~/Scripts/Views/ServiceType/Manage.js",
                        "~/Scripts/Views/ManagePage.js",
                        "~/Scripts/Views/Student/Details.js",
                        "~/Scripts/Views/Student/StudentDataTable.js",
                        "~/Scripts/Views/Student/StudentFilter.js",
                        "~/Scripts/Views/Student/StudentSelector.js",
                        "~/Scripts/Views/Student/Finder.js",
                        "~/Scripts/Views/ServiceAttendance/ServiceAttendanceDataTable.js",
                        "~/Scripts/Views/ServiceAttendance/ServiceAttendance.js",
                        "~/Scripts/Views/User/UserFilter.js",
                        "~/Scripts/Views/User/UserSelector.js",
                        "~/Scripts/Views/User/UserDataTable.js",
                        "~/Scripts/Views/User/Manage.js",
                        "~/Scripts/Views/User/AccessAudit.js",
                        "~/Scripts/Views/User/AccessAuditDataTable.js",
                        "~/Scripts/Views/User/LoginAudit.js",
                        "~/Scripts/Views/User/LoginAuditDataTable.js",
                        "~/Scripts/Views/StudentApproval/StudentApprovalDataTable.js",
                        "~/Scripts/Views/StudentApproval/StudentApprovalFilter.js",
                        "~/Scripts/Views/StudentApproval/Manage.js",
                        "~/Scripts/Views/Data/UploadWizard.js",
                        "~/Scripts/Views/Data/UploadWizard2.js",
                        "~/Scripts/Views/Data/UploadWizard3.js",
                        "~/Scripts/Views/CustomField/PublicFieldDataTable.js",
                        "~/Scripts/Views/CustomField/ManagePublic.js",
                        "~/Scripts/Views/CustomField/PrivateHealthFieldDataTable.js",
                        "~/Scripts/Views/CustomField/ManagePrivateHealth.js",
                        "~/Scripts/Views/DataFile/StudentProfileExport.js"));
        }
    }
}