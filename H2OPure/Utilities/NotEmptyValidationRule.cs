using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Controls;


namespace H2OPure.Utilities
{

        public class NotEmptyValidationRule : ValidationRule
        {
            private bool _validateOnTargetUpdated;

            public bool ValidatesOnTargetUpdated
            {
                get { return _validateOnTargetUpdated; }
                set { _validateOnTargetUpdated = value; }
            }

            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                // Verificar si el valor es nulo o una cadena vacía
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    // Devolver un resultado de validación fallido solo si la validación está activada
                    if (_validateOnTargetUpdated)
                    {
                        return new ValidationResult(false, "El campo no puede estar vacío.");
                    }
                    else
                    {
                        // Devolver un resultado de validación válido durante la digitación y filtrado
                        return ValidationResult.ValidResult;
                    }
                }

                // Devolver un resultado de validación válido
                return ValidationResult.ValidResult;
            }
        }

}
