using PhoneNumbers;
using System.Text.RegularExpressions;

namespace TMT.Helpers.PhoneNumbers
{
    public class PhoneNumberServcie : IPhoneNumberServcie
    {
        public bool CheckPhoneNumber(string telephoneNumber,string countryCode= "VN")
        {
            Regex regex = new Regex(@"^(\+)?[\d -]+$");
            try
            {
                if (!regex.IsMatch(telephoneNumber))
                {
                    throw new NumberParseException(ErrorType.NOT_A_NUMBER,"PhoneNumber is not valid");

                }
                var original=GetOriginalNumber(telephoneNumber, countryCode);
                if(countryCode=="VN" && original.Length != 12)
                {
                    throw new NumberParseException(ErrorType.NOT_A_NUMBER, "PhoneNumber is not valid");

                }
                return true;
            }
            catch (NumberParseException ex)
            {
                return false;
            }
        }


        public string GetOriginalNumber(string telephoneNumber,string countryCode= "VN")
        {
            PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
            try
            {
                PhoneNumber phoneNumber = phoneUtil.Parse(telephoneNumber, countryCode);

                return phoneUtil.Format(phoneNumber, PhoneNumberFormat.E164); // Produces "+447825152591"

            }
            catch (NumberParseException ex)
            {

                throw ex;

            }
        }
    }
}
