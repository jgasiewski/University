using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ColorPicker
{
    class ColorRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                int result = int.Parse(value.ToString());

                if(result < 0 || result > 255)
                {
                    return new ValidationResult(false, "Value out of range");
                }
                else
                {
                    return ValidationResult.ValidResult;
                }
            }
            catch
            {
                return new ValidationResult(false, "Non int value");
            }
        }
    }
}
