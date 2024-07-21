using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class Report
    {
        public string ReportId { get; set; }
        public string ReportNo { get; set; }
        public string TargetedSlice { get; set; }
        public int TargetedCount { get; set; }
        public string Field { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public int ExecutionPeriod { get; set; }
        public string ExecutionData { get; set; }
        public double ParticipantsRatio { get; set; }
        public string ChallengesSolus { get; set; }
        public DateTime? SentDateTime { get; set; }
        public double Evaluation { get; set; }
        public DateTime? EvaluationDate { get;set; }
        public string UploadsLink { get; set; }
        public bool AllowEdit { get; set; } = true;
        public bool IsSignedByPrinciple { get; set; } = false;
        public bool IsEvaluated { get; set; } = false;
        public List<ReportUploadedFile> Uploads { get; set; }
        public List<ReportResponse> Responses { get; set; }

        //new elements
        public int ProceduresCount { get; set; }
        public string ProceduresSuggestions { get; set; }
    

        #region Visitor Activities
        public List<ReportActivity> Activities { get; set; }
        public int VisitorOverallRating { get; set; } = 0;
        public int VisitorRatingCount { get; set; } = 0;
        #endregion

        // Archived information.
        // TODO: Enable soft-delete in the system in order to get rid of archived string properties.

        #region School archived information
        public string SchoolId { get; set; }
        [ForeignKey("SchoolId")]
        public School School { get; set; }
        public string SchoolName { get; set; }
        public string SchoolMinistryNo { get; set; }
        #endregion

        #region EduProgram archived information
        public string EduProgramId { get; set; }
        [ForeignKey("EduProgramId")]
        public EduProgram EduProgram { get; set; }
        public string EduProgramName { get; set; }
        /// <summary>
        /// It is not neccesary to make one-to-many relationship between EducationalYear and
        /// Reports since report already has a relationship with EduProgam which already
        /// has a relationship with EducationalYear (In other words: Report >> EduProgram >> EducationalYear),
        /// but we need to archive all informations about each report inside of it, so that is why
        /// we created this relationship between Report and Educational Year. In addition, this will help
        /// to improve performance when query reports and their educational years.
        /// </summary>
        [ForeignKey(nameof(EducationalYearId))]
        public EducationalYear EducationalYear { get; set; }
        public string EducationalYearId { get; set; }
        public string EducationalYearName { get; set; }
        public string EducationalYearShortName { get; set; }
        #endregion

        #region Owner archived information
        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }
        public string OwnerFullName { get; set; }
        public string OwnerIdNo { get; set; }
        #endregion

        #region Signing Principle archived information
        public string SigningPrincipleId { get; set;}
        [ForeignKey("SigningPrincipleId")]
        public ApplicationUser SigningPrinciple { get; set; }
        public string SigningPrincipleFullName { get; set; }
        public string SigningPrincipleIdNo { get; set; }
        public DateTime? SigningDateTime { get; set; }
        #endregion

        #region Evaluator archived information
        public string EvaluatorId { get; set; }
        public ApplicationUser Evaluator { get; set; }
        public string EvaluatorFullName { get; set; }
        public string EvaluatorIdNo { get; set; }
        #endregion
    }
}
