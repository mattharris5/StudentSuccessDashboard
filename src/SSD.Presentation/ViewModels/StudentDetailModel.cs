using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.ViewModels
{
    public class StudentDetailModel : IStateCopier<Student>
    {
        public int Id { get; set; }
        public string SISId { get; set; }
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int Grade { get; set; }
        public string SchoolName { get; set; }
        public string Parents { get; set; }
        public bool OnlyUploadedCustomField { get; set; }
        public IEnumerable<ServiceRequest> ServiceRequests { get; set; }
        public IEnumerable<StudentAssignedOffering> StudentAssignedOfferings { get; set; }
        public IEnumerable<Class> Classes { get; set; }
        public IEnumerable<CustomDataModel> CustomData { get; set; }
        public IEnumerable<CustomDataModel> CustomPrivateData { get; set; }

        public StudentDetailModel()
        {
            ServiceRequests = new List<ServiceRequest>();
            StudentAssignedOfferings = new List<StudentAssignedOffering>();
            Classes = new List<Class>();
            CustomData = new List<CustomDataModel>();
        }

        public void CopyTo(Student model)
        {
            throw new NotSupportedException("Student records cannot be modified.");
        }

        public void CopyFrom(Student model)
        {
            Id = model.Id;
            SISId = model.StudentSISId;
            Name = model.FullName;
            DateOfBirth = model.DateOfBirth;
            Grade = model.Grade;
            SchoolName = model.School.Name;
            Parents = model.Parents;
            ServiceRequests = model.ServiceRequests;
            StudentAssignedOfferings = model.StudentAssignedOfferings.Where(s => s.IsActive).ToList();
            Classes = model.Classes;
            CustomData = CreateCustomDataViewModelList(model);
            CustomPrivateData = CreateCustomPrivateDataViewModelList(model);
        }

        private static List<CustomDataModel> CreateCustomDataViewModelList(Student model)
        {
            var customData = new List<CustomDataModel>();
            var customFieldValues = model.CustomFieldValues.OrderBy(c => c.CustomField.Name).Where(c => c.CustomField.GetType() == typeof(PublicField));
            foreach (var customField in customFieldValues)
            {
                CustomDataModel customFieldViewModel = new CustomDataModel();
                customFieldViewModel.CopyFrom(customField);
                customData.Add(customFieldViewModel);
            }
            return customData;
        }

        private static List<CustomDataModel> CreateCustomPrivateDataViewModelList(Student model)
        {
            var customData = new List<CustomDataModel>();
            var customFieldValues = model.CustomFieldValues.OrderBy(c => c.CustomField.Name).Where(c => c.CustomField.GetType() == typeof(PrivateHealthField));
            foreach (var customField in customFieldValues)
            {
                CustomDataModel customFieldViewModel = new CustomDataModel();
                customFieldViewModel.CopyFrom(customField);
                customData.Add(customFieldViewModel);
            }
            return customData;
        }
    }
}
