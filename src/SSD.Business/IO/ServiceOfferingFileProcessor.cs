using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace SSD.IO
{
    public class ServiceOfferingFileProcessor : BaseFileProcessor
    {
        public ServiceOfferingFileProcessor(IBlobClient blobClient, IRepositoryContainer repositories)
            : base(blobClient, repositories)
        { }

        protected override void ProcessDataTable(EducationSecurityPrincipal user, DataTable dataTable, ServiceUploadModel model)
        {
            string studentSISId, notes;
            DateTime? startDate, endDate;
            Student student;
            for (int i = StartRow; i < dataTable.Rows.Count; i++)
            {
                studentSISId = dataTable.Rows[i][1].ToString();
                student = StudentRepository.Items.Where(s => s.StudentSISId == studentSISId).SingleOrDefault();
                if (IsValidServiceOffering(user, i, studentSISId, student, dataTable, out startDate, out endDate, model))
                {
                    notes = dataTable.Rows[i][4].ToString();
                    CreateNewStudentAssignedOffering(student, model, startDate, endDate, notes, user);
                    model.SuccessfulRowsCount++;
                }
                model.ProcessedRowCount++;
            }
        }

        private bool IsValidServiceOffering(EducationSecurityPrincipal user, int index, string studentSISId, Student student, DataTable dataTable, out DateTime? startDate, out DateTime? endDate, ServiceUploadModel model)
        {
            startDate = endDate = null;
            if (studentSISId == null || student == null)
            {
                ProcessError(dataTable.Rows[index], string.Format(CultureInfo.CurrentCulture, "Malformed Student Id on row {0}", index + 1), model);
                return false;
            }
            else if (!GrantUserAccessToSchedulingAnOffering(user, student))
            {
                ProcessError(dataTable.Rows[index], string.Format(CultureInfo.CurrentCulture, "You do not have permission to interact with the referenced student on row {0}", index + 1), model);
                return false;
            }
            else if (!ExcelUtility.TryGetOADate(dataTable.Rows[index][2].ToString(), out startDate))
            {
                ProcessError(dataTable.Rows[index], string.Format(CultureInfo.CurrentCulture, "Malformed Start Date on row {0}", index + 1), model);
                return false;
            }
            else if (!ExcelUtility.TryGetOADate(dataTable.Rows[index][3].ToString(), out endDate))
            {
                ProcessError(dataTable.Rows[index], string.Format(CultureInfo.CurrentCulture, "Malformed End Date on row {0}", index + 1), model);
                return false;
            }
            return true;
        }

        protected override void CreateFileUploadErrorRows(DataRow row, ServiceUploadModel model)
        {
            List<string> rowOfErrors = new List<string>();
            rowOfErrors.Add(row[1].ToString());
            rowOfErrors.Add(row[2].ToString());
            rowOfErrors.Add(row[3].ToString());
            rowOfErrors.Add(row[4].ToString());
            model.RowErrorValues.Add(new FileRowModel
            {
                RowErrors = rowOfErrors
            });
        }

        private void CreateNewStudentAssignedOffering(Student student, ServiceUploadModel model, DateTime? startDate, DateTime? endDate, string notes, EducationSecurityPrincipal user)
        {
            var newOffering = new StudentAssignedOffering()
            {
                StudentId = student.Id,
                CreatingUser = user.Identity.User,
                ServiceOfferingId = model.ServiceOfferingId,
                StartDate = startDate,
                EndDate = endDate,
                Notes = notes,
                IsActive = true
            };
            StudentAssignedOfferingRepository.Add(newOffering);
        }
    }
}
