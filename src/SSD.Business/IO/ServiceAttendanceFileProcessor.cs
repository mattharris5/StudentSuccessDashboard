using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace SSD.IO
{
    public class ServiceAttendanceFileProcessor : BaseFileProcessor
    {
        private static Subject _DefaultSubjectCache;

        public ServiceAttendanceFileProcessor(IBlobClient blobClient, IRepositoryContainer repositories)
            : base(blobClient, repositories)
        { }

        protected override void ProcessDataTable(EducationSecurityPrincipal user, DataTable dataTable, ServiceUploadModel model)
        {
            string studentSISId, subjectName;
            Subject subject = new Subject();
            int duration;
            DateTime? dateAttended;
            Student student;

            for (int i = StartRow; i < dataTable.Rows.Count; i++)
            {
                studentSISId = dataTable.Rows[i][1].ToString();
                subjectName = dataTable.Rows[i][3].ToString();
                student = StudentRepository.Items.Where(s => s.StudentSISId == studentSISId).SingleOrDefault();
                subject = RetrieveSubject(subjectName);
                if (IsValidServiceOffering(user, i, studentSISId, student, dataTable, out dateAttended, out duration, model, subject))
                {
                    string notes = dataTable.Rows[i][5].ToString();
                    var studentAssignedOffering = StudentAssignedOfferingRepository.Items.Where(s => s.StudentId == student.Id && s.ServiceOfferingId == model.ServiceOfferingId).FirstOrDefault();
                    if (studentAssignedOffering == null)
                    {
                        studentAssignedOffering = CreateNewStudentAssignedOffering(student, model, user);
                    }
                    Create(studentAssignedOffering, (DateTime)dateAttended, subject, duration, notes, user);
                    model.SuccessfulRowsCount++;
                }
                model.ProcessedRowCount++;
            }
        }

        private Subject RetrieveSubject(string subjectName)
        {
            if (subjectName == string.Empty)
            {
                if (_DefaultSubjectCache == null)
                {
                    _DefaultSubjectCache = SubjectRepository.Items.Where(s => s.Name.Equals(Subject.DefaultName)).FirstOrDefault();
                }
                return _DefaultSubjectCache;
            }
            else
            {
                return SubjectRepository.Items.Where(s => s.Name.Equals(subjectName)).FirstOrDefault();
            }
        }

        private bool IsValidServiceOffering(EducationSecurityPrincipal user, int index, string studentSISId, Student student, DataTable dataTable, out DateTime? dateAttended, out int duration, ServiceUploadModel model, Subject subject)
        {
            dateAttended = null;
            duration = 0;
            string subjectName = dataTable.Rows[index][3].ToString();
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
            else if (!ExcelUtility.TryGetOADate(dataTable.Rows[index][2].ToString(), out dateAttended))
            {
                ProcessError(dataTable.Rows[index], string.Format(CultureInfo.CurrentCulture, "Malformed Date Attended on row {0}", index + 1), model);
                return false;
            }
            else if (subject == null)
            {
                ProcessError(dataTable.Rows[index], string.Format(CultureInfo.CurrentCulture, "Malformed Subject on row {0}", index + 1), model);
                return false;
            }
            else if (dataTable.Rows[index][4].ToString() != string.Empty && !int.TryParse(dataTable.Rows[index][4].ToString(), out duration))
            {
                ProcessError(dataTable.Rows[index], string.Format(CultureInfo.CurrentCulture, "Malformed Duration on row {0}", index + 1), model);
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
            rowOfErrors.Add(row[5].ToString());
            model.RowErrorValues.Add(new FileRowModel
            {
                RowErrors = rowOfErrors
            });
        }

        private void Create(StudentAssignedOffering studentAssignedOffering, DateTime dateAttended, Subject subject, decimal duration, string notes, EducationSecurityPrincipal user)
        {
            var newAttendance = new ServiceAttendance();
            newAttendance.StudentAssignedOffering = studentAssignedOffering;
            newAttendance.DateAttended = dateAttended;
            newAttendance.Subject = subject;
            newAttendance.Duration = duration;
            newAttendance.Notes = notes;
            newAttendance.CreatingUser = user.Identity.User;
            ServiceAttendanceRepository.Add(newAttendance);
        }

        private StudentAssignedOffering CreateNewStudentAssignedOffering(Student student, ServiceUploadModel model, EducationSecurityPrincipal user)
        {
            var newOffering = new StudentAssignedOffering();
            newOffering.Student = student;
            newOffering.StudentId = student.Id;
            newOffering.CreatingUser = user.Identity.User;
            newOffering.ServiceOfferingId = model.ServiceOfferingId;
            newOffering.StartDate = DateTime.Now;
            newOffering.CreateTime = DateTime.Now;
            newOffering.IsActive = true;
            StudentAssignedOfferingRepository.Add(newOffering);
            return newOffering;
        }
    }
}
