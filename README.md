# Camunda External Task Client Dotnet
Just Another External Task Client: Execute ServiceTasks with C#

```C#
using System;
using Camunda;

class ExampleApp
{
    static void Main()
    {
        var task = new TopicSubscriptionManager("http://localhost:8080/engine-rest")
            .AddHandler("charge-card", new MyHandler())
            .StartTask();

        Console.ReadKey();
    }
}

class MyHandler : IHandler {

    public void Action(ExternalTask task, ExternalTaskService externalTaskService) {

        externalTaskService.complete(task);
        Console.WriteLine($"External Task {task} successfully completed!");
    }

}
```
