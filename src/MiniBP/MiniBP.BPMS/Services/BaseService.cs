using Common;

namespace MiniBP.BPMS.Services
{
    public class BaseService
    {
        private readonly IUserContext userContext;
        public BaseService(IUserContext userContext)
        {
            this.userContext = userContext;
        }

    }
}