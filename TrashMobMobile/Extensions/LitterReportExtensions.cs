namespace TrashMobMobile.Extensions
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMobMobile.Services;

    public static class LitterReportExtensions
    {
        public static string GetLitterStatusFromId(int? id)
        {
            return id == null ? string.Empty : Enum.GetName(typeof(LitterReportStatusEnum), id.Value) ?? string.Empty;
        }

        public static LitterReportViewModel ToLitterReportViewModel(this LitterReport litterReport, INotificationService notificationService)
        {
            var litterReportViewModel = new LitterReportViewModel
            {
                Id = litterReport.Id,
                Name = litterReport.Name,
                Description = litterReport.Description,
                LitterReportStatusId = litterReport.LitterReportStatusId,
            };

            if (litterReport.CreatedDate != null)
            {
                litterReportViewModel.CreatedDate = litterReport.CreatedDate.Value.GetFormattedLocalDate();
            }
            else
            {
                litterReport.CreatedDate = DateTime.MinValue;
            }

            if (litterReport.LitterImages == null)
            {
                litterReport.LitterImages = [];
            }
            else
            {
                foreach (var litterImage in litterReport.LitterImages)
                {
                    var litterImageViewModel = litterImage.ToLitterImageViewModel(notificationService);
                    if (litterImageViewModel != null)
                    {
                        litterImageViewModel.Address.DisplayName = litterReport.Name;
                        litterImageViewModel.Address.ParentId = litterReport.Id;
                        litterReportViewModel.LitterImageViewModels.Add(litterImageViewModel);
                    }
                }
            }

            return litterReportViewModel;
        }

        public static EventLitterReportViewModel ToEventLitterReportViewModel(this FullLitterReport litterReport, 
                                                                                   INotificationService notificationService, 
                                                                                   IEventLitterReportRestService eventLitterReportRestService, 
                                                                                   Guid eventId)
        {
            var litterReportViewModel = new EventLitterReportViewModel(eventLitterReportRestService, eventId)
            {
                Id = litterReport.Id,
                Name = litterReport.Name,
                Description = litterReport.Description,
                LitterReportStatusId = litterReport.LitterReportStatusId,
                CreatedDate = litterReport.CreatedDate?.GetFormattedLocalDate() ?? string.Empty,
            };

            foreach (var litterImage in litterReport.LitterImages)
            {
                var litterImageViewModel = litterImage.ToLitterImageViewModel(notificationService);
                if (litterImageViewModel != null)
                {
                    litterImageViewModel.Address.DisplayName = litterReport.Name;
                    litterImageViewModel.Address.ParentId = litterReport.Id;
                    litterReportViewModel.LitterImageViewModels.Add(litterImageViewModel);
                }
            }

            return litterReportViewModel;
        }

        public static EventLitterReportViewModel ToEventLitterReportViewModel(this LitterReport litterReport,
                                                                           INotificationService notificationService,
                                                                           IEventLitterReportRestService eventLitterReportRestService,
                                                                           Guid eventId)
        {
            var litterReportViewModel = new EventLitterReportViewModel(eventLitterReportRestService, eventId)
            {
                Id = litterReport.Id,
                Name = litterReport.Name,
                Description = litterReport.Description,
                LitterReportStatusId = litterReport.LitterReportStatusId,
                CreatedDate = litterReport.CreatedDate?.GetFormattedLocalDate() ?? string.Empty,
            };

            foreach (var litterImage in litterReport.LitterImages)
            {
                var litterImageViewModel = litterImage.ToLitterImageViewModel(notificationService);
                if (litterImageViewModel != null)
                {
                    litterImageViewModel.Address.DisplayName = litterReport.Name;
                    litterImageViewModel.Address.ParentId = litterReport.Id;
                    litterReportViewModel.LitterImageViewModels.Add(litterImageViewModel);
                }
            }

            return litterReportViewModel;
        }

        public static EventLitterReportViewModel ToEventLitterReportViewModel(this EventLitterReport eventLitterReport,
                                                                              INotificationService notificationService,
                                                                              IEventLitterReportRestService eventLitterReportRestService,
                                                                              Guid eventId)
        {
            var eventLitterReportViewModel = new EventLitterReportViewModel(eventLitterReportRestService, eventId)
            {
                Id = eventLitterReport.LitterReportId,
                Name = eventLitterReport.LitterReport.Name,
                Description = eventLitterReport.LitterReport.Description,
                LitterReportStatusId = eventLitterReport.LitterReport.LitterReportStatusId,
                CreatedDate = eventLitterReport.CreatedDate?.GetFormattedLocalDate() ?? string.Empty,
            };

            foreach (var litterImage in eventLitterReport.LitterReport.LitterImages)
            {
                var litterImageViewModel = litterImage.ToLitterImageViewModel(notificationService);
                if (litterImageViewModel != null)
                {
                    litterImageViewModel.Address.DisplayName = eventLitterReport.LitterReport.Name;
                    litterImageViewModel.Address.ParentId = eventLitterReport.LitterReport.Id;
                    eventLitterReportViewModel.LitterImageViewModels.Add(litterImageViewModel);
                }
            }

            return eventLitterReportViewModel;
        }

        public static LitterReport ToLitterReport(this LitterReportViewModel litterReportViewModel)
        {
            return new LitterReport
            {
                Id = litterReportViewModel.Id,
                Name = litterReportViewModel.Name,
                Description = litterReportViewModel.Description,
                LitterReportStatusId = litterReportViewModel.LitterReportStatusId,
                LitterImages = [],
            };
        }

        public static LitterImage ToLitterImage(this LitterImageViewModel litterImageViewModel)
        {
            return new LitterImage
            {
                Id = litterImageViewModel.Id,
                LitterReportId = litterImageViewModel.LitterReportId,
            };
        }

        public static LitterImageViewModel? ToLitterImageViewModel(this LitterImage litterImage, INotificationService notificationService)
        {
            if (litterImage?.Latitude == null || litterImage.Longitude == null)
            {
                return null;
            }

            return new LitterImageViewModel(notificationService)
            {
                Id = litterImage.Id,
                LitterReportId = litterImage.LitterReportId,
                AzureBlobUrl = litterImage.AzureBlobURL,
                CreatedByUserId = litterImage.CreatedByUserId,
                LastUpdatedByUserId = litterImage.LastUpdatedByUserId,
                CreatedDate = litterImage.CreatedDate,
                LastUpdatedDate = litterImage.LastUpdatedDate,
                Address = new AddressViewModel
                {
                    AddressType = AddressType.LitterImage,
                    City = litterImage.City,
                    Country = litterImage.Country,
                    Latitude = litterImage.Latitude,
                    Longitude = litterImage.Longitude,
                    PostalCode = litterImage.PostalCode,
                    Region = litterImage.Region,
                    StreetAddress = litterImage.StreetAddress,
                    Location = new Microsoft.Maui.Devices.Sensors.Location(litterImage.Latitude.Value, litterImage.Longitude.Value),
                },
            };
        }

        public static LitterImageViewModel? ToLitterImageViewModel(this FullLitterImage litterImage, INotificationService notificationService)
        {
            if (litterImage?.Latitude == null || litterImage.Longitude == null)
            {
                return null;
            }

            return new LitterImageViewModel(notificationService)
            {
                Id = litterImage.Id,
                LitterReportId = litterImage.LitterReportId,
                AzureBlobUrl = litterImage.ImageURL,
                CreatedByUserId = litterImage.CreatedByUserId,
                LastUpdatedByUserId = litterImage.LastUpdatedByUserId,
                CreatedDate = litterImage.CreatedDate,
                LastUpdatedDate = litterImage.LastUpdatedDate,
                Address = new AddressViewModel
                {
                    AddressType = AddressType.LitterImage,
                    City = litterImage.City,
                    Country = litterImage.Country,
                    Latitude = litterImage.Latitude,
                    Longitude = litterImage.Longitude,
                    PostalCode = litterImage.PostalCode,
                    Region = litterImage.Region,
                    StreetAddress = litterImage.StreetAddress,
                    Location = new Microsoft.Maui.Devices.Sensors.Location(litterImage.Latitude.Value, litterImage.Longitude.Value),
                },
            };
        }
    }
}