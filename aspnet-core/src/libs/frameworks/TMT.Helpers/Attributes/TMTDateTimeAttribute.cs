using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TMT.Helpers.Attributes
{
    public class TMTDateTimeAttribute : ValidationAttribute
    {

        private readonly Type _TypeResource;
        private readonly string _Name;
        private readonly string[] _LocalizationFormMat;
    
        public DateTime? Max { get; set; } = DateTime.Today;
        public DateTime? Min { get; set; } = null;



        public TMTDateTimeAttribute(  Type TypeResource, string Name, string[] Localizations = null)
        {
            _TypeResource = TypeResource;
            _LocalizationFormMat = Localizations;
            _Name = Name;
          

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var _localizer = validationContext.GetService(_TypeResource) as IStringLocalizer;
            var checkDate = value is DateTime;
            if (value == null)
            {
                return ValidationResult.Success;
            }
            if (  checkDate)
            {
                var date = (DateTime)value;
                if ((Min != null && date.Date < Min.GetValueOrDefault().Date) || (Max != null && date.Date > Max.GetValueOrDefault().Date))
                {
                    return ReturnValidate(_localizer);

                }
                return ValidationResult.Success;

            }
            else
            {
                return ReturnValidate(_localizer);
            }
        }
        private ValidationResult ReturnValidate(IStringLocalizer _localizer)
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
}
