using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ModepEduYanbu.DAL.DbContexts;
using ModepEduYanbu.Models;
using ModepEduYanbu.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Data
{
    public class ContextSeeder
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private IConfiguration _config;
        private IEducationalYearsRepo _educationalYearsRepo;

        public ContextSeeder(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration config,
            IEducationalYearsRepo educationalYearsRepo)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _educationalYearsRepo = educationalYearsRepo;
        }
        public async Task EnsureSeedPrimitiveDataAsync()
        {
            #region School level
            if (!_context.SchoolLevels.Any())
            {
                var primary = new SchoolLevel
                {
                    SchoollevelId = ((int)Templates.SchoolLevels.Primary).ToString(),
                    Name = "المرحلة الإبتدائية"
                };

                var middle = new SchoolLevel
                {
                    SchoollevelId = ((int)Templates.SchoolLevels.Middle).ToString(),
                    Name = "المرحلة المتوسطة"
                };

                var secondary = new SchoolLevel
                {
                    SchoollevelId = ((int)Templates.SchoolLevels.Secondary).ToString(),
                    Name = "المرحلة الثانوية"
                };

                _context.SchoolLevels.AddRange(primary, middle, secondary);
                await _context.SaveChangesAsync();
            }
            #endregion

            #region School types
            if (!_context.SchoolTypes.Any())
            {
                var gov = new SchoolType
                {
                    SchoolTypeId = ((int)Templates.SchoolTypes.Gov).ToString(),
                    ClassificationName = "حكومية"
                };

                var national = new SchoolType
                {
                    SchoolTypeId = ((int)Templates.SchoolTypes.National).ToString(),
                    ClassificationName = "أهلية"
                };

                var international = new SchoolType
                {
                    SchoolTypeId = ((int)Templates.SchoolTypes.International).ToString(),
                    ClassificationName = "عالمية"
                };

                _context.SchoolTypes.AddRange(gov, national, international);
                await _context.SaveChangesAsync();
            }
            #endregion
        }
        public async Task EnsureSeedRolesAsync()
        {
            if (!_roleManager.Roles.Any())
            {
                foreach (ApplicationRole r in Templates.Roles)
                    await _roleManager.CreateAsync(r);
            }
        }
        public async Task EnsureSeedSystemOwnerUser()
        {
            // sysowner
            if ((await _userManager.FindByEmailAsync("moabmu@outlook.com")) == null)
            {
                var moabmu = new ApplicationUser
                {
                    UserName = "moabmu",
                    Email = "moabmu@outlook.com",
                    PhoneNumber = "+966567894439",
                    FullName = "محمد الموسمي"
                };

                if ((await _userManager.CreateAsync(moabmu, "15002Mm##")).Succeeded)
                {
                    await _userManager.AddToRoleAsync(moabmu, Templates.Roles[0].Name);

                    // Confirm phone number
                    var smsCode = await _userManager.GenerateChangePhoneNumberTokenAsync(moabmu, moabmu.PhoneNumber);
                    await _userManager.ChangePhoneNumberAsync(moabmu, moabmu.PhoneNumber, smsCode);
                }
            }
        }
        public async Task EnsureSeedSchoolsAsync()
        {
            if (!_context.Schools.Any())
            {
                var school1 = new School
                {
                    MinistryNo = "8037",
                    Name = "مدارس الحديثة الأهلية الإبتدائية",
                    City = "ينبع البحر",
                    Level = _context.SchoolLevels
                    .FirstOrDefault(level => level.SchoollevelId == ((int)Templates.SchoolLevels.Primary).ToString()),
                    Type = _context.SchoolTypes.
                    FirstOrDefault(type => type.SchoolTypeId == ((int)Templates.SchoolTypes.National).ToString()),
                    PrincipleIdNo = "1020304044"
                };

                var school2 = new School
                {
                    MinistryNo = "8035",
                    Name = "مدارس الحديثة الأهلية المتوسطة",
                    City = "ينبع البحر",
                    Level = _context.SchoolLevels
        .FirstOrDefault(level => level.SchoollevelId == ((int)Templates.SchoolLevels.Middle).ToString()),
                    Type = _context.SchoolTypes.
        FirstOrDefault(type => type.SchoolTypeId == ((int)Templates.SchoolTypes.National).ToString()),
                    PrincipleIdNo = "1020304044"
                };

                var school3 = new School
                {
                    MinistryNo = "8050",
                    Name = "متوسطة سهيل بن عمرو",
                    City = "ينبع البحر",
                    Level = _context.SchoolLevels
    .FirstOrDefault(level => level.SchoollevelId == ((int)Templates.SchoolLevels.Middle).ToString()),
                    Type = _context.SchoolTypes.
    FirstOrDefault(type => type.SchoolTypeId == ((int)Templates.SchoolTypes.Gov).ToString()),
                    PrincipleIdNo = "1122335500"
                };

                _context.Schools.AddRange(school1, school2, school3);
                await _context.SaveChangesAsync();
            }
        }
        public async Task EnsureSeedAuthorizedPeopleAsync()
        {
            if (!_context.AuthorizedPeople.Any())
            {
                var person1 = new AuthorizedPerson
                {
                    IdNo = "1020304044",
                    FullName = "عواض محمد الشهري",
                    PhoneNumber = "0555555555",
                    Email = "awadh@a.com",
                    Role = await _roleManager.FindByNameAsync(Templates.Roles[3].Name),
                    SchoolMinistryNo = "8037",
                    ViaNoor = false
                };

                var person2 = new AuthorizedPerson
                {
                    IdNo = "1020304044",
                    FullName = "عواض محمد الشهري",
                    PhoneNumber = "0555555555",
                    Email = "awadh@a.com",
                    Role = await _roleManager.FindByNameAsync(Templates.Roles[3].Name),
                    SchoolMinistryNo = "8035",
                    ViaNoor = false
                };

                var person3 = new AuthorizedPerson
                {
                    IdNo = "1020304050",
                    FullName = "عباس السريع",
                    PhoneNumber = "0567894433",
                    Email = "a@a.com",
                    Role = await _roleManager.FindByNameAsync(Templates.Roles[2].Name),
                    ViaNoor = false
                };

                var person4 = new AuthorizedPerson
                {
                    IdNo = "1010010008",
                    FullName = "حمدان الصيدلاني",
                    PhoneNumber = "0567894433",
                    Email = "hamdan@a.com",
                    Role = await _roleManager.FindByNameAsync(Templates.Roles[1].Name),
                    ViaNoor = false
                };

                var person5 = new AuthorizedPerson
                {
                    IdNo = "1000500000",
                    FullName = "بندر الجهني",
                    PhoneNumber = "0508090603",
                    Email = "bandar@u.com",
                    Role = await _roleManager.FindByNameAsync(Templates.Roles[4].Name),
                    SchoolMinistryNo = "8037",
                    ViaNoor = false
                };

                var person6 = new AuthorizedPerson
                {
                    IdNo = "102030008",
                    FullName = "محمد العقيبي",
                    PhoneNumber = "0500012120",
                    Email = "mm@b.com",
                    Role = await _roleManager.FindByNameAsync(Templates.Roles[1].Name),
                    ViaNoor = false
                };

                var person7 = new AuthorizedPerson
                {
                    IdNo = "1122335500",
                    FullName = "برعي أبو جبهة",
                    PhoneNumber = "0505050505",
                    Email = "b@b.com",
                    Role = await _roleManager.FindByNameAsync(Templates.Roles[3].Name),
                    SchoolMinistryNo = "8050",
                    ViaNoor = false
                };

                _context.AuthorizedPeople.AddRange(person1, person2,
                    person3, person4, person5, person6, person7);
                await _context.SaveChangesAsync();
            }
        }

        public async Task EnsureSeedEduPrograms()
        {
            if (!_context.EduPrograms.Any())
            {
                var eduProgram1 = new EduProgram
                {
                    Name = ""
                };
            }
        }

        public async Task EnsureSeedUsersAsync()
        {
            //ApplicationUser moabmu, hamdan, alshehri; //maloqaibi, bandar, abbas, borei; // college1, accountantMiddle, accountantSecondary;
            //#region Create Roles then Users
            //if (!_roleManager.Roles.Any())
            //{
            //    foreach(ApplicationRole r in Templates.Roles)
            //    await _roleManager.CreateAsync(r);

            //    #region Create Users
            //    if (!_userManager.Users.Any())
            //    {
            //        // sysowner
            //        if (await _userManager.FindByEmailAsync("moabmu@outlook.com") == null)
            //        {
            //            moabmu = new ApplicationUser
            //            {
            //                UserName = "moabmu",
            //                Email = "moabmu@outlook.com",
            //                PhoneNumber = "00966567894439"
            //            };

            //            if ((await _userManager.CreateAsync(moabmu, "15002Mm##")).Succeeded)
            //            {
            //                await _userManager.AddToRoleAsync(moabmu, Templates.Roles[0].Name);
            //            }
            //        }

            //        // Hamdan
            //        if (await _userManager.FindByNameAsync("1010010008") == null)
            //        {
            //            hamdan = new ApplicationUser
            //            {
            //                UserName = "1010010008",
            //                Email = "hamdan@a.com",
            //                RegDate = DateTime.UtcNow,
            //                FullName = "حمدان محمد الصيدلاني",
            //                PhoneNumber = "00966567894439",
            //                ViaNoor = false
            //            };


            //            if ((await _userManager.CreateAsync(hamdan, "Hh#HH000")).Succeeded)
            //            {
            //                await _userManager.AddToRoleAsync(hamdan, Templates.Roles[1].Name);

            //                UserPositionTitle positionTitle = new UserPositionTitle
            //                {
            //                    User = hamdan,
            //                    PositionTitle = "رئيس قسم التوجيه والإرشاد"
            //                };
            //                hamdan.PositionTitle = positionTitle;
            //                await _context.SaveChangesAsync();
            //            }
            //        }

            //        // Awadh Alshehri
            //        if (await _userManager.FindByNameAsync("1020304044") == null)
            //        {
            //            alshehri = new ApplicationUser
            //            {
            //                UserName = "1020304044",
            //                Email = "wail@alhadetha.com",
            //                RegDate = DateTime.UtcNow,
            //                FullName = "عواض محمد الشهري",
            //                PhoneNumber = "00966567894439",
            //                ViaNoor = false
            //            };

            //            if ((await _userManager.CreateAsync(alshehri, "awadhH##0")).Succeeded)
            //            {
            //                await _userManager.AddToRoleAsync(alshehri, Templates.Roles[3].Name);

            //                alshehri = await _userManager.FindByNameAsync(alshehri.UserName);
            //                UserPositionTitle positionTitle = new UserPositionTitle
            //                {
            //                    User = alshehri,
            //                    PositionTitle = (await _userManager.GetRolesAsync(alshehri))[0]
            //                };
            //                alshehri.PositionTitle = positionTitle;
            //                await _context.SaveChangesAsync();
            //            }
            //        }

            //        #region other users
            //        //// Mohammed Aloqaibi
            //        //if (await _userManager.FindByNameAsync("1020030008") == null)
            //        //{
            //        //    maloqaibi = new ApplicationUser
            //        //    {
            //        //        UserName = "1020030008",
            //        //        Email = "mm@b.com",

            //        //    };

            //        //    if ((await _userManager.CreateAsync(maloqaibi, "mm$MM7")).Succeeded)
            //        //    {
            //        //        await _userManager.AddToRoleAsync(maloqaibi, Templates.Roles[1].Name);
            //        //    }
            //        //}

            //        //if (await _userManager.FindByNameAsync("salem") == null)
            //        //{
            //        //    salem = new ApplicationUser
            //        //    {
            //        //        UserName = "salem",
            //        //        Email = "salem@alhadetha.com"
            //        //    };

            //        //    if ((await _userManager.CreateAsync(salem, "sss@SCHOOL")).Succeeded)
            //        //    {
            //        //        await _userManager.AddToRoleAsync(salem, accountantRole.Name);
            //        //    }
            //        //}

            //        //if (await _userManager.FindByNameAsync("school") == null)
            //        //{
            //        //    school = new ApplicationUser
            //        //    {
            //        //        UserName = "school",
            //        //        Email = "boys@alhadetha.com"
            //        //    };

            //        //    if ((await _userManager.CreateAsync(school, "alhadetha@SCHOOL1")).Succeeded)
            //        //    {
            //        //        await _userManager.AddToRoleAsync(school, schoolManagerRole.Name);
            //        //    }
            //        //}
            //        #endregion
            //    }
            //    #endregion
            //}
            //#endregion

        }

        public async Task EnsureSeedAzureContainerAsync()
        {


            var containerName = _config["AzurePlatform:ContainerName"] + "1";

            var connStr = _config["AzurePlatform:StorageConnString"];

            // Set the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connStr);

            // Create a blob client. 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            #region Ensure Enable Streaming and Latest features on Azure Account
            var properties = await blobClient.GetServicePropertiesAsync();
            properties.DefaultServiceVersion = "2013-08-15";
            await blobClient.SetServicePropertiesAsync(properties);
            #endregion


            // Get a reference to a container  
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            if (!await container.ExistsAsync())
            {
                var newContainer = new AzureContainer
                {
                    AzureContainerId = containerName,
                    Name = containerName,
                    CreatedDate = DateTime.Now,
                    Length = 0
                };
                _context.AzureContainers.Add(newContainer);
                await _context.SaveChangesAsync();

                await container.CreateIfNotExistsAsync();
                await container.SetPermissionsAsync(
                    new BlobContainerPermissions {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
            }
        }

        public void EnsureSeedCurrentEducationalYear()
        {
            if (!_context.EducationalYears.Any())
            {
                _educationalYearsRepo.GetCurrentYearOrCreateIfNotExist();
            }
        }
    }
}
