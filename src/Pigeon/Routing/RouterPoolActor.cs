﻿using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Akka.Routing
{
    public class RouterPoolActor : RouterActor
    {
        private SupervisorStrategy supervisorStrategy;
        public RouterPoolActor(SupervisorStrategy supervisorStrategy) 
        {
            this.supervisorStrategy = supervisorStrategy;
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return this.supervisorStrategy;
        }
    }
}
