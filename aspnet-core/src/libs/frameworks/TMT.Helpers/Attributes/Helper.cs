using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMT.Helpers.Extensions;

namespace TMT.Helpers.Attributes
{
    public static class Helper
    {
        public static bool CheckValidateEmail(string email)
        {
            // Regex to check valid password. 
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            if (!string.IsNullOrWhiteSpace(email) && !regex.IsMatch(email))
            {
                return false;
            }
            return true;
        }
        public static bool CheckValidateName(string name)
        {
            // Regex to check valid name. 
            Regex regex = new Regex(@"^[a-zA-Z'\s]{2,30}$");
            var nameRemoveAscent = StringExtension.ConvertToNonUnicode(name);
            if (name != null && !regex.IsMatch(nameRemoveAscent.Trim()))
            {
                return false;
            }
            return true;

        }
        public static bool CheckValidatePassword(string password)
        {
           
            Regex regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s)(?=.*[\W]).{8,32}$");


            if (password != null && !regex.IsMatch(password))
            {
                return false;
            }
            return true;
        }
    }
}
