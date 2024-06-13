// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataImportHelper;
using Helper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AccommodationTransformer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;        
        private readonly IReadAccommodation _readAccoMessage;
        private readonly WorkerSettings _configuration;
        private DataImport _dataimport;

        public Worker(IReadAccommodation readAccoMessage, ILogger<Worker> logger, WorkerSettings configuration, DataImport dataimport)
        {
            _logger = logger;
            _readAccoMessage = readAccoMessage;            

            _configuration = configuration;

            _dataimport = dataimport;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Run the Read method
                await Task.Run(() => _readAccoMessage.Read(_configuration.RabbitConnectionString, _configuration.MongoDBConnectionString, new List<string>() { "lts.accommodationchanged", "lts.accommodationdetail" }, _dataimport));                //, 
            }
        }
    }

    public class WorkerSettings
    {
        public string? RabbitConnectionString { get; set; }
        public string? MongoDBConnectionString { get; set; }
        public string? ReadQueue { get; set; }
    }
}