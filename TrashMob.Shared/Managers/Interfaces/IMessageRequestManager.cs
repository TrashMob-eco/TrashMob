namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for sending message requests.
    /// </summary>
    public interface IMessageRequestManager
    {
        /// <summary>
        /// Sends a message request.
        /// </summary>
        /// <param name="messageRequest">The message request to send.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendMessageRequestAsync(MessageRequest messageRequest);
    }
}
