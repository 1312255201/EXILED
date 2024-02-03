// -----------------------------------------------------------------------
// <copyright file="GameEntity.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Core.Interfaces;
    using UnityEngine;

    /// <summary>
    /// The base class which defines in-game entities.
    /// </summary>
    public abstract class GameEntity : TypeCastObject<GameEntity>, IEntity
    {
        private readonly HashSet<EActor> componentsInChildren = new();

        /// <inheritdoc/>
        public IReadOnlyCollection<EActor> ComponentsInChildren => componentsInChildren;

        /// <summary>
        /// Gets or sets the <see cref="GameEntity"/>'s <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public virtual GameObject GameObject { get; protected set; }

        /// <inheritdoc/>
        public T AddComponent<T>(string name = "")
            where T : EActor
        {
            T component = EObject.CreateDefaultSubobject<T>(GameObject);

            if (!component)
                return null;

            componentsInChildren.Add(component);
            return component;
        }

        /// <inheritdoc/>
        public EActor AddComponent(Type type, string name = "")
        {
            EActor component = EObject.CreateDefaultSubobject(type, GameObject).Cast<EActor>();

            if (!component)
                return null;

            componentsInChildren.Add(component);
            return component;
        }

        /// <inheritdoc/>
        public T AddComponent<T>(EActor actor, string name = "")
            where T : EActor
        {
            if (!actor)
                throw new NullReferenceException("The provided EActor is null.");

            if (!string.IsNullOrEmpty(name))
                actor.Name = name;

            EActor.AttachTo(GameObject, actor);
            componentsInChildren.Add(actor);

            return actor.Cast(out T param) ? param : throw new InvalidCastException("The provided EActor cannot be cast to the specified type.");
        }

        /// <inheritdoc/>
        public T AddComponent<T>(Type type, string name = "")
            where T : EActor
        {
            T component = EObject.CreateDefaultSubobject<T>(type, GameObject, string.IsNullOrEmpty(name) ? $"{type.Name}-Component#{ComponentsInChildren.Count}" : name).Cast<T>();
            if (!component)
                return null;

            componentsInChildren.Add(component);

            return component.Cast(out T param) ? param : throw new InvalidCastException("The provided EActor cannot be cast to the specified type.");
        }

        /// <inheritdoc/>
        public EActor AddComponent(EActor actor, string name = "")
        {
            if (!actor)
                throw new NullReferenceException("The provided EActor is null.");

            if (!string.IsNullOrEmpty(name))
                actor.Name = name;

            EActor.AttachTo(GameObject, actor);
            componentsInChildren.Add(actor);

            return actor;
        }

        /// <inheritdoc/>
        public IEnumerable<EActor> AddComponents(IEnumerable<Type> types)
        {
            foreach (Type t in types)
                yield return AddComponent(t);
        }

        /// <inheritdoc/>
        public IEnumerable<EActor> AddComponents(IEnumerable<EActor> actors)
        {
            foreach (EActor actor in actors)
                yield return AddComponent(actor);
        }

        /// <inheritdoc/>
        public IEnumerable<T> AddComponents<T>(IEnumerable<T> actors)
            where T : EActor
        {
            foreach (T actor in actors)
                yield return AddComponent<T>(actor);
        }

        /// <inheritdoc/>
        public IEnumerable<T> AddComponents<T>(IEnumerable<EActor> types)
            where T : EActor
        {
            foreach (EActor type in types)
                yield return AddComponent<T>(type);
        }

                /// <inheritdoc />
        public T RemoveComponent<T>(string name = "")
            where T : EActor
        {
            T comp = null;

            if (string.IsNullOrEmpty(name))
            {
                if (!TryGetComponent<T>(out comp))
                    return null;

                comp.Base = null;
                componentsInChildren.Remove(comp);
                return comp;
            }

            foreach (EActor actor in GetComponents<T>())
            {
                if (actor.Name != name)
                    continue;

                comp = actor.Cast<T>();
            }

            return comp;
        }

        /// <inheritdoc />
        public T RemoveComponent<T>(EActor actor, string name = "")
            where T : EActor
        {
            T comp = null;

            if (string.IsNullOrEmpty(name))
            {
                if (!TryGetComponent<T>(out comp) || comp != actor)
                    return null;

                comp.Base = null;
                componentsInChildren.Remove(comp);
                return comp;
            }

            foreach (EActor component in GetComponents<T>())
            {
                if (component.Name != name && component == actor)
                    continue;

                comp = component.Cast<T>();
            }

            return comp;
        }

        /// <inheritdoc />
        public EActor RemoveComponent(Type type, string name = "")
        {
            EActor comp = null;

            if (string.IsNullOrEmpty(name))
            {
                if (!TryGetComponent(type, out comp))
                    return null;

                comp.Base = null;
                componentsInChildren.Remove(comp);
                return comp;
            }

            foreach (EActor actor in GetComponents(type))
            {
                if (actor.Name != name)
                    continue;

                comp = actor;
            }

            return comp;
        }

        /// <inheritdoc />
        public EActor RemoveComponent(EActor actor, string name = "")
        {
            if (!componentsInChildren.Contains(actor))
                return null;

            if (string.IsNullOrEmpty(name))
            {
                actor.Base = null;
                componentsInChildren.Remove(actor);
                return actor;
            }

            foreach (EActor component in componentsInChildren)
            {
                if (component != actor || actor.Name != name)
                    continue;

                actor = component;
            }

            return actor;
        }

        /// <inheritdoc />
        public IEnumerable<T> RemoveComponentOfType<T>(string name = "")
            where T : EActor
        {
            IEnumerable<T> components = GetComponents<T>();

            foreach (T comp in components)
                RemoveComponent(comp, name);

            return components;
        }

        /// <inheritdoc />
        public IEnumerable<EActor> RemoveComponentOfType(Type type, string name = "")
        {
            IEnumerable<EActor> components = GetComponents(type);

            foreach (EActor comp in components)
                RemoveComponent(comp, name);

            return components;
        }

        /// <inheritdoc />
        public void RemoveComponents(IEnumerable<Type> types) => types.ForEach(type => RemoveComponent(type));

        /// <inheritdoc />
        public void RemoveComponents(IEnumerable<EActor> actors) => actors.ForEach(actor => RemoveComponent(actor));

        /// <inheritdoc />
        public void RemoveComponents<T>(IEnumerable<T> actors)
            where T : EActor => actors.ForEach(actor => RemoveComponent(actor));

        /// <inheritdoc />
        public void RemoveComponents<T>(IEnumerable<EActor> types)
            where T : EActor => types.ForEach(type => RemoveComponent(type));

        /// <inheritdoc/>
        public T GetComponent<T>()
            where T : EActor => componentsInChildren.FirstOrDefault(comp => typeof(T) == comp.GetType()).Cast<T>();

        /// <inheritdoc/>
        public T GetComponent<T>(Type type)
            where T : EActor => componentsInChildren.FirstOrDefault(comp => type == comp.GetType()).Cast<T>();

        /// <inheritdoc/>
        public EActor GetComponent(Type type) => componentsInChildren.FirstOrDefault(comp => type == comp.GetType());

        /// <inheritdoc/>
        public IEnumerable<T> GetComponents<T>() => componentsInChildren.Where(comp => typeof(T).IsAssignableFrom(comp.GetType())).Cast<T>();

        /// <inheritdoc/>
        public IEnumerable<T> GetComponents<T>(Type type) => componentsInChildren.Where(comp => typeof(T).IsAssignableFrom(comp.GetType())).Cast<T>();

        /// <inheritdoc/>
        public IEnumerable<EActor> GetComponents(Type type) => componentsInChildren.Where(comp => type.IsAssignableFrom(comp.GetType()));

        /// <inheritdoc/>
        public bool TryGetComponent<T>(out T component)
            where T : EActor => component = GetComponent<T>();

        /// <inheritdoc/>
        public bool TryGetComponent(Type type, out EActor component) => component = GetComponent(type);

        /// <inheritdoc/>
        public bool TryGetComponent<T>(Type type, out T component)
            where T : EActor => component = GetComponent<T>(type);

        /// <inheritdoc/>
        public bool HasComponent<T>(bool depthInheritance = false) => depthInheritance
            ? componentsInChildren.Any(comp => typeof(T).IsAssignableFrom(comp.GetType()))
            : componentsInChildren.Any(comp => typeof(T) == comp.GetType());

        /// <inheritdoc/>
        public bool HasComponent(Type type, bool depthInheritance = false) => depthInheritance
            ? componentsInChildren.Any(comp => type.IsAssignableFrom(comp.GetType()))
            : componentsInChildren.Any(comp => type == comp.GetType());
    }
}