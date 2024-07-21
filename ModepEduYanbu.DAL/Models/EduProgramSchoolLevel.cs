using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    /// <summary>
    /// Key: EduProgramId, SchoolLevelId
    /// </summary>
    public class EduProgramSchoolLevel
    {
        public string EduProgramId { get; set; }
        public EduProgram EduProgram { get; set; }

        public string SchoolLevelId { get; set; }
        public SchoolLevel SchoolLevel { get; set; }
    }
}
