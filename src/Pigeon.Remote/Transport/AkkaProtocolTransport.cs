﻿using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akka.Remote.Transport
{
    public class ProtocolTransportAddressPair
    {
        public ProtocolTransportAddressPair(AkkaProtocolTransport protocolTransport,Address address)
        {
            this.ProtocolTransport = protocolTransport;
            this.Address = address;
        }

        public AkkaProtocolTransport ProtocolTransport { get;private set; }

        public Address Address { get; private set; }
    }
    public class AkkaProtocolTransport
    {
        public Transport Transport { get; private set; }
        private ActorSystem actorSystem;
        private AkkaProtocolSettings akkaProtocolSettings;

        public AkkaProtocolTransport(Transport wrappedTransport, ActorSystem actorSystem, AkkaProtocolSettings akkaProtocolSettings)
        {
            this.Transport = wrappedTransport;
            this.actorSystem = actorSystem;
            this.akkaProtocolSettings = akkaProtocolSettings;
        }
    }
}
