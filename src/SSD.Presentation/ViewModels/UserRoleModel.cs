using SSD.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class UserRoleModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        public IEnumerable<int> UserRoleIds { get; set; }

        [StringLength(255)]
        public string Comments { get; set; }

        public IEnumerable<int> PostedRoles { get; set; }
        public IEnumerable<Role> SelectedRoles { get; set; }
        public IEnumerable<Role> AvailableRoles { get; set; }

        public bool allSchoolsSelected { get; set; }
        public IEnumerable<int> SelectedSchoolIds { get; set; }
        public MultiSelectList Schools { get; set; }

        public IEnumerable<int> SelectedProviderIds { get; set; }
        public MultiSelectList Providers { get; set; }

        public void CopyFrom(User model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            UserId = model.Id;
            Comments = model.Comments;
            SelectedSchoolIds = model.UserRoles.SelectMany(u => u.Schools).Select(s => s.Id);
            SelectedProviderIds = model.UserRoles.SelectMany(u => u.Providers).Select(p => p.Id);
            Name = string.Format(CultureInfo.CurrentCulture, "{0}, {1}", model.LastName, model.FirstName);
        }
    }
}
