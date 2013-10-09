using SSD.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace SSD.ViewModels
{
    public class EulaModel : IStateCopier<EulaAgreement>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "EULA Text is required")]
        [DataType(DataType.MultilineText)]
        public string EulaText { get; set; }

        public AuditModel Audit { get; set; }

        public void CopyTo(EulaAgreement model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.EulaText = EulaText;
        }

        public void CopyFrom(EulaAgreement model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Id = model.Id;
            EulaText = model.EulaText;
            if (Audit == null)
            {
                Audit = new AuditModel();
            }
            Audit.CreatedBy = model.CreatingUser.DisplayName;
            Audit.CreateTime = model.CreateTime;
        }
    }
}
