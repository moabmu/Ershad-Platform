using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fiver.Api.HttpClient;
using System.Text;
using System.Net;
using System.IO;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ModepEduYanbu.Services
{
    // This class is used by the application to send Email and SMS
    public class AuthMessageSender : IEmailSender, ISmsSender, IPhoneCaller
    {
        private const string accountSid = null;
        private const string authToken = null;
        private const string phoneNumber = null;
        private readonly IRequestValidationService _requestValidationService;
        private const string sendGridUser = null;
        private const string sendGridKey = null;

        public SMSoptions Options;
        public AuthMessageSender(IOptions<SMSoptions> optionsAccessor,
            IRequestValidationService requestValidationService)
        {
            Options = optionsAccessor.Value;
            _requestValidationService = requestValidationService;
        }

        public async Task<IActionResult> MakePhoneCallAsync(string number, string message, HttpContext context)
        {
            //var twilioAuthToken = authToken;
            //if (!_requestValidationService.IsValidRequest(context, twilioAuthToken))
            //{
            //    return new UnauthorizedResult();
            //}

            //var response = new VoiceResponse();
            //response
            //    .Say("Welcome to Yanbu Education. Your code is " +
            //         "1 2 5 6. Again 1 2 5 6. Goodbye!")
            //    .Dial(number)
            //    .Hangup();

            //return new OkObjectResult(response);
            return null;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            //// Plug in your email service here to send an email.
            //return Task.FromResult(0);
            return ExecuteWithSendGrid(sendGridKey, email, subject, message);
        }

        public async Task SendSmsAsync(string number, string message)
        {

            try
            {
                var baseUri = "https://www.mobily.ws/api/msgSend.php";
                var mobile = null;
                var password = null;
                var numbers = number.Replace("+","");
                var sender = null;
                var msg = message;

                var response = SendMessage(mobile, password, ConvertToUnicode(msg), sender, numbers);

                if(response == "2" || response == "3" || response == "6" || response == "18")
                {
                    TwilioClient.Init(accountSid, authToken);
                    var numbersList = numbers.Split(',');
                    foreach(var num in numbersList)
                    {
                        int counter = 0;
                        sendTwilio:
                        var msgTwilio = await MessageResource.CreateAsync(
                            to: new PhoneNumber("+" + num),
                            from: new PhoneNumber(phoneNumber),
                            body: message);
                        if ((msgTwilio.ErrorCode.HasValue || msgTwilio.Status == MessageResource.StatusEnum.Undelivered) && counter < 1)
                        {
                            await Task.Delay(250);
                            counter++;
                            goto sendTwilio;
                        }
                    }
                }
            }
            catch { }
        }

        public async Task SendSmsAsync(IEnumerable<string> numbers, string message)
        {
            if (numbers == null)
                return;

            var numbersList = numbers.ToList();
            int packageSize = 100;
            int start = 0, end = (numbersList.Count > packageSize) ? packageSize - 1 : numbersList.Count - 1;
            StringBuilder builder;

            sendsms:
            builder = new StringBuilder();
            for(int i=start; i <= end; i++)
            {
                if(!String.IsNullOrEmpty(numbersList[i]))
                builder.Append(numbersList[i]).Append(",");
            }
            var strNumbers = builder.ToString();
            try { strNumbers = strNumbers.Remove(strNumbers.Length - 1); } catch { }
            await this.SendSmsAsync(strNumbers, message);

            if(numbersList.Count - 1 > end) {
                var remaining = numbersList.Count - 1 - end;
                start = end + 1;
                end += (remaining >= packageSize )? packageSize : remaining;
                goto sendsms;
            }
        }


        #region Mobily.ws Helpers
        private string ConvertToUnicode(string val)
        {
            string msg2 = string.Empty;

            for (int i = 0; i < val.Length; i++)
            {
                msg2 += convertToUnicode(System.Convert.ToChar(val.Substring(i, 1)));
            }

            return msg2;
        }
        private string convertToUnicode(char ch)
        {
            System.Text.UnicodeEncoding class1 = new System.Text.UnicodeEncoding();
            byte[] msg = class1.GetBytes(System.Convert.ToString(ch));

            return fourDigits(msg[1] + msg[0].ToString("X"));
        }
        private string fourDigits(string val)
        {
            string result = string.Empty;

            switch (val.Length)
            {
                case 1: result = "000" + val; break;
                case 2: result = "00" + val; break;
                case 3: result = "0" + val; break;
                case 4: result = val; break;
            }

            return result;
        }
        private string SendMessage(string username, string password, string msg, string sender, string numbers)
        {
            int temp = '0';

            HttpWebRequest req = (HttpWebRequest)
            WebRequest.Create("https://www.mobily.ws/api/msgSend.php");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            string postData = "mobile=" + username + "&password=" + password + "&numbers=" + numbers + "&sender=" + sender + "&msg=" + msg + "&applicationType=68";
            req.ContentLength = postData.Length;

            StreamWriter stOut = new
            StreamWriter(req.GetRequestStream(),
            System.Text.Encoding.ASCII);
            stOut.Write(postData);
            stOut.Close();
            // Do the request to get the response
            string strResponse;
            StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream());
            strResponse = stIn.ReadToEnd();
            stIn.Close();
            return strResponse;
        }

        #endregion

        private Task<Response> ExecuteWithSendGrid(string apiKey, string email, string subject, string message)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("noreply@moeduyanbu.com", "منصة إرشاد ينبع"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            msg.TrackingSettings = new TrackingSettings
            {
                ClickTracking = new ClickTracking { Enable = false }
            };

            return client.SendEmailAsync(msg);
        }
    }
}
