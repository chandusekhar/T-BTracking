using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.Uow;

namespace BugTracking.SendMail
{
    public class EmailSending : AsyncBackgroundJob<EmailSendingArgs>, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        public EmailSending(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        [UnitOfWork]
        public override async Task ExecuteAsync(EmailSendingArgs args)
        {
            await _emailSender.SendAsync(args.EmailAddress, args.Subject, args.Body);
        }
    }
}
