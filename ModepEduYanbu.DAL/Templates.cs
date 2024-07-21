using ModepEduYanbu.DAL.Models;
using ModepEduYanbu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Data
{
    public static class Templates
    {
        private const string SYSOWNER_ROLE_DESC = "مالك النظام";

        public const string mentorRoleUp = "mentor,principle,eduemployee,admin,sysowner";
        public const string principleRoleUp = "principle,eduemployee,admin,sysowner";
        public const string eduemployeeRoleUp = "eduemployee,admin,sysowner";
        public const string eduemployeeRoleUpExceptSysowner = "eduemployee,admin";
        public const string adminRoleUp = "admin,sysowner";

        public enum SchoolLevels { Primary = 10, Middle = 12, Secondary = 14}
        public enum SchoolTypes { Gov = 20, National = 22, International = 24}

        /// <summary>
        /// 0:sysowner, 1:admin, 2:eduemployee, 3:principle, 4:mentor.
        /// </summary>
        public static List<ApplicationRole> Roles { get; } = new List<ApplicationRole> {
            new ApplicationRole
            {
                CreatedDate = DateTime.UtcNow,
                Name = "sysowner",
                Description = SYSOWNER_ROLE_DESC,
                DescriptionAsTitle = SYSOWNER_ROLE_DESC,
            },
            new ApplicationRole
            {
                CreatedDate = DateTime.UtcNow,
                Name = "admin",
                Description = "مدير",
                DescriptionAsTitle = "المدير"
            },
            new ApplicationRole
            {
                CreatedDate = DateTime.UtcNow,
                Name = "eduemployee",
                Description = "موظف إداري",
                DescriptionAsTitle = "الموظف بقسم التوجيه والإرشاد"
            },
            new ApplicationRole
            {
                CreatedDate = DateTime.UtcNow,
                Name = "principle",
                Description = "قائد مدرسة",
                DescriptionAsTitle = "قائد المدرسة",
            },
            new ApplicationRole
            {
                CreatedDate = DateTime.UtcNow,
                Name = "mentor",
                Description = "مرشد طلابي",
                DescriptionAsTitle = "المرشد الطلابي"
            }
        };

        /// <summary>
        /// 0:sysowner, 1:admin, 2:eduemployee, 3:principle, 4:mentor.
        /// </summary>
        public static IEnumerable<string> RolesToStringObjectsAsEnumerable()
        {
            yield return Roles[0].Name;
            yield return Roles[1].Name;
            yield return Roles[2].Name;
            yield return Roles[3].Name;
            yield return Roles[4].Name;
        }

        public static IEnumerable<string> EduEmployeeRolesUpToStringObjectsAsEnumerable()
        {
            yield return Roles[0].Name;
            yield return Roles[1].Name;
            yield return Roles[2].Name;
        }

        public static IEnumerable<string> SchoolEmployeeRolesToStringObjectsAsEnumerable()
        {
            yield return Roles[3].Name;
            yield return Roles[4].Name;
        }

        public static List<KpiGradeForBootstrap> KpiGradesForBootstrap = new List<KpiGradeForBootstrap>
        {
            new KpiGradeForBootstrap
            {
                Order = 1,
                GradeTitle = "ممتاز",
                CssColor = "text-success",
                HighestValue = 1.0m,
                LowestValueInclusive = 0.9m
            },
            new KpiGradeForBootstrap
            {
                Order = 2,
                GradeTitle = "جيد جداً",
                CssColor = "text-primary",
                HighestValue = 0.9m,
                LowestValueInclusive = 0.8m
            },
            new KpiGradeForBootstrap
            {
                Order = 3,
                GradeTitle = "جيد",
                CssColor = "text-warning",
                HighestValue = 0.8m,
                LowestValueInclusive = 0.6m
            },
            new KpiGradeForBootstrap
            {
                Order = 4,
                GradeTitle = "مقبول",
                CssColor = "text-muted",
                HighestValue = 0.6m,
                LowestValueInclusive = 0.5m
            },
            new KpiGradeForBootstrap
            {
                Order = 5,
                GradeTitle = "سيئ",
                CssColor = "text-danger",
                HighestValue = 0.5m,
                LowestValueInclusive = 0.0m
            }
        };
    }
}
