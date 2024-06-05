// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.Extensions.Configuration;
using RabbitPusher;


Console.WriteLine("Press ESC to stop");
var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
.AddUserSecrets<Program>();
//.AddEnvironmentVariables();
IConfiguration config = builder.Build();

do
{
    while (!Console.KeyAvailable)
    {
        string operation = Console.ReadLine();

        if (operation.StartsWith("send"))
        {
            RabbitMQSend rabbitsend = new RabbitMQSend(config.GetConnectionString("RabbitConnection"));

            rabbitsend.Send("rudolf/testet", new { Name = "Test", Property = "xy", Source = "TestSource" });
        }
        else
            Console.WriteLine("no operation!");
    }
} while (Console.ReadKey(true).Key != ConsoleKey.Escape);