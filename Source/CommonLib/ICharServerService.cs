using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ICharServerService
    {
        int OnlinePlayers { [OperationContract] get; [OperationContract] set; }

        [OperationContract(IsInitiating = true)]
        bool RegisterServer(string name, IPAddress publicAddress, int publicPort);

        [OperationContract(IsTerminating = true)]
        void Disconnect();
    }
}
