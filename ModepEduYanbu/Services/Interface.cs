using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using ModepEduYanbu.Helpers.Extensions;

namespace ModepEduYanbu.Services
{
    public interface IRequestValidationService
    {
        bool IsValidRequest(HttpContext context, string authToken);
    }

    public class RequestValidationService : IRequestValidationService
    {
        public bool IsValidRequest(HttpContext context, string authToken)
        {
            // https://github.com/twilio/twilio-csharp/blob/ab0983ae00269746c5dbeb891955ae3edd83796d/src/Twilio.Twiml/RequestValidator.cs

            if (context.Request.IsLocal())
            {
                return true;
            }

            // validate request
            // http://www.twilio.com/docs/security-reliability/security
            // Take the full URL of the request, from the protocol (http...) through the end of the query string (everything after the ?)
            var value = new StringBuilder();
            //var fullUrl = context.Request.Url.AbsoluteUri;
            string fullUrl = String.Format("{0}://{1}{2}{3}", context.Request.Scheme, context.Request.Host, context.Request.Path, context.Request.QueryString);//@$"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";

            value.Append(fullUrl);

            // If the request is a POST, take all of the POST parameters and sort them alphabetically.
            if (context.Request.Method.ToUpper() == "POST")
            {
                // Iterate through that sorted list of POST parameters, and append the variable name and value (with no delimiters) to the end of the URL string
                //var sortedKeys = context.Request.Form.AllKeys.OrderBy(k => k, StringComparer.Ordinal).ToList();
                var sortedKeys = context.Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
                //foreach (KeyValuePair<string, string> entry in myDictionary)
                foreach (var key in sortedKeys)
                {
                    value.Append(key.Key);
                    value.Append(context.Request.Form[key.Value]);
                }
            }

            // Sign the resulting value with HMAC-SHA1 using your AuthToken as the key (remember, your AuthToken's case matters!).
            var sha1 = new HMACSHA1(Encoding.UTF8.GetBytes(authToken));
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(value.ToString()));

            // Base64 encode the hash
            var encoded = Convert.ToBase64String(hash);

            // Compare your hash to ours, submitted in the X-Twilio-Signature header. If they match, then you're good to go.
            var sig = context.Request.Headers["X-Twilio-Signature"];

            return sig == encoded;
        }
    }
}
