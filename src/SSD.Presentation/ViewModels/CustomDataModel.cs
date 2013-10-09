using SSD.Domain;
using System;

namespace SSD.ViewModels
{
    public class CustomDataModel : IStateCopier<CustomFieldValue>
    {
        public string FieldName { get; set; }
        public string Value { get; set; }
        public DateTime DateCreated { get; set; }
        public string Source { get; set; }

        public void CopyTo(CustomFieldValue model)
        {
            throw new NotImplementedException();
        }

        public void CopyFrom(CustomFieldValue model)
        {
            FieldName = model.CustomField.Name;
            DateCreated = model.CustomDataOrigin.CreateTime;
            Source = model.CustomDataOrigin.Source;
            Value = model.Value;
        }
    }
}
