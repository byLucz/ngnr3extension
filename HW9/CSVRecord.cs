namespace Inserter 
{
	internal class CSVRecord 
	{
		public string[] OrgPath { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string Position { get; set; }
		public List<CityMarker> Cities { get; set; }

		public static CSVRecord Parse(string line) 
		{
			var parts = line.Split(',');
			if (parts.Length < 4)
				return null;

			var orgPath = parts[0].Trim();
			var fio = parts[1].Trim();
			var pos = parts[2].Trim();
			var spec = parts[3].Trim();

			var pathSegments = orgPath.Split('\\').Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();
			var fioParts = fio.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			string firstName = fioParts[0];
			string middleName = fioParts[1];
			string lastName = fioParts[2];
			var cities = new List<CityMarker>();

			if (!string.IsNullOrEmpty(spec)) {
				var tokens = spec.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				foreach (var token in tokens) {
					var pair = token.Split(':');
					if (pair.Length != 2)
						continue;

					var cityName = pair[0].Trim();
					var markerStr = pair[1].Trim().ToLowerInvariant();
					bool marker = markerStr == "да";

					cities.Add(new CityMarker {
						CityName = cityName,
						Marker = marker
					});
				}
			}

			return new CSVRecord {
				OrgPath = pathSegments,
				LastName = lastName,
				FirstName = firstName,
				MiddleName = middleName,
				Position = pos,
				Cities = cities
			};
		}
	}

	internal class CityMarker 
	{
		public string CityName { get; set; }
		public bool Marker { get; set; }
	}
}
