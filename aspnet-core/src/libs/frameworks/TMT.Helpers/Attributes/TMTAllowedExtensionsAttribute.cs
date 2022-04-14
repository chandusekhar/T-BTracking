using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace TMT.Helpers.Attributes
{
    public class TMTAllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        private readonly Type _TypeResource;
        private readonly string _Name;
        private readonly string[] _LocalizationFormMat;
        public TMTAllowedExtensionsAttribute(string[] extensions, Type TypeResource, string Name, string[] Localizations = null)
        {
            _extensions = extensions;
            _TypeResource = TypeResource;
            _LocalizationFormMat = Localizations;
            _Name = Name;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var _localizer = validationContext.GetService(_TypeResource) as IStringLocalizer;

            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    if (_LocalizationFormMat == null || _LocalizationFormMat.Length == 0)
                    {
                        ErrorMessage = _localizer[_Name];
                        return new ValidationResult(_localizer[_Name]);

                    }
                    ErrorMessage = _localizer[_Name, _LocalizationFormMat];
                    return new ValidationResult(_localizer[_Name, _LocalizationFormMat]);
                }
             

            }
            return ValidationResult.Success;
        }

       
    }
}
