using System;
using System.Collections.Generic;
using System.Text;

namespace TMT.Helpers.PhoneNumbers
{
    public interface IPhoneNumberServcie
    {
         bool CheckPhoneNumber(string telephoneNumber, string countryCode="VN");
         string GetOriginalNumber(string telephoneNumber, string countryCode="VN");
      
    }
}
