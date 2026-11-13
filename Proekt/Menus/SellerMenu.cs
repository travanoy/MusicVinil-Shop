namespace Proekt.Menus;

using Proekt.Enums;
using Proekt.Menus;
using Proekt.Models;
using Proekt.Repositories.Interfaces;
using Proekt.rop;

public static class SellerMenu
{
    public static async Task ShowAsync(
        IEmployeeRepository empRepo,
        ICustomerRepository custRepo,
        IRecordRepository recRepo,
        ISaleRepository saleRepo,
        IPromotionRepository promoRepo,
        Employee seller)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== ПРОДАВЕЦ: {seller.FirstName} ===");
            Console.WriteLine("1. Продать пластинку");
            Console.WriteLine("2. Поиск пластинок");
            Console.WriteLine("3. Список пластинок");
            Console.WriteLine("4. Списать пластинку");
            Console.WriteLine("5. Отложить для покупателя");
            Console.WriteLine("6. Снять бронь");
            Console.WriteLine("7. Новинки");
            Console.WriteLine("0. Выход");
            Console.Write("Выбор: ");

            switch (Console.ReadLine())
            {
                case "1":
                    await AdminMenu.SellAsync(custRepo, recRepo, saleRepo, promoRepo, seller);
                    break;

                case "2":
                    await AdminMenu.SearchAsync(recRepo);
                    break;

                case "3":
                    var records = await recRepo.GetInStockAsync();
                    Console.WriteLine("\nId\tНазвание\tИсполнитель\tЖанр\tЦена");
                    Console.WriteLine("──────────────────────��─────────────────────────");
                    foreach (var r in records)
                        Console.WriteLine($"{r.Id}\t{r.Title}\t{r.Artist}\t{r.Genre}\t{r.SellingPrice}");
                    Console.ReadKey();
                    break;

                case "4":
                    Console.Write("Id пластинки: ");
                    int.TryParse(Console.ReadLine(), out var wId);
                    var wr = await recRepo.GetByIdAsync(wId);
                    if (wr != null && (wr.Status == RecordStatus.InStock || wr.Status == RecordStatus.Reserved))
                    {
                        wr.Status = RecordStatus.WrittenOff;
                        wr.ReservedForCustomerId = null;
                        await recRepo.UpdateAsync(wr);
                        Console.WriteLine("Списана!");
                    }
                    else Console.WriteLine("Не найдена или недоступна");
                    Console.ReadKey();
                    break;

                case "5":
                    Console.Write("Id пластинки: ");
                    int.TryParse(Console.ReadLine(), out var rId);
                    Console.Write("Id покупателя: ");
                    int.TryParse(Console.ReadLine(), out var cId);
                    var rec = await recRepo.GetByIdAsync(rId);
                    if (rec != null && rec.Status == RecordStatus.InStock)
                    {
                        rec.Status = RecordStatus.Reserved;
                        rec.ReservedForCustomerId = cId;
                        await recRepo.UpdateAsync(rec);
                        Console.WriteLine("Отложена!");
                    }
                    else Console.WriteLine("Недоступна");
                    Console.ReadKey();
                    break;

                case "6":
                    Console.Write("Id пластинки: ");
                    int.TryParse(Console.ReadLine(), out var crId);
                    var cr = await recRepo.GetByIdAsync(crId);
                    if (cr != null && cr.Status == RecordStatus.Reserved)
                    {
                        cr.Status = RecordStatus.InStock;
                        cr.ReservedForCustomerId = null;
                        await recRepo.UpdateAsync(cr);
                        Console.WriteLine("Бронь снята!");
                    }
                    else Console.WriteLine("Не забронирована");
                    Console.ReadKey();
                    break;

                case "7":
                    var news = await recRepo.GetNewArrivalsAsync(20);
                    Console.WriteLine("\nНовинки:");
                    foreach (var r in news)
                        Console.WriteLine($"\t[{r.Id}]\t{r.Title}\t{r.Artist}\t{r.DateAdded:d}");
                    Console.ReadKey();
                    break;

                case "0": return;
            }
        }
    }
}