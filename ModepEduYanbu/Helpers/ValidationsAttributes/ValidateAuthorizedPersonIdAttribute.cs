using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ModepEduYanbu.Repositories;
using ModepEduYanbu.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Helpers.ValidationAttributes
{
    public class ValidateAuthorizedPersonIdAttribute : ValidationAttribute, IClientModelValidator
    {
        // Custom Validation https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation#custom-validation
        // https://stackoverflow.com/questions/294216/why-does-c-sharp-forbid-generic-attribute-types

        public void AddValidation(ClientModelValidationContext context)
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Type objType = validationContext.ObjectInstance.GetType();
            var objProperty = objType.GetProperties().Where(prop => prop.Name == validationContext.MemberName).FirstOrDefault();

            // Get the injected repository in the controller. https://andrewlock.net/injecting-services-into-validationattributes-in-asp-net-core/
            var repo = (IAuthorizedPeopleRepo)validationContext.GetService(typeof(IAuthorizedPeopleRepo));

            string personId = objProperty.GetValue(validationContext.ObjectInstance).ToString();
            if (!repo.IsIdNoValid(personId))
            {
                return new ValidationResult("لا يمكن متابعة التسجيل مع رقم الهوية المدخل.");
            }
            return ValidationResult.Success;
        }
    }
}
