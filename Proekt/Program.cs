using Microsoft.EntityFrameworkCore;


using Proekt.Data;
using Proekt.Enums;
using Proekt.Menus;
using Proekt.Models;
using Proekt.Repositories.Interfaces;
using Proekt.Repositories.repos;
using Proekt.rop;


using var db = new MusicStoreDbContext();



IEmployeeRepository empRepo = new EmployeeRepository(db);
ICustomerRepository custRepo = new CustomerRepository(db);
IRecordRepository recRepo = new RecordRepository(db);
ISaleRepository saleRepo = new SaleRepository(db);
IPromotionRepository promoRepo = new PromotionRepository(db);


if (!await empRepo.ExistsAsync("admin"))
{
    
    await empRepo.AddAsync(new Employee
    {
        Login = "admin",
        PasswordHash = Passwordcheck.Hash("admin"),
        FirstName = "Администратор",
        LastName = "Главный",
        Role = EmployeeRole.Admin
    });
    Console.WriteLine("Создан админ: admin / admin");
    Console.ReadKey();
}

await MainMenu.ShowAsync(empRepo, custRepo, recRepo, saleRepo, promoRepo);