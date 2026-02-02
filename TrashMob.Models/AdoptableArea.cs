#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a geographic area available for team adoption (highway, park, trail, etc.).
    /// </summary>
    /// <remarks>
    /// Adoptable areas are created by community admins and can be adopted by teams for regular cleanup.
    /// Each area has requirements (cleanup frequency, minimum events per year) and safety guidelines.
    /// Areas can be defined using GeoJSON polygons or linestrings for geographic boundaries.
    /// </remarks>
    public class AdoptableArea : KeyedModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdoptableArea"/> class.
        /// </summary>
        public AdoptableArea()
        {
            Adoptions = [];
        }

        /// <summary>
        /// Gets or sets the community (partner) this area belongs to.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the area name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the area description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of area (Highway, Park, Trail, Waterway, Street, Spot).
        /// </summary>
        public string AreaType { get; set; }

        /// <summary>
        /// Gets or sets the adoption status (Available, Adopted, Unavailable).
        /// </summary>
        public string Status { get; set; } = "Available";

        #region Geographic Definition

        /// <summary>
        /// Gets or sets the GeoJSON representation of the area boundary/route.
        /// </summary>
        /// <remarks>
        /// Can be a Polygon for areas or LineString for routes (trails, highways).
        /// </remarks>
        public string GeoJson { get; set; }

        /// <summary>
        /// Gets or sets the start point latitude (for linear areas like roads/trails).
        /// </summary>
        public double? StartLatitude { get; set; }

        /// <summary>
        /// Gets or sets the start point longitude (for linear areas).
        /// </summary>
        public double? StartLongitude { get; set; }

        /// <summary>
        /// Gets or sets the end point latitude (for linear areas).
        /// </summary>
        public double? EndLatitude { get; set; }

        /// <summary>
        /// Gets or sets the end point longitude (for linear areas).
        /// </summary>
        public double? EndLongitude { get; set; }

        #endregion

        #region Requirements

        /// <summary>
        /// Gets or sets how often cleanup is required (in days).
        /// </summary>
        public int CleanupFrequencyDays { get; set; } = 90;

        /// <summary>
        /// Gets or sets the minimum events required per year.
        /// </summary>
        public int MinEventsPerYear { get; set; } = 4;

        /// <summary>
        /// Gets or sets safety requirements and guidelines.
        /// </summary>
        public string SafetyRequirements { get; set; }

        #endregion

        #region Configuration

        /// <summary>
        /// Gets or sets whether this area can be co-adopted by multiple teams.
        /// </summary>
        public bool AllowCoAdoption { get; set; } = false;

        /// <summary>
        /// Gets or sets whether this area is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets or sets the community (partner) that owns this area.
        /// </summary>
        public virtual Partner Partner { get; set; }

        /// <summary>
        /// Gets or sets the collection of team adoptions for this area.
        /// </summary>
        public virtual ICollection<TeamAdoption> Adoptions { get; set; }

        #endregion
    }
}
