namespace RapidFTP.Chilkat.Tests.Fixture
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public sealed class OneTimeFixture<TFixture> 
        where TFixture : class, new()
    {
        public OneTimeFixture()
        {
            object fixture;
            if (!OneTimeFixtureStorage.Fixtures.TryGetValue(typeof(TFixture), out fixture))
            {
                fixture = new TFixture();
                OneTimeFixtureStorage.Fixtures.Add(typeof(TFixture), fixture);

                var disposable = fixture as IDisposable;
                if (disposable != null)
                {
                    AppDomain.CurrentDomain.DomainUnload += (sender, args) => disposable.Dispose();
                }
            }

            this.Current = (TFixture)fixture;
        }

        public TFixture Current { get; private set; }
    }

    [SuppressMessage(
        "StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal class OneTimeFixtureStorage
    {
        internal static readonly Dictionary<Type, object> Fixtures;

        static OneTimeFixtureStorage()
        {
            Fixtures = new Dictionary<Type, object>();
        }
    }
}