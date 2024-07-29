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
        private IDictionary<string, DataImport> _dataimport;
        private ODHApiWriter _dataWriteToODHApi;

        public Worker(IReadAccommodation readAccoMessage, ILogger<Worker> logger, WorkerSettings configuration, IDictionary<string, DataImport> dataimport, ODHApiWriter datawritetoapi)
        {
            _logger = logger;

            _readAccoMessage = readAccoMessage;            

            _configuration = configuration;

            _dataimport = dataimport;

            _dataWriteToODHApi = datawritetoapi;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Run the Read method
                await Task.Run(() => _readAccoMessage.Read(_configuration.RabbitConnectionString, _configuration.MongoDBConnectionString, new List<string>() { "lts.accommodationchanged", "lts.accommodationdetail", "lts.accommodationdetail_open" }, _dataimport, _dataWriteToODHApi));                //, 
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