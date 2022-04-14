using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace TMT.Helpers.Attributes
{
    public class TMTUsernamePaternAttribute : ValidationAttribute
    {

        private readonly Type _TypeResource;
        private readonly string _Name;
        private readonly string[] _LocalizationFormMat;


        public TMTUsernamePaternAttribute(Type TypeResource, string Name, string[] Localizations = null)
        {
            _TypeResource = TypeResource;
            _LocalizationFormMat = Localizations;
            _Name = Name;

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _localizer = validationContext.GetService(_TypeResource) as IStringLocalizer;
            var username = value as string;
            // Regex to check valid username. 
            Regex regex = new Regex(@"^[a-zA-Z0-9_]{8,16}$");
            //Regex regex = new Regex(@"^[a-zA-Z\w]{8,16}$");
            //Regex regexDigit = new Regex(@"^[\d]{8,16}$");

            if (username != null && !regex.IsMatch(username))
            //if (username != null && (!regex.IsMatch(username) || regexDigit.IsMatch(username)))
            {
                if (_LocalizationFormMat == null || _LocalizationFormMat.Length == 0)
                {
                    ErrorMessage = _localizer[_Name];
                    return new ValidationResult(_localizer[_Name]);

                }
                ErrorMessage = _localizer[_Name, _LocalizationFormMat];
                return new ValidationResult(_localizer[_Name, _LocalizationFormMat]);
            }
            return ValidationResult.Success;


        }
    }
}

