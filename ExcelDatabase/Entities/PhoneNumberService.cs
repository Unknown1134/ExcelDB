using PhoneNumbers;

namespace ExcelDatabase.Entities
{
    public class PhoneNumberService
    {
        public bool TryValidatePhoneNumber(string phoneNumber, out string country, out bool isValid)
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            try
            {
                var numberProto = phoneNumberUtil.Parse(phoneNumber, null);
                isValid = phoneNumberUtil.IsValidNumber(numberProto);
                country = phoneNumberUtil.GetRegionCodeForNumber(numberProto);

                return true;
            }
            catch (NumberParseException)
            {
                country = null;
                isValid = false;
                return false;
            }
        }
    }


}
