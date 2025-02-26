namespace Rirais.Task.GrpcServer.Models;

public static class ValidationExtensions
{

    /// <summary>
    /// it's copy from stack stackoverflow.
    /// <see cref="https://stackoverflow.com/questions/44603570/how-to-validate-iranian-national-code-melli-code-or-code-melli-in-android"/>
    /// </summary>
    public static bool IsValidNationalCode(string nationalCode)
    {
        if (nationalCode.Length != 10)
        {
            return false;
        }
        else
        {
            /// Check for equal numbers
            String[] allDigitEqual = { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (allDigitEqual.Contains(nationalCode))
            {
                return false;
            }
            else
            {
                int sum = 0;
                int lenght = 10;
                for (int i = 0; i < lenght - 1; i++)
                {
                    sum += int.Parse(char.ToString(nationalCode[i])) * (lenght - i);
                }
                int r = int.Parse(char.ToString(nationalCode[9]));
                int c = sum % 11;
                return (((c < 2) && (r == c)) || ((c >= 2) && ((11 - c) == r)));
            }

        }
    }

}
