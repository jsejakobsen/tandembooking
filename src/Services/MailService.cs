﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace TandemBooking.Services
{
    public class MailService
    {
        private readonly MailSettings _settings;

        public MailService(MailSettings settings)
        {
            _settings = settings;
        }

        public async Task Send(string recipient, string subject, string body)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            emailMessage.To.Add(new MailboxAddress("", recipient));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort).ConfigureAwait(false);
                await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPassword).ConfigureAwait(false);
                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}