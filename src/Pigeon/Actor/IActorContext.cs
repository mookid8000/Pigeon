﻿using System;
using System.Collections.Generic;
namespace Akka.Actor
{
    public interface IActorContext : IActorRefFactory
    {        
        LocalActorRef Self { get; }
        Props Props { get; }
        void Become(Receive receive);
        void Unbecome();
        ActorRef Sender { get; }
        InternalActorRef Child(string name);
        IEnumerable<InternalActorRef> GetChildren();
        ActorSystem System { get; }
        InternalActorRef Parent { get; }
        void Watch(ActorRef subject);
        void Unwatch(ActorRef subject);

        /*
  def self: ActorRef
  def props: Props
  def receiveTimeout: Duration  def setReceiveTimeout(timeout: Duration): Unit
  def become(behavior: Actor.Receive, discardOld: Boolean = true): Unit
  def unbecome(): Unit
  def sender: ActorRef
  def children: immutable.Iterable[ActorRef]
  def child(name: String): Option[ActorRef]
  implicit def dispatcher: ExecutionContext
  implicit def system: ActorSystem
  def parent: ActorRef
  def watch(subject: ActorRef): ActorRef
  def unwatch(subject: ActorRef): ActorRef
         */

        void Stop(InternalActorRef child);
    }
}
