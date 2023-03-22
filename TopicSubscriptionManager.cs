using System.Collections.Generic;
using System.Threading;
using Refit;
using System.Threading.Tasks;

namespace Camunda
{
    class TopicSubscriptionManager
    {
        public TopicSubscriptionManager(string url)
        {
            _restService = RestService.For<IRequestExecutor>($"{url}/external-task");
        }

        private readonly Dictionary<string, IHandler> _handlers = new();
        private readonly IRequestExecutor _restService;
        private readonly ExternalTaskService _externalTaskService = new();
        private readonly FetchAndLockRequestDto _payload = new() {
            maxTasks = 500,
            workerId = "bar",
            topics = new List<TopicRequestDto>()
        };

        public TopicSubscriptionManager AddHandler(string topicName, IHandler handler)
        {
            _handlers.Add(topicName, handler);
            return this;
        }

        public Task StartTask()
        {
            return new TaskFactory().StartNew(Loop);
        }


        private async Task Loop() {

            while (true) {

                var topics = _payload.topics;

                topics.Clear();

                foreach(var entry in _handlers)
                {
                    topics.Add(new TopicRequestDto
                    {
                        topicName = entry.Key,
                        lockDuration = 20_000
                    });
                }

                var tasks = _restService.FetchAndLock(_payload);

                foreach (var task in tasks.Result)
                {
                    if (_handlers.TryGetValue(task.TopicName, out var handler)) {
                        handler.Action(task, _externalTaskService);
                    }
                }

                await Task.Delay(1000);
            }
        }
    }
}