using Microsoft.EntityFrameworkCore;
using CRUDConsole;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory());
builder.AddJsonFile("appsetings.json");
var config = builder.Build();
string? connectionString = config.GetConnectionString("DefaultConnection");
var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
var options = optionsBuilder.UseSqlServer(connectionString).Options;

int id;

DataContext context = new(options);
await context.Database.EnsureCreatedAsync();
await context.DisposeAsync();

for (; ; )
{
    int action;
    Console.WriteLine("\tМЕНЮ\t");
    Console.WriteLine("1. Просмотреть товары");
    Console.WriteLine("2. Добавить товар");
    Console.WriteLine("3. Удалить товар");
    Console.WriteLine("4. Редактировать товар");
    Console.WriteLine("0. Выход");
    Console.WriteLine("Выберите действие");
    int.TryParse(Console.ReadLine(), out action);
    switch (action)
    {
        case 0:          
            return; 
        case 1:
            List<Good> goods = await GetGoodsAsync();
            foreach (Good g in goods)
            {
                Console.WriteLine($"{g.Id}\t{g.Name}\t{g.Price}\t{g.Count}");
            }
            break;
        case 2:
            string? name;
            decimal price;
            int count;
            Good good = new();
            Console.WriteLine("Введите наименование");
            name = Console.ReadLine();
            if (name != null && name.Length > 0) good.Name = name;
            else
            {
                Console.WriteLine("Не введено наименование");
                continue;
            }
            Console.WriteLine("Введите цену");
            if (decimal.TryParse(Console.ReadLine(), out price) && price > 0) good.Price = price;
            else
            {
                Console.WriteLine("Некорректно введена цена");
                continue;
            }
            Console.WriteLine("Введите количество");
            if (int.TryParse(Console.ReadLine(), out count) && count >= 0) good.Count = count;
            else
            {
                Console.WriteLine("Некорректно введено количество");
                continue;
            }
            if (good != null) await AddGoodAsync(good);            
            else Console.WriteLine("При добавлении товара произошла ошибка");
            break;
        case 3:            
            Console.WriteLine("Введите номер удаляемого товара");
            if (int.TryParse(Console.ReadLine(), out id) && id > 0)
                await RemoveGoodAsync(id);                
            else Console.WriteLine("Некорректно введен номер товара");
            break;
        case 4:
            Console.WriteLine("Введите номер редактируемого товара");
            if (int.TryParse(Console.ReadLine(), out id) && id > 0)
                await EditGoodAsync(id);            
            else Console.WriteLine("Некорректно введен номер товара");
            break;
        default: 
            Console.WriteLine("Некорректный выбор действия");
            break;

    }

}


async Task<List<Good>> GetGoodsAsync()
{
    using (DataContext context = new(options))
    {
        return await context.Goods.ToListAsync();
    }
}

async Task AddGoodAsync(Good good)
{
    using(DataContext context = new(options))
    {
        await context.Goods.AddAsync(good);
        await context.SaveChangesAsync();
        await Console.Out.WriteLineAsync("Товар успешно добавлен");
    }
}

async Task RemoveGoodAsync(int id)
{
    using(DataContext context = new(options))
    {
        Good? good = await context.Goods.FindAsync(id);
        if(good != null) context.Goods.Remove(good);
        await context.SaveChangesAsync();
        await Console.Out.WriteLineAsync("Товар успешно удален");
    } 
}

async Task EditGoodAsync(int id)
{
    using(DataContext context = new(options))
    {
        string? name;
        decimal price;
        int count;
        Good? good = await context.Goods.FindAsync(id);
        if(good != null)
        {
            await Console.Out.WriteLineAsync("Введите новое наименование");
            name = Console.ReadLine();
            if (name != null && name.Length > 0) good.Name = name;
            else {
                await Console.Out.WriteLineAsync("Некорректно введено наименование");
                return;
            }
            await Console.Out.WriteLineAsync("Введите новую цену");
            if (decimal.TryParse(Console.ReadLine(), out price) && price > 0) good.Price = price;
            else
            {
                await Console.Out.WriteLineAsync("Некорректно введена цена");
                return;
            };
            await Console.Out.WriteLineAsync("Введите новое количество");
            if (int.TryParse(Console.ReadLine(), out count) && count >= 0) good.Count = count;
            else
            {
                await Console.Out.WriteLineAsync("Некорректно введено количество");
                return;
            }
            await context.SaveChangesAsync();
            await Console.Out.WriteLineAsync("Товар успешно обновлен");
        }
        else await Console.Out.WriteLineAsync("Товар с указанным номером не найден");
    }     
}




