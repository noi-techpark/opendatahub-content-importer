// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
        private readonly IReadAccommodationChanged _readAccoChangedMessage;
        private readonly IReadAccommodationDetail _readAccoDetailMessage;
        private readonly WorkerSettings _configuration;

        public Worker(IReadAccommodationChanged readaccoChangedMessage, IReadAccommodationDetail readaccoDetailMessage, ILogger<Worker> logger, WorkerSettings configuration)
        {
            _logger = logger;
            _readAccoChangedMessage = readaccoChangedMessage;
            _readAccoDetailMessage = readaccoDetailMessage;

            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Run the Read method
                await Task.Run(() => _readAccoChangedMessage.Read(_configuration.RabbitConnectionString, "lts.accommodationchanged", _configuration.MongoDBConnectionString));

                await Task.Run(() => _readAccoDetailMessage.Read(_configuration.RabbitConnectionString, "lts.accommodationdetail", _configuration.MongoDBConnectionString));
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