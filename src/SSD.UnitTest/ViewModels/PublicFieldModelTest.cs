using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SSD.ViewModels
{
    [TestClass]
    public class PublicFieldModelTest
    {
        [TestMethod]
        public void GivenModelHasAuditData_WhenCopyFrom_ThenModelStateSet()
        {
            CustomField expectedState = new PublicField
            {
                CreateTime = new DateTime(2005, 4, 30),
                CreatingUser = new User { DisplayName = "fredBob" },
                LastModifyTime = new DateTime(2010, 5, 13),
                LastModifyingUser = new User { DisplayName = "jimGeorge" }
            };
            CustomFieldModel target = new PublicFieldModel();

            target.CopyFrom(expectedState);

            AuditModel actualState = target.Audit;
            Assert.AreEqual(expectedState.CreateTime, actualState.CreateTime);
            Assert.AreEqual(expectedState.CreatingUser.DisplayName, actualState.CreatedBy);
            Assert.AreEqual(expectedState.LastModifyTime, actualState.LastModifyTime);
            Assert.AreEqual(expectedState.LastModifyingUser.DisplayName, actualState.LastModifiedBy);
        }

        [TestMethod]
        public void GivenModelNotModified_WhenCopyFrom_ThenModelStatelastModifyValuesNull()
        {
            CustomField expectedState = new PublicField
            {
                CreateTime = new DateTime(2005, 4, 30),
                CreatingUser = new User { DisplayName = "fredBob" }
            };
            CustomFieldModel target = new PublicFieldModel();

            target.CopyFrom(expectedState);

            Assert.IsNull(target.Audit.LastModifiedBy);
            Assert.IsFalse(target.Audit.LastModifyTime.HasValue);
        }

        [TestMethod]
        public void GivenModelNotModified_AndViewModelAuditDataAlreadySet_WhenCopyFrom_ThenModelStatelastModifyValuesNull()
        {
            CustomField expectedState = new PublicField
            {
                CreateTime = new DateTime(2005, 4, 30),
                CreatingUser = new User { DisplayName = "fredBob" }
            };
            CustomFieldModel target = new PublicFieldModel();
            target.Audit = new AuditModel { LastModifiedBy = "bob", LastModifyTime = DateTime.Now };

            target.CopyFrom(expectedState);

            Assert.IsNull(target.Audit.LastModifiedBy);
            Assert.IsFalse(target.Audit.LastModifyTime.HasValue);
        }
    }
}
