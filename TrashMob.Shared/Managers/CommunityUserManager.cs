
namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityUserManager : ExtendedManager<CommunityUser>, IExtendedManager<CommunityUser>
    {
        public CommunityUserManager(IRepository<CommunityUser> repository) : base(repository)
        {
        }
    }
}
