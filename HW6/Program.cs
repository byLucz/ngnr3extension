using System.Configuration;
using DocsVision.BackOffice.CardLib.CardDefs;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.ObjectModel;
using DocsVision.Platform.ObjectModel.Search;
using DocumentFormat.OpenXml.Drawing;

namespace BTGen {
	internal class Program {
		static string TripKindName = "Заявка на командировку";
		static string TargetEndStateName = "Approving";

		static string[] EmplPool = { "ENGINEER\\kolesnikovaPD", "ENGINEER\\lebedevSD", "ENGINEER\\kuznecovIT", "ENGINEER\\petrovaSP" };
		static string[] CheckPool = { "ENGINEER\\petrovaSP", "ENGINEER\\kuznecovIT", "ENGINEER\\lebedevSD", "ENGINEER\\kolesnikovaPD" };
		static string[] RegPool = { "ENGINEER\\lebedevSD", "ENGINEER\\kolesnikovaPD", "ENGINEER\\petrovaSP", "ENGINEER\\kuznecovIT" };
		static string[] PartnerNames = { "test", "MMWAVE", "AcneStudios" };
		static string[] CityNames = { "Санкт-Петербург", "Москва", "Новосибирск" };

		static string[] FilePaths =
		{
			@"C:\Users\admin\Downloads\test1.docx",
			@"C:\Users\admin\Downloads\test2.pdf",
			@"C:\Users\admin\Downloads\test3.docx"
		};

		static void Main(string[] args) {
			var serverURL = ConfigurationManager.AppSettings["DVUrl"];
			var username = ConfigurationManager.AppSettings["Username"];
			var password = ConfigurationManager.AppSettings["Password"];
			var sm = SessionManager.CreateInstance();
			sm.Connect(serverURL, string.Empty, username, password);
			var session = sm.CreateSession();
			var ctx = ContextFactory.CreateContext(session);
			var rnd = new Random();

			while (true) {
				Console.WriteLine();
				Console.WriteLine("1. Создать заявку");
				Console.WriteLine("2. Создать множество заявок");
				Console.WriteLine("3. Выход");
				var z = Console.ReadLine();

				if (z == "1") {
					var id = CreateNewBT(
						ctx, session,
						EmplPool[0], CheckPool[1], RegPool[2],
						PartnerNames[0], CityNames[0],
						DateTime.Today.AddDays(3), DateTime.Today.AddDays(7),
						25000m, 1, "Командировка", "X",
						FilePaths[0], randomState: false,
						rnd: rnd
					);
					Console.WriteLine("Создан документ: " + id);
				} else if (z == "2") {
					Console.Write("Количество: ");
					var input = Console.ReadLine();
					int n = 100;
					int.TryParse(input, out n);
					if (n < 1)
						n = 1;

					for (int i = 0; i < n; i++) {
						var emplAcc = EmplPool[i % EmplPool.Length];
						var checkAcc = CheckPool[(i + 1) % CheckPool.Length];
						var regAcc = RegPool[(i + 2) % RegPool.Length];
						var partner = PartnerNames[(i + 1) % PartnerNames.Length];
						var city = CityNames[(i + 2) % CityNames.Length];
						var offset = rnd.Next(1, 30);
						var days = rnd.Next(3, 10);
						var from = DateTime.Today.AddDays(offset);
						var to = from.AddDays(days);
						var sum = rnd.Next(100, 500) * 100m;
						var tickets = rnd.Next(0, 2);
						var form = rnd.Next(0, 2) == 0 ? "Встреча с партнерами" : "Конференция";
						var mainFile = FilePaths[rnd.Next(FilePaths.Length)];

						CreateNewBT(ctx, session, emplAcc, checkAcc, regAcc,
								   partner, city, from, to, sum, tickets, 
								   form, $"{i + 1}", mainFile, 
								   randomState: true, rnd: rnd);
					}
					Console.WriteLine($"Создано {n} карточек");
				} else if (z == "3")
					break;
			}

			session.Close();
		}

		static Guid CreateNewBT(ObjectContext ctx, UserSession session,
			string emplAccount, string checkAccount, string regAccount,
			string partnerName, string cityName,
			DateTime from, DateTime to, decimal sum,
			int tickets, string form, string counter,
			string mainFilePath, bool randomState, Random rnd) {
			var docSvc = ctx.GetService<IDocumentService>();
			var staffSvc = ctx.GetService<IStaffService>();
			var stateSvc = ctx.GetService<IStateService>();

			var curEmpl = staffSvc.GetCurrentEmployee();
			var kind = ctx.FindObject<KindsCardKind>(new QueryObject(KindsCardKind.NameProperty.Name, TripKindName));
			var emplOut = staffSvc.FindEmpoyeeByAccountName(emplAccount);
			var check = staffSvc.FindEmpoyeeByAccountName(checkAccount);
			var regOut = staffSvc.FindEmpoyeeByAccountName(regAccount);
			var mngrOut = staffSvc.GetEmployeeManager(emplOut);
			var partner = ctx.FindObject<PartnersCompany>(new QueryObject(PartnersCompany.NameProperty.Name, partnerName));
			var cityRow = ctx.FindObject<BaseUniversalItem>(new QueryObject(BaseUniversalItem.NameProperty.Name, cityName));
			var doc = docSvc.CreateDocument(null, kind);

			doc.MainInfo.Author = curEmpl;
			doc.MainInfo.Registrar = curEmpl;
			doc.MainInfo[CardDocument.MainInfo.RegDate] = DateTime.Now;
			doc.MainInfo.Name = $"Командировка {emplOut.AccountName} // {counter} // {DateTime.Now:dd.MM.yyyy HH:mm}";
			doc.MainInfo["emplOut"] = emplOut.GetObjectId();
			doc.MainInfo["regOut"] = regOut.GetObjectId();
			doc.MainInfo["mngrOut"] = mngrOut.GetObjectId();
			doc.MainInfo["checkEmplOut"] = check.GetObjectId();
			doc.MainInfo["phoneOut"] = emplOut.Phone;
			doc.MainInfo["dateOut"] = from;
			doc.MainInfo["dateIn"] = to;
			doc.MainInfo["daysOut"] = (to - from).Days;
			doc.MainInfo["formOut"] = form;
			doc.MainInfo["sumOut"] = sum;
			doc.MainInfo["ticketsOut"] = tickets;

			if (partner != null)
				doc.MainInfo["orgOut"] = partner.GetObjectId();
			if (cityRow != null)
				doc.MainInfo["city"] = cityRow.GetObjectId();

			ctx.SaveObject(doc);

			if (!string.IsNullOrEmpty(mainFilePath) && File.Exists(mainFilePath))
				docSvc.AddMainFile(doc, mainFilePath);

			if (!randomState) {
				var branch = stateSvc.FindLineBranchesByStartState(doc.SystemInfo.State)
									 .FirstOrDefault(b => b.EndState.DefaultName == TargetEndStateName);
				if (branch != null)
					stateSvc.ChangeState(doc, branch);
			} else {
				for (int i = 0; i < 3; i++) {
					var branches = stateSvc
						.FindLineBranchesByStartState(doc.SystemInfo.State)
						.ToList();

					if (branches.Count == 0)
						break;

					var branch = branches[rnd.Next(branches.Count)];
					stateSvc.ChangeState(doc, branch);

					if (rnd.Next(0, 2) == 0)
						break;
				}
			}

			ctx.AcceptChanges();
			return doc.GetObjectId();
		}
	}
}
