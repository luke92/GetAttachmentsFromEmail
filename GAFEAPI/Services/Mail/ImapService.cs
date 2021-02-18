using GAFEAPI.Models;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GAFEAPI.Services.Mail
{
    public interface IImapService
    {
        Task<IList<UniqueId>> GetMailList(bool unread = false, bool notDeleted = false, string subject = null, bool onlyWithAttachments = false);

        Task<string> GetMessageContent(string id, bool markAsRead = false);

        Task<byte[]> GetAttachmentContent(string id, bool markAsRead = false);
    }
    public class ImapService : IImapService
    {
        private readonly IMAPConfiguration _imapConfiguration;

        public ImapService(IMAPConfiguration imapConfiguration)
        {
            _imapConfiguration = imapConfiguration;
        }

        public async Task<IList<UniqueId>> GetMailList(bool unread = false, bool notDeleted = false, string subject = null, bool onlyWithAttachments = false)
        {
            var client = await OpenClient();

            // The Inbox folder is always available on all IMAP servers...
            var folder = client.Inbox;
            await folder.OpenAsync(FolderAccess.ReadOnly);

            var query = SearchQuery.All;

            if (unread)
            {
                query = query.And(SearchQuery.NotSeen);
            }

            if (notDeleted)
            {
                query = query.And(SearchQuery.NotDeleted);
            }

            if (!string.IsNullOrEmpty(subject))
            {
                query = query.And(SearchQuery.SubjectContains(subject));
            }

            var list = await folder.SearchAsync(query);

            if (onlyWithAttachments)
            {
                // Get Items
                var items = MessageSummaryItems.BodyStructure | MessageSummaryItems.UniqueId;
                var matched = new UniqueIdSet();
                foreach (var messageSummary in folder.Fetch(list, items))
                {
                    //Has Attachment
                    if (messageSummary.BodyParts.Any(x => x.IsAttachment))
                    {
                        matched.Add(messageSummary.UniqueId);
                    }
                }
                await CloseClient(client);
                return matched;
            }
            else
            {
                await CloseClient(client);
                return list;
            }
        }

        public async Task<string> GetMessageContent(string id, bool markAsRead = false)
        {
            var client = await OpenClient();

            // The Inbox folder is always available on all IMAP servers...
            var folder = client.Inbox;
            await folder.OpenAsync(FolderAccess.ReadWrite);

            var uniqueId = GetUid(id);
            var message = await folder.GetMessageAsync(uniqueId);

            //var subject = message.Subject;
            var response = message.ToString();

            if (markAsRead)
            {
                await MarkAsRead(folder, uniqueId);
            }

            await CloseClient(client);

            return response;
        }

        public async Task<byte[]> GetAttachmentContent (string id, bool markAsRead = false)
        {
            var client = await OpenClient();

            // The Inbox folder is always available on all IMAP servers...
            var folder = client.Inbox;
            await folder.OpenAsync(FolderAccess.ReadWrite);

            var uniqueId = GetUid(id);
            var message = folder.GetMessage(uniqueId);

            if (message.Attachments.Any())
            {
                var attachment = message.Attachments.FirstOrDefault();

                //Filename of Attachment
                //var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;

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

                if (markAsRead)
                {
                    await MarkAsRead(folder, uniqueId);
                }

                client.Disconnect(true);

                return stream.ToArray();

            }
            else
            {
                client.Disconnect(true);
                return null;
            }
        }

        private async Task<ImapClient> OpenClient()
        {
            var client = new ImapClient();
            await client.ConnectAsync(_imapConfiguration.Server, _imapConfiguration.Port, _imapConfiguration.UseSSL);
            await client.AuthenticateAsync(_imapConfiguration.Email, _imapConfiguration.Password);
            return client;
        }

        private async Task CloseClient(ImapClient client)
        {
            await client.DisconnectAsync(true);
        }

        private UniqueId GetUid(string id)
        {
            uint.TryParse(id, out var uid);
            var uniqueId = new UniqueId(uid);
            return uniqueId;
        }

        private async Task MarkAsRead(IMailFolder folder, UniqueId uniqueId)
        {
            await folder.AddFlagsAsync(uniqueId, MessageFlags.Seen, true);
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
