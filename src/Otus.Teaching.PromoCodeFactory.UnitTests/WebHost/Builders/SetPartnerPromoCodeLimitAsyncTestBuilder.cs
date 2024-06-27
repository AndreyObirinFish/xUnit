using AutoFixture;
using AutoFixture.AutoMoq;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Otus.Teaching.PromoCodeFactory.UnitTests.WebHost.Builders
{
    internal class SetPartnerPromoCodeLimitAsyncTestBuilder
    {
        private readonly IFixture _fixture;
        private readonly Random _random;

        public SetPartnerPromoCodeLimitAsyncTestBuilder()
        {
            _random = new Random();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        public Guid GetGuid()
        {
            return _fixture.Create<Guid>();
        }

        public SetPartnerPromoCodeLimitRequest GetRequest()
        {
            return _fixture.Build<SetPartnerPromoCodeLimitRequest>()
                           .With(x => x.Limit, _random.Next(1, 500))
                           .Create();
        }

        public SetPartnerPromoCodeLimitRequest GetRequestWithWrongLimit()
        {
            var request = GetRequest();
            request.Limit = _random.Next(-50, 0);

            return request;
        }

        public Partner GetPartner()
        {
            return _fixture.Build<Partner>()
                           .Without(x => x.PartnerLimits)
                           .With(x => x.NumberIssuedPromoCodes, _random.Next(1, 50))
                           .Create();
        }

        public Partner GetPartnerWithLimit(bool isExpiredLimits)
        {
            var partner = GetPartner();
            var limits = GetActiveLimits(isExpiredLimits);

            limits.ForEach(x =>
            {
                x.Partner = partner;
                x.PartnerId = partner.Id;
            });

            partner.PartnerLimits = limits;
            return partner;
        }

        public List<PartnerPromoCodeLimit> GetActiveLimits(bool isExpiredLimits)
        {
            var result = _fixture.Build<PartnerPromoCodeLimit>()
                                 .Without(x => x.Partner)
                                 .CreateMany(1)
                                 .ToList();
            if (!isExpiredLimits)
                result.ForEach(x => x.CancelDate = null);

            return result;
        }
    }
}


