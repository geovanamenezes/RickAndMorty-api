using Serilog.Context;

namespace Correlation.Services
{
    public interface ICorrelationService
    {
        void SetCorrelationId(Guid processId);
        Guid? GetCorrelationId();
    }
    public class CorrelationService : ICorrelationService
    {
        private static readonly AsyncLocal<Guid?> _correlationId = new();

        public void SetCorrelationId(Guid processId)
        {
            _correlationId.Value = processId;
            LogContext.PushProperty("CorrelationId", processId);
        }

        public Guid? GetCorrelationId()
        {
            return _correlationId.Value;
        }
    }
}
