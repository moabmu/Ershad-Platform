using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Models
{
    public class SchoolLevel
    {
        public string SchoollevelId { get; set; }
        public string Name { get; set; }

        public List<School> Schools { get; set; }
        public List<EduProgramSchoolLevel> EduPrograms { get; set; }
    }
}

