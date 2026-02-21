namespace TrashMob.Shared.Managers.Areas
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IAreaSuggestionService
    {
        Task<AreaSuggestionResult> SuggestAreaAsync(
            AreaSuggestionRequest request, CancellationToken cancellationToken = default);
    }
}
