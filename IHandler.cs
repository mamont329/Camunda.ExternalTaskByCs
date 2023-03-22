namespace Camunda
{
    public interface IHandler
    {
        
        void Action(ExternalTask task, ExternalTaskService service);

    }
}