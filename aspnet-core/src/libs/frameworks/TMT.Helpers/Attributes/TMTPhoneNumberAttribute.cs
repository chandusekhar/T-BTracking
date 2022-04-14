using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using TMT.Helpers.PhoneNumbers;

namespace TMT.Helpers.Attributes
{
    public class TMTPhoneNumberAttribute : ValidationAttribute
    {

        private readonly Type _TypeResource;
        private readonly string _Name;
        private readonly string[] _LocalizationFormMat;


        public TMTPhoneNumberAttribute(Type TypeResource, string Name, string[] Localizations = null)
        {
            _TypeResource = TypeResource;
            _LocalizationFormMat = Localizations;
            _Name = Name;

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _localizer = validationContext.GetService(_TypeResource) as IStringLocalizer;
            var phoneNumber = value as string;
            var service= validationContext.GetService(typeof(IPhoneNumberServcie)) as IPhoneNumberServcie;
            if (!service.CheckPhoneNumber(phoneNumber))
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
