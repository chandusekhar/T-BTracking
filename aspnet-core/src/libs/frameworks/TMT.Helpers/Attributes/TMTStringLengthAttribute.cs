using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TMT.Helpers.Attributes
{
    public class TMTStringLengthAttribute: StringLengthAttribute
    {
        private readonly Type _TypeResource;
        private readonly string _Name;
        private readonly string[] _LocalizationFormMat;
        public TMTStringLengthAttribute(int maximumLength, Type TypeResource, string Name, string[] Localizations = null) :base(maximumLength)
        {
            _TypeResource = TypeResource;
            _LocalizationFormMat = Localizations;
            _Name = Name;
        }
        protected override ValidationResult IsValid(
               object value, ValidationContext validationContext)
        {
            var _localizer = validationContext.GetService(_TypeResource) as IStringLocalizer;
            if (!this.IsValid(value))
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
