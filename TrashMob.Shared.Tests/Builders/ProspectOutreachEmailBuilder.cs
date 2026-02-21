namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    public class ProspectOutreachEmailBuilder
    {
        private readonly ProspectOutreachEmail _email;

        public ProspectOutreachEmailBuilder()
        {
            var creatorId = Guid.NewGuid();
            _email = new ProspectOutreachEmail
            {
                Id = Guid.NewGuid(),
                ProspectId = Guid.NewGuid(),
                CadenceStep = 1,
                Subject = "Test Subject",
                HtmlBody = "<p>Test body</p>",
                Status = "Sent",
                SentDate = DateTimeOffset.UtcNow,
                CreatedByUserId = creatorId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = creatorId,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };
        }

        public ProspectOutreachEmailBuilder WithId(Guid id)
        {
            _email.Id = id;
            return this;
        }

        public ProspectOutreachEmailBuilder WithProspectId(Guid prospectId)
        {
            _email.ProspectId = prospectId;
            return this;
        }

        public ProspectOutreachEmailBuilder WithCadenceStep(int step)
        {
            _email.CadenceStep = step;
            return this;
        }

        public ProspectOutreachEmailBuilder WithStatus(string status)
        {
            _email.Status = status;
            return this;
        }

        public ProspectOutreachEmail Build() => _email;
    }
}
