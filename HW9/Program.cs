using System.Configuration;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.ObjectModel.Search;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.CardLib.CardDefs;
using ObjectContext = DocsVision.Platform.ObjectModel.ObjectContext;
using Npgsql;

namespace Inserter 
{
	internal class Program 
	{
		static void Main(string[] args) 
		{
			var dvUrl = ConfigurationManager.AppSettings["DVUrl"];
			var dvUser = ConfigurationManager.AppSettings["Username"];
			var dvPassword = ConfigurationManager.AppSettings["Password"];
			var pgConnString = ConfigurationManager.AppSettings["DVDB"];
			var csvPath = ConfigurationManager.AppSettings["CSVPath"];

			Console.WriteLine("Импорт в бд из файла " + csvPath);
			Console.WriteLine();

			var sessionManager = SessionManager.CreateInstance();
			sessionManager.Connect(dvUrl, string.Empty, dvUser, dvPassword);
			var userSession = sessionManager.CreateSession();
			var objectContext = ContextFactory.CreateContext(userSession);
			var staffService = objectContext.GetService<IStaffService>();
			var staff = objectContext.GetObject<Staff>(RefStaff.ID);
			var cityCache = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

			using var pg = new NpgsqlConnection(pgConnString);
			pg.Open();
			using var cmd = pg.CreateCommand();
			cmd.CommandText = @"INSERT INTO public.""checkSpecEmplCity"" (""Employee"", ""City"", ""Marker"")
								VALUES (@empl, @city, @marker)
								ON CONFLICT (""Employee"", ""City"")
								DO UPDATE SET ""Marker"" = EXCLUDED.""Marker"";";
			cmd.CommandTimeout = 0;
			cmd.Parameters.Add(new NpgsqlParameter("empl", NpgsqlTypes.NpgsqlDbType.Uuid));
			cmd.Parameters.Add(new NpgsqlParameter("city", NpgsqlTypes.NpgsqlDbType.Uuid));
			cmd.Parameters.Add(new NpgsqlParameter("marker", NpgsqlTypes.NpgsqlDbType.Boolean));

			int num = 0;

			foreach (var line in File.ReadLines(csvPath)) {
				num++;
				if (string.IsNullOrWhiteSpace(line))
					continue;

				CSVRecord record;
				try {
					record = CSVRecord.Parse(line);
					if (record == null) {
						Console.WriteLine($"Строка {num}:неверный формат");
						continue;
					}
				} 
				catch (Exception ex) {
					Console.WriteLine($"Строка {num}:{ex.Message}");
					continue;
				}

				try {
					var unit = CheckPath(objectContext, staffService, staff, record.OrgPath);
					var position = CheckPos(objectContext, staffService, staff, record.Position);
					var employee = CheckEmpl(objectContext, staffService, unit, position, record.LastName, record.FirstName, record.MiddleName);

					CheckSpecCities(objectContext, cmd, employee, record.Cities, cityCache);

					Console.WriteLine($"Строка {num}:OK");
				} 
				catch (Exception ex) {
					Console.WriteLine($"Строка {num}:ERR {ex.Message}");
				}
			}

			objectContext.AcceptChanges();
			userSession.Close();

			Console.WriteLine();
			Console.WriteLine("Импорт завершен");
		}

		static StaffUnit CheckPath(ObjectContext objectContext, IStaffService staffService, Staff staff, string[] pathSegments) 
		{
			var current = staff.Units.FirstOrDefault(u => string.Equals(u.Name, pathSegments[0], StringComparison.CurrentCultureIgnoreCase));

			if (current == null) {
				current = staffService.AddNewUnit(null);
				current.Name = pathSegments[0];
				current.Type = StaffUnitType.Organization;
				objectContext.SaveObject(current);
			}

			for (int i = 1; i < pathSegments.Length; i++) {
				var name = pathSegments[i];

				var child = current.Units.FirstOrDefault(u => string.Equals(u.Name, name, StringComparison.CurrentCultureIgnoreCase));

				if (child == null) {
					child = staffService.AddNewUnit(current);
					child.Name = name;
					child.Type = StaffUnitType.Department;
					objectContext.SaveObject(child);
				}

				current = child;
			}

			return current;
		}

		static StaffPosition CheckPos(ObjectContext objectContext, IStaffService staffService, Staff staff, string positionName) 
		{
			var existing = staff.Positions
				.FirstOrDefault(p => string.Equals(p.Name, positionName, StringComparison.CurrentCultureIgnoreCase));

			if (existing != null)
				return existing;

			var position = staffService.AddNewPosition();
			position.Name = positionName;
			position.ShortName = positionName;

			objectContext.SaveObject(position);
			return position;
		}

		static StaffEmployee CheckEmpl(ObjectContext objectContext, IStaffService staffService, StaffUnit unit, StaffPosition position, 
											string lastName, string firstName, string middleName) 
		{
			var existing = unit.Employees.FirstOrDefault(e =>
				string.Equals(e.LastName, lastName, StringComparison.CurrentCultureIgnoreCase) &&
				string.Equals(e.FirstName, firstName, StringComparison.CurrentCultureIgnoreCase) &&
				string.Equals(e.MiddleName, middleName, StringComparison.CurrentCultureIgnoreCase));

			if (existing != null)
				return existing;

			var employee = staffService.AddNewEmployee(unit);

			employee.LastName = lastName;
			employee.FirstName = firstName;
			employee.MiddleName = middleName;

			if (position != null)
				employee.Position = position;

			objectContext.SaveObject(employee);
			return employee;
		}

		static void CheckSpecCities(ObjectContext objectContext, NpgsqlCommand cmd, StaffEmployee employee, List<CityMarker> cityMarkers,
										Dictionary<string, Guid> cityCache) 
		{
			if (cityMarkers == null || cityMarkers.Count == 0)
				return;

			var employeeId = objectContext.GetObjectRef(employee).Id;

			foreach (var cm in cityMarkers) {
				if (string.IsNullOrWhiteSpace(cm.CityName))
					continue;

				if (!cityCache.TryGetValue(cm.CityName, out var cityId)) {
					var cityItem = objectContext.FindObject<BaseUniversalItem>(
						new QueryObject(BaseUniversalItem.NameProperty.Name, cm.CityName));

					if (cityItem == null) {
						cityCache[cm.CityName] = Guid.Empty;
						continue;
					}

					cityId = objectContext.GetObjectRef(cityItem).Id;
					cityCache[cm.CityName] = cityId;
				}

				if (cityId == Guid.Empty)
					continue;

				cmd.Parameters["empl"].Value = employeeId;
				cmd.Parameters["city"].Value = cityId;
				cmd.Parameters["marker"].Value = cm.Marker;

				cmd.ExecuteNonQuery();
			}
		}
	}
}
