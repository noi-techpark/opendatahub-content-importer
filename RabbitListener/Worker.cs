// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitListener
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IReadMessage _readMessage;
        private readonly WorkerSettings _configuration;        

        public Worker(IReadMessage readMessage, ILogger<Worker> logger, WorkerSettings configuration)
        {
            _logger = logger;
            _readMessage = readMessage;
            _configuration = configuration;            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Run the Read method
                await Task.Run(() => _readMessage.Read(_configuration.RabbitConnectionString, _configuration.ReadQueue, _configuration.MongoDBConnectionString));
            }

            //sync
            //stoppingToken.ThrowIfCancellationRequested();

            //_readMessage.Read(_configuration.RabbitConnectionString, _configuration.ReadQueue, _configuration.MongoDBConnectionString);

            //return Task.CompletedTask;
        }
    }

    public class WorkerSettings
    {
        public string? RabbitConnectionString { get; set; }
        public string? MongoDBConnectionString { get; set; }
        public string? ReadQueue { get; set; }
    }
}