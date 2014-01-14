﻿using System;
using System.Diagnostics;
using System.Linq.Expressions;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Building;
using StructureMap.Building.Interception;

namespace StructureMap.Testing.Building.Interception
{
    [TestFixture]
    public class ActivatorInterceptorTester
    {
        private ActivatorInterceptor<ITarget> theActivator;

        [SetUp]
        public void SetUp()
        {
            theActivator = new ActivatorInterceptor<ITarget>(x => x.Activate());
        }

        [Test]
        public void the_description()
        {
            theActivator.Description.ShouldEqual("ITarget.Activate()");
        }

        [Test]
        public void the_role_is_activates()
        {
            theActivator.Role.ShouldEqual(InterceptorRole.Activates);
        }

        [Test]
        public void the_accepts_type()
        {
            theActivator.Accepts.ShouldEqual(typeof (ITarget));
        }
        [Test]
        public void the_return_type()
        {
            theActivator.Returns.ShouldEqual(typeof (ITarget));
        }

        [Test]
        public void create_the_expression_when_the_variable_is_the_right_type()
        {
            var variable = Expression.Variable(typeof (ITarget), "target");

            var expression = theActivator.ToExpression(Parameters.Session, variable);

            expression.ToString().ShouldEqual("target.Activate()");
        }

        [Test]
        public void compile_and_use_by_itself()
        {
            var variable = Expression.Variable(typeof(ITarget), "target");

            var expression = theActivator.ToExpression(Parameters.Session, variable);

            var lambdaType = typeof (Action<ITarget>);
            var lambda = Expression.Lambda(lambdaType, expression, variable);

            var action = lambda.Compile().As<Action<ITarget>>();

            var target = new Target();
            action(target);

            target.HasBeenActivated.ShouldBeTrue();
        }

    }

    public interface ITarget
    {
        void Activate();
    }

    public class Target : ITarget
    {
        public bool HasBeenActivated;

        void ITarget.Activate()
        {
            HasBeenActivated = true;
        }

        public void TurnGreen()
        {
            Color = "Green";
        }

        public string Color = "Red";

        public void ThrowUp()
        {
            throw new DivideByZeroException("you stink!");
        }
    }
}