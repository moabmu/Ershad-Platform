using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.Helpers.ValidationsAttributes
{
    /// <summary>
    /// Validate HijriDate (Umm Alqura calculations)
    /// </summary>
    public class ValidateHijriDate : ValidationAttribute, IClientModelValidator
    {

        public void AddValidation(ClientModelValidationContext context)
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var strValue = value.ToString();
                DateTime date = DateTime.Parse(strValue);
            }
            catch
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return $"التاريخ المدخل غير صحيح.";
        }
    }
}
