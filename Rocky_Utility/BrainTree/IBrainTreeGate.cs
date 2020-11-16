using Braintree;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocky_Utility.BrainTree
{
    public interface IBrainTreeGate
    {
        IBraintreeGateway CreateGateway();

        IBraintreeGateway GetGateway();
    }
}
