using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

            foreach (var messageSummary in inbox.Fetch(results, items))
            {
                //Has Attachment
                if (messageSummary.BodyParts.Any(x => x.IsAttachment))
                {
                    var message = await inbox.GetMessageAsync(messageSummary.UniqueId);
                    response += string.Format("{0} {1} {2}",messageSummary.UniqueId,message.Subject,Environment.NewLine);
                    matched.Add(messageSummary.UniqueId);
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
            var message = inbox.GetMessage(uniqueId);

            response += message.ToString();

            //Mark as read
            inbox.AddFlags(uniqueId, MessageFlags.Seen, true);

            client.Disconnect(true);

            return response;

        }

        // GET: api/<AttachmentController>/id
        [HttpGet("GetAttachment/{id}")]
        public async Task<IActionResult> GetAttachment(string id)
        {
            try
            {
                var client = new ImapClient();
                client.Connect(_imapConfiguration.Server, _imapConfiguration.Port, _imapConfiguration.UseSSL);
                client.Authenticate(_imapConfiguration.Email, _imapConfiguration.Password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);

                uint.TryParse(id, out var uid);

                var message = inbox.GetMessage(new UniqueId(uid));

                if (message.Attachments.Any())
                {
                    var attachment = message.Attachments.FirstOrDefault();

                    var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;

                    var stream = new MemoryStream();
                    
                    if (attachment is MessagePart)
                    {
                        var rfc822 = (MessagePart)attachment;

                        await rfc822.Message.WriteToAsync(stream);
                    }
                    else
                    {
                        var part = (MimePart)attachment;

                        await part.Content.DecodeToAsync(stream);
                    }

                    client.Disconnect(true);

                    var mimeType = "application/pdf";

                    var content = stream.ToArray();
                    return File(content, contentType: mimeType);
                                        
                }
                else
                {
                    return NotFound();
                }
                
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            

        }

        private void ReadMessage()
        {
            /*
            var message = folder.GetMessage(uid);

            // here's how to get the Subject
            string subject = message.Subject;

            // here's how to get the body (in the simple way)
            string text = message.TextBody;
            string html = message.HtmlBody;

            // here's how to get *just* attachments:
            foreach (MimeEntity attachment in message.Attachments)
            {
                // ...
            }

            // here's how to get both attachments and inline attachments:
            foreach (MimeEntity attachment in message.BodyParts)
            {
                // ...
            }
            */
        }
    }
}
