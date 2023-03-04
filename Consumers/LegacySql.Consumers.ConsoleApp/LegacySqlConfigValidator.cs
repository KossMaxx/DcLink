﻿using System;
 using FluentValidation;

 namespace LegacySql.Consumers.ConsoleApp
{
    class LegacySqlConfigValidator : AbstractValidator<LegacySqlConfig>
    {
        public LegacySqlConfigValidator()
        {
            RuleFor(i => i.ConnectionStrings.LegacyDbContext).NotNull().NotEmpty();
            RuleFor(i => i.ConnectionStrings.AppDbContext).NotNull().NotEmpty();
            RuleFor(i => i.RabbitMq.HostAddress).NotNull().NotEmpty();
            RuleFor(i => i.RabbitMq.Username).NotNull().NotEmpty();
            RuleFor(i => i.RabbitMq.Password).NotNull().NotEmpty();
        }
    }
}
