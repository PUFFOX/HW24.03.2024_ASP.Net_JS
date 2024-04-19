using System.Text.RegularExpressions;
using WebApplication2;

List<Item> items = new List<Item>()
{
    new Item() { Id = 1, Name = "Процесор (CPU):", Description = "Intel Core i3", Price = 150},
    new Item() { Id = 2, Name = "Материнська плата (Motherboard)", Description = "Високий клас", Price = 300},
    new Item() { Id = 3, Name = "Оперативна пам'ять (RAM)", Description = "32GB DDR4", Price = 200},
    new Item() { Id = 4, Name = "Графічна карта (GPU)", Description = "NVIDIA RTX 3070", Price = 500},
    new Item() { Id = 5, Name = "Накопичувачі (HDD або SSD)", Description = "SSD 1TB", Price = 500},
    new Item() { Id = 6, Name = "Живлення (Power Supply)", Description = "Високий клас", Price = 150},
    new Item() { Id = 7, Name = "Корпус (Case)", Description = "NVIDIA RTX 3070", Price = 100}

};


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    var path = context.Request.Path;

    string expression = "^/api/items/([0-9]+)$";

    if (path == "/api/items")
    {
        if (request.Method == "GET")
        {
            await response.WriteAsJsonAsync(items);
        }
        else if (request.Method == "POST")
        {
            try
            {
                var item = await request.ReadFromJsonAsync<Item>();
                if (item != null && item.Id != null && item.Name != null && item.Description != null && item.Price > 0)
                {
                    items.Add(item);
                    await response.WriteAsJsonAsync(item);
                }
                else
                {
                    response.StatusCode = 400;
                    await response.WriteAsJsonAsync(new { message = "Invalid item data" });
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = $"Failed to process request: {ex.Message}" });
            }
        }
        else
        {
            response.StatusCode = 405; // Method Not Allowed
            await response.WriteAsJsonAsync(new { message = "Method Not Allowed" });
        }
    }
    else if (Regex.IsMatch(path, expression) && request.Method == "GET")
    {
        int? id = int.Parse(path.Value?.Split("/")[3]);
        Item? item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            response.StatusCode = 404;
            await response.WriteAsJsonAsync(new { message = "Item not found" });
        }
        else
        {
            await response.WriteAsJsonAsync(item);
        }
    }
    else if ((path == "/api/items") && (request.Method == "PUT"))
    {
        try
        {
            var item = await request.ReadFromJsonAsync<Item>();
            if (item != null)
            {
                var oldItem = items.FirstOrDefault((i => i.Id == item.Id));
                if (oldItem != null)
                {
                    oldItem.Name = item.Name;
                    oldItem.Description = item.Description;
                    oldItem.Price = item.Price;
                    await response.WriteAsJsonAsync(oldItem);
                }
                else
                {
                    throw new Exception("Not correct data");
                }

            }
            else
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Item wasn't found" });
            }

        }
        catch (Exception ex)
        {

            response.StatusCode = 400;
            await response.WriteAsJsonAsync(new { message = $"This is not belongs Item class" });
        }
    }
    else if ((Regex.IsMatch(path, expression)) && (request.Method == "DELETE"))
    {

        int? id = int.Parse(path.Value?.Split("/")[3]);
        Item? item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            response.StatusCode = 404;
            await response.WriteAsJsonAsync(new { message = "Item wasn't found" });
        }
        else
        {
            items.Remove(item);
            //await response.WriteAsJsonAsync(new { message = $"Item {item.Id} was delete" });
            await response.WriteAsJsonAsync(item);
        }
    }
    else
    {
        response.ContentType = "text/html; charset=uft-8";
        await response.SendFileAsync("html/index.html");        
    }
});



app.Run();