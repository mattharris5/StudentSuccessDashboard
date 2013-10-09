using SSD.Domain;
using System;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class PrivateHealthFieldModel : CustomFieldModel
    {
        public int? SelectedProviderId { get; set; }

        public SelectList Providers { get; set; }

        public override void CopyTo(CustomField model)
        {
            base.CopyTo(model);
            PrivateHealthField validModel = model as PrivateHealthField;
            if (validModel == null)
            {
                throw new ArgumentException(string.Format("Model must be an instance of type {0}.", typeof(PrivateHealthField).Name), "model");
            }
            validModel.ProviderId = SelectedProviderId;
        }

        public override void CopyFrom(CustomField model)
        {
            base.CopyFrom(model);
            PrivateHealthField validModel = model as PrivateHealthField;
            if (validModel == null)
            {
                throw new ArgumentException(string.Format("Model must be an instance of type {0}.", typeof(PrivateHealthField).Name), "model");
            }
            SelectedProviderId = validModel.ProviderId;
        }
    }
}
