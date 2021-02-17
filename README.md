# GetAttachmentsFromEmail
- Login in Email and get attachments of unread emails

# Requirements
- MailKit

# Config for Gmail
- Username and password of the user mentioned in the property panel of IMAP mail activity
- In ur gmail Accounts settings Turn on “Allow Secure apps” and try the workflow.
- also make sure that IMAP is enables
- In gmail you need to turn on the “Less secure app access”, this will allow UIPath to access gmail.
- You can turn this on from the following: Google Account settings > Security > Less secure app access > on

# Remember for google
- Put the credentials.json
- Drag credentials.json (downloaded in Step 1) into your Visual Studio Solution Explorer.
- Select credentials.json, and then go to the Properties window and set the Copy to Output Directory field to Copy always.

# Links for reference
## Google
- https://developers.google.com/gmail/api/quickstart/dotnet
- https://github.com/googleworkspace/dotnet-samples/blob/master/gmail/GmailQuickstart/GmailQuickstart.cs
- https://developers.google.com/api-client-library/dotnet
- https://developers.google.com/gmail/api/reference/rest

## Others resources (POP - IMAP)
- https://www.limilabs.com/blog/download-email-attachments-net
- http://hpop.sourceforge.net/
- https://stackoverflow.com/questions/4006896/check-for-unread-emails
- https://social.msdn.microsoft.com/Forums/vstudio/en-US/bb9b6dd8-7a27-4c4f-b4f9-bde788a2eec3/how-do-i-download-unread-email-and-attachment-using-pop-or-imap?forum=csharpgeneral
- https://www.limilabs.com/blog/receive-unseen-emails-using-imap

# Deploy net core web api in heroku
- https://jakubwajs.wordpress.com/2020/01/31/deploying-asp-net-core-3-1-web-api-to-heroku-with-docker/
