namespace Proekt.Menus;

using Proekt.Enums;
using Proekt.Models;
using Proekt.Repositories.Interfaces;
using Proekt.rop;

public static class CustomerMenu
{
    public static async Task ShowAsync(
        IEmployeeRepository empRepo,
        ICustomerRepository custRepo,
        IRecordRepository recRepo,
        ISaleRepository saleRepo,
        IPromotionRepository promoRepo,
        Customer customer)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== ПОКУПАТЕЛЬ: {customer.FirstName} ===");
            Console.WriteLine($"Скидка: {customer.GetDiscountPercent()}%\tПотрачено: {customer.TotalSpent} руб.");
            Console.WriteLine();
            Console.WriteLine("1. Каталог пластинок");
            Console.WriteLine("2. Купить");
            Console.WriteLine("3. Поиск");
            Console.WriteLine("4. Новинки");
            Console.WriteLine("5. Мои покупки");
            Console.WriteLine("6. Мои отложенные");
            Console.WriteLine("7. Моя скидка");
            Console.WriteLine("0. Выход");
            Console.Write("Выбор: ");

            switch (Console.ReadLine())
            {
                case "1":
                    var all = await recRepo.GetInStockAsync();
                    Console.WriteLine("\nId\tНазвание\tИсполнитель\tЖанр\tЦена");
                    Console.WriteLine("────────────────────────────────────────────────");
                    foreach (var r in all)
                        Console.WriteLine($"{r.Id}\t{r.Title}\t{r.Artist}\t{r.Genre}\t{r.SellingPrice}");
                    Console.ReadKey();
                    break;

                case "2":
                    await BuyAsync(empRepo, custRepo, recRepo, saleRepo, promoRepo, customer);
                    break;

                case "3":
                    await AdminMenu.SearchAsync(recRepo);
                    break;

                case "4":
                    var news = await recRepo.GetNewArrivalsAsync(20);
                    Console.WriteLine("\nНовинки:");
                    foreach (var r in news)
                        Console.WriteLine($"\t[{r.Id}]\t{r.Title}\t{r.Artist}\t{r.SellingPrice} руб.");
                    Console.ReadKey();
                    break;

                case "5":
                    var sales = await saleRepo.GetByCustomerAsync(customer.Id);
                    if (!sales.Any())
                    { Console.WriteLine("Покупок пока нет"); Console.ReadKey(); break; }

                    foreach (var s in sales)
                    {
                        Console.WriteLine($"\n--- Чек #{s.Id}\t{s.SaleDate:g} ---");
                        foreach (var item in s.Items)
                        {
                            Console.WriteLine($"\t{item.Record.Title}\t{item.UnitPrice} -> {item.FinalPrice} руб.");
                            if (item.AppliedPromotionName != null)
                                Console.WriteLine($"\t\tАкция: {item.AppliedPromotionName}\t-{item.PromotionDiscount}%");
                        }
                        Console.WriteLine($"\tИтого:\t{s.DiscountedAmount} руб.");
                    }
                    Console.ReadKey();
                    break;

                case "6":
                    var reserved = await recRepo.GetReservedByCustomerAsync(customer.Id);
                    if (!reserved.Any())
                    { Console.WriteLine("Нет отложенных"); Console.ReadKey(); break; }
                    Console.WriteLine("\nОтложенные:");
                    foreach (var r in reserved)
                        Console.WriteLine($"\t[{r.Id}]\t{r.Title}\t{r.Artist}\t{r.SellingPrice} руб.");
                    Console.ReadKey();
                    break;

                case "7":
                    Console.WriteLine($"\nВаша карта лояльности:");
                    Console.WriteLine($"\tПотрачено:\t{customer.TotalSpent} руб.");
                    Console.WriteLine($"\tСкидка:\t\t{customer.GetDiscountPercent()}%");
                    Console.WriteLine();
                    Console.WriteLine("\tШкала скидок:");
                    Console.WriteLine("\tот  1 000 руб.\t3%");
                    Console.WriteLine("\tот  5 000 руб.\t5%");
                    Console.WriteLine("\tот 10 000 руб.\t7%");
                    Console.WriteLine("\tот 20 000 руб.\t10%");
                    Console.WriteLine("\tот 50 000 руб.\t15%");
                    Console.ReadKey();
                    break;

                case "0": return;
            }
        }
    }

    private static async Task BuyAsync(
        IEmployeeRepository empRepo,
        ICustomerRepository custRepo,
        IRecordRepository recRepo,
        ISaleRepository saleRepo,
        IPromotionRepository promoRepo,
        Customer customer)
    {
        var inStock = await recRepo.GetInStockAsync();
        if (!inStock.Any())
        { Console.WriteLine("Нет пластинок в наличии"); Console.ReadKey(); return; }

        Console.WriteLine("\nДоступные пластинки:");
        Console.WriteLine("Id\tНазвание\tИсполнитель\tЦена");
        Console.WriteLine("────────────────────────────────────────────────");
        foreach (var r in inStock)
            Console.WriteLine($"{r.Id}\t{r.Title}\t{r.Artist}\t{r.SellingPrice} руб.");

        Console.Write("\nId пластинок через запятую (напр. 1,3,5): ");
        var ids = (Console.ReadLine() ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s.Trim(), out var i) ? i : 0)
            .Where(i => i > 0).ToList();

        var records = await recRepo.GetByIdsAsync(ids);
        if (!records.Any())
        { Console.WriteLine("Пластинки не найдены"); Console.ReadKey(); return; }

        var promos = await promoRepo.GetActiveAsync();
        decimal custDiscount = customer.GetDiscountPercent();

        var seller = await empRepo.GetFirstAsync();
        if (seller == null)
        { Console.WriteLine("Ошибка: нет сотрудников"); Console.ReadKey(); return; }

        decimal totalOrig = 0, totalFinal = 0;
        var saleItems = new List<SaleItem>();

        Console.WriteLine("\n--- Ваш заказ ---");

        foreach (var rec in records)
        {
            var promo = PromotionCreater.GetBestDiscount(rec, promos);
            decimal promoDisc = promo?.percent ?? 0;
            decimal afterPromo = rec.SellingPrice * (1 - promoDisc / 100m);
            decimal final_ = Math.Round(afterPromo * (1 - custDiscount / 100m), 2);

            saleItems.Add(new SaleItem
            {
                RecordId = rec.Id,
                UnitPrice = rec.SellingPrice,
                PromotionDiscount = promoDisc,
                AppliedPromotionName = promo?.name,
                FinalPrice = final_
            });

            totalOrig += rec.SellingPrice;
            totalFinal += final_;

            Console.WriteLine($"\t{rec.Title}\t{rec.SellingPrice} руб.");
            if (promo != null)
                Console.WriteLine($"\t\tАкция: {promo.Value.name}\t-{promo.Value.percent}%");
        }

        Console.WriteLine("---");
        if (custDiscount > 0) Console.WriteLine($"\tВаша скидка:\t{custDiscount}%");
        Console.WriteLine($"\tИтого:\t\t{totalOrig} руб.");
        Console.WriteLine($"\tК оплате:\t{totalFinal} руб.");

        Console.Write("\nОформить покупку? (да/нет): ");
        if ((Console.ReadLine() ?? "").ToLower() != "да")
        { Console.WriteLine("Покупка отменена"); Console.ReadKey(); return; }

        var sale = new Sale
        {
            EmployeeId = seller.Id,
            CustomerId = customer.Id,
            DiscountPercent = custDiscount,
            TotalAmount = totalOrig,
            DiscountedAmount = totalFinal
        };

        foreach (var item in saleItems) sale.Items.Add(item);
        foreach (var rec in records) rec.Status = RecordStatus.Sold;

        customer.TotalSpent += totalFinal;

        await saleRepo.AddAsync(sale);
        await custRepo.UpdateAsync(customer);
        await recRepo.SaveAsync();

        Console.WriteLine("\nПокупка оформлена!");
        Console.WriteLine($"Чек #{sale.Id}");
        Console.WriteLine($"Новая сумма покупок: {customer.TotalSpent} руб.");
        Console.WriteLine($"Ваша скидка теперь: {customer.GetDiscountPercent()}%");
        Console.ReadKey();
    }
}