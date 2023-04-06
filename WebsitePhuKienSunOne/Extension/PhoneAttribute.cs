using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class PhoneAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value == null)
        {
            return true;
        }

        var phoneNumber = value.ToString();

        var pattern = @"^\+?\d{0,3}[\s.-]?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";

        return Regex.IsMatch(phoneNumber, pattern);
    }
}
