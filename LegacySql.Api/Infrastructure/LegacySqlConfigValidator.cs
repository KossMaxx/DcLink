﻿using System;
 using FluentValidation;

 namespace LegacySql.Api.Infrastructure
{
    class LegacySqlConfigValidator : AbstractValidator<LegacySqlConfig>
    {
        public LegacySqlConfigValidator()
        {
            RuleFor(i => i.ConnectionStrings.LegacyDbContext).NotNull().NotEmpty();
            RuleFor(i => i.ConnectionStrings.AppDbContext).NotNull().NotEmpty();

            RuleFor(i => i.RabbitMq.HostAdress).NotNull().NotEmpty();
            RuleFor(i => i.RabbitMq.Username).NotNull().NotEmpty();
            RuleFor(i => i.RabbitMq.Password).NotNull().NotEmpty();
            
            RuleFor(i => i.LegacySqlFilters.NotFullMappingIdPortion).NotNull().Must(i => int.TryParse(i, out var result));
            RuleFor(i => i.LegacySqlFilters.ClientOrderPeriod).NotNull().Must(i => DateTime.TryParse(i, out var result));
        }
    }
}
