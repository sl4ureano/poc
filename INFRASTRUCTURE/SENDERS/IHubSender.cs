using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Consumer
{
    public interface IHubSender
    {
        void publishMSG(object msg);
    }
}