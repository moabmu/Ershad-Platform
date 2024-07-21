//using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
//using ModepEduYanbu.Repositories;
//using ModepEduYanbu.Repositories.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ModepEduYanbu.Helpers.ValidationsAttributes
//{
//    public class ValidateSchoolMinistryNoAttribute : ValidationAttribute, IClientModelValidator
//    {
//        public void AddValidation(ClientModelValidationContext context)
//        {

//        }

//        public string SchoolId { get; set; }
//        public string PersonIdNo { get; set; }

//        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//        {
//            Type objType = validationContext.ObjectInstance.GetType();
//            var objProperty = objType.GetProperties().Where(prop => prop.Name == validationContext.MemberName).FirstOrDefault();

//            // Get the injected repository in the controller. https://andrewlock.net/injecting-services-into-validationattributes-in-asp-net-core/
//            var repo = (ISchoolRepo)validationContext.GetService(typeof(ISchoolRepo));

//            string personId = objProperty.GetValue(validationContext.ObjectInstance).ToString();
//            if (!(1==1))
//            {
//                return new ValidationResult("لا توجد بيانات لرقم الهوية المدخل.");
//            }
//            return ValidationResult.Success;
//        }
//    }
//}
