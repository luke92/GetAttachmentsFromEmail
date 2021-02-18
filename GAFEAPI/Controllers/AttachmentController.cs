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
using GAFEAPI.Services.Mail;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GAFEAPI.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IImapService _imapService;

        public AttachmentController(IImapService imapService)
        {
            _imapService = imapService;
        }

        // GET: api/<AttachmentController>
        [HttpGet]
        public async Task<string> Get()
        {
            var response = "";

            var results = await _imapService.GetMailList(true, true, null);

            foreach(var item in results)
            {
                response += item.ToString() + Environment.NewLine;
            }

            return response;

        }

        // GET: api/<AttachmentController>/id
        [HttpGet("GetMessage/{id}")]
        public async Task<string> GetMessage(string id)
        {
            return await _imapService.GetMessageContent(id,true);
        }

        // GET: api/<AttachmentController>/id
        [HttpGet("GetAttachment/{id}")]
        public async Task<IActionResult> GetAttachment(string id)
        {
            try
            {
                var mimeType = "application/pdf";
                var content = await _imapService.GetAttachmentContent(id, true);
                return File(content, contentType: mimeType);                                   
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
