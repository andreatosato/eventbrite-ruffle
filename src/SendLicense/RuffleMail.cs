using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SendLicense
{
    public class RuffleMail
    {
		public async Task SendMail(RuffleData data)
        {
			GraphServiceClient graphClient = new GraphServiceClient(new SimpleAuthProvider("de182c6d-2bb3-4f3c-b6d7-13ab68713c10", "cd4ffa3e-99e9-4d72-9822-3864779cf97a"));

			var message = new Message
			{
				Subject = "Ruffle winner",
				Body = new ItemBody
				{
					ContentType = BodyType.Html,
					Content = @$"Dear <i>{data.name}</i> and Codegen 2021 participant.
<br/>We thank you for participating and leave you the code to redeem for a microsoft exam with a 50 % discount.
<br/><h4><i>This code is valid until April 30, 2021.</i></h4>
<br/>The code allows the purchase of an exam, the exam can also be scheduled in the following months but it is necessary to redeem it after the deadline.

<br/><h2>Here is the code: <b>{ data.licenseNumber}</b></h2>"
				},
				ToRecipients = new List<Recipient>()
				{
					new Recipient
					{
						EmailAddress = new EmailAddress
						{
							Address = data.email
						}						
					}
				},
				CcRecipients = new [] { 
					new Recipient { EmailAddress = new EmailAddress { Address = "andrea.tosato@cloudgen.it" } },
					new Recipient { EmailAddress = new EmailAddress { Address = "marco.zamana@cloudgen.it" } }
				}
			};

			Console.WriteLine($"Sending mail to: {data.name}");

			await graphClient.Me
				.SendMail(message, true)
				.Request()
				.PostAsync();

			Console.WriteLine($"Sent mail to: {data.name}");
		}

    }

    public class SimpleAuthProvider : IAuthenticationProvider
    {
		private string clientId;
		private string tenantId;
        public SimpleAuthProvider(string clientId, string tenantId)
        {
			this.clientId = clientId;
			this.tenantId = tenantId;
        }
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
			var app = PublicClientApplicationBuilder.Create(clientId)
				.WithDefaultRedirectUri()
				.WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
				.Build();

			var authResult = await app.AcquireTokenByUsernamePassword(new[] { "Mail.Send" }, 
				"associazione@cloudgen.it", ConvertToSecureString("Password")
				)
									  .ExecuteAsync();

			request.Headers.Add("Authorization", authResult.AccessToken);
		}

		private SecureString ConvertToSecureString(string password)
		{
			if (password == null)
				throw new ArgumentNullException("password");

			var securePassword = new SecureString();

			foreach (char c in password)
				securePassword.AppendChar(c);

			securePassword.MakeReadOnly();
			return securePassword;
		}
	}
}
