namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Poco;
    using System.Threading;

    [Route("api/activedirectory")]
    public class ActiveDirectoryController : BaseController
    {
        private readonly IActiveDirectoryManager activeDirectoryManager;

        public ActiveDirectoryController(IActiveDirectoryManager activeDirectoryManager)
            : base()
        {
            this.activeDirectoryManager = activeDirectoryManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(ActiveDirectoryNewUserRequest activeDirectoryNewUserRequest, CancellationToken cancellationToken)
        {
            var response = await activeDirectoryManager.CreateUserAsync(activeDirectoryNewUserRequest, cancellationToken);

            return Ok(response);
        }
    }
}
