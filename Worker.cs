using Application.Domain;
using EchoService.Configuration;
using FunctionExtensions.Result;
using Majordomo;
using Microsoft.Extensions.Options;
using NetMQ;
using Error = Application.Domain.Error;

namespace EchoService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly WorkerConfig _workerConfig;

        public Worker(ILogger<Worker> logger, IOptionsMonitor<WorkerConfig> config)
        {
            _logger = logger;
            _workerConfig = config.CurrentValue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                Thread workerThread = new Thread(async () => { await Start(stoppingToken); });

                workerThread.Start();

                stoppingToken.Register(() =>
                {
                    workerThread.Join();
                    _logger.LogInformation($"Worker service token is canceled");
                });
            });
           

        }
        private async Task Start(CancellationToken stoppingToken)
        {
              var id = new[] { (byte)_workerConfig.WorkerIdPrefix, (byte)_workerConfig.WorkerNumber };
                try
                {
                    // create worker offering the service 'echo'
                    using var session = new MDPWorker($"tcp://{_workerConfig.BrokerAddress}:{_workerConfig.BrokerPort}", _workerConfig.ServiceName, id);
                    session.HeartbeatDelay = TimeSpan.FromMilliseconds(2500);
                // logging info to be displayed on screen
                _logger.LogInformation($"{_workerConfig.ServiceName} {_workerConfig.WorkerIdPrefix} {_workerConfig.WorkerNumber} is started");
                session.LogInfoReady += (s, e) => _logger.LogDebug("{0}", e.Info);
                    // there is no initial reply
                    NetMQMessage reply = null;


                    while (!stoppingToken.IsCancellationRequested)
                    {
                        // send the reply and wait for a request
                        var request = await session.TryReceive(reply, stoppingToken);
                        _logger.LogInformation("Received and Reply: {0}", request?.ToString());

                        // was the worker interrupted
                        if (request is null)
                            break;
                    // echo the request b 
                    Error error = Errors.General.InternalServerError("exception");

                    error.Serialize();

                    reply = request;

                    }
                    session.Stop();
                }
                catch (Exception ex)
                {
                    _logger.LogError("ERROR:");
                    _logger.LogError($"{ex.Message}");
                    _logger.LogError($"{ex.StackTrace}");
                }

            }
        }
}