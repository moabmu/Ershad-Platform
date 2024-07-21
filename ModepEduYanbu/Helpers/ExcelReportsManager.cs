using ExcelLibrary.SpreadSheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ModepEduYanbu.DAL.DbContexts;
using ModepEduYanbu.Data;
using ModepEduYanbu.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Helpers
{
    public class ExcelReportsManager
    {

        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;

        public ExcelReportsManager(ApplicationDbContext context,
                        UserManager<ApplicationUser> userManager,
                        RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public struct UploadSchoolsAndPrinciplesResult
        {
            public int SchoolsCount { get; set; }
            public int PrinciplesCount { get; set; }
        }

        public async Task<UploadSchoolsAndPrinciplesResult> UploadSchoolsAndPrinciples(List<IFormFile> excelFiles,
            ApplicationUser uploaderUser)
        {
            int schoolsCount = 0;
            int principlesCount = 0;

            try
            {
                var filePath = Path.GetTempPath();

                foreach (var formFile in excelFiles)
                {
                    if (formFile.Length > 0)
                    {
                        using (var stream = new FileStream(Path.Combine(filePath, formFile.FileName), FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }

                var task = Task.Run(async () =>
                {

                    //List<School> schools = new List<School>();
                    //List<AuthorizedPerson> principles = new List<AuthorizedPerson>();
                    foreach (var filename in excelFiles)
                    {
                        var path = Path.Combine(filePath, filename.FileName);
                        var workbook = Workbook.Load(path);

                        foreach (var sheet in workbook.Worksheets)
                        {
                            for (int i = 1; i <= sheet.Cells.LastRowIndex; i++)
                            {
                                if (!_context.Schools.Any(x => x.MinistryNo == sheet.Cells[i, 5].StringValue))
                                {
                                    var school = new School
                                    {
                                        Name = sheet.Cells[i, 1].StringValue,
                                        City = sheet.Cells[i, 2].StringValue,
                                        PhoneNumber = sheet.Cells[i, 4].StringValue,
                                        MinistryNo = sheet.Cells[i, 5].StringValue,
                                        PrincipleIdNo = sheet.Cells[i, 7].StringValue
                                    };

                                    SchoolType schoolType = null;
                                    switch (sheet.Cells[i, 0].StringValue)
                                    {
                                        case "حكومي":
                                            schoolType = _context.SchoolTypes
                                            .FirstOrDefault(type => type.SchoolTypeId == ((int)Templates.SchoolTypes.Gov).ToString());
                                            break;
                                        case "أهلي":
                                            schoolType = _context.SchoolTypes
                                            .FirstOrDefault(type => type.SchoolTypeId == ((int)Templates.SchoolTypes.National).ToString());
                                            break;
                                    }
                                    if (schoolType != null) school.Type = schoolType;

                                    SchoolLevel schoolLevel = null;
                                    switch (sheet.Cells[i, 3].StringValue)
                                    {
                                        case "الإبتدائية":
                                            schoolLevel = _context.SchoolLevels
                                            .FirstOrDefault(level => level.SchoollevelId == ((int)Templates.SchoolLevels.Primary).ToString());
                                            break;
                                        case "المتوسطة":
                                            schoolLevel = _context.SchoolLevels
                                            .FirstOrDefault(level => level.SchoollevelId == ((int)Templates.SchoolLevels.Middle).ToString());
                                            break;
                                        case "الثانوية":
                                            schoolLevel = _context.SchoolLevels
                                            .FirstOrDefault(level => level.SchoollevelId == ((int)Templates.SchoolLevels.Secondary).ToString());
                                            break;
                                    }
                                    if (schoolLevel != null) school.Level = schoolLevel;

                                    _context.Schools.Add(school);
                                    await _context.SaveChangesAsync();
                                    schoolsCount++;
                                }

                                if (!_context.AuthorizedPeople.Any(x => x.IdNo == sheet.Cells[i, 7].StringValue
                                && x.SchoolMinistryNo == sheet.Cells[i, 5].StringValue))
                                {
                                    var principle = new AuthorizedPerson
                                    {
                                        AddedByUser = uploaderUser,
                                        FullName = sheet.Cells[i, 6].StringValue,
                                        IdNo = sheet.Cells[i, 7].StringValue,
                                        PhoneNumber = $"0{sheet.Cells[i, 8].StringValue}",
                                        SchoolMinistryNo = sheet.Cells[i, 5].StringValue,
                                        ViaNoor = true,
                                        Role = await _roleManager.FindByNameAsync(Templates.Roles[3].Name)
                                    };
                                    _context.AuthorizedPeople.Add(principle);
                                    await _context.SaveChangesAsync();
                                    principlesCount++;
                                }
                            }
                        }

                        System.IO.File.Delete(path);
                    }
                    //return await UploadStudentsToDbAsync(schools);
                    return new UploadSchoolsAndPrinciplesResult { SchoolsCount = schoolsCount, PrinciplesCount = principlesCount };
                });

                return task.Result;
            }
            catch (Exception ex)
            {
                //return -1; //Task.Run(() => -1);  
                throw ex;
            }
        }

        public async Task<dynamic> AddLevelsForSchools(List<IFormFile> excelFiles)
        {
            int schoolsCount = 0;
            int ContextChangesCount = 0;
            try
            {
                var filePath = Path.GetTempPath();

                foreach (var formFile in excelFiles)
                {
                    if (formFile.Length > 0)
                    {
                        using (var stream = new FileStream(Path.Combine(filePath, formFile.FileName), FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }

                //List<School> schools = new List<School>();
                //List<AuthorizedPerson> principles = new List<AuthorizedPerson>();
                foreach (var filename in excelFiles)
                {
                    var path = Path.Combine(filePath, filename.FileName);
                    var workbook = Workbook.Load(path);

                    foreach (var sheet in workbook.Worksheets)
                    {
                        for (int i = 1; i <= sheet.Cells.LastRowIndex; i++)
                        {

                            var school = _context.Schools.FirstOrDefault(x => x.MinistryNo == sheet.Cells[i, 5].StringValue);
                            if (school is null) continue;


                            SchoolLevel schoolLevel = null;
                            switch (sheet.Cells[i, 3].StringValue)
                            {
                                case "الإبتدائية":
                                    schoolLevel = _context.SchoolLevels
                                    .FirstOrDefault(level => level.SchoollevelId == ((int)Templates.SchoolLevels.Primary).ToString());
                                    break;
                                case "المتوسطة":
                                    schoolLevel = _context.SchoolLevels
                                    .FirstOrDefault(level => level.SchoollevelId == ((int)Templates.SchoolLevels.Middle).ToString());
                                    break;
                                case "الثانوية":
                                    schoolLevel = _context.SchoolLevels
                                    .FirstOrDefault(level => level.SchoollevelId == ((int)Templates.SchoolLevels.Secondary).ToString());
                                    break;
                            }
                            if (schoolLevel != null) school.Level = schoolLevel;

                            _context.Schools.Update(school);
                            schoolsCount++;
                        }
                    }
                    ContextChangesCount = await _context.SaveChangesAsync();
                    System.IO.File.Delete(path);
                }
                //return await UploadStudentsToDbAsync(schools);
                return new { SchoolsCount = schoolsCount, PrinciplesCount = ContextChangesCount };
            }
            catch (Exception ex)
            {
                //return -1; //Task.Run(() => -1);  
                throw ex;
            }
        }
    }
}
