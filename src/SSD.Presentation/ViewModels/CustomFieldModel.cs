using SSD.DataAnnotations;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public abstract class CustomFieldModel : IStateCopier<CustomField>
    {
        protected CustomFieldModel()
        {
            SelectedCategories = new List<int>();
        }

        public int Id { get; set; }

        [StringLength(255)]
        [Required(ErrorMessage = "Field Name is required")]
        public string FieldName { get; set; }

        [Required(ErrorMessage = "Field Type is required")]
        public int SelectedFieldTypeId { get; set; }
        public SelectList FieldTypes { get; set; }

        [RequiredElements]
        public IEnumerable<int> SelectedCategories { get; set; }
        public MultiSelectList Categories { get; set; }

        public AuditModel Audit { get; set; }

        public virtual void CopyTo(CustomField model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.Name = FieldName;
            model.CustomFieldTypeId = SelectedFieldTypeId;
        }

        public virtual void CopyFrom(CustomField model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Id = model.Id;
            FieldName = model.Name;
            SelectedCategories = model.Categories.Select(c => c.Id);
            SelectedFieldTypeId = model.CustomFieldTypeId;
            if (Audit == null)
            {
                Audit = new AuditModel();
            }
            Audit.CreatedBy = model.CreatingUser.DisplayName;
            Audit.CreateTime = model.CreateTime;
            if (model.LastModifyingUser != null)
            {
                Audit.LastModifiedBy = model.LastModifyingUser.DisplayName;
                Audit.LastModifyTime = model.LastModifyTime;
            }
            else
            {
                Audit.LastModifiedBy = null;
                Audit.LastModifyTime = null;
            }
        }
    }
}
