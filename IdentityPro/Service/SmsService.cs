using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IdentityPro.Service
{
    public class SmsService
    {
        public void Send(string PhoneNumbr , string Code)
        {
            var client = new WebClient();
            //apikey کلید اصلی برای هر یوزر از سایت کاوه نگار
            //VerifyBugetoAccount اسم الگویی که برای پیامک ساختیم در سایت کاوه نگار
            string url = $"http://panel.kavenegar.com/v1/apikey/verify/lookup.json?receptor={PhoneNumbr}&token={Code}&template=VerifyBugetoAccount";
            var contect = client.DownloadString(url);
        }
    }
}
