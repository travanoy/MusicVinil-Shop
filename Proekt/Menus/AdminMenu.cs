namespace Proekt.Menus;

using Proekt.Enums;
using Proekt.Models;
using Proekt.Repositories.Interfaces;
using Proekt.rop;

public static class AdminMenu
{
    public static async Task ShowAsync(
        IEmployeeRepository empRepo,
        ICustomerRepository custRepo,
        IRecordRepository recRepo,
        ISaleRepository saleRepo,
        IPromotionRepository promoRepo,
        Employee admin)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== АДМИН: {admin.FirstName} ===");
            Console.WriteLine("1.  Добавить пластинку");
            Console.WriteLine("2.  Список пластинок");
            Console.WriteLine("3.  Редактировать пластинку");
            Console.WriteLine("4.  Удалить пластинку");
            Console.WriteLine("5.  Продать пластинку");
            Console.WriteLine("6.  Списать пластинку");
            Console.WriteLine("7.  Отложить для покупателя");
            Console.WriteLine("8.  Снять бронь");
            Console.WriteLine("9.  Поиск пластинок");
            Console.WriteLine("10. Создать акцию");
            Console.WriteLine("11. Список акций");
            Console.WriteLine("12. Статистика");
            Console.WriteLine("13. Добавить сотрудника");
            Console.WriteLine("14. Новинки");
            Console.WriteLine("0.  Выход");
            Console.Write("Выбор: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Название: ");
                    var title = Console.ReadLine() ?? "";
                    Console.Write("Исполнитель: ");
                    var artist = Console.ReadLine() ?? "";
                    Console.Write("Издатель: ");
                    var publisher = Console.ReadLine() ?? "";
                    Console.Write("Кол-во треков: ");
                    int.TryParse(Console.ReadLine(), out var tracks);
                    Console.Write($"Жанр ({string.Join(", ", Enum.GetNames<Genre>())}): ");
                    Enum.TryParse<Genre>(Console.ReadLine(), true, out var genre);
                    Console.Write("Год издания: ");
                    int.TryParse(Console.ReadLine(), out var year);
                    Console.Write("Себестоимость: ");
                    decimal.TryParse(Console.ReadLine(), out var cost);
                    Console.Write("Цена продажи: ");
                    decimal.TryParse(Console.ReadLine(), out var price);

                    await recRepo.AddAsync(new Record
                    {
                        Title = title, Artist = artist, Publisher = publisher,
                        TrackCount = tracks, Genre = genre, ReleaseYear = year,
                        CostPrice = cost, SellingPrice = price
                    });
                    Console.WriteLine("Пластинка добавлена!");
                    Console.ReadKey();
                    break;

                case "2":
                    var allRecs = await recRepo.GetAllAsync();
                    Console.WriteLine("\nId\tНазвание\tИсполнитель\tЖанр\tЦена\tСтатус");
                    Console.WriteLine("────────────────────────────────────────────────────────────");
                    foreach (var r in allRecs)
                        Console.WriteLine($"{r.Id}\t{r.Title}\t{r.Artist}\t{r.Genre}\t{r.SellingPrice}\t{r.Status}");
                    Console.WriteLine($"\nВсего: {allRecs.Count}");
                    Console.ReadKey();
                    break;

                case "3":
                    Console.Write("Id пластинки: ");
                    int.TryParse(Console.ReadLine(), out var editId);
                    var editRec = await recRepo.GetByIdAsync(editId);
                    if (editRec == null) { Console.WriteLine("Не найдена"); Console.ReadKey(); break; }

