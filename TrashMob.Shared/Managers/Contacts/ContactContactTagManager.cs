namespace TrashMob.Shared.Managers.Contacts
{
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class ContactContactTagManager(IBaseRepository<ContactContactTag> repository)
        : BaseManager<ContactContactTag>(repository), IBaseManager<ContactContactTag>
    {
    }
}
