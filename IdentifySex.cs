using System;
namespace InclinationSexUtils
{
	class Program
	{
		public static bool IsMale(string midname)
		{
			return midname.EndsWith("ич") || midname.EndsWith("лы");
		}
		
		public static bool IsFemale(string midname)
		{
			return midname.EndsWith("на") || midname.EndsWith("зы") || midname.EndsWith("ва");
		}
		
		// Проверяет является ли фамилия мужской
		public static bool IsMaleSurname(string surname)
		{
			return surname.EndsWith("ов") || surname.EndsWith("ев") || surname.EndsWith("ий") || surname.EndsWith("ин");
		}
		
		// Проверяет является ли фамилия женской
		public static bool IsFemaleSurname(string surname)
		{
			return surname.EndsWith("ова") || surname.EndsWith("ева") || surname.EndsWith("ина") || surname.EndsWith("ая");
		}
		
		// Определение пола по имени
		public static string IdentifySexByName(string name)
		{
			if (String.IsNullOrEmpty(name)) 
				throw new ArgumentNullException("Не указано имя");
			
			if (name.EndsWith("а") || name.EndsWith("я") || name.EndsWith("е")) {
				return "female";
			} else {
				return "male";
			}
		}
		
		// Определение пола по фамилии
		public static string IdentifySexBySurname(string surname)
		{
			if (String.IsNullOrEmpty(surname)) 
				throw new ArgumentNullException("Пустая строка");
			
			if (IsMaleSurname(surname)) {
				return "male";
			} else if (IsFemaleSurname(surname)) {
				return "female";
			} else {
				return "unknown";
			} 
		}
		
		// Определение пола по отчеству
		public static string IdentifySexByMidname(string midname)
		{
			if (String.IsNullOrEmpty(midname)) 
				throw new ArgumentNullException("Пустая строка");
			
			if (IsMale(midname)) {
				return "male";
			} else if (IsFemale(midname)) {
				return "female";
			} else {
				return "unknown";
			}
		}
		
		
		public static void Main(string[] args)
		{
			Console.WriteLine(IdentifySexByMidname("Борисовна"));
			Console.WriteLine(IdentifySexByMidname("Петрович"));
			Console.WriteLine(IdentifySexByMidname("Сергеевна"));
			Console.WriteLine(IdentifySexByMidname("Юрьевна"));
			Console.WriteLine(IdentifySexByMidname("Викторовна"));
			Console.WriteLine(IdentifySexByMidname("Ильич"));
			Console.WriteLine(IdentifySexByMidname("Анатольевна"));
			Console.ReadKey(true);
		}
	}
}
