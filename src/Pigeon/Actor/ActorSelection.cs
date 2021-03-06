﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Actor namespace.
/// </summary>
namespace Akka.Actor
{
    /// <summary>
    /// Class ActorSelection.
    /// </summary>
    public class ActorSelection : ICanTell
    {
        /// <summary>
        /// Gets the anchor.
        /// </summary>
        /// <value>The anchor.</value>
        public ActorRef Anchor { get; private set; }
        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public SelectionPathElement[] Elements { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorSelection"/> class.
        /// </summary>
        public ActorSelection() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ActorSelection"/> class.
        /// </summary>
        /// <param name="anchor">The anchor.</param>
        /// <param name="path">The path.</param>
        public ActorSelection(ActorRef anchor,SelectionPathElement[] path)
        {
            this.Anchor = anchor;
            this.Elements = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorSelection"/> class.
        /// </summary>
        /// <param name="anchor">The anchor.</param>
        /// <param name="path">The path.</param>
        public ActorSelection(ActorRef anchor, string path) : this(anchor,path == "" ? new string[]{} : path.Split('/'))
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorSelection"/> class.
        /// </summary>
        /// <param name="anchor">The anchor.</param>
        /// <param name="elements">The elements.</param>
        public ActorSelection(ActorRef anchor, IEnumerable<string> elements)
        {
            Anchor = anchor;
            Elements = elements.Select<string, SelectionPathElement>(e =>
            {
                if (e == "..")
                    return new SelectParent();
                else if (e.Contains("?") || e.Contains("*"))
                    return new SelectChildPattern(e);
                return new SelectChildName(e);
            }).ToArray();
        }

        /// <summary>
        /// Tells the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Tell(object message)
        {
            var sender = ActorRef.NoSender;
            if (ActorCell.Current != null && ActorCell.Current.Self != null)
                sender = ActorCell.Current.Self;

            Deliver(message, sender, 0, Anchor);
        }
        /// <summary>
        /// Tells the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        public void Tell(object message, ActorRef sender)
        {
            Deliver(message, sender, 0, Anchor);
        }

        /// <summary>
        /// Delivers the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="pathIndex">Index of the path.</param>
        /// <param name="current">The current.</param>
        private void Deliver(object message, ActorRef sender,int pathIndex,ActorRef current)
        {
            if (pathIndex == Elements.Length)
            {
                current.Tell(message, sender);
            }
            else
            {
                var element = this.Elements[pathIndex];
                if (current is ActorRefWithCell)
                {
                    var withCell = (ActorRefWithCell)current;
                    if (element is SelectParent)
                        Deliver(message, sender, pathIndex + 1, withCell.Parent);
                    else if (element is SelectChildName)
                        Deliver(message, sender, pathIndex + 1, withCell.GetSingleChild(element.AsInstanceOf<SelectChildName>().Name));
                    else
                    {
                        //pattern, ignore for now
                    }
                }
                else
                {
                    var rest = Elements.Skip(pathIndex).ToArray();
                    current.Tell(new ActorSelectionMessage(message,rest),sender);
                }
            }
        }
    }

    /// <summary>
    /// Class ActorSelectionMessage.
    /// </summary>
    [ProtoContract]
    public class ActorSelectionMessage : AutoReceivedMessage
    {
        public ActorSelectionMessage()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ActorSelectionMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="elements">The elements.</param>
        public ActorSelectionMessage(object message,SelectionPathElement[] elements)
        {
            this.Message = message;
            this.Elements = elements;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        [ProtoMember(1,DynamicType=true)]
        public object Message { get; set; }

        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        /// <value>The elements.</value>
        [ProtoMember(2)]
        public SelectionPathElement[] Elements { get; set; }
    }

    /// <summary>
    /// Class SelectionPathElement.
    /// </summary>
    [ProtoContract]
    [ProtoInclude(6,typeof(SelectChildName))]
    [ProtoInclude(7,typeof(SelectChildPattern))]
    [ProtoInclude(8,typeof(SelectParent))]
    public abstract class SelectionPathElement
    {

    }

    /// <summary>
    /// Class SelectChildName.
    /// </summary>
    [ProtoContract]
    public class SelectChildName : SelectionPathElement
    {
        public SelectChildName()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectChildName"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SelectChildName(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Class Pattern.
    /// </summary>
    [ProtoContract]
    public class Pattern
    {

    }

    /// <summary>
    /// Class Helpers.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Makes the pattern.
        /// </summary>
        /// <param name="patternStr">The pattern string.</param>
        /// <returns>Pattern.</returns>
        public static Pattern MakePattern(string patternStr)
        {
            return new Pattern();
        }
    }
    /// <summary>
    /// Class SelectChildPattern.
    /// </summary>
    [ProtoContract]
    public class SelectChildPattern : SelectionPathElement
    {
        public SelectChildPattern()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectChildPattern"/> class.
        /// </summary>
        /// <param name="patternStr">The pattern string.</param>
        public SelectChildPattern(string patternStr)
        {
            this.Pattern = Helpers.MakePattern(patternStr);
        }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>The pattern.</value>
        [ProtoMember(1)]
        public Pattern Pattern { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Pattern.ToString();
        }
    }


    /// <summary>
    /// Class SelectParent.
    /// </summary>
    [ProtoContract]
    public class SelectParent : SelectionPathElement
    {
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "..";
        }
    }
}
