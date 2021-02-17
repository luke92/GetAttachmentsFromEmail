using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MailKit.Net.Imap;
using MailKit;
using MailKit.Search;
using MimeKit;
using GAFEAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GAFEAPI.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IMAPConfiguration _imapConfiguration;

        public AttachmentController(IMAPConfiguration imapConfiguration)
        {
            _imapConfiguration = imapConfiguration;
        }

        // GET: api/<AttachmentController>
        [HttpGet]
        public async Task<string> Get()
        {
            var response = "";

            var client = new ImapClient();
            client.Connect(_imapConfiguration.Server, _imapConfiguration.Port, _imapConfiguration.UseSSL);
            client.Authenticate(_imapConfiguration.Email, _imapConfiguration.Password);

            // The Inbox folder is always available on all IMAP servers...
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadOnly);

            //Unread Emails and not deleted
            var query = SearchQuery.NotSeen.And(SearchQuery.NotDeleted);
            //Filter by Subject
            //query = query.And(SearchQuery.SubjectContains("subject"));

            var results = inbox.Search(query);
            // Get Items
            var items = MessageSummaryItems.BodyStructure | MessageSummaryItems.UniqueId;
            var matched = new UniqueIdSet();

            foreach (var message in inbox.Fetch(results, items))
            {
                //Has Attachment
                if (message.BodyParts.Any(x => x.IsAttachment))
                {
                    response += message.UniqueId + Environment.NewLine;
                    matched.Add(message.UniqueId);
                }                    
            }

            response += string.Format("Unread messages with attachments: {0}", matched.Count) + Environment.NewLine;

            client.Disconnect(true);

            return response;

        }

        // GET: api/<AttachmentController>/id
        [HttpGet("GetMessage/{id}")]
        public async Task<string> GetMessage(string id)
        {
            var response = "";

            var client = new ImapClient();
            client.Connect(_imapConfiguration.Server, _imapConfiguration.Port, _imapConfiguration.UseSSL);
            client.Authenticate(_imapConfiguration.Email, _imapConfiguration.Password);

            // The Inbox folder is always available on all IMAP servers...
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadWrite);

            uint.TryParse(id, out var uid);
            var uniqueId = new UniqueId(uid);
            response += inbox.GetMessage(uniqueId);

            //Mark as read
            inbox.AddFlags(uniqueId, MessageFlags.Seen, true);

            client.Disconnect(true);

            return response;

        }

        // GET: api/<AttachmentController>/id
        [HttpGet("GetAttachment/{id}")]
        public async Task<string> GetAttachment(string id)
        {
            var response = "";

            var client = new ImapClient();
            client.Connect(_imapConfiguration.Server, _imapConfiguration.Port, _imapConfiguration.UseSSL);
            client.Authenticate(_imapConfiguration.Email, _imapConfiguration.Password);

            // The Inbox folder is always available on all IMAP servers...
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadWrite);

            uint.TryParse(id, out var uid);

            var message = inbox.GetMessage(new UniqueId(uid));

            foreach (var attachment in message.Attachments)
            {
                var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;

                response += fileName + Environment.NewLine;

                /*
                using (var stream = File.Create(fileName))
                {
                    if (attachment is MessagePart)
                    {
                        var rfc822 = (MessagePart)attachment;

                        rfc822.Message.WriteTo(stream);
                    }
                    else
                    {
                        var part = (MimePart)attachment;

                        part.Content.DecodeTo(stream);
                    }
                }
                */
            }

            client.Disconnect(true);

            return response;

        }
    }
}
