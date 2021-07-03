namespace TrashMob.Shared.Engine
{
    using System.Collections.Generic;
    using System.Text;
    using TrashMob.Shared.Models;

    public static class EmailFormatter
    {
        public static string PopulateTemplate(string template, User user, Event mobEvent)
        {
            var populatedTemplate = template;
            populatedTemplate.Replace("{UserName}", user.UserName);
            populatedTemplate.Replace("{EventName}", mobEvent.Name);
            populatedTemplate.Replace("{EventDate}", mobEvent.EventDate.ToString());
            populatedTemplate.Replace("{EventStreet}", mobEvent.StreetAddress);
            populatedTemplate.Replace("{EventCity}", mobEvent.City);
            populatedTemplate.Replace("{EventRegion}", mobEvent.Region);
            populatedTemplate.Replace("{EventCountry}", mobEvent.Country);
            return populatedTemplate;
        }

        public static string PopulateTemplate(string template, User user, IEnumerable<Event> mobEvents)
        {
            var populatedTemplate = template;
            populatedTemplate.Replace("{UserName}", user.UserName);

            var eventGrid = new StringBuilder();
            eventGrid.AppendLine("<table>");
            eventGrid.AppendLine("<th>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event Name");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event Date");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event Address");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event City");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event Region");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event Country");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event Link");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("</th>");

            foreach (var mobEvent in mobEvents)
            {
                eventGrid.AppendLine("<tr>");
                eventGrid.AppendLine("<td>");
                eventGrid.AppendLine(mobEvent.Name);
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("<td>");
                eventGrid.AppendLine(mobEvent.EventDate.ToString());
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("<td>");
                eventGrid.AppendLine(mobEvent.StreetAddress);
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("<td>");
                eventGrid.AppendLine(mobEvent.City);
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("<td>");
                eventGrid.AppendLine(mobEvent.Region);
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("<td>");
                eventGrid.AppendLine(mobEvent.Country);
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("<td>");
                eventGrid.AppendFormat("https://www.trashmob.eco/eventdetails/{0}", mobEvent.Id);
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("</tr>");
            }
            eventGrid.AppendLine("</table>");

            populatedTemplate.Replace("{EventGrid}", eventGrid.ToString());

            return populatedTemplate;
        }
    }
}
