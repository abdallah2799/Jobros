using Core.Interfaces.IServices.IEmailServices;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services;

namespace Application.HealthChecks
{
    public class EmailHealthCheck : IHealthCheck
    {
        private readonly IEmailService _emailService;

        public EmailHealthCheck(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // SendGridEmailService has a client inside; 
                // we’ll perform a lightweight "ping" instead of sending real mail.
                var canReach = await _emailService.PingAsync();

                return canReach
                    ? HealthCheckResult.Healthy("Email service is reachable.")
                    : HealthCheckResult.Unhealthy("Email service is not responding.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Email service check failed.", ex);
            }
        }
    }
}
