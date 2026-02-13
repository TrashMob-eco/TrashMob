namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for team operations.
    /// </summary>
    [Route("api/teams")]
    public class TeamsController(
        ITeamManager teamManager,
        ITeamMemberManager teamMemberManager,
        ITeamPhotoManager teamPhotoManager,
        IImageManager imageManager,
        IKeyedManager<User> userManager)
        : SecureController
    {
        /// <summary>
        /// Gets all public teams.
        /// </summary>
        /// <param name="latitude">Optional latitude for location filtering.</param>
        /// <param name="longitude">Optional longitude for location filtering.</param>
        /// <param name="radiusMiles">Optional radius in miles for location filtering.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicTeams(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] double? radiusMiles,
            CancellationToken cancellationToken)
        {
            var teams = await teamManager.GetPublicTeamsAsync(latitude, longitude, radiusMiles, cancellationToken);
            return Ok(teams);
        }

        /// <summary>
        /// Gets a team by ID.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{teamId}")]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeam(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null)
            {
                return NotFound();
            }

            // Private teams can only be viewed by members
            if (!team.IsPublic)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return NotFound();
                }

                var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
                if (!isMember)
                {
                    return NotFound();
                }
            }

            return Ok(team);
        }

        /// <summary>
        /// Gets all teams that the current user is a member of.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("my")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyTeams(CancellationToken cancellationToken)
        {
            var teams = await teamManager.GetTeamsByUserAsync(UserId, cancellationToken);
            return Ok(teams);
        }

        /// <summary>
        /// Gets all teams that the current user leads.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("my/leading")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeamsILead(CancellationToken cancellationToken)
        {
            var teams = await teamManager.GetTeamsUserLeadsAsync(UserId, cancellationToken);
            return Ok(teams);
        }

        /// <summary>
        /// Checks if a team name is available.
        /// </summary>
        /// <param name="name">The team name to check.</param>
        /// <param name="excludeTeamId">Optional team ID to exclude (for updates).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("check-name")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckTeamName(
            [FromQuery] string name,
            [FromQuery] Guid? excludeTeamId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Ok(false);
            }

            var isAvailable = await teamManager.IsTeamNameAvailableAsync(name, excludeTeamId, cancellationToken);
            return Ok(isAvailable);
        }

        /// <summary>
        /// Creates a new team. The creator becomes the first team lead.
        /// </summary>
        /// <param name="team">The team to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Team), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTeam([FromBody] Team team, CancellationToken cancellationToken)
        {
            // Check if team name is available
            var isAvailable = await teamManager.IsTeamNameAvailableAsync(team.Name, cancellationToken: cancellationToken);
            if (!isAvailable)
            {
                return BadRequest("A team with this name already exists.");
            }

            // Verify the user is an adult (18+) to create a team
            var user = await userManager.GetAsync(UserId, cancellationToken);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Set audit fields
            team.Id = Guid.NewGuid();
            team.IsActive = true;
            team.CreatedByUserId = UserId;
            team.CreatedDate = DateTimeOffset.UtcNow;
            team.LastUpdatedByUserId = UserId;
            team.LastUpdatedDate = DateTimeOffset.UtcNow;

            var createdTeam = await teamManager.AddAsync(team, UserId, cancellationToken);

            // Add creator as team lead
            await teamMemberManager.AddMemberAsync(createdTeam.Id, UserId, isTeamLead: true, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetTeam), new { teamId = createdTeam.Id }, createdTeam);
        }

        /// <summary>
        /// Updates a team. Only team leads can update a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="team">The updated team data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{teamId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTeam(Guid teamId, [FromBody] Team team, CancellationToken cancellationToken)
        {
            if (teamId != team.Id)
            {
                return BadRequest("Team ID mismatch.");
            }

            var existingTeam = await teamManager.GetAsync(teamId, cancellationToken);
            if (existingTeam == null)
            {
                return NotFound();
            }

            // Check if user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            // Check if name is being changed and is available
            if (!string.Equals(existingTeam.Name, team.Name, StringComparison.OrdinalIgnoreCase))
            {
                var isAvailable = await teamManager.IsTeamNameAvailableAsync(team.Name, teamId, cancellationToken);
                if (!isAvailable)
                {
                    return BadRequest("A team with this name already exists.");
                }
            }

            // Update fields
            existingTeam.Name = team.Name;
            existingTeam.Description = team.Description;
            existingTeam.LogoUrl = team.LogoUrl;
            existingTeam.IsPublic = team.IsPublic;
            existingTeam.RequiresApproval = team.RequiresApproval;
            existingTeam.Latitude = team.Latitude;
            existingTeam.Longitude = team.Longitude;
            existingTeam.City = team.City;
            existingTeam.Region = team.Region;
            existingTeam.Country = team.Country;
            existingTeam.PostalCode = team.PostalCode;
            existingTeam.LastUpdatedByUserId = UserId;
            existingTeam.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updatedTeam = await teamManager.UpdateAsync(existingTeam, UserId, cancellationToken);
            return Ok(updatedTeam);
        }

        /// <summary>
        /// Deactivates a team (soft delete). Only team leads can deactivate a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{teamId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateTeam(Guid teamId, CancellationToken cancellationToken)
        {
            var existingTeam = await teamManager.GetAsync(teamId, cancellationToken);
            if (existingTeam == null)
            {
                return NotFound();
            }

            // Check if user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            // Soft delete
            existingTeam.IsActive = false;
            existingTeam.LastUpdatedByUserId = UserId;
            existingTeam.LastUpdatedDate = DateTimeOffset.UtcNow;

            await teamManager.UpdateAsync(existingTeam, UserId, cancellationToken);
            return NoContent();
        }

        #region Team Photos

        /// <summary>
        /// Gets all photos for a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{teamId}/photos")]
        [ProducesResponseType(typeof(IEnumerable<TeamPhoto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeamPhotos(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null)
            {
                return NotFound();
            }

            // Private teams can only have photos viewed by members
            if (!team.IsPublic)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return NotFound();
                }

                var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
                if (!isMember)
                {
                    return NotFound();
                }
            }

            var photos = await teamPhotoManager.GetByTeamIdAsync(teamId, cancellationToken);
            return Ok(photos);
        }

        /// <summary>
        /// Uploads a photo for a team. Only team leads can upload photos.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="imageUpload">The image upload data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{teamId}/photos")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamPhoto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadTeamPhoto(
            Guid teamId,
            [FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null)
            {
                return NotFound();
            }

            // Check if user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            // Create the team photo record
            var photoId = Guid.NewGuid();
            var teamPhoto = new TeamPhoto
            {
                Id = photoId,
                TeamId = teamId,
                Caption = string.Empty,
            };

            // Upload to blob storage
            imageUpload.ParentId = photoId;
            imageUpload.ImageType = ImageTypeEnum.TeamPhoto;
            await imageManager.UploadImage(imageUpload);

            // Get the image URL and save photo record
            var imageUrl = await imageManager.GetImageUrlAsync(photoId, ImageTypeEnum.TeamPhoto, ImageSizeEnum.Reduced, cancellationToken);
            teamPhoto.ImageUrl = imageUrl;
            var createdPhoto = await teamPhotoManager.AddAsync(teamPhoto, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetTeamPhotos), new { teamId }, createdPhoto);
        }

        /// <summary>
        /// Deletes a team photo. Only team leads can delete photos.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{teamId}/photos/{photoId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeamPhoto(
            Guid teamId,
            Guid photoId,
            CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null)
            {
                return NotFound();
            }

            // Check if user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var photo = await teamPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo == null || photo.TeamId != teamId)
            {
                return NotFound();
            }

            // HardDelete removes from blob storage and database
            await teamPhotoManager.HardDeleteAsync(photoId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updates a team photo caption. Only team leads can update photos.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="caption">The new caption.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{teamId}/photos/{photoId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamPhoto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTeamPhotoCaption(
            Guid teamId,
            Guid photoId,
            [FromBody] string caption,
            CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null)
            {
                return NotFound();
            }

            // Check if user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var photo = await teamPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo == null || photo.TeamId != teamId)
            {
                return NotFound();
            }

            photo.Caption = caption ?? string.Empty;
            photo.LastUpdatedByUserId = UserId;
            photo.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updatedPhoto = await teamPhotoManager.UpdateAsync(photo, UserId, cancellationToken);
            return Ok(updatedPhoto);
        }

        #endregion

        #region Team Logo

        /// <summary>
        /// Uploads a logo for a team. Only team leads can upload logos.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="imageUpload">The image upload data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{teamId}/logo")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadTeamLogo(
            Guid teamId,
            [FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null)
            {
                return NotFound();
            }

            // Check if user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            // Delete existing logo if present
            if (!string.IsNullOrWhiteSpace(team.LogoUrl))
            {
                await imageManager.DeleteImage(teamId, ImageTypeEnum.TeamLogo);
            }

            // Upload new logo to blob storage
            imageUpload.ParentId = teamId;
            imageUpload.ImageType = ImageTypeEnum.TeamLogo;
            await imageManager.UploadImage(imageUpload);

            // Get the image URL and update team
            var logoUrl = await imageManager.GetImageUrlAsync(teamId, ImageTypeEnum.TeamLogo, ImageSizeEnum.Reduced, cancellationToken);
            team.LogoUrl = logoUrl;
            team.LastUpdatedByUserId = UserId;
            team.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updatedTeam = await teamManager.UpdateAsync(team, UserId, cancellationToken);
            return Ok(updatedTeam);
        }

        /// <summary>
        /// Deletes the team logo. Only team leads can delete logos.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{teamId}/logo")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeamLogo(
            Guid teamId,
            CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null)
            {
                return NotFound();
            }

            // Check if user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            // Delete logo from blob storage
            if (!string.IsNullOrWhiteSpace(team.LogoUrl))
            {
                await imageManager.DeleteImage(teamId, ImageTypeEnum.TeamLogo);
            }

            // Clear logo URL on team
            team.LogoUrl = string.Empty;
            team.LastUpdatedByUserId = UserId;
            team.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updatedTeam = await teamManager.UpdateAsync(team, UserId, cancellationToken);
            return Ok(updatedTeam);
        }

        #endregion
    }
}