                    Console.WriteLine($"Текущее название: {editRec.Title}");
                    Console.Write("Новое (Enter — оставить): ");
                    var v = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(v)) editRec.Title = v;

                    Console.WriteLine($"Текущий исполнитель: {editRec.Artist}");
                    Console.Write("Новый (Enter — оставить): ");
                    v = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(v)) editRec.Artist = v;

                    Console.WriteLine($"Текущая цена: {editRec.SellingPrice}");
                    Console.Write("Новая (Enter — оставить): ");
                    v = Console.ReadLine();
                    if (decimal.TryParse(v, out var np)) editRec.SellingPrice = np;

                    await recRepo.UpdateAsync(editRec);
                    Console.WriteLine("Обновлено!");
                    Console.ReadKey();
                    break;

                case "4":
                    Console.Write("Id пластинки: ");
                    int.TryParse(Console.ReadLine(), out var delId);
                    await recRepo.DeleteAsync(delId);
                    Console.WriteLine("Удалена!");
                    Console.ReadKey();
                    break;

                case "5":
                    await SellAsync(custRepo, recRepo, saleRepo, promoRepo, admin);
                    break;

                case "6":
                    Console.Write("Id пластинки: ");
                    int.TryParse(Console.ReadLine(), out var woId);
                    var woRec = await recRepo.GetByIdAsync(woId);
                    if (woRec == null) { Console.WriteLine("Не найдена"); Console.ReadKey(); break; }
                    woRec.Status = RecordStatus.WrittenOff;
                    woRec.ReservedForCustomerId = null;
                    await recRepo.UpdateAsync(woRec);
                    Console.WriteLine("Списана!");
                    Console.ReadKey();
                    break;

                case "7":
                    Console.Write("Id пластинки: ");
                    int.TryParse(Console.ReadLine(), out var resRecId);
                    Console.Write("Id покупателя: ");
                    int.TryParse(Console.ReadLine(), out var resCustId);
                    var resRec = await recRepo.GetByIdAsync(resRecId);
                    if (resRec == null || resRec.Status != RecordStatus.InStock)
                    { Console.WriteLine("Недоступна"); Console.ReadKey(); break; }
                    resRec.Status = RecordStatus.Reserved;
                    resRec.ReservedForCustomerId = resCustId;
                    await recRepo.UpdateAsync(resRec);
                    Console.WriteLine("Отложена!");
                    Console.ReadKey();
                    break;

                case "8":
                    Console.Write("Id пластинки: ");
                    int.TryParse(Console.ReadLine(), out var crId);
                    var crRec = await recRepo.GetByIdAsync(crId);
                    if (crRec == null || crRec.Status != RecordStatus.Reserved)
                    { Console.WriteLine("Не забронирована"); Console.ReadKey(); break; }
                    crRec.Status = RecordStatus.InStock;
                    crRec.ReservedForCustomerId = null;
                    await recRepo.UpdateAsync(crRec);
                    Console.WriteLine("Бронь снята!");
                    Console.ReadKey();
                    break;

                case "9":
                    await SearchAsync(recRepo);
                    break;

                case "10":
                    await CreatePromotionAsync(promoRepo);
                    break;

                case "11":
                    var promos = await promoRepo.GetAllAsync();
                    foreach (var p in promos)
                    {
                        var st = p.IsActive ? "активна" : "неактивна";
                        Console.WriteLine($"\n[{p.Id}] {p.Name}\tскидка {p.DiscountPercent}%\t({st})");
                        Console.WriteLine($"\t{p.StartDate:d} — {p.EndDate:d}");
                        foreach (var c in p.Conditions)
                        {
                            var btw = c.Operator == ConditionOperator.Between ? $"..{c.ValueMax}" : "";
                            Console.WriteLine($"\tУсловие: {c.Field} {c.Operator} {c.Value}{btw}");
                        }
                    }
                    Console.ReadKey();
                    break;

                case "12":
                    await ShowStatisticsAsync(saleRepo);
                    break;

                case "13":
                    Console.Write("Логин: ");
                    var eLog = Console.ReadLine() ?? "";
                    if (await empRepo.ExistsAsync(eLog))
                    { Console.WriteLine("Логин занят"); Console.ReadKey(); break; }
                    Console.Write("Пароль: ");
                    var ePass = Console.ReadLine() ?? "";
                    Console.Write("Имя: ");
                    var eFirst = Console.ReadLine() ?? "";
                    Console.Write("Фамилия: ");
                    var eLast = Console.ReadLine() ?? "";
                    Console.Write("Роль (Admin/Seller): ");
                    Enum.TryParse<EmployeeRole>(Console.ReadLine(), true, out var role);

                    await empRepo.AddAsync(new Employee
                    {
                        Login = eLog, PasswordHash = Passwordcheck.Hash(ePass),
                        FirstName = eFirst, LastName = eLast, Role = role
                    });
                    Console.WriteLine("Сотрудник добавлен!");
                    Console.ReadKey();
                    break;

                case "14":
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

    public static async Task SellAsync(
        ICustomerRepository custRepo,
        IRecordRepository recRepo,
        ISaleRepository saleRepo,
        IPromotionRepository promoRepo,
        Employee seller)
    {
        Console.Write("Id пластинок через запятую (напр. 1,3,5): ");
        var ids = (Console.ReadLine() ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s.Trim(), out var i) ? i : 0)
            .Where(i => i > 0).ToList();

        var records = await recRepo.GetByIdsAsync(ids);
        if (!records.Any())
        { Console.WriteLine("Пластинки не найдены"); Console.ReadKey(); return; }

        Console.Write("Id покупателя (Enter — без карты): ");
        int.TryParse(Console.ReadLine(), out var custId);
        var customer = custId > 0 ? await custRepo.GetByIdAsync(custId) : null;
        decimal custDiscount = customer?.GetDiscountPercent() ?? 0;

        var promos = await promoRepo.GetActiveAsync();

        var sale = new Sale
        {
            EmployeeId = seller.Id,
            CustomerId = customer?.Id,
            DiscountPercent = custDiscount
        };

        decimal totalOrig = 0, totalFinal = 0;
        Console.WriteLine("\n--- ЧЕК ---");

        foreach (var rec in records)
        {
            var promo = PromotionCreater.GetBestDiscount(rec, promos);
            decimal promoDisc = promo?.percent ?? 0;
            decimal afterPromo = rec.SellingPrice * (1 - promoDisc / 100m);
            decimal final_ = Math.Round(afterPromo * (1 - custDiscount / 100m), 2);

            sale.Items.Add(new SaleItem
            {
                RecordId = rec.Id,
                UnitPrice = rec.SellingPrice,
                PromotionDiscount = promoDisc,
                AppliedPromotionName = promo?.name,
                FinalPrice = final_
            });

            rec.Status = RecordStatus.Sold;
            totalOrig += rec.SellingPrice;
            totalFinal += final_;

            Console.WriteLine($"\t{rec.Title}\t{rec.SellingPrice} руб.");
            if (promo != null)
                Console.WriteLine($"\t\tАкция: {promo.Value.name}\t-{promo.Value.percent}%");
        }

        sale.TotalAmount = totalOrig;
        sale.DiscountedAmount = totalFinal;

        if (customer != null)
        {
            customer.TotalSpent += totalFinal;
            await custRepo.UpdateAsync(customer);
        }

        await saleRepo.AddAsync(sale);
        await recRepo.SaveAsync();

        Console.WriteLine("---");
        if (custDiscount > 0) Console.WriteLine($"Скидка покупателя:\t{custDiscount}%");
        Console.WriteLine($"Итого:\t\t\t{totalOrig} руб.");
        Console.WriteLine($"К оплате:\t\t{totalFinal} руб.");
        Console.WriteLine("Продажа оформлена!");
        Console.ReadKey();
    }

    public static async Task SearchAsync(IRecordRepository recRepo)
    {
        Console.WriteLine("Поиск (Enter — пропустить):");
        Console.Write("Название: ");
        var title = Console.ReadLine();
        Console.Write("Исполнитель: ");
        var artist = Console.ReadLine();
        Console.Write("Жанр: ");
        Enum.TryParse<Genre>(Console.ReadLine(), true, out var genre);

        Genre? genreFilter = genre != default ? genre : null;
        var results = await recRepo.SearchAsync(title, artist, genreFilter);

        Console.WriteLine("\nId\tНазвание\tИсполнитель\tЖанр\tЦена");
        Console.WriteLine("────────────────────────────────────────────────");
        foreach (var r in results)
            Console.WriteLine($"{r.Id}\t{r.Title}\t{r.Artist}\t{r.Genre}\t{r.SellingPrice}");
        Console.WriteLine($"\nНайдено: {results.Count}");
        Console.ReadKey();
    }

    private static async Task CreatePromotionAsync(IPromotionRepository promoRepo)
    {
        Console.Write("Название акции: ");
        var name = Console.ReadLine() ?? "";
        Console.Write("Описание: ");
        var desc = Console.ReadLine() ?? "";
        Console.Write("Скидка (%): ");
        decimal.TryParse(Console.ReadLine(), out var disc);
        Console.Write("Дата начала (yyyy-MM-dd): ");
        DateTime.TryParse(Console.ReadLine(), out var start);
        Console.Write("Дата конца (yyyy-MM-dd): ");
        DateTime.TryParse(Console.ReadLine(), out var end);

        var promo = new Promotion
        {
            Name = name, Description = desc,
            DiscountPercent = disc, StartDate = start, EndDate = end
        };

        Console.WriteLine("\nДобавляйте условия (пустое поле — завершить):");
        while (true)
        {
            Console.Write($"Поле ({string.Join(", ", Enum.GetNames<ConditionField>())}): ");
            var fieldStr = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fieldStr)) break;
            if (!Enum.TryParse<ConditionField>(fieldStr, true, out var field)) continue;

            Console.Write($"Оператор ({string.Join(", ", Enum.GetNames<ConditionOperator>())}): ");
            if (!Enum.TryParse<ConditionOperator>(Console.ReadLine(), true, out var op)) continue;

            Console.Write("Значение: ");
            var val = Console.ReadLine() ?? "";

            string? valMax = null;
            if (op == ConditionOperator.Between)
            {
                Console.Write("Значение макс: ");
                valMax = Console.ReadLine();
            }

            promo.Conditions.Add(new PromotionCondition
            {
                Field = field, Operator = op, Value = val, ValueMax = valMax
            });
        }

        await promoRepo.AddAsync(promo);
        Console.WriteLine("Акция создана!");
        Console.ReadKey();
    }

    private static async Task ShowStatisticsAsync(ISaleRepository saleRepo)
    {
        Console.Write("Период (day/week/month/year): ");
        var period = Console.ReadLine() ?? "month";

        var now = DateTime.UtcNow;
        var from = period.ToLower() switch
        {
            "day" => now.Date,
            "week" => now.AddDays(-7),
            "month" => now.AddMonths(-1),
            "year" => now.AddYears(-1),
            _ => now.AddMonths(-1)
        };

        var items = await saleRepo.GetSaleItemsInPeriodAsync(from, now);

        Console.WriteLine("\nТоп пластинок:");
        var topRecs = items.GroupBy(si => new { si.Record.Title, si.Record.Artist })
            .Select(g => new { g.Key.Title, g.Key.Artist, Count = g.Count() })
            .OrderByDescending(x => x.Count).Take(10);
        foreach (var r in topRecs)
            Console.WriteLine($"\t{r.Title}\t{r.Artist}\t{r.Count} продаж");

        Console.WriteLine("\nТоп исполнителей:");
        var topArt = items.GroupBy(si => si.Record.Artist)
            .Select(g => new { Artist = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count).Take(10);
        foreach (var a in topArt)
            Console.WriteLine($"\t{a.Artist}\t{a.Count} продаж");

        Console.WriteLine("\nТоп жанров:");
        var topGen = items.GroupBy(si => si.Record.Genre)
            .Select(g => new { Genre = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count).Take(10);
        foreach (var g in topGen)
            Console.WriteLine($"\t{g.Genre}\t{g.Count} продаж");

        Console.ReadKey();
    }
}