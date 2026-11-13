namespace Proekt.Menus;

using Proekt.Enums;
using Proekt.Models;
using Proekt.Repositories.Interfaces;
using Proekt.rop;

public static class MainMenu
{
    public static async Task ShowAsync(
        IEmployeeRepository empRepo,
        ICustomerRepository custRepo,
        IRecordRepository recRepo,
        ISaleRepository saleRepo,
        IPromotionRepository promoRepo)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========= МУЗЫКАЛЬНЫЙ МАГАЗИН =========");
            Console.WriteLine("1. Вход для сотрудников");
            Console.WriteLine("2. Вход для покупателей");
            Console.WriteLine("3. Регистрация покупателя");
            Console.WriteLine("0. Выход");
            Console.Write("Выбор: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Логин: ");
                    var eLogin = Console.ReadLine() ?? "";
                    Console.Write("Пароль: ");
                    var ePass = Console.ReadLine() ?? "";

                    var emp = await empRepo.GetByLoginAsync(eLogin);
                    if (emp == null || !Passwordcheck.Verify(ePass, emp.PasswordHash))
                    {
                        Console.WriteLine("Неверный логин или пароль");
                        Console.ReadKey();
                        break;
                    }

                    Console.WriteLine($"Добро пожаловать, {emp.FirstName} ({emp.Role})!");
                    Console.ReadKey();

                    if (emp.Role == EmployeeRole.Admin)
                        await AdminMenu.ShowAsync(empRepo, custRepo, recRepo, saleRepo, promoRepo, emp);
                    else
                        await SellerMenu.ShowAsync(empRepo, custRepo, recRepo, saleRepo, promoRepo, emp);
                    break;

                case "2":
                    Console.Write("Логин: ");
                    var cLogin = Console.ReadLine() ?? "";
                    Console.Write("Пароль: ");
                    var cPass = Console.ReadLine() ?? "";

                    var cust = await custRepo.GetByLoginAsync(cLogin);
                    if (cust == null || !Passwordcheck.Verify(cPass, cust.PasswordHash))
                    {
                        Console.WriteLine("Неверный логин или пароль");
                        Console.ReadKey();
                        break;
                    }

                    Console.WriteLine($"Добро пожаловать, {cust.FirstName}!");
                    Console.ReadKey();

                    await CustomerMenu.ShowAsync(empRepo, custRepo, recRepo, saleRepo, promoRepo, cust);
                    break;

                case "3":
                    Console.Write("Логин: ");
                    var rLogin = Console.ReadLine() ?? "";
                    if (await custRepo.ExistsAsync(rLogin))
                    {
                        Console.WriteLine("Логин уже занят");
                        Console.ReadKey();
                        break;
                    }

                    Console.Write("Пароль: ");
                    var rPass = Console.ReadLine() ?? "";
                    Console.Write("Имя: ");
                    var first = Console.ReadLine() ?? "";
                    Console.Write("Фамилия: ");
                    var last = Console.ReadLine() ?? "";
                    Console.Write("Телефон: ");
                    var phone = Console.ReadLine() ?? "";
                    Console.Write("Email: ");
                    var email = Console.ReadLine() ?? "";

                    await custRepo.AddAsync(new Customer
                    {
                        Login = rLogin,
                        PasswordHash = Passwordcheck.Hash(rPass),
                        FirstName = first, LastName = last,
                        
                    });

                    Console.WriteLine("Регистрация успешна!");
                    Console.ReadKey();
                    break;

                case "0": return;
            }
        }
    }
}