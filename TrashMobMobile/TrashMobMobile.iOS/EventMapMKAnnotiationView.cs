namespace TrashMobMobile.iOS
{
	using MapKit;
	using System;

	public class EventMapMKAnnotationView : MKAnnotationView
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }

		public EventMapMKAnnotationView(IMKAnnotation annotation, string id)
			: base(annotation, id)
		{
		}
	}
}
