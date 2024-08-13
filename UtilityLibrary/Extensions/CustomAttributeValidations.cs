using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace UtilityLibrary.Extensions
{
    public class CustomNameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string obj = value.ToString();
            bool isValid = Regex.IsMatch(obj, @"^[A-Za-z'-][A-Za-z'-]*$");
            if (isValid)
                return true;
            else
            {
                ErrorMessage = $"{obj} contains an invalid character (Hyphens and quotes are allowed)";
                return false;
            }
        }
    }
    public class CustomStringAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string obj = value.ToString();
            bool isValid = Regex.IsMatch(obj, @"^[A-Za-z'-@&,.:;0-9 ][ A-Za-z'-@&,.:;0-9]*$");
            if (isValid)
                return true;
            else
            {
                ErrorMessage = $"{obj} contains an invalid character (Special characters like period, comma, hyphen, quote, colon, and semi-colon are allowed)";
                return false;
            }
        }
    }
    public class PureStringAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string obj = value.ToString();
            bool isValid = Regex.IsMatch(obj, @"^[A-Za-z'-][ A-Za-z'-]*$");
            if (isValid)
                return true;
            else
            {
                ErrorMessage = $"{obj} contains an invalid character (Hyphens quotes and spaces are allowed)";
                return false;
            }
        }
    }

    public class DateOfBirthValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var obj = Convert.ToDateTime(value.ToString());
            if (obj.Date > DateTime.Now)
            {
                ErrorMessage = $"{obj} contains an invalid character (Hyphens quotes and spaces are allowed)";
                return false;
            }
            else
                return true;
        }
    }

    public class CustomPhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string obj = value.ToString();
            if (obj.Length < 12)
            {
                ErrorMessage = "Please enter a valid Phone Number with Country Code(e.g. +2348012345678)";
                return false;
            }
            else
            {
                bool isValid = Regex.IsMatch(obj, @"^(\+[0-9]{9})$");
                if (isValid) return true;
                else
                {
                    ErrorMessage = "Please enter a valid Phone Number with Country Code(e.g. +2348012345678)";
                    return false;
                }
            }
        }       
    }

    public class CustomEmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string obj = value.ToString();
            bool isValid = Regex.IsMatch(obj, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (isValid)
                return true;
            else
            {
                ErrorMessage = $"{obj} is not a valid Email Address (eg john@mail.com))";
                return false;
            }
        }
    }
}
