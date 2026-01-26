namespace TrashMob.Controllers;

using Microsoft.AspNetCore.Mvc;
using TrashMob.Models;
using TrashMob.Shared.Managers.Interfaces;

/// <summary>
/// Controller for managing weight unit lookup data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WeightUnitsController"/> class.
/// </remarks>
/// <param name="manager">The weight unit lookup manager.</param>
[Route("api/weightunits")]
public class WeightUnitsController(ILookupManager<WeightUnit> manager) : LookupController<WeightUnit>(manager)
{
}